@echo OFF
rem start nuitka --standalone --onefile --windows-console-mode=disable --enable-plugin=tk-inter --output-dir=output/exe --output-file=DUH_Startcenter.exe --windows-icon-from-ico=src/images/duhGroup_Logo.ico NX_Startcenter.py

pyinstaller --noconfirm --onefile --windowed --name "DUH_Startcenter" ^
--icon "D:/Projekte/Projekte_Python/duh_startcenter/src/images/duhGroup_Logo.ico" ^
--add-data "D:/Projekte/Projekte_Python/duh_startcenter/controller;controller/" ^
--add-data "D:/Projekte/Projekte_Python/duh_startcenter/model;model/" ^
--add-data "D:/Projekte/Projekte_Python/duh_startcenter/view;view/" ^
--add-data "D:/Projekte/Projekte_Python/duh_startcenter/env.py;." ^
--add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/site-packages/ttkbootstrap;ttkbootstrap/" ^
--add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/site-packages/ttkcreator;ttkcreator/" ^
--add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/tkinter;tkinter/" ^
--add-data "C:/Users/niklas.beitler/AppData/Local/Programs/Python/Python312/Lib/site-packages/PIL;PIL/" ^
--hidden-import "PIL._imagingft" --hidden-import "PIL._imaging" ^
--distpath "D:/Projekte/Projekte_Python/duh_startcenter/output/exe" ^
--workpath "D:/Projekte/Projekte_Python/duh_startcenter/output/build" ^
--specpath "D:/Projekte/Projekte_Python/duh_startcenter/output/spec" ^
"D:/Projekte/Projekte_Python/duh_startcenter/NX_Startcenter.py"

pause
exit