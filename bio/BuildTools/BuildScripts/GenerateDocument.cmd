@echo off
REM -- ****************************************
REM --     Description
REM -- ****************************************
REM -- Generates chm file for Bio.dll. 
REM -- ****************************************

SET Local_BINARYROOT=%BINARYROOT%
SET Local_SOURCEROOT=%SOURCEROOT%

IF NOT "%1"=="" SET Local_BINARYROOT=%1
IF NOT "%2"=="" SET Local_SOURCEROOT=%2

IF "%Local_BINARYROOT%"=="" goto EnErr
IF "%Local_SOURCEROOT%"=="" goto EnErr

REM ********** Create a temporary folder for chm creation ****************************

SET Local_TempPath=%Local_BINARYROOT%\tmp
IF EXIST "%Local_TempPath%\Nul" rmdir /s /q "%Local_TempPath%"
MD "%Local_TempPath%"

echo ************************************************************
echo  Generating class documentation...
echo ************************************************************

REM Try to generate doc from debug if not possible, try to generate it from release binaries.

REM -- CALL Doc generator for Debug binaries
IF NOT EXIST "%Local_BINARYROOT%\Binaries\Debug\Bio.dll" GOTO ReleaseDoc

Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.dll" "%Local_TempPath%\Bio.dll"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.xml" "%Local_TempPath%\Bio.xml"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.IO.dll" "%Local_TempPath%\Bio.IO.dll"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.IO.xml" "%Local_TempPath%\Bio.IO.xml"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.Comparative.dll" "%Local_TempPath%\Bio.Comparative.dll"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.Comparative.xml" "%Local_TempPath%\Bio.Comparative.xml"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.Padena.dll" "%Local_TempPath%\Bio.Padena.dll"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.Padena.xml" "%Local_TempPath%\Bio.Padena.xml"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.Pamsam.dll" "%Local_TempPath%\Bio.Pamsam.dll"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.Pamsam.xml" "%Local_TempPath%\Bio.Pamsam.xml"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.WebServiceHandlers.dll" "%Local_TempPath%\Bio.WebServiceHandlers.dll"
Copy "%Local_BINARYROOT%\Binaries\Debug\Bio.WebServiceHandlers.xml" "%Local_TempPath%\Bio.WebServiceHandlers.xml"

GOTO CreateDoc

REM -- CALL Doc geneartor for Release binaries
:ReleaseDoc
IF NOT EXIST "%Local_BINARYROOT%\Binaries\Release\Bio.dll" Goto DLLNotFound
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.dll" "%Local_TempPath%\Bio.dll"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.xml" "%Local_TempPath%\Bio.xml"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.IO.dll" "%Local_TempPath%\Bio.IO.dll"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.IO.xml" "%Local_TempPath%\Bio.IO.xml"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.Comparative.dll" "%Local_TempPath%\Bio.Comparative.dll"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.Comparative.xml" "%Local_TempPath%\Bio.Comparative.xml"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.Padena.dll" "%Local_TempPath%\Bio.Padena.dll"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.Padena.xml" "%Local_TempPath%\Bio.Padena.xml"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.Pamsam.dll" "%Local_TempPath%\Bio.Pamsam.dll"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.Pamsam.xml" "%Local_TempPath%\Bio.Pamsam.xml"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.WebServiceHandlers.dll" "%Local_TempPath%\Bio.WebServiceHandlers.dll"
Copy "%Local_BINARYROOT%\Binaries\Release\Bio.WebServiceHandlers.xml" "%Local_TempPath%\Bio.WebServiceHandlers.xml"

:CreateDoc

pushd %Local_TempPath%

if exist output rmdir output /s /q
if exist chm rmdir chm /s /q

msbuild "%Local_BINARYROOT%\..\..\BuildTools\BuildScripts\BioChm.shfbproj"

popd 

IF NOT EXIST "%Local_TempPath%\chm\Documentation.chm" GOTO DOCERROR

IF NOT EXIST "%Local_BINARYROOT%\Binaries\docs" MD "%Local_BINARYROOT%\Binaries\docs"

Copy /Y "%Local_TempPath%\chm\Documentation.chm" "%Local_BINARYROOT%\Binaries\docs\Bio.chm"

rmdir /s /q "%Local_TempPath%"

Goto END

REM Environment error.
:EnErr 
Echo Environment variables not found.
Goto DOCERROR

:DLLNotFound
Echo "Bio.dll not found"

:DOCERROR
ECHO Failed to generate Bio SDK Documentation 
PAUSE

:END
SET Local_BINARYROOT=
SET Local_SOURCEROOT=
SET Local_TempPath=








