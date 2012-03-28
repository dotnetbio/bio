@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

SET SOURCEROOT=%1
IF "%SOURCEROOT%" == "" GOTO SOURCEROOTMISSING

SET SETUPTMPFOLDER=%2
IF "%SETUPTMPFOLDER%" == "" GOTO SETUPTMPFOLDERMISSING

REM ** Note: current directory is Bio\Source\Framework\Installer**

echo ************************************************************
echo  .NET Bio msi creation started
echo ************************************************************

REM ** Set product version number (this is used internally by Wix)**
SET ProductMajorVersion=1
SET ProductMinorVersion=01
SET ProductBuildVersion=0
SET ProductVersion=%ProductMajorVersion%.%ProductMinorVersion%

PUSHD ..\..
SET BioSOURCE=%CD%
POPD
SET SETUP=%BioSOURCE%\Framework\Installer
SET UI=%BioSOURCE%\Framework\Installer\UI
SET BITMAPS=%BioSOURCE%\Framework\Installer\Bitmaps

SET WIXTOOLS=%SOURCEROOT%\BuildTools\ToolSource\ExternalTools\wix3

REM ** set temporary output folders **
SET OUTDIR=%BioSOURCE%\Framework\Installer\OUTPUT
SET TEMP=%BioSOURCE%\Framework\Installer\TEMP

REM ** create temporary output folders **
IF EXIST %TEMP% rmdir /S /Q %TEMP%
mkdir %TEMP%
IF EXIST %OUTDIR% rmdir /S /Q %OUTDIR%
mkdir %OUTDIR%

REM ** delete existing installer **
IF EXIST ".NET Bio.msi" del /F ".NET Bio.msi"

echo --- Copying readme.txt file to temporary output folder ---
xcopy /y /q %SETUP%\source\readme.txt %OUTDIR%

echo --- Harvesting source folders ---
CALL "%WIXTOOLS%\heat" dir "%SETUPTMPFOLDER%\.NET Bio\Framework" -srd -dr FRAMEWORKFOLDER -gg -g1 -nologo -cg FrameworkComponentGroup -sfrag -template:fragment -ke -out %SETUP%\source\FrameworkComponents.wxs
CALL "%WIXTOOLS%\heat" dir "%SETUPTMPFOLDER%\.NET Bio\SDK" -srd -dr SDKFolder -gg -g1 -nologo -cg SDKComponentGroup -sfrag -template:fragment -ke -out %SETUP%\source\SDKComponents.wxs
CALL "%WIXTOOLS%\heat" dir "%SETUPTMPFOLDER%\.NET Bio\BioTemplateWizard" -srd -dr dir1D48513452BE58C4C0FC75C9F711E37E -gg -g1 -nologo -cg BioTemplateGroup -sfrag -template:fragment -ke -out %SETUP%\source\BioTemplateComponents.wxs

echo --- Adding Framework components to merge module source file ---
PUSHD %SETUP%\source
Attrib -r MergeModule.wxs
%SETUP%\MergeWxs FrameworkComponents.wxs MergeModule.wxs
Attrib +r MergeModule.wxs
POPD

echo --- Copying Bitmaps to temporary output folder ---
IF NOT EXIST %OUTDIR%\Bitmaps mkdir %OUTDIR%\Bitmaps
xcopy /E /H /R /C /Y /Q %BITMAPS%\*.* %OUTDIR%\Bitmaps\*.*

echo --- Copying license to temporary output folder ---
xcopy /y /Q %SETUP%\License.rtf %OUTDIR%

echo --- Copying source files to temporary output folder ---
xcopy /y /q /e "%SETUPTMPFOLDER%\.NET Bio\Framework" %OUTDIR%
xcopy /y /q /e "%SETUPTMPFOLDER%\.NET Bio\SDK" %OUTDIR%
xcopy /y /q /e "%SETUPTMPFOLDER%\.NET Bio\BioTemplateWizard" %OUTDIR%

echo --- Copying Visual Studio Bio project template files to output folder ---
xcopy /y /q "%SETUPTMPFOLDER%\.NET Bio\BioConsoleApplicationTemplate.zip" %OUTDIR%
xcopy /y /q /e "%SETUPTMPFOLDER%\.NET Bio\Bio.TemplateWizard.dll" %OUTDIR%

echo --- Creating merge module for Framework ---
IF EXIST %SETUP%\source\MergeModule.msm DEL /Q %SETUP%\source\MergeModule.msm
xcopy /y /q %SETUP%\source\MergeModule.wxs %OUTDIR%
PUSHD %OUTDIR%
CALL "%WIXTOOLS%\candle" -nologo MergeModule.wxs
CALL "%WIXTOOLS%\light" -nologo MergeModule.wixobj -out %SETUP%\source\MergeModule.msm
POPD

