@echo off
rem *****************************************************************************
rem 					     WICHTIGER HINWEIS!!
rem     Anpassungen an dieser Datei werden bei einem Update überschrieben!
rem *****************************************************************************


set "DEBUG=%~6"
if "%DEBUG%" == "1" echo on

set "CUSTOMERNAME=%~1"
set "NX_Version_DUH=%~2"
set "UGII_LANG=%~3"
set "CUSTOMER_PATH=%~4"
set "NX_INSTALL_PATH=%~5"
set "PPCHECK=%~7"
set "INSTALLED_MACHINES_CHECK=%~8"
set "TOOLCHECK=%~9"

shift
shift
shift
shift
shift
shift
shift
shift
shift

set "DEVICECHECK=%~1"
set "FEEDSPEEDCHECK=%~2"
set "CLOUD_LICENSE=%~3"
set "MANAGED=%~4"
set "VORLAGE_ROOT=%~5"
set "TC_PFAD=%~6"

set "SCRIPT_DIR=%~dp0"

rem ------------------------------------------------------------------------------
rem Umgebungsvariablen setzen
rem ------------------------------------------------------------------------------
	set UGII_BASE_DIR=%NX_INSTALL_PATH%\%NX_Version_DUH%
	set PLM_SHARE_DUH=%CUSTOMER_PATH%
	set UMGEBUNG=5_Umgebung
	set UGII_USER_DIR=%APPDATA%\Siemens\%NX_Version_DUH%\ConfigSettings
	set UGII_CAM_RESOURCE_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\
	set UGII_CAM_CSE_USER_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Temp\
	set UGII_LOAD_OPTIONS=%PLM_SHARE_DUH%\Vorlage\load_options\load_options.def
	set UGII_CAM_CUSTOM_DIR=UGII_CAM_RESOURCE_DIR
	set DUH_ToolBars_DIR=%VORLAGE_ROOT%\ToolBars\DUH_Group	
	set Siemens_ToolBars_DIR=%VORLAGE_ROOT%\ToolBars\Siemens
	set SPLM_SHR_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%
	set NX_SHR_VERSION_DIR=%NX_Version_DUH%



if "CLOUD_LICENSE" == "1" (
	set SPLM_LICENSE_SERVER=CLOUD
) 
	
REM DUH Pause %NX_Version_DUH%.bat 2
if "%DEBUG%" == "1" Pause
rem ------------------------------------------------------------------------------
	set UGII_TMP_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\temp\NX

rem ------------------------------------------------------------------------------

REM rem Kunden Variablen setzen
rem ------------------------------------------------------------------------------
	if EXIST %UGII_CAM_RESOURCE_DIR%configuration\%CUSTOMERNAME%.dat set UGII_CAM_CONFIG=%UGII_CAM_RESOURCE_DIR%configuration\%CUSTOMERNAME%.dat
	if EXIST %UGII_CAM_RESOURCE_DIR%library\device\%CUSTOMERNAME%\ set UGII_CAM_LIBRARY_DEVICE_DATA_DIR=%UGII_CAM_RESOURCE_DIR%library\device\%CUSTOMERNAME%\
	if EXIST %UGII_CAM_RESOURCE_DIR%library\feeds_speeds\%CUSTOMERNAME%\ set UGII_CAM_LIBRARY_FEEDS_SPEEDS_DATA_DIR=%UGII_CAM_RESOURCE_DIR%library\feeds_speeds\%CUSTOMERNAME%\
	if EXIST %UGII_CAM_RESOURCE_DIR%library\machine\%CUSTOMERNAME%\ set UGII_CAM_LIBRARY_MACHINE_DATA_DIR=%UGII_CAM_RESOURCE_DIR%library\machine\%CUSTOMERNAME%\
	if EXIST %UGII_CAM_RESOURCE_DIR%library\tool\%CUSTOMERNAME%\ set UGII_CAM_LIBRARY_TOOL_METRIC_DIR=%UGII_CAM_RESOURCE_DIR%library\tool\%CUSTOMERNAME%\
rem ------------------------------------------------------------------------------


rem duh_tools_DIR

	if EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\cx_tools rd /s /q  %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\cx_tools
 

rem ------------------------------------------------------------------------------

	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\custom_dirs.dat (

		copy %VORLAGE_ROOT%\Vorlage\custom_dirs.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\

	)

	set UGII_CUSTOM_DIRECTORY_FILE=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\custom_dirs.dat
	
	if EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\ug_custom_dirs.dat set UGII_UG_CUSTOM_DIRECTORY_FILE=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\ug_custom_dirs.dat
	if EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\ugii_env_switch.dat set UGII_ENV_FILE=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\ugii_env_switch.dat
