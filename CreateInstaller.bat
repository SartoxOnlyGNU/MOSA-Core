@echo off
echo Removing Previous Installer...
del bin\MOSA-Installer.exe /q
cd Source\Inno-Setup-Script
create-installer.bat
pause