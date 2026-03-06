import os
import subprocess
from tkinter import messagebox
from controller.Fork import Fork

class VSCode:
    """
    This class handles the Open VS Code Button
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.vsCode_btn.config(command=self.open)

    def open(self):
        self.controller.view.set_message()
        machine_dir = fr"{self.controller.model.get_installed_machines_dir_path()}\{self.controller.model.machine}"
        pp_dir = f"{machine_dir}\\postprocessor\\"

        if not os.path.exists(pp_dir) or not os.path.isdir(pp_dir):
            self.controller.view.set_message(f"Der PP Ordner konnte nicht geöffnet werden da er nicht existiert.\n{pp_dir}")
            return

        fork = Fork(self.controller)
        fork.open()

        try:
            # os.system(f"code {pp_dir} && exit")
            subprocess.run(f"code {pp_dir}", shell=True)
        except Exception:
            self.controller.view.set_message(f"Der PP Ordner konnte nicht geöffnet werden. VSCode installiert?\n{pp_dir}")