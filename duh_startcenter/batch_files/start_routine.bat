@echo off
rem *****************************************************************************
rem 					     IMPORTANT!!
rem     Changes made to this file will be overwritten during an update!
rem     Please make permanent changes in user_settings.bat!
rem *****************************************************************************
rem 					     WICHTIGER HINWEIS!!
rem     Anpassungen an dieser Datei werden bei einem Update ueberschrieben!
rem		Dauerhafte anpassungen bitte in user_settings.bat vornehmen!
rem *****************************************************************************


set "DEBUG=%~6"
if "%DEBUG%" == "True" echo on

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
set "IS_PP_DEVELOPER=%~7"
set "LOAD_FULL_RESOURCE_DIR=%~8"
set "ROLES_PATH=%~9"

set "SCRIPT_DIR=%~dp0"
set "UMGEBUNG=5_Umgebung"

rem ------------------------------------------------------------------------------
rem Set Main Env Variables
rem ------------------------------------------------------------------------------
	call :SetIfExist UGII_BASE_DIR "%NX_INSTALL_PATH%\%NX_Version_DUH%"
	call :SetIfExist PLM_SHARE_DUH "%CUSTOMER_PATH%"
	call :SetIfExist UGII_USER_DIR "%APPDATA%\Siemens\%NX_Version_DUH%\ConfigSettings"
	call :SetIfExist UGII_CAM_CSE_USER_DIR "%PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Temp\"
	call :SetIfExist UGII_LOAD_OPTIONS "%VORLAGE_ROOT%\Vorlage\load_options\load_options.def"
	call :SetIfExist DUH_ToolBars_DIR "%VORLAGE_ROOT%\ToolBars\DUH_Group"	
	call :SetIfExist NX_SHR_VERSION_DIR "%NX_Version_DUH%"
	call :SetIfExist SPLM_SHR_DIR "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%"

rem ------------------------------------------------------------------------------
rem set Toolbar for PP developers
rem ------------------------------------------------------------------------------
	if "%IS_PP_DEVELOPER%" == "True" (
		call :SetIfExist DUH_PP_TOOLS "%VORLAGE_ROOT%\ToolBars\DUH_Group\PP_Tools"
	)

rem ------------------------------------------------------------------------------
rem Custom Roles
rem ------------------------------------------------------------------------------
	call :SetIfExist DUH_CUSTOM_ROLES "%ROLES_PATH%"
	set UGII_DEFAULT_ROLE=empty

rem ------------------------------------------------------------------------------
rem set CAM Setup Variables
rem ------------------------------------------------------------------------------
set "MACH_DIR=%SPLM_SHR_DIR%\%NX_Version_DUH%\MACH\"
set "RESOURCE_DIR=%MACH_DIR%resource\"
set "LIBRARY_DIR=%RESOURCE_DIR%library\

	if "%LOAD_FULL_RESOURCE_DIR%" == "True" (
		call :SetIfExist UGII_CAM_RESOURCE_DIR "%RESOURCE_DIR%"
		call :SetIfExist UGII_CAM_CUSTOM_DIR "%RESOURCE_DIR%"
	) else (
		if /i "%PPCHECK%" == "True" (
			call :SetIfExist UGII_CAM_POST_DIR "%RESOURCE_DIR%postprocessor\"
			call :SetIfExist UGII_CAM_USER_DEF_EVENT_DIR "%RESOURCE_DIR%user_def_event\"
		)

		if /i "%INSTALLED_MACHINES_CHECK%" == "True" (
			call :SetIfExist UGII_CAM_LIBRARY_MACHINE_DIR "%LIBRARY_DIR%machine\"
			call :SetIfExist UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR "%LIBRARY_DIR%machine\installed_machines\"
		)

		if /i "%TOOLCHECK%" == "True" (
			call :SetIfExist UGII_CAM_LIBRARY_TOOL_DIR "%LIBRARY_DIR%tool\"
		)

		if /i "%FEEDSPEEDCHECK%" == "True" (
			call :SetIfExist UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR "%LIBRARY_DIR%\feeds_speeds\"
		)

		if /i "%DEVICECHECK%" == "True" (
			call :SetIfExist UGII_CAM_LIBRARY_DEVICE_DIR "%LIBRARY_DIR%device\"
		)
	)

rem ------------------------------------------------------------------------------
rem set License Server if NX X is Used
rem ------------------------------------------------------------------------------
if "%DEBUG%" == "True" Pause
if "%CLOUD_LICENSE%" == "True" (
	set SPLM_LICENSE_SERVER=CLOUD
) 

