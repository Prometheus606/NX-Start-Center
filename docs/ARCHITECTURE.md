# Neue Code-Struktur

Die Struktur wurde so umgebaut, dass Settings nicht mehr an drei Stellen gepflegt werden müssen. Die zentrale Regel ist:

> Neue persistente Settings werden in `src/core/settings_schema.py` ergänzt. Daraus entstehen Defaults, Config-Migration, UI-Felder, Browse-Buttons und Save-on-change-Verhalten.

## Zielbild

```text
src/
├── NX_Startcenter.py              # Composition Root / Programmstart
├── core/                          # Infrastruktur-nahe Basiskomponenten
│   ├── config_store.py            # JSON lesen, schreiben, Defaults migrieren
│   └── settings_schema.py         # zentrale Definition aller Settings
├── model/
│   └── Model.py                   # zentraler App-State, kompatibel zu alten Controllern
├── view/
│   ├── View.py                    # Root Window und Tk-Variablen
│   └── SettingFrame.py            # generischer, schema-getriebener Settings-Dialog
└── controller/
    ├── Settings.py                # generischer Settings-Controller
    └── ...                        # bestehende Feature-Controller
```

Die bestehenden Controller wurden bewusst kompatibel gehalten. Zugriffe wie `model.nx_installation_path`, `model.settings[...]` oder `view.nx_installation_path` funktionieren weiter. Intern werden diese Werte aber nicht mehr verstreut initialisiert, sondern aus dem Schema abgeleitet.

## Zentrale Settings-Definition

Alle Settings liegen hier:

```python
# src/core/settings_schema.py
SettingDefinition(
    key="nx_installation_path",
    label="NX Installationspfad:",
    default=r"D:\Siemens\NX_Versionen",
    dialog="directory",
)
```

Ein `SettingDefinition` beschreibt:

- `key`: Name in der JSON-Datei und im Model
- `label`: Text im UI
- `default`: Standardwert bei neuer oder alter Config-Datei
- `widget`: `entry`, `option` oder `checkbox`
- `dialog`: optional `directory`, `file` oder `files`
- `filetypes`: Dateifilter für File-Dialoge
- `options_attr`: Model-Attribut für Dropdown-Optionen, z. B. `themes`
- `save_on_change`: direkt speichern, sobald sich der Wert ändert

## Neues Setting hinzufügen

Beispiel: ein neuer Pfad zur Dokumentation.

```python
SettingDefinition(
    key="documentation_path",
    label="Dokumentationspfad:",
    default="",
    dialog="directory",
)
```

Damit passiert automatisch:

1. Der Default wird in neuen und bestehenden `config.json` Dateien ergänzt.
2. Das Model erlaubt `model.documentation_path`.
3. Die View erzeugt `view.documentation_path` als Tk-Variable.
4. Der Settings-Dialog zeigt Label, Entry und Browse-Button.
5. Der Settings-Controller speichert den Wert.

Kein zusätzlicher Code in Model, View und Controller ist nötig.

## Config-Handling

`src/core/config_store.py` kapselt das JSON-Handling. Der alte Importpfad bleibt über `controller/Config_file_handler.py` erhalten, damit bestehende Controller nicht alle auf einmal angepasst werden müssen.

## Model

`src/model/Model.py` ist jetzt ein zentraler State-Container mit Kompatibilitätsschicht:

- Persistente Settings kommen aus `settings_schema.py`.
- `__getattr__` und `__setattr__` leiten Setting-Zugriffe auf `model.settings` um.
- Fachliche Daten wie Kunde, Version, Maschine, Sprachen und Listen bleiben im Model.

## Settings-UI

`src/view/SettingFrame.py` baut die Felder in einer Schleife aus `SETTINGS`. Dadurch ist die Reihenfolge und Darstellung der Settings nicht mehr hart codiert.

## Settings-Controller

`src/controller/Settings.py` kennt keine einzelnen Settings mehr. Er bindet Browse-Buttons, Change-Handler und Speichern generisch anhand des Schemas.

## Warum diese Lösung?

Die Lösung ist absichtlich evolutionär:

- Die wichtigste Schwachstelle wurde beseitigt: neue Settings sind zentral definierbar.
- Bestehende Feature-Controller funktionieren weiter.
- Das Projekt kann schrittweise weiter in Services und Feature-Module zerlegt werden, ohne jetzt alles auf einmal zu riskieren.

