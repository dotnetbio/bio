REM -- ********************************************************************************
REM --     Description
REM -- ********************************************************************************
REM -- Prepares folder structure required for installer and copies the required files.
REM -- ********************************************************************************


@Echo Off
@if not "%ECHO%"=="" Echo %ECHO%

echo ************************************************************
echo Preparing folder structure.
echo ************************************************************

SET SourceFolder=%1
SET TargetFolder=%2
SET SETUP_BIO_ONLY=%3

CD %TargetFolder%

IF EXIST .\Setup.Tmp RMDIR /S /Q .\Setup.Tmp
MD .\Setup.Tmp
echo ************************************************************
echo Copying .NET Bio binaries
echo ************************************************************

SET BioFolder=".\Setup.Tmp\.NET Bio"

MD %BioFolder%

echo ************************************************************
echo Copying Visual Studio template
echo ************************************************************
echo SOURCE FOLDER PATH IS: %SourceFolder%
Xcopy /y /i %SourceFolder%\Binaries\Release\Bio.TemplateWizard.dll %BioFolder%
Xcopy /y /i %SourceFolder%\Binaries\Release\BioConsoleApplicationTemplate.zip %BioFolder%

Xcopy /y /i %SourceFolder%\Binaries\Release\Bio.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.WebServiceHandlers.dll %BioFolder%\Framework\

echo ************************************************************
echo Copying Add-ins Binaries
echo ************************************************************

XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Padena.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Pamsam.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Comparative.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Hpc.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Hpc.DistributeApp.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Hpc.DistributeApp.exe.config %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\HpcLibSettings.local.xml %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\HpcLibSettings.xml %BioFolder%\Framework\

echo ************************************************************
echo Copying Tools Binaries
echo ************************************************************
XCopy /y /i %SourceFolder%\Binaries\Release\ReadSimulator.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\SAMUtils.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Workflow.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\MumUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\LISUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\NucmerUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\PadenaUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\RepeatResolutionUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\LayoutRefinementUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\ConsensusUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\ScaffoldUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\ComparativeUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\FilterReadsUtil.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\SampleClusterApp.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\Tools.VennToNodeXL.dll %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\BedStats.exe %BioFolder%\Framework\
XCopy /y /i %SourceFolder%\Binaries\Release\VennTool.exe %BioFolder%\Framework\

echo ************************************************************
echo Copying SDK
echo ************************************************************

MD %BioFolder%\SDK

echo ************************************************************
echo Copying Credits and License text files
echo ************************************************************
XCopy /y /i %SourceFolder%\Source\Credits.txt %BioFolder%\SDK\
XCopy /y /i %SourceFolder%\Source\License.txt %BioFolder%\SDK\


echo ************************************************************
echo Copying SDK Documents
echo ************************************************************

MD %BioFolder%\SDK\Docs

XCopy /y /i %SourceFolder%\docs\Bio.chm %BioFolder%\SDK\Docs
Xcopy /y /i %SourceFolder%\docs\Becoming_A_Committer_v2.docx %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Coding_Conventions.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Commenting_Conventions.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Committer_Guide.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Committer_Onboarding.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Comparative_Assembly_Technical_Guide.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Contribution_Documentation_Template.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Contributor_Guide.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Getting_Started.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Overview.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Parallel_de_Novo_Assembler_Technical_Guide.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Programming_Guide.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Sample_for_IronPython_Programming_Guide.docx" %BioFolder%\SDK\Docs\
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Testing_Guide.docx" %BioFolder%\SDK\Docs\

echo ************************************************************
echo Copying Framework source code
echo ************************************************************

MD %BioFolder%\SDK\Framework
XCopy /s /y /i %SourceFolder%\Source\Framework\*.* %BioFolder%\SDK\Framework\

echo ************************************************************
echo Copying IronPython Scripts
echo ************************************************************

MD %BioFolder%\SDK\Tools\IronPython

