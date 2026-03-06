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

pause
endlocal
exit /b