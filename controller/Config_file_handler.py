import json
from tkinter import messagebox

def load_config(config_file: str, setting: str):
    """
    This function reads the setting groups from the config File. (not a specific setting)
    If the file, or the setting group not exists, a new one with standard settings will be created
    """
    while True:
        try:
            with open(config_file, 'r') as f:
                config = json.load(f)

            # Wenn die Einstellung nicht vorhanden ist, setze sie auf eine Standardkonfiguration
            if setting not in config:
                if setting == "settings":
                    config[setting] = {
                        "nx_installation_path": "D:\\Siemens\\NX_Versionen",
                        "customer_environment_path": "D:\\Siemens\\Kundenumgebung",
                        "licence_server_path": "D:\\Siemens\\License Server\\lmtools.exe",
                        "licence_path": "D:\\Siemens\\License\\License_ugslmd.lic"
                    }
                elif setting == "last_configuration":
                    config[setting] = {}

                # Aktualisiere die Konfigurationsdatei mit der neuen Einstellung
                with open(config_file, 'w') as f:
                    json.dump(config, f, indent=4)

            return config.get(setting)  # Gib die Einstellung zurück oder ein leeres Dictionary, falls nicht vorhanden

        except FileNotFoundError:
            # Wenn die Datei nicht gefunden wird, erstelle eine neue Konfigurationsdatei
            with open(config_file, 'w') as f:
                json.dump({}, f, indent=4)
            print(f"Die Datei '{config_file}' wurde nicht gefunden. Sie wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")
            messagebox.showinfo("Fehler", f"Die Datei '{config_file}' wurde nicht gefunden. Sie wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")

        except json.decoder.JSONDecodeError:
            # Wenn die Datei nicht gefunden wird, erstelle eine neue Konfigurationsdatei
            with open(config_file, 'w') as f:
                json.dump({}, f, indent=4)
            print(f"Die Datei '{config_file}' ist falsch formatiert. Sie wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")
            messagebox.showinfo("Fehler", f"Die Datei '{config_file}' ist falsch formatiert. Sie wurde neu erzeugt. Bitte überprüfe deine Einstellungen.")

def save_config(config_file: str, group:str, **kwargs):
    """
    This function saves all given key-value pairs into the given setting group, into the config file.
    """
    try:
        with open(config_file, 'r') as f:
            data = json.load(f)

        for key, value in kwargs.items():
            data[group][key] = value

        with open(config_file, 'w') as f:
            json.dump(data, f, indent=4)
    except Exception as e:
        print("Error saving json:", e)
        messagebox.showerror("Fehler", f"Deine Einstellungen konnten nicht gespeichert werden.")