To Enable VBE, Modify Run.bat to
start "" "C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" "" True
exit

To Disable VBE, Modify Run.bat to
start "" "C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" "" False
exit