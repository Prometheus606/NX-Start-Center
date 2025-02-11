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
        self.controller.view.set_message()
        machine_dir = fr"{self.controller.model.get_installed_machines_dir_path()}\{self.controller.model.machine}"
        pp_dir = f"{machine_dir}\\postprocessor\\"
        try:
            if not os.path.isdir(pp_dir) or not os.path.isdir(pp_dir):
                raise Exception

            # os.system(f"code {pp_dir} && exit")
            subprocess.run(f"code {pp_dir}", shell=True)
        except Exception:
            self.controller.view.set_message(f"Der PP Ordner konnte nicht geöffnet werden.\nVSCode ist nicht installiert oder der Ordner existiert nicht.\n{pp_dir}")