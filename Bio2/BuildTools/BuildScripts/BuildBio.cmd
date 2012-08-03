@echo off
REM -- ********************************************************************************
REM --     Description
REM -- ********************************************************************************
REM -- Builds the MBI solution locally.
REM -- ********************************************************************************


SET OUTPATH=%1
SET SOURCEPATH=%2

IF EXIST "%OUTPATH%" (
rmdir /s /q "%OUTPATH%" 
) ELSE ( 
mkdir "%OUTPATH%" )

mkdir "%OUTPATH%\Binaries"

SET BPATH="%OUTPATH%\Binaries"

REM Get Absolute paths.

pushd %SOURCEPATH%
SET SOURCEPATH=%CD%
popd

pushd %BPATH%
SET BPATH=%CD%
popd

SET MSBUILDPATH=%windir%\Microsoft.NET\Framework\v4.0.30319

%MSBUILDPATH%\msbuild.exe %SOURCEPATH%\Bio.sln /p:Configuration=Debug;OutDir=%BPATH%\Debug\
IF %ERRORLEVEL% NEQ 0 GOTO BUILDERROR

%MSBUILDPATH%\msbuild.exe %SOURCEPATH%\Bio.sln /p:Configuration=Release;OutDir=%BPATH%\Release\
IF %ERRORLEVEL% NEQ 0 GOTO BUILDERROR

SET SOURCEPATH=
SET BPATH=
SET OUTPATH=

GOTO END

:BUILDERROR
echo -----------------------------------------------------------------------------
echo Build error occurred.
echo Script terminated.
echo -----------------------------------------------------------------------------

:END

