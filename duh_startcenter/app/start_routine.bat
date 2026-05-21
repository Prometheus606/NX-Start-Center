@echo off


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
set "SCRIPT_DIR=%~dp0"
set "TC_PFAD=D:\Siemens\TC2512\portal\portal.bat"

rem 
rem  REV   	AUTHOR      DATE     	COMMENT for custom_nx.bat
rem  ====   ========== 	==========	=============================================
REM  ????	set DAI_CD_COMMAND_ADDON_IS_ACTIVE=0

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
	set VORLAGE_ROOT=C:\Users\niklas.beitler\Downloads\DUH_Umgebung\DUH_Umgebung\D\_Kunden
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

rem Umgebung erstellen
rem ------------------------------------------------------------------------------
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\1_Kundendaten mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\1_Kundendaten
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\4_Calls mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\4_Calls
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\Reuse_Library mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\Reuse_Library
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\start_apps
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII\menus mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\UGII\menus
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource
	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library" (
		xcopy %UGII_BASE_DIR%\MACH\resource %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource /D /E /I
	
	)
	
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\installed_machines (
		mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\installed_machines
	)
	
	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\templates" (	
		xcopy %UGII_BASE_DIR%\MACH\templates %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\templates /D /E /I
		copy %UGII_BASE_DIR%\UGII\templates\ugs_model_templates.pax %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\templates
	)

rem neue Umgebung
	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\ug_library_%CUSTOMERNAME%" (
		xcopy %UGII_BASE_DIR%\MACH\resource\ug_library %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\ug_library_%CUSTOMERNAME% /D /E /I
	)
	if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\user_def_event_%CUSTOMERNAME%" (
		xcopy %UGII_BASE_DIR%\MACH\resource\user_def_event %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\user_def_event_%CUSTOMERNAME% /D /E /I
	)
rem library

	rem device
	
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\device\ascii_%CUSTOMERNAME%" (
			if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\device\ascii_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\device\ascii_%CUSTOMERNAME%
			xcopy %UGII_BASE_DIR%\MACH\resource\library\device\ascii\device_database.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\device\ascii_%CUSTOMERNAME%  /E /I
		)
		if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\device\graphics_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\device\graphics_%CUSTOMERNAME%

	rem feeds_speeds
		
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%" (
			if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\cut_methods.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\feeds_speeds.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\machining_data.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\part_materials.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\process_force_parameters.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\tool_machining_data.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
			xcopy %UGII_BASE_DIR%\MACH\resource\library\feeds_speeds\ascii\tool_materials.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\feeds_speeds\ascii_%CUSTOMERNAME%  /E /I
		)

	rem machine
	
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\ascii_%CUSTOMERNAME%" (
			if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\ascii_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\ascii_%CUSTOMERNAME%
			xcopy %UGII_BASE_DIR%\MACH\resource\library\machine\ascii\machine_database.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\ascii_%CUSTOMERNAME%  /E /I
		)
		if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\installed_machines_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\machine\installed_machines_%CUSTOMERNAME%		

	rem tool
	
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\tool\ascii_%CUSTOMERNAME%" (
			xcopy %UGII_BASE_DIR%\MACH\resource\library\tool\ascii %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\tool\ascii_%CUSTOMERNAME%  /D /E /I
		)
		if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\tool\graphics_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\tool\graphics_%CUSTOMERNAME%
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\tool\metric_%CUSTOMERNAME%" (
			xcopy %UGII_BASE_DIR%\MACH\resource\library\tool\metric %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\tool\metric_%CUSTOMERNAME%  /D /E /I
		)
	rem fixture_automation
	if "%NX_Version_DUH%"=="NX2512" (
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\fixture_automation\ascii_%CUSTOMERNAME%" (
			xcopy %UGII_BASE_DIR%\MACH\resource\library\fixture_automation\ascii %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\fixture_automation\ascii_%CUSTOMERNAME%  /D /E /I
		)
		if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\fixture_automation\graphics_%CUSTOMERNAME% mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\fixture_automation\graphics_%CUSTOMERNAME%
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\fixture_automation\metric_%CUSTOMERNAME%" (
			xcopy %UGII_BASE_DIR%\MACH\resource\library\fixture_automation\metric %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\library\fixture_automation\metric_%CUSTOMERNAME%  /D /E /I
		)
	)
