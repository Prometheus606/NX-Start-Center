import os
import sys
from pathlib import Path
import tkinter as tk
from tkinter import filedialog

import ttkbootstrap as ttk

from controller.Config_file_handler import save_config
from controller.Utils import open_editor
from core.settings_schema import SETTINGS, SETTING_BY_KEY


class Settings:
    """
    Settings controller.

    The settings screen is schema-driven: fields, defaults, dialogs and
    save-on-change behaviour are defined once in core/settings_schema.py.
    """

    def __init__(self, controller):
        self.controller = controller

        for key, button in self.controller.view.setting_frame.browse_buttons.items():
            button.config(command=lambda setting_key=key: self.select_folder_or_file(setting_key))

        self.controller.view.setting_frame.return_btn.config(command=self.hide_settings)

        for setting in SETTINGS:
            if setting.save_on_change:
                self.controller.view.setting_vars[setting.key].trace_add(
                    "write",
                    lambda *_args, setting_key=setting.key: self.on_setting_changed(setting_key),
                )

        self.controller.view.menubar.settings_menu[0]["command"] = self.show_settings
        self.controller.view.menubar.settings_menu[1]["command"] = self.edit_startbatch
        self.controller.view.menubar.settings_menu[2]["command"] = self.edit_user_settings_file
        self.controller.view.menubar.file_menu[1]["command"] = self.show_settings
        self.controller.view.menubar.file_menu[0]["command"] = self.hide_settings

        self.apply_theme()

    def edit_startbatch(self):
        suffix = ".bat"
        if getattr(sys, 'frozen', False):
            base_path = ""  # PyInstaller temp folder
        else:
            base_path = "app"
        file_path = os.path.join(os.getcwd(), base_path, f"start_routine{suffix}")
        open_editor(self.controller, file_path, self.controller.model.editor.lower())

    def edit_user_settings_file(self):
        suffix = ".bat"
        if getattr(sys, 'frozen', False):
            base_path = ""  # PyInstaller temp folder
        else:
            base_path = "app"
        file_path = os.path.join(os.getcwd(), base_path, f"user_settings{suffix}")
        open_editor(self.controller, file_path, self.controller.model.editor.lower())

    def hide_settings(self):
        values = self.controller.state.save_all_settings()

        self.controller.model.theme = values["preferred_theme"]

        self.controller.ui_changes.reload_environment()

        self.controller.view.set_message()
        self.controller.view.setting_frame.pack_forget()
        self.controller.view.left_frame.pack(
            side=tk.LEFT,
            fill=tk.BOTH,
            expand=True,
            padx=10,
            pady=10
        )
        self.controller.view.right_frame.pack(
            side=tk.RIGHT,
            fill=tk.BOTH,
            expand=True,
            padx=10,
            pady=10
        )


    def show_settings(self):
        self.controller.view.set_message()
        self.controller.view.left_frame.pack_forget()
        self.controller.view.right_frame.pack_forget()
        self.controller.view.setting_frame.pack(side=tk.TOP, fill=tk.BOTH, expand=True, padx=10, pady=10)

    def on_setting_changed(self, key):
        value = self.controller.state.get(key)

        self.controller.state.save_setting(key, value)

        if key == "preferred_theme":
            self.controller.model.theme = value
            self.apply_theme()

        self.controller.view.set_message()
        self.controller.view.set_message()

    def apply_theme(self):
        self.controller.view.style = ttk.Style(self.controller.model.theme)
        if self.get_current_theme_mode() == "dark":
            self.controller.view.menubar.fg_color = "white"
            self.controller.view.dark_title_bar()
        else:
            self.controller.view.menubar.fg_color = "black"
            self.controller.view.light_title_bar()

    # Compatibility methods for existing tests or external callers.
    def on_theme_change(self, *args):
        self.on_setting_changed("preferred_theme")

    def on_editor_change(self, *args):
        self.on_setting_changed("editor")

    def batchstart_modified(self):
        self.on_setting_changed("batchstart")

    def startNXWithDebug_modified(self):
        self.on_setting_changed("startNXWithDebug")

    def select_folder_or_file(self, variable):
        setting = SETTING_BY_KEY[variable]
        path = self._ask_path(setting)

        if not path:
            return

        if setting.dialog == "files":
            current = self.controller.state.get(variable) or ""
            selected = [
                str(p)
                for p in path
                if self._is_valid_path(p, setting)
            ]
            value = ";".join(filter(None, [current, *selected]))
        else:
            value = str(path)

            if not self._is_valid_path(Path(value), setting):
                return

        self.controller.state.save_setting(variable, value)

    def _ask_path(self, setting):
        if setting.dialog == "directory":
            value = filedialog.askdirectory(title=f"{setting.label} auswählen")
            return Path(value) if value else None
        if setting.dialog == "file":
            handle = filedialog.askopenfile(title=f"{setting.label} auswählen", filetypes=setting.filetypes or None)
            return Path(handle.name) if handle else None
        if setting.dialog == "files":
            handles = filedialog.askopenfiles(title=f"{setting.label} auswählen", filetypes=setting.filetypes or None)
            return [Path(handle.name) for handle in handles]
        return None

    def _is_valid_path(self, path: Path, setting):
        if setting.dialog == "directory":
            return path.exists() and path.is_dir()
        if setting.dialog in {"file", "files"}:
            if not (path.exists() and path.is_file()):
                return False
            if setting.filetypes:
                allowed_suffixes = [pattern.replace("*", "").lower() for _, pattern in setting.filetypes]
                return path.suffix.lower() in allowed_suffixes
        return True

    def _var_value(self, key):
        return self.controller.state.get(key)

    def get_current_theme_mode(self):
        theme_name = self.controller.view.style.theme_use()
        return "dark" if theme_name.lower() in self.controller.model.dark_themes else "light"
