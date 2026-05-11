import tkinter as tk
from pathlib import Path


LOAD_PATHS = {
    "load_device": ("MACH", "resource", "library", "device"),
    "load_tool": ("MACH", "resource", "library", "tool"),
    "load_feed": ("MACH", "resource", "library", "feeds_speeds"),
    "load_pp": ("MACH", "resource", "postprocessor"),
    "load_installed_machines": (
        "MACH",
        "resource",
        "library",
        "machine",
        "installed_machines",
    ),
}


class UiChanges:
    """
    Handles UI events and synchronizes changed UI values through StateService.
    """

    def __init__(self, controller):
        self.controller = controller

        self._bind_comboboxes()
        self._bind_language()
        self._bind_checkboxes()

        self.controller.view.register.bind(
            "<<NotebookTabChanged>>",
            self.on_tab_changed
        )

        self.set_checkboxes()

    def _bind_comboboxes(self):
        register = self.controller.view.register

        register.customer_combobox.bind(
            "<<ComboboxSelected>>",
            self.customer_selected
        )
        register.nxversion_combobox.bind(
            "<<ComboboxSelected>>",
            self.version_selected
        )
        register.machine_combobox.bind(
            "<<ComboboxSelected>>",
            self.machine_selected
        )

    def _bind_language(self):
        language_frame = self.controller.view.language_frame

        language_frame.german_radio.config(
            command=lambda: self._sync_var("language")
        )
        language_frame.english_radio.config(
            command=lambda: self._sync_var("language")
        )

    def _bind_checkboxes(self):
        bindings = {
            "load_pp": self.controller.view.option_frame.pp_check,
            "load_installed_machines": self.controller.view.option_frame.installed_machines_check,
            "load_tool": self.controller.view.option_frame.tool_check,
            "load_device": self.controller.view.option_frame.device_check,
            "load_feed": self.controller.view.option_frame.feed_check,
        }

        for key, widget in bindings.items():
            widget.config(command=lambda setting_key=key: self._sync_var(setting_key))

    def _sync_var(self, key):
        self.controller.view.set_message()
        self.controller.state.sync_from_view(key)
     
    def reload_environment(self):
        """
        Reloads customer/version/machine lists after settings changed.
        """

        self.controller.model.customers = self.controller.model.get_customers()
        self._select_first_or_clear(
            values=self.controller.model.customers,
            model_attr="customer",
            view_key="customer",
            combobox=self.controller.view.register.customer_combobox,
        )

        self.controller.model.native_versions = self.controller.model.get_native_versions()
        self._select_first_or_clear(
            values=self.controller.model.native_versions,
            model_attr="native_version",
            view_key="native_version",
            combobox=self.controller.view.native_frame.nxversion_native_combobox,
        )

        self.controller.model.versions = self.controller.model.get_versions()
        self._select_first_or_clear(
            values=self.controller.model.versions,
            model_attr="version",
            view_key="version",
            combobox=self.controller.view.register.nxversion_combobox,
        )

        self.controller.model.machines = self.controller.model.get_machines()
        self._select_first_or_clear(
            values=self.controller.model.machines,
            model_attr="machine",
            view_key="machine",
            combobox=self.controller.view.register.machine_combobox,
        )

        self.controller.model.postbuilder_versions = self.controller.model.get_postbuilder_versions()
        self._select_first_or_clear(
            values=self.controller.model.postbuilder_versions,
            model_attr="postbuilder_version",
            view_key="postbuilder_version",
            combobox=self.controller.view.native_frame.postbuilder_combobox,
        )

        self.set_checkboxes()

    def customer_selected(self, e=None):
        self.controller.view.set_message()

        self.controller.state.sync_from_view("customer")

        self.controller.model.versions = self.controller.model.get_versions()
        self._select_first_or_clear(
            values=self.controller.model.versions,
            model_attr="version",
            view_key="version",
            combobox=self.controller.view.register.nxversion_combobox,
        )

        self.controller.model.machines = self.controller.model.get_machines()
        self._select_first_or_clear(
            values=self.controller.model.machines,
            model_attr="machine",
            view_key="machine",
            combobox=self.controller.view.register.machine_combobox,
        )

        self.set_checkboxes()

    def version_selected(self, e=None):
        self.controller.view.set_message()

        self.controller.state.sync_from_view("version")

        self.controller.model.machines = self.controller.model.get_machines()
        self._select_first_or_clear(
            values=self.controller.model.machines,
            model_attr="machine",
            view_key="machine",
            combobox=self.controller.view.register.machine_combobox,
        )

        self.set_checkboxes()

    def machine_selected(self, e=None):
        self.controller.view.set_message()
        self.controller.state.sync_from_view("machine")

    def _select_first_or_clear(self, values, model_attr, view_key, combobox):
        if values:
            value = values[0]
            setattr(self.controller.model, model_attr, value)
            self.controller.state.set(view_key, value)
            combobox.config(values=values)
            combobox.current(0)
            return

        setattr(self.controller.model, model_attr, "")
        self.controller.state.set(view_key, "")
        combobox.config(values=[])

    def set_checkboxes(self):
        base_path = Path(
            self.controller.model.settings["customer_environment_path"],
            self.controller.model.customer,
            "5_Umgebung",
            self.controller.model.version,
        )

        for key, relative_path_parts in LOAD_PATHS.items():
            exists = base_path.joinpath(*relative_path_parts).exists()
            self.controller.state.set(key, exists)

    def on_tab_changed(self, e=None):
        self.controller.view.set_message()

        selected_tab_index = self.controller.view.register.index(
            self.controller.view.register.select()
        )

        if selected_tab_index == 1:
            self._show_create_project_tab()
        else:
            self._show_start_nx_tab()

    def _show_create_project_tab(self):
        self.controller.view.buttons_frame.pack(
            side=tk.TOP,
            fill="both",
            expand=False
        )

        self.controller.view.pp_dir_btn.pack_forget()
        self.controller.view.vsCode_btn.pack_forget()
        self.controller.view.fork_btn.pack_forget()
        self.controller.view.option_frame.pack_forget()
        self.controller.view.language_frame.pack_forget()

        self.controller.view.register.new_machine_controller_entry.grid(
            row=4,
            column=1,
            padx=5,
            pady=5
        )
        self.controller.view.register.new_machine_type_entry.grid(
            row=5,
            column=1,
            padx=5,
            pady=5
        )
        self.controller.view.register.new_machine_type_lbl.grid(
            row=5,
            column=0,
            sticky="w"
        )
        self.controller.view.register.new_machine_controller_lbl.grid(
            row=4,
            column=0,
            sticky="w"
        )

        self.controller.view.start_btn.config(
            text="Projekt Anlegen",
            command=self.controller.create_new_customer.create_new_customer
        )

    def _show_start_nx_tab(self):
        self.controller.view.register.new_machine_controller_entry.grid_forget()
        self.controller.view.register.new_machine_type_entry.grid_forget()
        self.controller.view.register.new_machine_controller_lbl.grid_forget()
        self.controller.view.register.new_machine_type_lbl.grid_forget()

        self.controller.view.buttons_frame.pack(
            side=tk.BOTTOM,
            fill="both",
            expand=False
        )

        self.controller.view.pp_dir_btn.pack(side=tk.LEFT, padx=5)
        self.controller.view.vsCode_btn.pack(side=tk.LEFT, padx=5)
        self.controller.view.fork_btn.pack(side=tk.LEFT, padx=5)

        self.controller.view.start_btn.config(
            text="NX Starten",
            command=self.controller.start_nx.start_NX_customer
        )

        self.controller.view.option_frame.pack(
            fill="both",
            expand=False,
            pady=10
        )
        self.controller.view.language_frame.pack(
            fill="both",
            expand=False,
            pady=5
        )