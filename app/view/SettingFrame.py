import tkinter as tk
import ttkbootstrap as ttk
from PIL import Image, ImageTk

from core.settings_schema import SETTINGS


class SettingFrame(ttk.LabelFrame):
    def __init__(self, master, model, variables, icon, *args, **kwargs):
        super().__init__(master, *args, **kwargs)
        self.model = model
        self.variables = variables
        self.inputs = {}
        self.browse_buttons = {}

        original_icon = Image.open(icon)
        scaled_icon = original_icon.resize((20, 20), Image.LANCZOS)
        self.icon = ImageTk.PhotoImage(scaled_icon)

        for row, setting in enumerate(SETTINGS):
            variable = variables[setting.key]

            if setting.widget == "checkbox":
                widget = ttk.Checkbutton(self, text=setting.label, variable=variable, bootstyle="round-toggle")
                widget.grid(row=row, column=1, sticky="w", pady=setting.row_padding)
                self.inputs[setting.key] = widget
                continue

            ttk.Label(self, text=setting.label).grid(row=row, column=0, padx=20, pady=setting.row_padding)

            if setting.widget == "option":
                values = list(getattr(model, setting.options_attr or "", []))
                current = variable.get() or setting.default
                widget = ttk.OptionMenu(self, variable, current, *values)
                widget.configure(width=20)
                widget.grid(row=row, column=1, sticky="w")
            else:
                widget = ttk.Entry(self, textvariable=variable, width=50)
                widget.grid(row=row, column=1)

            self.inputs[setting.key] = widget

            if setting.dialog:
                button = tk.Button(self, image=self.icon)
                button.grid(row=row, column=2, sticky="w", padx=10)
                self.browse_buttons[setting.key] = button
                setattr(self, f"browse_{setting.key}_btn", button)

        self.return_btn = tk.Button(self, text="Zurück", width=15)
        self.return_btn.grid(row=len(SETTINGS), column=2, pady=10)

        # Backwards-compatible widget attributes for older controller code/tests.
        # The aliases live in settings_schema.py, not scattered through the view.
        for setting in SETTINGS:
            widget = self.inputs.get(setting.key)
            if widget is None:
                continue
            setattr(self, f"{setting.key}_entry", widget)
            if setting.widget_alias:
                setattr(self, setting.widget_alias, widget)
