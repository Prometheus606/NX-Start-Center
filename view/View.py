import ctypes as ct
import os
import tkinter as tk
import ttkbootstrap as ttk
from view.CustomMenuBar import CustomMenuBar
from view.SettingFrame import SettingFrame
from view.NativStartFrame import NativStartFrame
from view.ImageFrame import ImageFrame
from view.ProjectFrame import ProjectFrame
from view.ProjectOptionFrame import ProjectOptionFrame
from view.LanguageFrame import LanguageFrame

class View(tk.Tk):
    def __init__(self, model):
        super().__init__()

        # ==================================== variables ====================================
        self.model = model
        self.win_width = 700
        self.win_height = 550
        self.duh_logo_png = fr'src/images/duhGroup_Logo.png'
        self.duh_logo_ico = fr"src/images\duhGroup_Logo.ico"
        self.folder_image = fr"src/images/folder.png"

        self.customer = ttk.StringVar()
        self.customer.set(self.model.customer)
        self.version = ttk.StringVar()
        self.version.set(self.model.version)
        self.machine = ttk.StringVar()
        self.machine.set(self.model.machine)

        self.new_customer = ttk.StringVar()
        self.new_version = ttk.StringVar()
        self.new_machine = ttk.StringVar()
        self.new_order = ttk.StringVar()

        self.load_pp = ttk.BooleanVar()
        self.load_cse = ttk.BooleanVar()
        self.load_device = ttk.BooleanVar()
        self.load_tool = ttk.BooleanVar()
        self.load_feed = ttk.BooleanVar()

        self.language = ttk.StringVar()

        self.native_version = ttk.StringVar()

        self.nx_installation_path = tk.StringVar()
        self.customer_environment_path = tk.StringVar()
        self.theme = tk.StringVar()
        self.batchstart = ttk.BooleanVar()

        # ==================================== menubar ====================================

        self.menubar = CustomMenuBar(self, self.model)
        self.menubar.pack(side="top", fill="x")

        # ==================================== General settings ====================================

        self.title("NX Startcenter")
        self.resizable(width=False, height=False)
        self.wm_iconbitmap(self.duh_logo_ico)
        self.attributes("-topmost", True)
        self.lift()
        self.attributes("-topmost", False)
        self.geometry(f"{self.win_width}x{self.win_height}+{int(self.winfo_screenwidth() / 2 - self.win_width / 2)}+{int(self.winfo_screenheight() / 2 - self.win_height / 2)}")

        # =================================== root frames ==================================

        self.left_frame = ttk.Frame(self)
        self.left_frame.pack(side=ttk.LEFT, fill=ttk.BOTH, expand=True, padx=10, pady=10)

        self.right_frame = ttk.Frame(self)
        self.right_frame.pack(side=ttk.RIGHT, fill=ttk.BOTH, expand=True, padx=10, pady=10)

        # ==================================== Project Frame ====================================

        self.register = ProjectFrame(self.left_frame, self.model, self.customer, self.version, self.machine, self.new_customer, self.new_version, self.new_machine, self.new_order)
        self.register.pack(fill="both")

        # ==================================== Project options group ====================================

        self.option_frame = ProjectOptionFrame(self.left_frame, self.load_pp, self.load_cse, self.load_tool, self.load_device, self.load_feed, text="Ladeoptionen")
        self.option_frame.pack(fill="both", expand=False, pady=10)

        # ==================================== language frame ====================================

        self.language_frame = LanguageFrame(self.left_frame, self.language, text="Spracheinstellung")
        self.language_frame.pack(fill="both", expand=False, pady=5)

        # ==================================== Image group ====================================

        self.image_frame = ImageFrame(self.right_frame, self.duh_logo_png)
        self.image_frame.pack(fill="both", expand=False)

        # ==================================== Error Message ====================================

        self.message_frame = ttk.Frame(self.right_frame, height=50)
        self.message_frame.pack(fill="both", expand=False)

        self.messageLabel = ttk.Label(self.message_frame, foreground="red")
        self.messageLabel.pack(fill="both", expand=False)

        # ==================================== NX Nativ start group ====================================

        self.native_frame = NativStartFrame(self.right_frame, self.model, self.native_version, text="NX Nativ starten")
        self.native_frame.place(x=0, y=317, width=260)

        # ==================================== Buttons at the bottom ====================================

        self.buttons_frame = ttk.Frame(self.left_frame)
        self.buttons_frame.pack(side=ttk.BOTTOM, fill="both", expand=False)

        self.start_btn = ttk.Button(self.buttons_frame, text="NX Starten")
        self.start_btn.pack(side=ttk.LEFT, padx=5, pady=20)

        self.pp_dir_btn = ttk.Button(self.buttons_frame, text="Maschinen Ordner Öffnen")
        self.pp_dir_btn.pack(side=ttk.LEFT, padx=5)

        self.vsCode_btn = ttk.Button(self.buttons_frame, text="VS Code Öffnen")
        self.vsCode_btn.pack(side=ttk.LEFT, padx=5)

        # ==================================== Setting Frame ====================================

        self.setting_frame = SettingFrame(self, self.model, self.theme, self.nx_installation_path, self.customer_environment_path, "src/images/folder.png", self.batchstart, text="Einstellungen")

        self.style = ttk.Style(self.model.theme)


    def dark_title_bar(self):
        """
        Changes the tkinter Titlebar to dark
        MORE INFO:
        https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute
        """
        self.update()
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20
        set_window_attribute = ct.windll.dwmapi.DwmSetWindowAttribute
        get_parent = ct.windll.user32.GetParent
        hwnd = get_parent(self.winfo_id())
        rendering_policy = DWMWA_USE_IMMERSIVE_DARK_MODE
        value = 2    # 0: light, 2: dark
        value = ct.c_int(value)
        set_window_attribute(hwnd, rendering_policy, ct.byref(value),ct.sizeof(value))

    def light_title_bar(self):
        """
        Changes the tkinter Titlebar to light (default value windows)
        MORE INFO:
        https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute
        """
        self.update()
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20
        set_window_attribute = ct.windll.dwmapi.DwmSetWindowAttribute
        get_parent = ct.windll.user32.GetParent
        hwnd = get_parent(self.winfo_id())
        rendering_policy = DWMWA_USE_IMMERSIVE_DARK_MODE
        value = 0    # 0: light, 2: dark
        value = ct.c_int(value)
        set_window_attribute(hwnd, rendering_policy, ct.byref(value),ct.sizeof(value))
