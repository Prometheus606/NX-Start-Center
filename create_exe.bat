@echo OFF
start nuitka --standalone --onefile --windows-console-mode=disable --enable-plugin=tk-inter --output-dir=output/exe --output-file=DUH_Startcenter.exe --windows-icon-from-ico=src/images/duhGroup_Logo.ico NX_Startcenter.py
pause
exit