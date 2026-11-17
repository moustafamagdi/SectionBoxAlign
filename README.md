# Section Box Align

Navisworks Manage / Simulate 2024–2027 add-in by MMTools.

Two buttons in **My Navis Tools → Section Align** start picking immediately:

- **Align to Face**: align a box side with a planar face using its horizontal normal.
- **Align to Dir**: pick two points to align the box X axis with their horizontal direction.

Always Vertical is fixed on. The section box center and dimensions are preserved. Esc or right-click cancels. Horizontal faces retain the current heading; inclined faces cannot be fully aligned while keeping the box vertical. Use Align to Dir for those cases.

## Install

Download `SectionBoxAlign-2024-2027-v1.3.0-User-Setup.exe` from Releases. Close Navisworks and run Setup normally. Installation is for the current Windows user and requires no administrator rights. Setup detects installed Manage or Simulate versions and installs their matching binaries. Run it again after installing another supported Navisworks version.

The 2027 package integrates with the existing shared My Navis Tools ribbon. Its six other commands refer to separately installed tools; their implementations are not included. The 2024–2026 ribbon contains the two alignment buttons.

## Build

Windows, .NET Framework C# compiler, matching Autodesk Navisworks API reference assemblies, and Inno Setup 6 are required. Autodesk assemblies are not redistributed in this repository or installer. Obtain references from the appropriate licensed Navisworks installations.

```powershell
./Build.ps1 -ApiPaths @{
    2024 = 'C:\SDK\2024\Autodesk.Navisworks.Api.dll'
    2025 = 'C:\SDK\2025\Autodesk.Navisworks.Api.dll'
    2026 = 'C:\SDK\2026\Autodesk.Navisworks.Api.dll'
    2027 = 'C:\SDK\2027\Autodesk.Navisworks.Api.dll'
} -Setup -Compiler 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
```

Without overrides, the build looks in each year's standard Navisworks Manage installation directory. `Test.ps1` runs standalone geometry/JSON regression checks. `Build-Installer.ps1 -Compiler <ISCC.exe>` packages the checked-in MMTools binary snapshot without rebuilding it. Setup output is ignored by Git and distributed through Releases. `Make-Icons.ps1` regenerates the 2027 alignment icons.

## Validation and limits

- Geometry/JSON regression suite: 20,025 checks for each source variant.
- Native host checks performed on Navisworks 2024 and 2027, including rotation/center/size round-trips and immediate tool activation.
- 2025 and 2026 compiled against matching API assemblies; live host validation remains pending. The release used community NuGet reference packages for these two compile-only targets.
- Installer detection was exercised on a machine with 2024 and 2027 installed; full install/uninstall on all four versions and Simulate remain pending.
- The installer runs asInvoker/current user. Source includes native diagnostic commands; no customer models or screenshots are included.

`src/Legacy` targets 2024–2026; `src/2027` uses the newer picking API with a sampled-surface fallback. `src/Ribbon2027` integrates the commands, and `src/RibbonBaseline` supports restoring the shared ribbon on uninstall. `installer/SectionBoxAlign.iss` contains version detection and user-only installation logic.
