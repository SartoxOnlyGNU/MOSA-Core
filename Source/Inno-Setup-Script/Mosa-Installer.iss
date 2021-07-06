; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "MOSA-Project"
#define MyAppPublisher "MOSA-Project"
#define MyAppURL "http://www.mosa-project.org"
#define MyAppVersion "20210703"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{516E4655-F79C-44AC-AA6D-D9A879450A64}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf}\{#MyAppName}
DisableDirPage=yes
DisableReadyPage=yes
DefaultGroupName={#MyAppName}
OutputDir=..\..\bin
OutputBaseFilename=MOSA-Installer-{#MyAppVersion}
SolidCompression=yes
MinVersion=0,6.0
AllowUNCPath=False
Compression=lzma2/ultra64
InternalCompressLevel=ultra64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]

[Run]

[Dirs]
Name: "{app}\Tools"

[Files]
Source: "..\..\*.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\*.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\*.bat"; DestDir: "{app}"; Flags: ignoreversion

;Visual Studio Extension
Source: "..\Mosa.VisualStudio.ProjectTemplate\Boot.cs"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\MOSA.csproj"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\MyTemplate.vstemplate"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\Program.cs"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\README.txt"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\__TemplateIcon.ico"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\Properties\launchSettings.json"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project\Properties"; Flags: ignoreversion
Source: "..\Mosa.VisualStudio.ProjectTemplate\Properties\Run.bat"; DestDir: "{userdocs}\Visual Studio 2019\Templates\ProjectTemplates\Mosa Project\Properties"; Flags: ignoreversion

   
Source: "..\..\bin\*.*"; DestDir: "{app}\bin"; Flags: ignoreversion


;Tools
Source: "..\..\Tools\nasm\*.*"; DestDir: "{app}\Tools\nasm"; Flags: ignoreversion
Source: "..\..\Tools\ndisasm\*.*"; DestDir: "{app}\Tools\ndisasm"; Flags: ignoreversion
Source: "..\..\Tools\mkisofs\*.*"; DestDir: "{app}\Tools\mkisofs"; Flags: ignoreversion
Source: "..\..\Tools\syslinux\*.*"; DestDir: "{app}\Tools\syslinux"; Flags: ignoreversion
Source: "..\..\Tools\vmware\*.*"; DestDir: "{app}\Tools\vmware"; Flags: ignoreversion

[ThirdParty]
UseRelativePaths=True
