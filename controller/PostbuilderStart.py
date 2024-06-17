import subprocess
from controller.Config_file_handler import save_config

class PostbuilderStart:
    """
    This class handles the Start NX Native Frame
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.native_frame.start_postbuilder_btn.config(command=self.start_postbuilder)
        self.controller.view.native_frame.postbuilder_combobox.bind("<<ComboboxSelected>>", self.postbuilder_version_selected)

    def start_postbuilder(self):
        self.controller.view.messageLabel.config(text="")

        base_path = self.controller.model.settings['nx_installation_path']
        try:
            nx_version = (int(self.controller.model.postbuilder_version.replace("NX", "").strip()))
            if nx_version < 2206:
                command = f"{base_path}\\NX{nx_version}\\POSTBUILD\\post_builder.bat {base_path}\\NX{nx_version}\\"
            else:
                command = f"{base_path}\\NX{nx_version}\\POSTBUILD\\post_builder.bat {base_path}\\NX{nx_version}\\"

            subprocess.Popen(command, stderr=subprocess.PIPE, shell=False)
            save_config(self.controller.model.config_file, "last_configuration", last_postbuilder_version=self.controller.model.postbuilder_version)

        except FileNotFoundError:
            self.controller.view.messageLabel.configure(text="Postbuilder version kann nicht gestartet werden!", foreground="red")

    def postbuilder_version_selected(self, e):
        self.controller.view.messageLabel.config(text="")
        self.controller.model.postbuilder_version = self.controller.view.postbuilder_version.get()