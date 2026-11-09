// Non-destructive: only create metadata for the new Jelly Pop assets.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const root = path.resolve(__dirname, '..');
const id = () => crypto.randomBytes(16).toString('hex');
const common = '  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n';
function meta(file, type) {
  const dest = file + '.meta';
  if (fs.existsSync(dest)) return;
  let body = `fileFormatVersion: 2\nguid: ${id()}\n`;
  if (type === 'folder') body += 'folderAsset: yes\nDefaultImporter:\n' + common;
  else if (type === '.png') {
    body += `TextureImporter:
  externalObjects: {}
  serializedVersion: 12
  mipmaps:
    enableMipMap: 0
    sRGBTexture: 1
  isReadable: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  textureType: 0
  textureShape: 1
  spriteMode: 0
  alphaUsage: 1
  alphaIsTransparency: 1
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 100
    crunchedCompression: 0
    overridden: 0
  userData: Jelly Pop generated art; preserve alpha and native dimensions
  assetBundleName: 
  assetBundleVariant: 
`;
  } else if (type === '.shader') body += 'ShaderImporter:\n  defaultTextures: []\n  nonModifiableTextures: []\n' + common;
  else if (type === '.cs') body += 'MonoImporter:\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n' + common;
  else return;
  fs.writeFileSync(dest, body);
}
function walk(folder) {
  meta(folder, 'folder');
  for (const entry of fs.readdirSync(folder, { withFileTypes: true })) {
    const file = path.join(folder, entry.name);
    if (entry.isDirectory()) walk(file); else if (!file.endsWith('.meta')) meta(file, path.extname(file));
  }
}
meta(path.join(root, 'Assets/Resources'), 'folder');
walk(path.join(root, 'Assets/Resources/JellyPop'));
for (const file of ['JellyPopAssets', 'JellyPopVisuals', 'JellyPopPress']) meta(path.join(root, `Assets/Scenes/Scripts/${file}.cs`), '.cs');
meta(path.join(root, 'Assets/Tests/Editor/JellyPopTests.cs'), '.cs');
console.log('Jelly Pop metadata ready. Existing GUIDs retained.');
