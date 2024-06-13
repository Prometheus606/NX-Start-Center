import subprocess
from controller.Config_file_handler import save_config
import getpass
from pathlib import Path
import os
import shutil

class NativeStart:
    """
    This class handles the Start NX Native Frame
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.native_frame.start_nativ_btn.config(command=self.start_NX_nativ)
        self.controller.view.native_frame.nxversion_native_combobox.bind("<<ComboboxSelected>>", self.native_version_selected)

    def start_NX_nativ(self):
        self.controller.view.messageLabel.config(text="")

        base_path = self.controller.model.settings['nx_installation_path']
        try:
            nx_version = (int(self.controller.model.native_version.replace("NX", "").strip()))
            if nx_version < 2206:
                nx_path = fr"{base_path}\NX{nx_version}\UGII\ugraf.exe"
            else:
                nx_path = fr"{base_path}\NX{nx_version}\NXBIN\ugraf.exe"

            subprocess.Popen(nx_path, stderr=subprocess.PIPE, shell=False)
            save_config(self.controller.model.config_file, "last_configuration", last_native_version=self.controller.model.native_version)

        except FileNotFoundError:
            self.controller.view.messageLabel.configure(text="Version kann nicht gestartet werden!", foreground="red")

    def native_version_selected(self, e):
        self.controller.view.messageLabel.config(text="")
        self.controller.model.native_version = self.controller.view.native_version.get()