rem ------------------------------------------------------------------------------
rem set temp dir
rem ------------------------------------------------------------------------------
	call :SetIfExist UGII_TMP_DIR "%PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\temp\NX

rem ------------------------------------------------------------------------------

REM rem Kunden Variablen setzen
rem ------------------------------------------------------------------------------
	call :SetIfExist UGII_CAM_CONFIG "%UGII_CAM_RESOURCE_DIR%configuration\%CUSTOMERNAME%.dat"
	call :SetIfExist UGII_CAM_LIBRARY_DEVICE_DATA_DIR "%UGII_CAM_RESOURCE_DIR%library\device\%CUSTOMERNAME%\"
	call :SetIfExist UGII_CAM_LIBRARY_FEEDS_SPEEDS_DATA_DIR "%UGII_CAM_RESOURCE_DIR%library\feeds_speeds\%CUSTOMERNAME%\"
	call :SetIfExist UGII_CAM_LIBRARY_MACHINE_DATA_DIR "%UGII_CAM_RESOURCE_DIR%library\machine\%CUSTOMERNAME%\"
	call :SetIfExist UGII_CAM_LIBRARY_TOOL_METRIC_DIR "%UGII_CAM_RESOURCE_DIR%library\tool\%CUSTOMERNAME%\"

rem ------------------------------------------------------------------------------
rem remove not needet files and folders (from old environments)
rem ------------------------------------------------------------------------------

	if EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\cx_tools" rd /s /q  "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\cx_tools"
	if EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\customer_defaults" rd /s /q "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\customer_defaults"
	if EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\custom_dirs.dat" del "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\custom_dirs.dat"
	if EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\startup\nxtitel_configgroup.men" del "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\startup\nxtitel_configgroup.men"
	if EXIST "%LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.dpv" del "%LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.dpv"
	if EXIST "%LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.xsl" del "%LOCALAPPDATA%\Siemens\%NX_Version_DUH%\NX_user.xsl"

rem ------------------------------------------------------------------------------
rem Provide .men file for NX app title
rem ------------------------------------------------------------------------------

	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup\nxtitel_configgroup.men" (
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup" mkdir "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup"	
	)
	copy "%VORLAGE_ROOT%\Vorlage\usertools\startup\nxtitel_configgroup.men" "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup"
	echo TITLE CAM - %CUSTOMERNAME% %NX_Version_DUH% >> "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup\nxtitel_configgroup.men"
 
rem ------------------------------------------------------------------------------
rem copy custom_dirs.dat
rem ------------------------------------------------------------------------------
	set "CUSTOM_DIRS_DIR=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus"
	set "CUSTOM_DIRS_FILE=%CUSTOM_DIRS_DIR%\custom_dirs.dat"

	if NOT EXIST "%CUSTOM_DIRS_FILE%" (
		if NOT EXIST "%CUSTOM_DIRS_DIR%" mkdir "%CUSTOM_DIRS_DIR%"
		copy "%VORLAGE_ROOT%\Vorlage\custom_dirs.dat" "%CUSTOM_DIRS_DIR%\"
	) else (
		call :UpdateCustomDirs "%CUSTOM_DIRS_FILE%"
	)

	call :SetIfExist UGII_CUSTOM_DIRECTORY_FILE "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\custom_dirs.dat"	
	call :SetIfExist UGII_UG_CUSTOM_DIRECTORY_FILE "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\menus\ug_custom_dirs.dat"

rem ------------------------------------------------------------------------------
rem ugii_env.dat
rem ------------------------------------------------------------------------------
	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII\ugii_env.dat" (
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII" mkdir "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII"
		copy "%VORLAGE_ROOT%\Vorlage\ugii_env.dat" "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII"
	)

	call :SetIfExist UGII_ENV_FILE "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\ugii_env_switch.dat"
	call :SetIfExist UGII_ENV_FILE "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\ugii\ugii_env.dat"

rem ------------------------------------------------------------------------------
rem customer_defaults_Site
rem ------------------------------------------------------------------------------
	

	call :SetIfExist UGII_SITE_DIR "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Site"
	call :SetIfExist UGII_GROUP_DIR "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Group"
	call :SetIfExist UGII_USER_DIR "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User"
	call :SetIfExist UGII_LOCAL_USER_DEFAULTS "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User\startup\nx_user.dpv"


rem ------------------------------------------------------------------------------
rem Early Access function
rem ------------------------------------------------------------------------------
	call :SetIfExist UGII_LOCAL_USER_TOGGLE_DEFAULTS "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\EarlyAccessFeature\feature_toggle_user.fcg"

