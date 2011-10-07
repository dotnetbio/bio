@echo OFF

SET SOURCEROOT=%1
IF "%SOURCEROOT%" == "" GOTO SOURCEROOTMISSING

SET SETUPTMPFOLDER=%2
IF "%SETUPTMPFOLDER%" == "" GOTO SETUPTMPFOLDERMISSING

REM ** Excel Addin **

REM ** Note: current directory is Bio\Source\MBT\Installer\BioExcel **

echo ************************************************************
echo  .NET Bio MSI creation started
echo ************************************************************

REM ** Set product version number (this is used internally by Wix)**
SET ProductMajorVersion=1
SET ProductMinorVersion=0
SET ProductBuildVersion=0
SET ProductVersion=%ProductMajorVersion%.%ProductMinorVersion%

SET SETUP=%CD%
SET COMMONFOLDER=%SETUP%\..\Common

SET UI=%SETUP%\UI
SET BITMAPS=%SETUP%\Bitmaps

SET WIXTOOLS=%SOURCEROOT%\BuildTools\ToolSource\ExternalTools\wix3

REM ** temporary output folders **
SET OUTDIR=%SETUP%\OUTPUT
SET TEMP=%SETUP%\TEMP

IF EXIST %TEMP% rmdir /S /Q %TEMP%
mkdir %TEMP%
IF EXIST %OUTDIR% rmdir /S /Q %OUTDIR%
mkdir %OUTDIR%

REM ** delete existing installer **
IF EXIST BioExcel.msi del /F BioExcel.msi

echo --- Copying readme.txt file to temporary output folder ---
xcopy /y /q %SETUP%\source\readme.txt %OUTDIR%

echo --- Copying Bitmaps to temporary output folder ---
IF NOT EXIST %OUTDIR%\Bitmaps mkdir %OUTDIR%\Bitmaps
xcopy /E /H /R /C /Y /Q %BITMAPS%\*.* %OUTDIR%\Bitmaps\*.*

echo --- Copying license to the output folder ---
xcopy /y /Q %SETUP%\License.rtf %OUTDIR%

echo --- Copying source files to output folder ---
xcopy /y /q /e "%SETUPTMPFOLDER%\Microsoft Biology Tools\Excel Biology Extension" %OUTDIR%

echo --- Copying Framework merge module to output folder ---
xcopy /y /q %COMMONFOLDER%\MergeModule.msm %OUTDIR%\

echo --- Harvesting source folders ---
CALL "%WIXTOOLS%\heat" dir "%SETUPTMPFOLDER%\Microsoft Biology Tools\Excel Biology Extension" -srd -sw5151 -dr EXCELADDINFOLDER -gg -g1 -nologo -cg ExcelAddinComponentGroup -sfrag -template:fragment -ke -out %SETUP%\source\ExcelAddinComponents.wxs

PUSHD %UI%

echo --- Compiling UI dialogs ---
CALL "%WIXTOOLS%\candle" -nologo -out %TEMP%\ BrowseDlg.wxs Common.wxs CommonFonts.wxs DiskCostDlg.wxs ExcelAddinUI.wxs CustomSetupTypeDlg.wxs ErrorDlg.wxs ErrorProgressText.wxs ExitDialog.wxs InstallDirDlg.wxs LicenseAgreementDlg.wxs MaintenanceTypeDlg.wxs MaintenanceWelcomeDlg.wxs MsiRMFilesInUse.wxs PrepareDlg.wxs ProgressDlg.wxs ResumeDlg.wxs UserExit.wxs VerifyReadyDlg.wxs WelcomeDlg.wxs CancelDlg.wxs FatalError.wxs OutOfDiskDlg.wxs OutOfRbDiskDlg.wxs WaitForCostingDlg.wxs CustomizeDlg.wxs ExcelPrerequisiteDeterminationDlg.wxs
POPD

PUSHD %TEMP%

REM ** Link UI dialogs **
CALL "%WIXTOOLS%\lit" -nologo -out %OUTDIR%\ExcelAddinUILib.wixlib BrowseDlg.wixobj Common.wixobj CommonFonts.wixobj DiskCostDlg.wixobj ExcelAddinUI.wixobj ErrorDlg.wixobj ErrorProgressText.wixobj ExitDialog.wixobj InstallDirDlg.wixobj LicenseAgreementDlg.wixobj MaintenanceTypeDlg.wixobj MaintenanceWelcomeDlg.wixobj MsiRMFilesInUse.wixobj PrepareDlg.wixobj ProgressDlg.wixobj ResumeDlg.wixobj UserExit.wixobj VerifyReadyDlg.wixobj WelcomeDlg.wixobj  CancelDlg.wixobj FatalError.wixobj OutOfDiskDlg.wixobj OutOfRbDiskDlg.wixobj WaitForCostingDlg.wixobj CustomSetupTypeDlg.wixobj CustomizeDlg.wixobj ExcelPrerequisiteDeterminationDlg.wixobj
POPD

PUSHD %SETUP%\source

echo --- Compiling core installer files ---
CALL "%WIXTOOLS%\candle" -nologo -out %OUTDIR%\ ExcelAddin.wxs ExcelAddinComponents.wxs

POPD

PUSHD %OUTDIR%

REM ** Linking installer object files and UI library **
REM ** referencing localization file and external assemblies **
CALL "%WIXTOOLS%\light" -nologo -sice:ICE05 -sw1076 -out %SETUP%\BioExcel.msi ExcelAddin.wixobj ExcelAddinComponents.wixobj ExcelAddinUILib.wixlib -loc %UI%\ExcelAddin_WixUI_en-us.wxl -ext "%WIXTOOLS%\WixUIExtension.dll" -ext "%WIXTOOLS%\WixUtilExtension.dll"

POPD

REM ** Clean up temporary files ** 
IF EXIST %TEMP% RMDIR /S /Q %TEMP%
IF EXIST %OUTDIR% RMDIR /S /Q %OUTDIR%
IF EXIST %SETUP%\*.wixpdb DEL /Q %SETUP%\*.wixpdb
IF EXIST %SETUP%\source\ExcelAddinComponents.wxs DEL /Q %SETUP%\source\ExcelAddinComponents.wxs

echo ************************************************************
echo  BioExcel MSI Setup creation complete
echo ************************************************************

GOTO EOF

:SOURCEROOTMISSING
echo Error: Source root path not specified.

:SETUPTMPFOLDERMISSING
echo Error: Setup folder not specified.

:EOF