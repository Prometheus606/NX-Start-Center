import json
import os
from pathlib import Path
from tkinter import messagebox, TclError
from typing import Any

from core.settings_schema import setting_defaults


class JsonConfigStore:
    def __init__(self, config_file: str):
        self.path = Path(config_file)

    def load_group(self, group: str) -> dict[str, Any]:
        data = self._load_all()
        if group not in data or not isinstance(data[group], dict):
            data[group] = self._default_group(group)
            self._save_all(data)
        elif group == "settings":
            changed = False
            defaults = setting_defaults()
            for key, value in defaults.items():
                if key not in data[group]:
                    data[group][key] = value
                    changed = True
            if changed:
                self._save_all(data)
        return data[group]

    def save_group_values(self, group: str, **values: Any) -> None:
        data = self._load_all()
        if group not in data or not isinstance(data[group], dict):
            data[group] = self._default_group(group)
        data[group].update(values)
        self._save_all(data)

    def _default_group(self, group: str) -> dict[str, Any]:
        if group == "settings":
            return setting_defaults()
        if group == "last_configuration":
            return {}
        return {}

    def _load_all(self) -> dict[str, Any]:
        while True:
            try:
                with self.path.open("r", encoding="utf-8") as handle:
                    data = json.load(handle)
                    return data if isinstance(data, dict) else {}
            except FileNotFoundError:
                self.path.parent.mkdir(parents=True, exist_ok=True)
                self._save_all({})
                print(f"Die Datei '{self.path}' wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")
                self._show_info("Hinweis", f"Die Datei '{self.path}' wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")
            except json.decoder.JSONDecodeError:
                self._save_all({})
                print(f"Die Datei '{self.path}' war falsch formatiert und wurde neu erzeugt.")
                self._show_info("Hinweis", f"Die Datei '{self.path}' war falsch formatiert und wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")

    def _save_all(self, data: dict[str, Any]) -> None:
        self.path.parent.mkdir(parents=True, exist_ok=True)
        with self.path.open("w", encoding="utf-8") as handle:
            json.dump(data, handle, indent=4)

    def _show_info(self, title: str, message: str) -> None:
        try:
            messagebox.showinfo(title, message)
        except TclError:
            # Allows non-GUI tests and CI checks to instantiate the config store.
            pass
