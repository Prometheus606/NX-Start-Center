import re
import shutil
import os
from pathlib import Path

from controller.Utils import copy_folder

class CreateNewCustomer:
    """
    This class handles the create new customer Button
    """
    def __init__(self, controller):
        self.controller = controller

    def create_new_customer(self):
        self.controller.view.messageLabel.config(text="")
        customer_is_new = True
        customer_path = fr"{self.controller.model.settings.get('customer_environment_path')}/{self.controller.view.new_customer.get()}"
        machine_controller = "Niklas"
        installed_machines_dir = fr"{customer_path}/5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines"
        machine_name = self.controller.view.new_machine.get()
        machine_dir = fr"{installed_machines_dir}\{machine_name}"
        pp_dir = fr"{machine_dir}/postprocessor"
        cse_dir = fr"{machine_dir}/cse_driver/{machine_controller}"

        new_version = self.controller.view.new_version.get()
        new_machine = self.controller.view.new_machine.get()
        new_order = self.controller.view.new_order.get()
        new_customer = self.controller.view.new_customer.get()

        if not new_customer or not new_version:
            print("Kundenname und Version muss angegeben sein!")
            self.controller.view.messageLabel.config(text="Kundenname und Version muss angegeben sein!", foreground="red")
            return

        if " " in new_customer or " " in new_version or " " in new_machine or " " in new_order:
            print("Leerzeichen sind nicht Zulässig!")
            self.controller.view.messageLabel.config(text="Leerzeichen sind nicht Zulässig!", foreground="red")
            return

        if not re.match(r"^NX\d{2,4}", new_version):
            print("Die Version muss das Format NXxxxx haben.")
            self.controller.view.messageLabel.config(text="Die Version muss das Format NXxxxx haben.", foreground="red")
            return

        if [i for i in os.listdir(self.controller.model.settings.get('customer_environment_path')) if new_customer.lower() == i.lower()]:
            print("Kunde bereits angelegt!")
            # self.controller.view.messageLabel.config(text="Kunde bereits angelegt!", foreground="red")
            customer_is_new = False

        if Path(fr"{installed_machines_dir}/{new_machine}").exists():
            print("Maschine bereits angelegt!")
            self.controller.view.messageLabel.config(text="Maschine bereits angelegt!", foreground="red")
            return

        if not new_order:
            self.controller.view.new_order.set("0000")

        if customer_is_new:
            os.makedirs(fr"{customer_path}/1_Kundendaten/{new_order}/")
            os.makedirs(fr"{customer_path}/2_Testdaten/{new_order}/")
            os.makedirs(fr"{customer_path}/3_Auslieferung/{new_order}/")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/usertools/")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/ascii")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/inclass")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/installed_machines")
            os.makedirs(fr"{customer_path}/4_Calls/{new_order}/")
            os.makedirs(fr"{customer_path}/6_Custom/{new_order}/{new_version}/roles/")
            os.makedirs(fr"{customer_path}/6_Custom/{new_order}/{new_version}/startup/")
            os.makedirs(fr"{customer_path}/7_Dokumentation/{new_order}/")

            copy_folder(
                fr"{self.controller.model.settings.get('nx_installation_path')}/{new_version}/MACH/resource/library/machine/ascii/",
                fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/ascii/")
            copy_folder(
                fr"{self.controller.model.settings.get('nx_installation_path')}/{new_version}/MACH/resource/library/machine/inclass/",
                fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/inclass/")

        # refresh the customers combobox
        self.controller.model.customers = self.controller.model.get_customers()
        self.controller.view.register.customer_combobox['values'] = self.controller.model.customers


        if new_machine:
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/installed_machines/{new_machine}/postprocessor")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/installed_machines/{new_machine}/sample")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/installed_machines/{new_machine}/cse_driver")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/installed_machines/{new_machine}/documentation")
            os.makedirs(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/installed_machines/{new_machine}/graphics")
            if not self.create_dat_file() or not self.create_add_to_machine_database_file() or not self.add_to_ascii_file():
                return

        self.controller.view.messageLabel.config(text="Neues Projekt wurde angelegt.", foreground="orange")

    def create_dat_file(self):
        customer_path = fr"{self.controller.model.settings.get('customer_environment_path')}\{self.controller.view.new_customer.get()}"
        machine_controller = self.controller.view.new_machine_controller.get()
        installed_machines_dir = fr"{customer_path}\5_Umgebung\{self.controller.view.new_version.get()}\MACH\resource\library\machine\installed_machines"
        machine_name = self.controller.view.new_machine.get()
        machine_dir = fr"{installed_machines_dir}\{machine_name}"
        pp_dir = fr"{machine_name}\postprocessor"
        cse_dir = fr"{machine_name}\cse_driver\{machine_controller}"
        try:
            with open(fr"{machine_dir}\{machine_name}.dat", "w") as dat_file:
                dat_file.write(machine_name + ",${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + pp_dir + "\\" + machine_name + ".tcl,${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + pp_dir + "\\" + machine_name + ".def")
                dat_file.write("\nCSE_FILES, ${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + cse_dir + "\\" + machine_name + ".MCF")
            return True
        except Exception as e:
            print("Fehler beim erzeugen der .dat Datei: ", e)
            self.controller.view.messageLabel.config(text="Fehler beim erzeugen der .dat Datei!", foreground="red")
            return False

    def create_add_to_machine_database_file(self):
        customer_path = fr"{self.controller.model.settings.get('customer_environment_path')}\{self.controller.view.new_customer.get()}"
        machine_controller = self.controller.view.new_machine_controller.get()
        installed_machines_dir = fr"{customer_path}\5_Umgebung\{self.controller.view.new_version.get()}\MACH\resource\library\machine\installed_machines"
        machine_name = self.controller.view.new_machine.get()

        machine_type = self.controller.view.new_machine_type.get()
        machinetyp_ID = self.controller.model.machinetypes[machine_type]

        try:
            with open(fr"{installed_machines_dir}\{machine_name}\add_to_machine_database.dat", "w") as dat_file:
                dat_file.write("DATA|" + machine_name + "|" + machinetyp_ID + "|" + machine_name + "|" + machine_controller + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + machine_name + "/" + machine_name + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + machine_name + "/graphics/" + machine_name + "_SIM")
            return True

        except Exception as e:
            print("Fehler beim erzeugen der add_to_machine_database.dat Datei: ", e)
            self.controller.view.messageLabel.config(text="Fehler beim erzeugen der add_to_machine_database.dat Datei!", foreground="red")
            return False

    def add_to_ascii_file(self):
        customer_path = fr"{self.controller.model.settings.get('customer_environment_path')}\{self.controller.view.new_customer.get()}"
        machine_controller = self.controller.view.new_machine_controller.get()
        machine_name = self.controller.view.new_machine.get()
        new_version = self.controller.view.new_version.get()

        machine_type = self.controller.view.new_machine_type.get()
        machinetyp_ID = self.controller.model.machinetypes[machine_type]

        try:
            with open(fr"{customer_path}/5_Umgebung/{new_version}/MACH/resource/library/machine/ascii/machine_database.dat", "a") as dat_file:
                dat_file.write("\nDATA|" + machine_name + "|" + machinetyp_ID + "|" + machine_name + "|" + machine_controller + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + machine_name + "/" + machine_name + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + machine_name + "/graphics/" + machine_name + "_SIM")
            return True

        except Exception as e:
            print("Fehler beim hinzufügen zur Ascii datei: ", e)
            self.controller.view.messageLabel.config(text="Fehler beim hinzufügen zur Ascii datei!", foreground="red")
            return False

