from tkinter import messagebox
from controller.Config_file_handler import save_config
import shutil
import subprocess
import getpass

class NXStart:
    """
    This class handles the Start NX Button
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.start_btn.config(command=self.start_NX_customer)

    def start_NX_customer(self):
        batchstart = self.controller.model.batchstart

        self.controller.view.messageLabel.config(text="")
        script = "startbatch.bat" if batchstart else "startbatch.py"

        # Rolle bei mir in die jeweilige NX version kopieren
        username = getpass.getuser()
        if username == "niklas.beitler":
            source_folder = fr"D:/Siemens/Meine_Rolle/roles/"
            destination_folder = fr"C:/Users/niklas.beitler/AppData/Local/Siemens/{self.controller.model.version}/"
            shutil.copytree(source_folder, destination_folder, dirs_exist_ok=True)

        command = [
            "python" if script.endswith(".py") else script,
            script,
            self.controller.model.customer,
            self.controller.model.version,
            self.controller.model.language,
            self.controller.model.settings.get("customer_environment_path"),
            self.controller.model.settings.get("nx_installation_path"),
            str(int(self.controller.model.load_pp)),
            str(int(self.controller.model.load_cse)),
            str(int(self.controller.model.load_tool)),
            str(int(self.controller.model.load_device)),
            str(int(self.controller.model.load_feed))
        ]

        try:
            process = subprocess.run(command, shell=False, capture_output=True, text=True)
            stdout = process.stdout
            stderr = process.stderr

            if stdout:
                print(stdout)
                messagebox.showinfo("Info", f"{stdout}")

            if process.returncode == 3:
                print("Fehler beim starten von NX")
                messagebox.showerror("Fehler",
                                     f"Die Datei 'ugraf.exe' konnte nicht gefunden werden.\nBitte überprüfe den Pfad in der startbatch.")
            elif process.returncode != 0:
                print("Fehler beim starten von NX")
                print(stderr)
                messagebox.showerror("Fehler", f"Fehler beim starten von NX:\n{stderr}")

        except FileNotFoundError:
            messagebox.showerror("Fehler", f"Die Datei {script} konnte nicht gefunden werden.")

        # if not startbatch.start(
        #             self.controller.model.customer,
        #             NX_VERSION,
        #             self.controller.model.language,
        #             self.controller.model.settings.get("customer_environment_path"),
        #             NXPFAD,
        #             self.controller.model.load_pp,
        #             self.controller.model.load_cse,
        #             self.controller.model.load_tool,
        #             self.controller.model.load_device,
        #             self.controller.model.load_feed):
        #
        #     print("Fehler beim starten von NX")
        #     messagebox.showinfo("Info", f"Die Datei\n'{nx_path}'\nkonnte nicht gefunden werden.")

        save_config(
            self.controller.model.config_file,
            "last_configuration",
            last_customer=self.controller.model.customer,
            last_version=self.controller.model.version,
            last_machine=self.controller.model.machine,
            last_load_pp=int(self.controller.model.load_pp),
            last_load_cse=int(self.controller.model.load_cse),
            last_load_tool=int(self.controller.model.load_tool),
            last_load_device=int(self.controller.model.load_device),
            last_load_feed=int(self.controller.model.load_feed),
            last_language=self.controller.model.language
        )