echo ************************************************************
echo Copying IronPython Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\IronPython\BioIronPython
XCopy /y /i %SourceFolder%\Source\Tools\Python\BioDebug.py %BioFolder%\SDK\Tools\IronPython\
XCopy /y /i %SourceFolder%\Source\Tools\Python\BioDemo.py %BioFolder%\SDK\Tools\IronPython\
XCopy /y /i %SourceFolder%\Source\Tools\Python\BioMenu.py %BioFolder%\SDK\Tools\IronPython\
XCopy /s /y /i %SourceFolder%\Source\Tools\Python\BioIronPython\*.* %BioFolder%\SDK\Tools\IronPython\BioIronPython\

echo ************************************************************
echo Copying ReadSimulator Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\ReadSimulator
XCopy /s /y /i %SourceFolder%\Source\Tools\ReadSimulator\*.* %BioFolder%\SDK\Tools\ReadSimulator\

echo ************************************************************
echo Copying SAMUtils Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\SAMUtils
XCopy /s /y /i %SourceFolder%\Source\Tools\SAMUtil\*.* %BioFolder%\SDK\Tools\SAMUtils\

echo ************************************************************
echo Copying TridentWorkflows Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\TridentWorkflows
XCopy /s /y /i %SourceFolder%\Source\Tools\Bio.Workflow\*.* %BioFolder%\SDK\Tools\TridentWorkflows\

echo ************************************************************
echo Copying MumUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\MumUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\MumUtil\*.* %BioFolder%\SDK\Tools\MumUtil\

echo ************************************************************
echo Copying LisUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\LisUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\LisUtil\*.* %BioFolder%\SDK\Tools\LisUtil\

echo ************************************************************
echo Copying NucmerUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\NucmerUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\NucmerUtil\*.* %BioFolder%\SDK\Tools\NucmerUtil\

echo ************************************************************
echo Copying PadenaUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\PadenaUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\PadenaUtil\*.* %BioFolder%\SDK\Tools\PadenaUtil\

echo ************************************************************
echo Copying RepeatResolutionUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\RepeatResolutionUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\RepeatResolutionUtil\*.* %BioFolder%\SDK\Tools\RepeatResolutionUtil\

echo ************************************************************
echo Copying LayoutRefinementUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\LayoutRefinementUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\LayoutRefinementUtil\*.* %BioFolder%\SDK\Tools\LayoutRefinementUtil\

echo ************************************************************
echo Copying ConsensusUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\ConsensusUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\ConsensusUtil\*.* %BioFolder%\SDK\Tools\ConsensusUtil\

echo ************************************************************
echo Copying ScaffoldUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\ScaffoldUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\ScaffoldUtil\*.* %BioFolder%\SDK\Tools\ScaffoldUtil\

echo ************************************************************
echo Copying ComparativeUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\ComparativeUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\ComparativeUtil\*.* %BioFolder%\SDK\Tools\ComparativeUtil\

echo ************************************************************
echo Copying FilterReadsUtil Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\FilterReadsUtil
XCopy /s /y /i %SourceFolder%\Source\Tools\FilterReadsUtil\*.* %BioFolder%\SDK\Tools\FilterReadsUtil\

echo ************************************************************
echo Copying SampleClusterApp Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\SampleClusterApp
XCopy /s /y /i %SourceFolder%\Source\Tools\SampleClusterApp\*.* %BioFolder%\SDK\Tools\SampleClusterApp\

echo ************************************************************
echo Copying FileFormat Converter Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\FileFormatConverter
XCopy /s /y /i %SourceFolder%\Source\Tools\FileFormatConverter\*.* %BioFolder%\SDK\Tools\FileFormatConverter\

echo ************************************************************
echo Copying BedStats Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\BedStats
XCopy /s /y /i %SourceFolder%\Source\Tools\DevUtils\Tools.VennToNodeXL\*.* %BioFolder%\SDK\Tools\Tools.VennToNodeXL\

MD %BioFolder%\SDK\Tools\Tools.VennToNodeXL
XCopy /s /y /i %SourceFolder%\Source\Tools\DevUtils\BedStats\*.* %BioFolder%\SDK\Tools\BedStats\

echo ************************************************************
echo Copying VennTool Source
echo ************************************************************

MD %BioFolder%\SDK\Tools\VennTool
XCopy /s /y /i %SourceFolder%\Source\Tools\DevUtils\VennTool\*.* %BioFolder%\SDK\Tools\VennTool\

