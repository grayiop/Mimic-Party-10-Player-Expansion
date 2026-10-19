using System.Runtime.InteropServices;
using Arribbaa.MimicParty.ModdingCore;
using Arribbaa.MimicParty.ModdingCore.Native;
using BepInEx.Logging;

namespace Arribbaa.MimicParty.TenPlayerExpansion;

internal static class NativeCapacityPatches
{
    private const string DynamicCapacityBuildV0173Sha256 =
        "44bbc82bdae73c1c86559a1f091ee9c7a3ae510a02c2d83f16b686ecdd9c8b11";

    private const string DynamicCapacityBuildV023Sha256 =
        "adc318d8ad108a2eac4e130421d20c21aef840d8ec44203fba667d6eef08e199";

    private const string DynamicCapacityBuildV0233Sha256 =
        "03757842d82c83534a686b0acbf247c9a5b76d0a15c74c7cb27458e731d4b9d4";

    private const string DynamicCapacityBuildV025Sha256 =
        "02546fc8797c32a9f98a8b93a7eefc11f52fb58e83fd51db29fd83eb5601d25a";

    private const string DynamicCapacityBuildV0210Sha256 =
        "35bf5cc8a954a827c3c20fe76bd9d92191eed9dc83140101231081ba018a15f2";

    private const string DynamicCapacityMetadataV0210Sha256 =
        "7db463145f922d7a83c0c29879f5467bd40f82562ea549701f71221f6a35aaea";

    private const string DynamicCapacityHelper =
        "33 C0 83 F9 01 0F 95 C0 83 C0 04 C3";

    private const string DynamicPatchName = "Shared Classic/Versus capacity helper";

    private const string HostCapacityGate =
        "48 8B 43 40 48 85 C0 0F 84 ?? ?? ?? ?? 83 78 18 05 " +
        "0F 8D ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 83 B9 E4 00 00 00 00";

    private const string ConnectStatusMax =
        "48 8D 54 24 30 48 8B D8 C7 44 24 30 05 00 00 00 " +
        "E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 45 33 C9 4C 8B C0 48 8B D3";

    private const string FullRoomMax =
        "48 8D 54 24 30 48 8B D8 C7 44 24 30 05 00 00 00 " +
        "E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 45 33 C0 48 8B D0";

    private const string RoomListMax =
        "48 8D 4C 24 60 48 89 44 24 38 0F 57 C0 8B 44 24 5C 4C 8B CD " +
        "44 88 64 24 30 4D 8B C5 C7 44 24 28 05 00 00 00 48 8B D7 89 44 24 20";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CapacityHelperProbe(int mode);

    public static PatchTransaction Create(int desiredMaxPlayers)
    {
        if (desiredMaxPlayers is < 6 or > 10)
            throw new ArgumentOutOfRangeException(nameof(desiredMaxPlayers), "Supported player count is 6-10.");

        byte replacement = checked((byte)desiredMaxPlayers);

        if (IsDynamicCapacityBuild())
        {
            return CoreApi.CreatePatchTransaction(PluginConstants.Guid)
                .Add(
                    DynamicPatchName,
                    DynamicCapacityHelper,
                    patchOffset: 5,
                    expected: new byte[] { 0x0F, 0x95, 0xC0, 0x83, 0xC0, 0x04, 0xC3 },
                    replacement: new byte[] { 0xB8, replacement, 0x00, 0x00, 0x00, 0xC3, 0x90 });
        }

        return CoreApi.CreatePatchTransaction(PluginConstants.Guid)
            .Add(
                "Host connection capacity",
                HostCapacityGate,
                patchOffset: 16,
                expected: new byte[] { 0x05 },
                replacement: new byte[] { replacement })
            .Add(
                "Connect status maximum",
                ConnectStatusMax,
                patchOffset: 12,
                expected: new byte[] { 0x05 },
                replacement: new byte[] { replacement })
            .Add(
                "Full-room maximum",
                FullRoomMax,
                patchOffset: 12,
                expected: new byte[] { 0x05 },
                replacement: new byte[] { replacement })
            .Add(
                "Room-list maximum",
                RoomListMax,
                patchOffset: 32,
                expected: new byte[] { 0x05 },
                replacement: new byte[] { replacement });
    }

    public static void VerifyRuntime(
        int desiredMaxPlayers,
        IReadOnlyList<RuntimePatchHandle> handles,
        ManualLogSource log)
    {
        ArgumentNullException.ThrowIfNull(handles);
        ArgumentNullException.ThrowIfNull(log);

        if (!IsDynamicCapacityBuild())
        {
            log.LogInfo("Capacity self-check: legacy build uses transactional byte verification; runtime helper probe not required.");
            return;
        }

        RuntimePatchHandle handle = handles.Single(h => h.Name == DynamicPatchName);
        IntPtr helperAddress = IntPtr.Subtract(handle.Address, 5);
        CapacityHelperProbe probe =
            Marshal.GetDelegateForFunctionPointer<CapacityHelperProbe>(helperAddress);

        int mode0 = probe(0);
        int mode1 = probe(1);
        int mode2 = probe(2);

        log.LogInfo(
            $"CAPACITY SELF-CHECK: mode0={mode0}, mode1={mode1}, mode2={mode2}, expected={desiredMaxPlayers}.");

        if (mode0 != desiredMaxPlayers ||
            mode1 != desiredMaxPlayers ||
            mode2 != desiredMaxPlayers)
        {
            throw new InvalidOperationException(
                $"Capacity self-check failed. Expected {desiredMaxPlayers}; " +
                $"got mode0={mode0}, mode1={mode1}, mode2={mode2}.");
        }

        log.LogInfo(
            $"CAPACITY SELF-CHECK PASS: the verified shared helper returns {desiredMaxPlayers} " +
            "for all tested mode selectors. No additional players are required for this verification.");
    }

    private static bool IsDynamicCapacityBuild()
    {
        string hash = CoreApi.Build.GameAssemblySha256;
        return string.Equals(hash, DynamicCapacityBuildV0173Sha256, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(hash, DynamicCapacityBuildV023Sha256, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(hash, DynamicCapacityBuildV0233Sha256, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(hash, DynamicCapacityBuildV025Sha256, StringComparison.OrdinalIgnoreCase) ||
               (string.Equals(hash, DynamicCapacityBuildV0210Sha256, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(CoreApi.Build.MetadataSha256, DynamicCapacityMetadataV0210Sha256,
                    StringComparison.OrdinalIgnoreCase));
    }
}
