param($installPath, $toolsPath, $package, $project)

# add post-build event

$cmd = @"
copy /Y $installPath\lib\native\netcdf-4.3.dll $(TargetDir)
"@

if (!$project.Properties.Item("PostBuildEvent").Value.Contains($cmd)) {
    $project.Properties.Item("PostBuildEvent").Value += $cmd
}
