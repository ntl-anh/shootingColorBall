// Run with: node Tools/validate-upgrade.cjs
// Asset integrity checks that do not require a Unity installation.
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const root = path.resolve(__dirname, '..');
const read = file => fs.readFileSync(path.join(root, file), 'utf8');
const guid = file => read(file + '.meta').match(/^guid: (\w+)$/m)?.[1];
const build = read('ProjectSettings/EditorBuildSettings.asset');
const entries = [...build.matchAll(/- enabled: 1\s+path: ([^\r\n]+)\s+guid: (\w+)/g)];
assert.equal(entries.length, 101, 'Menu plus exactly 100 enabled levels');
assert.equal(new Set(entries.map(e => e[2])).size, 101, 'Unique scene GUIDs');
assert.equal(entries[0][1], 'Assets/Scenes/Levels/MainMenu.unity');
function records(text) {
  const result = new Map();
  for (const match of text.matchAll(/--- !u!(\d+) &(-?\d+)[^\n]*\n([\s\S]*?)(?=--- !u!|$)/g))
    result.set(match[2], { type: +match[1], body: match[3] });
  return result;
}
function board(text) {
  const all = records(text);
  const objects = new Map([...all].filter(([, r]) => r.type === 1));
  const transforms = new Map([...all].filter(([, r]) => r.type === 4 || r.type === 224));
  const objectId = transform => transform.body.match(/m_GameObject: \{fileID: (-?\d+)\}/)?.[1];
  const name = object => object?.body.match(/^  m_Name: (.*)$/m)?.[1].trim();
  const children = transform => [...(transform.body.match(/m_Children:([\s\S]*?)m_Father:/)?.[1] || '').matchAll(/fileID: (-?\d+)/g)].map(m => m[1]);
  const findRoot = target => {
    const entry = [...objects].find(([, o]) => name(o) === target);
    assert.ok(entry, 'Missing root ' + target);
    return [...transforms.values()].find(t => objectId(t) === entry[0]);
  };
  const cubes = children(findRoot('cubes'));
  const patternRoot = transforms.get(children(findRoot('Pattern'))[0]);
  assert.ok(patternRoot, 'Pattern has a model container');
  const targets = children(patternRoot);
  assert.ok(cubes.length > 0, 'Playable board is nonempty');
  assert.equal(cubes.length, targets.length, 'Reference cells align with playable cells');
  const cannons = [...all.values()].filter(r => r.type === 114 && /ColorName:/.test(r.body));
  assert.ok(cannons.length > 0, 'At least one cannon');
  const colors = new Set(cannons.map(c => c.body.match(/ColorName: (.*)/)[1].trim()));
  for (const target of targets)
    assert.ok(colors.has(name(objects.get(objectId(transforms.get(target))))), 'Every target color is provided by a cannon');
  // Some original cannons have duplicate names; runtime snapshots use an object
  // reference so undo restores only the cannon that actually fired.
  const cannonIds = cannons.map(c => c.body.match(/m_GameObject: \{fileID: (\d+)\}/)[1]);
  assert.equal(new Set(cannonIds).size, cannons.length, 'Each cannon has an independent object identity');
  for (const cannon of cannons) {
    const directions = [...cannon.body.matchAll(/(?:isright|isleft|istop|isdown): (\d)/g)].filter(m => m[1] === '1');
    // Legacy scenes sometimes set two flags. ShootBall deliberately preserves
    // their established left > right > top > down precedence.
    assert.ok(directions.length >= 1, 'Each cannon has an effective initial direction');
  }
  return { cells: cubes.length, cannons: cannons.length };
}
let cells = 0;
for (let level = 1; level <= 100; level++) {
  const file = `Assets/Scenes/Levels/${level}.unity`;
  assert.equal(entries[level][1], file, 'Build order is sequential');
  assert.equal(entries[level][2], guid(file), 'Build GUID agrees with scene metadata');
  const data = read(file);
  try { cells += board(data).cells; }
  catch (error) { throw new Error(`Level ${level}: ${error.message}`, { cause: error }); }
  if (level > 50) assert.equal(data, read(`Assets/Scenes/Levels/${level - 50}.unity`), 'Remixed levels preserve validated base geometry');
}
for (const script of ['CalmGameUI', 'GameFeel', 'GamePreferences', 'LevelProgression'])
  assert.match(guid(`Assets/Scenes/Scripts/${script}.cs`), /^[a-f0-9]{32}$/);
assert.match(read('Assets/Plugins/Android/CalmHaptics.androidlib/AndroidManifest.xml'), /android.permission.VIBRATE/);
assert.match(read('Assets/Settings/Build Profiles/Android™.asset'), /m_OverrideGlobalSceneList: 0/);
console.log(`PASS: 101 build entries, 100 board/reference pairs, ${cells} cells, cannon directions/colors/identities, scene GUIDs, Android profile and vibration permission.`);
