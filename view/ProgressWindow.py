import tkinter as tk
import ttkbootstrap as ttk
import time

class ProgressWindow(tk.Tk):
    def __init__(self, model):
        super().__init__()
        self.model = model

        # ==================================== General settings ====================================
        self.win_width = 300
        self.win_height = 100
        self.duh_logo_ico = fr"src/images\duhGroup_Logo.ico"

        self.title("NX Startcenter")
        self.resizable(width=False, height=False)
        self.wm_iconbitmap(self.duh_logo_ico)
        self.attributes("-topmost", True)
        self.lift()
        self.attributes("-topmost", False)
        self.geometry(f"{self.win_width}x{self.win_height}+{int(self.winfo_screenwidth() / 2 - self.win_width / 2)}+{int(self.winfo_screenheight() / 2 - self.win_height / 2)}")

        self.progress_var = tk.IntVar()

        self.progressbar = ttk.Progressbar(self, variable=self.progress_var, maximum=100, mode='determinate')
        self.progressbar.pack(fill=tk.X, ipady=7, padx=20, pady=20)

        self.progress_lbl = ttk.Label(self)
        self.progress_lbl.pack(padx=20, fill=tk.X)

        self.style = ttk.Style(self.model.theme)


    def set_progress(self, value=20, text="Wird installiert...", intervall=0.01):
        """
        builds progress with given intervall until the given value is reached
        """
        self.progress_lbl.config(text=text)
        for i in range(self.progress_var.get(), value + 1):
            self.progress_var.set(i)
            self.update_idletasks()
            time.sleep(intervall)

        if self.progress_var.get() == 100:
            self.destroy()
            self.quit()
