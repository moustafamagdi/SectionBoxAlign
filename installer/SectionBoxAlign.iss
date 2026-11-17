[Setup]
; Keep the 2027 AppId so the unified Setup upgrades the existing entry.
AppId=MMTools.SectionBoxAlign.Navisworks2027.User
AppName=Section Box Align (Navisworks 2024-2027)
AppVersion=1.3.0
AppPublisher=MMTools
VersionInfoVersion=1.3.0.0
DefaultDirName={localappdata}\MMTools\SectionBoxAlign
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableWelcomePage=no
WizardStyle=modern
SetupLogging=yes
CloseApplications=no
RestartApplications=no
Compression=lzma2
SolidCompression=yes
OutputDir=..\setup
OutputBaseFilename=SectionBoxAlign-2024-2027-v1.3.0-User-Setup
UninstallDisplayName=Section Box Align (Navisworks 2024-2027, Current User)

[Messages]
WelcomeLabel2=Install Section Box Align for your Windows account only.%n%nSetup detects Navisworks Manage and Simulate 2024, 2025, 2026 and 2027 and installs only the matching versions.%n%nAlign to Face / Align to Dir. Always Vertical. No settings window.%n%nNo Administrator permissions are needed. Save your work and close Navisworks before installing.
FinishedLabel=Section Box Align has been installed for the detected Navisworks versions.%n%nOpen My Navis Tools > Section Align.%n%nIf you install another Navisworks release later, run this Setup again.

[Files]
Source: "..\payload\MMTools.SectionBoxAlign.Legacy.bundle\Contents\2024\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.Legacy.bundle\Contents\2024"; Flags: ignoreversion recursesubdirs; Check: HasYear(2024)
Source: "..\payload\MMTools.SectionBoxAlign.Legacy.bundle\Contents\2025\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.Legacy.bundle\Contents\2025"; Flags: ignoreversion recursesubdirs; Check: HasYear(2025)
Source: "..\payload\MMTools.SectionBoxAlign.Legacy.bundle\Contents\2026\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.Legacy.bundle\Contents\2026"; Flags: ignoreversion recursesubdirs; Check: HasYear(2026)
Source: "{tmp}\Legacy-PackageContents.xml"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.Legacy.bundle"; DestName: "PackageContents.xml"; Flags: external ignoreversion; Check: HasLegacy
Source: "..\payload\MMTools.SectionBoxAlign.bundle\Contents\1.1.0\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.bundle\Contents\1.1.0"; Flags: ignoreversion recursesubdirs; Check: HasYear(2027)
Source: "..\payload\MMTools.SectionBoxAlign.bundle\PackageContents.xml"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.bundle"; Flags: ignoreversion; Check: HasYear(2027)
Source: "..\payload\MMTools.FloatingTools.bundle\Contents\Align1.1.0\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.FloatingTools.bundle\Contents\Align1.1.0"; Flags: ignoreversion recursesubdirs uninsneveruninstall; Check: HasYear(2027)
Source: "..\payload\MMTools.FloatingTools.bundle\PackageContents.xml"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.FloatingTools.bundle"; Flags: ignoreversion uninsneveruninstall; Check: HasYear(2027)
Source: "RibbonBeforeAlign\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\MMTools.FloatingTools.bundle\Contents\BeforeAlign1.1.0"; Flags: ignoreversion recursesubdirs uninsneveruninstall; Check: HasYear(2027)
Source: "RestoreRibbon.xml"; DestDir: "{app}"; Flags: ignoreversion; Check: HasYear(2027)
Source: "..\payload\README.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{userprograms}\Section Box Align\User Guide"; Filename: "{app}\README.txt"
Name: "{userprograms}\Section Box Align\Uninstall"; Filename: "{uninstallexe}"

[Code]
var Found: array[0..3] of Boolean; Locations: array[0..3] of String;

function HasYear(Year: Integer): Boolean;
begin
 Result := Found[Year-2024];
end;
function HasLegacy: Boolean;
begin
 Result := Found[0] or Found[1] or Found[2];
end;
function Candidate(const Value: String; IsExe: Boolean): String;
var Path: String;
begin
 Result := '';
 Path := RemoveQuotes(Value);
 if not IsExe then Path := AddBackslash(Path)+'Roamer.exe';
 if FileExists(Path) then Result := Path;
