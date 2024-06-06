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
                title = latest_release["name"]
                description = latest_release["body"]
                if user_accepts_update(current_version, latest_version, description, title):
                    download_and_install_update(download_url, latest_version)
                    return True
        else:
            print("Fehler beim Abrufen der Update-Informationen.")
        return False

    except requests.exceptions.ConnectionError:
        print("No connection.")


def user_accepts_update(current_version, latest_version, description, title):
    print(f"Ein neues Update ist verfügbar!")
    return messagebox.askyesno("Update verfügbar", f"Eine neue Version ist verfügbar!\n\nDeine Version:{current_version}\nNeue Version: {latest_version}\n\nWas ist neu:\n{description}\n\nJetzt installieren?")


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
    subprocess.Popen([installer_path])
