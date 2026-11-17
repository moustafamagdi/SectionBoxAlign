param([hashtable]$ApiPaths = @{}, [string]$Compiler, [switch]$Setup)
$ErrorActionPreference = 'Stop'
$csc = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
foreach ($year in 2024,2025,2026,2027) {
    $api = $ApiPaths[$year]
    if (-not $api) { $api = "C:\Program Files\Autodesk\Navisworks Manage $year\Autodesk.Navisworks.Api.dll" }
    if (-not (Test-Path $api)) { throw "Supply -ApiPaths @{$year='path\Autodesk.Navisworks.Api.dll'}" }
    if ([Reflection.AssemblyName]::GetAssemblyName($api).Version.Major -ne ($year-2003)) { throw "Wrong API version for $year" }
    $source = if ($year -eq 2027) { '2027' } else { 'Legacy' }
    $output = if ($year -eq 2027) { "$PSScriptRoot\payload\MMTools.SectionBoxAlign.bundle\Contents\1.1.0" } else { "$PSScriptRoot\payload\MMTools.SectionBoxAlign.Legacy.bundle\Contents\$year" }
    $sources = @(Get-ChildItem "$PSScriptRoot\src\$source\*.cs" | Select-Object -ExpandProperty FullName)
    & $csc /nologo /target:library /platform:x64 /optimize+ "/out:$output\MMTools.SectionBoxAlign.dll" "/reference:$api" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Web.Extensions.dll /reference:Microsoft.CSharp.dll @sources
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $year" }
    if ($year -ne 2027) {
        Copy-Item "$PSScriptRoot\src\Legacy\SectionAlign.xaml" $output -Force
        Copy-Item "$PSScriptRoot\src\Legacy\SectionAlign.xaml" "$output\en-US" -Force
    } else {
        foreach ($ribbon in 'Ribbon2027','RibbonBaseline') {
            $dest = if ($ribbon -eq 'Ribbon2027') { "$PSScriptRoot\payload\MMTools.FloatingTools.bundle\Contents\Align1.1.0" } else { "$PSScriptRoot\installer\RibbonBeforeAlign" }
            & $csc /nologo /target:library /platform:x64 /optimize+ "/out:$dest\MMTools.FloatingTools.dll" "/reference:$api" /reference:System.Windows.Forms.dll "$PSScriptRoot\src\$ribbon\FloatingRibbon.cs"
            if ($LASTEXITCODE -ne 0) { throw "Ribbon build failed: $ribbon" }
            Copy-Item "$PSScriptRoot\src\$ribbon\FloatingTools.xaml" $dest -Force
            Copy-Item "$PSScriptRoot\src\$ribbon\FloatingTools.xaml" "$dest\en-US" -Force
        }
    }
}
& "$PSScriptRoot\Test.ps1"
if ($Setup) { & "$PSScriptRoot\Build-Installer.ps1" -Compiler $Compiler }
