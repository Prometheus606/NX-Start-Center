import requests
import os
from tkinter import messagebox


def install_update(current_version, repo_url, api_token):
    api_url = f"{repo_url}/releases/latest"

    headers = {'Authorization': f'token {api_token}'}
    response = requests.get(api_url, headers=headers)
    if response.status_code == 200:
        latest_release = response.json()
        latest_version = latest_release["tag_name"]

        if latest_version.lower() > current_version.lower():
            download_url = latest_release["assets"][0]["browser_download_url"]
            if user_accepts_update():
                download_and_install_update(download_url, latest_version)
                return True
    else:
        print("Fehler beim Abrufen der Update-Informationen.")
    return False


def user_accepts_update():
    print(f"Ein neues Update ist verfügbar!")
    return messagebox.askyesno("Update verfügbar", "Eine neue Version ist verfügbar! Jetzt installieren?")


def download_and_install_update(download_url, latest_version):
    response = requests.get(download_url)
    download_path = os.path.expanduser(f"~/Downloads/startcenter installer {latest_version}.exe")
    with open(download_path, "wb") as file:
        file.write(response.content)
    execute_installer(download_path)


def execute_installer(installer_path):
    import subprocess
    subprocess.run([installer_path])