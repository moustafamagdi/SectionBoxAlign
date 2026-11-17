Section Box Align - Unified User Setup 1.3.0
Navisworks Manage / Simulate 2024, 2025, 2026 and 2027 (Windows x64)

This installer detects installed releases and installs only their matching binaries.
It checks the Autodesk per-machine/per-user registry locations and standard paths,
and verifies that Roamer.exe exists. Custom installation paths are supported via
Autodesk's Location\Path and Install\ICE43 registry entries.

No Administrator permissions are needed. Files and uninstall registration are
per-user. Multiple installed Navisworks releases are supported simultaneously.
The Ready page lists the versions detected. If none are found, installation stops.
If you install another Navisworks release later, run this Setup again.

Save your work and close Navisworks before installing.
Open My Navis Tools > Section Align.
Align to Face: click inside a planar face.
Align to Dir: click two points for the horizontal direction.
Always Vertical is fixed. Center and dimensions are preserved.
No settings window. Esc or right-click cancels. Prompts appear in the viewport.
Enable Sectioning > Box first. For inclined faces, use Align to Dir.

2027: preserves the existing My Navis Tools buttons and adds the alignment pair.
2024-2026: includes the My Navis Tools tab with the two alignment buttons.
Older compatible bundles are reused so the same tool is not registered twice.
This Setup upgrades the existing 2027 User Setup entry to the unified product.

Logs: Desktop\SectionBoxAlign Logs
Uninstall: Windows Settings > Apps > Section Box Align (Navisworks 2024-2027).

Validation: actual host tests on Manage 2024 and 2027. 2025 and 2026 have separate
API-specific builds but have not been live-tested on those releases.
