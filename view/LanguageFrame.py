import ttkbootstrap as ttk

class LanguageFrame(ttk.LabelFrame):
    def __init__(self, master, language, *args, **kwargs):
        super().__init__(master, *args, **kwargs)

        self.german_radio = ttk.Radiobutton(self, text="Deutsch", variable=language, value="german")
        self.german_radio.grid(row=0, column=0, sticky="w", pady=5)
        self.english_radio = ttk.Radiobutton(self, text="Englisch", variable=language,
                                             value="english")
        self.english_radio.grid(row=1, column=0, sticky="w", pady=5)