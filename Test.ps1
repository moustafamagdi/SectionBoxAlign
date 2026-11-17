$ErrorActionPreference = 'Stop'
$out = "$PSScriptRoot\build"
New-Item -ItemType Directory -Force $out | Out-Null
foreach ($variant in 'Legacy','2027') {
    & "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe "/out:$out\MathTests-$variant.exe" /reference:System.Web.Extensions.dll "$PSScriptRoot\src\$variant\Geometry.cs" "$PSScriptRoot\src\$variant\Box.cs" "$PSScriptRoot\tests\MathTests.cs"
    if ($LASTEXITCODE -ne 0) { throw "Test compile failed: $variant" }
    & "$out\MathTests-$variant.exe"
    if ($LASTEXITCODE -ne 0) { throw "Tests failed: $variant" }
}
