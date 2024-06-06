from controller.Update import install_update
from view.View import View
from model.Model import Model
from controller.Controller import Controller

if __name__ == "__main__":
    config_file = "config.json"
    app_date = "04.06.2024"
    app_author = "Niklas Beitler"
    app_support_mail = "niklas.beitler@duh-group.com"
    update_url = "https://api.github.com/repos/Prometheus606/NX-Start-Center"
    app_version = "V1.1.1"

    if not install_update(app_version, update_url):
        model = Model(config_file, app_version, app_date, app_author, app_support_mail)
        view = View(model)
        controller = Controller(model, view)
        controller.start()