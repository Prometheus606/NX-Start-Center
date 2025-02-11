import ttkbootstrap as ttk
from pathlib import Path

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
        self.controller.view.option_frame.installed_machines_check.config(command=self.installed_machines_check_selected)
        self.controller.view.option_frame.tool_check.config(command=self.tool_check_selected)
        self.controller.view.option_frame.device_check.config(command=self.device_check_selected)
        self.controller.view.option_frame.feed_check.config(command=self.feed_check_selected)
        self.controller.view.register.bind("<Button-1>", self.on_tab_changed)
        
    def customer_selected(self, e):
        """
        Defines what happens if the customer Combobox was modified
        :param e: event
        """
        self.controller.view.set_message()

        self.controller.model.customer = self.controller.view.customer.get()

        self.controller.model.versions = self.controller.model.get_versions()
        if len(self.controller.model.versions) > 0:
            self.controller.model.version = self.controller.model.versions[0]
            self.controller.view.version.set(self.controller.model.version)
            self.controller.view.register.nxversion_combobox.config(values=self.controller.model.versions)
            self.controller.view.register.nxversion_combobox.current(self.controller.model.versions.index(self.controller.model.version))
        else:
            self.controller.model.version = ""
            self.controller.view.version.set("")
            self.controller.view.register.nxversion_combobox.config(values=self.controller.model.versions)

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

        self.set_checkboxes()

    def version_selected(self, e):
        """
        Defines what happens if the version Combobox was modified
        :param e: event
        """
        self.controller.view.set_message()

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

        self.set_checkboxes()

    def machine_selected(self, e):
        """
        Defines what happens if the Machine Combobox was modified
        :param e: event
        """
        self.controller.view.set_message()
        self.controller.model.machine = self.controller.view.machine.get()

    def set_checkboxes(self):
        if Path(f"{self.controller.model.settings['customer_environment_path']}\\{self.controller.model.customer}\\5_Umgebung\\{self.controller.model.version}\\MACH\\resource\\library\\device").exists():
            self.controller.model.load_device = True
        else:
            self.controller.model.load_device = False
        self.controller.view.load_device.set(self.controller.model.load_device)

        if Path(f"{self.controller.model.settings['customer_environment_path']}\\{self.controller.model.customer}\\5_Umgebung\\{self.controller.model.version}\\MACH\\resource\\library\\tool").exists():
            self.controller.model.load_tool = True
        else:
            self.controller.model.load_tool = False
        self.controller.view.load_tool.set(self.controller.model.load_tool)

        if Path(f"{self.controller.model.settings['customer_environment_path']}\\{self.controller.model.customer}\\5_Umgebung\\{self.controller.model.version}\\MACH\\resource\\library\\feeds_speeds").exists():
            self.controller.model.load_feed = True
        else:
            self.controller.model.load_feed = False
        self.controller.view.load_feed.set(self.controller.model.load_feed)

        if Path(f"{self.controller.model.settings['customer_environment_path']}\\{self.controller.model.customer}\\5_Umgebung\\{self.controller.model.version}\\MACH\\resource\\postprocessor").exists():
            self.controller.model.load_pp = True
        else:
            self.controller.model.load_pp = False
        self.controller.view.load_pp.set(self.controller.model.load_pp)

    def language_selected(self):
        """
        Defines what happens if the language radiobox was modified
        """
        self.controller.view.set_message()
        self.controller.model.language = self.controller.view.language.get()

    def pp_check_selected(self):
        """
        Defines what happens if the PP Checkbox was modified
        """
        self.controller.view.set_message()
        self.controller.model.load_pp = self.controller.view.load_pp.get()

    def installed_machines_check_selected(self):
        """
        Defines what happens if the cse Checkbox was modified
        """
        self.controller.view.set_message()
        self.controller.model.load_installed_machines = self.controller.view.load_installed_machines.get()

    def tool_check_selected(self):
        """
        Defines what happens if the tool Checkbox was modified
        """
        self.controller.view.set_message()
        self.controller.model.load_tool = self.controller.view.load_tool.get()

    def device_check_selected(self):
        """
        Defines what happens if the device Checkbox was modified
        """
        self.controller.view.set_message()
        self.controller.model.load_device = self.controller.view.load_device.get()

    def feed_check_selected(self):
        """
        Defines what happens if the feed Checkbox was modified
        """
        self.controller.view.set_message()
        self.controller.model.load_feed = self.controller.view.load_feed.get()


    def on_tab_changed(self, e):
        """
        Defines what happens if the tab in the notebook was modified
        if the x position >= 131, then right tab is selected.
        THE VALUE MIGHT BE WRONG IF THE WINDOW SIZE CHANGES!!
        """
        self.controller.view.set_message()

        if e.x >= 260:
            return

        if e.x >= 131:
            self.controller.view.buttons_frame.pack(side=ttk.TOP, fill="both", expand=False)
            self.controller.view.pp_dir_btn.pack_forget()
            self.controller.view.vsCode_btn.pack_forget()
            self.controller.view.option_frame.pack_forget()
            self.controller.view.language_frame.pack_forget()
            self.controller.view.register.new_machine_controller_entry.grid(row=4, column=1, padx=5, pady=5)
            self.controller.view.register.new_machine_type_entry.grid(row=5, column=1, padx=5, pady=5)
            self.controller.view.register.new_machine_type_lbl.grid(row=5, column=0, sticky="w")
            self.controller.view.register.new_machine_controller_lbl.grid(row=4, column=0, sticky="w")
            self.controller.view.start_btn.config(text="Projekt Anlegen", command=self.controller.create_new_customer.create_new_customer)
        else:
            self.controller.view.register.new_machine_controller_entry.grid_forget()
            self.controller.view.register.new_machine_type_entry.grid_forget()
            self.controller.view.register.new_machine_controller_lbl.grid_forget()
            self.controller.view.register.new_machine_type_lbl.grid_forget()
            self.controller.view.buttons_frame.pack(side=ttk.BOTTOM, fill="both", expand=False)
            self.controller.view.pp_dir_btn.pack(side=ttk.LEFT, padx=5)
            self.controller.view.vsCode_btn.pack(side=ttk.LEFT, padx=5)
            self.controller.view.start_btn.config(text="NX Starten", command=self.controller.start_nx.start_NX_customer)
            self.controller.view.option_frame.pack(fill="both", expand=False, pady=10)
            self.controller.view.language_frame.pack(fill="both", expand=False, pady=5)