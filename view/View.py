import ctypes as ct
import tkinter as tk
import ttkbootstrap as ttk
from view.CustomMenuBar import CustomMenuBar
from view.SettingFrame import SettingFrame
from view.NativStartFrame import NativStartFrame
from view.ImageFrame import ImageFrame
from view.ProjectFrame import ProjectFrame
from view.ProjectOptionFrame import ProjectOptionFrame
from view.LanguageFrame import LanguageFrame
from PIL import Image, ImageTk  # Für das Icon-Bild

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
        self.new_machine_controller = ttk.StringVar()
        self.new_machine_type = ttk.StringVar()

        self.load_pp = ttk.BooleanVar()
        self.load_installed_machines = ttk.BooleanVar()
        self.load_device = ttk.BooleanVar()
        self.load_tool = ttk.BooleanVar()
        self.load_feed = ttk.BooleanVar()

        self.language = ttk.StringVar()

        self.native_version = ttk.StringVar()
        self.postbuilder_version = ttk.StringVar()

        self.nx_installation_path = tk.StringVar()
        self.customer_environment_path = tk.StringVar()
        self.licence_path = tk.StringVar()
        self.licence_server_path = tk.StringVar()
        self.roles_path = tk.StringVar()
        self.theme = tk.StringVar()
        self.batchstart = ttk.BooleanVar()
        self.editor = ttk.StringVar()

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
        self.left_frame.pack(side=ttk.LEFT, fill=ttk.BOTH, expand=False, padx=10, pady=10)

        self.right_frame = ttk.Frame(self)
        self.right_frame.pack(side=ttk.RIGHT, fill=ttk.BOTH, expand=True, padx=10, pady=10)

        # ==================================== Project Frame ====================================

        self.register = ProjectFrame(self.left_frame, self.model, self.customer, self.version, self.machine, self.new_customer, self.new_version, self.new_machine, self.new_order, self.new_machine_type, self.new_machine_controller)
        self.register.pack(fill="both")

        # ==================================== Project options group ====================================

        self.option_frame = ProjectOptionFrame(self.left_frame, self.load_pp, self.load_installed_machines, self.load_tool, self.load_device, self.load_feed, text="Ladeoptionen")
        self.option_frame.pack(fill="both", expand=False, pady=10)

        # ==================================== language frame ====================================

        self.language_frame = LanguageFrame(self.left_frame, self.language, text="Spracheinstellung")
        self.language_frame.pack(fill="both", expand=False, pady=5)

        # ==================================== Image group ====================================

        self.image_frame = ImageFrame(self.right_frame, self.duh_logo_png)
        self.image_frame.pack(fill="both", expand=False)

        # ==================================== Error Message ====================================

        # Kopier-Icon laden (ersetze durch deinen eigenen Pfad oder nutze ein Tkinter-Symbol)
        copy_icon_path = "src/images/copy_icon.png"  # Pfad zum Icon
        copy_image = Image.open(copy_icon_path).resize((16, 16))  # Größe anpassen
        copy_icon = ImageTk.PhotoImage(copy_image)
        self.copy_icon_hover = ImageTk.PhotoImage(copy_image.point(lambda p: min(255, p + 80)))  # Heller für Hover

        self.message_frame = ttk.Frame(self.right_frame, height=0)
        self.message_frame.pack(fill="both", expand=False)

        # Button mit Kopier-Icon
        self.copy_button = ttk.Button(
            self.message_frame,
            image=copy_icon,
            command=self.copy_to_clipboard,
            style="Transparent.TButton",
            takefocus=0,
            padding=0
        )
        self.copy_button.image = copy_icon  # Verhindert, dass das Bild aus dem Speicher gelöscht wird

        # Hover-Effekt hinzufügen
        self.copy_button.bind("<Enter>", self.on_hover)
        self.copy_button.bind("<Leave>", self.on_leave)

        # # Tooltip hinzufügen
        self.copy_button_tooltip = None  # Tooltip-Referenz speichern
        self.copy_button.bind("<Enter>", self.show_tooltip)
        self.copy_button.bind("<Leave>", self.hide_tooltip)

        self.messageLabel = ttk.Label(self.message_frame, foreground="red", wraplength=257, justify="left")

        # Tooltip erstellen
        self.messageLabel_tooltip = None
        self.messageLabel.bind("<Enter>", self.show_tooltip1)
        self.messageLabel.bind("<Leave>", self.hide_tooltip)

        # ==================================== NX Nativ start group ====================================

        self.native_frame = NativStartFrame(self.right_frame, self.model, self.native_version, self.postbuilder_version)
        self.native_frame.place(x=0, y=293, width=260)

        # ==================================== Buttons at the bottom ====================================

        self.buttons_frame = ttk.Frame(self.left_frame)
        self.buttons_frame.pack(side=ttk.BOTTOM, fill="both", expand=False)

        self.start_btn = ttk.Button(self.buttons_frame, text="NX Starten")
        self.start_btn.pack(side=ttk.LEFT, padx=5, pady=20)

        self.pp_dir_btn = ttk.Button(self.buttons_frame, text="Maschinen Ordner Öffnen")
        self.pp_dir_btn.pack(side=ttk.LEFT, padx=5)

        self.vsCode_btn = ttk.Button(self.buttons_frame, text="PP Öffnen")
        self.vsCode_btn.pack(side=ttk.LEFT, padx=5)

        # ==================================== Setting Frame ====================================

        self.setting_frame = SettingFrame(self, self.model, self.theme, self.nx_installation_path, self.customer_environment_path, self.licence_path, self.licence_server_path, "src/images/folder.png", self.editor, self.batchstart, self.roles_path, text="Einstellungen")

        self.style = ttk.Style(self.model.theme)

    # Funktion zum Kopieren des Fehlertexts
    def copy_to_clipboard(self):
        self.clipboard_clear()
        self.clipboard_append(self.messageLabel["text"])
        self.update()  # Notwendig, damit der Text wirklich kopiert wird

    def set_message(self, message="", color="red"):
        """ Aktualisiert das Label und zeigt/versteckt den Button je nach Inhalt. """
        self.messageLabel.config(text="", foreground="red")

        if message:  # Wenn Text vorhanden ist, zeige den Button
            self.messageLabel.config(text=message, foreground=color)
            self.copy_button.pack(side="right", padx=3)
            self.messageLabel.pack(fill="both", expand=True)
        else:  # Kein Text → Button entfernen
            self.copy_button.pack_forget()
            self.messageLabel.pack_forget()

    def on_hover(self, event):
        """ Ändert das Icon bei Mouse-Over. """
        self.copy_button.config(image=self.copy_icon_hover)

    def on_leave(self, event):
        """ Setzt das Icon zurück, wenn die Maus den Button verlässt. """
        self.copy_button.config(image=self.copy_icon)

    def show_tooltip(self, event):
        """ Zeigt einen Tooltip mit der Nachricht "Text kopieren". """
        if self.copy_button_tooltip:
            self.copy_button_tooltip.destroy()  # Falls bereits ein Tooltip existiert, entfernen

        x, y, _, _ = self.copy_button.bbox("insert")  # Button-Position
        x += self.copy_button.winfo_rootx() + 20
        y += self.copy_button.winfo_rooty() - 20

        self.copy_button_tooltip = tk.Toplevel(self)
        self.copy_button_tooltip.wm_overrideredirect(True)  # Entfernt Fensterrahmen
        self.copy_button_tooltip.geometry(f"+{x}+{y}")

        label = tk.Label(self.copy_button_tooltip, text="Text kopieren", bg="yellow", relief="solid", borderwidth=1, padx=5, pady=2)
        label.pack()

    def hide_tooltip(self, event):
        """ Entfernt den Tooltip, wenn die Maus den Button verlässt. """
        if self.copy_button_tooltip:
            self.copy_button_tooltip.destroy()
            self.copy_button_tooltip = None
        elif self.messageLabel_tooltip:
                self.messageLabel_tooltip.destroy()
                self.messageLabel_tooltip = None



    def show_tooltip1(self, event):
        """Zeigt einen Tooltip mit dem vollständigen Text an"""
        if self.messageLabel_tooltip:
            self.messageLabel_tooltip.destroy()

        self.messageLabel_tooltip = tk.Toplevel(self)
        self.messageLabel_tooltip.wm_overrideredirect(True)  # Kein Fensterrahmen
        self.messageLabel_tooltip.geometry(f"+{self.winfo_pointerx()}+{self.winfo_pointery() + 20}")  # Position nahe Maus

        label = ttk.Label(self.messageLabel_tooltip, text=self.messageLabel["text"], relief="solid", borderwidth=1,
                          padding=5, wraplength=300)
        label.pack()


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
