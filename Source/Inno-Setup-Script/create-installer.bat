@echo off
echo Before create installer you need to install Inno-Setup and Build the Mosa.sln. Are you done? If so press any key to continue!
pause

rem set INNOSETUP=C:\Program Files (x86)\Inno Setup 5\ISCC.exe
set INNOSETUP=C:\Program Files (x86)\Inno Setup 6\ISCC.exe

if exist "%INNOSETUP%" "%INNOSETUP%" /DMyAppVersion=2.0.0 Mosa-Installer.iss

start "" ..\..\bin\MOSA-Installer.exe