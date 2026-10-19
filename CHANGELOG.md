# Changelog

## Runtime 1.1.6 — local v0.2.10 compatibility candidate

- Recognizes the captured v0.2.10 / Steam build 26008035 GameAssembly and metadata hashes and selects the existing shared capacity-helper patch.
- Retains transactional byte verification, rollback, and the required live capacity self-check; no signature or hook changes were needed by static inspection.
- Includes a companion source patch for Interop Bootstrap 1.0.4 to repair the new generated CoreModule layout. Pack 1.0.3 alone cannot load plugins on this build.
- Aligns the assembly version with the plugin version and marks release metadata as prerelease.
- Build and offline metadata-repair checks pass. Live startup and multiplayer acceptance remain pending; this is not a verified stable release.

## Packaging revision R2 — 30 September 2026

- Promotes Expansion **1.1.5** to the stable, live-verified release path for Mimic Party **v0.2.5 / Steam build 25629135**.
- Real Windows acceptance confirmed every required hook target resolved and every Harmony hook installed.
- Confirmed the shared Classic/Versus capacity patch applied at RVA `0x1F8D915`.
- Confirmed runtime self-check `mode0=10, mode1=10, mode2=10`.
- Keeps the Expansion runtime at **1.1.5**; R2 finalizes packaging/documentation after live acceptance.
- Requires stable BepInEx Pack **1.0.3** and Modding Core **1.0.0**.

## Runtime 1.1.5 — 30 September 2026

- Adds explicit support for the captured Mimic Party **v0.2.5 / Steam build 25629135** GameAssembly fingerprint.
- Confirms the shared Classic/Versus capacity-helper signature remains unique and still encodes stock Classic 5 / Versus 4 behavior.
- Records the new helper RVA `0x1F8D910`; runtime patch application begins at `0x1F8D915`.
- Explicitly routes v0.2.5 through the shared-helper path; the four legacy capacity patterns have no matches in the captured build.
- Revalidates the required v0.2.5 interop target names for max-player, playback voice-isolation and rematch hooks.
- Preserves the runtime 10/10/10 helper self-check and transactional patch verification.
- Requires BepInEx Pack **1.0.3** for the new interop fingerprint.
- Live v0.2.5 acceptance passed: all required hooks installed, the shared native capacity patch applied and the 10/10/10 helper self-check passed.

## Runtime 1.1.4 — 22 September 2026

- Adds explicit support for the captured Mimic Party v0.2.33 GameAssembly fingerprint.
- Confirms the shared Classic/Versus capacity helper remains unique with the same stock 5/4 signature.
- Records the current helper RVA `0x1FCA310`; runtime patch application begins at `0x1FCA315`.
- Keeps the shared-capacity path fail-closed to explicitly supported builds.
- Revalidates all required v0.2.33 interop targets for max-player, playback voice-isolation and rematch hooks.
- Keeps the live helper self-check that must return the configured capacity for mode selectors 0, 1 and 2.
- Live v0.2.33 acceptance passed: all hooks installed, native patch applied, 10/10/10 self-check passed, Classic/Versus mode cards displayed Up to 10, and the Classic private lobby displayed 1/10.
- Does not patch remote RoomListing values directly.
- Requires BepInEx Pack 1.0.2 for the v0.2.33 interop bootstrap.

## Runtime 1.1.3 — 22 September 2026

- Added explicit support for the captured Mimic Party v0.2.3 GameAssembly fingerprint.
- Kept the shared Classic/Versus capacity helper patch fail-closed to known builds.
- Revalidated the required v0.2.3 interop targets.

## Packaging revision R3 — 11 September 2026

- Made the main archive Expansion-only, with explicit Core and BepInEx Pack requirements.
- Connected all three Nexus pages and dedicated repositories.
- Placed documentation in MimicParty10PlayerExpansion/ to prevent overwriting other guides.
- Kept the verified Expansion 1.1.2 DLL byte-for-byte unchanged.

## Runtime 1.1.2

Corrected LaunchRematch(INetworkService) resolution, added per-hook logging and preserved the shared Classic/Versus capacity helper with its live self-check.

## Legacy 1.0.x

Older on-disk distribution. Restore original game files before migrating.
