#define AppName "Exact Loan Calculator"
#ifndef AppVersion
  #define AppVersion "0.1.0"
#endif
#ifndef AppRuntime
  #define AppRuntime "win-x64"
#endif
#ifndef AppArch
  #define AppArch "x64compatible"
#endif

[Setup]
UninstallDisplayIcon={app}\app-icon.ico
SetupIconFile=app-icon.ico
AppId={{8C28E7E6-A14E-4F97-A9AC-923C0F743A91}
AppName={#AppName}
AppVersion={#AppVersion}
DefaultDirName={autopf}\Exact Loan Calculator
DefaultGroupName={#AppName}
OutputDir=..\..\artifacts
OutputBaseFilename=ExactLoanCalculator-{#AppVersion}-{#AppRuntime}-setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed={#AppArch}
ArchitecturesInstallIn64BitMode={#AppArch}

[Files]
Source: "app-icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Exact Loan Calculator"; Filename: "{app}\SimpleLoanCalculator.App.exe"; IconFilename: "{app}\app-icon.ico"
