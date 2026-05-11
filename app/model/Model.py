import os
from pathlib import Path
from typing import Any

from controller.Config_file_handler import load_config
from core.settings_schema import SETTING_KEYS, setting_defaults


class Model:
    """
    Central application state.

    Persistent settings are schema-driven.
    Add new settings only in core/settings_schema.py.
    """

    _setting_keys = set(SETTING_KEYS)

    def __init__(self, config_file: str, app_version, app_date, app_author, app_support_mail):
        object.__setattr__(self, "config_file", config_file)

        loaded_settings = load_config(self.config_file, "settings") or {}
        settings = {
            **setting_defaults(),
            **loaded_settings,
        }

        object.__setattr__(self, "settings", settings)
        object.__setattr__(
            self,
            "last_configuration",
            load_config(self.config_file, "last_configuration") or {}
        )

        self.customer = ""
        self.version = ""
        self.machine = ""
        self.native_version = ""
        self.postbuilder_version = ""

        self.language = self.last_configuration.get("last_language", "german")
        self.load_pp = bool(int(self.last_configuration.get("last_load_pp", 0)))
        self.load_installed_machines = bool(int(self.last_configuration.get("last_load_installed_machines", 1)))
        self.load_tool = bool(int(self.last_configuration.get("last_load_tool", 0)))
        self.load_device = bool(int(self.last_configuration.get("last_load_device", 0)))
        self.load_feed = bool(int(self.last_configuration.get("last_load_feed", 0)))

        self.order_number = "0000"
        self.use_role = 1

        self.dark_themes = ["solar", "darkly", "cyborg"]
        self.light_themes = ["cosmo", "flatly", "minty", "pulse", "lumen"]
        self.themes = [*self.light_themes, *self.dark_themes]
        self.editors = ["Notepad", "Notepad++", "VSCode"]

        self.theme = self._resolve_theme()
        self.settings["preferred_theme"] = self.theme

        self.customers = self.get_customers()
        self.native_versions = self.get_native_versions()
        self.versions = self.get_versions()
        self.machines = self.get_machines()
        self.postbuilder_versions = self.get_postbuilder_versions()

        self.app_version = app_version
        self.app_date = app_date
        self.app_author = app_author
        self.app_support_mail = app_support_mail

        self.machinetypes = {
            "Mill machine": "MDM0101",
            "TurnMill machine": "MDM0104",
            "Lathe machine": "MDM0201",
            "MillTurn machine": "MDM0204",
            "Wedm machine": "MDM0301",
            "Robot machine": "MDM0401",
            "Generic machine": "MDM0901",
        }

        self.machine_controllers = [
            "Sinumerik",
            "Fanuk",
            "Okuma",
            "HeidenhainTNC",
        ]

    def __getattr__(self, name: str) -> Any:
        if name in self._setting_keys:
            return self.settings.get(name)

        raise AttributeError(name)

    def __setattr__(self, name: str, value: Any) -> None:
        if name in self._setting_keys and "settings" in self.__dict__:
            self.settings[name] = value

            if name == "preferred_theme":
                object.__setattr__(self, "theme", value)

            return

        object.__setattr__(self, name, value)

    def _resolve_theme(self):
        preferred = self.settings.get("preferred_theme")
        return preferred if preferred in self.themes else "darkly"

    def set_theme(self):
        self.theme = self._resolve_theme()

    def get_postbuilder_versions(self):
        version_list = self.native_versions
        last = self.last_configuration.get("last_postbuilder_version")
        self.postbuilder_version = (
            last if last in version_list else version_list[0] if version_list else ""
        )
        return version_list

    def get_native_versions(self):
        base_path = self.settings.get("nx_installation_path", "")

        if Path(base_path).exists() and Path(base_path).is_dir():
            version_list = [
                i for i in os.listdir(base_path)
                if Path(base_path, i).is_dir() and i.lower().startswith("nx")
            ]
            version_list = version_list[::-1]

            last = self.last_configuration.get("last_native_version")
            self.native_version = (
                last if last in version_list else version_list[0] if version_list else ""
            )

            return version_list

        print("Der Pfad zu den NX Versionen existiert nicht!")
        return []

    def get_customers(self):
        base_path = self.settings.get("customer_environment_path", "")

        if Path(base_path).exists() and Path(base_path).is_dir():
            customer_list = [
                i for i in os.listdir(base_path)
                if Path(base_path, i).is_dir()
            ]

            if not customer_list:
                print("Keine Kundenumgebung angelegt.")
                return []

            last = self.last_configuration.get("last_customer")
            self.customer = (
                last if last in customer_list else customer_list[0]
            )

            return customer_list

        print("Der Pfad zu der Kundenumgebung existiert nicht!")
        return []

    def get_versions(self):
        base_path = Path(
            self.settings.get("customer_environment_path", ""),
            self.customer,
            "5_Umgebung"
        )

        if base_path.exists() and base_path.is_dir():
            version_list = [
                i for i in os.listdir(base_path)
                if Path(base_path, i).is_dir() and i.lower().startswith("nx")
            ]

            if not version_list:
                print("Keine Version angelegt.")
                return []

            version_list = version_list[::-1]

            last = self.last_configuration.get("last_version")
            self.version = (
                last if last in version_list else version_list[0]
            )

            return version_list

        print("Keine NX Version gefunden, überprüfe deine Ordnerstruktur.")
        return []

    def get_machines(self):
        base_path = self.get_installed_machines_dir_path()

        if Path(base_path).exists() and Path(base_path).is_dir():
            machines_list = [
                i for i in os.listdir(base_path)
                if Path(base_path, i).is_dir()
            ]

            if not machines_list:
                print("Keine Maschine angelegt.")
                return []

            last = self.last_configuration.get("last_machine")
            self.machine = (
                last if last in machines_list else machines_list[0]
            )

            return machines_list

        print("Keine Maschinen gefunden, überprüfe deine Ordnerstruktur.")
        return []

    def get_installed_machines_dir_path(self):
        base = Path(
            self.settings.get("customer_environment_path", ""),
            self.customer,
            "5_Umgebung",
            self.version,
            "MACH",
            "resource",
            "library",
            "machine",
            "installed_machines",
        )

        cam_path = Path(f"{base}_{self.customer}")

        return str(cam_path if cam_path.exists() else base)