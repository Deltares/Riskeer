Remove-Item .\Riskeer_src_*.zip -ErrorAction Ignore
Remove-Item .\repo-output*.zip -ErrorAction Ignore
if ( Test-Path '.\Riskeer_src' -PathType Container ) { Remove-Item -recurse -path .\Riskeer_src -ErrorAction Ignore}
mkdir Riskeer_src
git archive --output ./repo-output.zip HEAD: Application build/DatabaseStructure.sql Core Migration Riskeer
Get-ChildItem ./ | Where-Object { $_.Extension -eq ".zip" } | ForEach-Object { expand-archive $_.FullName Riskeer_src/ }
compress-archive -Path Riskeer_src/ -DestinationPath ('Riskeer_src_' + (get-date -Format yyyyMMdd) + '.zip')
Remove-Item .\repo-output*.zip