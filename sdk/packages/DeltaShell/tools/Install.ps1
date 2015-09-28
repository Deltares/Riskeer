param($installPath, $toolsPath, $package, $project)

# add post-build event

$packageDir = $package.Id + '.' + $package.Version

$pluginDir = "`$(SolutionDir)packages\$packageDir\delta-shell\plugins\`$(ProjectName)"

$cmd = @"
if not exist "$pluginDir" md "$pluginDir"
copy /Y "`$(TargetPath)" "$pluginDir"
"@

if (!$project.Properties.Item("PostBuildEvent").Value.Contains($cmd)) {
    $project.Properties.Item("PostBuildEvent").Value += $cmd
}

# configure start program

$deltaShellExePath ="$installPath\delta-shell\bin\DeltaShell.Gui.exe"

foreach ($cfg in $project.ConfigurationManager)
{
    $properties = $cfg.Properties
    $properties.Item("StartAction").Value = 1 # program
    $properties.Item("StartProgram").Value = $deltaShellExePath
}
