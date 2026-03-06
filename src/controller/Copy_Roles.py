import shutil
import getpass
from pathlib import Path
import os


def Copy_Roles(controller, nx_version):
    # Rolle(n)in die jeweilige NX version kopieren
    roles = controller.model.roles_path
    if roles and roles != "":
        username = getpass.getuser()
        roles = roles.split(";")
        destination = f"C:\\Users\\{username}\\AppData\\Local\\Siemens\\{nx_version}\\roles\\"
        if not Path(destination).exists() or not Path(destination).is_dir():
            os.mkdir(destination)
        for role in roles:
            if role != "" and Path(role).exists() and Path(role).is_file():
                shutil.copy(role, destination)