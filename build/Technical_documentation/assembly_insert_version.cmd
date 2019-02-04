rem This script is used for adding the .Net assembly version to the properties of a doxygen configuration file
setlocal enabledelayedexpansion
@ECHO off

SET ORG_DIR=%CD%

SET argcount=0
FOR %%x IN (%*) DO SET /A argcount+=1

IF %argcount%==0 (
    ECHO "invalid arguments: [versionPath] [path] (infile) (outfile)"
    ECHO "usage: replace SVNREV text with revision number, SVNROOT with repository"
    ECHO "versionFile: path of file containing the version info"
	ECHO "path: execute at specific location"
    ECHO "[optional] infile: file to read, defaults to Riskeer.doxyfile"
    ECHO "[optional] outfile: file to create, defaults to Riskeer.doxygen"
    GOTO EOF
)

ECHO "%argcount% arguments received"

IF %argcount%==1 (
    ECHO "setting default in and out files."
	SET VERSIONPATH=..\..\Core\Common\src\Core.Common.Version
	SET VERSIONFILE=GlobalAssembly.cs
    SET INTEXTFILE=Riskeer.doxyfile
    SET OUTTEXTFILE=Riskeer.doxygen
) ELSE (
    ECHO "setting custom in and out files."
    SET VERSIONPATH=%1
    SET VERSIONFILE=%2
    SET INTEXTFILE=%3
    SET OUTTEXTFILE=%4
) 
SET TEMPTEXTFILE= %OUTTEXTFILE%.temp



REM GET THE SVN VERSION NUMBER AND  REVISION PATH
CD %VERSIONPATH%
ECHO executing in directory "%VERSIONPATH%"

FOR /f "tokens=1,* delims=¶" %%A IN ( '"type %VERSIONFILE%"') DO (
	ECHO %%A | findstr /I "^[assembly: AssemblyVersion(" && SET REV_BUF=%%A
)

IF NOT DEFINED REV_BUF (
    SET SVN_REV=0
) ELSE (
    SET SVN_REV=%REV_BUF:~28,-4%
)

ECHO "using rev %SVN_REV%"
CD %ORG_DIR%

REM SUBSTITUTE THE VERSION NUMBER IN TEMPLATE
SET SEARCHTEXT=ASSEMBLYVERSION

ECHO executing in directory "%ORG_DIR%"

IF EXIST %TEMPTEXTFILE% (
    ECHO "removing %TEMPTEXTFILE%"
    DEL %TEMPTEXTFILE%
)

FOR /f "tokens=1,* delims=¶" %%A IN ( '"type %INTEXTFILE%"') DO (
    SET string=%%A
    SET modified=!string:%SEARCHTEXT%=%SVN_REV%!
    ECHO !modified! >> %TEMPTEXTFILE%
)

REM COMPARE TEMP FILE WITH OUTFILE
FC /A /L %TEMPTEXTFILE% %OUTTEXTFILE%

REM IF THEY ARE IDENTICAL
IF %ERRORLEVEL% == 0 (
    DEL %TEMPTEXTFILE%
    CD %ORG_DIR%
    GOTO EOF
)
REM IF DIFFERENT
MOVE /Y %TEMPTEXTFILE% %OUTTEXTFILE%

:EOF
