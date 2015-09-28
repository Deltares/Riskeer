svn st -q |cut -c9- | grep -v 'status on extern'| grep -v 'Externals' > changes.txt

java -jar build\tools\tcc.jar run --host https://build.deltares.nl -m "%1" -c "bt565,bt55,bt563,bt75,bt567,bt53,bt9,bt564,bt77" @changes.txt


rem C:\opt\bin\tcc.cmd run -m "%1" -c "bt55,bt75,bt53,bt313,bt9,bt77" @changes.txt
rem tcc run -m "%1" -c "bt504" @changes.txt
rem tcc run -m "%1" -c "bt55,bt75,bt53,bt313,bt9,bt77,bt504" @changes.txt


rem bt55, bt75, bt53, bt313, bt9, bt77

rem bt55 - Data Access 
rem bt75 - Integration
rem bt53 - Performance
rem bt313 - Slow
rem bt9 - UnitTest
rem bt77 - WindowsForms
rem bt504 - All + Coverage
