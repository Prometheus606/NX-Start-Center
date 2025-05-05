import re
import shutil
import os
from pathlib import Path

from controller.Utils import copy_folder, copy_file, remove_readonly_recursive

class CreateNewCustomer:
    """
    This class handles the create new customer Button
    """
    def __init__(self, controller):
        self.controller = controller
        self.customer_environment_path = self.controller.model.settings.get('customer_environment_path')
        self.nx_installation_path = self.controller.model.settings.get('nx_installation_path')

        self.new_version = self.controller.view.new_version.get()
        self.new_machine = self.controller.view.new_machine.get()
        self.new_order = self.controller.view.new_order.get()
        self.new_customer = self.controller.view.new_customer.get()
        self.machine_controller = self.controller.view.new_machine_controller.get()
        self.machine_type = self.controller.view.new_machine_type.get()
        self.machinetyp_ID = self.controller.model.machinetypes[self.machine_type]

        self.customer_path = fr"{self.customer_environment_path}/{self.new_customer}"
        self.installed_machines_dir = fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/installed_machines"
        self.machine_dir = fr"{self.installed_machines_dir}/{self.new_machine}"
        self.pp_dir = fr"{self.machine_dir}/postprocessor"
        self.cse_dir = fr"{self.machine_dir}/cse_driver"

    def create_new_customer(self):
        self.controller.view.set_message("")
        customer_is_new = True

        self.customer_environment_path = self.controller.model.settings.get('customer_environment_path')
        self.nx_installation_path = self.controller.model.settings.get('nx_installation_path')

        self.new_version = self.controller.view.new_version.get()
        self.new_machine = self.controller.view.new_machine.get()
        self.new_order = self.controller.view.new_order.get()
        self.new_customer = self.controller.view.new_customer.get()
        self.machine_controller = self.controller.view.new_machine_controller.get()
        self.machine_type = self.controller.view.new_machine_type.get()
        self.machinetyp_ID = self.controller.model.machinetypes[self.machine_type]

        self.customer_path = fr"{self.customer_environment_path}/{self.new_customer}"
        self.installed_machines_dir = fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/installed_machines"
        self.machine_dir = fr"{self.installed_machines_dir}\{self.new_machine}"
        self.pp_dir = fr"{self.machine_dir}/postprocessor"
        self.cse_dir = fr"{self.machine_dir}/cse_driver"

        if not self.new_customer or not self.new_version:
            print("Kundenname und Version muss angegeben sein!")
            self.controller.view.set_message("Kundenname und Version muss angegeben sein!")
            return

        if " " in self.new_customer or " " in self.new_version or " " in self.new_machine or " " in self.new_order:
            print("Leerzeichen sind nicht Zulässig!")
            self.controller.view.set_message("Leerzeichen sind nicht Zulässig!")
            return

        if not re.match(r"^NX\d{2,4}", self.new_version):
            print("Die Version muss das Format NXxxxx haben.")
            self.controller.view.set_message("Die Version muss das Format NXxxxx haben.""red")
            return

        if [i for i in os.listdir(self.customer_environment_path) if self.new_customer.lower() == i.lower()]:
            print("Kunde bereits angelegt!")
            # self.controller.view.messageLabel.config(text="Kunde bereits angelegt!", foreground="red")
            customer_is_new = False

        if Path(fr"{self.installed_machines_dir}/{self.new_machine}").exists():
            print("Maschine bereits angelegt!")
            self.controller.view.set_message("Maschine bereits angelegt!")
            return

        if not self.new_order:
            self.controller.view.new_order.set("0000")
            self.new_order = self.controller.view.new_order.get()

        if not os.path.exists(fr"{self.customer_path}/1_Kundendaten/{self.new_order}/"):
            os.makedirs(fr"{self.customer_path}/1_Kundendaten/{self.new_order}/")
        if not os.path.exists(fr"{self.customer_path}/2_Testdaten/{self.new_order}/"):
            os.makedirs(fr"{self.customer_path}/2_Testdaten/{self.new_order}/")
        if not os.path.exists(fr"{self.customer_path}/3_Auslieferung/{self.new_order}/"):
            os.makedirs(fr"{self.customer_path}/3_Auslieferung/{self.new_order}/")
        if not os.path.exists(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/usertools/"):
            os.makedirs(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/usertools/")
        if not os.path.exists(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/ascii"):
            os.makedirs(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/ascii")
        if not os.path.exists(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/inclass"):
            os.makedirs(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/inclass")
        if not os.path.exists(self.machine_dir):
            os.makedirs(self.machine_dir)
        if not os.path.exists(fr"{self.customer_path}/4_Calls/{self.new_order}/"):
            os.makedirs(fr"{self.customer_path}/4_Calls/{self.new_order}/")
        if not os.path.exists(fr"{self.customer_path}/6_Custom/{self.new_order}/{self.new_version}/roles/"):
            os.makedirs(fr"{self.customer_path}/6_Custom/{self.new_order}/{self.new_version}/roles/")
        if not os.path.exists(fr"{self.customer_path}/6_Custom/{self.new_order}/{self.new_version}/startup/"):
            os.makedirs(fr"{self.customer_path}/6_Custom/{self.new_order}/{self.new_version}/startup/")
        if not os.path.exists(fr"{self.customer_path}/7_Dokumentation/{self.new_order}/"):
            os.makedirs(fr"{self.customer_path}/7_Dokumentation/{self.new_order}/")

        if customer_is_new:
            copy_folder(
                fr"{self.nx_installation_path}/{self.new_version}/MACH/resource/library/machine/ascii/",
                fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/ascii/")
            copy_folder(
                fr"{self.nx_installation_path}/{self.new_version}/MACH/resource/library/machine/inclass/",
                fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/inclass/")

        # refresh the customers combobox
        self.controller.model.customers = self.controller.model.get_customers()
        self.controller.view.register.customer_combobox['values'] = self.controller.model.customers

        copy_file("./src/Leistungsnachweis_AN000000.docx", fr"{self.customer_path}/1_Kundendaten/")

        if self.new_machine:
            os.makedirs(self.pp_dir)
            os.makedirs(fr"{self.machine_dir}/sample")
            os.makedirs(self.cse_dir)
            os.makedirs(fr"{self.machine_dir}/documentation")
            os.makedirs(fr"{self.machine_dir}/graphics")

            self.create_readme_file()
            copy_file("./src/.gitignore", self.machine_dir)

            remove_readonly_recursive(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/ascii")

            if not self.create_dat_file() or not self.create_add_to_machine_database_file() or not self.add_to_ascii_file():
                return

        self.controller.view.set_message("Neues Projekt wurde angelegt.", "orange")



    def create_dat_file(self):
        try:
            with open(fr"{self.machine_dir}\{self.new_machine}.dat", "w") as dat_file:
                dat_file.write(self.new_machine + ",${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + self.new_machine + "\\postprocessor\\" + self.new_machine + ".tcl,${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + self.new_machine + "\\postprocessor\\" + self.new_machine + ".def")
                dat_file.write("\nCSE_FILES, ${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}\\" + self.new_machine + f"\\cse_driver\\{self.machine_controller}\\" + self.new_machine + ".MCF")
            return True
        except Exception as e:
            print("Fehler beim erzeugen der .dat Datei: ", e)
            self.controller.view.set_message("Fehler beim erzeugen der .dat Datei!")
            return False

    def create_add_to_machine_database_file(self):
        try:
            with open(fr"{self.installed_machines_dir}\{self.new_machine}\add_to_machine_database.dat", "w") as dat_file:
                dat_file.write("DATA|" + self.new_machine + "|" + self.machinetyp_ID + "|" + self.new_machine + "|" + self.machine_controller + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + self.new_machine + "/" + self.new_machine + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + self.new_machine + "/graphics/" + self.new_machine + "_SIM")
            return True

        except Exception as e:
            print("Fehler beim erzeugen der add_to_machine_database.dat Datei: ", e)
            self.controller.view.set_message("Fehler beim erzeugen der add_to_machine_database.dat Datei!")
            return False

    def add_to_ascii_file(self):
        try:
            with open(fr"{self.customer_path}/5_Umgebung/{self.new_version}/MACH/resource/library/machine/ascii/machine_database.dat", "a") as dat_file:
                dat_file.write("\nDATA|" + self.new_machine + "|" + self.machinetyp_ID + "|" + self.new_machine + "|" + self.machine_controller + "|Example|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + self.new_machine + "/" + self.new_machine + ".dat|1.000000|${UGII_CAM_LIBRARY_INSTALLED_MACHINES_DIR}" + self.new_machine + "/graphics/" + self.new_machine + "_SIM")
            return True

        except Exception as e:
            print("Fehler beim hinzufügen zur Ascii datei: ", e)
            self.controller.view.set_message("Fehler beim hinzufügen zur Ascii datei!")
            return False

    def create_readme_file(self):
        try:
            with open(fr"{self.machine_dir}\README.md", "w") as readme_file:
                readme_file.write(f"# {self.new_machine}\n")
                readme_file.write(f"\nMaschine: {self.new_machine}")
                readme_file.write(f"\nSteuerung: {self.machine_controller}")
                readme_file.write(f"\nFirma: {self.new_customer}")
                readme_file.write(f"\nPost Configurator: -")
                readme_file.write(f"\nNX-Version: {self.new_version}")
            return True
        except Exception as e:
            print("Fehler beim erzeugen der README.md Datei: ", e)
            self.controller.view.set_message("Fehler beim erzeugen der README.md Datei!")
            return False


