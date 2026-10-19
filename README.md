# 10 Player Expansion for Mimic Party

**Runtime 1.1.6 compatibility candidate · original mod by arribbaa**

This checkout adds a **local test candidate for Mimic Party v0.2.10 / Steam build 26008035**. It requires the companion **Interop Bootstrap 1.0.4 candidate**; Pack 1.0.3 alone fails before plugins load. See [v0.2.10 setup and validation](compatibility/v0.2.10/README.md). Builds and offline checks pass; live gameplay acceptance is pending. Published 1.1.5 downloads below retain their documented v0.2.5 support.

Up to **10 total players** in private Classic and Versus lobbies. Versus keeps the game's RED/BLUE selection: the mod does not impose equal teams or a separate five-player quota per side.

[Download on Nexus](https://www.nexusmods.com/mimicparty/mods/1) · [GitHub downloads](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/releases/latest) · [Report an issue](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/issues)

## Requirements

- [BepInEx Pack for Mimic Party](https://www.nexusmods.com/mimicparty/mods/3) **1.0.3**
- For v0.2.10: the local **Interop Bootstrap 1.0.4 candidate** described above, installed over Pack 1.0.3.
- [Mimic Party Modding Core](https://www.nexusmods.com/mimicparty/mods/2) **1.0.0**
- Windows x64 / Steam / IL2CPP build listed in [Compatibility](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/blob/main/COMPATIBILITY.md)

The main ZIP contains the Expansion only. Install both requirements separately.

## Installation / update

1. Close Mimic Party.
2. Install/update BepInEx Pack 1.0.3 in the folder containing `Mimic Party.exe`.
3. Install Core 1.0.0 into the same folder.
4. Extract this Expansion ZIP there; it adds/replaces `BepInEx/plugins/MimicParty10PlayerExpansion.dll`.
5. Launch normally through Steam and allow interop generation to finish when required.

## Features and settings

Classic and Versus share the configured **total** capacity, default 10. After a successful start, close the game and edit:

`BepInEx/config/com.arribbaa.mimicparty.10playerexpansion.cfg`

```ini
[Multiplayer]
MaxPlayers = 10
```

Allowed range: **6–10**. Values outside it are clamped.

Runtime 1.1.5 explicitly recognizes the captured v0.2.5 / Steam build 25629135 GameAssembly fingerprint. The same shared Classic/Versus capacity helper remains present exactly once with stock behavior Classic 5 / Versus 4:

`33 C0 83 F9 01 0F 95 C0 83 C0 04 C3`

On v0.2.5 the helper is located at RVA `0x1F8D910`; the actual runtime patch begins at RVA `0x1F8D915`. The patch remains signature-based, fail-closed to explicitly supported builds, verifies the expected bytes transactionally and performs its live 10/10/10 helper self-check before declaring the Expansion active.

The captured v0.2.5 interop assemblies retain all required reflected target names for max-player, playback voice-isolation and rematch behavior. Public room-browser values are still not spoofed by directly editing remote `RoomListing` data.

**v0.2.5 status:** live verified on Steam build 25629135. All required reflected targets resolved, every Harmony hook installed, the shared native capacity patch applied at RVA `0x1F8D915`, and the runtime helper self-check returned **10 / 10 / 10**.

## v0.2.5 live acceptance

A real Windows v0.2.5 session confirmed:

- Pack 1.0.3 completed the interop compatibility repair and the IL2CPP chainloader initialized.
- Modding Core 1.0.0 loaded and reported the expected v0.2.5 fingerprints.
- Every required Expansion hook target resolved.
- Every Harmony hook installed.
- Shared native capacity patch applied at RVA `0x1F8D915`.
- Capacity self-check returned **10 / 10 / 10**.
- Expansion declared itself active for 10 total Classic/Versus players.

## v0.2.33 historical live acceptance

A real Windows v0.2.33 session confirmed:

- Bootstrap/Core loaded successfully.
- Every required Expansion hook target resolved.
- Every Harmony hook installed.
- Shared native capacity patch applied.
- Capacity self-check returned **10 / 10 / 10**.
- Classic mode card displayed **Up to 10 players**.
- Versus mode card displayed **Up to 10 players**.
- Classic private lobby displayed **1/10**.

## Removal and troubleshooting

Close the game and remove only `BepInEx/plugins/MimicParty10PlayerExpansion.dll`. Keep Core/Pack if other mods use them. Native capacity changes are in memory and disappear when the process exits.

Unrecognized game version or loader failure: do not force the patch; check `BepInEx/LogOutput.log`.

[Compatibility](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/blob/main/COMPATIBILITY.md) · [Changelog](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/blob/main/CHANGELOG.md) · [Build instructions](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/blob/main/BUILDING.md) · [License](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/blob/main/LICENSE.txt) · [Security](https://github.com/LocoPablito/Mimic-Party-10-Player-Expansion/blob/main/SECURITY.md)
