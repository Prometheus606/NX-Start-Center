# *****************************************************************************
# 					        WICHTIGER HINWEIS!!
#     Anpassungen an dieser Datei werden bei einem Update überschrieben!
#     Änderungen die bleiben sollen, bitte in der user_settings.py vornehmen
# *****************************************************************************

import subprocess
import sys
from pathlib import Path
import os
import shutil

# print("Mit mir kann man debuggen :)")

KUNDENNAME = sys.argv[1]
NX_VERSION = sys.argv[2]
UGII_LANG = sys.argv[3]
KUNDENPFAD = sys.argv[4]
NXPFAD = sys.argv[5]
PPCHECK = int(sys.argv[6])
INSTALLED_MACHINES_CHECK = int(sys.argv[7])
TOOLCHECK = int(sys.argv[8])
DEVICECHECK = int(sys.argv[9])
FEEDSPEEDCHECK = int(sys.argv[10])

# Folder structure in Customers ENV
ENV_FOLDER_NAME = "5_Umgebung"
CUSTOMER_DIR = f"{KUNDENPFAD}\\{KUNDENNAME}\\"
ENV_DIR = f"{CUSTOMER_DIR}{ENV_FOLDER_NAME}\\"
VERSION_DIR = f"{ENV_DIR}{NX_VERSION}\\"
UGII_DIR = f"{VERSION_DIR}UGII\\"
MACH_DIR = f"{VERSION_DIR}MACH\\"
RESOURCE_DIR = f"{MACH_DIR}resource\\"
LIBRARY_DIR = f"{RESOURCE_DIR}library\\"
MACHINE_DIR = f"{LIBRARY_DIR}machine\\"
INSTALLED_MACHINES_DIR = f"{MACHINE_DIR}installed_machines\\"

# Folder structure in NX Installation
NATIVE_BASE_DIR = f"{NXPFAD}\\{NX_VERSION}\\"
NATIVE_UGII_DIR = f"{NATIVE_BASE_DIR}UGII\\"
NATIVE_MACH_DIR = f"{NATIVE_UGII_DIR}MACH\\"
NATIVE_RESOURCE_DIR = f"{NATIVE_MACH_DIR}resource\\"
NATIVE_LIBRARY_DIR = f"{NATIVE_RESOURCE_DIR}library\\"
NATIVE_MACHINE_DIR = f"{NATIVE_LIBRARY_DIR}machine\\"
NATIVE_INSTALLED_MACHINES_DIR = f"{NATIVE_MACHINE_DIR}installed_machines\\"



# ------------------------------------------------------------------------------
# Einstellungen für lokale Computer
# ------------------------------------------------------------------------------
os.environ['UGII_BASE_DIR'] = NATIVE_BASE_DIR
os.environ['UGII_LANG'] = UGII_LANG

os.environ['CX_PP_TOOLS'] = f"{os.path.dirname(os.getcwd())}"
os.environ['CX_CUSTOM_DIRS'] = UGII_DIR
os.environ['UGII_CUSTOM_DIRECTORY_FILE'] = f"{UGII_DIR}custom_dirs.dat"

# wird nur bei NX - Versionen vor NX12 benötigt
# UGII_ROOT_DIR = C:\\Siemens\\ %NX_VERSION %\\UGII

# Setzen des Temp - Ordners
os.environ['UGII_CAM_CSE_USER_DIR'] = f"{CUSTOMER_DIR}2_Testdaten\\Temp\\"

# Multi core cpu unterstützung *
os.environ['UGII_SMP_ENABLE'] = "1"


# Setzen des User - Ordners
# Verwendung von eigenen Rollen, Loadoptions usw...
os.environ['UGII_VENDOR_DIR'] = f"{KUNDENPFAD}6_Custom\\"

# Rolle
#os.environ['UGII_DEFAULT_ROLE'] = f"{KUNDENPFAD}\\6_Custom\\roles\\nx_role0.mtx"

# Ladeoptionen
os.environ['UGII_LOAD_OPTIONS'] = f"{MACH_DIR}resource\\"

# ------------------------------------------------------------------------------
# CAM - Machordner mit Standardstruktur *
# ------------------------------------------------------------------------------

# Library
os.environ['UGII_CAM_LIBRARY_MACHINE_DIR'] = NATIVE_MACHINE_DIR
os.environ['UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR'] = NATIVE_INSTALLED_MACHINES_DIR
os.environ['UGII_CAM_POST_DIR'] = f"{NATIVE_RESOURCE_DIR}postprocessor\\"
os.environ['UGII_CAM_POST_CONFIG_FILE'] = f"{NATIVE_RESOURCE_DIR}postprocessor\\template_post.dat"
os.environ['UGII_CAM_TOOL_PATH_DIR'] = f"{NATIVE_RESOURCE_DIR}tool_path\\"
os.environ[f'UGII_CAM_LIBRARY_TOOL_DIR'] = f"{NATIVE_LIBRARY_DIR}library\\tool\\"
os.environ['UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR'] = f"{NATIVE_LIBRARY_DIR}feeds_speeds\\"
os.environ['UGII_CAM_LIBRARY_DEVICE_DIR'] = f"{NATIVE_LIBRARY_DIR}device\\"

