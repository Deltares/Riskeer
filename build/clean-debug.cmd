@echo off
cd ../
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe DeltaShell.proj /t:CleanDebug /m /nr:true /v:m
if exist build-debug.log del build-debug.log
cd build
