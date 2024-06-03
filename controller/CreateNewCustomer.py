import re
import shutil
import os
from controller.Utils import copy_folder

class CreateNewCustomer:
    """
    This class handles the create new customer Button
    """
    def __init__(self, controller):
        self.controller = controller

    def create_new_customer(self):
        self.controller.view.messageLabel.config(text="")

        if not self.controller.view.new_customer.get() or not self.controller.view.new_order.get() or not self.controller.view.new_version.get() or not self.controller.view.new_machine.get():
            print("Alle Felder müssen ausgefüllt sein!")
            self.controller.view.messageLabel.config(text="Alle Felder müssen ausgefüllt sein!", foreground="red")
            return

        if not re.match(r"^NX\d{2,4}", self.controller.view.new_version.get()):
            print("Die Version muss das Format NXxxxx haben.")
            self.controller.view.messageLabel.config(text="Die Version muss das Format NXxxxx haben.", foreground="red")
            return

        if [i for i in os.listdir(self.controller.model.settings.get('customer_environment_path')) if self.controller.view.new_customer.get().lower() == i.lower()]:
            print("Kunde bereits angelegt!")
            self.controller.view.messageLabel.config(text="Kunde bereits angelegt!", foreground="red")
            return

        customer_path = fr"{self.controller.model.settings.get('customer_environment_path')}/{self.controller.view.new_customer.get()}/"

        os.makedirs(fr"{customer_path}1_Kundendaten/{self.controller.view.new_order.get()}/")
        os.makedirs(fr"{customer_path}2_Testdaten/{self.controller.view.new_order.get()}/")
        os.makedirs(fr"{customer_path}3_Auslieferung/{self.controller.view.new_order.get()}/")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/usertools/")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/ascii")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/inclass")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/{self.controller.view.new_machine.get()}/postprocessor")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/{self.controller.view.new_machine.get()}/sample")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/{self.controller.view.new_machine.get()}/cse_driver")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/{self.controller.view.new_machine.get()}/documentation")
        os.makedirs(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/{self.controller.view.new_machine.get()}/graphics")
        os.makedirs(fr"{customer_path}4_Calls/{self.controller.view.new_order.get()}/")
        os.makedirs(fr"{customer_path}6_Custom/{self.controller.view.new_order.get()}/{self.controller.view.new_version.get()}/roles/")
        os.makedirs(fr"{customer_path}6_Custom/{self.controller.view.new_order.get()}/{self.controller.view.new_version.get()}/startup/")
        os.makedirs(fr"{customer_path}7_Dokumentation/{self.controller.view.new_order.get()}/")

        open(fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/{self.controller.view.new_machine.get()}/add_to_machine_database.dat","w")
        copy_folder(fr"{self.controller.model.settings.get('nx_installation_path')}/{self.controller.view.new_version.get()}/MACH/resource/library/machine/ascii/",fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/ascii/")
        copy_folder(fr"{self.controller.model.settings.get('nx_installation_path')}/{self.controller.view.new_version.get()}/MACH/resource/library/machine/inclass/",fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/inclass/")
        shutil.copy2(fr"{self.controller.model.settings.get('nx_installation_path')}/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines/sim08_mill_5ax/sim08_mill_5ax_sinumerik.dat",fr"{customer_path}5_Umgebung/{self.controller.view.new_version.get()}/MACH/resource/library/machine/installed_machines//{self.controller.view.new_machine.get()}/{self.controller.view.new_machine.get()}.dat")

        self.controller.model.customers = self.controller.model.get_customers()
        self.controller.view.register.customer_combobox['values'] = self.controller.model.customers
        self.controller.view.messageLabel.config(text="Neue Kundenumgebung wurde angelegt.", foreground="orange")