@echo off
echo Preparing report...
FOR /F %%G IN ('dir /b *.rxzlog') DO set _rxzlog=%%G

ren %_rxzlog% zippedReport.zip
md unzipped
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('zippedReport.zip', 'unzipped'); }"
FOR /F %%G IN ('dir /b /ad unzipped\*images*') DO set _d1=%%G
md %_d1%
xcopy unzipped\%_d1% %_d1% >xcopy.log
FOR /F %%G IN ('dir /b /ad unzipped\*videos*') DO set _d2=%%G
md %_d2%
xcopy unzipped\%_d2% %_d2% >xcopy.log
rd /s /q unzipped
del xcopy.log

ren zippedReport.zip %_rxzlog%
FOR /F "tokens=1,2,3,4 delims=." %%i IN ('dir /b *.xml') DO ren "%%i.%%j.%%k.%%l" "%%i.html.%%k.%%l"
FOR /F "tokens=1,2,3 delims=." %%i IN ('dir /b *.data') DO ren "%%i.%%j.%%k" "%%i.html.%%k"
FOR /F "tokens=1,2 delims=." %%i IN ('dir /b *.rxlog') DO ren "%%i.%%j" "%%i.html"
