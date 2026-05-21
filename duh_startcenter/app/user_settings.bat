@echo off
if "%DEBUG%" == "1" echo on
if "%DEBUG%" == "1" Pause
REM # *****************************************************************************
REM # 					        WICHTIGER HINWEIS!!
REM #
REM #     Änderungen die nicht überschrieben werden sollen, bitte in dieser Datei vornehmen
REM #
REM  # *****************************************************************************


REM ------------------------------------------------------------------------------
REM Kundenabhängige Umgebungsvariablen setzen
REM ------------------------------------------------------------------------------
if "%CUSTOMERNAME%"=="Hans_Weber" (
    set "CX_PROBE_APP=%KUNDENPFAD%\Hans_Weber\5_Umgebung\NX2206\CX_Probing\CX_Probing_Main.dll"
)

if "%CUSTOMERNAME%"=="Mercedes" (
    set "DAIMLER_LIBRARY_INSTALLED_MACHINES_DIR=%UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR%"
)

if "%CUSTOMERNAME%"=="Steinway" (
    set "UGII_BITMAP_PATH=%UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR%\ELHA_PKM_AB\Bitmap"
)

if "%CUSTOMERNAME%"=="Jokey" (
    set "UGII_CAM_CSE_USER_DIR="
)

if "%CUSTOMERNAME%"=="WuH" (
    set "UGII_CAM_CSE_USER_DIR=C:\temp\dmc340fd_4341_VNCK\cse_prog_projects"
    set "UGII_NCD_DIR=C:\temp\"
)

if "%DEBUG%" == "1" Pause