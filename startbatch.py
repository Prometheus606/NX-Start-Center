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

# ------------------------------------------------------------------------------
# Einstellungen für lokale Computer
# ------------------------------------------------------------------------------
os.environ['UGII_BASE_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\"
os.environ['UGII_LANG'] = UGII_LANG

os.environ['CX_PP_TOOLS'] = f"{os.path.dirname(os.getcwd())}"

# wird nur bei NX - Versionen vor NX12 benötigt
# UGII_ROOT_DIR = C:\\Siemens\\ %NX_VERSION %\\UGII

# Setzen des Temp - Ordners
os.environ['UGII_CAM_CSE_USER_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\2_Testdaten\\Temp\\"

# Multi core cpu unterstützung *
os.environ['UGII_SMP_ENABLE'] = "1"


# Setzen des User - Ordners
# Verwendung von eigenen Rollen, Loadoptions usw...
os.environ['UGII_VENDOR_DIR'] = f"{KUNDENPFAD}\\6_Custom\\"

# Rolle
#os.environ['UGII_DEFAULT_ROLE'] = f"{KUNDENPFAD}\\6_Custom\\roles\\nx_role0.mtx"

# Ladeoptionen
os.environ['UGII_LOAD_OPTIONS'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\"

# ------------------------------------------------------------------------------
# CAM - Machordner mit Standardstruktur *
# ------------------------------------------------------------------------------

# Library
os.environ['UGII_CAM_LIBRARY_MACHINE_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\library\\machine\\"
os.environ['UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\library\\machine\\installed_machines\\"
os.environ['UGII_CAM_POST_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\postprocessor\\"
os.environ['UGII_CAM_POST_CONFIG_FILE'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\postprocessor\\template_post.dat"
os.environ['UGII_CAM_TOOL_PATH_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\tool_path\\"
os.environ[f'UGII_CAM_LIBRARY_TOOL_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\library\\tool\\"
os.environ['UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\library\\feeds_speeds\\"
os.environ['UGII_CAM_LIBRARY_DEVICE_DIR'] = f"{NXPFAD}\\{NX_VERSION}\\MACH\\resource\\library\\device\\"

if PPCHECK:
    os.environ['UGII_CAM_POST_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\postprocessor\\"
    os.environ['UGII_CAM_USER_DEF_EVENT_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\user_def_event\\"

if INSTALLED_MACHINES_CHECK:
    os.environ['UGII_CAM_LIBRARY_MACHINE_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\library\\machine\\"
    os.environ['UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\library\\machine\\installed_machines\\"

if TOOLCHECK:
    os.environ['UGII_CAM_LIBRARY_TOOL_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\library\\tool\\"

if FEEDSPEEDCHECK:
    os.environ['UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\library\\feeds_speeds\\"

if DEVICECHECK:
    os.environ['UGII_CAM_LIBRARY_DEVICE_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\library\\device\\"


if Path(f"{KUNDENPFAD}{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\template_part").exists():
    os.environ['UGII_CAM_TEMPLATE_PART_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\template_part\\"
    os.environ['UGII_CAM_TEMPLATE_PART_METRIC_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\template_part\\metric\\"
    os.environ['UGII_TEMPLATE_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\MACH\\resource\\usertools\\templates\\"


# ------------------------------------------------------------------------------
# Anpassungen für CAD \\ CAM Umgebung
# ------------------------------------------------------------------------------
if Path(f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\UGII\\cx_tools\\").exists():
    os.environ['CX_DLL_SYSDIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\UGII\\cx_tools\\"


# NX title in Leiste schreiben und conmatix tab erstellen
startup_path = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\UGII\\startup"
if not Path(f"{startup_path}\\nxtitel_configgroup.men").exists() or not Path(f"{startup_path}\\CX_PP_tools.rtb").exists():
    if not Path(f"{startup_path}").exists():
        os.makedirs(f"{startup_path}")
    nxtitle_file = "src\\NX_UI\\nxtitel_configgroup.men"
    pp_tools_file = "src\\NX_UI\\CX_PP_tools.rtb"
    shutil.copy(pp_tools_file, f"{startup_path}\\CX_PP_tools.rtb")
    with open(nxtitle_file) as f:
        data = f.read()
    with open(f"{startup_path}\\nxtitel_configgroup.men", "w") as f:
        f.write(data)
        f.write(f"TITLE CAM - {KUNDENNAME} {NX_VERSION}")

os.environ['UGII_USER_DIR'] = f"{KUNDENPFAD}\\{KUNDENNAME}\\5_Umgebung\\{NX_VERSION}\\UGII\\"

# nur bei externer Simulation verwendbar
os.environ['UGII_CAM_IPW_SNAPSHOT'] = "1"

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
