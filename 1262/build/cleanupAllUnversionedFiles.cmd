@echo off
setlocal
ECHO WARNING! This will remove ALL unversioned files from the solution
:PROMPT
SET /P AREYOUSURE=Continue? (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END

TortoiseProc.exe /command:cleanup /nodlg /cleanup /noui /delunversioned /delignored /refreshshell /externals /path:../ /noprogressui

:END
endlocal