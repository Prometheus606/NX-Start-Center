@echo off
setlocal

set "TOKEN=glpat-DEIN-TOKEN-HIER"

setx GITLAB_TOKEN "%TOKEN%" >nul

if errorlevel 1 (
    echo ERROR: GITLAB_TOKEN konnte nicht gesetzt werden.
    pause
    exit /b 1
)

echo.
echo GITLAB_TOKEN wurde erfolgreich gesetzt.
echo Die Variable steht neu gestarteten Programmen zur Verfuegung.
echo.
echo Diese Datei wird jetzt geloescht.

REM Eigenen Dateipfad merken
set "SELF=%~f0"

REM Separaten Prozess zum Loeschen starten
start "" /b cmd /c "timeout /t 2 /nobreak >nul & del /f /q ""%SELF%"""

exit /b 0