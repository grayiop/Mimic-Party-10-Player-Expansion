# v0.2.10 compatibility candidate

This local candidate targets the captured Windows Steam build **26008035**. It contains Expansion **1.1.6** and requires the companion Interop Bootstrap **1.0.4** candidate. Core **1.0.0** and the original BepInEx **6.0.0-be.788** loader remain dependencies.

## Why two components need changes

Pack 1.0.3 rejects the new GameAssembly/metadata fingerprints. Without its compatibility repair, generated `UnityEngine.CoreModule.dll` contains duplicate `<>O`/`<>c` types. The loader reports a missing CoreModule and stops before loading any plugin. After fixing that, Expansion 1.1.5 would select obsolete native capacity signatures because it does not recognize the new game hash.

The candidate adds exact fingerprints to both components. The bootstrap validates the newly captured metadata layout before renaming three empty helper types. The Expansion reuses the unchanged, unique native capacity signature. No native game binary is edited on disk.

## Install the local update

1. Close Mimic Party. Keep Pack 1.0.3 and Core 1.0.0 installed.
2. Back up `BepInEx/patchers/MimicParty.InteropBootstrap.dll` and `BepInEx/plugins/MimicParty10PlayerExpansion.dll` outside the game directory.
3. Extract `MimicParty_v0.2.10_Local_Compatibility_Update.zip` into the directory containing `Mimic Party.exe`, replacing those two files.
4. Launch normally through Steam. The bootstrap preserves its own backup of generated CoreModule before repairing it.
5. Check `BepInEx/LogOutput.log` for `INTEROP COMPATIBILITY READY [Mimic Party v0.2.10`, all `HOOK INSTALLED` entries, and `CAPACITY SELF-CHECK: mode0=10, mode1=10, mode2=10, expected=10` at the default setting.

Then check Classic/Versus capacity displays, joining beyond the stock player limit, voice isolation, and rematches. Startup alone does not verify full ten-client gameplay. To revert, close the game and restore the two saved DLLs; returning to the old files does not add support for v0.2.10.

## Rebuild the companion bootstrap

Use Pack source revision `354557816373b1771184b19439ae4e715391586b` from `https://github.com/LocoPablito/BepInEx-Pack-for-Mimic-Party`. In that checkout:

```powershell
git apply --check C:\path\to\Expansion\compatibility\v0.2.10\interop-bootstrap.patch
git apply C:\path\to\Expansion\compatibility\v0.2.10\interop-bootstrap.patch
./build.ps1
```

The resulting file is `src/MimicParty.InteropBootstrap/bin/Release/net6.0/MimicParty.InteropBootstrap.dll`. Build the Expansion with this repository's `build.ps1`; its normal packaging command creates an Expansion-only ZIP. The local combined update additionally contains the companion bootstrap, its modified source, and original notices. It is a personal test artifact, not an upstream release.

## Validation evidence and limits

- Both projects compiled targeting .NET 6 with SDK **9.0.317**, with zero warnings/errors. CI normally uses SDK 8.
- Existing native signature: one executable-section match at RVA **0x1FBF570**; patch start **0x1FBF575**. All four legacy signatures had zero matches.
- Required generated hook signatures remain present; `RoundPhase.Playback` remains **4**.
- CoreModule layout: **4021 TypeDefs, 22542 methods, 14641 fields**. The previous v0.2.5 layout correctly rejects it.
- Offline repair changed only bytes at file offsets **0xFA0B6**, **0xFABF6**, and **0xFAE36**, representing name indexes in TypeDef rows **0x324**, **0x3C4**, and **0x3E4**. A second repair made no changes.
- Original CoreModule SHA-256: `bffc34ad452cbc39850e1939b8205c7ce505d50bdfc6efc885205111e5bc0224`.
- Repaired CoreModule SHA-256: `884424c74bbcaa65f9db6bd4845ee3b58a482801ab9864248c613721a1af07f2`.
- A separate .NET 6 process reproduced the original duplicate-type load failure and successfully loaded `UnityEngine.Object`, `UnityEngine.Application`, and `UnityEngine.LogType` after repair.
- All eight hook methods also resolved through .NET reflection against the local generated assemblies after the repair.

**Live acceptance remains pending.** The offline checks did not launch the game or install the candidate. Game binaries and generated interop files are excluded from the update archive.
