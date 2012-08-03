@echo off
IF EXIST ..\build\Target RD /S /Q ..\build\Binaries
SET WORKINGF=%CD%
CD ..\build
Ren Target Binaries
CD %WORKINGF%
IF EXIST ..\build\setup.tmp RD /S /Q ..\build\setup.tmp
