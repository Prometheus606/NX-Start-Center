# Update Releasen
#### Alle Versionnummern an allen orten müssen mit v oder V beginnen!
### 1. neue versionsnummer und datum in env.py eintragen
### 2. exe datei erstellen mit der create_exe.bat:
### 3. create_installer.iss mit Inno Setup öffnen und neu kompilieren (output/installer)
### 4. repo auf github pushen und tag erstellen mit neuer versionsnummer
~~~ Bash
git add .
git commit -m "Update Info" for short message or just git commit for long message
git branch -M main
git tag -a v1.0.0 -m "Version 1.0.0 Release"
git push -u origin main --tags
~~~
### 5. auf github ein neues release aus dem letzten tag erstellen und installer.exe dort reinziehen (aus output/installer/Output)
### 6. password beim installer:
~~~
7. @tm5$a!Nt#7!HzAH
~~~


### exe datei erstellen mit pyinstaller: 
~~~ Bash
pyinstaller "DUH_Startcenter.py" --windowed --icon ".\src\images\duhGroup_Logo.ico" --add-data "./venv/Lib/site-packages/ttkbootstrap;ttkbootstrap" --add-data "./venv/Lib/site-packages/dotenv;dotenv" --add-data "./venv/Lib/site-packages/ttkcreator;ttkcreator" --add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python311/Lib/site-packages/customtkinter;customtkinter" --add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python311/Lib/tkinter;tkinter" --add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python311/Lib/site-packages/PIL;PIL" --onefile -y --clean --distpath "./output/exe"
~~~
- Terminal im Hauptorder öffnen
- befehl reinkopieren
- Dieser Command muss direkt in der CMD eingegeben werden, nicht über eine IDE oder python code!