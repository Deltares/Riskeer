Remove-Item .\Riskeer_src_*.zip
Remove-Item .\repo-output*.zip
if ( Test-Path '.\Riskeer_src' -PathType Container ) { Remove-Item -recurse -path .\Riskeer_src}
md Riskeer_src
git archive --output ./repo-output.zip HEAD: Application build Core Demo Migration Riskeer
compress-archive -Path Riskeer_src/ -DestinationPath ('Riskeer_src_' + (get-date -Format yyyyMMdd) + '.zip')
Remove-Item .\repo-output*.zip