@echo off
setlocal

rem === Basis Ordner: duh_startcenter ===
set "BASE=%~dp0.."
for %%i in ("%BASE%") do set "BASE=%%~fi"

cd /d "%BASE%" || exit /b 1

dotnet publish "%BASE%\duh_startcenter\duh_startcenter.csproj" ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  /p:PublishSingleFile=true ^
  /p:IncludeNativeLibrariesForSelfExtract=true ^
  /p:PublishDir="%BASE%\output\exe\"

if errorlevel 1 (
  echo Publish fehlgeschlagen.
  pause
  exit /b 1
)

xcopy "%BASE%\duh_startcenter\resources\AxelsPunkt.exe" "%BASE%\output\exe\resources\" /Y /I

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign ^
  /sha1 "25b8123c7ea491f66dc913e37d668a7297f35ab6" ^
  /t http://time.certum.pl ^
  /fd SHA256 ^
  /v "%BASE%\output\exe\DUH_Startcenter.exe"

if errorlevel 1 (
  echo Signieren fehlgeschlagen.
  pause
  exit /b 1
)

echo Successfull created executable under output\exe\.
pause
endlocal
exit /b 0