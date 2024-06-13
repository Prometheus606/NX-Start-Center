import tkinter as tk
import ttkbootstrap as ttk
from PIL import Image, ImageTk

class SettingFrame(ttk.LabelFrame):
    def __init__(self, master, model, theme, nx_installation_path, customer_environment_path, licence_path, licence_server_path, icon, editor, batchstart, *args, **kwargs):
        super().__init__(master, *args, **kwargs)

        original_icon = Image.open(icon)
        scaled_icon = original_icon.resize((20, 20), Image.LANCZOS)  # Scale to 200x200 pixels
        self.icon = ImageTk.PhotoImage(scaled_icon)

        ttk.Label(self, text="NX installations pfad:").grid(row=0, column=0, padx=20, pady=15)
        self.nx_installation_path_entry = ttk.Entry(self, textvariable=nx_installation_path, width=50)
        self.nx_installation_path_entry.grid(row=0, column=1)
        self.browse_nx_installation_path_btn = tk.Button(self, image=self.icon)
        self.browse_nx_installation_path_btn.grid(row=0, column=2, sticky="w", padx=10)

        ttk.Label(self, text="Pfad zu den Kundenumgebungen:").grid(row=1, column=0, padx=20, pady=15)
        self.customer_environment_path_entry = ttk.Entry(self, textvariable=customer_environment_path, width=50)
        self.customer_environment_path_entry.grid(row=1, column=1)
        self.browse_customer_environment_path_btn = tk.Button(self, image=self.icon)
        self.browse_customer_environment_path_btn.grid(row=1, column=2, sticky="w", padx=10)

        ttk.Label(self, text="Lizenzpfad:").grid(row=2, column=0, padx=20, pady=15)
        self.licence_path_entry = ttk.Entry(self, textvariable=licence_path, width=50)
        self.licence_path_entry.grid(row=2, column=1)
        self.browse_licence_path_btn = tk.Button(self, image=self.icon)
        self.browse_licence_path_btn.grid(row=2, column=2, sticky="w", padx=10)

        ttk.Label(self, text="Lizenz Server Pfad:").grid(row=3, column=0, padx=20, pady=15)
        self.licence_server_path_entry = ttk.Entry(self, textvariable=licence_server_path, width=50)
        self.licence_server_path_entry.grid(row=3, column=1)
        self.browse_licence_server_path_btn = tk.Button(self, image=self.icon)
        self.browse_licence_server_path_btn.grid(row=3, column=2, sticky="w", padx=10)

        ttk.Label(self, text="Farbschema anpassen:").grid(row=4, column=0, padx=20, pady=15)
        self.theme_menu = ttk.OptionMenu(self, theme, model.theme, *model.light_themes, *model.dark_themes)
        self.theme_menu.configure(width=20)
        self.theme_menu.grid(row=4, column=1, sticky="w")

        ttk.Label(self, text="Text Editor:").grid(row=5, column=0, padx=20, pady=15)
        self.editor_menu = ttk.OptionMenu(self, editor, model.editor, *model.editors)
        self.editor_menu.configure(width=20)
        self.editor_menu.grid(row=5, column=1, sticky="w")

        self.batchstart_check = ttk.Checkbutton(self, text="Mit Batch Datei starten (nicht empfohlen!)", variable=batchstart, bootstyle="round-toggle")
        self.batchstart_check.grid(row=6, column=1, sticky="w", pady=15)

        self.return_btn = tk.Button(self, text="Zurück", width=15)
        self.return_btn.grid(row=7, column=2, pady=40)
