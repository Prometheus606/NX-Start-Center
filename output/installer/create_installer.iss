; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{F5D57AD7-7B6A-4FBF-A776-F7548BAC26C6}
AppName=DUH_Startcenter
AppVersion=1.0.0
;AppVerName=NX Start Center 1.0.0
AppPublisher=Niklas Beitler
DefaultDirName={autopf}\DUH_Startcenter
DefaultGroupName=DUH_Startcenter
AllowNoIcons=yes
LicenseFile=..\..\LICENSE.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputBaseFilename=DUH_Startcenter-installer
SetupIconFile=..\..\src\images\duhGroup_Logo.ico
Password=6acb22090d42b4c234d00fe3e0f4e5db
Encryption=yes
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\..\output\exe\DUH_Startcenter.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\startbatch.bat"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\startbatch.py"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\USER_README.md"; DestDir: "{app}"; DestName: "README.md"; Flags: ignoreversion
Source: "..\..\user_settings.py"; DestDir: "{app}"; Flags: ignoreversion onlyifdoesntexist
Source: "..\..\src\*"; DestDir: "{app}\src"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\exe\NX_Startcenter.dist\certifi\*"; DestDir: "{app}\certifi"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
;
Source: "..\..\..\NX Open VSCode\*"; DestDir: "{app}\..\NX Open VSCode"; Flags: recursesubdirs createallsubdirs
Source: "..\..\..\MTK_deliverer\*"; DestDir: "{app}\..\MTK_deliverer"; Flags: recursesubdirs createallsubdirs
Source: "D:\Projekte\VS_Code_extensions\NX_Language\releases\*.vsix"; DestDir: "{tmp}"; Flags: ignoreversion
Source: "D:\Projekte\VS_Code_extensions\NX_Language\install.bat"; DestDir: "{tmp}"; Flags: ignoreversion

[Dirs]
Name: "{app}\..\NX Open VSCode"; Permissions: everyone-modify
Name: "{app}\..\MTK_deliverer"; Permissions: everyone-modify

[Icons]
Name: "{group}\DUH_Startcenter"; Filename: "{app}\DUH_Startcenter.exe"
Name: "{group}\{cm:UninstallProgram,DUH_Startcenter}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\DUH_Startcenter"; Filename: "{app}\DUH_Startcenter"; Tasks: desktopicon

[Run]
Filename: "{app}\DUH_Startcenter.exe"; Description: "{cm:LaunchProgram,DUH_Startcenter}"; Flags: nowait postinstall skipifsilent
Filename: "{tmp}\install.bat"; Description: "NX Language in VS Code installieren"; Flags: runhidden postinstall skipifsilent
