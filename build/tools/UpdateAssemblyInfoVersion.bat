@echo off
rem Usage: %0 <derectory> <file (AssemblyInfo.cs)>


IF DEFINED BUILD_NUMBER (
	set version=%BUILD_NUMBER%
	goto update_assembly
) else if not exist "%1/.svn/entries" (
	set version=0
	goto update_assembly
) 

svn info "%1" | gawk '{ if(match($0, /Revision/)) { printf("%%s", substr($0, 11)) } }' | sed "s/exported/0/" | sed "s/[^:]*://" | sed "s/\([0-9]*\)[A-Z]/\1/" | sed "s/\(.*\)/set version=\1/" > setversion_.bat
call setversion_.bat & del setversion_.bat > NUL

:update_assembly
sed s/Version(\"\([0-9]*\).\([0-9]*\).\([0-9]*\)[^)]*)/Version(\"\1.\2.\3.%version%\")/g %2
