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

        self._create_scroll_area()
        self._create_settings()
        self._create_return_button()
        self._create_legacy_aliases()

    def _create_scroll_area(self):
        self.canvas = tk.Canvas(self, highlightthickness=0)
        self.scrollbar = ttk.Scrollbar(
            self,
            orient=tk.VERTICAL,
            command=self.canvas.yview
        )

        self.content_frame = ttk.Frame(self.canvas)

        self.content_window = self.canvas.create_window(
            (0, 0),
            window=self.content_frame,
            anchor="nw"
        )

        self.canvas.configure(yscrollcommand=self.scrollbar.set)

        self.canvas.grid(row=0, column=0, sticky="nsew")
        self.scrollbar.grid(row=0, column=1, sticky="ns")

        self.grid_rowconfigure(0, weight=1)
        self.grid_columnconfigure(0, weight=1)

        self.content_frame.bind(
            "<Configure>",
            self._update_scroll_region
        )
        self.canvas.bind(
            "<Configure>",
            self._update_content_width
        )

        self.canvas.bind_all("<MouseWheel>", self._on_mousewheel)

    def _update_scroll_region(self, event=None):
        self.canvas.configure(scrollregion=self.canvas.bbox("all"))

    def _update_content_width(self, event):
        self.canvas.itemconfigure(
            self.content_window,
            width=event.width
        )

    def _on_mousewheel(self, event):
        self.canvas.yview_scroll(
            int(-1 * (event.delta / 120)),
            "units"
        )

    def _create_settings(self):
        for row, setting in enumerate(SETTINGS):
            variable = self.variables[setting.key]

            if setting.widget == "checkbox":
                widget = ttk.Checkbutton(
                    self.content_frame,
                    text=setting.label,
                    variable=variable,
                    bootstyle="round-toggle"
                )
                widget.grid(
                    row=row,
                    column=1,
                    sticky="w",
                    pady=setting.row_padding
                )
                self.inputs[setting.key] = widget
                continue

            ttk.Label(
                self.content_frame,
                text=setting.label
            ).grid(
                row=row,
                column=0,
                padx=20,
                pady=setting.row_padding,
                sticky="w"
            )

            if setting.widget == "option":
                if setting.options:
                    values = list(setting.options)
                else:
                    values = list(getattr(self.model, setting.options_attr or "", []))
                current = variable.get() or setting.default

                widget = ttk.OptionMenu(
                    self.content_frame,
                    variable,
                    current,
                    *values
                )
                widget.configure(width=20)
                widget.grid(row=row, column=1, sticky="w")
            else:
                widget = ttk.Entry(
                    self.content_frame,
                    textvariable=variable,
                    width=50
                )
                widget.grid(row=row, column=1, sticky="ew")

            self.inputs[setting.key] = widget

            if setting.dialog:
                button = tk.Button(
                    self.content_frame,
                    image=self.icon
                )
                button.grid(row=row, column=2, sticky="w", padx=10)
                self.browse_buttons[setting.key] = button
                setattr(self, f"browse_{setting.key}_btn", button)

        self.content_frame.grid_columnconfigure(1, weight=1)

    def _create_return_button(self):
        self.return_btn = tk.Button(
            self,
            text="Zurück",
            width=15
        )
        self.return_btn.grid(row=1, column=1, columnspan=1, padx=50, pady=10)

    def _create_legacy_aliases(self):
        for setting in SETTINGS:
            widget = self.inputs.get(setting.key)

            if widget is None:
                continue

            setattr(self, f"{setting.key}_entry", widget)

            if setting.widget_alias:
                setattr(self, setting.widget_alias, widget)