@echo off
setlocal

rem *****************************************************************************
rem 					     WICHTIGER HINWEIS!!
rem     Anpassungen an dieser Datei werden bei einem Update überschrieben!
rem *****************************************************************************

shift
set KUNDENNAME=%1
set NX_VERSION=%2
set UGII_LANG=%3
set KUNDENPFAD=%4
set NXPFAD=%5
set PPCHECK=%6
set INSTALLED_MACHINES_CHECK=%7
set TOOLCHECK=%8
set DEVICECHECK=%9
shift
set FEEDSPEEDCHECK=%9

rem *****************************************************************************
rem 	     			Einstellungen für lokale Computer      					*
rem *****************************************************************************

set UGII_BASE_DIR=%NXPFAD%\%NX_VERSION%

rem ***** UGII_ROOT_DIR wird nur bei bei NX-Versionen vor NX12 ben�tigt *****
rem set UGII_ROOT_DIR=C:\Siemens\%NX_VERSION%\UGII

rem ***** 5_Umgebung *****

set CX_SETTINGS_DIR=%KUNDENPFAD%\
set CX_PP_TOOLS=%~dp0..
set CX_CUSTOM_DIRS=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\UGII
set UGII_CUSTOM_DIRECTORY_FILE=%KUNDENPFAD%\KUNDENNAME%\5_Umgebung\%NX_VERSION%\UGII\custom_dirs.dat


rem *****************************************************************************
rem 	         Setzen der Variablen für Hans Weber Messzyklen								
rem *****************************************************************************
set CX_PROBE_APP=%KUNDENPFAD%\Hans_Weber\5_Umgebung\NX2206\CX_Probing\CX_Probing_Main.dll

rem *****************************************************************************
rem 						Setzen des Temp-Ordners								*
rem *****************************************************************************

set UGII_CAM_CSE_USER_DIR="%KUNDENPFAD%\%KUNDENNAME%\2_Testdaten\Temp\"


rem *****************************************************************************
rem 					Multi core cpu unterstuetzung							*
rem *****************************************************************************
set UGII_SMP_ENABLE=1

rem *****************************************************************************
rem 						Setzen des User-Ordners								*
rem 		***** Verwendung von eigenen Rollen, Loadoptions usw... *****		*
rem *****************************************************************************
set UGII_VENDOR_DIR=%KUNDENPFAD%\6_Custom\

rem ***** Ladeoptionen *****
set UGII_LOAD_OPTIONS=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\

rem *****************************************************************************
rem 					CAM - Machordner mit Standardstruktur					*
rem *****************************************************************************

rem ***** Library *****
set UGII_CAM_LIBRARY_MACHINE_DIR=%NXPFAD%\%NX_VERSION%\MACH\resource\library\machine\
set UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR=%NXPFAD%\%NX_VERSION%\MACH\resource\library\machine\
set UGII_CAM_POST_CONFIG_FILE=%NXPFAD%\%NX_VERSION%\mach\resource\postprocessor\template_post.dat
set UGII_CAM_POST_DIR=%NXPFAD%\%NX_VERSION%\mach\resource\postprocessor\
set UGII_CAM_TOOL_PATH_DIR=%NXPFAD%\%NX_VERSION%\mach\resource\tool_path\
set UGII_CAM_LIBRARY_TOOL_DIR=%NXPFAD%\%NX_VERSION%\MACH\resource\library\tool\
set UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR=%NXPFAD%\%NX_VERSION%\MACH\resource\library\feeds_speeds\
set UGII_CAM_LIBRARY_DEVICE_DIR=%NXPFAD%\%NX_VERSION%\MACH\resource\library\device\

if /i %PPCHECK% == 1 (
	set UGII_CAM_POST_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\postprocessor\
	set UGII_CAM_USER_DEF_EVENT_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\user_def_event\
)

if /i %INSTALLED_MACHINES_CHECK% == 1 (
	set UGII_CAM_LIBRARY_MACHINE_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\library\machine\
	set UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\library\machine\installed_machines\
)

if /i %TOOLCHECK% == 1 (
    set UGII_CAM_LIBRARY_TOOL_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\library\tool\
)

if /i %FEEDSPEEDCHECK% == 1 (
    set UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\library\feeds_speeds\
)

