/****************************************************************************
 * Constants.cs
 * 
 * This file contains the constants used across the project.
 * 
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.TestAutomation.Util
{
    internal static class Constants
    {
        internal const string FilePathNode = "FilePath";
        internal const string FilePathNode1 = "FilePath1";
        internal const string FilePathNode2 = "FilePath2";
        internal const string ExpectedSequenceNode = "ExpectedSequence";
        internal const string ExpectedSequenceNode1 = "ExpectedSequence1";
        internal const string ExpectedSequenceNode2 = "ExpectedSequence2";
        internal const string ExpectedSequenceNode3 = "ExpectedSequence3";
        internal const string ExpectedSequenceNode4 = "ExpectedSequence4";
        internal const string ExpectedSequenceNode5 = "ExpectedSequence5";
        internal const string ExpectedSequenceNode6 = "ExpectedSequence6";
        internal const string ExpectedSequenceNode7 = "ExpectedSequence7";
        internal const string ExpectedSequenceNode8 = "ExpectedSequence8";
        internal const string ExpectedSequenceNode9 = "ExpectedSequence9";
        internal const string ExpectedSequenceNode10 = "ExpectedSequence10";
        internal const string NumberOfSequencesNode = "NumberOfSequences";
        internal const string AlphabetNameNode = "AlphabetName";
        internal const string AlphabetNameNodeV2 = "AlphabetNameV2";
        internal const string AlphabetNameNode1 = "AlphabetName1";
        internal const string AlphabetNameNode2 = "AlphabetName2";
        internal const string AlphabetNameNode3 = "AlphabetName3";
        internal const string FriendlyNameNode = "FriendlyNameNode";
        internal const string NucleotidesNode = "Nucleotides";
        internal const string ExpectedFriendlyNamesDnaNode = "ExpectedFriendlyNamesDna";
        internal const string ExpectedFriendlyNamesRnaNode = "ExpectedFriendlyNamesRna";
        internal const string ExpectedFriendlyNamesAmbiguousDnaNode = "ExpectedFriendlyNamesAmbiguousDna";
        internal const string ExpectedFriendlyNamesAmbiguousRnaNode = "ExpectedFriendlyNamesAmbiguousRna";
        internal const string ExpectedFriendlyNamesProteinNode = "ExpectedFriendlyNamesProtein";
        internal const string ExpectedFriendlyNamesAmbiguousProteinNode = "ExpectedFriendlyNamesAmbiguousProtein";
        internal const string SequenceIdNode = "SequenceID";
        internal const string SequenceIdNode1 = "SequenceID1";
        internal const string SequenceIdNode2 = "SequenceID2";
        internal const string SequenceIdNode3 = "SequenceID3";
        internal const string FormatStringNode = "FormatString";
        internal const string FormatStringNode1 = "FormatString1";
        internal const string FormatStringNode2 = "FormatString2";
        internal const string FormatStringNode3 = "FormatString3";
        internal const string IsReadOnlyNode = "IsReadOnly";
        internal const string StrandTopologyNode = "StrandTopology";
        internal const string StrandTypeNode = "StrandType";
        internal const string DivisionNode = "Division";
        internal const string DateNode = "Date";
        internal const string VersionNode = "Version";
        internal const string PrimaryIdNode = "PrimaryID";
        internal const string ExpectedOutputNode = "ExpectedOutput";
        internal const string MetadataCountNode = "MetaDataCount";
        internal const string ExpectedSequence1inLowerNode = "ExpectedSequence1inLower";
        internal const string ExpectedSequence2inLowerNode = "ExpectedSequence2inLower";
        internal const string ExpectedGapExtensionSequence1InLower = "ExpectedGapExtensionSequence1InLower";
        internal const string ExpectedGapExtensionSequence2InLower = "ExpectedGapExtensionSequence2InLower";

        // Parent Node name specific to the test cases
        internal const string LocationRangesNode = "LocationRanges";
        internal const string SimpleGenBankNodeName = "SimpleGenBank";
        internal const string GenBankTempFileName = "temp.gbk";
        internal const string FastaTempFileName = "temp.fasta";
        internal const string WiggleTempFileName = "temp.wig";
        internal const string SimpleFastaNodeName = "SimpleFasta";
        internal const string SimpleFastaDnaNodeName = "SimpleFastaDNA";
        internal const string SimpleFastaDnaDVNodeName = "SimpleFastaDnaDV";
        internal const string SimpleFastaRnaNodeName = "SimpleFastaRNA";
        internal const string SimpleFastaProteinNodeName = "SimpleFastaProtein";
        internal const string MediumSizeFastaNodeName = "MediumSizeFasta";
        internal const string MultipleSequenceFastaNodeName =
            "MultipleSequenceFasta";
        internal const string MultipleSequenceGenBankNodeName =
            "MultipleSequenceGenBank";
        internal const string OneLineSequenceFastaNodeName = "OneLineSeqFasta";
        internal const string MultipleSequenceDnaRnaFastaNodeName =
            "MultipleSequenceDNARNAFasta";
        internal const string MultipleSequenceRnaProFastaNodeName =
            "MultipleSequenceRNAProteinFasta";
        internal const string MultipleSequenceDnaProFastaNodeName =
            "MultipleSequenceDNAProteinFasta";
        internal const string MultipleSequenceDnaRnaProFastaNodeName =
            "MultipleSequenceDNARNAProteinFasta";
        internal const string MediumSizeGenBankNodeName = "MediumSizeGenBank";
        internal const string OneLineSequenceGenBankNodeName =
            "OneLineSeqGenBank";
        internal const string SimpleGenBankDnaNodeName = "SimpleGenBankDNA";
        internal const string BoyerMooreSequence = "BoyerMooreSequence";
        internal const string ExpectedMatch = "ExpectedMatch";
        internal const string SimpleGenBankRnaNodeName = "SimpleGenBankRNA";
        internal const string SimpleGenBankProNodeName = "SimpleGenBankProtein";
        internal const string MandatoryGenBankHeadersNodeName =
            "MandatoryGenBankHeaders";
        internal const string MultipleReferenceGenBankNodeName =
            "MultipleReferenceGenBank";
        internal const string MultipleGeneCDSGenBankNodeName =
            "MultipleGeneCDSGenBank";
        internal const string DNAStandardFeaturesKeyNode =
            "DNAStandardKeyFeatures";
        internal const string MultiSequenceGenBankDNANode =
            "MultiSequenceGenBankDNA";
        internal const string MultiSeqGenBankProteinNode =
            "MultiSeqGenBankProtein";
        internal const string RNAGenBankFeaturesNode = "RNAGenBankFeatures";
        internal const string GenBankFeaturesCount = "GenBankFeaturesCount";
        internal const string GenBankRNAMetaData = "SimpleGenBankRNA";
        internal const string CDSCount = "CDSCount";
        internal const string mRNACount = "mRNACount";
        internal const string CDSKey = "CDSKeyName";
        internal const string ExonKey = "ExonKeyName";
        internal const string IntronKey = "IntronKeyName";
        internal const string mRNAKey = "mRNAKeyName";
        internal const string rRNACount = "rRNACount";
        internal const string emptyCount = "emptyCount";
        internal const string ExonCount = "ExonCount";
        internal const string IntronCount = "IntronCount";
        internal const string SigPeptideCount = "SigPeptideCount";
        internal const string MiscFeatureCount = "MiscFeatureCount";
        internal const string GeneCount = "GeneCount";
        internal const string tRNACount = "tRNACount";
        internal const string PromoterCount = "PromoterCount";
        internal const string SourceKey = "SourceKeyName";
        internal const string ProteinKeyName = "proteinKeyName";
        internal const string FirstKey = "FirstKey";
        internal const string SecondKey = "SecondKey";
        internal const string FirstLocation = "FirstLocation";
        internal const string SecondLocation = "SecondLocation";
        internal const string FirstQualifier = "FirstQualifier";
        internal const string SecondQualifier = "SecondQualifier";
        internal const string FirstRangeStartPoint = "FirstRangeStartPoint";
        internal const string SecondRangeStartPoint = "SecondRangeStartPoint";
        internal const string FirstRangeEndPoint = "FirstRangeEndPoint";
        internal const string SecondRangeEndPoint = "SecondRangeEndPoint";
        internal const string FeaturesWithinFirstRange =
            "FeaturesWithinFirstRange";
        internal const string FeaturesWithinSecondRange =
            "FeaturesWithinSecondRange";
        internal const string CDSProductQualifier = "CDSProductQualifier";
        internal const string CDSException = "CDSException";
        internal const string CDSCodonStart = "CDSCodonStart";
        internal const string CDSLabel = "CDSLabel";
        internal const string StandardFeaturesCount = "StandardFeaturesCount";
        internal const string CDSDBReference = "CDSDBReference";
        internal const string GenBankAttenuatorQualifiers =
            "GenBankAttenuatorQualifiers";
        internal const string GeneSymbol = "GeneSymbol";
        internal const string CDSNote = "CDSNote";
        internal const string citationReferencedCount =
            "citationReferencedCount";
        internal const string MediumSizeDNAGenBankFeaturesNode =
            "MediumSizeDNAGenBankFeatures";
        internal const string MediumSizeRNAGenBankFeaturesNode =
            "MediumSizeRNAGenBankFeatures";
        internal const string MediumSizePROTEINGenBankFeaturesNode =
            "MediumSizePROTEINGenBankFeatures";
        internal const string MulitSequenceGenBankRNANode =
            "MulitSequenceGenBankRNA";
        internal const string MediumSizeMulitSequenceGenBankRNANode =
            "MulitSequenceGenBankRNA";
        internal const string GeneFeatureGeneSymbol = "GeneFeatureGeneSymbol";
        internal const string GeneFeatureDBReference = "GeneDBReference";
        internal const string GeneComplement = "GeneComplement";
        internal const string TRNAGeneSymbol = "tRNAGeneSymbol";
        internal const string TRNAProduct = "tRNAProduct";
        internal const string TRNAComplement = "tRNAComplement";
        internal const string MRNAGeneSymbol = "mRNAGeneSymbol";
        internal const string MRNAComplementStart = "mRNAComplementStart";
        internal const string MRNANote = "mRNANote";
        internal const string MRNAComplement = "mRNAComplement";
        internal const string QualifiersCount = "QualifiersCount";
        internal const string MRNAcitationReferencedCount =
            "mRNAcitationReferencedCount";
        internal const string EnhancerCitationReferencedCount =
            "EnhancercitationReferencedCount";
        internal const string ExonCitationReferencedCount =
            "ExoncitationReferencedCount";
        internal const string IntronCitationReferencedCount =
            "IntroncitationReferencedCount";
        internal const string PromotersCitationReferencedCount =
            "PromoterscitationReferencedCount";
        internal const string MiscFeatureNoteSymbol = "MiscFeatureNoteSymbol";
        internal const string ExonComplement = "ExonComplement";
        internal const string ExonNumber = "ExonNumber";
        internal const string ExonGeneSymbol = "ExonGeneSymbol";
        internal const string IntronComplement = "IntronComplement";
        internal const string IntronNumber = "IntronNumber";
        internal const string IntronGeneSymbol = "IntronGeneSymbol";
        internal const string PromoterComplement = "PromoterComplement";
        internal const string DNAGenBankVariationNode = "DNAGenBank";
        internal const string RNAGenBankVariationNode = "RNAGenBank";
        internal const string ProteinGenBankVariationNode = "ProteinGenBank";
        internal const string VariationReplace = "VariationReplace";
        internal const string VarationCount = "VarationCount";
        internal const string ProteinBindingCount = "ProteinBindingCount";
        internal const string BoundMoiety = "boundmoiety";
        internal const string MiscDiffCount = "MiscDiffCount";
        internal const string MiscQualifiersCount = "MiscQualifiersCount";
        internal const string GenBank = "GenBank";
        internal const string MiscPeptideCount = "MiscPeptideCount";
        internal const string ProteinGenBankMPeptideQualifiersNode =
            "ProteinGenBankMPeptideQualifiers";
        internal const string ProteinGenBankMiscRecombinationQualifiersNode =
            "ProteinGenBankRecombinationQualifiers";
        internal const string ProteinGenBankMiscRNAQualifiersNode =
            "GenBankMiscRNAQualifiers";
        internal const string GenBankMinus35SignalNode = "GenBankMInus35Signal";
        internal const string GenBankMInus10SignalNode = "GenBankMInus10Signal";
        internal const string GenBankPolyASignalNode = "GenBankPolyASignal";
        internal const string GenBankTerminatorNode = "GenBankTerminator";
        internal const string GenBankDLoopNode = "GenBankDLoop";
        internal const string product = "Product";
        internal const string GenBankrRNAQualifiersNode =
            "GenBankRibosomalRNAQualifiers";
        internal const string GenBankMiscSignalNode = "GenBankMiscSignal";
        internal const string GenBankInterveningDNA = "GenBankInterveningDNA";
        internal const string ProductName = "ProductName";
        internal const string Location = "Location";
        internal const string Intereference = "Intereference";
        internal const string FeaturesCount = "FeaturesCount";
        internal const string ROriginLocation = "ROriginLocation";
        internal const string ROriginCount = "ROriginCount";
        internal const string Note = "Note";
        internal const string QualifierCount = "Count";
        internal const string CaatSignalCount = "CaatSignalCount";
        internal const string TataSignalCount = "TataSignalCount";
        internal const string CaatSignalGene = "CaatSignalGene";
        internal const string TataSignalGene = "TataSignalGene";
        internal const string CaatSignalLocation = "CaatSignalLocation";
        internal const string TataSignalLocation = "TataSignalLocation";
        internal const string ThreePrimeUTRGene = "ThreePrimeUTRGene";
        internal const string ThreePrimeUTRCount = "ThreePrimeUtrCount";
        internal const string RepeatRegionLocation =
            "RepeatRegionLocation";
        internal const string RepeatRegionMobile_element =
            "RepeatRegionmobile_element";
        internal const string GenBankRepeatRegionQualifiersNode =
            "GenBankRepeatRegionQualifiers";
        internal const string ExpectedFeatureSubSequence = "SubSequence";
        internal const string FivePrimeUTRLocation = "FivePrimeUTRLocation";
        internal const string FivePrimeUTRGene = "FivePrimeUTRGene";
        internal const string SignalPeptideGene = "SignalPeptideGene";
        internal const string SignalPeptideLocation = "SignalPeptideLocation";
        internal const string FirstRangeCount = "FirstRangeCount";
        internal const string SecondRangeCount = "SecondRangeCount";
        internal const string LocationSeperatorNode = "LocationSeperator";
        internal const string LoocationEndNode = "LoocationEnd";
        internal const string LoocationStartNode = "LoocationStart";
        internal const string LocationStringValue = "LocationString";
        internal const string NormalLocationBuilderNode =
            "NormalLocationBuilder";
        internal const string SingleDotLocationBuilderNode =
            "SingleDotLocationBuilder";
        internal const string JoinOperatorLocationBuilderNode =
            "JoinOperatorLocationBuilder";
        internal const string SubLocationString = "SubLocationString";
        internal const string SubLocationStart = "SubLocationStart";
        internal const string SubLocationEnd = "SubLocationEnd";
        internal const string SubLocationCount = "SubLocationCount";
        internal const string SubLocationSeperator = "SubLocationSeperator";
        internal const string ComplementOperatorLocationBuilderNode =
            "ComplementOperatorLocationBuilder";
        internal const string OrderOperatorLocationBuilderNode =
            "OrderOperatorLocationBuilder";
        internal const string GenBankFileLocationBuilderNode =
            "GenBankFileLocationBuilder";
        internal const string Accession = "Accession";
        internal const string GenBankFileSubSequenceNode =
            "GenBankFileSubSequence";
        internal const string GenBankFileSubSequenceDnaNode =
            "GenBankFileSubSequenceDNA";
        internal const string GenBankFileSubSequenceProteinNode =
            "GenBankFileSubSequenceProtein";
        internal const string ExpectedSubSequence = "expectedSubSequence";
        internal const string SequenceStart = "SequenceStart";
        internal const string SequenceEnd = "SequenceEnd";
        internal const string referenceSeq = "referenceSeq";
        internal const string ThirdFeatureKey = "ThirdFeatureKey";
        internal const string FourthKey = "FourthKey";
        internal const string ThirdLocation = "ThirdLocation";
        internal const string FourthLocation = "FourthLocation";
        internal const string FifthLocation = "FifthLocation";
        internal const string FifthKey = "FiftKey";
        internal const string MainFeaturesCount = "FeaturesCount";
        internal const string GenBankSequenceFeaturesNode =
            "GenBankSequenceFeatures";
        internal const string GenBankSequenceFeaturesForMRNA =
            "GenBankSequenceFeaturesFormRNA";
        internal const string SecondCount = "SecondCount";
        internal const string ThirdCount = "ThirdCount";
        internal const string Reference = "Reference1";
        internal const string GenBankMiscStructureNode = "GenBankMiscStructure";
        internal const string CitationNode = "Citation";
        internal const string ExperimentNode = "experiment";
        internal const string GeneSynonymNode = "Gene_synonym";
        internal const string InferenceNode = "Inference";
        internal const string LabelNode = "Label";
        internal const string LocusTagNode = "LocusTag";
        internal const string OldLocusTagNode = "OldLocusTag";
        internal const string AlleleNode = "Allele";
        internal const string GenbankMapNode = "Map";
        internal const string GenBankTransitPeptideNode =
            "GenBankTransitPeptide";
        internal const string GenBankStemLoopNode = "GenBankStemLoop";
        internal const string GenBankModifiedBaseNode = "GenBankModifiedBase";
        internal const string GenBankPrecursorRNANode = "GenBankPrecursorRNA";
        internal const string GenBankPolySiteNode = "GenBankPolySite";
        internal const string GenBankMiscBindingNode = "GenBankMiscBinding";
        internal const string GenBankEnhancerNode = "GenBankEnhancer";
        internal const string GenBankGCSignalNode = "GenBankGCSignal";
        internal const string GenBankLongTerminalRepeatNode =
            "GenBankLongTerminalRepeat";
        internal const string GenBankOperonFeatureNode = "GenBankOperonFeature";
        internal const string GenBankUnsureSequenceRegionNode =
            "GenbankUnsureSequenceRegion";
        internal const string GenBankNonCodingRNANode = "GenBankNonCodingRNA";
        internal const string GenBankCodingSequenceNode = "GenBankCodingSequence";
        internal const string NonCodingRnaClassNode = "NonCodingRnaClass";
        internal const string StandardQualifierNamesNode =
            "StandardQualifierNames";
        internal const string AlleleQualifier = "AlleleQualifier";
        internal const string GeneQualifier = "GeneQualifier";
        internal const string DBReferenceQualifier = "DBReferenceQualifier";
        internal const string CodonStartNode = "CodonStart";
        internal const string ProductNode = "Product";
        internal const string ProteinIdNode = "ProteinId";
        internal const string GenbankTranslationNode = "Translation";
        internal const string GenBankCDSNode = "GenBankCodingSequence";
        internal const string StandardNameNode = "StandardName";
        internal const string OperonNode = "Operon";
        internal const string FastAFileParserNode = "FastAFileParser";
        internal const string GenBankFileParserNode = "GenBankFileParser";
        internal const string FastQFileParserNode = "FastQFileParser";
        internal const string FastAFileFormatterNode = "FastAFileFormatter";
        internal const string FastQFileFormatterNode = "FastQFileFormatter";
        internal const string GffFileFormatterNode = "GffFileFormatter";
        internal const string GenBankFileFormatterNode = "GenBankFileFormatter";
        internal const string GffFileParserNode = "GffFileParser";
        internal const string BamFileParserNode = "BamFileParser";
        internal const string SamFileParserNode = "SamFileParser";
        internal const string BamFileFormatterNode = "BamFileFormatter";
        internal const string SamFileFormatterNode = "SamFileFormatter";
        internal const string FilePathsNode = "FilePaths";
        internal const string ParserNameNode = "ParserName";
        internal const string DescriptionNode = "Description";
        internal const string FileTypesNode = "FileTypes";
        internal const string ExpSequenceWithOperator = "SequenceWithOperator";
        internal const string LocationWithComplementOperator =
            "LocationWithComplementOperator";
        internal const string LocationWithJoinOperatorNode =
            "LocationWithJoinOperator";
        internal const string LocationWithOutComplementOperatorNode =
            "LocationWithOutComplementOperator";
        internal const string LocationWithOutJoinOperatorNode =
           "LocationWithOutJoinOperator";
        internal const string LocationWithOutOrderOperatorNode =
            "LocationWithOutOrderOperator";
        internal const string LocationWithDotOperatorNode =
            "LocationWithDotOperator";
        internal const string InvalidLocationWithComplementOperatorNode =
            "InvalidLocationWithComplementOperator";
        internal const string EndData = "EndData";
        internal const string Position = "Position";
        internal const string LocationWithEndDataNode =
            "LocationWithEndData";
        internal const string LocationWithEndDataUsingOperatorNode =
            "LocationWithEndDataUsingOperator";
        internal const string InvalidateGenBankNodeName =
            "InvalidateGenBank";
        internal const string InvalidGenBankWithoutLocusNode =
            "InvalidGenBankWithoutLocus";
        internal const string InvalidGenBankWithSegmentNode =
            "InvalidGenBankWithSegment";
        internal const string InvalidGenBankWithPrimaryNode =
            "InvalidGenBankWithPrimary";
        internal const string SimpleGenBankPrimaryNode =
            "SimpleGenBankPrimary";
        internal const string InvalidateGenBankParseFeaturesHasReaderNode =
            "InvalidateGenBankParseFeaturesHasReader";
        internal const string InvalidGenBankUnknownLocusNode =
            "InvalidGenBankUnknownLocus";
        internal const string InvalidGenBankUnknownStrandTypeNode =
            "InvalidGenBankUnknownStrandType";
        internal const string InvalidGenBankUnknownStrandTopologyNode =
            "InvalidGenBankUnknownStrandTopology";
        internal const string InvalidGenBankUnknownRawDateNode =
            "InvalidGenBankUnknownRawDate";
        internal const string InvalidGenBankUnknownMoleculeTypeNode =
            "InvalidGenBankUnknownMoleculeType";
        internal const string InvalidGenBankParseReferenceNode =
            "InvalidGenBankParseReference";
        internal const string InvalidGenBankParseReferenceDefaultNode =
            "InvalidGenBankParseReferenceDefault";
        internal const string InvalidGenBankParseSequenceDefaultNode =
            "InvalidGenBankParseSequenceDefault";
        internal const string InvalidGenBankParseSequenceNode =
            "InvalidGenBankParseSequence";
        internal const string InvalidGenBankParseSourceNode =
            "InvalidGenBankParseSource";
        internal const string OperatorGenBankFileNode =
            "OperatorGenBankFile";
        internal const string MediumSizeGenBankFileNode =
            "MediumSizeGenBankFile";
        internal const string CompareLocationsNode =
            "CompareLocations";
        internal const string Location1Node = "Location1";
        internal const string Location2Node = "Location2";
        internal const string Location3Node = "Location3";
        internal const string LeafLocationCountNode =
            "LeafLocationCount";
        internal const string StartData = "StartData";
        internal const string InvalidGenBankHeaderDataTypeNode =
            "InvalidGenBankHeaderDataType";
        internal const string InvalideGenBankReferenceNode =
            "InvalideGenBankReference";
        internal const string InvalideGenBankParseHeaderNode =
            "InvalideGenBankParseHeader";
        internal const string GenBankRibosomeSiteBindingNode =
            "GenBankRibosomeSiteBinding";

        // XML nodes for Alignment Algorithm
        internal const string SimilarityMatrixFewerLinesException =
            "SimilarityMatrixFewerLinesException";
        internal const string NeedlemanWunschAlignAlgorithmNodeName =
            "NeedlemanWunschAlignAlgorithm";
        internal const string NeedlemanWunschAlignAlgorithmNodeNameFor1000BP =
            "NeedlemanWunschAlignAlgorithmFor1000BP";
        internal const string SmithWatermanAlignAlgorithmNodeName =
            "SmithWatermanAlignAlgorithm";
        internal const string SmithWatermanAlignAlgorithmNodeNameFor1000BP =
            "SmithWatermanAlignAlgorithmFor1000BP";
        internal const string PairwiseOverlapAlignAlgorithmNodeName =
            "PairwiseOverlapAlignAlgorithm";
        internal const string NeedlemanWunschMediumSizeProAlignAlgorithmNodeName =
            "NeedlemanWunschMediumSizeProAlignAlgorithm";
        internal const string NeedlemanWunschLargeSizeProAlignAlgorithmNodeName =
            "NeedlemanWunschLargeSizeProAlignAlgorithm";
        internal const string NeedlemanWunschVeryLargeSizeProAlignAlgorithmNodeName =
            "NeedlemanWunschVeryLargeSizeProAlignAlgorithm";
        internal const string NeedlemanWunschEqualAlignAlgorithmNodeName =
            "NeedlemanWunschEqualAlignAlgorithm";
        internal const string SmithWatermanEqualAlignAlgorithmNodeName =
            "SmithWatermanEqualAlignAlgorithm";
        internal const string PairwiseOverlapEqualAlignAlgorithmNodeName =
            "PairwiseOverlapEqualAlignAlgorithm";
        internal const string AlignAlgorithmNodeName = "AlignAlgorithm";
        internal const string AlignDnaAlgorithmNodeName = "AlignDnaAlgorithm";
        internal const string AlignRnaAlgorithmNodeName = "AlignRnaAlgorithm";
        internal const string AlignProteinAlgorithmNodeName = "AlignProteinAlgorithm";
        internal const string NeedlemanWunschDnaAlignAlgorithmNodeName =
            "NeedlemanWunschDnaAlignAlgorithm";
        internal const string NeedlemanWunschRnaAlignAlgorithmNodeName =
            "NeedlemanWunschRnaAlignAlgorithm";
        internal const string NeedlemanWunschProAlignAlgorithmNodeName =
            "NeedlemanWunschProAlignAlgorithm";
        internal const string NeedlemanWunschBlosumAlignAlgorithmNodeName =
            "NeedlemanWunschBlosumAlignAlgorithm";
        internal const string NeedlemanWunschPamAlignAlgorithmNodeName =
            "NeedlemanWunschPamAlignAlgorithm";
        internal const string NeedlemanWunschGapCostMaxAlignAlgorithmNodeName =
            "NeedlemanWunschGapCostMaxAlignAlgorithm";
        internal const string NeedlemanWunschGapCostMinAlignAlgorithmNodeName =
            "NeedlemanWunschGapCostMinAlignAlgorithm";
        internal const string NeedlemanWunschDiagonalSimMatAlignAlgorithmNodeName =
            "NeedlemanWunschDiagonalSMAlignAlgorithm";

        internal const string SmithWatermanMediumSizeProAlignAlgorithmNodeName =
            "SmithWatermanMediumSizeProAlignAlgorithm";
        internal const string SmithWatermanLargeSizeProAlignAlgorithmNodeName =
            "SmithWatermanLargeSizeProAlignAlgorithm";
        internal const string SmithWatermanVeryLargeSizeProAlignAlgorithmNodeName =
            "SmithWatermanVeryLargeSizeProAlignAlgorithm";
        internal const string SmithWatermanDnaAlignAlgorithmNodeName =
            "SmithWatermanDnaAlignAlgorithm";
        internal const string SmithWatermanRnaAlignAlgorithmNodeName =
            "SmithWatermanRnaAlignAlgorithm";
        internal const string SmithWatermanProAlignAlgorithmNodeName =
            "SmithWatermanProAlignAlgorithm";
        internal const string SmithWatermanBlosumAlignAlgorithmNodeName =
            "SmithWatermanBlosumAlignAlgorithm";
        internal const string SmithWatermanPamAlignAlgorithmNodeName =
            "SmithWatermanPamAlignAlgorithm";
        internal const string SmithWatermanGapCostMaxAlignAlgorithmNodeName =
            "SmithWatermanGapCostMaxAlignAlgorithm";
        internal const string SmithWatermanGapCostMinAlignAlgorithmNodeName =
            "SmithWatermanGapCostMinAlignAlgorithm";
        internal const string SmithWatermanDiagonalSimMatAlignAlgorithmNodeName =
            "SmithWatermanDiagonalSMAlignAlgorithm";

        internal const string PairwiseOverlapMediumSizeProAlignAlgorithmNodeName =
            "PairwiseOverlapMediumSizeProAlignAlgorithm";
        internal const string PairwiseOverlapLargeSizeProAlignAlgorithmNodeName =
            "PairwiseOverlapLargeSizeProAlignAlgorithm";
        internal const string PairwiseOverlapVeryLargeSizeProAlignAlgorithmNodeName =
            "PairwiseOverlapVeryLargeSizeProAlignAlgorithm";
        internal const string PairwiseOverlapDnaAlignAlgorithmNodeName =
            "PairwiseOverlapDnaAlignAlgorithm";
        internal const string PairwiseOverlapRnaAlignAlgorithmNodeName =
            "PairwiseOverlapRnaAlignAlgorithm";
        internal const string PairwiseOverlapProAlignAlgorithmNodeName =
            "PairwiseOverlapProAlignAlgorithm";
        internal const string PairwiseOverlapBlosumAlignAlgorithmNodeName =
            "PairwiseOverlapBlosumAlignAlgorithm";
        internal const string PairwiseOverlapPamAlignAlgorithmNodeName =
            "PairwiseOverlapPamAlignAlgorithm";
        internal const string PairwiseOverlapGapCostMaxAlignAlgorithmNodeName =
            "PairwiseOverlapGapCostMaxAlignAlgorithm";
        internal const string PairwiseOverlapGapCostMinAlignAlgorithmNodeName =
            "PairwiseOverlapGapCostMinAlignAlgorithm";
        internal const string PairwiseOverlapDiagonalSimMatAlignAlgorithmNodeName =
            "PairwiseOverlapDiagonalSMAlignAlgorithm";

        internal const string GapSequenceNode = "GapSequence";
        internal const string GapIndexNode = "GapIndex";
        internal const string ExpectedScoreNode = "ExpectedScore";
        internal const string SequenceNode1 = "Sequence1";
        internal const string SequenceCompareNode = "SequenceCompare";
        internal const string ReplaceNode = "Replace";
        internal const string SequenceNode2 = "Sequence2";
        internal const string BlosumFilePathNode = "BlosumFilePath";
        internal const string BlosumInvalidFilePathNode = "BlosumInvalidFilePath";
        internal const string BlosumEmptyFilePathNode = "BlosumEmptyFilePath";
        internal const string BlosumOnlyAlphabetFilePathNode = "BlosumOnlyAlphabetFilePath";
        internal const string BlosumFewAlphabetsFilePathNode = "BlosumFewAlphabetsFilePath";
        internal const string BlosumModifiedFilePathNode = "BlosumModifiedFilePath";
        internal const string GapOpenCostNode = "GapOpenCost";
        internal const string GapExtensionCostNode = "GapExtensionCost";
        internal const string ExpectedGapExtensionScoreNode = "ExpectedGapExtensionScore";
        internal const string ExpectedGapExtensionSequence1Node = "ExpectedGapExtensionSequence1";
        internal const string ExpectedGapExtensionSequence2Node = "ExpectedGapExtensionSequence2";
        internal const string ExpectedErrorMessage = "ExpectedErrorMessage";
        internal const string InvalidOffsetErrorMessage = "InvalidOffsetErrorMessage";
        internal const string EmptyMatrixErrorMessage = "EmptyMatrixErrorMessage";
        internal const string OnlyAlphabetErrorMessage = "OnlyAlphabetErrorMessage";
        internal const string ModifiedMatrixErrorMessage = "ModifiedMatrixErrorMessage";
        internal const string InvalidFilePathNode1 = "InvalidFilePath1";
        internal const string InvalidSequence1 = "InvalidSequence1";
        internal const string EmptyFilePath1 = "EmptyFilePath1";
        internal const string UnicodeFilePath1 = "UnicodeFilePath1";
        internal const string GapFilePath1 = "GapFilePath1";
        internal const string SpacesFilePath1 = "SpacesFilePath1";
        internal const string SpacesSequence = "SpacesSequence";
        internal const string NullErrorMessage = "NullErrorMessage";
        internal const string EmptySequenceErrorMessage = "EmptySequenceMessage";
        internal const string InvalidSequenceErrorMessage = "InvalidSeqMessage";
        internal const string UnicodeSequenceErrorMessage = "UnicodeSeqMessage";
        internal const string SequenceWithSpaceErrorMessage = "SeqErrorWithSpace";
        internal const string InvalidAlphabetErrorMessage = "AlphabetMapError";
        internal const string AlignedSeqCountAfterAddAlignedSeqNode = "AlignedSeqCountAfterAddAlignedSeq";
        internal const string SeqCountNode = "SeqCount";
        internal const string ArraySizeNode = "ArraySize";
        internal const string ReadOnlyExceptionNode = "ReadOnlyException";
        internal const string GetObjectDataNullErrorMessageNode = "GetObjectNullErrorMessage";

        // XML nodes for Assembly Algorithm
        internal const string AssemblyAlgorithmNodeName = "AssemblyAlgorithm";
        internal const string AssemblySequenceAlgorithmNodeName =
            "AssemblySequenceAlgorithm";
        internal const string AssemblyMaxSequenceAlgorithmNodeName =
            "AssemblyMaxSequenceAlgorithm";
        internal const string AssemblyMinSequenceAlgorithmNodeName =
            "AssemblyMinSequenceAlgorithm";
        internal const string AssemblyMaxThresholdSequenceAlgorithmNodeName =
            "AssemblyMaxThresholdSequenceAlgorithm";
        internal const string AssemblyMinThresholdSequenceAlgorithmNodeName =
            "AssemblyMinThresholdSequenceAlgorithm";
        internal const string SequenceNode3 = "Sequence3";
        internal const string SequencesNode = "Sequences";
        internal const string SequenceNode = "Sequence";
        internal const string SequenceCountNode = "SequenceCount";
        internal const string MatchScoreNode = "MatchScore";
        internal const string MisMatchScoreNode = "MisMatchScore";
        internal const string GapCostNode = "GapCost";
        internal const string MergeThresholdNode = "MergeThreshold";
        internal const string ConsensusThresholdNode = "ConsensusThreshold";
        internal const string ContigConsensusNode = "ContigConsensus";
        internal const string UnMergedSequencesCountNode = "UnMergedSequencesCount";
        internal const string ContigsCountNode = "ContigsCount";
        internal const string ContigSequencesCountNode = "ContigSequencesCount";
        internal const string UseAmbiguityCodesNode = "UseAmbiguityCodes";
        internal const string DocumentaionNode = "Documentaion";
        internal const string SequenceAssemblerPropertiesNode =
            "SequenceAssemblerProperties";
        internal const string NameNode = "Name";
        internal const string OverlapAlgoNameNode = "OverlapAlgoName";
        internal const string Description = "Description";
        internal const string KmerSequenceNode = "KmerSeq";
        internal const string KmrSeqCountNode = "KmerSeqCount";
        internal const string PositionsNode = "KmerPos";
        internal const string KmerbuilderNode = "Kmerbuilder";
        internal const string CompareTwoSequencesNode = "CompareTwoSequences";
        internal const string FeatureCount = "FeatureCount";
        internal const string KmerLengthNode = "KmerLength";
        internal const string FeatureName = "Feature";
        internal const string FeatureType = "FeatureType";
        internal const string EndIndexNode = "EndIndex";
        internal const string StartIndexNode = "StartIndex";
        internal const string CompareTwoProtienSequencesNode =
            "CompareTwoProtienSequences";

        // XML nodes for the Sequence BVT Test cases.
        internal const string SimpleDnaAlphabetNode = "SimpleDNA";
        internal const string SimpleRnaAlphabetNode = "SimpleRNA";
        internal const string SimplePatternNode = "SimplePattern";
        internal const string PatternNode = "Pattern";
        internal const string SeqFileMatchNode =
            "SeqFileMatch";
        internal const string ConvertExpectedPatternCountNode =
            "ConvertExpectedPatternCount";
        internal const string ConvertExpectedPatternSequenceNode =
            "ConvertExpectedPatternSequence";
        internal const string FileMatchesExpectedKeyNode =
            "FileMatchesExpectedKey";
        internal const string FileMatchesExpectedCountNode =
            "FileMatchesExpectedCount";
        internal const string FileMatchesExpectedValueNode =
            "FileMatchesExpectedValue";
        internal const string SimpleProteinPatternNode =
            "SimpleProteinPattern";
        internal const string SimpleDnaPatternNode =
            "SimpleDnaPattern";
        internal const string SimpleRnaPatternNode =
            "SimpleRnaPattern";
        internal const string SimpleProteinAlphabetNode = "SimplePROTEIN";
        internal const string SimpleGeneBankNodeName = "SimpleGenBank";
        internal const string SimpleGeneBankSequenceCount =
            "ExpectedSequenceCount";
        internal const string ExpectedSingleChar = "ExpectedChar";
        internal const string ExpectedNormalString = "ExpectedNormalString";
        internal const string ExpectedSeqAfterAdd = "ExpectedSequenceAfterAdd";
        internal const string ExpectedReverseSequence = "ExpectedRevSequence";
        internal const string DnaDefaultMap = "DNADefaultMap";
        internal const string RnaDefaultMap = "RNADefaultMap";
        internal const string ProteinDefaultMap = "PROTEINDefaultMap";
        internal const string IupacNaEncodedValue = "IupacNaEncodedValue";
        internal const string NcbiStdAAEncodedValue = "NcbiStdAAEncodedValue";
        internal const string Ncbi4NaEncodedValue = "Ncbi4NaEncodedValue";
        internal const string EncodedDnaNormalSequenceCount = "ExpectedSequenceCount";
        internal const string SimpleFastaSequenceCount = "ExpectedSequenceCount";
        internal const string SimpleDecoderNode = "Decoder";
        internal const string Ncbi2NAEncodingDecoderNode = "Ncbi2NADecoder";
        internal const string NcbiStdAAEncodingDecoderNode = "NcbiStdAADecoder";
        internal const string NcbiEAAEncodingDecoderNode = "NcbiEAADecoder";
        internal const string DecoderByteValue = "AByteValue";
        internal const string DecoderAlphabetName = "AlphabetAName";
        internal const string AlphabetSymbol = "AlphabetASymbol";
        internal const string Alphabet4NAEncodingSymbol =
            "Alphabet4NAEncodingSymbol";
        internal const string AlphabetEAAEncodingSymbol =
            "AlphabetEAAEncodingSymbol";
        internal const string AlphabetStdAAEncodingSymbol =
            "AlphabetStdAAEncodingSymbol";
        internal const string SimpleEncoderName = "Encoder";
        internal const string ByteArray = "ByteArray";
        internal const string SequenceByteArray = "SequenceByteArray";
        internal const string ExpectedString = "ExpectedString";
        internal const string StringByteArray = "StringByteArray";
        internal const string EncodingNode = "Encoding";
        internal const string Ncbi2NAEncodingByteValue =
            "Ncbi2NAEncodingByteValue";
        internal const string Ncbi4NAEncodingByteValue =
            "Ncbi4NAEncodingByteValue";
        internal const string NcbiEAAEncodingByteValue =
            "NcbiEAAEncodingByteValue";
        internal const string NcbiStdAAEncodingByteValue =
            "NcbiStdAAEncodingByteValue";
        internal const string LookUpNode = "LookUpSymbol";
        internal const string LookUpInputChar = "InputChar";
        internal const string LookUpOutputChar = "OuputChar";
        internal const string LookUpInputString = "InputString";
        internal const string LookUpOutputString = "OuputString";
        internal const string LookUpInputByte = "InputByte";
        internal const string LookUpOutputSymbol = "OuputSymbol";
        internal const string AminoAcidsListLength = "AminoAcidsListLength";
        internal const string NucleotidesListLength = "NucleotidesListLength";
        internal const string MapNode = "Map";
        internal const string MapToEncoding = "MapToEncoding";
        internal const string MapToAlphabet = "MapToAlphabet";
        internal const string ExpectedSequenceAfterInsert =
            "ExpectedSequenceAfterInsert";
        internal const string ExpectedSequenceAfterInsertAtFirstPosition =
            "ExpectedSequenceAfterInsertAtFirstPosition";
        internal const string ExpectedSequenceAfterRemoveSequenceData =
            "ExpectedSequenceAfterRemoveSequenceData";
        internal const string ExpectedSequenceAfterReplace =
            "ExpectedSequenceAfterReplace";
        internal const string ExpectedDerivedSequence = "DerivedSequence";
        internal const string LargeSizeGenBank = "LargeSizeGenBank";
        internal const string LargeSizeFasta = "LargeSizeFasta";
        internal const string IupacNAEncodingDecoderNode = "IupacNADecoder";
        internal const int LargeMediumUniqueSymbolCount = 4;
        internal const string EncodingSimpleDna = "EncodingSimpleDNA";
        internal const string EncodingSimpleRna = "EncodingSimpleRNA";
        internal const string EncodingSimpleProtein = "EncodingSimplePROTEIN";
        internal const string DnaAlphabetNode = "DnaAlphabetName";
        internal const string RnaAlphabetNode = "RnaAlphabetName";
        internal const string ProteinAlphabetNode = "ProteinAlphabetName";
        internal const string AlphabetsCountNode = "AlphabetsCount";
        internal const string BasicSeqInfoNode = "BasicSeqInfo";
        internal const string SeqPosWithNonGapCharNode = "SeqPosWithNonGapChar";
        internal const string SeqPosWithGapCharNode = "SeqPosWithGapChar";
        internal const string BasicSeqInfoLastIndexGapCharNode =
            "BasicSeqInfoLastIndexGapChar";
        internal const string NullSeqErrorMessageNode = "NullSeqErrorMessage";
        internal const string StartPosErrorNode = "StartPosError";
        internal const string EndPosErrorNode = "EndPosError";
        internal const string GapCharSeqNode = "GapCharSeq";
        internal const string NullSeqInfoErrorMessageNode =
            "NullSeqInfoErrorMessage";

        // XML nodes for the Qualitative Sequence Test cases.
        internal const string ExpectedScore = "Score";
        internal const string FastQFormatType = "FormatType";
        internal const string SolexaTypeNode = "SolexaType";
        internal const string IlluminaTypeNode = "IlluminaType";
        internal const string inputSequenceNode = "InputSequence";
        internal const string SimpleDnaSangerNode = "SimpleDNASanger";
        internal const string SimpleRnaSangerNode = "SimpleRnaSanger";
        internal const string SimpleProteinSangerNode = "SimpleProteinSanger";
        internal const string SimpleDnaIlluminaNode = "SimpleDNAIllumina";
        internal const string SimpleFastQDVIscNodeName = "SimpleFastQDVIsc";
        internal const string SimpleDNAIlluminaByteArrayNode =
            "SimpleDNAIlluminaByteArray";
        internal const string SimpleProteinSolexaNode = "SimpleProteinSolexa";
        internal const string SimpleRnaIlluminaNode = "SimpleRnaIllumina";
        internal const string SimpleProteinIlluminaNode = "SimpleProteinIllumina";
        internal const string SimpleDnaSolexaNode = "SimpleDNASolexa";
        internal const string SimpleRnaSolexaNode = "SimpleRNASolexa";
        internal const string QSequenceCount = "SequenceCount";
        internal const string InputScoreNode = "InputScore";
        internal const string InputByteArrayNode = "InputByteArray";
        internal const string SequenceCountAfterAdd = "SequenceCountAfterAdd";
        internal const string MaxScoreNode = "MaxScore";
        internal const string ExpectedSequenceAfterRemove =
            "ExpectedSequenceAfterRemove";
        internal const string ItemToBeInserted = "InsertItem";
        internal const string ScoreToBeUpdated = "InsertScore";
        internal const string NewScoreToInsert = "NewScoreToInsert";
        internal const string ExpectedSequenceAfterInsertString =
            "ExpectedSequenceAfterInsertString";
        internal const string ExpectedSequenceAfterInsertSeq =
            "ExpectedSequenceAfterInsertSeq";
        internal const string SequenceAferReplace = "SequenceAferReplaceItem";
        internal const string SequenceAfterReplaceSeq =
            "SequenceAfterReplaceSeq";
        internal const string SangerToSolexaAndIlluminaNode =
            "SangerToSolexaAndIllumina";
        internal const string SolexaToSangerAndIlluminaNode =
            "SolexaToSangerAndIllumina";
        internal const string IlluminaToSangerAndSolexaNode =
            "IlluminaToSangerAndSolexa";
        internal const string SangerSequence = "SangerSequence";
        internal const string IlluminaSequence = "IlluminaSequence";
        internal const string SolexaSequence = "SolexaSequence";
        internal const string SangerQualScore = "SangerQualScore";
        internal const string IlluminaQualScore = "IlluminaQualScore";
        internal const string SolexaQualScore = "SolexaQualScore";
        internal const string InsertByteArray = "InsertByteArray";
        internal const string FirstItemIndex = "firstItemIndex";
        internal const string LastItemIndex = "LastIndex";
        internal const string IndexOfGap = "IndexOfGap";
        internal const string QualSeqWithinRange = "SeqWithinRange";
        internal const string SequenceAfterRemoveWithRange =
            "SequenceAfterRemovewithRange";
        internal const string DefualtMaxScore = "DefualtMaxScore";
        internal const string DefaultMinScore = "DefaultMinScore";
        internal const string AlphabetNullExceptionNode =
            "AlphabetNullException";
        internal const string InvalidQualityScore = "InvalidQualityScore";
        internal const string InvalidByteQualScore = "InvalidByteQualScore";
        internal const string EncodingError = "EncodingError";
        internal const string ReadOnlyException = "ReadOnlyException";
        internal const string ReverseQualSeq = "ReverseQualSeq";
        internal const string ComplementException = "ComplementException";
        internal const string MediumSizeDNASolexaNode = "MediumSizeDNASolexa";
        internal const string MediumSizeDNASangerNode = "MediumSizeDNASanger";
        internal const string MediumSizeDNAIlluminaNode = "MediumSizeDNAIllumina";
        internal const string ComplementQualSeqNode = "ComplementQualSeq";
        internal const string LargeSizeDNASolexaNode = "LargeSizeDNASolexa";
        internal const string LargeSizeDNAIlluminaNode = "LargeSizeDNAIllumina";
        internal const string LargeSizeDNASangerNode = "LargeSizeDNASanger";
        internal const string VeryLargeSizeDNAIlluminaNode =
            "VeryLargeSizeDNAIllumina";
        internal const string VeryLargeSizeDNASangerNode =
            "VeryLargeSizeDNASanger";
        internal const string VeryLargeSizeDNASolexaNode =
            "VeryLargeSizeDNASolexa";
        internal const string EncodedValuesNode = "EncodedValues";
        internal const string RevComplement = "RevComplement";
        internal const string ReplaceChar = "ReplaceChar";
        internal const int SequenceLength = 5;
        internal const int StartPosition = 0;
        internal const string NullExceptionError = "NullExceptionError";
        internal const string InvalidScoreErrorNode = "InvalidScoreError";
        internal const string FormatTypeConvertionErrosNode =
            "FormatTypeConvertionErros";
        internal const string InvalidByteScoreErrorNode =
            "InvalidByteScoreError";
        internal const string QualitativeSequenceInsertSeqItemNode =
            "QualitativeSequenceInsertSeqItem";
        internal const string InfoNullErrorNode = "InfoNullError";
        internal const string ExpectedSeqRangeNode = "ExpectedSeqRange";
        internal const string OutOfRangeExceptionNode = "OutOfRangeException";
        internal const string LengthOutOfRangeExceptionNode =
            "LengthOutOfRangeException";
        internal const string SequenceAfterReplaceNode = "SequenceAfterReplace";
        internal const string InvalidItemExceptionNode = "InvalidItemException";
        internal const string InvalidPositionErrorNode = "InvalidPositionError";
        internal const string InvalidSeqNode = "InvalidSeq";

        // XML nodes for the FastQ Parser and Formatter.
        internal const string SimpleIlluminaFastQNode = "SimpleIlluminaFastQ";
        internal const string SimpleIlluminaFqFastQNode =
            "SimpleIlluminaFqFastQ";
        internal const string SimpleSolexaFqFastQNode = "SimpleSolexaFqFastQ";
        internal const string SimpleSangerFastQNode = "SimpleSangerFastQ";
        internal const string SingleSequenceSangerFastQNode =
            "SingleSequenceSangerFastQ";
        internal const string SingleSequenceSolexaFastQNode =
            "SingleSequenceSolexaFastQ";
        internal const string SingleSequenceIlluminaFastQNode =
            "SingleSequenceIlluminaFastQ";
        internal const string FastQTempFileName = "temp.fastq";
        internal const string StreamWriterFastQTempFileName = "StreamTemp.fastq";
        internal const string TextWriterTempFile = "Temp.fastq";
        internal const string FastQTempFqFileName = "temp.fq";
        internal const string FastQTempTxtFileName = "temp.txt";
        internal const string SeqsCount = "SeqCount";
        internal const string SeqLength = "SeqLength";
        internal const string MediumSizeDnaSangerFastQNode = "MediumSizeDnaSangerFastQ";
        internal const string MediumSizeDnaIlluminaFastQNode = "MediumSizeDnaIlluminaFastQ";
        internal const string MediumSizeDnaSolexaFastQNode = "MediumSizeDnaSolexaFastQ";
        internal const string LargeSizeDnaSangerFastQNode = "LargeSizeDnaSangerFastQ";
        internal const string LargeSizeDnaIlluminaFastQNode = "LargeSizeDnaIlluminaFastQ";
        internal const string LargeSizeDnaIlluminaFastQFileNode = "LargeSizeDnaIlluminaFastQFile";
        internal const string LargeSizeDnaSolexaFastQNode = "LargeSizeDnaSolexaFastQ";
        internal const string OneLineDnaIlluminaFastQNode = "SingleSequenceIlluminaFastQ";
        internal const string OneLineDnaSolexaFastQNode = "SingleSequenceSolexaFastQ";
        internal const string TwoLineDnaSangerFastQNode = "TwoLineDnaSanger";
        internal const string TwoLineDnaSolexaFastQNode = "TwoLineDnaSolexa";
        internal const string TwoLineDnaIlluminaFastQNode = "TwoLineDnaIllumina";
        internal const string SimpleRnaSangerFastQNode = "SimpleRnaSanger";
        internal const string SimpleRnaSolexaFastQNode = "SimpleRnaSolexa";
        internal const string SimpleRnaIlluminaFastQNode = "SimpleRnaIllumina";
        internal const string SimpleProteinSangerFastQNode = "SimpleProteinSanger";
        internal const string SimpleProteinSolexaFastQNode = "SimpleProteinSolexa";
        internal const string SimpleProteinIlluminaFastQNode = "SimpleProteinIllumina";
        internal const string MultiSeqSangerDnaRnaNode = "MultiSeqSanger-DNA_RNA";
        internal const string MultiSeqIlluminaDnaRnaNode = "MultiSeqIllumina-DNA_RNA";
        internal const string MultiSeqSolexaDnaRnaNode = "MultiSeqSolexa-DNA_RNA";
        internal const string MultiSeqSangerRnaProNode = "MultiSeqSanger-RNA_PRO";
        internal const string MultiSeqIlluminaRnaProNode = "MultiSeqIllumina-RNA_PRO";
        internal const string MultiSeqSolexaRnaProNode = "MultiSeqSolexa-RNA_PRO";
        internal const string MultiSeqSangerDnaProNode = "MultiSeqSanger-DNA_PRO";
        internal const string MultiSeqIlluminaDnaProNode = "MultiSeqIllumina-DNA_PRO";
        internal const string MultiSeqSolexaDnaProNode = "MultiSeqSolexa-DNA_PRO";
        internal const string MultiSeqSangerDnaRnaProNode = "MultiSeqSanger-DNA_RNA_PRO";
        internal const string MultiSeqIlluminaDnaRnaProNode = "MultiSeqIllumina-DNA_RNA_PRO";
        internal const string MultiSeqSolexaDnaRnaProNode = "MultiSeqSolexa-DNA_RNA_PRO";
        internal const string ExpectedSequence1Node = "ExpectedSequence1";
        internal const string ExpectedSequenceId1Node = "SequenceID1";
        internal const string ExpectedSequence2Node = "ExpectedSequence2";
        internal const string ExpectedSequence3Node = "ExpectedSequence3";
        internal const string SeqFormatString = "SeqFormatString";
        internal const string SecondFormatString = "SecondFormatString";
        internal const string ThirdFormatString = "ThirdFormatString";
        internal const string AlphabetName1Node = "AlphabetName1";
        internal const string AlphabetName1NodeV2 = "AlphabetName1V2";
        internal const string AlphabetName2Node = "AlphabetName2";
        internal const string AlphabetName2NodeV2 = "AlphabetName2V2";
        internal const string AlphabetName3Node = "AlphabetName3";
        internal const string AlphabetName3NodeV2 = "AlphabetName3V2";
        internal const string FastQSequenceWithInvalidSeqIdNode =
            "FastQSequenceWithInvalidSequenceId";
        internal const string FastQParserEmptySequenceNode =
            "FastQSequenceWithEmptySeq";
        internal const string FastQParserWithInvalidQualScoreNode =
            "FastQSequenceWithInvalidQualityScore";
        internal const string FastQParserWithEmptyQualScoreNode =
            "FastQSequenceWithEmptyQualityScore";
        internal const string FastQParserWithEmptyQualScoreAndQualID =
            "FastQSequenceWithEmptyQualityScoreAndSeqId";
        internal const string FastQParserWithInvalidAlphabet =
            "FastQSequenceWithInvalidAlphabet";
        internal const string EmptyFastQFileNode =
            "FastQSequenceWithEmptyFile";
        internal const string FastQDataVirtulizationMultiSequenceFileNode =
            "FastQDataVirtulizationMultiSequenceFile";
        internal const string FastQInvalidFormatFileNode =
            "FastQInvalidFormatFile";
        internal const string FastQInvalidQualScoreFileNode =
            "FastQInvalidQualScoreFile";
        internal const int MaximumDictionaryLength = 102400;
        internal const int VirtualQualSeqCount = 1000;
        internal const int SeqIndexNumber = 10;

        // XML nodes for the Translation BVT Test cases.
        internal const string CodonsNode = "Codons";
        internal const string Nucleotide = "NucleotideTriplet";
        internal const string SequenceWithmoreThanTweleveChars = "SequenceWithmoreThanTweleveChars";
        internal const string SequenceWithSixChars = "SequenceWithmoreSixChars";
        internal const string AminoAcid = "AminoAcid";
        internal const string AminoAcidSymbol = "AminoAcidSymbol";
        internal const string OffsetZeroTwelveCharsAminoAcid = "OffsetZeroTwelveCharsAminoAcid";
        internal const string OffsetZeroTwelveCharsAminoAcidV2 = "OffsetZeroTwelveCharsAminoAcidV2";
        internal const string OffsetZeroSixCharsAminoAcid = "OffsetZeroSixCharsAminoAcid";
        internal const string OffsetZeroSixCharsAminoAcidV2 = "OffsetZeroSixCharsAminoAcidV2";
        internal const string OffsetOneMoreThanTwelveCharsAminoAcid = "OffsetOneMoreThanTwelveCharsAminoAcid";
        internal const string OffsetOneMoreThanTwelveCharsAminoAcidV2 = "OffsetOneMoreThanTwelveCharsAminoAcidV2";
        internal const string DnaSeqAminoAcid = "DnaSeqAminoAcid";
        internal const string DnaSeqAminoAcidV2 = "DnaSeqAminoAcidV2";
        internal const string DnaSeqAminoAcidWithOffsetValueOne = "DnaSeqAminoAcidWithOffsetValueOne";

        internal const string DnaSeqAminoAcidWithOffsetValueOneDna = "DnaSeqAminoAcidWithOffsetValueOneDna";
        internal const string SeqAminoAcid = "SeqAminoAcid";
        internal const string SeqAminoAcidV2 = "SeqAminoAcidV2";
        internal const string OffsetVaule = "OffsetValue";
        internal const string OffsetVaule1 = "OffsetValue1";
        internal const string OffsetVaule2 = "OffsetValue2";
        internal const string OffsetVaule3 = "OffsetValue3";
        internal const string OffsetVaule4 = "OffsetValue4";
        internal const string ComplementNode = "Complementation";
        internal const string DnaSequence = "DnaSequence";
        internal const string RnaSequence = "RnaSequence";
        internal const string AmbiguousRnaSequence = "AmbiguousRnaSequence";
        internal const string AmbiguousProteinSequence = "AmbiguousProteinSequence";
        internal const string DnaComplement = "DnaComplement";
        internal const string DnaRevComplement = "DnaRevComplement";
        internal const string DnaSymbol = "DnaSymbol";
        internal const string DnaSymbolComplement = "DnaSymbolComplement";
        internal const string DnaSymbolRevComplement = "DnaSymbolRevComplement";
        internal const string DnaSequenceWithMoreThanTwelveChars = "DnaSequenceWithMoreThanTwelveChars";
        internal const string DnaSequenceWithMoreThanTwelveCharsV2 = "DnaSequenceWithMoreThanTwelveCharsV2";
        internal const string DnaComplementForMoreThanTwelveChars = "DnaComplementForMoreThanTwelveChars";
        internal const string DnaRevComplementForMoreThanTwelveChars = "DnaRevComplementForMoreThanTwelveChars";
        internal const string TranscribeNode = "Transcribe";
        internal const string Transcribe = "Transcribe";
        internal const string TranscribeV2 = "TranscribeV2";
        internal const string RevTranscribe = "RevTranscribe";
        internal const string RevTranscribeV2 = "RevTranscribeV2";
        internal const string TranslationNode = "Translation";
        internal const string DnaSymbolTranscribe = "DnaSymbolTranscribe";
        internal const string DnaSymbolTranscribeV2 = "DnaSymbolTranscribeV2";
        internal const string DnaSymbolRevTranscribe = "DnaSymbolRevTranscribe";
        internal const string DnaTranscribeForMoreThanTwelveChars = "DnaTranscribeForMoreThanTwelveChars";
        internal const string DnaTranscribeForMoreThanTwelveCharsV2 = "DnaTranscribeForMoreThanTwelveCharsV2";
        internal const string DnaRevTranscribeForMoreThanTwelveChars = "DnaRevTranscribeForMoreThanTwelveChars";
        internal const string RnaSequenceWithTwelveChars = "RnaSequenceWithTwelveChars";
        internal const string AminoAcidForTwelveChars = "AminoAcidForTwelveChars";
        internal const string RnaSequenceWithMoreThanTwelveChars = "RnaSequenceWithMoreThanTwelveChars";
        internal const string AminoAcidForMoreThanTwelveChars = "AminoAcidForMoreThanTwelveChars";
        internal const string AminoAcidForSixChars = "AminoAcidForSixChars";
        internal const string AminoAcidForSixCharsV2 = "AminoAcidForSixCharsV2";
        internal const string AminoAcidForMoreOffsetThree = "AminoAcidForMoreOffsetThree";
        internal const string AminoAcidForOffsetSix = "AminoAcidForOffsetSix";
        internal const string AminoAcidForOffsetTwelve = "AminoAcidForOffsetTwelve";
        internal const string AminoAcidForOffsetTwelveV2 = "AminoAcidForOffsetTwelveV2";
        internal const string AminoAcidForOffsetTwelveDna = "AminoAcidForOffsetTwelveDna";
        internal const string ExpectedSequence = "ExpectedSequence";
        internal const string RnaSequenceWithThreeChars = "RnaSequenceWithThreeChars";
        internal const string AminoAcidForThreeChars = "AminoAcidForThreeChars";
        internal const string AminoAcidForThreeCharsV2 = "AminoAcidForThreeCharsV2";
        internal const string DnaSequenceWithThreeChars = "DnaSequenceWithThreeChars";
        internal const string DnaSequenceWithThreeCharsV2 = "DnaSequenceWithThreeCharsV2";
        internal const string TranscribeForThreeChars = "TranscribeForThreeChars";
        internal const string TranscribeForThreeCharsV2 = "TranscribeForThreeCharsV2";
        internal const string DnaSequenceWithTweleveChars = "DnaSequenceWithTweleveChars";
        internal const string DnaSequenceWithTweleveCharsV2 = "DnaSequenceWithTweleveCharsV2";
        internal const string TranscribeForTweleveChars = "TranscribeForTweleveChars";
        internal const string TranscribeForTweleveCharsV2 = "TranscribeForTweleveCharsV2";

        // XML nodes for the WebService BVT Test cases.
        internal const string BlastParametersNode = "BlastParameters";
        internal const string BlastProteinSequenceParametersNode = "BlastParameters";
        internal const string BlastDnaSequenceParametersNode = "BlastDnaSequenceParameters";
        internal const string QuerySequencyparameter = "QueryParameter";
        internal const string DatabaseParameter = "DatabaseParameter";
        internal const string ProgramParameter = "ProgramParameter";
        internal const string QuerySequency = "SequenceQuery";
        internal const string DatabaseValue = "DatabaseValue";
        internal const string ProgramValue = "ProgramValue";
        internal const string SimpleBlastXmlNode = "SimpleBlastXml";
        internal const string BlastResultfilePath = "FilePath";
        internal const string ResultsCount = "ResultsCount";
        internal const string HitAccession = "HitAccession";
        internal const string ParameterGap = "ParameterGap";
        internal const string ParameterMatrix = "ParameterMatrix";
        internal const string BitScore = "BitScore";
        internal const string AlignmentLength = "AlignmentLength";
        internal const string DatabaseLength = "DatabaseLength";
        internal const string HitSequence = "HitSequence";
        internal const string HitsCount = "HitsCount";
        internal const string BlastRequestParametersNode = "BlastRequestParameters";
        internal const string BlastWebServiceUri = "blastWebSerive";
        internal const string AddparameterNode = "AddParameter";
        internal const string NewParameter = "NewParameterName";
        internal const string AsynchronousResultsNode = "AsynchronousResults";
        internal const string KappaStatistics = "KappaStatics";
        internal const string EntropyStatistics = "EntropyStatucs";
        internal const string LambdaStatistics = "LambdaStatics";
        internal const string HitID = "HitID";
        internal const string BlastDataBaseNode = "BlastDatabaseValues";
        internal const string DefaultDatabaseValue = "DfaultDatabaseValue";
        internal const string SwissprotDBValue = "SwissprotDBValue";
        internal const string PatndedProteinSequence = "PatndedProteinSequence";
        internal const string ProteinDatabankProteins = "ProteinDatabankProteins";
        internal const string BlastProgramNode = "BlastProgramValues";
        internal const string blastnValue = "blastnValue";
        internal const string blastpValue = "blastpValue";
        internal const string blastxValue = "blastxValue";
        internal const string tblastnValue = "tblastnValue";
        internal const string tblastx = "tblastx";
        internal const string optionalValue = "ExpectValue";
        internal const string Expectparameter = "ExpectParameter";
        internal const string BlastMediumSizeDnaSequenceParametersNode = "BlastMediumSizeDnaSequenceParameters";
        internal const string BlastMediumSizeProteinSequenceParametersNode = "BlastMediumSizeProteinSequenceParameters";
        internal const string BlastRequestNodeWithBlastxProgramParameterNode = "BlastParametersWithBlastXProgram";
        internal const string BlastRequestNodeWithtBlastxProgramParameter = "BlastParametersWithtBlastXProgram";
        internal const string BlastRequestNodeWithtTBlastnProgramParameter = "BlastParametersWithTBlastNProgram";
        internal const string BlastRequestNodeWithBlastnProgramParameter = "BlastParametersWithBlastNProgram";
        internal const string BlastRequestNodeWithtSwissprotDataseParameter = "BlastProteinSequenceWithSwissprotDatabase";
        internal const string BlastProteinSequenceWithPatDatabaseParameter = "BlastProteinSequenceWithPatDatabase";
        internal const string BlastDnaSequenceWithAmpersandLetter = "BlastDnaSequenceWithampCharParameters";
        internal const string BlastProteinSequenceWithAmpersandLetter = "BlastProteinSequenceWithampcharParameters";
        internal const string expectedParameterCount = "ExpectedCount";
        internal const string BlastExpectParameter = "ExpectParameter";
        internal const string BlastExpectparameterValue = "ExpectParameterValue";
        internal const string BlastSequenceOptionalParameter = "BlastProteinSequenceWithOptionalParameters";
        internal const string BlastCompositionBasedStaticsParameter = "CompositionBasedStaticsParameter";
        internal const string BlastCompositionBasedStaticsValue = "CompositionBasedStaticsParameterValue";
        internal const string MediumSizedBlastXmlNode = "MediumSizedBlastXml";
        internal const string LargeSizedBlastXmlNode = "LargeSizedBlastXml";
        internal const string BlastResultWithMoreThanThreeRecords = "BlastResultWithMoreThanThreeRecords";
        internal const string ProteinFetchResults = "ProteinAsynchronousResults";
        internal const string ProteinFetchResultsWithSwissprot = "ProteinAsynchronousResultsWithSwissprot";
        internal const string ProteinFetchResultsWithPatDatabaseNode = "ProteinAsynchronousResultsWithPatDatabase";
        internal const string ProteinFetchResultsWithPdbDatabase = "ProteinAsynchronousResultsWithPdbDatabase";
        internal const string DnaSeqFetchResultsWithBlastnProgramNode = "DnaSeqAsynchronousResultsWithBlastn";
        internal const string DnaSeqFetchResultsWithBlastxProgramNode = "DnaSeqAsynchronousResultsWithBlastx";
        internal const string ProteinSeqFetchResultsWithtBlastnNode = "DnaSeqAsynchronousResultsWithtBlastn";
        internal const string DnaSeqFetchResultsWithtBlastxNode = "DnaSeqAsynchronousResultsWithtBlastx";
        internal const string DnaSeqAFetchResultsWithMandatoryFieldsOnlyNode = "DnaSeqAsynchronousResultsWithMandatoryFieldsOnly";
        internal const string DnaSeqFetchResultsWithOptionalParameters = "DnaSeqAsynchronousResultsWithOptionalParameters";
        internal const string ConfigurationParametersNode = "ConfigurationParameters";
        internal const string EmailAdress = "EmailAddress";
        internal const string EmailAdress1 = "EmailAdress1";
        internal const string EmailAdress2 = "EmailAdress1";
        internal const string Emailparameter = "Emailparameter";
        internal const string RetryCount = "RetryCount";
        internal const string RetryInterval = "RetryInterval";
        internal const string DefaultTimeOut = "DefaultTimeOut";
        internal const string BlastWithMoreThanTenRecordsNode = "BlastWithMoreThanTenRecords";
        internal const string DoubleRangeParameterNode = "DoubleRangeParameter";
        internal const string IntRangeParameterNode = "IntRangeParameter";
        internal const string newParameterValue = "NewParameterNameValue";
        internal const string lowestRange = "LowestValue";
        internal const string highestRange = "HighestValue";
        internal const string AzureBlastResultsNode = "AzureResults";
        internal const string DnaSeqAsynchronousResultsWithtBlastxNode = "DnaSeqAsynchronousResultsWithtBlastx";
        internal const string AluDatabaseParametersNode = "AluDatabaseParameters";
        internal const string DefaultDatabaseParameters = "DefaultDatabaseParameters";
        internal const string RnaAzureResultsNode = "RnaAzureResults";
        internal const string HspHitsCount = "HspHitsCount";
        internal const string SleepTime = "SleepTime";
        internal const int Offset = 3;
        internal const string AzureWebServiceDescription = "Azure Blast hosted by Cloud Computing";
        internal const string AzureWebServiceName = "Azure BLAST";
        internal const string NcbiWebServiceDescription = "The QBLAST Web Service hosted by NCBI (www.ncbi.nlm.nih.gov)";
        internal const string NcbiWebServiceName = "NCBI QBLAST";
        internal const string NcbiDnaSeqAsynchronousResultsNode = "NcbiDnaSeqAsynchronousResults";
        internal const string NcbiBlastMediumSizeEbiDnaSequenceParametersNode = "NcbiBlastMediumSizeEbiDnaSequenceParameters";
        internal const string NcbiBlastMediumSizeEbiProteinSequenceParametersNode = "NcbiBlastMediumSizeEbiProteinSequenceParameters";
        internal const string EmRelNcbiDatabaseParametersNode = "EmRelNcbiDatabaseParameters";
        internal const string BlastParmsConst = "BLASTPARAMETERS";
        internal const string QuerySeqString = "QUERYSEQ";
        internal const string AlphabetString = "ALPHABET";
        internal const string EmailString = "EMAIL";
        internal const string GetRequestStatusInfoForProteinSeqTest = "GetRequestStatusInfoForProteinSeq";
        internal const string GetRequestStatusInfoForDnaSeqTest = "GetRequestStatusInfoForDnaSeq";
        internal const string FetchResultsSyncTestNode = "GetRequestStatusInfoForProteinSeq";
        internal const string FetchResultASyncTestNode = "FetchResultASyncTest";
        internal const string EbiGetRequestStatusInfoForProteinSeqTest = "EbiGetRequestStatusInfoForProteinSeq";
        internal const string EbiGetRequestStatusInfoForDnaSeqNode = "EbiGetRequestStatusInfoForDnaSeq";
        internal const string EBiFetchResultSyncTestNode = "EBiFetchResultSyncTest";
        internal const string EBiFetchResultASyncTestNode = "EBiFetchResultASyncTest";
        internal const string AzureWebServiceRequestStatusForDnaTest = "AzureWebServiceRequestStatusForDna";
        internal const string AzureWebServiceFetchResultsAsyncForDna = "AzureWebServiceFetchResultsAsyncForDna";
        internal const string AzureWebServiceFetchResultsAsyncForProtein = "AzureWebServiceFetchResultsAsyncForProtein";
        internal const string AzureWebServiceFetchResultsSyncForDnaTest = "AzureWebServiceFetchResultsSyncForDna";
        internal const string AzureWebServiceCancelSubmitRequestTest = "AzureWebServiceCancelSubmitRequest";

        // Gff Parsers & Formatters nodes
        internal const string SimpleGffNodeName = "SimpleGff";
        internal const string SimpleGffProteinNodeName = "SimpleGffProtein";
        internal const string SimpleGffDnaNodeName = "SimpleGffDna";
        internal const string SimpleGffRnaNodeName = "SimpleGffRna";
        internal const string MultiSeqRnaProGffNodeName = "MultiSeqRnaProGff";
        internal const string MultiSeqDnaProGffNodeName = "MultiSeqDnaProGff";
        internal const string MultiSeqDnaRnaProGffNodeName = "MultiSeqDnaRnaProGff";
        internal const string LargeSizeGffNodeName = "LargeSizeGff";
        internal const string MediumSizeGffNodeName = "MediumSizeGff";
        internal const string MaxSequenceGffNodeName = "MaxSequenceGff";
        internal const string OneLineSeqGffNodeName = "OneLineSeqGff";
        internal const string OnlyFeaturesGffNodeName = "OnlyFeaturesGff";
        internal const string MultiSeqDnaRnaGffNodeName = "MultiSeqDnaRnaGff";
        internal const string SequenceNameNodeName = "SequenceName";
        internal const string SequenceNameItemNode = "SequenceNameItem";
        internal const string SourceNodeName = "Source";
        internal const string SourceItemNode = "SourceItem";
        internal const string FeatureNameNodeName = "FeatureName";
        internal const string FeatureItemNode = "FeatureItem";
        internal const string StartNodeName = "Start";
        internal const string StartItemNode = "StartItem";
        internal const string EndNodeName = "End";
        internal const string EndItemNode = "EndItem";
        internal const string ScoreNodeName = "Score";
        internal const string ScoreItemNode = "ScoreItem";
        internal const string StrandNodeName = "Strand";
        internal const string StrandItemNode = "StrandItem";
        internal const string FrameNodeName = "Frame";
        internal const string FrameItemNode = "FrameItem";
        internal const string AttributesNodeName = "Attributes";
        internal const string AttributeItemNode = "AttributeItem";
        internal const string ExpectedSequenesNode = "ExpectedSequenes";
        internal const string AlphabetsNode = "Alphabets";
        internal const string SequenceIdsNode = "SequenceIds";
        internal const string SimpleGffFeaturesNode =
            "SimpleGffFeatures";
        internal const string SimpleGffFeaturesReaderNode =
            "SimpleGffFeaturesReader";
        internal const string InvalidateSpecificSeqsNode =
            "InvalidateSpecificSeqs";
        internal const string InvalidFeatureLengthNode =
            "InvalidFeatureLength";
        internal const string InvalidateStartFeatureNode =
            "InvalidateStartFeature";
        internal const string InvalidateEndFeatureNode =
            "InvalidateEndFeature";
        internal const string InvalidateScoreFeatureNode =
            "InvalidateScoreFeature";
        internal const string InvalidateStrandFeatureNode =
            "InvalidateStrandFeature";
        internal const string InvalidateFrameFeatureNode =
            "InvalidateFrameFeature";
        internal const string InvalidateNullFeatureNode =
            "InvalidateNullFeature";
        internal const string InvalidateParseHeaderDateFormateNode =
            "InvalidateParseHeaderDateFormate";
        internal const string InvalidateParseHeaderFieldLenNode =
            "InvalidateParseHeaderFieldLen";
        internal const string InvalidateParseHeaderProteinNode =
            "InvalidateParseHeaderProtein";
        internal const string InvalidateParseHeaderVersionNode =
            "InvalidateParseHeaderVersion";

        internal const string SequenceNameNode1Name = "SequenceName1";
        internal const string SourceNode1Name = "Source1";
        internal const string FeatureNameNode1Name = "FeatureName1";
        internal const string StartNode1Name = "Start1";
        internal const string EndNode1Name = "End1";
        internal const string ScoreNode1Name = "Score1";
        internal const string StrandNode1Name = "Strand1";
        internal const string FrameNode1Name = "Frame1";
        internal const string AttributesNode1Name = "Attributes1";
        internal const string SequenceNameNode2Name = "SequenceName2";
        internal const string SourceNode2Name = "Source2";
        internal const string FeatureNameNode2Name = "FeatureName2";
        internal const string StartNode2Name = "Start2";
        internal const string EndNode2Name = "End2";
        internal const string ScoreNode2Name = "Score2";
        internal const string StrandNode2Name = "Strand2";
        internal const string FrameNode2Name = "Frame2";
        internal const string AttributesNode2Name = "Attributes2";
        internal const string SequenceNameNode3Name = "SequenceName3";
        internal const string SourceNode3Name = "Source3";
        internal const string FeatureNameNode3Name = "FeatureName3";
        internal const string StartNode3Name = "Start3";
        internal const string EndNode3Name = "End3";
        internal const string ScoreNode3Name = "Score3";
        internal const string StrandNode3Name = "Strand3";
        internal const string FrameNode3Name = "Frame3";
        internal const string AttributesNode3Name = "Attributes3";

        internal const string SequenceNameNode4Name = "SequenceName4";
        internal const string SourceNode4Name = "Source4";
        internal const string FeatureNameNode4Name = "FeatureName4";
        internal const string StartNode4Name = "Start4";
        internal const string EndNode4Name = "End4";
        internal const string ScoreNode4Name = "Score4";
        internal const string StrandNode4Name = "Strand4";
        internal const string FrameNode4Name = "Frame4";
        internal const string AttributesNode4Name = "Attributes4";

        internal const string Features = "features";
        internal const string FeatureStart = "start";
        internal const string FeatureSource = "source";
        internal const string FeatureEnd = "end";
        internal const string FeatureScore = "score";
        internal const string FeatureStrand = "strand";
        internal const string FeatureFrame = "frame";
        internal const string GffTempFileName = "temp.gff";

        // Virtual Sequence nodes.
        internal const string DnaVirtualSeqNode = "DnaVSequence";
        internal const string DisplayId = "DisplayId";
        internal const string Id = "Id";
        internal const string ExpectedVSeqCount = "ExpectedCount";
        internal const string IndexValue = "Index";
        internal const string Documentaion = "Documentaion";
        internal const string RnaVirtualSeqNode = "RnaVSequence";
        internal const string ProteinVirtualSeqNode = "ProteinVSequence";
        internal const string ExceptionMessage = "ExceptionMessage";
        internal const string Clonable = "Clonable";
        internal const string VirtualSequence = "VirtualSequence";
        internal const string MoleculeType = "MoleculeType";
        internal const string SeqInfo = "seqInfo";
        internal const string BioBasicSequenceInfo = "Bio.BasicSequenceInfo";
        internal const string Documentation = "Documentation";

        //Sparse Sequence nodes
        internal const string DnaSparseSequenceNode = "DnaSparseSequence";
        internal const string SparseNullExceptionMessage = "SparseNullException";
        internal const string NegativeIndexErrorMessage = "NegativeIndexError";
        internal const string AlphabetNullExceptionMessage = "AlphabetNullException";
        internal const string SymbolExceptionMessage = "SymbolException";
        internal const string ReadOnlyExceptionMessage = "ReadOnlyExceptionMessage";
        internal const string Complement = "Complement";
        internal const string Reverse = "Reverse";
        internal const string ReverseComplement = "ReverseComplement";
        internal const string IndexError = "IndexError";
        internal const string InvalidSequenceCountError = "InvalidSequenceCountError";
        internal const string NullArrayException = "NullArrayException";
        internal const string OutOfIndexException = "OutOfIndexException";
        internal const string SeqPositionError = "SeqPositionError";
        internal const string InvalidStartIndex = "InvalidStartIndex";
        internal const string InvalidLengthError = "InvalidLengthError";
        internal const string InvalidPositionError = "InvalidPositionError";
        internal const string SequenceSymbolException = "PSymbolException";
        internal const string ToStringException = "ToStringException";

        // Segmented Sequence nodes.
        internal const string DnaSegSequenceNode = "DnaSegSequence";
        internal const string DnaSegSequenceListNode = "DnaSegSequenceList";
        internal const string SegmetedSeqCount = "SegmetedSeqCount";
        internal const string SequencesCount = "SequencesCount";
        internal const string RnaSegSequenceNode = "RnaSegSequence";
        internal const string RnaSegSequenceListNode = "RnaSegSequenceList";
        internal const string ProteinSegSequenceNode = "ProteinSegSequence";
        internal const string ProteinSegSequenceListNode = "ProteinSegSequenceList";
        internal const string Sequence1 = "Sequence1";
        internal const string Sequence2 = "Sequence2";
        internal const string Sequence3 = "Sequence3";
        internal const string Sequence4 = "Sequence4";
        internal const string Sequence5 = "Sequence5";
        internal const string Sequence6 = "Sequence6";
        internal const string Sequence7 = "Sequence7";
        internal const string Sequence8 = "Sequence8";
        internal const string Sequence9 = "Sequence9";
        internal const string Sequence10 = "Sequence10";
        internal const string ExpectedSequenceCountAfterSeqAdd = "ExpectedSequenceCountAfterAdd";
        internal const string SmallSizeDnaSegSequenceListNode = "SmallSizeDnaSegSequenceList";
        internal const string SmallSizeRnaSegSequenceListNode = "SmallSizeRnaSegSequenceList";
        internal const string SmallSizeProteinSegSequenceNode = "SmallSizeProteinSegSequence";
        internal const string SmallSizeDnaSegSequenceNode = "SmallSizeDnaSegSequence";
        internal const string SmallSizeRnaSegSequencetNode = "SmallSizeRnaSegSequence";
        internal const string SmallSizeProteinSegSequenceListNode = "SmallSizeProteinSegSequenceList";
        internal const string MediumSizeDnaSegSequenceListNode = "MediumSizeDnaSegSequenceList";
        internal const string MediumSizeRnaSegSequenceListNode = "MediumSizeRnaSegSequenceList";
        internal const string MediumSizeProteinSegSequenceNode = "MediumSizeProteinSegSequence";
        internal const string MediumSizeDnaSegSequenceNode = "MediumSizeDnaSegSequence";
        internal const string MediumSizeRnaSegSequencetNode = "MediumSizeRnaSegSequence";
        internal const string MediumSizeProteinSegSequenceListNode = "MediumSizeProteinSegSequenceList";
        internal const string ExpectedIndexValue = "ExpectedIndexValue";
        internal const string ExpectedInsertSeq = "ExpectedInsertSeq";
        internal const string InsertSeq = "InsertSeq";
        internal const string RangeSeq = "RangeSeq";
        internal const string SequenceAfterRemove = "SequenceAfterRemove";
        internal const string SeqAfterRemoveSeqRange = "SeqAfterRemoveSeqRange";
        internal const string SequenceAfterReplace = "SequenceAfterReplace";
        internal const string SequenceAfterReplaceRange = "SequenceAfterReplaceRange";
        internal const string FirstSequence = "DnaSequence";
        internal const string SecondSegSequence = "RnaSequence";
        internal const string AlphabetException = "AlphabetException";
        internal const string InvalidRnaSegSequenceNode = "InvalidRnaSegSequence";
        internal const string InvalidDnaSegSequenceNode = "InvalidDnaSegSequence";
        internal const string DnaSegSequenceForErrorValidationNode = "DnaSegSequenceForErrorValidation";
        internal const string ProteinSegSequenceForErrorValidationNode = "ProteinSegSequenceForErrorValidation";
        internal const string InvalidSeqItemError = "InvalidSeqItemError";
        internal const string SeqReadOnlyException = "ReadOnlyException";
        internal const string SegSeqNullArrayException = "NullArrayException";
        internal const string ArrayBoundException = "ArrayBoundException";
        internal const string NullSequenceItemException = "NullSequenceItemException";
        internal const string NullSequenceException = "NullSequenceException";
        internal const string SymbolACountNode = "SymbolACount";
        internal const string SequenceWithGapCharsNode = "SequenceWithGapChars";

        // Ebi Blast Web Service nodes.
        internal const string EbiAsynchronousResultsNode = "EbiAsynchronousResults";
        internal const string EBlastDnaSequenceParameters = "EBlastDnaSequenceParameters";
        internal const string EBlastRnaSequenceParametersNode = "EBlastRnaSequenceParameters";
        internal const string EbiSynchronousResults = "EbiSynchronousResults";
        internal const string EBlastRequestParametersNode = "EBlastRequestParameters";
        internal const string EbiBlastParametersNode = "EbiBlastParameters";
        internal const string StrandParameter = "StrandParameter";
        internal const string Length = "Length";
        internal const string StrandParameterValue = "StrandParameterValue";
        internal const string SensitivityParameter = "SensitivityParameter";
        internal const string SensitivityParameterValue = "SensitivityParameterValue";
        internal const string EBlastMediumSizeDnaSequenceParametersNode = "EBlastMediumSizeDnaSequenceParameters";
        internal const string EbiBlastMediumSizeProteinSequenceParametersNode = "EbiBlastMediumSizeProteinSequenceParameters";
        internal const string EbiBlastParametersWithBlastXNode = "EbiBlastParametersWithBlastX";
        internal const string EBlastDnaSequenceParametersWithTblastxNode = "EBlastDnaSequenceParametersWithTblastx";
        internal const string EBlastDnaSequenceParametersWithBlastNNode = "EBlastDnaSequenceParametersWithBlastN";
        internal const string EbiBlastParametersWithSwissprotNode = "EbiBlastParametersWithSwissprot";
        internal const string EbiBlastParametersWithEPOPDatabaseNode = "EbiBlastParametersWithEPOPDatabase";
        internal const string EbiBlastParametersWithJPOPDatabaseNode = "EbiBlastParametersWithJPOPDatabase";
        internal const string EBlastRequestParametersWithMediumSizeDnaNode = "EBlastRequestParametersWithMediumSizeDna";
        internal const string EbiRnaAsynchronousResultsNode = "EbiRnaAsynchronousResults";
        internal const string EbiProteinAsynchronousResultsNode = "EbiProteinAsynchronousResults";
        internal const string EbiProteinAsynchronousResultsWithUniprotNode = "EbiProteinAsynchronousResultsWithUniprot";
        internal const string EbiProteinAsynchronousResultsWithEPOPDataBase = "EbiProteinAsynchronousResultsWithEPOPDataBase";
        internal const string EbiProteinAsynchronousResultsWithJPOPDataBase = "EbiProteinAsynchronousResultsWithJPOPDataBase";
        internal const string EbiProteinAsynchronousResultsWithKIPOPDataBase = "EbiProteinAsynchronousResultsWithKPOPDataBase";
        internal const string EbiProteinAsynchronousResultsBlastpProgram = "EbiProteinAsynchronousResultsBlastp";
        internal const string EbiAsynchronousResultsWithBlastn = "EbiAsynchronousResultsWithBlastn";
        internal const string EbiRnaAsynchronousResultsWithBlastN = "EbiRnaAsynchronousResultsWithBlastN";
        internal const string EbiAsynchronousResultsWithEBIDataBaseNode = "EbiAsynchronousResultsWithEBIDataBase";
        internal const string EbiProteinAsynchronousResultsWithSwissprotNode = "EbiProteinAsynchronousResultsWithSwissprot";
        internal const string EbiAsynchronousResultsWithOptionalparametersNode = "EbiAsynchronousResultsWithOptionalparameters";
        internal const string EbiAsynchronousResultsWithBTblastnNode = "EbiAsynchronousResultsWithBTblastn";
        internal const string EbiDnaSeqAsynchronousResultsNode = "EbiDnaSeqAsynchronousResults";
        internal const string EbiBlastMediumSizeEbiDnaSequenceParametersNode = "EbiBlastMediumSizeEbiDnaSequenceParameters";
        internal const string EbiBlastMediumSizeEbiProteinSequenceParametersNode = "EbiBlastMediumSizeEbiProteinSequenceParameters";
        internal const string EbiWebServiceDescription = "The WU-BLAST Web Service hosted by EBI (www.ebi.ac.uk)";
        internal const string EbiWebServiceName = "EBI WU-BLAST";
        internal const string EmRelDatabaseParametersNode = "EmRelDatabaseParameters";
        internal const string MetadataDatabases = "MetadataDatabases";
        internal const string MetadataFilter = "MetadataFilter";
        internal const string MetadataMatrices = "MetadataMatrices";
        internal const string MetadataPrograms = "MetadataPrograms";
        internal const string MetadataSensitivity = "MetadataSensitivity";
        internal const string MetadataSort = "MetadataSort";
        internal const string MetadataStatistics = "MetadataStatistics";
        internal const string MetadataXmlFormats = "MetadataXmlFormats";
        internal const string EbiBlastResultsNode = "EbiResults";

        // Ebi Blast Web Service nodes.
        internal const string BioHPCWebServiceDescription =
            "BLAST service hosted at BioHPC installation at CBSU (cbsuapps.tc.cornell.edu)";
        internal const string BioHPCWebServiceName =
            "BLAST @ BioHPC";
        internal const string BioHPCAsynchronousResultsForDnaNode =
            "BioHPCAsynchronousResultsForDna";
        internal const string BioHPCAsynchronousResultsForProteinNode =
            "BioHPCAsynchronousResultsForProtein";
        internal const string BioHPCBlastParametersNode =
            "BioHPCBlastParameters";
        internal const string BioHPCBlastDnaSequenceParametersNode =
            "BioHPCBlastDnaSequenceParameters";
        internal const string BioHPCBlastProteinSequenceParametersNode =
            "BioHPCBlastProteinSequenceParameters";
        internal const string BioHPCBlastDnaSequenceCancelParametersNode =
            "BioHPCBlastDnaSequenceCancelParameters";
        internal const string BioHPCBlastProteinSequenceCancelParametersNode =
            "BioHPCBlastProteinSequenceCancelParameters";
        internal const string BioHPCAsynchronousResultsNode =
            "BioHPCAsynchronousResults";
        internal const string MaxAttemptsNode =
            "MaxAttempts";
        internal const string WaitTimeNode =
            "WaitingTime";
        internal const string ExpectNode =
            "Expect";
        internal const string EmailNotifyNode =
            "EmailNotify";
        internal const string JobNameNode =
            "JobName";
        internal const string EmailNotifyParameterNode =
            "EmailNotifyParameter";
        internal const string JobNameParameterNode =
            "JobNameParameter";
        internal const string BioHPCGetRequestStatusInfoForProteinSeqTest =
            "BioHPCGetRequestStatusInfoForProteinSeq";
        internal const string BioHPCGetRequestStatusInfoForDnaSeqTest =
           "BioHPCGetRequestStatusInfoForDnaSeq";
        internal const string BioHPCRequestIdentifierForDnaSeqTest =
           "BioHPCRequestIdentifierForDnaSeq";
        internal const string BioHPCRequestIdentifierForProteinSeqTest =
           "BioHPCRequestIdentifierForProteinSeq";
        internal const string BioHPCFetchResultsSyncTest =
            "BioHPCFetchResultsSync";
        internal const string BioHPCFetchResultsASyncTest =
           "BioHPCFetchResultsASync";
        internal const string MaxAttemptString =
            "MAXATTEMPT";
        internal const string WaitTimeString = "WAITTIME";
        internal const string FetchResultsUsingProteinSeq = "BioHPCFetchResultsUsingProteinSeq";
        internal const string FetchResultsSyncUsingProteinSeq = "BioHPCFetchResultsSyncUsingProteinSeq";
        internal const string BioHPCCancelRequestUsingProteinSeq = "BioHPCCancelRequestUsingProteinSeq";
        internal const string BioHPCCancelRequestUsingDnaSeq = "BioHPCCancelRequestUsingDnaSeq";

        // MUMmer node names
        internal const string OneLineSequenceNodeName = "OneLineSequence";
        internal const string SmallSizeSequenceNodeName = "SmallSizeSequence";
        internal const string MediumSizeSequenceNodeName = "MediumSizeSequence";
        internal const string LargeSizeSequenceNodeName = "LargeSizeSequence";
        internal const string OneLineTwoMatchSequenceNodeName = "OneLineTwoMatchSequence";
        internal const string OneLineTwoMatchOverlapSequenceNodeName = "OneLineTwoMatchOverlapSequence";
        internal const string OneLineRepeatingCharactersNodeName = "OneLineRepeatingCharacters";
        internal const string OneLineAlternateRepeatingCharactersNodeName = "OneLineAlternateRepeatingCharacters";
        internal const string OneLineOnlyRepeatingCharactersNodeName = "OneLineOnlyRepeatingCharacters";
        internal const string DnaSearchAmbiguitySequenceNodeName = "DnaSearchAmbiguitySequence";
        internal const string DnaQueryAmbiguitySequenceNodeName = "DnaQueryAmbiguitySequence";
        internal const string RnaSearchAmbiguitySequenceNodeName = "RnaSearchAmbiguitySequence";
        internal const string RnaQueryAmbiguitySequenceNodeName = "RnaQueryAmbiguitySequence";
        internal const string ProteinSearchAmbiguitySequenceNodeName = "ProteinSearchAmbiguitySequence";
        internal const string ProteinQueryAmbiguitySequenceNodeName = "ProteinQueryAmbiguitySequence";
        internal const string DnaQueryDnaRnaSequenceNodeName = "DnaQueryDnaRnaSequence";
        internal const string RnaQueryRnaProteinSequenceNodeName = "RnaQueryRnaProteinSequence";
        internal const string ProteinQueryDnaProteinSequenceNodeName = "ProteinQueryDnaProteinSequence";
        internal const string OneLineMoreThanTwoMatchSequenceNodeName = "OneLineMoreThanTwoMatchSequence";
        internal const string OneLineMoreThanTwoMatchOverlapSequenceNodeName = "OneLineMoreThanTwoMatchOverlapSequence";
        internal const string OneLineMultipleLongMatchOverlapSequenceNodeName = "OneLineMultipleLongMatchOverlapSequence";
        internal const string OneLineMultipleSameLengthMatchOverlapSequenceNodeName = "OneLineMultipleSameLengthMatchOverlapSequence";
        internal const string OneLineMultipleMatchOverlapSequenceNodeName = "OneLineMultipleMatchOverlapSequence";
        internal const string OneCharacterSequenceNodeName = "OneCharacterSequence";
        internal const string SimpleDnaFastaNodeName = "SimpleDnaFasta";
        internal const string SimpleDnaGenBankNodeName = "SimpleDnaGenBank";
        internal const string SimpleDnaGffNodeName = "SimpleDnaGff";
        internal const string DnaSequenceNodeName = "DnaSequence";
        internal const string Dna1000BPSequenceNodeName = "Dna1000BPSequence";
        internal const string RnaSequenceNodeName = "RnaSequence";
        internal const string ProteinSequenceNodeName = "ProteinSequence";
        internal const string DnaRnaSequenceNodeName = "DnaRnaSequence";
        internal const string RnaProteinSequenceNodeName = "RnaProteinSequence";
        internal const string DnaProteinSequenceNodeName = "DnaProteinSequence";
        internal const string OneLineSameCharactersNodeName = "OneLineSameCharacters";
        internal const string OneLineOverlapSequenceNodeName = "OneLineOverlapSequence";
        internal const string OneLineNoMatchSequenceNodeName = "OneLineNoMatchSequence";
        internal const string EdgeStartIndexesNode = "EdgeStartIndexes";
        internal const string EdgeEndIndexesNode = "EdgeEndIndexes";
        internal const string NodesNode = "Nodes";
        internal const string MUMAlignLengthNode = "MUMAlignLength";
        internal const string SearchSequenceNode = "SearchSequence";
        internal const string SearchSequenceAlphabetNode = "SearchSequenceAlphabet";
        internal const string MUMLengthNode = "MUMLength";
        internal const string SearchSequenceFilePathNode = "SearchSequenceFilePath";
        internal const string FirstSequenceMumOrderNode = "FirstSequenceMumOrder";
        internal const string FirstSequenceStartNode = "FirstSequenceStart";
        internal const string LengthNode = "Length";
        internal const string SecondSequenceMumOrderNode = "SecondSequenceMumOrder";
        internal const string SecondSequenceStartNode = "SecondSequenceStart";
        internal const string LisFirstSequenceMumOrderNode = "LISFirstSequenceMumOrder";
        internal const string LisFirstSequenceStartNode = "LISFirstSequenceStart";
        internal const string LisLengthNode = "LISLength";
        internal const string LisSecondSequenceMumOrderNode = "LISSecondSequenceMumOrder";
        internal const string LisSecondSequenceStartNode = "LISSecondSequenceStart";
        internal const string SubLisFirstSequenceMumOrderNode = "SubLISFirstSequenceMumOrder";
        internal const string SubLisFirstSequenceStartNode = "SubLISFirstSequenceStart";
        internal const string SubLisLengthNode = "SubLISLength";
        internal const string SubLisSecondSequenceMumOrderNode = "SubLISSecondSequenceMumOrder";
        internal const string SubLisSecondSequenceStartNode = "SubLISSecondSequenceStart";
        internal const string ExpectedSequencesNode = "ExpectedSequences";
        internal const string MUMDescription = "Pairwise global alignment";
        internal const string MUMLength = "20";
        internal const string MUMName = "MUMmer 3.0";
        internal const string MUMRefSeqNumber = "0";
        internal const string MUMGapOpenCost = "-13";
        internal const string MUMPairWiseAlgorithm = "Bio.Algorithms.Alignment.NeedlemanWunschAligner";
        internal const string QuerySeqeunceListNode = "QuerySeqeunceList";
        internal const string OneLineMultipleMatchOverlapSequenceUpToLISNode = "OneLineMultipleMatchOverlapSequenceUpToLIS";
        internal const string OneLineMultipleMatchOverlapSequenceAfterLISNode = "OneLineMultipleMatchOverlapSequenceAfterLIS";
        internal const string OneLineMultipleSameLengthMatchOverlapSequenceUptoLISNode = "OneLineMultipleSameLengthMatchOverlapSequenceUptoLIS";
        internal const string OneLineMoreThanTwoMatchSequenceAfterLISNode = "OneLineMoreThanTwoMatchSequenceAfterLIS";
        internal const string OneLineMoreThanTwoMatchSequenceUpToLISNode = "OneLineMoreThanTwoMatchSequenceUpToLIS";
        internal const string MultiWaytreeEdgeStartIndexesNode = "MultiWaytreeEdgeStartIndexes";
        internal const string MultiWaytreeEdgeEndIndexesNode = "MultiWaytreeEdgeEndIndexes";
        internal const string PersistentThresholdNode = "PersistentThreshold";
        internal const string ChildrenCountNode = "ChildrenCount";
        internal const string FileStorageNode = "FileStorage";
        internal const string ChildrenToReplaceNode = "ChildrenToReplace";
        internal const string PersistentTreeCountNode = "PersistentTreeCount";
        internal const string ByteCharacterNode =
          "ByteCharacter";
        internal const string ExpectedStartIndexNode = "ExpectedStartIndex";
        internal const string ExpectedEndIndexNode = "ExpectedEndIndex";


        // Derived Sequence xml nodes.
        internal const string AddSequence = "AddSequence";
        internal const string RemoveRange = "RemoveRange";
        internal const string RemoveRange1 = "RemoveRange1";
        internal const string DerivedSequence = "DerivedSequence";
        internal const string RemoveDerivedSequence = "RemoveDerivedSequence";
        internal const string RemoveDerivedSequence1 = "RemoveDerivedSequence1";
        internal const string RemoveDerivedSequence2 = "RemoveDerivedSequence2";
        internal const string AddDerivedSequence = "AddDerivedSequence";
        internal const string InsertDerivedSequence = "InsertDerivedSequence";
        internal const string InsertSequence = "InsertSequence";
        internal const string DnaDerivedSequenceNode = "DnaDerivedSequence";
        internal const string UpdatedItemsIndex = "UpdatedItemsIndex";
        internal const string UpdatedItemList = "UpdatedItemList";
        internal const string UpdatedTypeList = "UpdatedTypeList";
        internal const string Range = "Range";
        internal const string RangeSequence = "RangeSequence";
        internal const string ReplaceRangeSequence = "ReplaceRangeSequence";
        internal const string ReplaceSequence = "ReplaceSequence";
        internal const string IndexOfSequence = "IndexOfSequence";
        internal const string RnaDerivedSequenceNode = "RnaDerivedSequence";
        internal const string UpdatedItemInsertParams = "UpdatedItemInsertParams";
        internal const string UpdatedItemRemovePosition = "UpdatedItemRemovePosition";
        internal const string RemoveInsertDerivedSequence = "RemoveInsertDerivedSequence";
        internal const string ProteinDerivedSequenceNode = "ProteinDerivedSequence";

        // Snp Parser xml nodes
        internal const string SimpleSnpNodeName = "SimpleSnp";
        internal const string OneLineSnpNode = "OneLineSnp";
        internal const string MultiChromosomeSnpNodeName = "MultiChromosomeSnp";
        internal const string ExpectedPositionNode = "ExpectedPosition";
        internal const string ExpectedPositionsNode = "ExpectedPositions";
        internal const string NumberOfChromosomesNode = "NumberOfChromosomes";
        internal const string ExpectedSequenceAllele2Node = "ExpectedSequenceAllele2";
        internal const string SnpDescription = "Basic SNP Parser that uses XSV format";
        internal const string SnpFileTypes = ".tsv";
        internal const string SnpName = "Basic SNP";

        // Xml nodes for NUCmer Test cases
        internal const string ReferenceSequencesNode = "ReferenceSequences";
        internal const string SearchSequencesNode = "SearchSequences";
        internal const string NUCDescription = "Pairwise local alignment";
        internal const string NUCLength = "20";
        internal const string NUCName = "NUCmer 3.0";
        internal const string NUCRefSeqNumber = "0";
        internal const string NUCFixedSeperation = "5";
        internal const string NUCMinimumScore = "200";
        internal const string NUCMaximumSeparation = "1000";
        internal const string NUCSeparationFactor = "0.05";
        internal const string NUCGapOpenCost = "-13";
        internal const string NUCGapExtensionCostNode = "-8";
        internal const string ClustFirstSequenceMumOrderNode = "ClustFirstSequenceMumOrder";
        internal const string ClustFirstSequenceStartNode = "ClustFirstSequenceStart";
        internal const string ClustLengthNode = "ClustLength";
        internal const string ClustSecondSequenceMumOrderNode = "ClustSecondSequenceMumOrder";
        internal const string ClustSecondSequenceStartNode = "ClustSecondSequenceStart";
        internal const string OneUniqueMatchSequenceNodeName = "OneUniqueMatchSequence";
        internal const string TwoUniqueMatchWithoutCrossOverlapSequenceNodeName = "TwoUniqueMatchWithoutCrossOverlapSequence";
        internal const string TwoUniqueMatchWithCrossOverlapSequenceNodeName = "TwoUniqueMatchWithCrossOverlapSequence";
        internal const string MinimumScoreNode = "MinimumScore";
        internal const string MaximumSeparationNode = "MaximumSeparation";
        internal const string FixedSeparationNode = "FixedSeparation";
        internal const string SeparationFactorNode = "SeparationFactor";
        internal const string RnaNucmerSequenceNodeName = "RnaNucmerSequence";
        internal const string DnaNucmerSequenceNodeName = "DnaNucmerSequence";
        internal const string SameSequenceNodeName = "SameSequence";
        internal const string DnaAmbiguityReferenceSequenceNodeName = "DnaAmbiguityReferenceSequence";
        internal const string DnaAmbiguitySearchSequenceNodeName = "DnaAmbiguitySearchSequence";
        internal const string RnaAmbiguityReferenceSequenceNodeName = "RnaAmbiguityReferenceSequence";
        internal const string RnaAmbiguitySearchSequenceNodeName = "RnaAmbiguitySearchSequence";
        internal const string MinimumScoreGreaterSequenceNodeName = "MinimumScoreGreaterSequence";
        internal const string OneLineOneReferenceQuerySequenceNodeName = "OneLineOneReferenceQuerySequence";
        internal const string SingleDnaNucmerSequenceNodeName = "SingleDnaNucmerSequence";
        internal const string SingleDna1000BPSequence = "SingleDna1000BPSequence";
        internal const string SingleRnaNucmerSequenceNodeName = "SingleRnaNucmerSequence";
        internal const string SingleRefMultiQueryDnaNucmerSequenceNodeName = "SingleRefMultiQueryDnaNucmerSequenceNodeName";
        internal const string SingleRefMultiQueryRnaNucmerSequenceNodeName = "SingleRefMultiQueryRnaNucmerSequenceNodeName";
        internal const string MultiRefMultiQueryDnaMatchSequence = "MultiRefMultiQueryDnaMatchSequence";
        internal const string MultiRefMultiQueryRnaMatchSequence = "MultiRefMultiQueryRnaMatchSequence";
        internal const string SimpleRnaNucmerSequenceNodeName = "SimpleAlignRnaNucmerSequence";
        internal const string SimpleDnaNucmerSequenceNodeName = "SimpleAlignDnaNucmerSequence";
        internal const string InvalidMumLengthSequence = "InvalidMumLengthSequence";
        internal const string GreaterMumLengthSequence = "GreaterMumLengthSequence";
        internal const string SimpleAlignMediumSizeSequence = "SimpleAlignMediumSizeSequence";
        internal const string OneOverlapMatchSequenceNodeName = "OneOverlapMatchSequence";
        internal const string MultiRefSingleQueryMatchSequenceNodeName = "MultiRefSingleQueryMatchSequence";
        // Xml nodes for Phylogenetic Tree 
        internal const string OneLinePhyloTreeNodeName = "OneLinePhyloTree";
        internal const string SmallSizePhyloTreeNodeName = "SmallSizePhyloTree";
        internal const string OneLinePhyloTreeObjectNodeName = "OneLinePhyloTreeObject";
        internal const string OneNodePhyloTreeNodeName = "OneNodePhyloTree";
        internal const string OneNodePhyloTreeObjectNodeName = "OneNodePhyloTreeObject";
        internal const string OneLineGreaterThanOnePhyloTreeNodeName = "OneLineGreaterThanOnePhyloTree";
        internal const string RootBranchCountNode = "RootBranchCount";
        internal const string NodeNamesNode = "NodeNames";
        internal const string EdgeDistancesNode = "EdgeDistances";
        internal const string OutputFilePathNode = "OutputFilePath";
        internal const string ParserFileTypes = ".txt, .tre, .newick";
        internal const string ParserName = "Newick";
        internal const string ParserDescription = "Reads from a source of text that is formatted according to the Newick flat\r\nfile specification, and converts the data to in-memory PhylogeneticTree object.";
        internal const string FormatDescription = "Writes a PhylogeneticTree to a particular location, usually a file. The output is formatted\r\naccording to the Newick format.";
        internal const string SpecialCharSmallSizePhyloTreeNode =
            "SpecialCharSmallSizePhyloTree";
        internal const string InvalidateNewickParseNode =
            "InvalidateNewickParse";
        internal const string InvalidateNewickParserPeekNode =
            "InvalidateNewickParserPeek";
        internal const string InvalidateNewickLeafNode =
            "InvalidateNewickLeaf";
        internal const string InvalidateNewickBranchNode =
            "InvalidateNewickBranch";

        // Xml nodes for Compound Sequence
        internal const string CompoundNucleotideNode = "CompoundNucleotide";
        internal const string BaseSymbolsNode = "BaseSymbols";
        internal const string BaseValuesNode = "BaseValues";
        internal const string CompoundItemName = "Compound";
        internal const string SequenceItemWeightsNode = "SequenceItemWeights";
        internal const string CompoundAminoAcidNode = "CompoundAminoAcid";
        internal const string SequenceItemSymbolsNode = "SequenceItemSymbols";
        internal const string SequenceItemBytesNode = "SequenceItemBytesNode";
        internal const string CompoundNucleotideRnaNode = "CompoundNucleotideRna";

        // Xml nodes for Sequence P2 test cases
        internal const string EmptyGenBankNodeName = "EmptyGenBank";
        internal const string InvalidAddExceptionNode = "InvalidAddException";
        internal const string ReverseSequenceNode = "ReverseSequence";
        internal const string ComplementSequenceNode = "ComplementSequence";
        internal const string ReverseComplementSequenceNode = "ReverseComplementSequence";
        internal const string ComplementExceptionNode = "ComplementException";
        internal const string InsertExceptionNode = "InsertException";
        internal const string RemoveExceptionNode = "RemoveException";
        internal const string MediumSizeP2FastaNodeName = "MediumSizeP2Fasta";

        // Xml nodes for Bed Parsers & Formatters
        internal const string SmallSizeBedNodeName = "SmallSizeBed";
        internal const string IDNode = "ID";
        internal const string StartNode = "Start";
        internal const string EndNode = "End";
        internal const string IDNode1 = "ID1";
        internal const string StartNode1 = "Start1";
        internal const string EndNode1 = "End1";
        internal const string OneLineBedNodeName = "OneLineBed";
        internal const string BedTempFileName = "temp.Bed";
        internal const string BedName = "BED";
        internal const string BedDescription = "Chromosome sequence ranges format.";
        internal const string BedFileTypes = ".bed";
        internal const string ThreeChromoBedNodeName = "ThreeChromoBed";
        internal const string LongStartEndBedNodeName = "LongStartEndBed";
        internal const string SequenceRangeNode = "SequenceRange";
        internal const string SequenceRangeCountNode = "SequenceRangeCount";
        internal const string MergeBedFileNode = "MergeBedFile";
        internal const string IntersectResultsWithoutPiecesOfIntervals = "IntersectResultsWithoutPiecesOfIntervals";
        internal const string IntersectResultsWithPiecesOfIntervals = "IntersectResultsWithPiecesOfIntervals";
        internal const string IntersectWithoutPiecesOfIntervalsForSmallSizeFile = "IntersectWithoutPiecesOfIntervalsForSmallSizeFile";
        internal const string IntersectWithPiecesOfIntervalsForSmallSizeFile = "IntersectWithPiecesOfIntervalsForSmallSizeFile";
        internal const string QueryFilePath = "FilePath1";
        internal const string OverlapValue = "OverlapValue";
        internal const string ComparisonResult = "ComparisonResult";
        internal const string CompareSequenceRangeWithIdenticalStartNode = "CompareSequenceRangeWithIdenticalStart";
        internal const string CompareSequenceRangeWithIdenticalENDNode = "CompareSequenceRangeWithIdenticalEND";
        internal const string MergesmallFilewithIdenticalChromosomesNode = "MergesmallFilewithIdenticalChromosomes";
        internal const string MergeFilewithAllIdenticalChromosomesNode = "MergeFilewithAllIdenticalChromosomes";
        internal const string MergeTwosmallFilesNode = "MergeTwosmallFiles";
        internal const string MergeTwoFileswithAllIdenticalChromosomesNode = "MergeTwoFileswithAllIdenticalChromosomes";
        internal const string IntersectWithIdenticalChromoWithoutIntervals = "IntersectsmallFilewithIdenticalChromosomesWithoutPieces";
        internal const string IntersectWithAllIdenticalChromoWithoutIntervals = "IntersectsmallFilewithAllIdenticalChromosomesWithoutPieces";
        internal const string IntersectBedFilesWithTenChromo = "IntersectBedFilesWithTenChromo";
        internal const string IntersectBedFilesWithTenChromoWithMinimalOverlap = "IntersectBedFilesWithTenChromoWithMinimalOverlap";
        internal const string MergeTwoFiles = "MergeTwoFiles";
        internal const string LargeSizeBedNodeName = "LargeSizeBed";
        internal const string SubtractBedFilesWithMinimalOverlapNodeName = "SubtractBedFilesWithMinimalOverlap";
        internal const string SubtractBedFilesNodeName = "SubtractBedFiles";
        internal const string SubtractBedFilesWithIntervalsNodeName = "SubtractBedFilesWithIntervals";
        internal const string SubtractMultipleChromosomesWithIntervalsNodeName = "SubtractMultipleChromosomesWithIntervals";
        internal const string SubtractMultipleChromosomesBedFilesNodeName = "SubtractMultipleChromosomesBedFiles";
        internal const string SubtractSmallBedFilesWithMinimalOverlapNodeName = "SubtractSmallBedFilesWithMinimalOverlap";
        internal const string SubtractSmallBedFilesNodeName = "SubtractSmallBedFiles";
        internal const string SubtractSmallBedFilesWithIntervalsNodeName = "SubtractSmallBedFilesWithIntervals";

        // MSA xml Node Names
        internal const string MuscleDnaSequenceNode = "MultipleNWProfilerDnaSequence";
        internal const string MuscleDnaWithJensenShannonDivergence = "MultipleNWProfilerDnaSequenceWithJensenShannonDivergence";
        internal const string MuscleDnaWithLogExponentialInnerProduct = "MultipleNWProfilerDnaSequenceWithLogExponentialInnerProduct";
        internal const string MuscleDnaSequenceWithWeightedInnerProduct = "MultipleNWProfilerDnaSequenceWithWeightedInnerProduct";
        internal const string MuscleDnaWithPearsonCorrelation = "MultipleNWProfilerDnaSequenceWithPearsonCorrelation";
        internal const string MuscleDnaWithSymmetrizedEntropy = "MultipleNWProfilerDnaSequenceWithSymmetrizedEntropy";
        internal const string MuscleSWProfilerDnaSequenceNode = "MultipleSWProfilerDnaSequence";
        internal const string Stage1ExpectedSequenceNode1 = "Stage1ExpectedSequence1";
        internal const string Stage1ExpectedSequenceNode2 = "Stage1ExpectedSequence2";
        internal const string Stage1ExpectedSequenceNode3 = "Stage1ExpectedSequence3";
        internal const string Stage1ExpectedSequenceNode4 = "Stage1ExpectedSequence4";
        internal const string Stage1ExpectedSequenceNode5 = "Stage1ExpectedSequence5";
        internal const string Stage1ExpectedSequenceNode6 = "Stage1ExpectedSequence6";
        internal const string Stage1ExpectedSequenceNode7 = "Stage1ExpectedSequence7";
        internal const string Stage1ExpectedScoreNode = "Stage1ExpectedScore";
        internal const string Stage2ExpectedSequenceNode1 = "Stage2ExpectedSequence1";
        internal const string Stage2ExpectedSequenceNode2 = "Stage2ExpectedSequence2";
        internal const string Stage2ExpectedSequenceNode3 = "Stage2ExpectedSequence3";
        internal const string Stage2ExpectedSequenceNode4 = "Stage2ExpectedSequence4";
        internal const string Stage2ExpectedSequenceNode5 = "Stage2ExpectedSequence5";
        internal const string Stage2ExpectedSequenceNode6 = "Stage2ExpectedSequence6";
        internal const string Stage2ExpectedSequenceNode7 = "Stage2ExpectedSequence7";
        internal const string Stage2ExpectedScoreNode = "Stage2ExpectedScore";
        internal const string Stage3ExpectedSequenceNode1 = "Stage3ExpectedSequence1";
        internal const string Stage3ExpectedSequenceNode2 = "Stage3ExpectedSequence2";
        internal const string Stage3ExpectedSequenceNode3 = "Stage3ExpectedSequence3";
        internal const string Stage3ExpectedSequenceNode4 = "Stage3ExpectedSequence4";
        internal const string Stage3ExpectedSequenceNode5 = "Stage3ExpectedSequence5";
        internal const string Stage3ExpectedSequenceNode6 = "Stage3ExpectedSequence6";
        internal const string Stage3ExpectedSequenceNode7 = "Stage3ExpectedSequence7";
        internal const string Stage3ExpectedScoreNode = "Stage3ExpectedScore";
        internal const string KmerDistanceMatrixNode = "KmerDistanceMatrix";
        internal const string KmerDistanceMatrixWithCoVariance = "KmerDistanceMatrixWithCoVariance";
        internal const string KmerDistanceMatrixWithModifiedMuscle = "KmerDistanceMatrixWithModifiedMuscle";
        internal const string KmerDistanceMatrixWithPearsonCorrelation = "KmerDistanceMatrixWithPearsonCorrelation";
        internal const string KmerDistanceMatrixRna = "KmerDistanceMatrixRna";
        internal const string KmerDistanceMatrixRnaWithCoVariance = "KmerDistanceMatrixRnaWithCoVariance";
        internal const string KmerDistanceMatrixRnaWithModifiedMuscle = "KmerDistanceMatrixRnaWithModifiedMuscle";
        internal const string KmerDistanceMatrixRnaWithPearsonCorrelation = "KmerDistanceMatrixRnaWithPearsonCorrelation";
        internal const string KmerDistanceMatrixProtein = "KmerDistanceMatrixProtein";
        internal const string KmerDistanceMatrixProteinWithPearsonCorrelation = "KmerDistanceMatrixProteinWithPearsonCorrelation";
        internal const string KmerDistanceMatrixProteinWithCoVariance = "KmerDistanceMatrixProteinWithCoVariance";
        internal const string KmerDistanceMatrixProteinWithModifiedMuscle = "KmerDistanceMatrixProteinWithModifiedMuscle";
        internal const string Dimension = "Dimension";
        internal const string MinimumValue = "MinimumValue";
        internal const string NearestDistances = "NearestDistances";
        internal const string HierarchicalClusteringNode = "HierarchicalClustering";
        internal const string HierarchicalClusteringWeightedMAFFT = "HierarchicalClusteringWithWeightedMAFFT";
        internal const string HierarchicalClusteringStage2Node = "HierarchicalClusteringStage2";
        internal const string HierarchicalClusteringStage2WithWeightedMAFFT = "HierarchicalClusteringStage2WithWeightedMAFFT";
        internal const string HierarchicalClusteringProteinNode = "HierarchicalClusteringProtein";
        internal const string HierarchicalClusteringProteinWithComplete = "HierarchicalClusteringProteinWithComplete";
        internal const string HierarchicalClusteringProteinStage2WithSingle = "HierarchicalClusteringProteinStage2WithSingle";
        internal const string HierarchicalClusteringProteinWithWeightedMAFFT = "HierarchicalClusteringProteinWithWeightedMAFFT";
        internal const string HierarchicalClusteringProteinStage2Node = "HierarchicalClusteringProteinStage2";
        internal const string HierarchicalClusteringProteinStage2WithComplete = "HierarchicalClusteringProteinStage2WithComplete";
        internal const string HierarchicalClusteringProteinStage2WithWeightedMAFFT = "HierarchicalClusteringProteinStage2WithWeightedMAFFT";
        internal const string HierarchicalClusteringRnaNode = "HierarchicalClusteringRna";
        internal const string HierarchicalClusteringRnaWeightedMAFFT = "HierarchicalClusteringRnaWithWeightedMAFFT";
        internal const string HierarchicalClusteringRnaStage2Node = "HierarchicalClusteringRnaStage2";
        internal const string HierarchicalClusteringRnaStage2WithWeightedMAFFT = "HierarchicalClusteringRnaStage2WithWeightedMAFFT";
        internal const string NodesCount = "NodesCount";
        internal const string Nodes = "Nodes";
        internal const string EdgesCount = "EdgesCount";
        internal const string NodesLeftChild = "NodesLeftChild";
        internal const string NodesRightChild = "NodesRightChild";
        internal const string BinaryTreeNode = "BinaryTree";
        internal const string BinaryTreeProteinNode = "BinaryTreeWithProteinSequences";
        internal const string BinaryTreeRnaNode = "BinaryTreeWithRnaSequences";
        internal const string BinarySubTree = "BinarySubTree";
        internal const string BinarySubTreeRna = "BinarySubTreeRna";
        internal const string BinarySubTreeProtein = "BinarySubTreeProtein";
        internal const string RootId = "RootId";
        internal const string LeavesCount = "LeavesCount";
        internal const string BinaryTreeStage2Node = "BinaryTreeStage2";
        internal const string SequenceIndicesWithCutTree = "SequenceIndicesWithCutTree";
        internal const string ProfileAligner = "ProfileAligner";
        internal const string ProfileAlignerRna = "ProfileAlignerRna";
        internal const string GenerateProfileAlignerRna = "GenerateProfileAlignerRna";
        internal const string ProfileAlignerProtein = "ProfileAlignerProtein";
        internal const string SubTreeEdges = "SubTreeEdges";
        internal const string SubTreeRoots = "SubTreeRoot";
        internal const string GenerateSequenceString = "GenerateSequenceString";
        internal const string KimuraDistanceMatrix = "KimuraDistanceMatrix";
        internal const string MuscleProteinSequenceNode = "MultipleNWProfilerProteinSequence";
        internal const string MuscleProteinSequenceEuclieanDistanceNode = "MultipleNWProfilerProteinEuclieanDistanceSequence";
        internal const string MuscleProteinSequenceWithComplete = "MultipleNWProfilerProteinSequenceWithComplete";
        internal const string MuscleProteinWithJensenShannonDivergence = "MultipleNWProfilerProteinSequenceWithJensenShannonDivergence";
        internal const string MuscleProteinWithLogExponentialInnerProduct = "MultipleNWProfilerProteinSequenceWithLogExponentialInnerProduct";
        internal const string MuscleProteinWithLogExponentialInnerProductShifted = "MultipleNWProfilerProteinSequenceWithLogExponentialInnerProductShifted";
        internal const string MuscleProteinSequenceWithWeightedInnerProductShifted = "MultipleNWProfilerProteinSequenceWithWeightedInnerProductShifted";
        internal const string MuscleProteinWithPearsonCorrelationProfileScore = "MultipleNWProfilerProteinSequenceWithPearsonCorrelationProfileScore";
        internal const string MuscleProteinWithSymmetrizedEntropy = "MultipleNWProfilerProteinSequenceWithSymmetrizedEntropy";
        internal const string MuscleRnaSequenceNode = "MultipleNWProfilerRnaSequence";
        internal const string MultipleNWProfilerRnaSequenceWithWeightedInnerProduct = "MultipleNWProfilerRnaSequenceWithWeightedInnerProduct";
        internal const string MultipleNWProfilerRnaSequenceWithWeightedInnerProductShifted = "MultipleNWProfilerRnaSequenceWithWeightedInnerProductShifted";
        internal const string MuscleRnaWithJensenShannonDivergence = "MultipleNWProfilerRnaSequenceWithJensenShannonDivergence";
        internal const string MultipleNWProfilerRnaSequenceWithWeightedMAFFTMethod = "MultipleNWProfilerRnaSequenceWithWeightedMAFFTMethod";
        internal const string MuscleRnaWithLogExponentialInnerProduct = "MultipleNWProfilerRnaSequenceWithLogExponentialInnerProduct";
        internal const string MultipleNWProfilerRnaSequenceWithLogExponentialInnerProductShiftedNode = "MultipleNWProfilerRnaSequenceWithLogExponentialInnerProductShifted";
        internal const string MuscleRnaWithPearsonCorrelation = "MultipleNWProfilerRnaSequenceWithPearsonCorrelation";
        internal const string MultipleNWProfilerRnaSequenceWithPearsonCorrelationScore = "MultipleNWProfilerRnaSequenceWithPearsonCorrelationScore";
        internal const string MultipleNWProfilerRnaSequenceWithModifiedMuscle = "MultipleNWProfilerRnaSequenceWithModifiedMuscle";
        internal const string MultipleNWProfilerRnaSequenceWithSingleMethod = "MultipleNWProfilerRnaSequenceWithSingleMethod";
        internal const string MuscleRnaWithSymmetrizedEntropy = "MultipleNWProfilerRnaSequenceWithSymmetrizedEntropy";
        internal const string MultipleNWProfilerRnaSequenceWithWeightedEuclideanDistance = "MultipleNWProfilerRnaSequenceWithWeightedEuclideanDistance";
        internal const string MuscleSWProfilerRnaSequenceNode = "MultipleSWProfilerRnaSequence";
        internal const string ExpectedScoreWithInnerProduct = "ExpectedScoreWithInnerProduct";
        internal const string ExpectedScoreWithWeightedInnerProduct = "ExpectedScoreWithWeightedInnerProduct";
        internal const string ExpectedScoreWithWeightedInnerProductShifted = "ExpectedScoreWithWeightedInnerProductShifted";
        internal const string ExpectedScoreWithInnerProductShifted = "ExpectedScoreWithInnerProductShifted";
        internal const string MuscleProteinOneLineSequence = "MultipleProteinOneLineSequence";
        internal const string MuscleDnaOneLineSequence = "MultipleDnaOneLineSequence";
        internal const string MuscleRnaOneLineSequence = "MultipleRnaOneLineSequence";
        internal const string MuscleDnaEqualGapCost = "MultipleDnaSequenceWithEqualGapCost";
        internal const string MuscleRnaEqualGapCost = "MultipleRnaSequenceWithEqualGapCost";
        internal const string MuscleProteinEqualGapCost = "MultipleProteinSequenceWithEqualGapCost";
        internal const string ColumnSize = "ColumnSize";
        internal const string FunctionNode = "Function";
        internal const string DbReferenceNode = "DBReference";
        internal const string RowSize = "RowSize";
        internal const string ProfileMatrix = "ProfileMatrix";
        internal const string BinarySubTreeNeedRealignment = "BinarySubTreeNeedRealignment";
        internal const string BinarySmallestTreeNode = "BinarySmallestTreeNode";
        internal const string KimuraDistanceMatrixProtein = "KimuraDistanceMatrixProtein";
        internal const string KimuraDistanceMatrixRna = "KimuraDistanceMatrixRna";
        internal const string MultipleProfileAligner = "MultipleProfileAligner";
        internal const string MultipleProfileAlignerRna = "MultipleProfileAlignerRna";
        internal const string MultipleProfileAlignerProtein = "MultipleProfileAlignerProtein";
        internal const string MuscleProteinSequenceWithProgresiveAlignerNodeName = "MultipleNWProfilerProteinSequenceWithProgresiveAligner";
        internal const string MuscleDnaSequenceWithCoVarianceNodeName = "MultipleNWProfilerDnaSequenceWithCoVariance";
        internal const string MuscleDnaSequenceWithLogExponentialInnerProductShiftedNodeName = "MultipleNWProfilerDnaSequenceWithLogExponentialInnerProductShifted";
        internal const string MuscleDnaSequenceWithWeightedEuclideanDistanceNodeName = "MultipleNWProfilerDnaSequenceWithWeightedEuclideanDistance";
        internal const string MuscleDnaSequenceWithPearsonCorrelationDistanceMethodNodeName = "MultipleNWProfilerDnaSequenceWithPearsonCorrelationDistanceMethod";
        internal const string MuscleDnaSequenceWithSingleDistanceMethodNodeName = "MultipleNWProfilerDnaSequenceWithSingleDistanceMethod";
        internal const string MuscleDnaSequenceWithWeightedMAFFTDistanceMethodNodeName = "MultipleNWProfilerDnaSequenceWithWeightedMAFFTDistanceMethod";
        internal const string MuscleDnaSequenceWithModifiedMuscleDistanceMethodNodeName = "MultipleNWProfilerDnaSequenceWithModifiedMuscleDistanceMethod";
        internal const string MuscleProteinSequenceWithCovarianceNodeName = "MultipleNWProfilerProteinSequenceWithCovarience";
        internal const string MuscleProteinSequenceWithInnerProductNodeName = "MultipleNWProfilerProteinSequenceWithInnerProduct";
        internal const string MuscleProteinSequenceWithModifiedMuscleNodeName = "MultipleNWProfilerProteinSequenceWithModifiedMuscle";
        internal const string MuscleProteinSequenceWithPearsonCorrelationNodeName = "MultipleNWProfilerProteinSequenceWithPearsonCorrelation";
        internal const string MuscleProteinSequenceWithSingleMethodNodeName = "MultipleNWProfilerProteinSequenceWithSingleMethod";
        internal const string MuscleProteinWithWeightedEuclideanDistanceNodeName = "MultipleNWProfilerProteinSequenceWithWeightedEuclideanDistance";
        internal const string MuscleProteinSequenceWithWeightedMAFFTNodeName = "MultipleNWProfilerProteinSequenceWithWeightedMAFFT";
        internal const string MuscleRnaSequenceWithCoVarianceNodeName = "MultipleNWProfilerRnaSequenceWithCovariance";
        internal const string MultipleNWProfilerRnaSequenceWithInnerProductFastNodeName = "MultipleNWProfilerRnaSequenceWithInnerProductFast";
        internal const string MultipleNWProfilerRnaSequenceWithInnerProduct = "MultipleNWProfilerRnaSequenceWithInnerProduct";
        internal const string HierarchicalClusteringStage2WithCompleteNode = "HierarchicalClusteringStage2WithComplete";
        internal const string HierarchicalClusteringRnaStage2WithCompleteNode = "HierarchicalClusteringRnaStage2WithComplete";
        internal const string ProfileAlignerWithWeightedInnerProductCachedNode = "ProfileAlignerWithWeightedInnerProductCached";
        internal const string ProfileAlignerWithWeightedEuclideanDistanceFastNode = "ProfileAlignerWithWeightedEuclideanDistanceFast";
        internal const string ProfileAlignerWithWeightedInnerProductNode = "ProfileAlignerWithWeightedInnerProduct";
        internal const string DnaWith12SequencesNode = "DnaWith12Sequences";
        internal const string QScoreNode = "QScore";
        internal const string TCScoreNode = "TCScore";
        internal const string ResiduesCountNode = "ResiduesCount";
        internal const string DnaFunctionsNode = "DnaFunctions";
        internal const string CorrelationNode = "Correlation";
        internal const string MaxIndexNode = "MaxIndex";
        internal const string JensenShanonDivergenceNode = "JensenShanonDivergence";
        internal const string KullbackLeiblerDistanceNode = "KullbackLeiblerDistance";
        internal const string PairWiseScoreNode = "PairWiseScore";
        internal const string MuscleDnaWithLogExponentialInnerProductFastNode = "MultipleNWProfilerDnaSequenceWithLogExponentialInnerProductFast";
        internal const string MuscleDnaSequenceWithWeightedInnerProductShiftedFastNode = "MultipleNWProfilerDnaSequenceWithWeightedInnerProductShiftedFast";
        internal const string MuscleDnaSequenceWithWeightsNode = "MultipleNWProfilerDnaSequenceWithWeights";
        internal const int ParallelProcess = 2;
        internal const int SerialProcess = 1;
        internal const string PairWiseMatrixDnaNode = "PairWiseMatrixDna";
        internal const string ProfileAlignerRnaWithSimpleAlignNode = "ProfileAlignerRnaWithSimpleAlign";
        internal const string ProfileAlignerProteinWithSimpleAlignNode = "ProfileAlignerProteinWithSimpleAlign";
        internal const string RnaWith12SequencesNode = "RnaWith12Sequences";
        internal const string RnaFunctionsNode = "RnaFunctions";
        internal const string ProteinWith12SequencesNode = "ProteinWith12Sequences";
        internal const string ProteinFunctionsNode = "ProteinFunctions";
        internal const string PairWiseMatrixRnaNode = "PairWiseMatrixRna";
        internal const string PairWiseMatrixProteinNode = "PairWiseMatrixProtein";
        internal const string MatrixNameNode = "MatrixName";
        internal const string MuscleRnaSequenceWithLogExponentialInnerProductFastNode =
            "MultipleNWProfilerRnaSequenceWithLogExponentialInnerProductFast";
        internal const string MuscleRnaSequenceWithWeightedEuclideanDistanceFast =
            "MultipleNWProfilerRnaSequenceWithWeightedEuclideanDistanceFast";
        internal const string MuscleProteinSequenceWithLogExponentialInnerProductFastNode =
            "MultipleNWProfilerProteinSequenceWithLogExponentialInnerProductFast";
        internal const string MuscleProteinSequenceWithLogExponentialInnerProductShiftedFastNode =
        "MultipleNWProfilerProteinSequenceWithLogExponentialInnerProductShiftedFast";
        internal const string MuscleProteinSequenceWithWeightedInnerProductShiftedFastNode =
            "MultipleNWProfilerProteinSequenceWithWeightedInnerProductShiftedFast";
        internal const string ProfileAlignerWithLogExponentialInnerProductFastNode =
            "ProfileAlignerWithLogExponentialInnerProductFast";
        internal const string ProfileAlignerWithLogExponentialInnerProductShiftedFastNode =
            "ProfileAlignerWithLogExponentialInnerProductShiftedFast";
        internal const string HierarchicalClusteringStage2WithSingleNode =
            "HierarchicalClusteringStage2WithSingle";
        internal const string MultipleNWProfilerDnaSequenceWithInnerProductFastNode =
            "MultipleNWProfilerDnaSequenceWithInnerProductFast";
        internal const string MusclerDnaSequenceWithWeightedInnerProductShiftedNode =
            "MultipleNWProfilerDnaSequenceWithWeightedInnerProductShifted";
        internal const string MuscleDnaSequenceWithWeightedMafftNode =
            "MultipleNWProfilerDnaSequenceWithWeightedMafft";
        internal const string ProfileAlignerWithAlignmentNode =
            "ProfileAlignerWithAlignment";
        internal const string RefFilePathNode = "RefFilePath";
        internal const string MuscleProteinSequenceWithInnerProductFastNode =
            "MultipleNWProfilerProteinSequenceWithInnerProductFast";

        internal const string MultipleSequenceDNADNAFastaNodeName =
            "MultipleSequenceDNADNAFasta";
        internal const string SmallSizeClustalWNodeName = "SmallSizeClustalW";
        internal const string SupportedFileTypesNode = "SupportedFileTypes";
        internal const string ClustalWDescriptionsNode = "ClustalWDescriptions";
        internal const string ExpectedAlignmentNode = "ExpectedAlignment";
        internal const string SmallSizeNexusNodeName = "SmallSizeNexus";
        internal const string SmallSizePhylipNodeName = "SmallSizePhylip";
        internal const string PhylipDescriptionNode =
            "PhylipDescription";
        internal const string PhylipNameNode =
            "PhylipName";
        internal const string PhylipFileTypesNode =
            "PhylipFileTypes";
        internal const string PhylipEncodingNode =
            "PhylipEncoding";
        internal const string PhylipPropertyNode =
            "PhylipProperty";
        internal const string EmptyPhylipParserFileNode =
            "EmptyPhylipParserFile";
        internal const string PhylipParserEncodingNode =
            "PhylipParserEncoding";
        internal const string CommonSequenceParserRNA =
            "AUUGUCUUCUUAAUUAGCUCAUUAGUACUUUACAUUAUUUCACUAAUACUAACGACAAAGCUGACCCAUACAAGCACGAUAGAUGCACAAGA";
        internal const string CommonSequenceParserProtein =
            "IFYEPVEILGYDNKSSLVLVKRLITRIFYEPVIFYEPV";
        internal const string DNA = "DNA";
        internal const string NA = "NA";
        internal const string RNA = "RNA";
        internal const string TRNA = "TRNA";
        internal const string RRNA = "RRNA";
        internal const string MRNA = "MRNA";
        internal const string URNA = "URNA";
        internal const string SNRNA = "SNRNA";
        internal const string SNORNA = "SNORNA";
        internal const string PROTEIN = "PROTEIN";
        internal const string InvalidatePhylipParserAlignAlphabetNode =
            "InvalidatePhylipParserAlignAlphabet";
        internal const string InvalidatePhylipParserAlphabetNode =
            "InvalidatePhylipParserAlphabet";
        internal const string InvalidatePhylipParserCountNode =
            "InvalidatePhylipParserCount";
        internal const string InvalidatePhylipParserSeqLengthNode =
            "InvalidatePhylipParserSeqLength";
        internal const string SimpleNexusFileNode =
            "SimpleNexusFile";
        internal const string NexusDescriptionNode =
            "NexusDescription";
        internal const string NexusNameNode =
            "NexusName";
        internal const string NexusFileTypesNode =
            "NexusFileTypes";
        internal const string NexusEncodingNode =
            "NexusEncoding";
        internal const string NexusPropertyNode =
            "NexusProperty";
        internal const string EmptyNexusFileNode =
            "EmptyNexusFile";
        internal const string SimpleNexusCharBlockNode =
            "SimpleNexusCharBlock";
        internal const string InvalidateNexusParserSeqCountNode =
            "InvalidateNexusParserSeqCount";
        internal const string InvalidateNexusParserHeaderNode =
            "InvalidateNexusParserHeader";
        internal const string InvalidateNexusParserAlphabetNode =
            "InvalidateNexusParserAlphabet";
        internal const string InvalidateNexusParserAlignAlphabetNode =
            "InvalidateNexusParserAlignAlphabet";
        internal const string InvalidateNexusParserSeqLengthNode =
            "InvalidateNexusParserSeqLength";

        // SAM Config xml nodes
        internal const string SmallSAMFileNode = "SmallSAMFile";
        internal const string SAMTempFileName = "temp.sam";
        internal const string SAMFileWithAllFieldsNode = "SAMFileWithAllFields";
        internal const string ScoresCount = "ScoresCount";
        internal const string CIGARNode = "CIGAR";
        internal const string SAMFileWithRefNode = "SAMFileWithRef";
        internal const string ReferenceSeqNode = "ReferenceSeq";
        internal const string SAMParserDescription =
            "A SAMParser reads from a source of text that is formatted according to the SAM \r\nfile specification, and converts the data to in-memory SequenceAlignmentMap objects.";
        internal const string SAMFormatterDescription =
            "Writes a SequenceAlignmentMap to a particular location, usually a file. The output is formatted\r\naccording to the SAM file format.";
        internal const string SAMName = "SAM";
        internal const string SAMFileType = ".sam";
        internal const string EmptySamFileNode =
            "EmptySamFile";
        internal const string OneEmptySequenceSamFileNode =
            "OneEmptySequenceSamFile";
        internal const string InvalidSamBioReaderNode =
            "InvalidSamBioReader";
        internal const string QualitySequence =
            "GCAATGATAAAAGGAGTAACCTGTGAAAAAGATGC";
        internal const string QualityLength =
            "EDCCCBAAAA@@@@?>===<;;9:99987776";
        internal const string SamFormatterFileNode =
            "SamFormatterFile";
        internal const string FormatterString =
            "@HD\tVN:1.0\tSO:unsorted\r\n@SQ\tSN:gi|110640213|ref|NC_008253.1|\tLN:4938920\r\n@PG\tID:Bowtie\tVN:0.11.3\tCL:\"bowtie -S e_coli reads/e_coli_10000snp.fq ec_snp.sam\"\r\nr0\t16\tgi|110640213|ref|NC_008253.1|\t5212\t255\t35M\t*\t0\t0\tGCAATGATAAAAGGAGTAACCTGTGAAAAAGATGC\t45567778999:9;;<===>?@@@@AAAABCCCDE\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr1\t0\tgi|110640213|ref|NC_008253.1|\t2568\t255\t35M\t*\t0\t0\tTATGTTGGCAATATTGATGAAGATGGCGTCTGCCG\tEDCCCBAAAA@@@@?>===<;;9:99987776554\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr2\t16\tgi|110640213|ref|NC_008253.1|\t5783\t255\t35M\t*\t0\t0\tCTGGCTTGATAATCTCGGCATTCAATTTCTTCGGC\t45567778999:9;;<===>?@@@@AAAABCCCDE\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr3\t0\tgi|110640213|ref|NC_008253.1|\t2863\t255\t35M\t*\t0\t0\tGCGGCGGTGACACCTGTTGATGGTGCATTGCTCGG\tEDCCCBAAAA@@@@?>===<;;9:99987776554\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr4\t0\tgi|110640213|ref|NC_008253.1|\t4068\t255\t35M\t*\t0\t0\tCTTTATGGCACAAATGCTGACCCATATTGCGGGCG\tEDCCCBAAAA@@@@?>===<;;9:99987776554\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr5\t16\tgi|110640213|ref|NC_008253.1|\t3930\t255\t35M\t*\t0\t0\tAATCCCGCAGGAAATCCTGGAAGAGCGCGTACGTG\t45567778999:9;;<===>?@@@@AAAABCCCDE\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr6\t16\tgi|110640213|ref|NC_008253.1|\t4023\t255\t35M\t*\t0\t0\tATTGTTCCACGGGCCAACGCTGGCATTTAAAGATT\t45567778999:9;;<===>?@@@@AAAABCCCDE\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr7\t0\tgi|110640213|ref|NC_008253.1|\t3070\t255\t35M\t*\t0\t0\tCCGATCGGTTCGGGCTTAGGCTCCAGCGCCTGTTC\tEDCCCBAAAA@@@@?>===<;;9:99987776554\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr8\t16\tgi|110640213|ref|NC_008253.1|\t1394\t255\t35M\t*\t0\t0\tTTCCGAATACAGTATCAGTTTCTGCGTTCCGCAAA\t45567778999:9;;<===>?@@@@AAAABCCCDE\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr9\t16\tgi|110640213|ref|NC_008253.1|\t5012\t255\t35M\t*\t0\t0\tATCAGTAACATCTATTCATTATCTCAATCAGGCCG\t45567778999:9;;<===>?@@@@AAAABCCCDE\tXA:i:0\tMD:Z:35\tNM:i:0\r\nr10\t0\tgi|110640213|ref|NC_008253.1|\t6230\t255\t35M\t*\t0\t0\tGCGTCAGTTTCCGCGCTTCATGGATCAGCTGCTGG\tEDCCCBAAAA@@@@?>===<;;9:99987776554\tXA:i:0\tMD:Z:35\tNM:i:0\r\n";


        // PaDeNA Constant variables
        internal const string OneLineReadsNode = "OneLineReads";
        internal const string KmersOutputFileNode = "KmersOutputFile";
        internal const string SmallChromosomeReadsNode = "SmallChromosomeReads";
        internal const string ChromosomeReads = "SmallChromosomeReadsForCtr";
        internal const string NodesSequenceNode = "NodesSequence";
        internal const string NodesLeftEdgesCountNode = "NodeLeftEdgesCount";
        internal const string NodeRightEdgesCountNode = "NodeRightEdgesCount";
        internal const string OneLineStep2GraphNode = "OneLineStep2Graph";
        internal const string OneLineStep2GraphWithRevCompNode = "OneLineStep2GraphWithRevComp";
        internal const string OneLineStep3GraphNode = "OneLineStep3Graph";
        internal const string DangleNodeSequenceNode = "DangleNodeSequence";
        internal const string OneLineStep4ReadsNode = "OneLineStep4Reads";
        internal const string OneLineStep5ReadsNode = "OneLineStep5Reads";
        internal const string OneLineStep4ReadsAfterErrorRemove = "OneLineStep4ReadsAfterErrorRemove";
        internal const string ContigsNode = "Contigs";
        internal const string SequenceFromPathNode = "SequenceFromPath";
        internal const string ContigPathWithoutExtensionNode = "ContigPathWithoutExtension";
        internal const string ViralGenomeReadsNode = "VirulGenomeReads";
        internal const string NodesCountAfterDanglingGraphNode = "NodesCountAfterDanglingGraph";
        internal const string OneLineReadsWithRCNode = "OneLineReadsWithRC";
        internal const string OneLineWithRCStep2Node = "OneLineWithRCStep2";
        internal const string OneLineReadsWithClustersNode = "OneLineReadsWithClusters";
        internal const string OneLineReadsWithClustersAfterDangling = "OneLineReadsWithClustersAfterDangling";
        internal const string ReadsWithDanglingLinksNode = "ReadsWithDanglingLinks";
        internal const string ReadsWithMultipleDanglingLinksNode = "ReadsWithMultipleDanglingLinks";
        internal const string DanglingLinkThresholdNode = "DanglingLinkThreshold";
        internal const string ReadsWithMultipleBubblesNode = "ReadsWithMultipleBubbles";
        internal const string ReadsWithBubblesNode = "ReadsWithBubbles";
        internal const string PathLengthThresholdNode = "PathLengthThreshold";
        internal const string BaseSequenceNode = "BaseSequence";
        internal const string GraphNodesCountNode = "NodesCount";
        internal const string NodeExtensionsCountNode = "NodeExtensionsCount";
        internal const string LeftNodeExtensionsCountNode = "LeftNodeExtensionsCount";
        internal const string RightNodeExtensionsCountNode = "RightNodeExtensionsCount";
        internal const string KmersCountNode = "KmersCount";
        internal const string Step2SmallChromosomeReadsNode = "Step2SmallChromosomeReads";
        internal const string Step4ReadsWithSmallSize = "ReadsWithSmallSize";
        internal const string OneLineStep4ReadsAfterRemoveRedundancy = "OneLineStep4ReadsAfterRemoveRedundancy";
        internal const string Step4RedundantPathReadsNode = "Step4RedundantPathReads";
        internal const string ContigsCount = "ContigsCount";
        internal const string ExpectedKmersCount = "ExpectedKmersCount";
        internal const string ExpectedNodesCount = "ExpectedNodesCount";
        internal const string ExpectedNodesCountAfterDangling = "ExpectedNodesCountAfterDangling";
        internal const string ExpectedNodesCountRemoveRedundancy = "ExpectedNodesCountRemoveRedundancy";
        internal const string MicroorganismReadsNode = "MicroorganismReads";
        internal const string X1AndY1FormatPairedReadsNode = "X1AndY1FormatPairedReads";
        internal const string PairedReadsCountNode = "PairedReadsCount";
        internal const string ForwardReadsNode = "ForwardReads";
        internal const string BackwardReadsNode = "BackwardReads";
        internal const string LibraryNode = "Library";
        internal const string MeanLengthNode = "MeanLength";
        internal const string DeviationNode = "Deviation";
        internal const string FAndRPairedReadsNode = "FAndRFormatPairedReads";
        internal const string OneAndTwoPairedReadsNode = "OneAndTwoFormatPairedReads";
        internal const string X1AndY1PairedReadsNode = "X1AndY1FormatPairedReads";
        internal const string LibraryName = "LibraryName";
        internal const string StdDeviation = "StdDeviation";
        internal const string Mean = "Mean";
        internal const string ReadsWithDotsNode = "ReadsWithDots";
        internal const string ReadsWithDotsBetweenSeqIdNode = "ReadsWithDotsBetweenSeqId";
        internal const string OneLineReadsForPairedReadsNode = "OneLineReadsForPairedReads";
        internal const string ReadsWithSpecialCharsNode = "ReadsWithSpecialChars";
        internal const string ReadsWith2KlibraryNode = "ReadsWith2Klibrary";
        internal const string ReadsWith10KAnd50KAnd100KlibraryNode = "ReadsWith10KAnd50KAnd100Klibrary";
        internal const string ReadsWithoutAnySeqIdNode = "ReadsWithoutAnySeqId";
        internal const string RedundantThreshold = "redundantThreshold";
        internal const string ReadMapLength = "readMapLength";
        internal const string ContigStartPos = "contigStartPos";
        internal const string ReadStartPos = "readStartPos";
        internal const string MapReadsToContigFullOverlapNode = "MapReadsToContigFullOverlap";
        internal const string MapReadsToContigPartialOverlapNode = "MapReadsToContigPartialOverlap";
        internal const string ContigGraphForSmallReadsNode = "ContigGraphForSmallReads";
        internal const string ContigGraphWithClusterContigsNode = "ContigGraphWithClusterContigs";
        internal const string ContigPairedReadCount = "ContigGraphWithClusterContigs";
        internal const string ContigPairedReadsCount = "ContigPairedReadsCount";
        internal const string ForwardReadStartPos = "ForwardReadStartPos";
        internal const string ReverseReadStartPos = "ReverseReadStartPos";
        internal const string RerverseReadReverseCompPos = "RerverseReadReverseCompPos";
        internal const string FilterPairedReadContigsNode = "FilterPairedReadContigs";
        internal const string FilterPairedReadReverseOrietnationContigsNode = "FilterPairedReadReverseOrietnationContigs";
        internal const string DistanceBetweenFirstContig = "DistanceBetweenFirstContig";
        internal const string DistanceBetweenSecondContig = "DistanceBetweenSecondContig";
        internal const string FirstContigStandardDeviation = "FirstContigStandardDeviation";
        internal const string SecondContigStandardDeviation = "SecondContigStandardDeviation";
        internal const string X1AndY1FormatPairedReadAddLibrary = "X1AndY1FormatPairedReadAddLibrary";
        internal const string FilterPairedReadContigsUsingRedundancy = "FilterPairedReadContigsUsingRedundancy";
        internal const string AssembledPathWithOverlap = "AssembledPathWithOverlap";
        internal const string AssembledPathWithoutOverlap = "AssembledPathWithoutOverlap";
        internal const string InputRedundancy = "Redundancy";
        internal const string ScaffoldNodes = "ScaffoldNodes";
        internal const string DepthNode = "DepthValue";
        internal const string ScaffoldPathCount = "ScaffoldPathCount";
        internal const string ScaffoldPathWithOverlapNode = "ScaffoldPathWithOverlap";
        internal const string ScaffoldPathWithOverlapReverseNode = "ScaffoldPathWithOverlapReverse";
        internal const string AssembledPathWithOverlapNode = "AssembledPathWithOverlap";
        internal const string SequencePathNode = "SequencePath";
        internal const string AddX1AndY1FormatPairedReadsNode = "AddX1AndY1FormatPairedReads";
        internal const string GetLibraryInformationNode = "GetLibraryInformation";
        internal const string MapReadsToContigForSmallSizeChromosomeNode = "MapReadsToContigForSmallSizeChromosome";
        internal const string MapReadsToContigForViralGenomeNode = "MapReadsToContigForViralGenome";
        internal const string MapPairedReadsToContigForClustalWContigsNode = "MapPairedReadsToContigForClustalWContigs";
        internal const string MapPairedReadsToContigForReverseComplementContigsNode = "MapPairedReadsToContigForReverseComplementContigs";
        internal const string MapPairedReadsToContigForLeftSideContigGeneratorNode = "MapPairedReadsToContigForLeftSideContigGenerator";
        internal const string MapPairedReadsToContigForSeqAndRevCompNode = "MapPairedReadsToContigForSeqAndRevComp";
        internal const string MapPairedReadsToContigForRightSideContigGeneratorNode = "MapPairedReadsToContigForRightSideContigGenerator";
        internal const string ContigGraphForViralGenomeReadsNode = "ContigGraphForViralGenomeReads";
        internal const string ContigGraphForContigsWithBothOrientationNode = "ContigGraphForContigsWithBothOrientation";
        internal const string ContigGraphForPalindromeContigsNode = "ContigGraphForPalindromeContigs";
        internal const string ContigGraphNodesForChromosomesNode = "ContigGraphNodesForChromosomes";
        internal const string FilterPairedReadContigsForFWOrnNode = "FilterPairedReadContigsForFWOrn";
        internal const string FilterPairedReadContigsForRevOrientationNode = "FilterPairedReadContigsForRevOrientation";
        internal const string FilterPairedReadContigsForFWDirectionWithRevCompContigNode = "FilterPairedReadContigsForFWDirectionWithRevCompContig";
        internal const string FilterPairedReadContigsForBackwardDirectionWithRevCompContig = "FilterPairedReadContigsForBackwardDirectionWithRevCompContig";
        internal const string FilterPairedReadContigsForForwardWithPalindromeContigNode = "FilterPairedReadContigsForForwardWithPalindromeContig";
        internal const string FilterPairedReadContigsForBackwardWithPalindromeContigNode = "FilterPairedReadContigsForBackwardWithPalindromeContig";
        internal const string FilterPairedReadContigsForChromosomeReads = "FilterPairedReadContigsForChromosomeReads";
        internal const string FilterPairedReadContigsForViralGenomeReads = "FilterPairedReadContigsForViralGenomeReads";
        internal const string ScaffoldPathWithForwardOrientationNode = "ScaffoldPathWithForwardOrientation";
        internal const string ScaffoldPathWithReverseOrientationNode = "ScaffoldPathWithReverseOrientation";
        internal const string ScaffoldPathWithForwardDirectionAndRevComp = "ScaffoldPathWithForwardDirectionAndRevComp";
        internal const string ScaffoldPathWithReverseDirectionAndRevComp = "ScaffoldPathWithReverseDirectionAndRevComp";
        internal const string ScaffoldPathWithForwardDirectionAndPalContig = "ScaffoldPathWithForwardDirectionAndPalContig";
        internal const string ScaffoldPathWithReverseDirectionAndPalContig = "ScaffoldPathWithReverseDirectionAndPalContig";
        internal const string ScaffoldPathForChromosomes = "ScaffoldPathForChromosomes";
        internal const string ScaffoldPathForViralGenomeReads = "ScaffoldPathForViralGenomeReads";
        internal const string AssembledPathForForwardWithReverseCompl = "AssembledPathForForwardWithReverseCompl";
        internal const string AssembledPathForReverseWithReverseCompl = "AssembledPathForReverseWithReverseCompl";
        internal const string AssembledPathForForwardAndPalContig = "AssembledPathForForwardAndPalContig";
        internal const string AssembledPathForReverseAndPalContig = "AssembledPathForReverseAndPalContig";
        internal const string AssembledPathForChromosomeReads = "AssembledPathForChromosomeReads";
        internal const string AssembledPathForViralGenomeReads = "AssembledPathForViralGenomeReads";
        internal const string ScaffoldSequenceNode = "ScaffoldSequence";
        internal const string ScaffoldSeq = "ScaffoldSeq";
        internal const string AssembledSequencesForSequenceReadsNode =
            "AssembledSequencesForSequenceReads";
        internal const string AssembledSeqCountNode = "AssembledSeqCount";
        internal const string AssembledSequencesForEulerDataNode =
            "AssembledSequencesForEulerData";
        internal const string AssembledSequencesForViralGenomeReadsNode =
            "AssembledSequencesForViralGenomeReads";
        internal const string AssembledSequencesForForwardAndRevComplContigNode =
            "AssembledSequencesForForwardAndRevComplContig";
        internal const string AssembledSequencesForForwardAndPalContigNode =
            "AssembledSequencesForForwardAndPalContig";
        internal const string AssembledContigsForSequenceReadsNode =
            "AssembledContigsForSequenceReads";
        internal const string AssembledSequencesForSequenceReadsWithErosionNode =
            "AssembledSequencesForSequenceReadsWithErosion";
        internal const string AssembledSequencesForSequenceReadsWithErosionAndLCCNode =
            "AssembledSequencesForSequenceReadsWithErosionAndLCC";
        internal const string AssembledSequencesForSequenceReadsWithLCCNode =
            "AssembledSequencesForSequenceReadsWithLCC";
        internal const string ErosionNode = "Erosion";
        internal const string LowCoverageContigNode = "LowCoverageContig";

        // ClustalW Constant Variables
        internal const string DefaultOptionNode = "DefaultOption";
        internal const string EmailIDNode = "EmailID";
        internal const string ActionAlignNode = "ActionAlign";
        internal const string ClusterOptionNode = "ClusterOption";
        internal const int ClusterRetryInterval = 2000;
        internal const string AzureUri = "http://azureblast2.cloudapp.net/BlastService.svc";

        internal const string MultiSequenceFileNodeName = "MultiSequenceFile";
        internal const string HugeFastAFileYAxisNodeName = "HugeFastAFileYAxis";
        internal const string HugeFastAFileXAxisNodeName = "HugeFastAFileXAxis";

        // BAM Parser and Formatter config values.
        internal const string SmallSizeBAMFileNode = "SmallSizeBAMFile";
        internal const string RecordTagKeysNode = "RecordTagKeys";
        internal const string RecordTagValuesNode = "RecordTagValues";
        internal const string HeaderTyepsNodes = "HeaderTyeps";
        internal const string RefIndexNode = "RefIndex";
        internal const string SeqRangeBAMFileNode = "SeqRangeBAMFile";
        internal const string AlignedSeqCountNode = "AlignedSeqCount";
        internal const string BAMTempFileName = "Temp.bam";
        internal const string BAMToSAMConversionNode = "BAMTOSAMConversion";
        internal const string BAMTempIndexFile = "Temp.bam.bai";
        internal const string BAMTempIndexFileForIndexData = "TempIndexFile.bam.bai";
        internal const string BAMTempIndexFileForSequenceAlignment = "TempIndexFileSequence.bam.bai";
        internal const string BAMTempIndexFileForInvalidData = "InvalidTempIndexFile.bam.bai";
        internal const string BAMFileWithRefSeqNode = "BAMFileWithRefSeq";
        internal const string BAMFileWithMultipleAlignedSeqsNode =
            "BAMFileWithMultipleAlignedSeqs";
        internal const string BAMFileWithQualityValuesNode =
            "BAMFileWithQualityValues";
        internal const string CigarsNode = "Cigars";
        internal const string BinsNode = "Bins";
        internal const string QNamesNode = "QNames";
        internal const string ChromosomeNameNode = "ChromosomeName";
        internal const string BAMFileWithSequenceRangeSeqsNode =
            "BAMFileWithSequenceRangeSeqs";
        internal const string BAMFileWithSequenceRangeRefSeqsNode =
            "BAMFileWithSequenceRangeRefSeqs";
        internal const string MediumSizeBAMFileNode = "MediumSizeBAMFile";
        internal const string MediumSizeBAMFileWithSmallerEndIndexNode =
            "MediumSizeBAMFileWithSmallerEndIndex";
        internal const string MediumSizeBAMFileWithRefIndexNode =
            "MediumSizeBAMFileWithRefIndex";
        internal const string BAMAlignedSeqPropertiesNode =
            "BAMAlignedSeqProperties";
        internal const string FlagValueNode = "FlagValue";
        internal const string Isize = "ISize";
        internal const string MapQValue = "MapQ";
        internal const string Metadata = "Metadata";
        internal const string MPos = "MPos";
        internal const string OptionalFieldsNode = "OptionalFields";
        internal const string Pos = "Pos";
        internal const string QueryLength = "QueryLength";
        internal const string RName = "RName";
        internal const string SAMToBAMConversionForMultipleAlignedSeqNode =
            "SAMToBAMConversionForMultipleAlignedSeq";
        internal const string BAMIndexFileNode = "BAMIndexFile";
        internal const string SAMToBAMConversionForQualitySeqsNode =
            "SAMToBAMConversionForQualitySeqs";
        internal const string BAMIndexNode = "BAMIndex";
        internal const string BAMIndexCountNode = "BAMIndexCount";
        internal const string BAMFileName = "BAM";
        internal const string BAMFileType = ".bam";
        internal const string InvalidBAMFileNode = "InvalidBAMFile";
        internal const string BAMDescription =
            "A BAMParser reads from a source of binary data that is formatted according "
            + "to the BAM file specification, and converts the data to in-memory SequenceAlignmentMap object.";
        internal const string BAMFormatterDescription =
            "Writes a SequenceAlignmentMap to a particular location, usually a file. The output is formatted according to the BAM file format.";
        internal const string ExpectedAlignedSeqCountNode = "ExpectedAlignedSeqCount";
        internal const string ExpectedSeqWithPointersNode = "ExpectedSeqWithPointers";
        internal const string LineNumberToPointNode = "LineNumberToPoint";
        internal const string VirtualAlignedSeqCountNode =
            "VirtualAlignedSeqCount";
        internal const string MediumSizeFileNode =
             "MediumSizeFile";
        internal const string FirstBlockSequenceNode =
            "FirstBlockSequence";
        internal const string MiddleBlockSequenceNode =
            "MiddleBlockSequence";
        internal const string LastBlockSequenceNode =
            "LastBlockSequence";
        internal const string BAMFileFormatWithMultipleAlignedSeqsNode =
            "BAMFileFormatWithMultipleAlignedSeqs";
        internal const string SAMToBAMConversionForMultipleQualitySeqsNode =
            "BAMToSAMConversionForQualitySeqs";
        internal const string SAMToBAMConversionForMultipleAlignedSeqWithDVNode =
            "SAMToBAMConversionForMultipleAlignedSeqWithDV";

        internal const string HugeFastQFileYAxisNodeName = "HugeFastQFileYAxis";
        internal const string HugeFastQFileXAxisNodeName = "HugeFastQFileXAxis";
        internal const string SimpleFastQDnaDVNodeName = "SimpleFastQDnaDV";
        internal const string PairedReadForSmallFileNodeName =
            "PairedReadForSmallFile";
        internal const string PairedReadsNode =
           "PairedReadsCount";
        internal const string MeanNode = "Mean";
        internal const string DeviationValueNode = "deviation";
        internal const string PairedReadTypeNode = "PairedReadType";
        internal const string InsertLengthNode = "InsertLength";
        internal const string LibraryNameNode = "LibraryName";
        internal const string PairedReadTypesNode = "PairedReadTypes";
        internal const string PairedReadTypesForMeanAndDeviationNode =
            "PairedReadTypesForMeanAndDeviation";
        internal const string PairedReadTypesForReadsNode =
            "PairedReadTypesForReads";
        internal const string PairedReadTypesForLibraryInfoNode =
            "PairedReadTypesForLibraryInfo";


        internal const string MediumSizeBAMSortOutputMatchReadNames = "MediumSizeBAMSortOutputMatchReadNames";
        internal const string MediumSizeBAMSortOutputMatchChromosomeCoordinates = "MediumSizeBAMSortOutputMatchChromosomeCoordinates";
        internal const string MediumSizeBAMSortOutputMatchChromosomeNameAndCoordinates = "MediumSizeBAMSortOutputMatchChromosomeNameAndCoordinates";

        // Xsv Sparse Parser config values
        internal const string SimpleXsvSparseNodeName = "SimpleXsvSparse";
        internal const char CharSeperator = ',';
        internal const char SequenceIDPrefix = '#';
        internal const string XsvSparseDescription = "Parses sparse sequences from character separated value reader";
        internal const string XsvSparseFileTypes = "csv,tsv";
        internal const string XsvSparseName = "XsvSparseParser";
        internal const string SparseSerializeFile = "SparseSequence.data";
        internal const string XsvTempFileName = "TempXsv.csv";
        internal const string expectedString = "ExpectedString";
        internal const string XsvSparseFormatterDescription =
            "Sparse Sequence formatter to character separated value file";
        internal const string XsvSparseFormatterNode = "XsvSparseFormatter";
        internal const char XsvSeperator = ',';
        internal const char XsvSeqIdPrefix = '#';

        // Matrix test cases config values
        internal const string SimpleMatrixNodeName = "SimpleMatrix";
        internal const string RowsNode = "Rows";
        internal const string DenseAnsiStringNode = "DenseAnsiString";
        internal const string DenseMatrixStringNode = "DenseMatrixString";
        internal const string DenseConvertValueStringNode = "DenseConvertValueString";
        internal const string DenseStringJoinNode = "DenseStringJoin";
        internal const string DenseStringJoinSeparatorNode = "DenseStringJoinSeparator";
        internal const string DenseStringJoinSeparatorEtcNode = "DenseStringJoinSeparatorEtc";
        internal const string DenseTransposeStringNode = "DenseTransposeString";
        internal const string ValueStringNode = "ValueString";
        internal const string SparseMatrixStringNode = "SparseMatrixString";
        internal const string KeysTempFile = "keys.txt";

        // Bio StreamReader Cases config values
        internal const string SimpleFastAStreamReaderNode = "SimpleFastAStreamReader";
        internal const string ExpectedLinesNode = "ExpectedLines";
        internal const string SimpleFastQStreamReaderNode = "SimpleFastQStreamReader";
        internal const string SimpleSAMStreamReaderNode = "SimpleSAMStreamReader";
        internal const string SimpleFastAStreamReaderWithBlankLinesNode =
            "SimpleFastAStreamReaderWithBlankLines";
        internal const string SimpleFastQStreamReaderWithBlankLinesNode =
            "SimpleFastQStreamReaderWithBlankLines";
        internal const string SimpleSAMStreamReaderWithBlankLinesNode =
            "SimpleSAMStreamReaderWithBlankLines";
        internal const string CharsCountNode = "CharsCount";
        internal const string CharsStartIndexNode = "CharsStartIndex";
        internal const string NewLineCharacterCountNode = "NewLineCharacterCount";
        internal const string PositionNode = "Position";
        internal const string NumberOfNewlineCharactersNode = "NumberOfNewlineCharacters";
        internal const string CurrentLineStartingIndexNode = "CurrentLineStartingIndex";
        internal const string ExpectedLineNode = "ExpectedLine";
        internal const string ExpectedHeaderNode = "ExpectedHeader";
        internal const string BioTextReaderValidationNode = "BioTextReaderValidation";

        internal const string ClustCount1Node = "ClustCount1";
        internal const string ClustCount2Node = "ClustCount2";

        //PCA config values.
        internal const string RepeatResolution = "RepeatResolution";
        internal const string RepeatResolutionWithPairedReadsNode = "RepeatResolutionWithPairedReads";
        internal const string RepeatResolutionWithErrorsInPairedReadsNode = "RepeatResolutionWithErrorsInPairedReads";
        internal const string CountOfExpectedSequencesNode = "CountOfExpectedSequences";
        internal const string FirstSequenceEndNode = "FirstSequenceEndNode";
        internal const string FirstSequenceStart = "FirstSequenceStartNode";
        internal const string DeltasNode = "DeltasNode";
        internal const string SecondSequenceStart = "SecondSequenceStartNode";
        internal const string SecondSequenceEndNode = "SecondSequenceEndNode";
        internal const string RefineLayout = "RefineLayout";
        internal const string RefineLayoutWithPairedReadsNode = "RefineLayoutWithPairedReads";
        internal const string RefineLayoutWithNullDeltasNode = "RefineLayoutWithNullDeltas";
        internal const string ResolveAmbiguityWithNoReadsNode = "ResolveAmbiguityWithNoReads";
        internal const string ResolveAmbiguityWithNullDeltasNode = "ResolveAmbiguityWithNullDeltas";
        internal const string ConsensusWithNullDeltaNode = "ConsensusWithNullDelta";
        internal const string FastaWithOneLineSequenceNode = "FastaWithOneLineSequence";
        internal const string FastaOneLineSequenceWithAdditionNode = "FastaOneLineSequenceWithAddition";
        internal const string FastaOneLineSequenceWithDeletionNode = "FastaOneLineSequenceWithDeletion";
        internal const string SequenceWithErrorsReadsNode = "SequenceWithErrorsReads";
        internal const string LargeSequenceWithDeletedReadsNode = "LargeSequenceWithDeletedReads";
        internal const string LargeSequenceWithAddedReadsNode = "LargeSequenceWithAddedReads";
        internal const string EcoliDataWithErrorsNode = "EcoliDataWithErrors";
        internal const string EcoliDataWithZeroErrorsNodeNonPaired = "EcoliDataWithZeroErrorsNonPaired";
        internal const string EcoliDataWithZeroErrorsPairedNode = "EcoliDataWithZeroErrorsPaired";
        internal const string SequenceWithZeroErrorsAnd10XCoverageNode = "SequenceWithZeroErrorsAnd10XCoverage";
        internal const string SequenceWithZeroErrorsAnd1XCoverageNode = "SequenceWithZeroErrorsAnd1XCoverage";
        internal const string SequenceWithZeroErrorsAnd10XCoveragePairedNode = "SequenceWithZeroErrorsAnd10XCoveragePaired";
        internal const string SequenceWithZeroErrorsAnd1XCoveragePairedNode = "SequenceWithZeroErrorsAnd1XCoveragePaired";
        internal const string FastaWithOneLineSequenceNodeWithErrors = "FastaWithOneLineSequenceNodeWithErrors";
        internal const string EColi500WithZeroError1XPairedReadsNode = "EColi500WithZeroError1XPairedReads";
        internal const string ApplicationLogNode = "ApplicationLog";
        internal const string AdditionalArgumentsNode = "AdditionalArguments";
        internal const string UtilParserNode = "UtilParser";
        internal const string SequenceTypeNode = "SequenceType";
        internal const string SequenceValuesNode = "SequenceValues";
        internal const string UtilTraceNode = "UtilTrace";
        internal const string ContextParameterNode = "context";
        internal const string DataParameterNode = "data";
        internal const string MessageNode = "message";
        internal const string StringExtensionsNode = "StringExtensions";
        internal const string RemoveEmptyItems = "removeEmptyItems";
        internal const string TextItems = "text";
        internal const string PrimitiveExtensionsNode = "PrimitiveExtensions";
        internal const string IEnumerableExtensionsNode = "IEnumerableExtensions";
        internal const string EtcStringNode = "EtcString";
        internal const string ParallelOptionsScopeNode = "ParallelOptionsScope";
        internal const string HelperNode = "Helper";

        //Clone Library Information Config values.
        internal const string CloneLibraryInformationNode = "CloneLibraryInformation";

        //Sequence Range and Formatters values.
        internal const string BedParser = "Bio.IO.Bed.BedParser";
        internal const string FastAParser = "Bio.IO.FastA.FastAParser";
        internal const string FastQParser = "Bio.IO.FastQ.FastQParser";
        internal const string GenBankParser = "Bio.IO.GenBank.GenBankParser";
        internal const string GffParser = "Bio.IO.Gff.GffParser";
        internal const string BedFormatter = "Bio.IO.Bed.BedFormatter";
        internal const string FastAFormatter = "Bio.IO.FastA.FastAFormatter";
        internal const string FastQFormatter = "Bio.IO.FastQ.FastQFormatter";
        internal const string GenBankFormatter = "Bio.IO.GenBank.GenBankFormatter";
        internal const string GffFormatter = "Bio.IO.Gff.GffFormatter";
        internal const string InvalidFileName = "InvalidName.fast";

        //Word Match Values.
        internal const string InsertionOfOneBaseIn2 = "Insertion of 1 bases in 2 ";
        internal const string InsertionOfOneBaseIn1 = "Insertion of 1 bases in 1 ";

        //Bio.Utils Values
        internal const string CallingAssemblyName = "CallingAssembly";
        internal const string CallingAssemblyFullNode = "CallingAssemblyFull";
        //ToString Test Cases Values
        internal const string ToStringNodeName = "ToString";
        internal const string SequenceStatisticsNode = "SequenceStatistics";
        internal const string AlignedSeqActualNode = "AlignedSeqActual";
        internal const string AlignedSeqExpectedNode = "AlignedSeqExpected";
        internal const string ClusterExpectedNode = "ClusterExpected";
        internal const string seqLargeStringNode = "seqLargeString";
        internal const string seqLargeExpectedNode = "seqLargeExpected";
        internal const string seqLargeExpected2Node = "seqLargeExpected2";
        internal const string Seq1StrNode = "Seq1Str";
        internal const string Seq2StrNode = "Seq2Str";
        internal const string OverlapDenovoExpectedNode = "OverlapDenovoExpected";
        internal const string MatePairExpectedNode = "MatePairExpected";
        internal const string ExpectedMatchExtnStringNode = "ExpectedMatchExtnString";
        internal const string ExpectedMatchStringNode = "ExpectedMatchString";
        internal const string DeltaAlignmentExpectedNode = "DeltaAlignmentExpected";
        internal const string SequenceRangeGroupingExpectedNode = "SequenceRangeGroupingExpected";
        internal const string SequenceAlignmentExpectedNode = "SequenceAlignmentExpected";

        //Wiggle config values.
        internal const string SimpleWiggleWithFixedStepNodeName="SimpleWiggleWithFixedStep";
        internal const string SimpleWiggleWithVariableStepNodeName = "SimpleWiggleWithVariableStep";        
        internal const string BasePositionNode="BasePosition";
        internal const string ChromosomeNode="Chromosome";
        internal const string AnnotationCountNode = "AnnotationCount";
        internal const string ExpectedValuesNode="ExpectedValues";
        internal const string ExpectedKeysNode = "ExpectedKeys";
        internal const string ExpectedPointsNode="ExpectedPoints";
        internal const string AnnotationMetadataNode="AnnotationMetadata";
        internal const string ExpectedSpanNode = "ExpectedSpan";
        internal const string ExpectedStepNode="ExpectedStep";
        internal const string AnnotationTypeNode = "AnnotationType";
        internal const string ParserDescriptionNode = "ParserDescription";
        internal const string FormatterDescriptionNode="FormatterDescription";
        internal const string AnnotationValuesNode = "AnnotationValues";
        internal const string ExpectedStartNode = "ExpectedStart";
        internal const string InValidFileNamesNode = "InValidFileNames";
        internal const string InvalidTrackNode="InvalidTrack";
        internal const string InvalidAutoScaleNode = "InvalidAutoScale";
        internal const string DeltaAlignmentExpected2Node = "DeltaAlignmentExpected2";

        //web service handler constants
        internal const string EmailForWS = "biohpcmbf11@hotmail.com";

        //Comparative assembly constants
        internal const string BreakLengthNode = "BreakLength";
    }
}
