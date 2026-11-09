// Decode real PNG scanlines to check the exported alpha, not just file extensions.
const fs = require('node:fs');
const path = require('node:path');
const zlib = require('node:zlib');
const assert = require('node:assert/strict');
const root = path.resolve(__dirname, '..');
const art = path.join(root, 'Assets/Resources/JellyPop');
const paeth = (a,b,c) => { const p=a+b-c, x=Math.abs(p-a), y=Math.abs(p-b), z=Math.abs(p-c); return x<=y && x<=z ? a : y<=z ? b : c; };
function png(file) {
  const data=fs.readFileSync(file), parts=[];
  assert.equal(data.subarray(1,4).toString(), 'PNG');
  const w=data.readUInt32BE(16), h=data.readUInt32BE(20), depth=data[24], type=data[25];
  assert.equal(depth,8); assert.ok(type===2 || type===6); assert.equal(data[28],0);
  for(let pos=8;pos<data.length;) { const length=data.readUInt32BE(pos); if(data.toString('ascii',pos+4,pos+8)==='IDAT') parts.push(data.subarray(pos+8,pos+8+length)); pos+=12+length; }
  const bytes=type===6?4:3, stride=w*bytes, raw=zlib.inflateSync(Buffer.concat(parts)), decoded=Buffer.alloc(stride*h);
  for(let y=0;y<h;y++) {
    const filter=raw[y*(stride+1)]; assert.ok(filter<=4);
    for(let x=0;x<stride;x++) {
      const a=x>=bytes?decoded[y*stride+x-bytes]:0, b=y?decoded[(y-1)*stride+x]:0, c=y && x>=bytes?decoded[(y-1)*stride+x-bytes]:0;
      const predictor=[0,a,b,Math.floor((a+b)/2),paeth(a,b,c)][filter];
      decoded[y*stride+x]=(raw[y*(stride+1)+x+1]+predictor)&255;
    }
  }
  return { w,h,bytes,decoded };
}
const names=['home','restart','settings','info','sound-on','sound-off','close','undo','button-primary','button-secondary','panel','star','block','sparkle','glow','ring'];
for(const name of names) {
  const file=path.join(art,'UI',name+'.png'), img=png(file);
  assert.equal(img.bytes,4,name+' must preserve RGBA');
  let transparent=0,visible=0;
  for(let i=3;i<img.decoded.length;i+=4) { if(img.decoded[i]===0) transparent++; if(img.decoded[i]>200) visible++; }
  assert.ok(transparent>img.w*img.h*.01,name+' must have actual transparency');
  assert.ok(visible>img.w*img.h*.01,name+' must contain visible art');
  assert.match(fs.readFileSync(file+'.meta','utf8'), /alphaIsTransparency: 1/);
}
for(const name of ['clouds','candy','sunset']) {
  const img=png(path.join(art,'Backgrounds',name+'.png'));
  assert.equal(img.w,512); assert.equal(img.h,1024);
}
const newBackgrounds=['icecream-coast','strawberry-garden','lavender-forest','lemon-balloons','moonlit-candy','pearl-reef'];
for(const name of newBackgrounds) {
  const img=png(path.join(art,'Backgrounds',name+'.png'));
  assert.equal(img.w,1024,name+' native width'); assert.equal(img.h,1536,name+' native height');
}
const assetsSource=fs.readFileSync(path.join(root,'Assets/Scenes/Scripts/JellyPopAssets.cs'),'utf8');
const catalog=[...assetsSource.match(/BackgroundNames = \{([\s\S]*?)\};/)[1].matchAll(/"([^"]+)"/g)].map(m=>m[1]);
assert.equal(catalog.length,10); assert.equal(new Set(catalog).size,9);
assert.deepEqual(catalog.slice(0,4),['clouds','candy','sunset','clouds'],'Saved original themes keep their indices');
for(const name of catalog) assert.ok(fs.existsSync(path.join(art,'Backgrounds',name+'.png')),name+' catalog resource exists');
const uiSource=fs.readFileSync(path.join(root,'Assets/Scenes/Scripts/CalmGameUI.cs'),'utf8');
for(const name of ['Backgrounds','Accents','Inks']) {
  const palette=uiSource.match(new RegExp(name+' = \\{([\\s\\S]*?)\\};'))[1];
  assert.equal([...palette.matchAll(/Hex\(/g)].length,catalog.length,name+' palette aligns with catalog');
}
const guids=[];
function metas(folder) { for(const entry of fs.readdirSync(folder,{withFileTypes:true})) { const file=path.join(folder,entry.name); if(entry.isDirectory()) metas(file); else if(file.endsWith('.meta')) guids.push(fs.readFileSync(file,'utf8').match(/^guid: ([a-f0-9]{32})$/m)?.[1]); } }
metas(art); assert.ok(guids.every(Boolean)); assert.equal(guids.length,new Set(guids).size);
for(const name of ['JellySurface','JellyBackdrop']) assert.ok(fs.existsSync(path.join(art,name+'.shader')));
console.log('PASS: 16 transparent RGBA sprites; 9 distinct portrait background files decoded (6 new at 1024x1536); 10 catalog entries and UI palettes align; legacy theme indices, import settings, shaders and unique GUIDs verified.');
