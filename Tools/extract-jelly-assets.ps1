param([string]$SourceDirectory = (Join-Path $PSScriptRoot '../Art/JellyPop'))
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$jellyRoot = Join-Path $PSScriptRoot '../Assets/Resources/JellyPop'
New-Item -ItemType Directory -Force -Path "$jellyRoot/UI", "$jellyRoot/Backgrounds" | Out-Null
function Export-Slice($bitmap, $name, $x, $y, $width, $height) {
    $rect = New-Object System.Drawing.Rectangle($x, $y, $width, $height)
    $slice = $bitmap.Clone($rect, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try { $slice.Save((Join-Path $jellyRoot "$name.png"), [System.Drawing.Imaging.ImageFormat]::Png) }
    finally { $slice.Dispose() }
}
$atlas = [System.Drawing.Bitmap]::FromFile((Join-Path $SourceDirectory 'ui-master.png'))
try {
    $names = @('home','restart','settings','info','sound-on','sound-off','close','undo')
    for ($i=0; $i -lt 8; $i++) {
        Export-Slice $atlas "UI/$($names[$i])" ([int]($i % 4)*319) ([int][math]::Floor($i/4)*310) 319 310
    }
    Export-Slice $atlas 'UI/button-primary' 12 680 330 178
    Export-Slice $atlas 'UI/button-secondary' 347 680 325 178
    Export-Slice $atlas 'UI/panel' 681 630 291 270
    Export-Slice $atlas 'UI/star' 990 630 278 270
    Export-Slice $atlas 'UI/block' 15 912 307 300
    Export-Slice $atlas 'UI/sparkle' 340 916 292 296
    Export-Slice $atlas 'UI/glow' 642 910 316 310
    Export-Slice $atlas 'UI/ring' 963 910 315 310
} finally { $atlas.Dispose() }
$backgrounds = [System.Drawing.Bitmap]::FromFile((Join-Path $SourceDirectory 'backgrounds-master.png'))
try {
    Export-Slice $backgrounds 'Backgrounds/clouds' 0 0 512 1024
    Export-Slice $backgrounds 'Backgrounds/candy' 512 0 512 1024
    Export-Slice $backgrounds 'Backgrounds/sunset' 1024 0 512 1024
} finally { $backgrounds.Dispose() }
Write-Output 'Exported 16 transparent UI/FX PNGs and 3 portrait backgrounds without resampling.'