end;
function RegistryLocation(Root: Integer; const Product: String; Year: Integer): String;
var Base, Value: String;
begin
 Result := '';
 Base := 'SOFTWARE\Autodesk\Navisworks '+Product+'\'+IntToStr(Year-2003)+'.0';
 if RegQueryStringValue(Root,Base+'\Location','Path',Value) then Result := Candidate(Value,False);
 if (Result='') and RegQueryStringValue(Root,Base+'\Install\ICE43','roamer.exe',Value) then Result := Candidate(Value,True);
end;
function ProductLocation(const Product: String; Year: Integer): String;
begin
 Result := RegistryLocation(HKLM64,Product,Year);
 if Result='' then Result := RegistryLocation(HKCU64,Product,Year);
 if Result='' then Result := RegistryLocation(HKLM32,Product,Year);
 if Result='' then Result := RegistryLocation(HKCU32,Product,Year);
 if Result='' then Result := Candidate(ExpandConstant('{pf64}')+'\Autodesk\Navisworks '+Product+' '+IntToStr(Year),False);
end;
function DetectedText: String;
var I: Integer;
begin
 Result := '';
 for I:=0 to 3 do if Found[I] then begin
  if Result<>'' then Result:=Result+', ';
  Result:=Result+IntToStr(2024+I);
 end;
end;
function BuildLegacyManifest: String;
var Year: Integer; Series: String;
begin
 Result := '<?xml version="1.0" encoding="utf-8"?>'+#13#10+
 '<ApplicationPackage SchemaVersion="1.0" AutodeskProduct="Navisworks" Name="Section Box Align 2024-2026" AppVersion="1.3.0" ProductType="Application" Author="MMTools" ProductCode="{2CBA7651-9276-4651-B392-44E361C9FAB6}" UpgradeCode="{C50EA756-5D50-4212-A7F3-AD297C059280}">'+#13#10;
 for Year:=2024 to 2026 do if HasYear(Year) then begin
  Series:='Nw'+IntToStr(Year-2003);
  Result:=Result+'<Components><RuntimeRequirements OS="Win64" Platform="NAVMAN|NAVSIM" SeriesMin="'+Series+'" SeriesMax="'+Series+'"/><ComponentEntry AppName="SectionBoxAlign'+IntToStr(Year)+'" AppType="ManagedPlugin" ModuleName="./Contents/'+IntToStr(Year)+'/MMTools.SectionBoxAlign.dll"/></Components>'+#13#10;
 end;
 Result:=Result+'</ApplicationPackage>';
end;
function InitializeSetup: Boolean;
var I: Integer; Report, ManifestPath, Detection: String;
begin
 for I:=0 to 3 do begin
  Locations[I]:=ProductLocation('Manage',I+2024);
  if Locations[I]='' then Locations[I]:=ProductLocation('Simulate',I+2024);
  Found[I]:=Locations[I]<>'';
 end;
 Report:=ExpandConstant('{param:DETECTREPORT|}');
 if Report<>'' then begin
  Detection:='Detected='+DetectedText+#13#10;
  for I:=0 to 3 do Detection:=Detection+IntToStr(I+2024)+'='+Locations[I]+#13#10;
  SaveStringToFile(Report,AnsiString(Detection),False);
  ManifestPath:=Report+'.xml';
  if HasLegacy then SaveStringToFile(ManifestPath,AnsiString(BuildLegacyManifest),False);
  Result:=False;
  Exit;
 end;
 Result:=DetectedText<>'';
 if not Result then SuppressibleMsgBox('No supported Navisworks Manage or Simulate installation was found. Install Navisworks 2024, 2025, 2026 or 2027, then run Setup again.',mbError,MB_OK,IDOK);
end;
function UpdateReadyMemo(Space,NewLine,MemoUserInfoInfo,MemoDirInfo,MemoTypeInfo,MemoComponentsInfo,MemoGroupInfo,MemoTasksInfo: String): String;
begin
 Result:='Detected Navisworks releases: '+DetectedText+NewLine+NewLine+
 'Only these versions will be installed for the current Windows user.'+NewLine+
 'Location: '+ExpandConstant('{userappdata}\Autodesk\ApplicationPlugins')+NewLine+NewLine+
 'Open My Navis Tools > Section Align after installation.';