echo ************************************************************
echo Copying .Net Bio Template Source
echo ************************************************************

MD %BioFolder%\BioTemplateWizard
XCopy /s /y /i %SourceFolder%\Source\Tools\Bio.TemplateWizard\*.* %BioFolder%\BioTemplateWizard\Bio.TemplateWizard\


echo ************************************************************
echo Copying Samples ReadMe.txt
echo ************************************************************
XCopy /y /i %SourceFolder%\Source\Tools\ReadMe.txt %BioFolder%\SDK\Tools\


IF "%SETUP_BIO_ONLY%" == "true" GOTO EOF

echo ************************************************************
echo Copying MBT
echo ************************************************************

SET ToolsFolder=".\Setup.Tmp\Microsoft Biology Tools"
MD %ToolsFolder%


echo ************************************************************
echo Copying SequenceAssembler binaries
echo ************************************************************

SET SequenceAssemblerFolder=%ToolsFolder%"\Sequence Assembler"
MD %SequenceAssemblerFolder%

XCopy /y /i %SourceFolder%\Binaries\Release\Bio.dll %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.WebServiceHandlers.dll %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\QUT.Bio.dll %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\WPFToolkit.dll %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\BioSequenceAssembler.exe.config %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\BioSequenceAssembler.exe %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Pamsam.dll %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Padena.dll %SequenceAssemblerFolder%\
XCopy /y /i %SourceFolder%\Binaries\Release\Bio.Comparative.dll %SequenceAssemblerFolder%\

echo ************************************************************
echo Copying .Net Bio SequenceAssembler Document
echo ************************************************************

MD %SequenceAssemblerFolder%\Docs
Xcopy /y /i %SourceFolder%\docs\".Net Bio_Sequence_Assembler_User_Guide*.docx" %SequenceAssemblerFolder%\Docs\".Net Bio_Sequence_Assembler_User_Guide*.docx"


echo ************************************************************
echo Copying .Net Bio Extensions for Excel binaries
echo ************************************************************

SET ExcelFolder=%ToolsFolder%"\Excel Biology Extension"
MD %ExcelFolder%

Copy /y %SourceFolder%\Binaries\Release\Bio.dll %ExcelFolder%\Bio.dll
Copy /y %SourceFolder%\Binaries\Release\BioExcel.Visualizations.Common.dll %ExcelFolder%\BioExcel.Visualizations.Common.dll
Copy /y %SourceFolder%\Binaries\Release\BioExcel.dll %ExcelFolder%\BioExcel.dll
Copy /y %SourceFolder%\Binaries\Release\Bio.WebServiceHandlers.dll %ExcelFolder%\Bio.WebServiceHandlers.dll
Copy /y %SourceFolder%\Binaries\Release\Tools.VennToNodeXL.dll %ExcelFolder%\Tools.VennToNodeXL.dll
Copy /y %SourceFolder%\Binaries\Release\Microsoft.Office.Tools.Common.v4.0.Utilities.dll %ExcelFolder%\Microsoft.Office.Tools.Common.v4.0.Utilities.dll
Copy /y %SourceFolder%\Binaries\Release\BioExcel.vsto %ExcelFolder%\BioExcel.vsto
Copy /y %SourceFolder%\Binaries\Release\BioExcel.dll.manifest %ExcelFolder%\BioExcel.dll.manifest
Copy /y %SourceFolder%\Binaries\Release\DisplayDNASequenceDistribution.bas %ExcelFolder%\DisplayDNASequenceDistribution.bas
Copy /y %SourceFolder%\Binaries\Release\Microsoft.Office.Tools.Common.v4.0.Utilities.xml %ExcelFolder%\Microsoft.Office.Tools.Common.v4.0.Utilities.xml

echo ************************************************************
echo Copying .Net Bio Extensions for Excel Documents
echo ************************************************************

MD %ExcelFolder%\Docs
XCopy /y %SourceFolder%\Docs\".Net Bio_Extension_for_Excel_User_Guide*.docx" %ExcelFolder%\Docs\".Net Bio_Extension_for_Excel_User_Guide*.docx"

:EOF
SET SourceFolder=
SET TargetFolder=
SET SETUP_BIO_ONLY=
