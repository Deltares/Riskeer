rem @echo off

rem TODO: merge it into DelftTools.target

rem                                                            %1               %2              %3            %4            %5            %6
rem call "$(SolutionDir)\build\tools\PluginPostBuild.cmd" "$(SolutionDir)" "$(TargetDir)" $(ProjectName) $(PluginName) $(InstallDir) $(ProjectDir)
echo Runnning PostBuild event script for plugin project: %3

set SOLUTION_DIR=%1
set TARGET_DIR=%2
set PROJECT_NAME=%3
set INSTALL_DIR=%5
set PROJECT_DIR=%6
set PLUGIN_NAME=%4

set PLUGIN_HOME=%INSTALL_DIR%\plugins\%PLUGIN_NAME%

if not exist %INSTALL_DIR%\skip-plugins.txt (
  echo "# add plugin names below to skip copying them during compilation" > %INSTALL_DIR%\skip-plugins.txt
)

%SOLUTION_DIR%\build\tools\grep.exe %PLUGIN_NAME% %INSTALL_DIR%\skip-plugins.txt
if %ERRORLEVEL% == 0 (
  echo Skipping plugin: %PLUGIN_NAME% ...
  exit 0
)

echo Install dir: %INSTALL_DIR%
echo Plugin home: %PLUGIN_HOME%

if not exist %PLUGIN_HOME% md %PLUGIN_HOME%

if exist %TARGET_DIR%\%PROJECT_NAME%.dll (  
  %windir%\System32\xcopy.exe /D /Y /Q %TARGET_DIR%%PROJECT_NAME%.dll %PLUGIN_HOME% > nul
)
if exist %TARGET_DIR%\%PROJECT_NAME%.exe (
  %windir%\System32\xcopy.exe /D /Y /Q %TARGET_DIR%%PROJECT_NAME%.exe %PLUGIN_HOME% > nul
)
if exist %TARGET_DIR%\%PROJECT_NAME%.dll.config (
  %windir%\System32\xcopy.exe /D /Y /Q %TARGET_DIR%%PROJECT_NAME%.dll.config %PLUGIN_HOME% > nul
  echo Copied plugin config file %TARGET_DIR%%PROJECT_NAME%.dll.config to %PLUGIN_HOME% 
) else (
  echo "Plugin config file (%TARGET_DIR%%PROJECT_NAME%.dll.config) does not exist, skipping copy."
)

rem copying plugin language-specific resource files
rem ============================================================
rem hardcoded for now, make it generic when necessary (ADD MORE LANGUAGES HERE)

if exist %TARGET_DIR%\%PROJECT_NAME%.dll (  
  %windir%\System32\xcopy.exe /D /Y /Q %TARGET_DIR%%PROJECT_NAME%.dll %PLUGIN_HOME% > nul
)

if exist %TARGET_DIR%\nl-NL (
  if exist %TARGET_DIR%\nl-NL\%PROJECT_NAME%.resources.dll (
    if not exist %PLUGIN_HOME%\nl-NL md %PLUGIN_HOME%\nl-NL
    %windir%\System32\xcopy.exe /D /Y /Q %TARGET_DIR%\nl-NL\%PROJECT_NAME%.resources.dll %PLUGIN_HOME%\nl-NL > nul
  )
)


if exist %TARGET_DIR%\ru-RU (
  if exist %TARGET_DIR%\ru-RU\%PROJECT_NAME%.resources.dll (
    if not exist %PLUGIN_HOME%\ru-RU md %PLUGIN_HOME%\ru-RU
    %windir%\System32\xcopy.exe /D /Y /Q %TARGET_DIR%\ru-RU\%PROJECT_NAME%.resources.dll %PLUGIN_HOME%\ru-RU > nul
  )
)

if exist %TARGET_DIR%\Data (
  if not exist %PLUGIN_HOME%\Data md %PLUGIN_HOME%\Data
  %windir%\System32\xcopy.exe /D /Y /S /Q %TARGET_DIR%\Data %PLUGIN_HOME%\Data > nul
)

rem ===============================================================



rem copying native libraries to loader dir

rem detect relative path to the project
for %%a in (%PROJECT_DIR%\..) do set PLUGIN_TYPE_DIR=%%~nxa

if "%PLUGIN_TYPE_DIR%" == "DeltaShell" (
    set NATIVE_RESOURCES_DIR=%SOLUTION_DIR%\lib\DeltaShell\%PROJECT_NAME%
) else (
    set NATIVE_RESOURCES_DIR=%SOLUTION_DIR%\lib\Plugins\%PLUGIN_TYPE_DIR%\%PROJECT_NAME%
)

echo Copying plugin native resources from: %NATIVE_RESOURCES_DIR% to %PLUGIN_HOME% ...

if exist %NATIVE_RESOURCES_DIR% (  
  %windir%\System32\xcopy.exe /E /D /Y /Q %NATIVE_RESOURCES_DIR%\* %PLUGIN_HOME% > nul
)


