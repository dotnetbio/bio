REM -- *************************************************************************
REM --     Description
REM -- *************************************************************************
REM -- This will be called Whenever a checkin build happens.
REM -- This will internally calls DocForTFS.cmd  and PrepareBinariesForDrop.cmd
REM -- *************************************************************************

@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

echo ************************************************************
echo  Post build script Started.
echo ************************************************************

if "%1" == "" GOTO EOF
if "%2" == "" GOTO EOF

SET SOURCEROOT=%1
SET BINARYROOT=%2

SET CreateDOC="TRUE"
SET CopySource="FALSE"

SET SilentValidation=
if "%3" == "true" SET SilentValidation="/S"

"%SOURCEROOT%\BuildTools\bin\DeveloperPreRequisiteCheck.exe" %SilentValidation%

IF EXIST ResetEnvironmentVariable.bat CALL ResetEnvironmentVariable.bat
SET OLDDIR=%CD%
PUSHD %BINARYROOT%
PUSHD ..

SET BINARYROOT=%CD%

POPD
POPD

PUSHD %OLDDIR%

call %SOURCEROOT%\BuildTools\BuildScripts\PrepareBioTemplate %BINARYROOT% %SOURCEROOT%

if %CreateDOC% NEQ "TRUE" goto SkipCreateDoc
call %SOURCEROOT%\BuildTools\BuildScripts\GenerateDocument %BINARYROOT% %SOURCEROOT%

goto PrepareFolderForDrop
:SkipCreateDoc
echo skipped Doc creating

:PrepareFolderForDrop
call %SOURCEROOT%\buildTools\BuildScripts\PrepareBinariesForDrop.cmd

:EOF

set errorlevel=0
echo ************************************************************
echo  Post build script completed.
echo ************************************************************
POPD