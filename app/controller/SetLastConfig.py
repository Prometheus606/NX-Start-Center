class SetLastConfig:
    """
    Initializes UI variables and model values from persisted config.
    """

    EMPTY_LIST_MESSAGES = (
        ("machines", "Keine Maschine gefunden,\nüberprüfe deine Ordnerstruktur."),
        ("versions", "Keine Version gefunden\nüberprüfe deine Ordnerstruktur."),
        ("customers", "Keine Kunden gefunden\nüberprüfe deine Einstellungen"),
        ("native_versions", "Keine NX Versionen gefunden\nüberprüfe deine Einstellungen"),
        ("postbuilder_versions", "Keine Postbuilder versionen gefunden\nüberprüfe deine Einstellungen"),
    )

    def __init__(self, controller):
        self.controller = controller

        self.controller.state.apply_last_config_from_config()
        self.controller.state.apply_settings_from_config()

        self.show_empty_list_messages()

    def show_empty_list_messages(self):
        for attribute, message in self.EMPTY_LIST_MESSAGES:
            if not getattr(self.controller.model, attribute, []):
                self.controller.view.set_message(message)