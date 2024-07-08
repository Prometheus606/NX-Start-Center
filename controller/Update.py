# import requests
# import os
# from tkinter import messagebox
# from view.ProgressWindow import ProgressWindow
#
#
# def install_update(model, repo_url):
#     try:
#         api_url = f"{repo_url}/releases/latest"
#
#         response = requests.get(api_url)
#         if response.status_code == 200:
#             latest_release = response.json()
#             latest_version = latest_release["tag_name"]
#
#             if latest_version.lower() > model.app_version.lower():
#                 download_url = latest_release["assets"][0]["browser_download_url"]
#                 title = latest_release["name"]
#                 description = latest_release["body"]
#                 if user_accepts_update(model.app_version, latest_version, description, title):
#                     download_and_install_update(model, download_url, latest_version)
#                     return True
#         else:
#             print("Fehler beim Abrufen der Update-Informationen.")
#         return False
#
#     except requests.exceptions.ConnectionError:
#         print("No connection.")
#
#
# def user_accepts_update(current_version, latest_version, description, title):
#     # print(f"Ein neues Update ist verfügbar!")
#     return messagebox.askyesno("Update verfügbar", f"Eine neue Version ist verfügbar!\n\nAktuelle Version:{current_version}\nNeue Version: {latest_version}\n\nWas macht das neue Update:\n{description}\n\nJetzt installieren?")
#
#
# def download_and_install_update(model, download_url, latest_version):
#     # progress_window = ProgressWindow(model)
#     # progress_window.set_progress(30, "Wird heruntergeladen...", 0.05)
#
#     try:
#         response = requests.get(download_url)
#         response.raise_for_status()
#
#         # progress_window.set_progress(60, "Wird heruntergeladen...", 0.05)
#
#         download_path = os.path.expanduser(f"~/Downloads/startcenter-installer-{latest_version}.exe")
#         with open(download_path, "wb") as file:
#             for chunk in response.iter_content(chunk_size=8192):
#                 file.write(chunk)
#
#         # progress_window.set_progress(80, "Wird installiert...", 0.05)
#
#         # execute_installer(download_path)
#
#         # progress_window.set_progress(100, "Wird installiert...", 0.02)
#
#     except requests.exceptions.RequestException as e:
#         print(f"Error downloading the update: {e}")
#
#     # progress_window.mainloop()
#
#
# def execute_installer(installer_path):
#     pass
#     # import subprocess
#     # subprocess.Popen([installer_path])
