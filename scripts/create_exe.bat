@echo off
setlocal

rem === Basis Ordner (duh_startcenter) ===
set "BASE=%~dp0..\"
for %%i in ("%BASE%") do set "BASE=%%~fi"
cd /d "%BASE%"

rem === venv aktivieren ===
if exist "%BASE%venv\Scripts\activate.bat" (
  call "%BASE%venv\Scripts\activate.bat"
) else (
  echo [WARN] Keine venv gefunden unter "%BASE%venv". Nutze System-Python.
)

rem === PyInstaller Build ===
pyinstaller --noconfirm --onefile --windowed --name "DUH_Startcenter" ^
  --icon "%BASE%resources\images\duhGroup_Logo.ico" ^
  --add-data "%BASE%app\controller;controller/" ^
  --add-data "%BASE%app\model;model/" ^
  --add-data "%BASE%app\view;view/" ^
  --add-data "%BASE%app\env.py;." ^
  --collect-all tkinter ^
  --collect-all ttkbootstrap ^
  --collect-all ttkcreator ^
  --collect-all PIL ^
  --hidden-import "PIL._imagingft" --hidden-import "PIL._imaging" ^
  --distpath "%BASE%output\exe" ^
  --workpath "%BASE%output\build" ^
  --specpath "%BASE%output\spec" ^
  "%BASE%app\NX_Startcenter.py"
  
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign /sha1 "25b8123c7ea491f66dc913e37d668a7297f35ab6" /t http://time.certum.pl /fd SHA256 /v %BASE%output\exe\DUH_Startcenter.exe

pause
endlocal
exit /b