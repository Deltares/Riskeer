cd ..\

rem @echo off

echo | .\build\tools\gawk '{ print systime(); }' > %TEMP%\time_before_build

rem uncomment to enable MSBuild cache file 
rem set MSBuildEmitSolution=1

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe DeltaShell.sln /t:src\DeltaShell\DeltaShell_Gui /p:Configuration=Debug /m /nr:true /v:m

rem /l:MSBuildProfileLogger,.\build\tools\MSBuildProfiler\MSBuildProfiler.dll 
rem /fileLoggerParameters:LogFile=build-debug.log;Verbosity=diagnostic;Encoding=UTF-8 

echo | .\build\tools\gawk '{ time = systime(); print "Build finished in : " time - $1 " sec"; }' %TEMP%\time_before_build
del %TEMP%\time_before_build
