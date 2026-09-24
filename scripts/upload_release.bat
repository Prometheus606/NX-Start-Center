@echo off
rem setlocal

REM ------------------------------------------------
REM Configuration
REM ------------------------------------------------

set "GITLAB_URL=https://duh-group.gitlabhosting.com"
set "PROJECT_ID=DUH-Startcenter%%2FDUH-Startcenter"
set "PACKAGE_NAME=DUH_Startcenter-installer"

REM ------------------------------------------------
REM Get current Git tag
REM ------------------------------------------------

set "TAG="

for /f "delims=" %%i in ('git describe --tags --exact-match HEAD 2^>nul') do (
    set "TAG=%%i"
)

if "%TAG%"=="" (
    echo ERROR: Current commit has no Git tag.
    pause
    exit /b 1
)

REM ------------------------------------------------
REM Token check
REM ------------------------------------------------
if defined GITLAB_TOKEN (
    echo Token ist gesetzt
) else (
    echo Token ist NICHT gesetzt
    pause
    exit /b 1
)

REM ------------------------------------------------
REM EXE information
REM ------------------------------------------------

set "EXENAME=DUH_Startcenter-installer-%TAG%.exe"
set "EXE=..\output\installer\%EXENAME%"

echo.
echo Tag:     %TAG%
echo EXE:     %EXE%
echo Project: %PROJECT_ID%
echo.

if not exist "%EXE%" (
    echo ERROR: EXE does not exist:
    echo %EXE%
    pause
    exit /b 1
)

set /p "DESCRIPTION=Release-Beschreibung eingeben (Was ist neu): "
set "DESCRIPTION=%DESCRIPTION:\n=<br>%"

REM ------------------------------------------------
REM Upload EXE to Generic Package Registry
REM ------------------------------------------------

echo Uploading %EXENAME%...

curl --fail --location ^
  --header "PRIVATE-TOKEN: %GITLAB_TOKEN%" ^
  --upload-file "%EXE%" ^
  "%GITLAB_URL%/api/v4/projects/%PROJECT_ID%/packages/generic/%PACKAGE_NAME%/%TAG%/%EXENAME%"

if errorlevel 1 (
    echo.
    echo ERROR: Upload failed.
    pause
    exit /b 1
)

REM ------------------------------------------------
REM Create GitLab Release
REM ------------------------------------------------

echo.
echo Creating release %TAG%...

curl --fail ^
  --request POST ^
  --header "PRIVATE-TOKEN: %GITLAB_TOKEN%" ^
  --data-urlencode "tag_name=%TAG%" ^
  --data-urlencode "name=%TAG%" ^
  --data-urlencode "description=Was ist neu:<br><br>%DESCRIPTION%" ^
  "%GITLAB_URL%/api/v4/projects/%PROJECT_ID%/releases"

if errorlevel 1 (
    echo.
    echo ERROR: Creating release failed.
    pause
    exit /b 1
)

REM ------------------------------------------------
REM Add EXE as Release Asset
REM ------------------------------------------------

echo.
echo Adding release asset...

curl --fail ^
  --request POST ^
  --header "PRIVATE-TOKEN: %GITLAB_TOKEN%" ^
  --data-urlencode "name=%EXENAME%" ^
  --data-urlencode "url=%GITLAB_URL%/api/v4/projects/%PROJECT_ID%/packages/generic/%PACKAGE_NAME%/%TAG%/%EXENAME%" ^
  --data-urlencode "link_type=package" ^
  "%GITLAB_URL%/api/v4/projects/%PROJECT_ID%/releases/%TAG%/assets/links"

if errorlevel 1 (
    echo.
    echo ERROR: Adding release asset failed.
    pause
    exit /b 1
)

echo.
echo Release %TAG% created successfully.
pause

endlocal