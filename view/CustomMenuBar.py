import tkinter as tk
from tkinter import messagebox

class CustomMenuBar(tk.Frame):
    """
    This class creates a custom menu bar.
    You can customize the menu labels and the colors in the constructor.
    The functions are added in the controller.

    Allowed in the menu lists: label, command or submenu
    """
    def __init__(self, master, model, *args, **kwargs):
        super().__init__(master, *args, **kwargs)
        self.model = model

        self.file_menu = [
            {'label': 'Projekte', 'command': self.placeholder},
            {'label': 'Einstellungen', 'command': self.placeholder},
            # {'label': 'Export As', 'submenu': [
            #     {'label': '.TXT', 'command': self.export_as_txt},
            #     {'label': '.PDF', 'command': self.export_as_pdf},
            # ]},
            # {},
            {"separator"},
            {'label': 'Beenden', 'command': self.quit},
        ]

        self.tools_menu = [
            {'label': 'Lizenzchecker', 'command': self.placeholder},
            {'label': 'LMTools starten', 'command': self.placeholder}
        ]

        self.menus = [
            {'label': 'Datei', 'submenu': self.file_menu},
            # {'label': 'Tools', 'submenu': self.tools_menu},
            {'label': 'Einstellungen', 'command': self.placeholder},
            {'label': 'Info', 'command':self.show_info},
        ]

        self.fg_color = "white"
        self.hover_fg_color = "darkgray"
        self.hover_bg_color = "black"

        self.create_menubar()


    def create_menubar(self):
        for menu in self.menus:
            lbl = tk.Label(self, text=menu['label'], fg=self.fg_color, padx=5)
            lbl.bind("<Enter>", lambda e, _lbl=lbl: self.on_hover(_lbl))
            lbl.bind("<Leave>", lambda e, _lbl=lbl: self.on_leave(_lbl))
            lbl.bind("<Button-1>", lambda e, _lbl=lbl, _menu=menu: self.on_click(_lbl, _menu))
            lbl.pack(side=tk.LEFT, padx=5, pady=5)

    def on_hover(self, lbl: tk.Label):
        lbl.config(fg=self.hover_fg_color)

    def on_leave(self, lbl: tk.Label):
        lbl.config(fg=self.fg_color)

    def on_click(self, lbl: tk.Label, menu: dict):
        lbl.config(fg=self.fg_color)
        if 'command' in menu:
            menu['command']()
        if 'submenu' in menu:
            self.show_submenu(lbl, menu['submenu'])


    def show_submenu(self, lbl: tk.Label, submenu: list[str]):
        """
        This Method creates the submenus. They are defined in the constructor.
        :param lbl: tk.Label object
        :param submenu: list of items in the submenu
        """
        menu = tk.Menu(self, tearoff=0, fg=self.fg_color, activebackground=self.hover_bg_color, activeforeground=self.hover_fg_color)
        for item in submenu:
            if 'command' in item:
                menu.add_command(label=item['label'], command=item['command'])
            elif 'submenu' in item:
                submenu = tk.Menu(menu, tearoff=0, fg=self.fg_color, activebackground=self.hover_bg_color, activeforeground=self.hover_fg_color)
                for subitem in item['submenu']:
                    submenu.add_command(label=subitem['label'], command=subitem['command'])
                menu.add_cascade(label=item['label'], menu=submenu)
            else:
                menu.add_separator()
        menu.post(lbl.winfo_rootx(), lbl.winfo_rooty() + lbl.winfo_height())


    def placeholder(self):
        """The real functions are added in the controller"""
        pass

    def show_info(self):
        messagebox.showinfo("Info", f"""
        Dieses Tool wurde mit Python entwickelt
        und soll das starten von NX Native und in 
        verschiedenen Kundenumgebungen ermöglichen.
         
        Entwickler: {self.model.app_author}
        E-mail: {self.model.app_support_mail}
        Version: {self.model.app_version}
        Datum: {self.model.app_date}
        """)
