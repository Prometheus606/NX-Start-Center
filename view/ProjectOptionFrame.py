import ttkbootstrap as ttk

class ProjectOptionFrame(ttk.LabelFrame):
    def __init__(self, master, load_pp, load_installed_machines, load_tool, load_device, load_feed, *args, **kwargs):
        super().__init__(master, *args, **kwargs)

        self.installed_machines_check = ttk.Checkbutton(self, text="Installed Machines laden", variable=load_installed_machines, bootstyle="round-toggle")
        self.installed_machines_check.grid(row=1, column=0, sticky="w", pady=5)
        self.pp_check = ttk.Checkbutton(self, text="Postprozessor ordner laden", variable=load_pp,bootstyle="round-toggle")
        self.pp_check.grid(row=0, column=0, sticky="w", pady=5)
        self.tool_check = ttk.Checkbutton(self, text="Werkzeuge laden", variable=load_tool, bootstyle="round-toggle")
        self.tool_check.grid(row=2, column=0, sticky="w", pady=5)
        self.device_check = ttk.Checkbutton(self, text="Device laden", variable=load_device, bootstyle="round-toggle")
        self.device_check.grid(row=3, column=0, sticky="w", pady=5)
        self.feed_check = ttk.Checkbutton(self, text="Speed/Feed laden", variable=load_feed, bootstyle="round-toggle")
        self.feed_check.grid(row=4, column=0, sticky="w", pady=5)