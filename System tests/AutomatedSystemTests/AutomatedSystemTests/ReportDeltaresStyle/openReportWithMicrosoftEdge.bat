@ECHO OFF
SET running=0
FOR /f "tokens=*" %%A IN ('tasklist^ /v^| findstr /i /c:"msedge.exe"') DO SET running=1
IF %running%==1 ECHO Microsoft Edge is running! Please, close it before opening the report.
IF %running%==1 PAUSE

IF %running%==0 ECHO Opening report...
IF %running%==0 FOR /F %%g IN ('dir /b *.html') DO START "" "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe"  --allow-file-access-from-files %~dps0%%g