rem ------------------------------------------------------------------------------
rem set nxtools environment
rem ------------------------------------------------------------------------------
	if EXIST "%VORLAGE_ROOT%\ToolBars\NX_tools\%NX_Version_DUH%\NXTools" (
		call :SetIfExist Siemens_ToolBars_DIR "%VORLAGE_ROOT%\ToolBars\Siemens"
		call :SetIfExist NXTOOLS_SYSDIR "%VORLAGE_ROOT%\ToolBars\NX_tools\%NX_Version_DUH%\NXTools"
		set NXTOOLS_LOOK_AHEAD=true
		call :SetIfExist UGII_USER_TOOLS_FILE "%NXTOOLS_SYSDIR%\usertools\usertools.utd"
		call :SetIfExist UGII_USER_TOOLS_MENU "%NXTOOLS_SYSDIR%\usertools\usertools.utm"
		call :SetIfExist UGII_USER_TOOLS_BITMAP_PATH "%NXTOOLS_SYSDIR%\usertools\bitmaps"
	) else (
		echo "================== ERROR ======================"
		echo "Siemens and NX Tools toolbar will dont show up, because the path to NX Tools doesnt exist: %VORLAGE_ROOT%\ToolBars\NX_tools\%NX_Version_DUH%\NXTools"
		echo "==============================================="
		if "%DEBUG%" == "True" Pause
	)

rem ------------------------------------------------------------------------------
rem Call custom_nx.bat
rem set variables for customers
rem custom_nx.bat at %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps  
rem ------------------------------------------------------------------------------
	if "%DEBUG%" == "True" Pause
	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat" IF /I "%LOAD_FULL_RESOURCE_DIR%"=="True" (
		echo	D| xcopy "%VORLAGE_ROOT%\Vorlage\start_apps\custom_nx.bat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat"  /d /Y
	)

	if exist "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat" IF /I "%LOAD_FULL_RESOURCE_DIR%"=="True" (
		set "KUNDE_DUH=%CUSTOMERNAME%"
		if "%DEBUG%" == "True" (
			set "SIDT_DEBUG=1"
		) else (
			set "SIDT_DEBUG=0"
		)
		if "%UGII_LANG%" == "german" (
			set "SIDT_PAR1=de"
		) else (
			set "SIDT_PAR1=en"
		)
		set "NX_SHR_VERSION_DIR=%NX_Version_DUH%"
		set "SIDT_PAR2=%MANAGED%"
		call "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps\custom_nx_%CUSTOMERNAME%.bat"
	) else (
		echo custom_nx_%CUSTOMERNAME% nicht gefunden
	)

REM ------------------------------------------------------------------------------
REM Call User Batch
REM ------------------------------------------------------------------------------
if exist "%SCRIPT_DIR%\Batch_files\user_settings.bat" (
    call "%SCRIPT_DIR%Batch_files\user_settings.bat"
) else (
	echo user settings nicht gefunden!
)

:START_NX
if "%DEBUG%" == "True" Pause
rem *****************************************************************************
rem 								Start NX
rem *****************************************************************************

rem start with Teamcenter!
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

REM ----------------------------------------------------------
REM Set ENV Variable if the path exists
REM Call sample:
REM call :SetIfExist UGII_USER_TOOLS_MENU "C:\Pfad\"
REM ----------------------------------------------------------
:SetIfExist
if exist "%~2" (
    set "%~1=%~f2"
)
exit /b

REM ----------------------------------------------------------
REM custom_dirs.dat prüfen und fehlende Einträge ergänzen
REM Aufruf:
REM call :UpdateCustomDirs "C:\Pfad\custom_dirs.dat"
REM ----------------------------------------------------------
:UpdateCustomDirs
setlocal

set "FILE=%~1"

if not exist "%FILE%" (
    echo Datei nicht gefunden: %FILE%
    endlocal & exit /b 1
)

call :AddLineIfMissing "%FILE%" "$NXTOOLS_SYSDIR\usertools"
call :AddLineIfMissing "%FILE%" "$DUH_ToolBars_DIR\duh_tools"
call :AddLineIfMissing "%FILE%" "$Siemens_ToolBars_DIR"
call :AddLineIfMissing "%FILE%" "$DUH_PP_TOOLS"
call :AddLineIfMissing "%FILE%" "$duh_tools_DIR"
call :AddLineIfMissing "%FILE%" "$DUH_CUSTOM_ROLES"

endlocal
exit /b 0


:AddLineIfMissing
findstr /x /c:"%~2" "%~1" >nul
if errorlevel 1 (
    echo %~2>>"%~1"
)
exit /b