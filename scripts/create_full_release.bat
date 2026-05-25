@echo off
setlocal EnableDelayedExpansion

rem ============================================
rem Basisverzeichnis bestimmen
rem ============================================

set "BASE=%~dp0.."
for %%i in ("%BASE%") do set "BASE=%%~fi"

cd /d "%BASE%" || exit /b 1

echo ============================================
echo DUH_Startcenter Build
echo ============================================

rem ============================================
rem Letzten Git-Tag von main lesen
rem ============================================

set "GIT_TAG="

for /f "usebackq delims=" %%v in (`git describe --tags --abbrev^=0 main`) do (
    set "GIT_TAG=%%v"
)

if not defined GIT_TAG (
    echo Konnte keinen Git-Tag auf main finden.
    pause
    exit /b 1
)

echo Git Version: !GIT_TAG!

rem ============================================
rem Version fuer verschiedene Zwecke aufbereiten
rem ============================================

rem Fuer C# Datei -> V1.5.4
set "VERSION_WITH_V=!GIT_TAG!"

rem Fuer Dateiname -> 1.5.4
set "VERSION_NUMBER=!GIT_TAG!"
set "VERSION_NUMBER=!VERSION_NUMBER:V=!"
set "VERSION_NUMBER=!VERSION_NUMBER:v=!"

echo Versionsnummer: !VERSION_NUMBER!

rem ============================================
rem Version in test.cs schreiben
rem ============================================

set "VERSION_FILE=%BASE%\duh_startcenter\AppInfo.cs"

if not exist "!VERSION_FILE!" (
    echo Datei nicht gefunden:
    echo !VERSION_FILE!
    pause
    exit /b 1
)

echo Schreibe Version in AppInfo.cs ...

powershell -NoProfile -ExecutionPolicy Bypass ^
"$file = '!VERSION_FILE!'; ^
$version = '!VERSION_WITH_V!'; ^
$content = Get-Content $file; ^
$content = $content -replace 'Version = \".*\";', ('Version = \"' + $version + '\";'); ^
Set-Content $file $content"

if errorlevel 1 (
    echo Fehler beim Schreiben der Version.
    pause
    exit /b 1
)

rem ============================================
rem Alte Installer-Datei entfernen
rem ============================================

taskkill /f /im DUH_Startcenter-installer.exe >nul 2>&1

del /f /q "%BASE%\output\installer\DUH_Startcenter-installer.exe" >nul 2>&1

timeout /t 2 /nobreak >nul

rem ============================================
rem EXE erstellen
rem ============================================

echo.
echo ============================================
echo Erstelle EXE
echo ============================================

call "%BASE%\scripts\create_exe.bat"

if errorlevel 1 (
    echo EXE Build fehlgeschlagen.
    pause
    rem exit /b 1
)

rem ============================================
rem Installer erstellen
rem ============================================

echo.
echo ============================================
echo Erstelle Installer
echo ============================================

call "%BASE%\scripts\create_installer.bat"

if errorlevel 1 (
    echo Installer Build fehlgeschlagen.
    pause
    rem exit /b 1
)

rem ============================================
rem Installer umbenennen
rem ============================================

set "INSTALLER_DIR=%BASE%\output\installer"

set "OLD_INSTALLER=%INSTALLER_DIR%\DUH_Startcenter-installer.exe"

set "NEW_INSTALLER=%INSTALLER_DIR%\DUH_Startcenter-installer-!VERSION_NUMBER!.exe"

if not exist "!OLD_INSTALLER!" (
    echo Installer wurde nicht gefunden:
    echo !OLD_INSTALLER!
    pause
    exit /b 1
)

if exist "!NEW_INSTALLER!" (
    del /f /q "!NEW_INSTALLER!"
)

ren "!OLD_INSTALLER!" "DUH_Startcenter-installer-!VERSION_NUMBER!.exe"

if errorlevel 1 (
    echo Fehler beim Umbenennen des Installers.
    pause
    exit /b 1
)

echo.
echo ============================================
echo Fertig
echo ============================================

echo Installer:
echo !NEW_INSTALLER!

pause
endlocal
exit /b 0