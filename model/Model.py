import os
from pathlib import Path
from controller.Config_file_handler import load_config

class Model:
    def __init__(self, config_file: str, app_version, app_date, app_author, app_support_mail):
        self.config_file = config_file
        self.settings = load_config(self.config_file, "settings")
        self.last_configuration = load_config(self.config_file, "last_configuration")
        self.customer = ""
        self.version = ""
        self.machine = ""
        self.native_version = ""
        self.language = ""
        self.load_pp = 0
        self.load_installed_machines = 0
        self.load_tool = 0
        self.load_device = 0
        self.load_feed = 0
        self.order_number = "0000"
        self.batchstart = 0
        self.customers = self.get_customers()
        self.native_versions = self.get_native_versions()
        self.versions = self.get_versions()
        self.machines = self.get_machines()
        self.editors = ["Notepad", "Notepad++", "VSCode"]
        self.editor = "notepad"

        self.nx_installation_path = self.settings.get("nx_installation_path")
        self.customer_environment_path = self.settings.get("customer_environment_path")
        self.licence_server_path = self.settings.get("licence_server_path")
        self.licence_path = self.settings.get("licence_path")

        self.dark_themes = ["solar", "darkly", "cyborg"]
        self.light_themes = ["cosmo", "flatly", "minty", "pulse", "lumen"]
        self.theme = ""
        self.set_theme()

        self.app_version = app_version
        self.app_date = app_date
        self.app_author = app_author
        self.app_support_mail = app_support_mail

        self.machinetypes = {"Mill machine": "MDM0101",
                                  "TurnMill machine": "MDM0104",
                                  "Lathe machine": "MDM0201",
                                  "MillTurn machine": "MDM0204",
                                  "Wedm machine": "MDM0301",
                                  "Robot machine": "MDM0401",
                                  "Generic machine": "MDM0901"}
        self.machine_controllers = ["Sinumerik", "Fanuk", "Okuma", "HeidenhainTNC"]

    def set_theme(self):
        """
        If a preferred theme is set in the configuration file, that theme will be applied. Otherwise, the darkly theme is applied.
        """
        if self.settings and self.settings.get("preferred_theme") is not None and (self.settings.get("preferred_theme") in self.light_themes or self.settings.get("preferred_theme") in self.dark_themes):
            self.theme = self.settings.get("preferred_theme")
        else:
            self.theme = "darkly"

    def get_native_versions(self):
        """
        :return: the installed NX Versions from the given path in the settings
        """
        base_path = self.settings["nx_installation_path"]
        if Path(base_path).exists() and Path(base_path).is_dir():
            version_list = []
            folder_content = os.listdir(base_path)
            for i in folder_content:
                if Path(f"{base_path}\\{i}").is_dir():
                    version_list.append(i)
            if self.last_configuration and self.last_configuration.get("last_native_version") is not None and self.last_configuration.get("last_native_version") in version_list:
                self.native_version = self.last_configuration["last_native_version"]
            else:
                self.native_version = version_list[0]
            return version_list
        else:
            print("Der Pfad zu den NX versionen existiert nicht!")
            return []

    def get_customers(self):
        """
        :return: the customers from the given path in the settings
        """
        base_path = self.settings["customer_environment_path"]
        if Path(base_path).exists() and Path(base_path).is_dir():
            customer_list = []
            folder_content = os.listdir(base_path)
            for i in folder_content:
                if Path(f"{base_path}\\{i}").is_dir():
                    customer_list.append(i)
            if self.last_configuration and self.last_configuration.get("last_customer") is not None and self.last_configuration.get("last_customer") in customer_list:
                self.customer = self.last_configuration["last_customer"]
            else:
                self.customer = customer_list[0]
            return customer_list
        else:
            print("Der Pfad zu der Kundenumgebung existiert nicht!")
            return []

    def get_versions(self):
        """
        :return: the installed NX Versions from the selected customer
        """
        base_path = f"{self.settings['customer_environment_path']}/{self.customer}/5_Umgebung"
        if Path(base_path).exists() and Path(base_path).is_dir():
            version_list = []
            folder_content = os.listdir(base_path)
            for i in folder_content:
                if Path(f"{base_path}\\{i}").is_dir():
                    version_list.append(i)
            if self.last_configuration and self.last_configuration.get("last_version") is not None and self.last_configuration.get("last_version") in version_list:
                self.version = self.last_configuration["last_version"]
            else:
                self.version = version_list[0]
            return version_list
        else:
            print("Keine NX Version gefunden, überprüfe deine Ordnerstruktur.")
            return []

    def get_machines(self):
        """
        :return: the installed machines from the selected customer
        """
        base_path = self.get_installed_machines_dir_path()
        if Path(base_path).exists() and Path(base_path).is_dir():
            machines_list = []
            folder_content = os.listdir(base_path)
            for i in folder_content:
                if Path(f"{base_path}\\{i}").is_dir():
                    machines_list.append(i)
            if self.last_configuration and self.last_configuration.get("last_machine") is not None and self.last_configuration.get("last_machine") in machines_list:
                self.machine = self.last_configuration["last_machine"]
            else:
                self.machine = machines_list[0]
            return machines_list
        else:
            print("Keine Maschinen gefunden, überprüfe deine Ordnerstruktur.")
            return []

    def get_installed_machines_dir_path(self):
        """
        :return: the installed machines dir path
        """
        return fr"{self.settings['customer_environment_path']}/{self.customer}/5_Umgebung/{self.version}/MACH\resource\library\machine\installed_machines"
