@echo off
REM # *****************************************************************************
REM # 					        WICHTIGER HINWEIS!!
REM #
REM #     Änderungen die nicht überschrieben werden sollen, bitte in dieser Datei vornehmen
REM #
REM  # *****************************************************************************


REM ------------------------------------------------------------------------------
REM Kundenabhängige Umgebungsvariablen setzen
REM ------------------------------------------------------------------------------
if "%KUNDENNAME%"=="Hans_Weber" (
    set "CX_PROBE_APP=%KUNDENPFAD%\Hans_Weber\5_Umgebung\NX2206\CX_Probing\CX_Probing_Main.dll"
)

if "%KUNDENNAME%"=="Mercedes" (
    set "DAIMLER_LIBRARY_INSTALLED_MACHINES_DIR=%UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR%"
)

if "%KUNDENNAME%"=="Steinway" (
    set "UGII_BITMAP_PATH=%UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR%\ELHA_PKM_AB\Bitmap"
)

if "%KUNDENNAME%"=="WuH" (
    REM WuH Settings
    set "UGII_CAM_CSE_USER_DIR=C:\temp\dmc340fd_4341_VNCK\cse_prog_projects"
    set "UGII_NCD_DIR=C:\temp\"
) else (
    set "UGII_CAM_CSE_USER_DIR=%KUNDENPFAD%\%KUNDENNAME%\2_Testdaten\Temp\"
)