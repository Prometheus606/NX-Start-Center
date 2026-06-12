# Update Releasen
### 1. Alle änderungen comitten
### 2. Tag erstellen in dem Format Vx.x.x
### 3. Repo Pushen
### 4. Den Token für SimplySign bei der IT anfordern um signieren zu können
### 5. das release script starten (create_full_release.bat)
    Was das script tut:
    1. Auf uncomittete änderungen prüfen und dann ggf. abbrechen
    2. Versionsnummer aus dem letzten Tag ziehen
    3. Versionsnummer temporär in das .iss script schreiben, damit auch der installer die richtige versionsnummer hat
    4. Die Versionsnummer in AppInfo.cs schreiben
    5. Das Projekt als release builden und die resultierende .exe datei signieren
    6. Die installer.exe erstellen und signieren
    7. die installer datei passend umbenennen
    8. die änderung in der .iss datei rückgängig machen, damit die änderung nicht im repo auftaucht.

### 6. auf github ein neues release aus dem letzten tag erstellen und installer.exe dort reinziehen (aus output/installer)
### 7. Beim nächsten starten der alten app kommt eine meldung das es eine neue version gibt