rem machining_knowledge
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%.dat" (
echo	D| xcopy %UGII_BASE_DIR%\MACH\resource\machining_knowledge\machining_knowledge.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%.dat  /d /Y
		)
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%.xml" (
echo		D|xcopy %UGII_BASE_DIR%\MACH\resource\machining_knowledge\machining_knowledge.xml %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%.xml  /d /Y
		)
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%_tc.xml" (
echo		D|xcopy %UGII_BASE_DIR%\MACH\resource\machining_knowledge\machining_knowledge_tc.xml %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%_tc.xml  /d /Y
		)
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%_tc.dat" (
echo		D|xcopy %UGII_BASE_DIR%\MACH\resource\machining_knowledge\machining_knowledge_part_planner.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\machining_knowledge\machining_knowledge_%CUSTOMERNAME%_tc.dat  /d /Y
		)

rem postprocessor
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\postprocessor\template_post_%CUSTOMERNAME%.dat" (
echo	D| xcopy %UGII_BASE_DIR%\MACH\resource\postprocessor\template_post.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\postprocessor\template_post_%CUSTOMERNAME%.dat  /d /Y
		)
rem shop_docr
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\shop_doc\shop_doc_%CUSTOMERNAME%.dat" (
echo	D| xcopy %UGII_BASE_DIR%\MACH\resource\shop_doc\shop_doc.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\shop_doc\shop_doc_%CUSTOMERNAME%.dat  /d /Y
		)
rem template_set
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\template_set\cam_%CUSTOMERNAME%_native.opt" (
echo	D| xcopy %UGII_BASE_DIR%\MACH\resource\template_set\cam_general.opt %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\template_set\cam_%CUSTOMERNAME%_native.opt  /d /Y
		)
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\template_set\cam_%CUSTOMERNAME%_tc.opt" (
echo	D| xcopy %UGII_BASE_DIR%\MACH\resource\template_set\cam_teamcenter_general.opt %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\template_set\cam_%CUSTOMERNAME%_tc.opt  /d /Y
		)
rem configuration
if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\configuration" (
echo	D| xcopy %UGII_BASE_DIR%\MACH\resource\configuration %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\configuration  /D /E /I
		)

@echo off
setlocal EnableDelayedExpansion

rem =========================
rem Basis-Pfade
rem =========================
set "base=%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\configuration"

set "quelle_native=%base%\cam_general.dat"
set "ziel_native=%base%\cam_%CUSTOMERNAME%_native.dat"

set "quelle_tc=%base%\cam_teamcenter_ascii_library.dat"
set "ziel_tc=%base%\cam_%CUSTOMERNAME%_tc.dat"

echo ================================
echo Erzeuge Dateien (Debug-Ausgabe)
echo Basis: %base%
echo Quelle native: "%quelle_native%"
echo Ziel native:   "%ziel_native%"
echo Quelle tc:     "%quelle_tc%"
echo Ziel tc:       "%ziel_tc%"
echo ================================

rem =========================
rem Native-Datei erzeugen
rem =========================
call :CreateFile "%quelle_native%" "%ziel_native%" "_native"

rem =========================
rem TC-Datei erzeugen
rem =========================
call :CreateFile "%quelle_tc%" "%ziel_tc%" "_tc"

goto :done

rem =========================
rem Subroutine: CreateFile
rem   %1 = Quelldatei
rem   %2 = Zieldatei
rem   %3 = Suffix (_native/_tc)
rem =========================
:CreateFile
set "a=%~1"
set "b=%~2"
set "c=%~3"

