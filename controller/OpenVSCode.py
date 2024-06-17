import os
import subprocess

class OpenVSCode:
    """
    This class handles the Open VS Code Button
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.vsCode_btn.config(command=self.open_vsCode)

    def open_vsCode(self):
        self.controller.view.messageLabel.config(text="")
        machine_dir = fr"{self.controller.model.get_installed_machines_dir_path()}\{self.controller.model.machine}"
        pp_dir = f"{machine_dir}\\postprocessor\\"
        try:
            # os.system(f"code {pp_dir} && exit")
            subprocess.run(f"code {pp_dir}", shell=True)
        except Exception:
            self.controller.view.messageLabel.configure(text=f"Die Datei konnte nicht in VSCode geöffnet werden.\n{pp_dir}", foreground="red")