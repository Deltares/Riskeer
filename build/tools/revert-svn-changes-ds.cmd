@echo off

rem TODO: update revision limit once in a while
set REVISION_LIMIT=11196

rem check if revision is entered as a first argument
if "%1"=="" (
   echo !!! BE VERY CAREFUL WITH THIS SCRIPT, THE WHOLE REPOSITORY WILL BE AFFECTED !!!

   echo ------------------------------------------
   echo Usage: "%~nx0 <revision>"
   echo ------------------------------------------
   echo "<revision>" - latest stable revision

   goto :eof
)


rem typo check
if /I %1 LSS %REVISION_LIMIT% (
   echo Revision number: %1 is too small, smaller than %REVISION_LIMIT%
   goto :eof
)

rem ask user if he's really sure what he's doing
setlocal
:PROMPT

SET /P AREYOUSURE=Are you sure you want to revert the *WHOLE* DS repository to revision %1 (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO :eof


rem do revert

echo Collecting changes for revision range %1:HEAD into revert-ds-%1.diff ...
svn diff -r%1:HEAD https://repos.deltares.nl/repos/ds/trunk/src > revert-ds-%1.diff

echo Removing trunk in remote repository ...
svn del https://repos.deltares.nl/repos/ds/trunk/src -m "Reverting to %1 ..."

echo Restoring trunk from revision %1 ...
svn copy https://repos.deltares.nl/repos/ds/trunk/src@%1 https://repos.deltares.nl/repos/ds/trunk/src@HEAD -m "Reverting to %1 ..."

echo Done! Don't forget to send an email to happy developers informing them about changes :)
echo Include zipped revert-ds-%1.diff file as an attachment.