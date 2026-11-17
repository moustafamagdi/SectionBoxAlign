param([string]$Compiler)
$ErrorActionPreference='Stop'
if (-not $Compiler) { $Compiler = (Get-Command ISCC.exe -ErrorAction SilentlyContinue).Source }; if (-not $Compiler -or -not (Test-Path $Compiler)) { throw 'Specify -Compiler with the path to Inno Setup 6 ISCC.exe' }
$defines=foreach($year in 2024,2025,2026){$hash=(Get-FileHash "$PSScriptRoot\payload\MMTools.SectionBoxAlign.Legacy.bundle\Contents\$year\MMTools.SectionBoxAlign.dll").Hash; "/DHash$year=$hash"}
$defines+="/DHash2027=$((Get-FileHash "$PSScriptRoot\payload\MMTools.SectionBoxAlign.bundle\Contents\1.1.0\MMTools.SectionBoxAlign.dll").Hash)"
$defines+="/DRibbonHash=$((Get-FileHash "$PSScriptRoot\payload\MMTools.FloatingTools.bundle\PackageContents.xml").Hash)"
& $compiler @defines "$PSScriptRoot\installer\SectionBoxAlign.iss"
if($LASTEXITCODE -ne 0){throw 'Unified Setup compilation failed'}
$setup="$PSScriptRoot\setup\SectionBoxAlign-2024-2027-v1.3.0-User-Setup.exe"
(Get-FileHash $setup).Hash | Set-Content "$setup.sha256"
