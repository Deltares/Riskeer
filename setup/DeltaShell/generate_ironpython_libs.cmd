rem Use this script to generate WIX script for IronPython libs, then:
rem    1. Copy/past all Components to ScriptingPlugin.wxi inside the DeltaShell.Plugins.Scripting dir. Make sure it includes DeltaShell.Scripting.dll
rem    2. Copy/paste all ComponentRefs into the PythonLibraries Feature in Feature_PythonLibraries.wxi
..\..\build\tools\Paraffin.exe -dir ..\..\lib\DeltaShell\DeltaShell.Plugins.Scripting\ -guids -custom IronPythonLibs IronPythonLibs.wxs