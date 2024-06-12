import ttkbootstrap as ttk
from pathlib import Path
from tkinter import filedialog
from controller.Config_file_handler import save_config
from controller.Utils import open_editor

class Settings:
    """
    This class handles the setting frame with all the actions
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.setting_frame.browse_nx_installation_path_btn.config(command=lambda: self.select_folder_or_file("nx_installation_path"))
        self.controller.view.setting_frame.browse_customer_environment_path_btn.config(command=lambda: self.select_folder_or_file("customer_environment_path"))
        self.controller.view.setting_frame.browse_licence_path_btn.config(command=lambda: self.select_folder_or_file("licence_path", "file"))
        self.controller.view.setting_frame.browse_licence_server_path_btn.config(command=lambda: self.select_folder_or_file("licence_server_path", "file"))
        self.controller.view.nx_installation_path.set(self.controller.model.nx_installation_path)
        self.controller.view.customer_environment_path.set(self.controller.model.customer_environment_path)
        self.controller.view.licence_path.set(self.controller.model.licence_path)
        self.controller.view.licence_server_path.set(self.controller.model.licence_server_path)
        self.controller.view.setting_frame.return_btn.config(command=self.hide_settings)
        self.controller.view.theme.trace_add("write", self.on_theme_change)
        self.controller.view.editor.trace_add("write", self.on_editor_change)
        self.controller.view.setting_frame.batchstart_check.config(command=self.batchstart_modified)
        self.controller.view.setting_frame.use_role_check.config(command=self.use_role_modified)

        self.controller.view.menubar.settings_menu[0]["command"] = self.show_settings
        self.controller.view.menubar.settings_menu[1]["command"] = self.edit_startbatch
        self.controller.view.menubar.file_menu[1]["command"] = self.show_settings
        self.controller.view.menubar.file_menu[0]["command"] = self.hide_settings

        self.on_theme_change()

    def edit_startbatch(self):
        import os

        suffix = ".bat" if self.controller.model.batchstart else ".py"
        file_path = os.path.join(os.getcwd(), f"startbatch{suffix}")
        editor = self.controller.model.editor.lower()

        open_editor(file_path, editor)

    def hide_settings(self):

        self.controller.model.nx_installation_path = self.controller.view.nx_installation_path.get()
        self.controller.model.customer_environment_path = self.controller.view.customer_environment_path.get()
        self.controller.model.licence_path = self.controller.view.licence_path.get()
        self.controller.model.licence_server_path = self.controller.view.licence_server_path.get()
        save_config(self.controller.model.config_file, "settings", licence_server_path=self.controller.model.licence_server_path, licence_path=self.controller.model.licence_path, nx_installation_path=self.controller.model.nx_installation_path, customer_environment_path=self.controller.model.customer_environment_path, batchstart=self.controller.model.batchstart, use_role=self.controller.model.use_role)

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

    def on_editor_change(self, *args):
        self.controller.model.editor = self.controller.view.editor.get()
        save_config(self.controller.model.config_file, "settings", editor=self.controller.model.editor)

    def select_folder_or_file(self, variable, art="folder"):
        if art == "file":
            path = filedialog.askopenfile()
        else:
            path = filedialog.askdirectory()

        if path and Path(path).exists():
            if variable == "customer_environment_path" and Path(path).is_dir():
                self.controller.model.customer_environment_path = path
                self.controller.view.customer_environment_path.set(path)
                save_config(self.controller.model.config_file, "settings", customer_environment_path=self.controller.model.customer_environment_path)

            elif variable == "nx_installation_path" and Path(path).is_dir():
                self.controller.model.nx_installation_path = path
                self.controller.view.nx_installation_path.set(path)
                save_config(self.controller.model.config_file, "settings", nx_installation_path=self.controller.model.nx_installation_path)

            elif variable == "licence_server_path" and Path(path).is_file():
                self.controller.model.licence_server_path = path
                self.controller.view.licence_server_path.set(path)
                save_config(self.controller.model.config_file, "settings", licence_server_path=self.controller.model.licence_server_path)

            elif variable == "licence_path" and Path(path).is_file():
                self.controller.model.licence_path = path
                self.controller.view.licence_path.set(path)
                save_config(self.controller.model.config_file, "settings", licence_path=self.controller.model.licence_path)

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

    def use_role_modified(self):
        """
        Defines what happens if the use_role Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.use_role = self.controller.view.use_role.get()
        save_config(self.controller.model.config_file, "settings", use_role=self.controller.model.use_role)