echo.
echo --- Verarbeitung: Quelle="%a%"  Ziel="%b%"  Suffix="%c%" ---

if not exist "%a%" (
  echo FEHLER: Quelldatei nicht gefunden: "%a%"
  exit /b 0
)

if exist "%b%" (
  echo Überspringe: Ziel existiert bereits: "%b%"
  exit /b 0
)

rem Versuche die Zieldatei zu schreiben
> "%b%" (
  echo TEMPLATE_OPERATION,${UGII_CAM_TEMPLATE_SET_DIR}cam_%CUSTOMERNAME%%c%.opt
  echo TEMPLATE_DOCUMENTATION,${UGII_CAM_SHOP_DOC_DIR}shop_doc_%CUSTOMERNAME%.dat
  echo TEMPLATE_POST,${UGII_CAM_POST_DIR}template_post_%CUSTOMERNAME%.dat

  rem jede Zeile (inkl. leer) nummerieren und ab Zeile 4 übernehmen
  for /f "tokens=1* delims=:" %%A in ('findstr /n "^" "%a%"') do (
    if %%A GEQ 4 echo %%B
  )
)

rem Kontrolle, ob Datei erstellt wurde
if exist "%b%" (
  for %%S in ("%b%") do echo Datei erstellt: "%%~fS" (Größe: %%~zS bytes)
) else (
  echo FEHLER: Konnte Datei nicht erstellen: "%b%"
)

exit /b

:done
endlocal
echo Fertig.
if "%DEBUG%" == "1" echo on



	rem auxiliary
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary" (
			if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary
			if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary\tagging mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary\tagging
			if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary\tagging\metric mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary\tagging\metric
echo	D| xcopy %UGII_BASE_DIR%\MACH\auxiliary\tagging\metric\tagging.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary\tagging\metric\tagging.dat  /d /Y
echo	D| xcopy %UGII_BASE_DIR%\MACH\auxiliary\tagging\metric\tagging_fea.dat %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\auxiliary\tagging\metric\tagging_fea.dat  /d /Y
		)

rem template_dir
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\template_dir_%CUSTOMERNAME%" (
		xcopy %UGII_BASE_DIR%\MACH\resource\template_dir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\resource\template_dir_%CUSTOMERNAME%  /D /E /I
		)

	rem CAM_POST_OUTPUT_DIR
		if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\CAM_POST_OUTPUT_DIR mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\CAM_POST_OUTPUT_DIR

	rem CAM_SETUP_ROOT_DIR
		if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\CAM_SETUP_ROOT_DIR mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\MACH\CAM_SETUP_ROOT_DIR

	rem duh_tools
		if NOT EXIST "%PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\duh_tools" (
			xcopy %VORLAGE_ROOT%\Vorlage\duh_tools %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\duh_tools /D /E /I
		)
		

	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\6_Custom mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\6_Custom
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\7_Dokumentation mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\7_Dokumentation
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Temp mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Temp
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Temp\NX mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Temp\NX
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Shop_Doc mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\2_Testdaten\Shop_Doc



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
	
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults mkdir %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults

	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Site (
		xcopy %VORLAGE_ROOT%\Vorlage\%NX_Version_DUH%\CustomerDefaults\Site %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Site /D /E /I
	)
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Group (
		xcopy %VORLAGE_ROOT%\Vorlage\%NX_Version_DUH%\CustomerDefaults\Group %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\Group /D /E /I
	)
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User (
		xcopy %VORLAGE_ROOT%\Vorlage\%NX_Version_DUH%\CustomerDefaults\User %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\User /D /E /I
	)

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
	if NOT EXIST %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\EarlyAccessFeature (
		xcopy %VORLAGE_ROOT%\Vorlage\%NX_Version_DUH%\CustomerDefaults\EarlyAccessFeature %PLM_SHARE_DUH%\%CUSTOMERNAME%\%UMGEBUNG%\%NX_Version_DUH%\CustomerDefaults\EarlyAccessFeature /D /E /I
	)
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