import tkinter as tk
import ttkbootstrap as ttk
from PIL import Image, ImageTk

class ProjectFrame(ttk.Notebook):
    def __init__(self, master, model, customer, version, machine, new_customer, new_version, new_machine, new_order, *args, **kwargs):
        super().__init__(master, *args, **kwargs)

        self.project_frame = ttk.Frame(self)
        self.project_frame.pack(fill="both", expand=False)

        ttk.Label(self.project_frame, text="Kunde:").grid(row=0, column=0, sticky="w")
        self.customer_combobox = ttk.Combobox(self.project_frame, values=model.customers, textvariable=customer,width=40)
        self.customer_combobox.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(self.project_frame, text="NX-Version:").grid(row=1, column=0, sticky="w")
        self.nxversion_combobox = ttk.Combobox(self.project_frame, values=model.versions, textvariable=version,width=40)
        self.nxversion_combobox.grid(row=1, column=1, padx=5, pady=5)

        ttk.Label(self.project_frame, text="Maschine:").grid(row=2, column=0, sticky="w")
        self.machine_combobox = ttk.Combobox(self.project_frame, values=model.machines, textvariable=machine,width=40)
        self.machine_combobox.grid(row=2, column=1, padx=5, pady=5)

        # new project register

        self.new_project_frame = ttk.Frame(self)
        self.new_project_frame.pack(fill="both", expand=False)

        ttk.Label(self.new_project_frame, text="Kunde:").grid(row=0, column=0, sticky="w")
        self.customer_entry = ttk.Entry(self.new_project_frame, textvariable=new_customer, width=40)
        self.customer_entry.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(self.new_project_frame, text="NX-Version:").grid(row=1, column=0, sticky="w")
        self.nxversion_entry = ttk.Combobox(self.new_project_frame, values=model.native_versions,textvariable=new_version, width=38)
        self.nxversion_entry.grid(row=1, column=1, padx=5, pady=5)

        ttk.Label(self.new_project_frame, text="Maschine:").grid(row=2, column=0, sticky="w")
        self.maschine_entry = ttk.Entry(self.new_project_frame, textvariable=new_machine, width=40)
        self.maschine_entry.grid(row=2, column=1, padx=5, pady=5)

        ttk.Label(self.new_project_frame, text="Auftragsnummer:").grid(row=3, column=0, sticky="w")
        self.order_entry = ttk.Entry(self.new_project_frame, textvariable=new_order, width=40)
        self.order_entry.grid(row=3, column=1, padx=5, pady=5)

        self.add(self.project_frame, text='Projekt weiterarbeiten')
        self.add(self.new_project_frame, text='Neues Projekt anlegen')