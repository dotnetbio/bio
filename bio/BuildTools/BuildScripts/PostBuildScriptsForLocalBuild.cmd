REM -- ****************************************************************************************************
REM --     Description
REM -- ****************************************************************************************************
REM -- This script will be called by TFS on Daily build or by CreateSetup.cmd script in installer folder.
REM -- This will internally call DocForTFS.cmd , PrepareBinariesForDrop.cmd and PreWixScript.cmd.
REM -- ****************************************************************************************************

@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

echo ************************************************************
echo  Post build script Started.
echo ************************************************************

if "%1" == "" GOTO EOF
if "%2" == "" GOTO EOF

SET SOURCEROOT=%1
SET BINARYROOT=%2
SET SELECTIVE_SETUP_BUILD=%3
SET SETUP_BIO_ONLY=%4

SET CreateDOC="TRUE"
SET CopySource="TRUE"

IF EXIST ResetEnvironmentVariable.bat CALL ResetEnvironmentVariable.bat
ECHO %BINARYROOT%
ECHO %SOURCEROOT%

call %SOURCEROOT%\BuildTools\BuildScripts\PrepareBioTemplate %BINARYROOT% %SOURCEROOT%

if %CreateDOC% NEQ "TRUE" goto SkipCreateDoc
call %SOURCEROOT%\BuildTools\BuildScripts\GenerateDocument %BINARYROOT% %SOURCEROOT%

goto PrepareFolderForDrop
:SkipCreateDoc
echo skipped Doc creating

:PrepareFolderForDrop
call %SOURCEROOT%\BuildTools\BuildScripts\PrepareBinariesForDrop.cmd

SET WorkingFolder=%CD%
CD %BINARYROOT%

REM --- Call WIX pre build scripts here
CALL %SOURCEROOT%\BuildTools\BuildScripts\PreWixScript.cmd %BINARYROOT%\Binaries %CD% %SETUP_BIO_ONLY%

SET SETUPTMPFOLDER=%CD%\setup.tmp
MD %BINARYROOT%\Binaries\Installer

echo ********************
echo %SELECTIVE_SETUP_BUILD%
echo ********************

REM --- CALL Bio WIX setup here
PUSHD %SOURCEROOT%\Source\Framework\Installer
call make.cmd %SOURCEROOT% %SETUPTMPFOLDER%
REM -- Copy the Bio Installer under %BINARYROOT%\Binaries\Installer
Copy /y .\".NET Bio.msi" %BINARYROOT%\Binaries\Installer\".NET Bio.msi"
POPD

IF "%SELECTIVE_SETUP_BUILD%" == "Bio" GOTO END
IF "%SELECTIVE_SETUP_BUILD%" == "BioExcel" GOTO SETUP_BIOEXCEL

:SETUP_SEQUENCEASSEMBLER
 
REM --- CALL Tools SequenceAssembler WIX setup here
PUSHD %SOURCEROOT%\Source\Tools\Installer\BioSequenceAssembler
call make.cmd %SOURCEROOT% %SETUPTMPFOLDER%
REM -- Copy the Installers under %BINARYROOT%\Installer
Copy /y .\BioSequenceAssembler.msi %BINARYROOT%\Binaries\Installer\BioSequenceAssembler.msi
POPD

IF "%SELECTIVE_SETUP_BUILD%" == "BioSequenceAssembler" GOTO END

:SETUP_BIOEXCEL

REM --- CALL Tools BioExcel WIX setup here
PUSHD %SOURCEROOT%\Source\Tools\Installer\BioExcel
call make.cmd %SOURCEROOT% %SETUPTMPFOLDER%
REM -- Copy the Installers under %BINARYROOT%\Installer
Copy /y  .\BioExcel.msi %BINARYROOT%\Binaries\Installer\BioExcel.msi
POPD


:END
CD %WorkingFolder%

:EOF
set errorlevel=0
echo ************************************************************
echo  Post build script completed.
echo ************************************************************
POPD