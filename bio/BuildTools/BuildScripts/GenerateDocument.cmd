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

REM ********** Set path for sandcastle, hhc ****************************

SET SANDCASTLE_PATH=%Local_SOURCEROOT%\BuildTools\ToolSource\ExternalTools\sandcastle
SET DXROOT=%SANDCASTLE_PATH%
SET HHC_PATH=%Local_SOURCEROOT%\BuildTools\ToolSource\ExternalTools\HtmlHelpCompiler

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
Copy %Local_BINARYROOT%\Binaries\Debug\Bio.xml "%Local_TempPath%\Bio.xml"

GOTO CreateDoc

REM -- CALL Doc geneartor for Release binaries
:ReleaseDoc
IF NOT EXIST "%Local_BINARYROOT%\Binaries\Release\Bio.dll" Goto DLLNotFound
Copy %Local_BINARYROOT%\Binaries\Release\Bio.dll "%Local_TempPath%\Bio.dll"
Copy %Local_BINARYROOT%\Binaries\Release\Bio.xml "%Local_TempPath%\Bio.xml"

:CreateDoc

IF NOT EXIST "%Local_TempPath%\Bio.xml" echo "%Local_TempPath%\Bio.xml" is missing, XML comments in code will be ignored

pushd %Local_TempPath%

if exist output rmdir output /s /q
if exist chm rmdir chm /s /q

if exist Bio.xml copy /y Bio.xml comments.xml

REM ********** Call MRefBuilder ****************************

"%SANDCASTLE_PATH%\ProductionTools\MRefBuilder" Bio.dll /config:"%Local_SOURCEROOT%\BuildTools\BuildScripts\BioRefBuilder.config" /out:reflection.org 

REM ********** Apply Transforms ****************************

"%SANDCASTLE_PATH%\ProductionTools\XslTransform" /xsl:"%SANDCASTLE_PATH%\ProductionTransforms\ApplyVSDocModel.xsl" reflection.org /xsl:"%SANDCASTLE_PATH%\ProductionTransforms\AddFriendlyFilenames.xsl" /out:reflection.xml /arg:IncludeAllMembersTopic=true /arg:IncludeInheritedOverloadTopics=true

"%SANDCASTLE_PATH%\ProductionTools\XslTransform" /xsl:"%SANDCASTLE_PATH%\ProductionTransforms\ReflectionToManifest.xsl"  reflection.xml /out:manifest.xml

call "%SANDCASTLE_PATH%\Presentation\vs2005\copyOutput.bat"

REM **************Generate an intermediate Toc file that simulates the Whidbey TOC format.

"%SANDCASTLE_PATH%\ProductionTools\XslTransform" /xsl:"%SANDCASTLE_PATH%\ProductionTransforms\createvstoc.xsl" reflection.xml /out:toc.xml 

REM ********** Call BuildAssembler ****************************
"%SANDCASTLE_PATH%\ProductionTools\BuildAssembler" /config:"%SANDCASTLE_PATH%\Presentation\vs2005\configuration\sandcastle.config" manifest.xml

REM ************ Generate CHM help project ******************************

if not exist chm mkdir chm
if not exist chm\html mkdir chm\html
if not exist chm\icons mkdir chm\icons
if not exist chm\scripts mkdir chm\scripts
if not exist chm\styles mkdir chm\styles

xcopy output\icons\* chm\icons\ /y /r
xcopy output\scripts\* chm\scripts\ /y /r
xcopy output\styles\* chm\styles\ /y /r

"%SANDCASTLE_PATH%\ProductionTools\ChmBuilder.exe" /project:Bio /html:Output\html /lcid:1033 /toc:Toc.xml /config:"%Local_SOURCEROOT%\BuildTools\BuildScripts\ChmBuilder.config" /out:Chm

"%SANDCASTLE_PATH%\ProductionTools\DBCSFix.exe" /d:Chm /l:1033 

"%HHC_PATH%\hhc" chm\Bio.hhp

popd 

IF NOT EXIST "%Local_TempPath%\chm\Bio.chm" GOTO DOCERROR

IF NOT EXIST "%Local_BINARYROOT%\Binaries\docs" MD "%Local_BINARYROOT%\Binaries\docs"

Copy /Y "%Local_TempPath%\chm\Bio.chm" "%Local_BINARYROOT%\Binaries\docs\Bio.chm"

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

:END
SET Local_BINARYROOT=
SET Local_SOURCEROOT=
SET Local_TempPath=








