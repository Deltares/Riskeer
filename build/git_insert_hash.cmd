REM This script is used for adding the GIT (short) hash to the properties of
REM a .Net assembly (AssemblyInformationalVersion).
REM It has to be positioned in the present dir, where the solution is,
REM because the this script (the pre-build-events in the .Net projects)
REM refer to $(SolutionDir)

setlocal enabledelayedexpansion
@ECHO off

SET SEARCHTEXT=GITHASH
SET ORG_DIR=%CD%
SET INTEXTFILE=%2
SET OUTTEXTFILE=%3
SET TEMPTEXTFILE= %OUTTEXTFILE%.temp

CD %1

REM GET THE GIT SHORT HASH
FOR /f %%i in ('git rev-parse --short HEAD') do set GIT_HASH=%%i

REM REMOVE PREVIOUS TEMP FILE
IF EXIST %TEMPTEXTFILE% (
    DEL %TEMPTEXTFILE%
)

REM SUBSTITUTE THE GIT SHORT HASH IN TEMPLATE
FOR /f "tokens=1,* delims=¶" %%A IN ( '"type %INTEXTFILE%"') DO (
    SET string=%%A
    SET modified=!string:%SEARCHTEXT%=%GIT_HASH%!
    ECHO !modified! >> %TEMPTEXTFILE%
)

REM COMPARE TEMP FILE WITH OUTFILE
FC /A /L %TEMPTEXTFILE% %OUTTEXTFILE%

IF %ERRORLEVEL% == 0 (
    REM IF THEY ARE IDENTICAL
    DEL %TEMPTEXTFILE%
) ELSE (
    REM IF DIFFERENT
    MOVE /Y %TEMPTEXTFILE% %OUTTEXTFILE%
)

CD %ORG_DIR%

:EOF