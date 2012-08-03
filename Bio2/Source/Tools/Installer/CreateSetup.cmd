@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

echo ************************************************************
echo Creating MBT Setups Locally - Start
echo ************************************************************

PUSHD ..\..\..
SET BioROOT=%CD%
POPD

set BINARYPATH=%BioROOT%\Build\LocalBuild

%BioROOT%\Buildtools\Bin\DeveloperPreRequisiteCheck.exe
IF %ERRORLEVEL% NEQ 0 GOTO PREREQERROR

call %BioROOT%\BuildTools\BuildScripts\BuildBio.cmd %BioROOT%\Build\LocalBuild %BioROOT%
IF %ERRORLEVEL% NEQ 0 GOTO END

:CHECK
PUSHD %BINARYPATH%\Release

if exist *.vshost.exe (
del *.vshost.exe )

if exist *.vshost.exe (
echo **************************************************************
echo ERROR: A host process is holding certain required resources in the release binaries folder.
echo Please close the Bio solution before proceeding.
echo **************************************************************
pause 
POPD
GOTO CHECK ) else ( POPD )

set errorlevel=0

PUSHD %BINARYPATH%\Debug

if exist *.vshost.exe (
del *.vshost.exe )

if exist *.vshost.exe (
echo **************************************************************
echo ERROR: A host process is holding certain required resources in the debug binaries folder.
echo Please close the Bio solution before proceeding.
echo **************************************************************
pause 
POPD
GOTO CHECK ) else ( POPD )

set errorlevel=0

Echo %BioROOT%
Echo %BINARYPATH%

CALL %BioROOT%\BuildTools\BuildScripts\PostBuildScriptsForLocalBuild.cmd %BioROOT% %BINARYPATH% true false

echo ************************************************************
echo Creating MBT Setups Locally - End
echo ************************************************************
GOTO END

:PREREQERROR
echo -----------------------------------------------------------------------------
echo Please install the missing prerequisite(s) and run this script again.
echo -----------------------------------------------------------------------------

:END