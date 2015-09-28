param($installPath, $toolsPath, $package, $project)

# remove our post-build event

$cmd = @"
copy /Y $installPath\lib\native\netcdf-4.3.dll $(TargetDir)
"@

$project.Properties.Item("PostBuildEvent").Value = $project.Properties.Item("PostBuildEvent").Value.Replace($cmd, '')

