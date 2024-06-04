import requests
import os
from tkinter import messagebox


def install_update(current_version, repo_url):
    try:
        api_url = f"{repo_url}/releases/latest"

        response = requests.get(api_url)
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

    except requests.exceptions.ConnectionError:
        print("No connection.")


def user_accepts_update():
    print(f"Ein neues Update ist verfügbar!")
    return messagebox.askyesno("Update verfügbar", "Eine neue Version ist verfügbar! Jetzt installieren?")


def download_and_install_update(download_url, latest_version):
    try:
        response = requests.get(download_url)
        response.raise_for_status()

        download_path = os.path.expanduser(f"~/Downloads/startcenter-installer-{latest_version}.exe")
        with open(download_path, "wb") as file:
            for chunk in response.iter_content(chunk_size=8192):
                file.write(chunk)

        execute_installer(download_path)

    except requests.exceptions.RequestException as e:
        print(f"Error downloading the update: {e}")


def execute_installer(installer_path):
    import subprocess
    subprocess.run([installer_path])