rem ------------------------------------------------------------------------------

rem ugii_env.dat
rem ------------------------------------------------------------------------------
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII\ugii_env.dat (

		copy %VORLAGE_ROOT%\Vorlage\ugii_env.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII
	)

	set UGII_ENV_FILE=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\ugii_env.dat
rem ------------------------------------------------------------------------------


rem customer_defaults_Site
rem ------------------------------------------------------------------------------
	if EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\customer_defaults rd /s /q %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\customer_defaults

	if EXIST %LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.dpv del %LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.dpv
	if EXIST %LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.xsl del %LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.xsl
	

	set UGII_SITE_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Site
	set UGII_GROUP_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Group
	set UGII_USER_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User
	set UGII_LOCAL_USER_DEFAULTS=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup\nx_user.dpv
rem ------------------------------------------------------------------------------


rem Provide .men file for NX app title

	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup\nxtitel_configgroup.men" (
	copy %VORLAGE_ROOT%\Vorlage\usertools\startup\nxtitel_configgroup.men %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup
	echo TITLE CAM - %CUSTOMERNAME% %NX_Version_DUH% >> %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup\nxtitel_configgroup.men
	)



rem Frühzugriffsfunktion
rem ------------------------------------------------------------------------------
	set UGII_LOCAL_USER_TOGGLE_DEFAULTS=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\EarlyAccessFeature\feature_toggle_user.fcg

rem ------------------------------------------------------------------------------


rem set nxtools environment
rem ------------------------------------------------------------------------------
	set NXTOOLS_SYSDIR=%VORLAGE_ROOT%\ToolBars\NX_tools\%NX_Version_DUH%\NXTools
	set NXTOOLS_LOOK_AHEAD=true
	set UGII_USER_TOOLS_FILE=%NXTOOLS_SYSDIR%\usertools\usertools.utd
	set UGII_USER_TOOLS_MENU=%NXTOOLS_SYSDIR%\usertools\usertools.utm
	set UGII_USER_TOOLS_BITMAP_PATH=%NXTOOLS_SYSDIR%\usertools\bitmaps
rem ------------------------------------------------------------------------------



REM DUH Pause %NX_Version_DUH%.bat 1
if "%DEBUG%" == "1" (	
	echo %PLM_SHARE_DUH%
	echo %CUSTOMERNAME%
	echo %UMGEBUNG%
	Pause
)
rem ------------------------------------------------------------------------------

rem custom_nx.bat
rem zum Variablen setzen für jeden Kunden 
rem costom_nx.bat unter %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps  
rem ------------------------------------------------------------------------------
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat (

echo	D| xcopy %VORLAGE_ROOT%\Vorlage\start_apps\custom_nx.bat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat  /d /Y
	)

REM DUH Pause %NX_Version_DUH%.bat 2
	if exist %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat (
		call %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat
	) else (
		echo custom_nx nicht gefunden
	)
rem ------------------------------------------------------------------------------



REM ------------------------------------------------------------------------------
REM Custom Batch lesen
REM ------------------------------------------------------------------------------
if exist "%SCRIPT_DIR%\app\user_settings.bat" (
    call "%SCRIPT_DIR%app\user_settings.bat"
) else (
		echo user settings nicht gefunden
	)

REM DUH Pause %NX_Version_DUH%.bat 3
if "%DEBUG%" == "1" Pause

rem *****************************************************************************
rem 								Start NX
rem *****************************************************************************

rem start with Teamcenter!
pause
if /i "%MANAGED%" == "portal_client" (
    call %TC_PFAD%
    goto DUH_ENDE
)

set is_old_version="FALSE"
if /i "%NX_Version_DUH%" == "NX100" set is_old_version="TRUE"
if /i "%NX_Version_DUH%" == "NX110" set is_old_version="TRUE"
if /i "%NX_Version_DUH%" == "NX120" set is_old_version="TRUE"
if /i "%NX_Version_DUH%" == "NX1872" set is_old_version="TRUE"
if /i "%NX_Version_DUH%" == "NX1892" set is_old_version="TRUE"

if /i %is_old_version% == "TRUE" goto old_version
start "" "%UGII_BASE_DIR%\nxbin\ugraf.exe" -nx
goto DUH_ENDE

:old_version
start "" "%UGII_BASE_DIR%\UGII\ugraf.exe" -nx

:DUH_ENDE
exit