import ttkbootstrap as ttk

class NativStartFrame(ttk.Notebook):
    def __init__(self, master, model, native_version, postbuilder_version, *args, **kwargs):
        super().__init__(master, *args, **kwargs)


        # Nativ Start Notebook
        self.nativ_start_frame = ttk.Frame(self)
        self.nativ_start_frame.pack(fill="both", expand=False)

        ttk.Label(self.nativ_start_frame, text="NX-Version:").grid(row=0, column=0, sticky="w")
        self.nxversion_native_combobox = ttk.Combobox(self.nativ_start_frame, values=model.native_versions, textvariable=native_version)
        if len(model.native_versions):
            self.nxversion_native_combobox.current(model.native_versions.index(model.native_version))
        self.nxversion_native_combobox.grid(row=0, column=1, padx=5, pady=15)

        self.start_nativ_btn = ttk.Button(self.nativ_start_frame, text="NX Starten")
        self.start_nativ_btn.grid(row=1, column=1, padx=5, pady=5)


        # Postbuilder Notebook

        self.post_builder_frame = ttk.Frame(self)
        self.post_builder_frame.pack(fill="both", expand=False)

        ttk.Label(self.post_builder_frame, text="PB-Version:").grid(row=0, column=0, sticky="w")
        self.postbuilder_combobox = ttk.Combobox(self.post_builder_frame, values=model.postbuilder_versions,
                                                      textvariable=postbuilder_version)
        if len(model.postbuilder_versions):
            self.postbuilder_combobox.current(model.postbuilder_versions.index(model.postbuilder_version))
        self.postbuilder_combobox.grid(row=0, column=1, padx=5, pady=15)

        self.start_postbuilder_btn = ttk.Button(self.post_builder_frame, text="Postbuilder Starten")
        self.start_postbuilder_btn.grid(row=1, column=1, padx=5, pady=5)

        self.add(self.nativ_start_frame, text='NX Native starten')
        self.add(self.post_builder_frame, text='Postbuilder starten')