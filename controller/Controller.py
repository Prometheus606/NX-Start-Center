from controller.NativeStart import NativeStart
from controller.NXStart import NXStart
from controller.CreateNewCustomer import CreateNewCustomer
from controller.Settings import Settings
from controller.UiChanges import UiChanges
from controller.OpenVSCode import OpenVSCode
from controller.OpenExplorer import OpenExplorer
from controller.SetLastConfig import SetLastConfig
from controller.Licence import Licence


class Controller:
    def __init__(self, model, view):
        self.model = model
        self.view = view

        self.start_native = NativeStart(self)
        self.start_nx = NXStart(self)
        self.create_new_customer = CreateNewCustomer(self)
        self.settings = Settings(self)
        self.ui_changes = UiChanges(self)
        self.open_vscode = OpenVSCode(self)
        self.open_explorer = OpenExplorer(self)
        self.set_last_config = SetLastConfig(self)
        self.tools = Licence(self)

    def start(self):
        self.view.mainloop()
