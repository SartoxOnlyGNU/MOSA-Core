To Enable VBE, Modify Run.bat Which in Properties Folder to
@echo off
start "" "C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" "" True
exit

To Disable VBE, Modify Run.bat Which in Properties Folder to
@echo off
start "" "C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" "" False
exit