import os
import subprocess
from tkinter import messagebox
from controller.Utils import open_editor

class Licence:
    """
    This class contains the tools from the tools menu bar:
    """
    def __init__(self, controller):
        self.controller = controller

        self.controller.view.menubar.licence_menu[0]["command"] = self.show_license_info
        self.controller.view.menubar.licence_menu[1]["command"] = self.start_lmtools

    def show_license_info(self):
        file_path = self.controller.model.licence_path
        editor = self.controller.model.editor.lower()
        open_editor(self.controller, file_path, editor)


    def start_lmtools(self):
        file_path = self.controller.model.licence_server_path
        try:
            print(os.getcwd())
            subprocess.Popen([f"{os.getcwd()}\src\Open_LMTools.bat", file_path])
        except FileNotFoundError:
            print(f"Der Pfad {file_path} wurde nicht gefunden.")
            # messagebox.showerror("Fehler", f"Der Pfad {file_path} wurde nicht gefunden.")
            self.controller.view.set_message(f"Der Pfad {file_path} wurde nicht gefunden.")