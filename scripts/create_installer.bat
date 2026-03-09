@echo off
setlocal

rem === Basis Ordner (duh_startcenter) ===
set "BASE=%~dp0..\"
for %%i in ("%BASE%") do set "BASE=%%~fi"
cd /d "%BASE%"

rem === PyInstaller Build ===
@echo off
set "ISCC=C:\Users\niklas.beitler\AppData\Local\Programs\Inno Setup 6\ISCC.exe"
set "SCRIPT=%~dp0create_installer.iss"

"%ISCC%" "%SCRIPT%"

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign /sha1 "25b8123c7ea491f66dc913e37d668a7297f35ab6" /t http://time.certum.pl /fd SHA256 /v %BASE%output\installer\DUH_Startcenter-installer.exe

pause
endloca
exit /b