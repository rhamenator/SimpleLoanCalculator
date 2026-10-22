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
AppId={{8C28E7E6-A14E-4F97-A9AC-923C0F743A91}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Your FoxPro Developer
AppPublisherURL=https://github.com/rhamenator/SimpleLoanCalculator
AppSupportURL=https://github.com/rhamenator/SimpleLoanCalculator/issues
AppUpdatesURL=https://github.com/rhamenator/SimpleLoanCalculator/releases/latest
DefaultDirName={localappdata}\Programs\Exact Loan Calculator
PrivilegesRequired=lowest
MinVersion=10.0.17763
WizardStyle=modern
DisableProgramGroupPage=yes
SetupIconFile=..\..\src\SimpleLoanCalculator.App\Assets\AppIcon.ico
UninstallDisplayIcon={app}\Assets\AppIcon.ico
CloseApplications=yes
RestartApplications=no
DefaultGroupName={#AppName}
OutputDir=..\..\artifacts
OutputBaseFilename=ExactLoanCalculator-{#AppRuntime}-setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed={#AppArch}
ArchitecturesInstallIn64BitMode={#AppArch}

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Shortcuts:"; Flags: unchecked

[Files]
Source: "..\..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Exact Loan Calculator"; Filename: "{app}\SimpleLoanCalculator.App.exe"; WorkingDir: "{app}"; IconFilename: "{app}\Assets\AppIcon.ico"
Name: "{autodesktop}\Exact Loan Calculator"; Filename: "{app}\SimpleLoanCalculator.App.exe"; WorkingDir: "{app}"; IconFilename: "{app}\Assets\AppIcon.ico"; Tasks: desktopicon

[Run]
Filename: "{app}\SimpleLoanCalculator.App.exe"; Description: "Launch Exact Loan Calculator"; WorkingDir: "{app}"; Flags: nowait postinstall skipifsilent
