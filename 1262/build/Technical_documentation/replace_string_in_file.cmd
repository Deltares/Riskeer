REM Replaces text by text from subject to outfile
@ECHO off

IF "%~1" NEQ "" IF "%~2" NEQ "" IF "%~3" NEQ "" (
	SET SEARCH=%1
	SET REPLACE=%2
	SET SUBJECT=%3
	IF "%~4" NEQ ""  (
		SET OUTFILE=%4
	) ELSE (
		SET OUTFILE=%3
	)	
) ELSE (
	ECHO "invalid arguments: [search] [replace] [subject] ([outfile])"
	GOTO EOF
)

REM Cleanup arguments
SET SEARCH=%SEARCH:"=%
SET REPLACE=%REPLACE:"=%
SET SUBJECT=%SUBJECT:"=%
SET OUTFILE=%OUTFILE:"=%

SET TEMPOUTFILE=%OUTFILE%.temp

ECHO Replace "%SEARCH%" with "%REPLACE%" in "%SUBJECT%", save as "%OUTFILE%"


IF NOT EXIST %SUBJECT% (
    ECHO File does not exist "%SUBJECT%"
    GOTO EOF
)

REM Remove outfile
IF %SUBJECT% NEQ %OUTFILE% IF EXIST %OUTFILE% (
	ECHO removing "%OUTFILE%"
	DEL %OUTFILE%
)

REM Replace Text
FOR /f "tokens=1,* delims=¶" %%A IN ( '"type %SUBJECT%"') DO (
	SET string=%%A
	setlocal enabledelayedexpansion
    SET modified=!string:%SEARCH%=%REPLACE%!
    ECHO !modified! >> %TEMPOUTFILE%
	endlocal
)

MOVE /Y %TEMPOUTFILE% %OUTFILE%

:EOF
