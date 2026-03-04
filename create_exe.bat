@echo off
setlocal

rem === Basis: Ordner der BAT (duh_startcenter) ===
set "BASE=%~dp0"
cd /d "%BASE%"

rem === (Optional, aber empfohlen) venv aktivieren ===
if exist "%BASE%venv\Scripts\activate.bat" (
  call "%BASE%venv\Scripts\activate.bat"
) else (
  echo [WARN] Keine venv gefunden unter "%BASE%venv". Nutze System-Python.
)

rem === PyInstaller Build ===
pyinstaller --noconfirm --onefile --windowed --name "DUH_Startcenter" ^
  --icon "%BASE%src\images\duhGroup_Logo.ico" ^
  --add-data "%BASE%controller;controller/" ^
  --add-data "%BASE%model;model/" ^
  --add-data "%BASE%view;view/" ^
  --add-data "%BASE%env.py;." ^
  --collect-all tkinter ^
  --collect-all ttkbootstrap ^
  --collect-all ttkcreator ^
  --collect-all PIL ^
  --hidden-import "PIL._imagingft" --hidden-import "PIL._imaging" ^
  --distpath "%BASE%output\exe" ^
  --workpath "%BASE%output\build" ^
  --specpath "%BASE%output\spec" ^
  "%BASE%NX_Startcenter.py"

pause
endlocal
exit /b