from controller.Update import install_update
from view.View import View
from model.Model import Model
from controller.Controller import Controller
import env

if __name__ == "__main__":
    config_file = env.CONFIG_FILE
    app_date = env.APP_DATE
    app_author = env.APP_AUTHOR
    app_support_mail = env.APP_SUPPORT_MAIL
    update_url = env.UPDATE_URL
    app_version = env.VERSION

    model = Model(config_file, app_version, app_date, app_author, app_support_mail)

    # if not install_update(model, update_url):
    view = View(model)
    controller = Controller(model, view)
    controller.start()