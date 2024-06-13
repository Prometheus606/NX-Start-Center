class SetLastConfig:
    """
    This class does two things:
        - checks if last configurations from the config file exists, and sets them to the UI
        - checks if the lists for customers, versions and machines are empty and shows an error, if so
    """
    def __init__(self, controller):
        self.controller = controller
        
        try:
            if self.controller.model.last_configuration and self.controller.model.last_configuration.get("last_load_pp") is not None:
                self.controller.view.load_pp.set(bool(int(self.controller.model.last_configuration["last_load_pp"])))
                self.controller.model.load_pp = self.controller.view.load_pp.get()
            else:
                self.controller.view.load_pp.set(False)
                self.controller.model.load_pp = self.controller.view.load_pp.get()

            if self.controller.model.last_configuration and self.controller.model.last_configuration.get("last_load_installed_machines") is not None:
                self.controller.view.load_installed_machines.set(bool(int(self.controller.model.last_configuration["last_load_installed_machines"])))
                self.controller.model.load_installed_machines = self.controller.view.load_installed_machines.get()
            else:
                self.controller.view.load_installed_machines.set(True)
                self.controller.model.load_installed_machines = self.controller.view.load_installed_machines.get()

            if self.controller.model.last_configuration and self.controller.model.last_configuration.get("last_load_device") is not None:
                self.controller.view.load_device.set(bool(int(self.controller.model.last_configuration["last_load_device"])))
                self.controller.model.load_device = self.controller.view.load_device.get()
            else:
                self.controller.view.load_device.set(False)
                self.controller.model.load_device = self.controller.view.load_device.get()

            if self.controller.model.last_configuration and self.controller.model.last_configuration.get("last_load_tool") is not None:
                self.controller.view.load_tool.set(bool(int(self.controller.model.last_configuration["last_load_tool"])))
                self.controller.model.load_tool = self.controller.view.load_tool.get()
            else:
                self.controller.view.load_tool.set(False)
                self.controller.model.load_tool = self.controller.view.load_tool.get()

            if self.controller.model.last_configuration and self.controller.model.last_configuration.get("last_load_feed") is not None:
                self.controller.view.load_feed.set(bool(int(self.controller.model.last_configuration["last_load_feed"])))
                self.controller.model.load_feed = self.controller.view.load_feed.get()
            else:
                self.controller.view.load_feed.set(False)
                self.controller.model.load_feed = self.controller.view.load_feed.get()

            if self.controller.model.last_configuration and self.controller.model.last_configuration.get("last_language") is not None:
                self.controller.view.language.set(self.controller.model.last_configuration["last_language"])
                self.controller.model.language = self.controller.view.language.get()
            else:
                self.controller.view.language.set("german")
                self.controller.model.language = self.controller.view.language.get()

            if self.controller.model.settings and self.controller.model.settings.get("batchstart") is not None:
                self.controller.view.batchstart.set(self.controller.model.settings["batchstart"])
                self.controller.model.batchstart = self.controller.view.batchstart.get()
            else:
                self.controller.view.batchstart.set(False)
                self.controller.model.batchstart = self.controller.view.batchstart.get()

            if self.controller.model.settings and self.controller.model.settings.get("editor") is not None:
                self.controller.view.editor.set(self.controller.model.settings["editor"])
                self.controller.model.editor = self.controller.view.editor.get()
            else:
                self.controller.view.editor.set("vscode")
                self.controller.model.editor = self.controller.view.editor.get()
        except KeyError:
            pass

        if not len(self.controller.model.machines):
            self.controller.view.messageLabel.config(text="Keine Maschine gefunden,\nüberprüfe deine Ordnerstruktur.")

        if not len(self.controller.model.versions):
            self.controller.view.messageLabel.config(text="Keine Version gefunden\nüberprüfe deine Ordnerstruktur.")

        if not len(self.controller.model.customers):
            self.controller.view.messageLabel.config(text="Keine Kunden gefunden\nüberprüfe deine Einstellungen")