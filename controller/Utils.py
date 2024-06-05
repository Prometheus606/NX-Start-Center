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

def open_editor(file_path, editor="notepad"):
    editor_command = {
        'notepad': [r'C:\Windows\System32\notepad.exe', file_path],
        'notepad++': [r'C:\Program Files\Notepad++\notepad++.exe', file_path],
        'vscode': [fr'C:\Users\{getpass.getuser()}\AppData\Local\Programs\Microsoft VS Code\Code.exe', file_path]
    }

    if not Path(editor_command[editor][0]).exists():
        print(f"Error: Editor not found: {editor_command[editor][0]}")
        messagebox.showerror("Fehler", f"Error: Editor not found: {editor_command[editor][0]}")
        return

    try:
        subprocess.Popen(editor_command[editor])
    except FileNotFoundError:
        print(f"Error: The file {file_path} does not exist.")
        messagebox.showerror("Fehler", f"Error: The file {file_path} does not exist.")
    except OSError as e:
        print(f"Error: Unable to open the file {file_path}. {e}")
        messagebox.showerror("Fehler", f"Error: Unable to open the file {file_path}. {e}")