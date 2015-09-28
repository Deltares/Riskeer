@echo off
cd ../
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe DeltaShell.proj /t:Clean /v:m /nr:false
if exist build.log del build.log
cd build