import subprocess
import sys
from pathlib import Path
import os

# print("Mit mir kann man debuggen :)")

KUNDENNAME = sys.argv[1]
NX_VERSION = sys.argv[2]
UGII_LANG = sys.argv[3]
KUNDENPFAD = sys.argv[4]
NXPFAD = sys.argv[5]
PPCHECK = int(sys.argv[6])
CSECHECK = int(sys.argv[7])
TOOLCHECK = int(sys.argv[8])
DEVICECHECK = int(sys.argv[9])
FEEDSPEEDCHECK = int(sys.argv[10])

# ------------------------------------------------------------------------------
# Einstellungen für lokale Computer
# ------------------------------------------------------------------------------
os.environ['UGII_BASE_DIR'] = fr"{NXPFAD}/{NX_VERSION}/"
os.environ['UGII_LANG'] = UGII_LANG

# wird nur bei NX - Versionen vor NX12 benötigt
# UGII_ROOT_DIR = C:/Siemens/ %NX_VERSION %/UGII

# os.environ['CX_SETTINGS_DIR'] = fr"{KUNDENPFAD}"

# Setzen des Temp - Ordners
os.environ['UGII_CAM_CSE_USER_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/2_Testdaten/Temp/"

# Multi core cpu unterstützung *
os.environ['UGII_SMP_ENABLE'] = "1"


# Setzen des User - Ordners
# Verwendung von eigenen Rollen, Loadoptions usw...
os.environ['UGII_VENDOR_DIR'] = fr"{KUNDENPFAD}/6_Custom/"

# Rolle
os.environ['UGII_DEFAULT_ROLE'] = fr"{KUNDENPFAD}/6_Custom/roles/nx_role0.mtx"

# Ladeoptionen
os.environ['UGII_LOAD_OPTIONS'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/"

# ------------------------------------------------------------------------------
# CAM - Machordner mit Standardstruktur *
# ------------------------------------------------------------------------------

# Library
if PPCHECK:
    os.environ['UGII_CAM_LIBRARY_MACHINE_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/library/machine/"
    os.environ['UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/library/machine/installed_machines/"
else:
    os.environ['UGII_CAM_LIBRARY_MACHINE_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/library/machine/"
    os.environ['UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/library/machine/installed_machines/"

os.environ['UGII_CAM_POST_CONFIG_FILE'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/postprocessor/template_post.dat"
os.environ['UGII_CAM_POST_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/postprocessor/"
os.environ['UGII_CAM_TOOL_PATH_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/tool_path/"

if TOOLCHECK:
    os.environ['UGII_CAM_LIBRARY_TOOL_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/library/tool/"
else:
    os.environ[f'UGII_CAM_LIBRARY_TOOL_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/library/tool/"

if FEEDSPEEDCHECK:
    os.environ['UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/library/feeds_speeds/"
else:
    os.environ['UGII_CAM_LIBRARY_FEEDS_SPEEDS_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/library/feeds_speeds/"

if DEVICECHECK:
    os.environ['UGII_CAM_LIBRARY_DEVICE_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/library/device/"
else:
    os.environ['UGII_CAM_LIBRARY_DEVICE_DIR'] = fr"{NXPFAD}/{NX_VERSION}/MACH/resource/library/device/"


if Path(fr"{KUNDENPFAD}{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/template_part").exists():
    os.environ['UGII_CAM_TEMPLATE_PART_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/template_part/"
    os.environ['UGII_CAM_TEMPLATE_PART_METRIC_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/template_part/metric/"
    os.environ['UGII_TEMPLATE_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/MACH/resource/usertools/templates/"


# ------------------------------------------------------------------------------
# Anpassungen für CAD / CAM Umgebung
# ------------------------------------------------------------------------------
if Path(fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/UGII/cx_tools/").exists():
    os.environ['CX_DLL_SYSDIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/UGII/cx_tools/"


# NX title in Leiste schreiben
if not Path(fr"{KUNDENPFAD}/{KUNDENNAME}/_Umgebung/{NX_VERSION}/UGII/startup/nxtitel_configgroup.men").exists():
    if not Path(fr"{KUNDENPFAD}/{KUNDENNAME}/_Umgebung/{NX_VERSION}/UGII/startup").exists():
        os.makedirs(fr"{KUNDENPFAD}/{KUNDENNAME}/_Umgebung/{NX_VERSION}/UGII/startup")
    source_file = "src/NX Title/nxtitel_configgroup.men"
    destination_file = f"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/UGII/startup/nxtitel_configgroup.men"
    with open(source_file) as f:
        data = f.read()
    with open(destination_file, "w") as f:
        f.write(data)
        f.write(f"TITLE CAM - {KUNDENNAME} {NX_VERSION}\n")

os.environ['UGII_USER_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/5_Umgebung/{NX_VERSION}/UGII/"

# nur bei externer Simulation verwendbar
os.environ['UGII_CAM_IPW_SNAPSHOT'] = "1"

# - -----------------------------------------------------------------------------
# Kundenanpassungen
# - -----------------------------------------------------------------------------
# Setzen der Variablen für Hans Weber Messzyklen
os.environ['CX_PROBE_APP'] = fr"{KUNDENPFAD}/Hans_Weber/5_Umgebung/NX2206/CX_Probing/CX_Probing_Main.dll"


# ------------------------------------------------------------------------------
# Start NX
# ------------------------------------------------------------------------------

NX_VERSION = (int(NX_VERSION.replace("NX", "").strip()))
if NX_VERSION < 2206:
    nx_path = fr"{NXPFAD}/NX{NX_VERSION}/UGII/ugraf.exe"
else:
    nx_path = fr"{NXPFAD}/NX{NX_VERSION}/NXBIN/ugraf.exe"

try:
    process = subprocess.Popen(nx_path)

except FileNotFoundError:
    sys.exit(3)


