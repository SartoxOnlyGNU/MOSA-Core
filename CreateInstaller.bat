@echo off
echo Removing previous installer
del bin\MOSA-Installer.exe
cd Source\Inno-Setup-Script
create-installer.bat
pause