if PPCHECK:
    os.environ['UGII_CAM_POST_DIR'] = f"{RESOURCE_DIR}postprocessor\\"
    os.environ['UGII_CAM_USER_DEF_EVENT_DIR'] = f"{RESOURCE_DIR}user_def_event\\"

if INSTALLED_MACHINES_CHECK:
    os.environ['UGII_CAM_LIBRARY_MACHINE_DIR'] = MACHINE_DIR
    os.environ['UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR'] = INSTALLED_MACHINES_DIR

if TOOLCHECK:
    os.environ['UGII_CAM_LIBRARY_TOOL_DIR'] = f"{LIBRARY_DIR}tool\\"

if FEEDSPEEDCHECK:
    os.environ['UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR'] = f"{LIBRARY_DIR}feeds_speeds\\"

if DEVICECHECK:
    os.environ['UGII_CAM_LIBRARY_DEVICE_DIR'] = f"{LIBRARY_DIR}device\\"


if Path(f"{KUNDENPFAD}\\template_part").exists():
    os.environ['UGII_CAM_TEMPLATE_PART_DIR'] = f"{RESOURCE_DIR}template_part\\"
    os.environ['UGII_CAM_TEMPLATE_PART_METRIC_DIR'] = f"{RESOURCE_DIR}template_part\\metric\\"
    os.environ['UGII_TEMPLATE_DIR'] = f"{RESOURCE_DIR}usertools\\templates\\"


# ------------------------------------------------------------------------------
# Anpassungen für CAD \\ CAM Umgebung
# ------------------------------------------------------------------------------
if Path(f"{UGII_DIR}cx_tools\\").exists():
    os.environ['CX_DLL_SYSDIR'] = f"{UGII_DIR}cx_tools\\"


# NX title in Leiste schreiben und conmatix tab erstellen
startup_path = f"{os.environ.get('CX_CUSTOM_DIRS')}\\startup"
nxtitle_file = "src\\NX_UI\\nxtitel_configgroup.men"
pp_tools_file = "src\\NX_UI\\CX_PP_tools.rtb"
custom_dirs_file = "src\\NX_UI\\custom_dirs.dat"
if not Path(f"{startup_path}").exists():
    os.makedirs(f"{startup_path}")
if not Path(f"{os.environ.get('CX_CUSTOM_DIRS')}\\custom_dirs.dat").exists():
    shutil.copy(custom_dirs_file, f"{os.environ.get('CX_CUSTOM_DIRS')}\\custom_dirs.dat")
shutil.copy(pp_tools_file, f"{startup_path}\\CX_PP_tools.rtb")
with open(nxtitle_file) as f:
    data = f.read()
with open(f"{startup_path}\\nxtitel_configgroup.men", "w") as f:
    f.write(data)
    f.write(f"TITLE CAM - {NX_VERSION}")

# nur bei externer Simulation verwendbar
os.environ['UGII_CAM_IPW_SNAPSHOT'] = "1"

# ------------------------------------------------------------------------------
# Startbatch CAM Team Checken
# - -----------------------------------------------------------------------------
try:
    # Batch-Datei und Argumente definieren
    cam_batch_datei = f"{VERSION_DIR}start_apps\\custom_nx.bat"
    if os.path.exists(cam_batch_datei):
        print("Es Existiert eine custom_nx Batch datei fuer eine Komplexe CAM Umgebung. Zum verwenden bitte Batch anstatt Python in den Einstellungen auswaehlen.")

except Exception:
    pass

# ------------------------------------------------------------------------------
# Kundenanpassungen lesen
# - -----------------------------------------------------------------------------
try:
    from user_settings import Customize
    Customize(KUNDENPFAD, KUNDENNAME, NX_VERSION, NXPFAD)

except ImportError:
    pass
# ------------------------------------------------------------------------------
# Start NX
# ------------------------------------------------------------------------------

NX_VERSION = (int(NX_VERSION.replace("NX", "").strip()))
if NX_VERSION < 2206:
    nx_path = f"{NXPFAD}\\NX{NX_VERSION}\\UGII\\ugraf.exe"
else:
    nx_path = f"{NXPFAD}\\NX{NX_VERSION}\\NXBIN\\ugraf.exe"

try:
    process = subprocess.Popen(nx_path)

except FileNotFoundError:
    sys.exit(3)
