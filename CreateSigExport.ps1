Remove-Item .\Riskeer_source_*.zip
Remove-Item .\repo-output*.zip
if ( Test-Path '.\Riskeer_source' -PathType Container ) { Remove-Item -recurse -path .\Riskeer_source}
md Riskeer_source
git archive --output ./repo-output.zip HEAD: Application Core Demo Migration Riskeer
git submodule foreach --recursive 'git archive --verbose --format zip master --prefix=Shared/ --output ../repo-output-sub-$sha1.zip'
dir ./ | where { $_.Extension -eq ".zip" } | foreach { expand-archive $_.FullName Riskeer_source/ }
compress-archive -Path Riskeer_source/ -DestinationPath ('Riskeer_source_' + (get-date -Format yyyyMMdd) + '.zip')
Remove-Item .\repo-output*.zip