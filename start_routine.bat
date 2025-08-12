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

REM ------------------------------------------------------------------------------
REM Folder structure in Customers ENV
REM ------------------------------------------------------------------------------

set ENV_FOLDER_NAME=5_Umgebung
set CUSTOMER_DIR=%KUNDENPFAD%\%KUNDENNAME%\
set ENV_DIR=%CUSTOMER_DIR%%ENV_FOLDER_NAME%
set VERSION_DIR=%ENV_DIR%\%NX_VERSION%\
set UGII_DIR=%VERSION_DIR%UGII\
set MACH_DIR=%VERSION_DIR%MACH\
set RESOURCE_DIR=%MACH_DIR%resource\
set LIBRARY_DIR=%RESOURCE_DIR%library\
set MACHINE_DIR=%LIBRARY_DIR%machine\
set INSTALLED_MACHINES_DIR=%MACHINE_DIR%installed_machines\

set NATIVE_BASE_DIR=%NXPFAD%\%NX_VERSION%\
set NATIVE_UGII_DIR=%NATIVE_BASE_DIR%UGII\
set NATIVE_MACH_DIR=%NATIVE_UGII_DIR%MACH\
set NATIVE_RESOURCE_DIR=%NATIVE_MACH_DIR%resource\
set NATIVE_LIBRARY_DIR=%NATIVE_RESOURCE_DIR%library\
set NATIVE_MACHINE_DIR=%NATIVE_LIBRARY_DIR%machine\
set NATIVE_INSTALLED_MACHINES_DIR=%NATIVE_MACHINE_DIR%installed_machines\

rem *****************************************************************************
rem 	     			Einstellungen für lokale Computer      					*
rem *****************************************************************************

set UGII_BASE_DIR=%NATIVE_BASE_DIR%

rem ***** UGII_ROOT_DIR wird nur bei bei NX-Versionen vor NX12 ben�tigt *****
rem set UGII_ROOT_DIR=C:\Siemens\%NX_VERSION%\UGII

rem ***** 5_Umgebung *****
set CX_PP_TOOLS=%~dp0..
set CX_CUSTOM_DIRS=%UGII_DIR%
set UGII_CUSTOM_DIRECTORY_FILE=%UGII_DIR%custom_dirs.dat

rem *****************************************************************************
rem 						Setzen des Temp-Ordners								*
rem *****************************************************************************

set UGII_CAM_CSE_USER_DIR=%CUSTOMER_DIR%2_Testdaten\Temp\


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
set UGII_LOAD_OPTIONS=%MACH_DIR%resource\
rem *****************************************************************************
rem 					CAM - Machordner mit Standardstruktur					*
rem *****************************************************************************

rem ***** Library *****
set UGII_CAM_LIBRARY_MACHINE_DIR=%NATIVE_MACHINE_DIR%
set UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR=%NATIVE_INSTALLED_MACHINES_DIR%
set UGII_CAM_POST_CONFIG_FILE=%NATIVE_RESOURCE_DIR%postprocessor\template_post.dat
set UGII_CAM_POST_DIR=%NATIVE_RESOURCE_DIR%postprocessor\
set UGII_CAM_TOOL_PATH_DIR=%NATIVE_RESOURCE_DIR%tool_path\
set UGII_CAM_LIBRARY_TOOL_DIR=%NATIVE_LIBRARY_DIR%tool\
set UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR=%NATIVE_LIBRARY_DIR%feeds_speeds\
set UGII_CAM_LIBRARY_DEVICE_DIR=%NATIVE_LIBRARY_DIR%device\

if /i "%PPCHECK%" == "1" (
	set UGII_CAM_POST_DIR=%RESOURCE_DIR%postprocessor\
	set UGII_CAM_USER_DEF_EVENT_DIR=%RESOURCE_DIR%user_def_event\
)

if /i "%INSTALLED_MACHINES_CHECK%" == "1" (
	set UGII_CAM_LIBRARY_MACHINE_DIR=%MACHINE_DIR%
	set UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR=%INSTALLED_MACHINES_DIR%
)

if /i "%TOOLCHECK%" == "1" (
    set UGII_CAM_LIBRARY_TOOL_DIR=%LIBRARY_DIR%tool\
)

if /i "%FEEDSPEEDCHECK%" == "1" (
    set UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR=%LIBRARY_DIR%\feeds_speeds\
)

if /i "%DEVICECHECK%" == "1" (
    set UGII_CAM_LIBRARY_DEVICE_DIR=%LIBRARY_DIR%device\
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

if not exist "%RESOURCE_DIR%template_part\" GOTO end_template
set UGII_CAM_TEMPLATE_PART_DIR=%RESOURCE_DIR%template_part\
set UGII_CAM_TEMPLATE_PART_METRIC_DIR=%RESOURCE_DIR%template_part\metric\
set UGII_TEMPLATE_DIR=%RESOURCE_DIR%usertools\templates\

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
if exist "%UGII_DIR%cx_tools\" (
    set CX_DLL_SYSDIR=%UGII_DIR%cx_tools\
)

rem NX title in Leiste schreiben und conmatix tab erstellen
set startup_path=%CX_CUSTOM_DIRS%\startup
set nxtitle_file=src\NX_UI\nxtitel_configgroup.men
set pp_tools_file=src\NX_UI\CX_PP_tools.rtb
set custom_dirs_file=src\NX_UI\custom_dirs.dat
if not exist "%startup_path%" (
    md "%startup_path%"
)
if not exist "%CX_CUSTOM_DIRS%\custom_dirs.dat" (
     copy "%~dp0%custom_dirs_file%" "%CX_CUSTOM_DIRS%\custom_dirs.dat"
)
copy "%~dp0%pp_tools_file%" "%startup_path%" /Y
copy "%~dp0%nxtitle_file%" "%startup_path%" /Y
rem in  "nxtitel_configgroup.men" letzte Zeile schreiben
echo TITLE CAM - %NX_VERSION% %KUNDENNAME% >> "%startup_path%\"nxtitel_configgroup.men"

rem nur bei Externer Simulation verwendbar
set UGII_CAM_IPW_SNAPSHOT=1

rem *****************************************************************************
rem 						Read CAM Batch file (custom_nx.bat)
rem *****************************************************************************
if exist %ENV_DIR%\%NX_VERSION%\start_apps\custom_nx.bat (
    set SPLM_SHR_DIR=%ENV_DIR%
    set PLM_SHARE_DUH=%ENV_DIR%
    set NX_Version_DUH=%NX_VERSION%
    set NX_SHR_VERSION_DIR=%NX_VERSION%
    set KUNDE_DUH=%KUNDENNAME%
    set SIDT_PAR2="nx"
    call %ENV_DIR%\%NX_VERSION%\start_apps\custom_nx.bat
)

    set "SCRIPT_DIR=%~dp0"
REM ------------------------------------------------------------------------------
REM Custom Batch lesen
REM ------------------------------------------------------------------------------
if exist "%SCRIPT_DIR%user_settings.bat" (
    call "%SCRIPT_DIR%user_settings.bat"
)

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
