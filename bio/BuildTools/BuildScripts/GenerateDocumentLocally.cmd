@Echo off
REM -- **************************************************************************
REM --     Description
REM -- **************************************************************************
REM -- Generates chm file for Bio.dll locally.
REM -- Output can be found in "Bio\Build\LocalBuild\Binaries\Docs" folder.
REM -- **************************************************************************

SET BioPath=..\..\
SET BuildPath=..\..\Build\LocalBuild

REM Get absolute path.

pushd "%BioPath%"
SET BioPath=%CD%
popd

IF EXIST "%BuildPath%" (
rmdir /s /q "%BuildPath%"
)

MKDIR "%BuildPath%"

pushd "%BuildPath%"
SET BuildPath=%CD%
popd


"%BioPath%\BuildTools\Bin\DeveloperPreRequisiteCheck.exe"

IF EXIST ResetEnvironmentVariable.bat CALL ResetEnvironmentVariable.bat

CALL "%BioPath%\BuildTools\BuildScripts\BuildBio.cmd" %BuildPath% %BioPath% 

CALL "%BioPath%\BuildTools\BuildScripts\GenerateDocument.cmd" %BuildPath% %BioPath%

SET BioPath=
SET BuildPath=
