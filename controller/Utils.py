import os
import shutil
import subprocess
import getpass
from pathlib import Path
from tkinter import messagebox


def copy_folder(src: str, dst: str, symlinks=False, ignore=None):
    for item in os.listdir(src):
        s = os.path.join(src, item)
        d = os.path.join(dst, item)
        if os.path.isdir(s):
            shutil.copytree(s, d, symlinks, ignore)
        else:
            shutil.copy2(s, d)


def copy_file(src: str, dst: str):
    shutil.copy2(src, dst)

def remove_readonly_recursive(path):
    for root, dirs, files in os.walk(path):
        for name in files:
            file_path = os.path.join(root, name)
            os.chmod(file_path, stat.S_IWRITE)
        for name in dirs:
            dir_path = os.path.join(root, name)
            os.chmod(dir_path, stat.S_IWRITE)

def open_editor(controller, file_path, editor="notepad"):
    editor_command = {
        'notepad': [r'C:\Windows\System32\notepad.exe', file_path],
        'notepad++': [r'C:\Program Files\Notepad++\notepad++.exe', file_path],
        'vscode': [fr'C:\Users\{getpass.getuser()}\AppData\Local\Programs\Microsoft VS Code\Code.exe', file_path]
    }

    if not Path(editor_command[editor][0]).exists():
        print(f"Error: Editor not found: {editor_command[editor][0]}")
        # messagebox.showerror("Fehler", f"Error: Editor not found: {editor_command[editor][0]}\nBitte wähle einen anderen Editor.")
        controller.view.set_message(f"Editor nicht gefunden: {editor_command[editor][0]}\nBitte wähle einen anderen Editor.")
        return

    try:
        subprocess.Popen(editor_command[editor])
    except FileNotFoundError:
        print(f"Error: The file {file_path} does not exist.")
        # messagebox.showerror("Fehler", f"Error: The file {file_path} does not exist.")
        controller.view.set_message(f"Die Datei {file_path} wurde nicht gefunden.")
    except OSError as e:
        print(f"Error: Unable to open the file {file_path}. {e}")
        # messagebox.showerror("Fehler", f"Error: Unable to open the file {file_path}. {e}")
        controller.view.set_message(f"Die Datei konnte nicht geöffnet werden:\n{file_path}")