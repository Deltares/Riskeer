@ECHO OFF

SET logfileDir=%CD%
ECHO %logfileDir%
CD /D "%APPDATA%\Mozilla\Firefox\Profiles"
CD *.default
SET ffile=%CD%
FOR /F "tokens=2 delims=,)" %%g IN ('type "%ffile%\prefs.js" ^| findstr "privacy.file_unique_origin"') DO FOR /F %%h IN ('echo %%g') DO SET setting=%%h
IF "%setting%"=="false" GOTO CanOpen

REM Report cannot be opened
ECHO Configuration must be changed to enable reading the repport. The option privacy.file_unique_origin must be set to false. Check the file 
ECHO.
ECHO howToEnableMozillaFirefoxToOpenReport.png
ECHO.
ECHO to see how to proceed. Once changed this setting, run again this batch file.
PAUSE
GOTO eof

REM Report can be opened
:CanOpen
ECHO Opening report...
CD /D %logfileDir%
FOR /F %%g IN ('dir /b *.html') DO START "" "%ProgramFiles%\Mozilla Firefox\firefox.exe" -new-window %~dps0%%g
