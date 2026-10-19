# Compatibility

## Mimic Party v0.2.10 — captured 19 October 2026 (candidate)

- Steam build **26008035**, Windows x64 / Steam / IL2CPP, Unity **6000.4.2f1**.
- Expansion **1.1.6 candidate**, Core **1.0.0**, Pack **1.0.3** with the companion **Interop Bootstrap 1.0.4 candidate**.
- GameAssembly SHA-256: `35bf5cc8a954a827c3c20fe76bd9d92191eed9dc83140101231081ba018a15f2`.
- Metadata SHA-256: `7db463145f922d7a83c0c29879f5467bd40f82562ea549701f71221f6a35aaea`.

Executable-section scanning found exactly one existing shared capacity signature at RVA `0x1FBF570`; the patch starts at `0x1FBF575`. None of the four legacy signatures matched. The new profile requires both captured hashes and retains the existing transactional checks and live capacity probe. Required generated hook signatures remain present, including `LaunchRematch(INetworkService)` and `RoundPhase.Playback = 4`.

Pack 1.0.3 rejects this fingerprint. Its uncorrected generated CoreModule contains duplicate empty helper types and cannot load in .NET 6. The companion bootstrap profile validates **4021 TypeDefs / 22542 methods / 14641 fields**, then repairs name indexes at TypeDef rows `0x324`, `0x3C4`, and `0x3E4`.

Offline checks reproduced the original load failure, rejected the older layout, verified exactly three changed bytes, confirmed repair idempotence, and loaded `UnityEngine.Object`, `UnityEngine.Application`, and `UnityEngine.LogType` from the repaired assembly. Original game installation files were not changed during these checks.

**Live acceptance is pending.** Successful startup, in-process capacity self-check, lobby displays, ten-client play, voice isolation, and rematches still need in-game validation. See [candidate instructions and bootstrap source patch](compatibility/v0.2.10/README.md).

## Mimic Party v0.2.5 — captured 30 September 2026

- Steam build **25629135**
- Windows x64 / Steam / IL2CPP
- Unity **6000.4.2f1**
- BepInEx **6.0.0-be.788**
- BepInEx Pack **1.0.3**
- Modding Core **1.0.0**
- Expansion **1.1.5**

GameAssembly.dll SHA-256:
`02546fc8797c32a9f98a8b93a7eefc11f52fb58e83fd51db29fd83eb5601d25a`

Metadata SHA-256:
`dded356e7a47bc941b63e4a09280bd7fb8e777bf5748366d47345363df9a9bbf`

Static executable-section scanning found exactly one copy of the shared Classic/Versus capacity helper signature:

`33 C0 83 F9 01 0F 95 C0 83 C0 04 C3`

Its stock semantics remain Classic 5 / Versus 4.

Helper RVA:
`0x1F8D910`

Runtime patch start:
`0x1F8D915`

The four legacy capacity patterns were not found, so v0.2.5 is explicitly routed through the verified shared-helper path rather than the legacy fallback.

The captured v0.2.5 interop assemblies retain the required hook target names:

- `FusionNetworkService.get_MaxPlayers()`
- `OfflineNetworkService.get_MaxPlayers()`
- `RoundController.SetPhase(RoundPhase)`
- `RoundController.PublishCurrentPlayer(Int32)`
- `VoiceDirector.get_Open()`
- `ResultsScreen.LaunchRematch(INetworkService)`
- `ResultsScreen.DropSilentPlayers(INetworkService)`
- `GameState.ResetForRematch()`

### Live acceptance

A real v0.2.5 Windows run confirmed:

- Pack 1.0.3 completed the interop compatibility repair and initialized the IL2CPP chainloader,
- Core 1.0.0 loaded with the expected v0.2.5 fingerprints,
- all required hook targets resolved,
- all Harmony hooks installed,
- the native shared-capacity patch applied at RVA `0x1F8D915`,
- self-check `mode0=10, mode1=10, mode2=10`,
- Expansion 1.1.5 declared itself active for 10 total Classic/Versus players.

The runtime log establishes the hook/patch/self-check acceptance. As before, a complete ten-client match, every Versus team distribution and full large-group audio/rematch behavior are outside this specific startup acceptance claim.

## Mimic Party v0.2.33 — captured 22 September 2026

- Windows x64 / Steam / IL2CPP
- Unity 6000.4.2f1
- BepInEx 6.0.0-be.788
- BepInEx Pack **1.0.2**
- Modding Core **1.0.0**
- Expansion **1.1.4**

GameAssembly.dll SHA-256:
`03757842d82c83534a686b0acbf247c9a5b76d0a15c74c7cb27458e731d4b9d4`

Metadata SHA-256:
`7f4b0ab25b7ba8ee05d9abebd507d3e2af29bd94c5ae48f4daadc2ddedc8bbf2`

Static compatibility verification found exactly one copy of the shared capacity helper signature:

`33 C0 83 F9 01 0F 95 C0 83 C0 04 C3`

Its stock semantics remain Classic 5 / Versus 4.

Helper RVA:
`0x1FCA310`

Runtime patch start:
`0x1FCA315`

The v0.2.33 generated interop assemblies retain the required hook targets:

- `FusionNetworkService.get_MaxPlayers()`
- `OfflineNetworkService.get_MaxPlayers()`
- `RoundController.SetPhase(RoundPhase)`
- `RoundController.PublishCurrentPlayer(Int32)`
- `VoiceDirector.get_Open()`
- `ResultsScreen.LaunchRematch(INetworkService)`
- `ResultsScreen.DropSilentPlayers(INetworkService)`
- `GameState.ResetForRematch()`

### Live acceptance

A real v0.2.33 Windows run confirmed:

- all required hook targets resolved,
- all Harmony hooks installed,
- the native shared-capacity patch applied at RVA `0x1FCA315`,
- self-check `mode0=10, mode1=10, mode2=10`,
- Classic and Versus cards displayed **Up to 10 players**,
- the Classic private lobby displayed **1/10**.

The public room-browser/session-listing API remains present. Expansion 1.1.4 does not falsify remote `RoomListing` values. Public room-browser end-to-end behavior with a large live group is not claimed as fully verified.

## Mimic Party v0.2.3 — captured 22 September 2026

GameAssembly.dll SHA-256:
`adc318d8ad108a2eac4e130421d20c21aef840d8ec44203fba667d6eef08e199`

Metadata SHA-256:
`96b52e058bbf5ea2a5218a2a6c01a9391c72eda9432b640bbdce8d2c10060405`

Expansion 1.1.3 passed startup, hook installation and 10/10/10 helper verification on this build.

## Mimic Party v0.1.73 — captured 11 September 2026

GameAssembly.dll SHA-256:
`44bbc82bdae73c1c86559a1f091ee9c7a3ae510a02c2d83f16b686ecdd9c8b11`

Metadata SHA-256:
`1586b9dd69e488706671d35490cc16377a612ca3af521031ac44464929b21e94`

Expansion 1.1.2 and Core 1.0.0 loaded in the documented Windows session; the shared helper returned 10 for checked selectors and Classic displayed 1/10.

## Verification boundary

A complete ten-client match, every Versus team distribution and full large-group audio/rematch behavior are not implied by hook installation alone. Future game builds are not automatically supported.
