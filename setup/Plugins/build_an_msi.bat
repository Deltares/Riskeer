@echo off
set /p pluginName="Enter plugin name: " %=%
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %pluginName%\%pluginName%.wixproj /t:Rebuild /p:SolutionDir=..\..\..\
PAUSE