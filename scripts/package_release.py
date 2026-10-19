"""Build the release archive from the current verified Expansion source build."""
from pathlib import Path, PurePosixPath
import hashlib
import json
import zipfile

ROOT = Path(__file__).resolve().parents[1]
CFG = json.loads((ROOT / 'release.json').read_text(encoding='utf-8'))
OUT = ROOT / 'dist'
SHA = lambda b: hashlib.sha256(b).hexdigest()
JSON = lambda x: (json.dumps(x, indent=2, ensure_ascii=False) + '\n').encode('utf-8')


def read_archive(path):
    result = {}
    seen = set()
    with zipfile.ZipFile(path) as archive:
        if archive.testzip() is not None:
            raise ValueError('Corrupt archive')
        for info in archive.infolist():
            if info.is_dir():
                continue
            name = info.filename.replace('\\', '/')
            p = PurePosixPath(name)
            if p.is_absolute() or '..' in p.parts or ':' in name:
                raise ValueError('Unsafe archive path: ' + name)
            if name.casefold() in seen:
                raise ValueError('Duplicate archive path: ' + name)
            seen.add(name.casefold())
            result[name] = archive.read(info)
    return result


def manifest(files):
    return ''.join(f'{SHA(data)}  {name}\n' for name, data in sorted(files.items())).encode('utf-8')


def write_archive(filename, files, manifest_path):
    files = dict(files)
    files[manifest_path] = manifest(files)
    if len({n.casefold() for n in files}) != len(files):
        raise ValueError('Case-insensitive path collision')

    forbidden = {'gameassembly.dll', 'unityplayer.dll', 'unityengine.coremodule.dll', 'global-metadata.dat'}
    for name in files:
        p = PurePosixPath(name)
        if p.is_absolute() or '..' in p.parts or ':' in name or p.name.casefold() in forbidden:
            raise ValueError('Forbidden path: ' + name)
        if p.suffix.lower() in {'.ttf', '.otf', '.woff', '.woff2', '.ttc', '.eot', '.zip', '.rar', '.7z'}:
            raise ValueError('Unexpected nested archive or font: ' + name)

    OUT.mkdir(exist_ok=True)
    path = OUT / filename
    with zipfile.ZipFile(path, 'w', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for name, data in sorted(files.items()):
            info = zipfile.ZipInfo(name, (2026, 9, 22, 0, 0, 0))
            info.compress_type = zipfile.ZIP_DEFLATED
            info.external_attr = 0o100644 << 16
            archive.writestr(info, data)

    if read_archive(path) != files:
        raise ValueError('Written archive mismatch')
    print(filename + ' ' + SHA(path.read_bytes()))
    return path


if CFG['Component'] != 'expansion':
    raise ValueError('Unexpected component')

dll_path = ROOT / 'src' / 'MimicParty.TenPlayerExpansion' / 'bin' / 'Release' / 'net6.0' / 'MimicParty10PlayerExpansion.dll'
if not dll_path.is_file():
    raise ValueError('Compiled Expansion DLL missing; run build.ps1 first')
dll = dll_path.read_bytes()
dll_sha = SHA(dll)

prefix = 'MimicParty10PlayerExpansion/'
files = {
    prefix + name: (ROOT / name).read_bytes()
    for name in [
        'README.md', 'CHANGELOG.md', 'COMPATIBILITY.md', 'LICENSE.txt', 'SECURITY.md',
        'compatibility/v0.2.10/README.md', 'compatibility/v0.2.10/interop-bootstrap.patch'
    ]
}
files['BepInEx/plugins/MimicParty10PlayerExpansion.dll'] = dll
files[prefix + 'PROVENANCE.json'] = JSON({
    'Author': CFG['Author'],
    'Component': CFG['Component'],
    'Version': CFG['Version'],
    'Revision': CFG['Revision'],
    'Prerelease': CFG['Prerelease'],
    'BinaryVersions': CFG['BinaryVersions'],
    'BinarySHA256': {'Expansion': dll_sha},
    'RuntimeRecompiled': True,
    'ReleaseRepository': CFG['Repositories']['expansion'],
    'NexusPage': CFG['Nexus']['expansion'],
    'IncludesCore': False,
    'IncludesBootstrap': False,
    'Requirements': {
        'Core': CFG['Nexus']['core'],
        'Pack': CFG['Nexus']['pack'],
        'InteropBootstrapMinimumVersionForV0210': '1.0.4'
    },
    'SupportedGameProfiles': [
        {
            'Label': 'v0.1.73 / Sep 11',
            'GameAssemblySHA256': '44bbc82bdae73c1c86559a1f091ee9c7a3ae510a02c2d83f16b686ecdd9c8b11',
            'MetadataSHA256': '1586b9dd69e488706671d35490cc16377a612ca3af521031ac44464929b21e94'
        },
        {
            'Label': 'v0.2.3 / Sep 22',
            'GameAssemblySHA256': 'adc318d8ad108a2eac4e130421d20c21aef840d8ec44203fba667d6eef08e199',
            'MetadataSHA256': '96b52e058bbf5ea2a5218a2a6c01a9391c72eda9432b640bbdce8d2c10060405'
        },
        {
            'Label': 'v0.2.33 / Sep 22',
            'GameAssemblySHA256': '03757842d82c83534a686b0acbf247c9a5b76d0a15c74c7cb27458e731d4b9d4',
            'MetadataSHA256': '7f4b0ab25b7ba8ee05d9abebd507d3e2af29bd94c5ae48f4daadc2ddedc8bbf2'
        },
        {
            'Label': 'v0.2.5 / Sep 30 / Steam 25629135',
            'GameAssemblySHA256': '02546fc8797c32a9f98a8b93a7eefc11f52fb58e83fd51db29fd83eb5601d25a',
            'MetadataSHA256': 'dded356e7a47bc941b63e4a09280bd7fb8e777bf5748366d47345363df9a9bbf'
        },
        {
            'Label': 'v0.2.10 / Oct 19 / Steam 26008035',
            'GameAssemblySHA256': '35bf5cc8a954a827c3c20fe76bd9d92191eed9dc83140101231081ba018a15f2',
            'MetadataSHA256': '7db463145f922d7a83c0c29879f5467bd40f82562ea549701f71221f6a35aaea',
            'Validation': 'Static checks only; live acceptance pending'
        }
    ]
})

version = CFG['Version']
revision = 'R' + str(CFG['Revision'])
output = write_archive(
    f'MimicParty_10_Player_Expansion_v{version}_{revision}.zip',
    files,
    prefix + 'SHA256SUMS.txt'
)
(OUT / 'SHA256SUMS.txt').write_text(
    f'{SHA(output.read_bytes())}  {output.name}\n',
    encoding='ascii'
)
