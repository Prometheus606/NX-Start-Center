import os
import shutil
import subprocess
import getpass
import stat
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

def find_executable(executables, fallback_paths=None):
    """
    Versucht ein Executable über PATH zu finden.
    Falls nicht gefunden, werden Fallback-Pfade geprüft.
    """
    # 1. PATH prüfen
    for exe in executables:
        path = shutil.which(exe)
        if path:
            return path

    # 2. Fallback-Pfade prüfen
    if fallback_paths:
        for path in fallback_paths:
            if Path(path).exists():
                return path

    return None


def open_editor(controller, file_path, editor="notepad"):

    user = getpass.getuser()

    editor_config = {
        "notepad": {
            "executables": ["notepad.exe"],
            "fallback": [r"C:\Windows\System32\notepad.exe"],
        },
        "notepad++": {
            "executables": ["notepad++.exe"],
            "fallback": [
                r"C:\Program Files\Notepad++\notepad++.exe",
                r"C:\Program Files (x86)\Notepad++\notepad++.exe",
            ],
        },
        "vscode": {
            "executables": ["code.cmd", "code.exe"],
            "fallback": [
                fr"C:\Users\{user}\AppData\Local\Programs\Microsoft VS Code\Code.exe",
            ],
        },
    }

    if editor not in editor_config:
        controller.view.set_message(f"Unbekannter Editor: {editor}")
        return

    config = editor_config[editor]

    exe_path = find_executable(
        config["executables"],
        config["fallback"]
    )

    if not exe_path:
        controller.view.set_message(
            f"Editor nicht gefunden ({editor}).\n"
            "Bitte prüfe PATH oder Installation."
        )
        return

    try:
        subprocess.Popen([exe_path, file_path])
    except FileNotFoundError:
        controller.view.set_message(
            f"Die Datei wurde nicht gefunden:\n{file_path}"
        )
    except OSError as e:
        controller.view.set_message(
            f"Die Datei konnte nicht geöffnet werden:\n{file_path}\n{e}"
        )