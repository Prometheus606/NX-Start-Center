from typing import Any

from controller.Config_file_handler import save_config
from core.settings_schema import SETTINGS, LAST_CONFIGS


class StateService:
    """
    Central access layer between controller, model and Tk variables.

    Controllers should use this service instead of directly accessing:
        self.controller.view.<variable>.get()
        self.controller.view.<variable>.set(...)
        self.controller.model.<attribute> = ...

    This keeps view/model synchronization in one place.
    """

    def __init__(self, model, view):
        self.model = model
        self.view = view

    def get(self, key: str) -> Any:
        variable = self._get_view_variable(key)

        if variable is not None:
            return variable.get()

        return getattr(self.model, key)

    def set(self, key: str, value: Any) -> None:
        variable = self._get_view_variable(key)

        if variable is not None:
            variable.set(value)

        setattr(self.model, key, value)

    def sync_from_view(self, key: str) -> Any:
        value = self.get(key)
        setattr(self.model, key, value)
        return value

    def sync_to_view(self, key: str) -> None:
        value = getattr(self.model, key)
        variable = self._get_view_variable(key)

        if variable is not None:
            variable.set(value)

    def apply_settings_from_config(self) -> None:
        stored_values = self.model.settings or {}

        for setting in SETTINGS:
            value = stored_values.get(setting.key, setting.default)
            self.set(setting.key, value)

            for alias in setting.variable_aliases:
                self.set(alias, value)

    def apply_last_config_from_config(self) -> None:
        stored_values = self.model.last_configuration or {}

        for config in LAST_CONFIGS:
            value = stored_values.get(config.config_key, config.default)

            if isinstance(config.default, bool):
                value = bool(int(value))

        self.set(config.key, value)

    def save_setting(self, key: str, value: Any | None = None) -> None:
        if value is None:
            value = self.get(key)

        self.set(key, value)
        save_config(self.model.config_file, "settings", **{key: value})

    def save_all_settings(self) -> dict[str, Any]:
        values = {}

        for setting in SETTINGS:
            value = self.get(setting.key)
            self.set(setting.key, value)
            values[setting.key] = value

        save_config(self.model.config_file, "settings", **values)
        return values

    def _get_view_variable(self, key: str):
        if hasattr(self.view, "setting_vars") and key in self.view.setting_vars:
            return self.view.setting_vars[key]

        variable = getattr(self.view, key, None)

        if variable is not None and hasattr(variable, "get") and hasattr(variable, "set"):
            return variable

        return None