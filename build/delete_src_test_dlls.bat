rem ! Run this from a delta-shell checkout root !

rem ! Store current directory
pushd .

rem ! Go up to teamcity work directory !
cd .. 

rem ! Process all DeltaShell check-out directories
FOR %%D IN (DeltaShell DFlow1DValidation DRRValidation DWAQValidation Sbk32Accept) DO (

	rem ! check if directory exists
	IF EXIST "%%D" (
		
		rem ! Jump to a check-out directory
		cd %%D

		rem ! Delete all dlls from src
		cd src
		FOR /F "tokens=*" %%G IN ('DIR /B /S *.dll') DO DEL /F "%%G"
		cd ..

		rem ! Delete all dlls from test
		cd test
		FOR /F "tokens=*" %%G IN ('DIR /B /S *.dll') DO DEL /F "%%G"
		cd ..
		
		rem ! back to teamcity work dir
		cd ..
	)
)

rem ! Restore to original directory
popd