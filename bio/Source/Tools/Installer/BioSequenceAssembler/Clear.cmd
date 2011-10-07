@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

REM if target folder exists then remove binaries folder and rename the target back to binaries

IF EXIST ..\..\build\Target RD /S /Q ..\..\build\Binaries
SET WORKINGF=%CD%
CD ..\..\build
Ren Target Binaries
CD %WORKINGF%
IF EXIST ..\..\build\setup.tmp RD /S /Q ..\..\build\setup.tmp
IF EXIST ..\..\build\binaries\debug\Doctemp RD /S /Q ..\..\build\binaries\debug\Doctemp
IF EXIST ..\..\build\binaries\release\Doctemp RD /S /Q ..\..\build\binaries\release\Doctemp
