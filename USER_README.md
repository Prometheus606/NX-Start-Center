# NX_StartCenter

NX_StartCenter ist ein Python-Tool, das die Verwaltung und den Start von Siemens NX erleichtert. Mit diesem Tool können Sie Siemens NX in verschiedenen Modi starten, darunter die native Version und benutzerdefinierte Kundenumgebungen. Darüber hinaus bietet NX_StartCenter die Möglichkeit, den Explorer und Visual Studio Code (VSCode) an der richtigen Stelle zu starten, um eine nahtlose Integration mit den gewählten Kundenumgebungen zu gewährleisten.

## Funktionen

- **Native Version starten:** Wählen Sie aus allen installierten Siemens NX-Versionen und starten Sie die gewünschte Version.
- **Kundenumgebungen starten:** Wählen Sie aus allen verfügbaren Kundenumgebungen und starten Sie Siemens NX in der entsprechenden Umgebung.
- **Explorer und VSCode starten:** Öffnen Sie den Explorer und VSCode direkt in der ausgewählten Kundenumgebung.
- **Einstellungsfenster:** Ändern Sie das Theme des Tools und passen Sie die Installationspfade von Siemens NX und den Kundenumgebungen an.
- **Speicherung der Einstellungen:** Das Tool speichert die zuletzt verwendeten Einstellungen, sodass beim nächsten Start keine erneuten Eingaben erforderlich sind.
- **Anpassung der Start-Batch:** Die startbatch kann angepasst werden. Lesen Sie dazu den Punkt "Anpassung der Start-Batch"

   
## Installation
1. Installieren Sie Python und setzen Sie den Haken bei "Hinzufügen zu PATH":
~~~ bash
https://www.python.org/downloads/windows/
~~~
2. Installieren Sie Visual Studio Code und setzen Sie den Haken bei "Hinzufügen zu PATH":
~~~ bash
https://code.visualstudio.com/
~~~
4. Starten Sie die Datei DUH_Startcenter-installer.exe
5. Passen SIe die EInstellungen an.

## Verwendung

1. Starten Sie das Tool:
    ```bash
    DUH_Startcenter-installer.exe
    ```
2. Wählen Sie die gewünschte Siemens NX-Version oder Kundenumgebung aus.
3. Starten Sie Siemens NX, den Explorer oder VSCode mit den ausgewählten Einstellungen.
4. Passen Sie bei Bedarf die Einstellungen im Einstellungsfenster an.

## Einstellungen

Im Einstellungsfenster können Sie:
- Das Theme des Tools ändern.
- Die Installationspfade von Siemens NX und den Kundenumgebungen anpassen.

## Anpassung der Start-Batch

Änderungen an der Startlogik und weiteren Einstellungen können in der Datei `startbatch.py` oder `startbatch.bat` vorgenommen werden. 
Hier können Sie die Batch-Befehle und andere Konfigurationen anpassen, um das Tool an Ihre spezifischen Anforderungen anzupassen. 
ACHTUNG! Diese Änderungen werden bei Updates Überschrieben! 
Anpassungen die nicht überschrieben werden sollen, können in "user_settings.py" gemacht werden.

## Lizenz

Dieses Projekt ist unter der GNU-Lizenz lizenziert. Weitere Informationen finden Sie in der `LICENSE`-Datei.
