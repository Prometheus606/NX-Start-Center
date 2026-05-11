from tkinter import messagebox

from core.config_store import JsonConfigStore


def load_config(config_file: str, setting: str):
    """Compatibility wrapper for the old controller modules."""
    return JsonConfigStore(config_file).load_group(setting)


def save_config(config_file: str, group: str, **kwargs):
    """Compatibility wrapper for the old controller modules."""
    try:
        JsonConfigStore(config_file).save_group_values(group, **kwargs)
    except Exception as exc:
        print("Error saving json:", exc)
        messagebox.showerror("Fehler", "Deine Einstellungen konnten nicht gespeichert werden.")
