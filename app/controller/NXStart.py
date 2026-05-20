from tkinter import messagebox
from controller.Config_file_handler import save_config
from controller.Copy_Roles import Copy_Roles
import subprocess
import winreg
import sys
import os
from pathlib import Path


class NXStart:
    """
    This class handles the Start NX Button
    """

    def __init__(self, controller):
        self.controller = controller
        self.controller.view.start_btn.config(command=self.start_NX_customer)

    def python_is_installed(self):
        try:
            with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\Python\PythonCore") as key:
                return True
        except FileNotFoundError:
            return False

    def start_NX_customer(self):
        debug_terminal = self.controller.model.startNXWithDebug

        self.controller.view.set_message()

        if getattr(sys, 'frozen', False):
            base_path = ""  # PyInstaller temp folder
        else:
            base_path = "app"

        script = os.path.join(os.getcwd(), base_path, "start_routine.bat")

        # Rolle(n)in die jeweilige NX version kopieren
        Copy_Roles(self.controller, self.controller.model.version)

        args = [
            self.controller.model.customer,
            self.controller.model.version,
            self.controller.model.language,
            self.controller.model.settings.get("customer_environment_path"),
            self.controller.model.settings.get("nx_installation_path"),
            str(int(self.controller.model.load_pp)),
            str(int(self.controller.model.load_installed_machines)),
            str(int(self.controller.model.load_tool)),
            str(int(self.controller.model.load_device)),
            str(int(self.controller.model.load_feed)),
        ]


        try:
            # to debug an batch file, rename the file suffix for test
                cmd = [script] + args
                workdir = Path(
                    self.controller.model.settings.get("customer_environment_path"),
                    self.controller.model.customer
                ).resolve()
                cmd = [str(script)] + [str(a) for a in args]
                cmdline = subprocess.list2cmdline([str(script)] + [str(a) for a in args])

                if debug_terminal:
                    debug_cmd = subprocess.list2cmdline(cmd)

                    subprocess.Popen(
                        ["cmd.exe", "/k", debug_cmd],
                        cwd=str(workdir),
                        creationflags=subprocess.CREATE_NEW_CONSOLE
                    )
                else:
                    subprocess.Popen(cmd, shell=False, creationflags=subprocess.CREATE_NO_WINDOW, cwd=str(workdir))

        except FileNotFoundError:
            messagebox.showerror("Fehler", f"Die Datei {script} konnte nicht gefunden werden.")
        except Exception as ex:
            messagebox.showerror("Fehler", ex)

        save_config(
            self.controller.model.config_file,
            "last_configuration",
            last_customer=self.controller.model.customer,
            last_version=self.controller.model.version,
            last_machine=self.controller.model.machine,
            last_load_pp=int(self.controller.model.load_pp),
            last_load_installed_machines=int(self.controller.model.load_installed_machines),
            last_load_tool=int(self.controller.model.load_tool),
            last_load_device=int(self.controller.model.load_device),
            last_load_feed=int(self.controller.model.load_feed),
            last_language=self.controller.model.language
        )