if /i %DEVICECHECK% == 1 (
    set UGII_CAM_LIBRARY_DEVICE_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\library\device\
)

rem ***** PP *****
rem set UGII_CAM_POSTUGII_VENDOR_DIR_DIR=%CX_CAM_SETTINGS_DIR%\MACH\postprocessor\
rem set UGII_CAM_USER_DEF_EVENT_DIR=%CX_CAM_SETTINGS_DIR%\MACH\user_def_event\

rem **** MKE **** 
rem set UGII_CAM_MACHINING_KNOWLEDGE_DIR=%~dp0\machining_knowledge\

rem **** Verwaltung der CAM-Vorlagen ****
rem set UGII_CAM_TEMPLATE_SET_DIR=%CX_CAM_SETTINGS_DIR%\MACH\template_set\
rem set UGII_CAM_LIBRARY_TEMPLATE_DIR=%CX_CAM_SETTINGS_DIR%\MACH\template_dir\
rem set UGII_CAM_LIBRARY_TEMPLATE_DATA_DIR=%CX_CAM_SETTINGS_DIR%\MACH\template_dir\

if not exist "%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\template_part\" GOTO end_template
set UGII_CAM_TEMPLATE_PART_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\template_part\
set UGII_CAM_TEMPLATE_PART_METRIC_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\template_part\metric\
set UGII_TEMPLATE_DIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\MACH\resource\usertools\templates\

rem set UGII_CAM_CONFIG_DIR=%CX_CAM_SETTINGS_DIR%\MACH\configuration\
rem set UGII_CAM_CONFIG=%CX_CAM_SETTINGS_DIR%\MACH\%KUNDENNAME%.dat
rem set UGII_CAM_DEFAULT_TYPE_TEMPLATE=%CX_CAM_SETTINGS_DIR%\MACH\template\%KUNDENNAME%.opt
rem set UGII_BITMAP_PATH=%CX_CAM_SETTINGS_DIR%\MACH\Bitmap\
:end_template

rem ------------------------------------------------------------------------------
rem - Anpassungen fuer CAD\CAM Umgebung
rem ------------------------------------------------------------------------------
rem Variable -CX_DLL_SYSDIR wird in der Datei "d:\splmshare\nx120_1872\win64\ugii\as_tools\startup\ribbon_tabs.rtb" verwendet
rem Variable -UGII_USER_DIR wird f�r das starten der CAMeasy Tools im Men� zu integieren ben�tigt
if exist "%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\UGII\cx_tools\" (
    set CX_DLL_SYSDIR=%KUNDENPFAD%\%KUNDENNAME%\5_Umgebung\%NX_VERSION%\UGII\cx_tools
)

rem NX title in Leiste schreiben und conmatix tab erstellen
set startup_path=%CX_CUSTOM_DIRS%\startup
if not exist "%startup_path%" (
    md "%startup_path%"
)
if not exist "%CX_CUSTOM_DIRS%\custom_dirs.dat" (
     copy "%~dp0src\NX_UI\custom_dirs.dat" "%CX_CUSTOM_DIRS%\custom_dirs.dat"
)
copy "%~dp0src\NX_UI\CX_PP_tools.rtb" "%startup_path%" /Y
copy "%~dp0src\NX_UI\nxtitel_configgroup.men" "%startup_path%" /Y
rem in  "nxtitel_configgroup.men" letzte Zeile schreiben
echo TITLE CAM - %NX_VERSION% >> "%startup_path%\"nxtitel_configgroup.men"

rem nur bei Externer Simulation verwendbar
set UGII_CAM_IPW_SNAPSHOT=1

rem *****************************************************************************
rem 								Start NX
rem *****************************************************************************
set result="FALSE"
if /i "%NX_VERSION%" == "NX100" set result="TRUE"
if /i "%NX_VERSION%" == "NX110" set result="TRUE"
if /i "%NX_VERSION%" == "NX120" set result="TRUE"
if /i "%NX_VERSION%" == "NX1872" set result="TRUE"
if /i "%NX_VERSION%" == "NX1892" set result="TRUE"


if /i %result% == "TRUE" goto old_version

start %NXPFAD%\%NX_VERSION%\nxbin\ugraf.exe"
goto end

:old_version
start %NXPFAD%\%NX_VERSION%\UGII\ugraf.exe
:end

exit
