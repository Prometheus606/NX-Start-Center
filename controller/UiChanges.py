import ttkbootstrap as ttk

class UiChanges:
    """
    This Class defines what happens if something in the UI changes, like a Combobox, checkbox, usw...
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.register.customer_combobox.bind("<<ComboboxSelected>>", self.customer_selected)
        self.controller.view.register.nxversion_combobox.bind("<<ComboboxSelected>>", self.version_selected)
        self.controller.view.register.machine_combobox.bind("<<ComboboxSelected>>", self.machine_selected)
        self.controller.view.language_frame.german_radio.config(command=self.language_selected)
        self.controller.view.language_frame.english_radio.config(command=self.language_selected)
        self.controller.view.option_frame.pp_check.config(command=self.pp_check_selected)
        self.controller.view.option_frame.cse_check.config(command=self.cse_check_selected)
        self.controller.view.option_frame.tool_check.config(command=self.tool_check_selected)
        self.controller.view.option_frame.device_check.config(command=self.device_check_selected)
        self.controller.view.option_frame.feed_check.config(command=self.feed_check_selected)
        self.controller.view.register.bind("<Button-1>", self.on_tab_changed)
        
    def customer_selected(self, e):
        """
        Defines what happens if the customer Combobox was modified
        :param e: event
        """
        self.controller.view.messageLabel.config(text="")

        self.controller.model.customer = self.controller.view.customer.get()

        self.controller.model.versions = self.controller.model.get_versions()
        self.controller.model.version = self.controller.model.versions[0]
        self.controller.view.version.set(self.controller.model.version)
        self.controller.view.register.nxversion_combobox.config(values=self.controller.model.versions)
        self.controller.view.register.nxversion_combobox.current(self.controller.model.versions.index(self.controller.model.version))

        self.controller.model.machines = self.controller.model.get_machines()
        if len(self.controller.model.machines) > 0:
            self.controller.model.machine = self.controller.model.machines[0]
            self.controller.view.machine.set(self.controller.model.machine)
            self.controller.view.register.machine_combobox.config(values=self.controller.model.machines)
            self.controller.view.register.machine_combobox.current(self.controller.model.machines.index(self.controller.model.machine))
        else:
            self.controller.model.machine = ""
            self.controller.view.machine.set("")
            self.controller.view.register.machine_combobox.config(values=self.controller.model.machines)

    def version_selected(self, e):
        """
        Defines what happens if the version Combobox was modified
        :param e: event
        """
        self.controller.view.messageLabel.config(text="")

        self.controller.model.version = self.controller.view.version.get()

        self.controller.model.machines = self.controller.model.get_machines()
        if len(self.controller.model.machines) > 0:
            self.controller.model.machine = self.controller.model.machines[0]
            self.controller.view.machine.set(self.controller.model.machine)
            self.controller.view.register.machine_combobox.config(values=self.controller.model.machines)
            self.controller.view.register.machine_combobox.current(self.controller.model.machines.index(self.controller.model.machine))
        else:
            self.controller.model.machine = ""
            self.controller.view.machine.set("")
            self.controller.view.register.machine_combobox.config(values=self.controller.model.machines)

    def machine_selected(self, e):
        """
        Defines what happens if the Machine Combobox was modified
        :param e: event
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.machine = self.controller.view.machine.get()

    def language_selected(self):
        """
        Defines what happens if the language radiobox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.language = self.controller.view.language.get()

    def pp_check_selected(self):
        """
        Defines what happens if the PP Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.load_pp = self.controller.view.load_pp.get()

    def cse_check_selected(self):
        """
        Defines what happens if the cse Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.load_cse = self.controller.view.load_cse.get()

    def tool_check_selected(self):
        """
        Defines what happens if the tool Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.load_tool = self.controller.view.load_tool.get()

    def device_check_selected(self):
        """
        Defines what happens if the device Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.load_device = self.controller.view.load_device.get()

    def feed_check_selected(self):
        """
        Defines what happens if the feed Checkbox was modified
        """
        self.controller.view.messageLabel.config(text="")
        self.controller.model.load_feed = self.controller.view.load_feed.get()


    def on_tab_changed(self, e):
        """
        Defines what happens if the tab in the notebook was modified
        if the x position >= 131, then right tab is selected.
        THE VALUE MIGHT BE WRONG IF THE WINDOW SIZE CHANGES!!
        """
        if e.x >= 131:
            self.controller.view.buttons_frame.pack(side=ttk.TOP, fill="both", expand=False)
            self.controller.view.pp_dir_btn.pack_forget()
            self.controller.view.vsCode_btn.pack_forget()
            self.controller.view.option_frame.pack_forget()
            self.controller.view.language_frame.pack_forget()
            self.controller.view.start_btn.config(text="Projekt Anlegen", command=self.controller.create_new_customer.create_new_customer)
        else:
            self.controller.view.buttons_frame.pack(side=ttk.BOTTOM, fill="both", expand=False)
            self.controller.view.pp_dir_btn.pack(side=ttk.LEFT, padx=5)
            self.controller.view.vsCode_btn.pack(side=ttk.LEFT, padx=5)
            self.controller.view.start_btn.config(text="NX Starten", command=self.controller.start_nx.start_NX_customer)
            self.controller.view.option_frame.pack(fill="both", expand=False, pady=10)
            self.controller.view.language_frame.pack(fill="both", expand=False, pady=5)