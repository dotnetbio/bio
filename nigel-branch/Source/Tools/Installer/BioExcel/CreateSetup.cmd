@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

echo ************************************************************
echo Creating BioExcel Setup Locally - Start
echo ************************************************************

PUSHD ..\..\..\..
SET MBIROOT=%CD%
POPD

set BINARYPATH=%MBIROOT%\Build\LocalBuild

%MBIROOT%\Buildtools\Bin\DeveloperPreRequisiteCheck.exe
IF %ERRORLEVEL% NEQ 0 GOTO PREREQERROR

call %MBIROOT%\BuildTools\BuildScripts\BuildBio.cmd %MBIROOT%\Build\LocalBuild %MBIROOT%
IF %ERRORLEVEL% NEQ 0 GOTO END

:CHECK
PUSHD %BINARYPATH%\Release

if exist *.vshost.exe (
del *.vshost.exe )

if exist *.vshost.exe (
echo **************************************************************
echo ERROR: A host process is holding certain required resources in the release binaries folder.
echo Please close the .NET Bio\Bio solution before proceeding.
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
echo Please close the .NET Bio\Bio solution before proceeding.
echo **************************************************************
pause 
POPD
GOTO CHECK ) else ( POPD )

set errorlevel=0

Echo %MBIROOT%
Echo %BINARYPATH%

CALL %MBIROOT%\BuildTools\BuildScripts\PostBuildScriptsForDailyBuild.cmd %MBIROOT% %BINARYPATH% BioExcel

echo ************************************************************
echo Creating BioExcel Setup Locally - End
echo ************************************************************

GOTO END

:PREREQERROR
echo -----------------------------------------------------------------------------
echo Please install the missing prerequisite(s) and run this script again.
echo -----------------------------------------------------------------------------

:END