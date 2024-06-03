import ttkbootstrap as ttk

class NativStartFrame(ttk.LabelFrame):
    def __init__(self, master, model, native_version, *args, **kwargs):
        super().__init__(master, *args, **kwargs)

        ttk.Label(self, text="NX-Version:").grid(row=0, column=0, sticky="w")
        self.nxversion_native_combobox = ttk.Combobox(self, values=model.native_versions, textvariable=native_version)
        if len(model.native_versions):
            self.nxversion_native_combobox.current(model.native_versions.index(model.native_version))
        self.nxversion_native_combobox.grid(row=0, column=1, padx=5, pady=5)

        self.start_nativ_btn = ttk.Button(self, text="NX Starten")
        self.start_nativ_btn.grid(row=1, column=1, padx=5, pady=10)