end;
function RunningReason: String;
var Locator, Service, Processes: Variant;
begin
 Result:='';
 try
  Locator:=CreateOleObject('WbemScripting.SWbemLocator');
  Service:=Locator.ConnectServer('.', 'root\CIMV2');
  Processes:=Service.ExecQuery('SELECT ProcessId FROM Win32_Process WHERE Name = ''Roamer.exe''');
  if Processes.Count>0 then Result:='Save your work and close Navisworks, then try again. Setup will not close it automatically.';
 except
  Result:='Cannot check whether Navisworks is running. Close Navisworks and try again.';
 end;
end;
function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
 Result:=RunningReason;
 if (Result='') and HasLegacy then
  if not SaveStringToFile(ExpandConstant('{tmp}\Legacy-PackageContents.xml'),AnsiString(BuildLegacyManifest),False) then Result:='Cannot prepare the version-specific plugin manifest.';
end;
procedure CurStepChanged(CurStep: TSetupStep);
var Root, Source, Backup: String;
begin
 if CurStep=ssInstall then begin
  ForceDirectories(ExpandConstant('{app}'));
  if HasYear(2027) then begin
   Source:=ExpandConstant('{userappdata}\Autodesk\ApplicationPlugins\MMTools.FloatingTools.bundle\PackageContents.xml');
   Backup:=ExpandConstant('{app}\PreviousRibbon.xml');
   if FileExists(Source) and not FileExists(Backup) then
    if not FileCopy(Source,Backup,True) then RaiseException('Could not back up the existing ribbon manifest.');
  end;
 end;
 if CurStep=ssPostInstall then begin
  Root:=ExpandConstant('{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.Legacy.bundle\Contents\');
  if HasYear(2024) then if CompareText(GetSHA256OfFile(Root+'2024\MMTools.SectionBoxAlign.dll'),'{#Hash2024}')<>0 then RaiseException('2024 payload verification failed.');
  if HasYear(2025) then if CompareText(GetSHA256OfFile(Root+'2025\MMTools.SectionBoxAlign.dll'),'{#Hash2025}')<>0 then RaiseException('2025 payload verification failed.');
  if HasYear(2026) then if CompareText(GetSHA256OfFile(Root+'2026\MMTools.SectionBoxAlign.dll'),'{#Hash2026}')<>0 then RaiseException('2026 payload verification failed.');
  if HasYear(2027) then begin
   Source:=ExpandConstant('{userappdata}\Autodesk\ApplicationPlugins\MMTools.SectionBoxAlign.bundle\Contents\1.1.0\MMTools.SectionBoxAlign.dll');
   if CompareText(GetSHA256OfFile(Source),'{#Hash2027}')<>0 then RaiseException('2027 payload verification failed.');
   SaveStringToFile(ExpandConstant('{app}\RibbonInstalled.txt'),'2027',False);
  end;
  SaveStringToFile(ExpandConstant('{app}\DetectedVersions.txt'),AnsiString(DetectedText),False);
 end;
end;
function InitializeUninstall: Boolean;
var Reason: String;
begin
 Reason:=RunningReason;
 Result:=Reason='';
 if not Result then MsgBox(Reason,mbError,MB_OK);
end;
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var Target, Previous, Restore: String; Content: AnsiString;
begin
 if (CurUninstallStep=usUninstall) and FileExists(ExpandConstant('{app}\RibbonInstalled.txt')) then begin
  Target:=ExpandConstant('{userappdata}\Autodesk\ApplicationPlugins\MMTools.FloatingTools.bundle\PackageContents.xml');
  Previous:=ExpandConstant('{app}\PreviousRibbon.xml');
  Restore:=ExpandConstant('{app}\RestoreRibbon.xml');
  if FileExists(Target) then
   if CompareText(GetSHA256OfFile(Target),'{#RibbonHash}')=0 then begin
    if LoadStringFromFile(Previous,Content) then
     if Pos('Align1.1.0',String(Content))=0 then Restore:=Previous;
    if not FileCopy(Restore,Target,False) then RaiseException('Could not restore the previous ribbon.');
   end;
 end;
end;
