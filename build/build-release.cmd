cd ..

echo | .\build\tools\gawk '{ print systime(); }' > %TEMP%\time_before_build

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe DeltaShell.proj /m /property:StartUsingCommandLine=true /t:Build /v:q

rem /fileLoggerParameters:LogFile=build-debug.log;Verbosity=diagnostic;Encoding=UTF-8 

echo | .\build\tools\gawk '{ time = systime(); print "Build finished in : " time - $1 " sec"; }' %TEMP%\time_before_build
del %TEMP%\time_before_build