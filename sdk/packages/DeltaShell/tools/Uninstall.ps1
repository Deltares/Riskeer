param($installPath, $toolsPath, $package, $project)

# remove our post-build event

$packageDir = $package.Id + '.' + $package.Version

$pluginDir = "`$(SolutionDir)packages\$packageDir\delta-shell\plugins\`$(ProjectName)"

$cmd = @"
if not exist "$pluginDir" md "$pluginDir"
copy /Y "`$(TargetPath)" "$pluginDir"
"@

$project.Properties.Item("PostBuildEvent").Value = $project.Properties.Item("PostBuildEvent").Value.Replace($cmd, '')

# configure start program to default

$deltaShellExePath ="$installPath\delta-shell\bin\DeltaShell.Gui.exe"

foreach ($cfg in $project.ConfigurationManager)
{
    # TODO: check if StartProgram equals to $deltaShellExePath

    $properties = $cfg.Properties
    $properties.Item("StartAction").Value = 0 # project
    $properties.Item("StartProgram").Value = ""
}