echo --- Copying MergeModule.msm to temporary output folder ---
xcopy /y /q %SETUP%\source\MergeModule.msm %OUTDIR%

echo --- Deleting MergeModule.msm from Bio\Source\Tools\Installer\Common ---
IF EXIST %BioSOURCE%\Tools\Installer\Common\MergeModule.msm (
DEL /F %BioSOURCE%\Tools\Installer\Common\MergeModule.msm )

echo --- Copying MergeModule.msm to Bio\Source\Tools\Installer\Common ---
xcopy /y /q %SETUP%\source\MergeModule.msm %BioSOURCE%\Tools\Installer\Common

PUSHD %UI%

echo --- Compiling UI dialogs ---
CALL "%WIXTOOLS%\candle" -nologo -out %TEMP%\ BrowseDlg.wxs Common.wxs CommonFonts.wxs DiskCostDlg.wxs FrameworkUI.wxs CustomBioSetupTypeDlg.wxs ErrorDlg.wxs ErrorProgressText.wxs ExitDialog.wxs InstallDirDlg.wxs LicenseAgreementDlg.wxs MaintenanceTypeDlg.wxs MaintenanceWelcomeDlg.wxs MsiRMFilesInUse.wxs PrepareDlg.wxs ProgressDlg.wxs ResumeDlg.wxs UserExit.wxs VerifyReadyDlg.wxs WelcomeDlg.wxs CancelDlg.wxs FatalError.wxs OutOfDiskDlg.wxs OutOfRbDiskDlg.wxs WaitForCostingDlg.wxs CustomizeDlg.wxs PrerequisiteDeterminationDlg.wxs
POPD

PUSHD %TEMP%

REM ** Link UI dialogs **
CALL "%WIXTOOLS%\lit" -nologo -out %OUTDIR%\FrameworkUILib.wixlib BrowseDlg.wixobj Common.wixobj CommonFonts.wixobj DiskCostDlg.wixobj FrameworkUI.wixobj CustomBioSetupTypeDlg.wixobj ErrorDlg.wixobj ErrorProgressText.wixobj ExitDialog.wixobj InstallDirDlg.wixobj LicenseAgreementDlg.wixobj MaintenanceTypeDlg.wixobj MaintenanceWelcomeDlg.wixobj MsiRMFilesInUse.wixobj PrepareDlg.wixobj ProgressDlg.wixobj ResumeDlg.wixobj UserExit.wixobj VerifyReadyDlg.wixobj WelcomeDlg.wixobj  CancelDlg.wixobj FatalError.wixobj OutOfDiskDlg.wixobj OutOfRbDiskDlg.wixobj WaitForCostingDlg.wixobj CustomizeDlg.wixobj PrerequisiteDeterminationDlg.wixobj
POPD

PUSHD %SETUP%\source

echo --- Compiling core installer files ---
CALL "%WIXTOOLS%\candle" -nologo -out %OUTDIR%\ Framework.wxs SDKComponents.wxs VStemplateComponents.wxs BioTemplateComponents.wxs

POPD

PUSHD %OUTDIR%

REM ** Linking installer object files and UI library, referencing localization file and external assemblies **
CALL "%WIXTOOLS%\light" -nologo -sice:ICE05 -sw1076 -out "%SETUP%\.NET Bio.msi" Framework.wixobj SDKComponents.wixobj VStemplateComponents.wixobj BioTemplateComponents.wixobj FrameworkUILib.wixlib -loc %UI%\WixUI_en-us.wxl -ext "%WIXTOOLS%\WixUIExtension.dll" -ext "%WIXTOOLS%\WixUtilExtension.dll"

POPD

echo --- Cleaning up temporary files ---
IF EXIST %TEMP% RMDIR /S /Q %TEMP%
IF EXIST %OUTDIR% RMDIR /S /Q %OUTDIR%
IF EXIST %SETUP%\*.wixpdb DEL /Q %SETUP%\*.wixpdb
IF EXIST %SETUP%\source\*.wixpdb DEL /Q %SETUP%\source\*.wixpdb
IF EXIST %SETUP%\source\FrameworkComponents.wxs DEL /Q %SETUP%\source\FrameworkComponents.wxs
IF EXIST %SETUP%\source\SDKComponents.wxs DEL /Q %SETUP%\source\SDKComponents.wxs
IF EXIST %SETUP%\source\BioTemplateComponents.wxs DEL /Q %SETUP%\source\BioTemplateComponents.wxs

echo ************************************************************
echo  .NET Bio msi creation complete
echo ************************************************************

GOTO EOF

:SOURCEROOTMISSING
echo Error: Source root path not specified.

:SETUPTMPFOLDERMISSING
echo Error: Setup folder not specified.

:EOF