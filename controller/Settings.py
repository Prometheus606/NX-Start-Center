import ttkbootstrap as ttk
from pathlib import Path
from tkinter import filedialog
from controller.Config_file_handler import save_config

class Settings:
    """
    This class handles the setting frame with all the actions
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.setting_frame.browse_nx_installation_path_btn.config(command=lambda: self.select_folder("nx_installation_path"))
        self.controller.view.setting_frame.browse_customer_environment_path_btn.config(command=lambda: self.select_folder("customer_environment_path"))
        self.controller.view.nx_installation_path.set(self.controller.model.nx_installation_path)
        self.controller.view.customer_environment_path.set(self.controller.model.customer_environment_path)
        self.controller.view.setting_frame.return_btn.config(command=self.hide_settings)
        self.controller.view.theme.trace_add("write", self.on_theme_change)
        self.controller.view.setting_frame.batchstart_check.config(command=self.batchstart_modified)

        self.controller.view.menubar.menus[1]["command"] = self.show_settings
        self.controller.view.menubar.file_menu[1]["command"] = self.show_settings
        self.controller.view.menubar.file_menu[0]["command"] = self.hide_settings

        self.on_theme_change()

    def hide_settings(self):

        self.controller.model.nx_installation_path = self.controller.view.nx_installation_path.get()
        self.controller.model.customer_environment_path = self.controller.view.customer_environment_path.get()
        save_config(self.controller.model.config_file, "settings", nx_installation_path=self.controller.model.nx_installation_path, customer_environment_path=self.controller.model.customer_environment_path, batchstart=self.controller.model.batchstart)

        self.controller.view.messageLabel.config(text="")
        self.controller.view.setting_frame.pack_forget()
        self.controller.view.left_frame.pack(side=ttk.LEFT, fill=ttk.BOTH, expand=True, padx=10, pady=10)
        self.controller.view.right_frame.pack(side=ttk.RIGHT, fill=ttk.BOTH, expand=True, padx=10, pady=10)

    def show_settings(self):
        self.controller.view.messageLabel.config(text="")
        self.controller.view.left_frame.pack_forget()
        self.controller.view.right_frame.pack_forget()
        self.controller.view.setting_frame.pack(side=ttk.TOP, fill=ttk.BOTH, expand=True, padx=10, pady=10)

    def on_theme_change(self, *args):
        self.controller.model.theme = self.controller.view.theme.get()
        save_config(self.controller.model.config_file, "settings", preferred_theme=self.controller.model.theme)
        self.controller.view.style = ttk.Style(self.controller.model.theme)

        if self.get_current_theme_mode() == "dark":
            self.controller.view.menubar.fg_color = "white"
            self.controller.view.dark_title_bar()
        else:
            self.controller.view.menubar.fg_color = "black"
            self.controller.view.light_title_bar()

    def select_folder(self, variable):
        folder_path = filedialog.askdirectory()
        if folder_path and Path(folder_path).exists() and Path(folder_path).is_dir():
            if variable == "customer_environment_path":
                self.controller.model.customer_environment_path = folder_path
                self.controller.view.customer_environment_path.set(folder_path)
                save_config(self.controller.model.config_file, "settings", customer_environment_path=self.controller.model.customer_environment_path)

            elif variable == "nx_installation_path":
                self.controller.model.nx_installation_path = folder_path
                self.controller.view.nx_installation_path.set(folder_path)
                save_config(self.controller.model.config_file, "settings", nx_installation_path=self.controller.model.nx_installation_path)

    def get_current_theme_mode(self):
        theme_name = self.controller.view.style.theme_use()
        if theme_name.lower() in self.controller.model.dark_themes:
            return "dark"
        else:
            return "light"

    def batchstart_modified(self):
        """
        Defines what happens if the batchstart Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.batchstart = self.controller.view.batchstart.get()
        save_config(self.controller.model.config_file, "settings", batchstart=self.controller.model.batchstart)