Remove-Item .\Riskeer_src_*.zip
Remove-Item .\repo-output*.zip
if ( Test-Path '.\Riskeer_src' -PathType Container ) { Remove-Item -recurse -path .\Riskeer_src}
md Riskeer_src
git archive --output ./repo-output.zip HEAD: Application Core Demo Migration Riskeer
git submodule foreach --recursive 'git archive --verbose --format zip master --prefix=Shared/ --output ../repo-output-sub-$sha1.zip'
dir ./ | where { $_.Extension -eq ".zip" } | foreach { expand-archive $_.FullName Riskeer_src/ }
compress-archive -Path Riskeer_src/ -DestinationPath ('Riskeer_src_' + (get-date -Format yyyyMMdd) + '.zip')
Remove-Item .\repo-output*.zip