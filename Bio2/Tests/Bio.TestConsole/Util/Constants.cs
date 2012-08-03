using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.TestConsole.Util
{
    internal static class Constants 
    {
        //common config values for all utilities
        internal const string ActualOutputFileNode = "ActualOutputFile";
        internal const string ExpectedOutputFileNode = "ExpectedOutputFile";
        internal const string CommandNode = "Command";
        internal const string ConsoleCommandNode = "ConsoleCommand";
        internal const string VerboseResultNode = "VerboseResult";

        //help validation related config values
        internal const string HelpValidationNodeName = "HelpValidation";
        internal const string LisUtilHelpCommandNodeName = "LisUtilHelpCommand";
        internal const string LisUtilExpectedHelpNodeName = "LisUtilExpectedHelp";
        internal const string ComparativeUtilHelpCommandNodeName = "ComparativeUtilHelpCommand";
        internal const string ComparativeUtilExpectedHelpNodeName = "ComparativeUtilExpectedHelp";
        internal const string MumUtilHelpCommandNodeName = "MumUtilHelpCommand";
        internal const string MumUtilExpectedHelpNodeName = "MumUtilExpectedHelp";
        internal const string NucmerUtilHelpCommandNodeName = "NucmerUtilHelpCommand";
        internal const string NucmerUtilExpectedHelpNodeName = "NucmerUtilExpectedHelp";
        internal const string SamUtilsHelpCommandNodeName = "SamUtilsHelpCommand";
        internal const string SamUtilsExpectedHelpNodeName = "SamUtilsExpectedHelp";
        internal const string SamUtilsImportHelpCommandNodeName = "SamUtilsImportHelpCommand";
        internal const string SamUtilsImportExpectedHelpNodeName = "SamUtilsImportExpectedHelp";
        internal const string SamUtilsMergeHelpCommandNodeName = "SamUtilsMergeHelpCommand";
        internal const string SamUtilsMergeExpectedHelpNodeName = "SamUtilsMergeExpectedHelp";
        internal const string SamUtilsIndexHelpCommandNodeName = "SamUtilsIndexHelpCommand";
        internal const string SamUtilsIndexExpectedHelpNodeName = "SamUtilsIndexExpectedHelp";
        internal const string SamUtilsViewHelpCommandNodeName = "SamUtilsViewHelpCommand";
        internal const string SamUtilsViewExpectedHelpNodeName = "SamUtilsViewExpectedHelp";
        internal const string SamUtilsSortHelpCommandNodeName = "SamUtilsSortHelpCommand";
        internal const string SamUtilsSortExpectedHelpNodeName = "SamUtilsSortExpectedHelp";
        internal const string ScaffoldUtilHelpCommandNodeName = "ScaffoldUtilHelpCommand";
        internal const string ScaffoldUtilExpectedHelpNodeName = "ScaffoldUtilExpectedHelp";
        internal const string ConsensusUtilHelpCommandNodeName = "ConsensusUtilHelpCommand";
        internal const string ConsensusUtilExpectedHelpNodeName = "ConsensusUtilExpectedHelp";
        internal const string LayoutRefinementUtilHelpCommandNodeName = "LayoutRefinementUtilHelpCommand";
        internal const string LayoutRefinementUtilExpectedHelpNodeName = "LayoutRefinementUtilExpectedHelp";
        internal const string RepeatResolutionUtilHelpCommandNodeName = "RepeatResolutionUtilHelpCommand";
        internal const string RepeatResolutionUtilExpectedHelpNodeName = "RepeatResolutionUtilExpectedHelp";
        internal const string PadenaUtilHelpCommandNodeName = "PadenaUtilHelpCommand";
        internal const string PadenaUtilExpectedHelpNodeName = "PadenaUtilExpectedHelp";

        //MumUtil Config values.
        internal const string MummerDefaultParamsNodeName = "MummerDefaultParams";
        internal const string MummerWithMaxMatchNode = "MummerWithMaxMatch";
        internal const string MummerWithAmbiguityNode = "MummerWithAmbiguity";
        internal const string MummerWithMumSwitchNode="MummerWithMumSwitch";
        internal const string MummerWithLengthAndBothSwitchNode = "MummerWithLengthAndBothSwitch";
        internal const string MummerDefaultParamsWithEColiDataNode="MummerDefaultParamsWithEColiData";

        //NucmerUtil Config values.
        internal const string NucmerUtilDefaultParamsNodeName="NucmerUtilDefaultParams";
        internal const string NucmerDefaultParamsNodeName="NucmerDefaultParams";
        internal const string NucmerUtilLengthAnchorClusterLengthAndDiagonalSwitchesNode="NucmerUtilLengthAnchorClusterLengthAndDiagonalSwitches";
        internal const string NucmerUtilWithManySwitchesNode = "NucmerUtilWithManySwitches";
        internal const string NucmerUtilWithEColiDataNode="NucmerUtilWithEColiData";

        //PadenaUtil Config values.
        internal const string PadenaUtilDefaultParamsNodeName="PadenaUtilDefaultParams";
        internal const string PadenaUtilWithScaffoldNode = "PadenaUtilWithScaffold";
        internal const string PadenaUtilAssembleWithScaffoldNode="PadenaUtilAssembleWithScaffold";
        internal const string PadenaUtilAssembleOnEColiNode = "PadenaUtilAssembleOnEColi";

        //RepeatResolutionUtil Config values.
        internal const string RepeatResolutionUtilNode="RepeatResolutionUtil";       
        internal const string RepeatResolutionUtilVerboseResultNode = "RepeatResolutionUtilVerboseResult";

        //LayoutRefinementUtil Config values.
        internal const string LayoutRefinementUtilNode = "LayoutRefinementUtil";
     
        //ConsensusUtil Config values.
        internal const string ConsensusUtilNode = "ConsensusUtil";      

        //ScaffoldUtil Config values.
        internal const string ScaffoldUtilNode = "ScaffoldUtil";

        //ComparativeUtil Config Values
        internal const string ComparativeUtilDefaultParamsNodeName = "ComparativeUtilDefaultParams";
        internal const string ComparativeUtilWithScaffoldEcoliNodeName = "ComparativeUtilWithScaffoldEcoli";
        internal const string ComparativeUtilEcoliDefaultParamsNodeName = "ComparativeUtilEcoliDefaultParams";
        internal const string ComparativeUtilVerboseNodeName = "ComparativeUtilVerbose";
        internal const string ComparativeUtilWithKmerAndMumNodeName = "ComparativeUtilWithKmerAndMum";
        internal const string ComparativeUtilWithMeanLengthInsertNodeName = "ComparativeUtilWithMeanLengthInsert";

        //LisUtil Config Values
        internal const string LisUtilDefaultParamsNodeName = "LisUtilDefaultParams";
        internal const string LisUtilEcoliDefaultParamsNodeName = "LisUtilEcoliDefaultParams";
        internal const string LisUtilVerboseNodeName = "LisUtilVerbose";
        internal const string LisUtilWithMaxMatchNodeName = "LisUtilWithMaxMatch";
        internal const string LisUtilWithLisOnlyNodeName = "LisUtilWithLisOnly";
        internal const string LisUtilWithReverseOnlyNodeName = "LisUtilWithReverseOnly";
        internal const string LisUtilWithBothNodeName = "LisUtilWithBoth";
        internal const string LisUtilWithNoAmbiguityNodeName = "LisUtilWithNoAmbiguity";
        internal const string LisUtilWithComplementReverseOnlyNodeName = "LisUtilWithComplementReverseOnly";
        internal const string LisUtilWithComplementBothNodeName = "LisUtilWithComplementBoth";
        internal const string LisUtilWithShowMatchingStrNodeName = "LisUtilWithShowMatchingStr";

        //SamUtils Config Values
        internal const string SamUtilsWithImportSAMtoBAMNodeName = "SamUtilsWithImportSAMtoBAM";
        internal const string SamUtilsWithImportBAMtoSAMNodeName = "SamUtilsWithImportBAMtoSAM";
        internal const string SamUtilsWithSortNodeName = "SamUtilsWithSort";
        internal const string SamUtilsWithSortByReadNamesNodeName = "SamUtilsWithSortByReadNames";
        internal const string SamUtilsWithIndexNodeName = "SamUtilsWithIndex";
        internal const string SamUtilsWithViewDefaultParamsNodeName = "SamUtilsWithViewDefaultParams";
        internal const string SamUtilsWithViewHeaderForSAMNodeName = "SamUtilsWithViewHeaderForSAM";
        internal const string SamUtilsWithViewHexNodeName = "SamUtilsWithViewHex";
        internal const string SamUtilsWithViewMinMapQualityNodeName = "SamUtilsWithViewMinMapQuality";
        internal const string SamUtilsWithViewOutputBAMNodeName = "SamUtilsWithViewOutputBAM";
        internal const string SamUtilsWithViewSAMFormatNodeName = "SamUtilsWithViewSAMFormat";
        internal const string SamUtilsWithViewCompressedBAMNodeName = "SamUtilsWithViewCompressedBAM";
    }
}
