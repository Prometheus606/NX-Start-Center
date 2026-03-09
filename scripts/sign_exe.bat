@echo off
setlocal

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign /sha1 "25b8123c7ea491f66dc913e37d668a7297f35ab6" /t http://time.certum.pl /fd SHA256 /v DUH_Startcenter.exe

pause
endlocal
exit /b