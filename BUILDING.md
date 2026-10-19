# Building

Use the .NET 8 SDK to build the net6.0 plugin. Clone the Core source next to this repository or set MIMIC_PARTY_CORE_PROJECT to its csproj.

```powershell
./build.ps1
```

Core source revision pinned by CI: de85239b8bb6955fce28ef584073cdd63b53dfcd. The build script also searches `_deps/MimicParty-Modding`.

Run `python scripts/package_release.py` after building. It packages the compiled `bin/Release/net6.0/MimicParty10PlayerExpansion.dll` and documentation, verifies the archive, and writes the ZIP and SHA-256 manifest to `dist/`. Version, revision, and prerelease status come from `release.json`.

The v0.2.10 candidate also needs an updated bootstrap from the separate Pack project. See [candidate build instructions](compatibility/v0.2.10/README.md) and its reproducible source patch. Private game-derived captures are not source-tree or release dependencies.
