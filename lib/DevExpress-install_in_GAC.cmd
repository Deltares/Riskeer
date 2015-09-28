@echo off
for %%f in (.\DevExpress*.dll) do "..\build\tools\gacutil.exe" -I %%f
rem for %%f in (DevExpress*.dll) do "C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\gacutil.exe" -U %%f
