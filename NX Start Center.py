import os
from dotenv import load_dotenv
from controller.Update import install_update
from view.View import View
from model.Model import Model
from controller.Controller import Controller

if __name__ == "__main__":
    load_dotenv()
    config_file = os.getenv('CONFIG_FILE')
    app_date = os.getenv('APP_DATE')
    app_author = os.getenv('APP_AUTHOR')
    app_support_mail = os.getenv('APP_SUPPORT_MAIL')
    update_url = os.getenv('UPDATE_URL')
    api_token = os.getenv('API_TOKEN')
    app_version = "v1.0.2"

    if not install_update(app_version, update_url, api_token):
        model = Model(config_file, app_version, app_date, app_author, app_support_mail)
        view = View(model)
        controller = Controller(model, view)
        controller.start()