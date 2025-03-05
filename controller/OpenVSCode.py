import os
import subprocess
from tkinter import messagebox

class OpenVSCode:
    """
    This class handles the Open VS Code Button
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.vsCode_btn.config(command=self.open_vsCode)

    def open_vsCode(self):
        self.controller.view.set_message()
        machine_dir = fr"{self.controller.model.get_installed_machines_dir_path()}\{self.controller.model.machine}"
        fork_path = self.controller.model.settings.get("fork_path")
        pp_dir = f"{machine_dir}\\postprocessor\\"
        repoExists = True

        if not os.path.exists(pp_dir) or not os.path.isdir(pp_dir):
            self.controller.view.set_message(f"Der PP Ordner konnte nicht geöffnet werden da er nicht existiert.\n{pp_dir}")
            return

        if not fork_path or not os.path.exists(fork_path) or not os.path.isfile(fork_path):
            self.controller.view.set_message(f"Fork ist nicht installiert oder der Pfad zur Fork.exe ist falsch.")
            return

        if not os.path.exists(f"{machine_dir}/.git"):
            messagebox.showinfo("Info", f"Achtung: Kein Repository angelegt!")
            repoExists = False

        try:
            if repoExists:
                subprocess.Popen([fork_path, machine_dir])
                messagebox.showinfo("Info", f"Nicht vergessen Projekt zu Pullen!")
            # os.system(f"code {pp_dir} && exit")
            subprocess.run(f"code {pp_dir}", shell=True)
        except Exception:
            self.controller.view.set_message(f"Der PP Ordner konnte nicht geöffnet werden. VSCode installiert?\n{pp_dir}")