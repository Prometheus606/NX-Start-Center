from dataclasses import dataclass, field
from typing import Any, Iterable, Optional, Tuple


@dataclass(frozen=True)
class SettingDefinition:
    key: str
    label: str
    default: Any = ""

    widget: str = "entry"

    dialog: Optional[str] = None
    filetypes: Tuple[Tuple[str, str], ...] = ()

    options_attr: Optional[str] = None
    options: Tuple[str, ...] = ()

    save_on_change: bool = False
    row_padding: int = 15

    variable_aliases: Tuple[str, ...] = field(default_factory=tuple)
    widget_alias: Optional[str] = None


SETTINGS: tuple[SettingDefinition, ...] = (
    SettingDefinition(
        key="nx_installation_path",
        label="NX Installationspfad:",
        default=r"D:\Siemens\NX_Versionen",
        dialog="directory",
    ),
    SettingDefinition(
        key="customer_environment_path",
        label="Pfad zu den Kundenumgebungen:",
        default=r"D:\Siemens\Kundenumgebung",
        dialog="directory",
    ),
    SettingDefinition(
        key="licence_path",
        label="Lizenzpfad:",
        default=r"D:\Siemens\License\License_ugslmd.lic",
        dialog="file",
        filetypes=(("LIC Dateien", "*.lic"),),
    ),
    SettingDefinition(
        key="licence_server_path",
        label="Lizenz Server Pfad:",
        default=r"D:\Siemens\License Server\lmtools.exe",
        dialog="file",
        filetypes=(("EXE Dateien", "*.exe"),),
    ),
    SettingDefinition(
        key="fork_path",
        label="Fork Pfad:",
        default=r"C:\Users\niklas.beitler\AppData\Local\Fork\current\Fork.exe",
        dialog="file",
        filetypes=(("EXE Dateien", "*.exe"),),
    ),
    SettingDefinition(
        key="roles_path",
        label="Pfade zu den Rollen:",
        default="",
        dialog="files",
        filetypes=(("MTX Dateien", "*.mtx"),),
    ),
    SettingDefinition(
        key="team",
        label="Team:",
        default="PP",
        widget="option",
        options=("CAM", "PP"),
        save_on_change=True,
        widget_alias="team_menu",
    ),
    SettingDefinition(
        key="preferred_theme",
        label="Farbschema anpassen:",
        default="darkly",
        widget="option",
        options_attr="themes",
        save_on_change=True,
        variable_aliases=("theme",),
        widget_alias="theme_menu",
    ),
    SettingDefinition(
        key="editor",
        label="Text Editor:",
        default="Notepad",
        widget="option",
        options_attr="editors",
        save_on_change=True,
        widget_alias="editor_menu",
    ),
    SettingDefinition(
        key="startNXWithDebug",
        label="Start Datei Debuggen",
        default=False,
        widget="checkbox",
        save_on_change=True,
        row_padding=15,
        widget_alias="debugStart_check",
    ),
)

SETTING_BY_KEY = {setting.key: setting for setting in SETTINGS}
SETTING_KEYS = tuple(SETTING_BY_KEY.keys())


def setting_defaults() -> dict[str, Any]:
    return {setting.key: setting.default for setting in SETTINGS}


def setting_keys() -> Iterable[str]:
    return SETTING_KEYS


@dataclass(frozen=True)
class LastConfigDefinition:
    """
    Single source of truth for one last-session configuration value.

    Used for values that are not normal settings, but should restore
    the previous UI/session state.
    """

    key: str
    config_key: str
    default: Any = ""


LAST_CONFIGS: tuple[LastConfigDefinition, ...] = (
    LastConfigDefinition(
        key="load_pp",
        config_key="last_load_pp",
        default=False,
    ),
    LastConfigDefinition(
        key="load_installed_machines",
        config_key="last_load_installed_machines",
        default=True,
    ),
    LastConfigDefinition(
        key="load_device",
        config_key="last_load_device",
        default=False,
    ),
    LastConfigDefinition(
        key="load_tool",
        config_key="last_load_tool",
        default=False,
    ),
    LastConfigDefinition(
        key="load_feed",
        config_key="last_load_feed",
        default=False,
    ),
    LastConfigDefinition(
        key="language",
        config_key="last_language",
        default="german",
    ),
)

LAST_CONFIG_BY_KEY = {config.key: config for config in LAST_CONFIGS}
LAST_CONFIG_KEYS = tuple(LAST_CONFIG_BY_KEY.keys())


def last_config_defaults() -> dict[str, Any]:
    return {
        config.config_key: config.default
        for config in LAST_CONFIGS
    }


def last_config_keys() -> Iterable[str]:
    return LAST_CONFIG_KEYS
