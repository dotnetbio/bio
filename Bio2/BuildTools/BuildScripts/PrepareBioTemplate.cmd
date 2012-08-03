@Echo Off
REM -- ***************************************************************************************************************************
REM --     Description
REM -- ***************************************************************************************************************************
REM -- Prepares BioConsoleApplicationTemplate.zip and copies it to Debug and Release folders.
REM -- ***************************************************************************************************************************


SET Local_BINARYROOT=%BINARYROOT%
SET Local_SOURCEROOT=%SOURCEROOT%

IF NOT "%1"=="" SET Local_BINARYROOT=%1
IF NOT "%2"=="" SET Local_SOURCEROOT=%2

IF "%Local_BINARYROOT%"=="" goto EnErr
IF "%Local_SOURCEROOT%"=="" goto EnErr

Pushd %Local_BINARYROOT%

IF EXIST .\Binaries\Debug\BioConsoleApplicationTemplate.zip del .\Binaries\Debug\BioConsoleApplicationTemplate.zip
IF EXIST .\Binaries\Release\BioConsoleApplicationTemplate.zip del .\Binaries\Release\BioConsoleApplicationTemplate.zip

%Local_SOURCEROOT%\BuildTools\Bin\DotNetZipConsole .\Binaries\Debug\BioConsoleApplicationTemplate.zip %Local_SOURCEROOT%\Source\Tools\Bio.TemplateWizard\BioConsoleApplicationTemplate
xCopy /i /s /y .\Binaries\Debug\BioConsoleApplicationTemplate*.zip .\Binaries\Release\BioConsoleApplicationTemplate*.zip

Popd
Goto END

:EnErr
Echo "Environment variables are not found"

:END
SET Local_BINARYROOT=
SET Local_SOURCEROOT=

