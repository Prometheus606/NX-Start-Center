import os

class OpenExplorer:
    """
    This class handles the Open Explorer Button
    """
    def __init__(self, controller):
        self.controller = controller
        self.controller.view.pp_dir_btn.config(command=self.open_pp_dir)

    def open_pp_dir(self):
        self.controller.view.messageLabel.config(text="")

        machine_dir = fr"{self.controller.model.get_installed_machines_dir_path()}\{self.controller.model.machine}"
        path = os.path.realpath(machine_dir)
        os.startfile(path)