# *****************************************************************************
# 					        WICHTIGER HINWEIS!!
#
#     Änderungen die nicht überschrieben werden sollen, bitte in dieser Datei vornehmen
#
# *****************************************************************************

import os
def Customize(KUNDENPFAD, KUNDENNAME, NX_VERSION, NXPFAD):

    # Setzen der Variablen für Hans Weber Messzyklen
    if KUNDENNAME == "Hans_Weber":
        os.environ['CX_PROBE_APP'] = fr"{KUNDENPFAD}/Hans_Weber/5_Umgebung/NX2206/CX_Probing/CX_Probing_Main.dll"


    # Setzen des Temp - Ordners für WuH
    if KUNDENNAME == "WuH":
        # WuH Settings
        os.environ['UGII_CAM_CSE_USER_DIR'] = fr"C:/temp/dmc340fd_4341_VNCK/cse_prog_projects"
        os.environ['UGII_NCD_DIR'] = fr"C:/temp/"
    else:
        os.environ['UGII_CAM_CSE_USER_DIR'] = fr"{KUNDENPFAD}/{KUNDENNAME}/2_Testdaten/Temp/"