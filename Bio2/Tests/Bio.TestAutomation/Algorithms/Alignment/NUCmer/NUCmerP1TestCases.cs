/****************************************************************************
 * NUCmerP1TestCases.cs
 * 
 *   This file contains the NUCmer P1 test cases
 * 
***************************************************************************/

using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Bio;
using Bio.IO;
using Bio.Algorithms;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.SuffixTree;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.SimilarityMatrices;
using Bio.Algorithms.MUMmer;
using Bio.IO.FastA;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    /// NUCmer P1 Test case implementation.
    /// </summary>
    [TestClass]
    public class NUCmerP1TestCases
    {

        #region Enums

        /// <summary>
        /// Lis Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AdditionalParameters
        {
            FindUniqueMatches,
            PerformClusterBuilder,
            AlignSimilarityMatrix,
            Default
        };

        /// <summary>
        /// Parser Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum ParserParameters
        {
            FastA,
            GenBank,
            Gff
        };

        /// <summary>
        /// Parameters which are used for different test cases 
        /// based on which the properties are updated.
        /// </summary>
        enum PropertyParameters
        {
            MaximumSeparation,
            MinimumScore,
            SeparationFactor,
            FixedSeparation,
            FixedSeparationAndSeparationFactor,
            MaximumFixedAndSeparationFactor,
            Default
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\NUCmerTestsConfig.xml");
        ASCIIEncoding encodingObj = new ASCIIEncoding();

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NUCmerP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.automation.log");
            }
        }

        #endregion Constructor

        #region Suffix Tree Test Cases

        /// <summary>
        /// Validate FindMatches() method with one line sequences
        /// and valid MUM length for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with Valid MUM length
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesOneLineSequenceValidMUMLength()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with DNA sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with Valid MUM length
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesDnaSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.DnaNucmerSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with RNA sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with Valid MUM length
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesRnaSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.RnaNucmerSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with medium sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : Medium size reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesMediumSizeSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.MediumSizeSequenceNodeName,
                true, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with continous repeating character sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with continous
        /// repeating characters
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesContinousRepeatingSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineRepeatingCharactersNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with same sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with same characters
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesSameSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.SameSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with overlap sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesWithCrossOverlapSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with no match sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with no match
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesWithNoMatchSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineNoMatchSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with overlap sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesWithOverlapSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithoutCrossOverlapSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with ambiguity characters in
        /// reference Dna sequence and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with ambiguity
        /// characters in reference Dna sequence
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityDnaReferenceSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.DnaAmbiguityReferenceSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with ambiguity characters in
        /// search Dna sequence and reference parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with ambiguity
        /// characters in search Dna sequence
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityDnaSearchSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.DnaAmbiguitySearchSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with ambiguity characters in
        /// reference Rna sequence and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with ambiguity
        /// characters in reference Rna sequence
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityRnaReferenceSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.RnaAmbiguityReferenceSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with ambiguity characters in
        /// search Rna sequence and reference parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter with ambiguity
        /// characters in search Rna sequence
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityRnaSearchSequences()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.RnaAmbiguitySearchSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// and without cross over lap and validate the clusters
        /// Input : Two unique matches without cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderTwoUniqueMatchesWithoutCrossOverlap()
        {
            ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithoutCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// and with cross over lap and validate the clusters
        /// Input : Two unique matches with cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderTwoUniqueMatchesWithCrossOverlap()
        {
            ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// and with overlap and no cross overlap and validate the clusters
        /// Input : Two unique matches with overlap and no cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithOverlapNoCrossOverlap()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with Minimum Score set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with minimum score 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithMinimumScoreZero()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder, PropertyParameters.MinimumScore);
        }

        /// <summary>
        /// Validate BuildCluster() method with MaximumSeparation set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with MaximumSeparation 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithMaximumSeparationZero()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder, PropertyParameters.MaximumSeparation);
        }

        /// <summary>
        /// Validate BuildCluster() method with SeperationFactor set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with SeperationFactor 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithSeperationFactoreZero()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder, PropertyParameters.SeparationFactor);
        }

        /// <summary>
        /// Validate BuildCluster() method with FixedSeparation set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with FixedSeparation 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithFixedSeparationZero()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder, PropertyParameters.FixedSeparation);
        }

        /// <summary>
        /// Validate BuildCluster() method with 
        /// MinimumScore set to greater than MUMlength 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with 
        /// MinimumScore set to greater than MUMlength
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithMinimumScoreGreater()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder, PropertyParameters.MinimumScore);
        }

        /// <summary>
        /// Validate BuildCluster() method with 
        /// FixedSeparation set to postive value and SeparationFactor=0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with 
        /// FixedSeparation set to postive value and SeparationFactor=0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithFixedSeparationAndSeparationFactor()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder,
                PropertyParameters.FixedSeparationAndSeparationFactor);
        }

        /// <summary>
        /// Validate BuildCluster() method with 
        /// MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with 
        /// MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0 
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ClusterBuilderWithMaximumFixedAndSeparationFactor()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder,
                PropertyParameters.MaximumFixedAndSeparationFactor);
        }

        #endregion Suffix Tree Test Cases

        #region NUCmer Align Test Cases

        /// <summary>
        /// Validate Align() method with one line Dna sequence 
        /// and validate the aligned sequences
        /// Input : One line Dna sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignDnaSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.DnaNucmerSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with one line Rna sequence 
        /// and validate the aligned sequences
        /// Input : One line Rna sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignRnaSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.RnaNucmerSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with one line list of sequence 
        /// and validate the aligned sequences
        /// Input : One line list of sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOneLineListOfSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineOneReferenceQuerySequenceNodeName,
                false, true, false);
        }

        /// <summary>
        /// Validate Align() method with small size list of sequence 
        /// and validate the aligned sequences
        /// Input : small size list of sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSmallSizeListOfSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineOneReferenceQuerySequenceNodeName,
                false, true, false);
        }

        /// <summary>
        /// Validate Align() method with one line Dna list of sequence 
        /// and validate the aligned sequences
        /// Input : One line Dna list of sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOneLineDnaListOfSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, true, false);
        }

        /// <summary>
        /// Validate Align() method with one line Rna list of sequence 
        /// and validate the aligned sequences
        /// Input : One line Rna list of sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOneLineRnaListOfSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, false, true, false);
        }

        /// <summary>
        /// Validate Align() method with medium size sequence 
        /// and validate the aligned sequences
        /// Input : Medium size sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignMediumSizeSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.MediumSizeSequenceNodeName, true, false, false);
        }

        /// <summary>
        /// Validate Align() method with One Line Repeating Characters sequence 
        /// and validate the aligned sequences
        /// Input : One Line Repeating Characters sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignRepeatingCharactersSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineRepeatingCharactersNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with One Line Alternate Repeating Characters sequence 
        /// and validate the aligned sequences
        /// Input : One Line Alternate Repeating Characters sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignAlternateRepeatingCharactersSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineAlternateRepeatingCharactersNodeName,
                false, false, false);
        }

        /// <summary>
        /// Validate Align() method with FastA file sequence 
        /// and validate the aligned sequences
        /// Input : FastA file sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignFastASequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SimpleDnaFastaNodeName, true, false,
                ParserParameters.FastA, false);
        }

        /// <summary>
        /// Validate Align() method with One Line only Repeating Characters sequence 
        /// and validate the aligned sequences
        /// Input : One Line only Repeating Characters sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOnlyRepeatingCharactersSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineOnlyRepeatingCharactersNodeName,
                false, false, false);
        }

        /// <summary>
        /// Validate Align() method with one reference multi search sequence 
        /// and validate the aligned sequences
        /// Input : one reference multi search sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOneRefMultiSearchSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SmallSizeSequenceNodeName, true, false, false);
        }

        /// <summary>
        /// Validate Align() method with valid MUM length 
        /// and validate the aligned sequences
        /// Input : One line sequence with valid MUM length
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOneLineSequenceValidMumLength()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with One Line same sequences
        /// and validate the aligned sequences
        /// Input : One Line same sequences
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSameSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SameSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with overlap sequences
        /// and validate the aligned sequences
        /// Input : One Line overlap sequences
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOverlapMatchSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneOverlapMatchSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with no match sequences
        /// and validate the aligned sequences
        /// Input : One Line no match sequences
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignNoMatchSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineNoMatchSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with cross overlap sequences
        /// and validate the aligned sequences
        /// Input : One Line cross overlap sequences
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignCrossOverlapMatchSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                false, false, false);
        }

        /// <summary>
        /// Validate Align() method with one line reference Dna sequence with ambiguity
        /// and validate the aligned sequences
        /// Input : One line Dna sequence with ambiguity
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignDnaReferenceAmbiguitySequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.DnaAmbiguityReferenceSequenceNodeName,
                false, false, true);
        }

        /// <summary>
        /// Validate Align() method with one line reference Rna sequence with ambiguity
        /// and validate the aligned sequences
        /// Input : One line Rna sequence with ambiguity
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignRnaReferenceAmbiguitySequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.RnaAmbiguityReferenceSequenceNodeName,
                false, false, true);
        }

        /// <summary>
        /// Validate Align() method with one line search Dna sequence with ambiguity
        /// and validate the aligned sequences
        /// Input : One line Dna sequence with ambiguity
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignDnaSearchAmbiguitySequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.DnaAmbiguitySearchSequenceNodeName, false, false, true);
        }

        /// <summary>
        /// Validate Align() method with one line search Rna sequence with ambiguity
        /// and validate the aligned sequences
        /// Input : One line Rna sequence with ambiguity
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignRnaSearchAmbiguitySequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.RnaAmbiguitySearchSequenceNodeName, false, false, true);
        }

        /// <summary>
        /// Validate Align() method with one ref. and one query sequence 
        /// and validate the aligned sequences
        /// Input : one reference and one query sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignOneRefOneQuerySequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate Align() method with multi reference one search sequence 
        /// and validate the aligned sequences
        /// Input : multiple reference one search sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignMultiRefOneSearchSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.MultiRefSingleQueryMatchSequenceNodeName,
                false, false, false);
        }

        /// <summary>
        /// Validate Align() method with multi reference multi search sequence 
        /// and validate the aligned sequences
        /// Input : multiple reference multi search sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignMultiRefMultiSearchSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName, false, false, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with Minimum Score set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with minimum score 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithMinimumScoreZero()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, false, ParserParameters.FastA, AdditionalParameters.Default,
                PropertyParameters.MinimumScore, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with MaximumSeparation set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with MaximumSeparation 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithMaximumSeparationZero()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                  false, false, ParserParameters.FastA, AdditionalParameters.Default,
                  PropertyParameters.MaximumSeparation, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with SeperationFactor set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with SeperationFactor 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithSeperationFactoreZero()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, false, ParserParameters.FastA, AdditionalParameters.Default,
                PropertyParameters.SeparationFactor, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with FixedSeparation set to 0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with FixedSeparation 0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithFixedSeparationZero()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, false, ParserParameters.FastA, AdditionalParameters.Default,
                PropertyParameters.FixedSeparation, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with 
        /// MinimumScore set to greater than MUMlength 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with 
        /// MinimumScore set to greater than MUMlength
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithMinimumScoreGreater()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                false, false, ParserParameters.FastA, AdditionalParameters.Default,
                PropertyParameters.MinimumScore, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with 
        /// FixedSeparation set to postive value and SeparationFactor=0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with 
        /// FixedSeparation set to postive value and SeparationFactor=0
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithFixedSeparationAndSeparationFactor()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                false, false, ParserParameters.FastA, AdditionalParameters.Default,
                PropertyParameters.FixedSeparationAndSeparationFactor, false);
        }

        /// <summary>
        /// Validate BuildCluster() method with 
        /// MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0 
        /// and validate the clusters
        /// Input : Reference and Search Sequences with 
        /// MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0 
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithMaximumFixedAndSeparationFactor()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                false, false, ParserParameters.FastA, AdditionalParameters.Default,
                PropertyParameters.MaximumFixedAndSeparationFactor, false);
        }

        /// <summary>
        /// Validate Align() method with IsAlign set to true
        /// and with gaps in the reference and query sequence.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)"), TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignWithIsAlignAndGaps()
        {
            IList<ISequence> seqList = new List<ISequence>();
            seqList.Add(new Sequence(Alphabets.DNA,
                encodingObj.GetBytes("CAAAAGGGATTGC---TGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGTAGTACAAAGGAGC")));
            seqList.Add(new Sequence(Alphabets.DNA,
                encodingObj.GetBytes("CAAAAGGGATTGC---")));
            seqList.Add(new Sequence(Alphabets.DNA,
                encodingObj.GetBytes("TAGTAGTTCTGCTATATACATTTG")));
            seqList.Add(new Sequence(Alphabets.DNA,
                encodingObj.GetBytes("GTTATCATGCGAACAATTCAACAGACACTGTAGA")));
            NucmerPairwiseAligner num = new NucmerPairwiseAligner();
            num.BreakLength = 8;
            num.FixedSeparation = 0;
            num.MinimumScore = 0;
            num.MaximumSeparation = 0;
            num.SeparationFactor = 0;
            num.LengthOfMUM = 8;
            IList<ISequence> sequenceList = seqList;
            IList<ISequenceAlignment> alignmentObj = num.Align(sequenceList);

            AlignedSequence alignedSeqs = (AlignedSequence)alignmentObj[0].AlignedSequences[0];
            Assert.AreEqual("CAAAAGGGATTGC---", new string(alignedSeqs.Sequences[0].Select(a => (char)a).ToArray()));
            Assert.AreEqual("CAAAAGGGATTGC---", new string(alignedSeqs.Sequences[1].Select(a => (char)a).ToArray()));

            Console.WriteLine("Successfully falidated Align method with IsAlign and Gaps");
            ApplicationLog.WriteLine("Successfully falidated Align method with IsAlign and Gaps");
        }

        #endregion NUCmer Align Test Cases

        #region NUCmer Align Simple Test Cases

        /// <summary>
        /// Validate AlignSimple() method with one line Dna list of sequence 
        /// and validate the aligned sequences
        /// Input : Dna list of sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleDnaListOfSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Rna list of sequence 
        /// and validate the aligned sequences
        /// Input : Rna list of sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleRnaListOfSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Dna sequence 
        /// and validate the aligned sequences
        /// Input : Single Reference and Single Query Dna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleSingleRefSingleQueryDnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Rna sequence 
        /// and validate the aligned sequences
        /// Input : Single Reference and Single Query Rna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleSingleRefSingleQueryRnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Dna sequence 
        /// and validate the aligned sequences
        /// Input : Single Reference and Multi Query Dna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleSingleRefMultiQueryDnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Rna sequence 
        /// and validate the aligned sequences
        /// Input : Single Reference and Multi Query Rna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleSingleRefMultiQueryRnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Dna sequence 
        /// and validate the aligned sequences
        /// Input : Multi Reference and Multi Query Dna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleMultiRefMultiQueryDnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.MultiRefMultiQueryDnaMatchSequence,
                false, true);
        }

        /// <summary>
        /// Validate AlignSimple() method with one line Rna sequence 
        /// and validate the aligned sequences
        /// Input : Multi Reference and Multi Query Rna sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void NUCmerAlignSimpleMultiRefMultiQueryRnaSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.MultiRefMultiQueryRnaMatchSequence,
                false, true);
        }

        #endregion NUCmer Align Simple Test Cases

        #region Supported Methods

        /// <summary>
        /// Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="additionalParam">LIS action type enum</param>
        void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath,
            AdditionalParameters additionalParam)
        {
            ValidateFindMatchSuffixGeneralTestCases(nodeName, isFilePath, additionalParam,
                PropertyParameters.Default);
        }

        /// <summary>
        /// Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="additionalParam">LIS action type enum</param>
        /// <param name="propParam">Property parameters</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath,
            AdditionalParameters additionalParam, PropertyParameters propParam)
        {
            ISequence referenceSeq = null;
            string[] referenceSequences = null;
            string[] searchSequences = null;
            IEnumerable<ISequence> referenceSeqList = null;

            List<ISequence> searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer P1 : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser(filePath);
                referenceSeqList = parser.Parse();

                List<Byte> byteList = new List<Byte>();
                foreach (ISequence seq in referenceSeqList)
                {
                    byteList.AddRange(seq);
                    byteList.Add((byte)'+');
                }

                referenceSeq = new Sequence(AlphabetExtensions.GetMummerAlphabet(referenceSeqList.ElementAt(0).Alphabet),
                    byteList.ToArray());

                // Gets the query sequence from the FastA file
                string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer P1 : Successfully validated the File Path '{0}'.", queryFilePath));

                FastAParser queryParserObj = new FastAParser(queryFilePath);
                IEnumerable<ISequence> querySeqList = queryParserObj.Parse();

                foreach (ISequence seq in querySeqList)
                {
                    searchSeqList.Add(seq);
                }
            }
            else
            {
                // Gets the reference & search sequences from the configurtion file
                referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.ReferenceSequencesNode);
                searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                  Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                       Constants.AlphabetNameNode));

                List<ISequence> refSeqList = new List<ISequence>();

                for (int i = 0; i < referenceSequences.Length; i++)
                {
                    ISequence referSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(referenceSequences[i]));
                    refSeqList.Add(referSeq);
                }

                List<Byte> byteList = new List<Byte>();
                foreach (ISequence seq in refSeqList)
                {
                    byteList.AddRange(seq);
                    byteList.Add((byte)'+');
                }

                referenceSeq = new Sequence(AlphabetExtensions.GetMummerAlphabet(refSeqList.ElementAt(0).Alphabet),
                    byteList.ToArray());

                for (int i = 0; i < searchSequences.Length; i++)
                {
                    ISequence searchSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(searchSequences[i]));
                    searchSeqList.Add(searchSeq);
                }
            }

            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Builds the suffix for the reference sequence passed.           
            MultiWaySuffixTree suffixTreeBuilder = new MultiWaySuffixTree(referenceSeq as Sequence);
            suffixTreeBuilder.MinLengthOfMatch = long.Parse(mumLength, null);

            Dictionary<ISequence, IEnumerable<Match>> matches =
                new Dictionary<ISequence, IEnumerable<Match>>();

            for (int i = 0; i < searchSeqList.Count; i++)
            {
                matches.Add(searchSeqList[i],
                    suffixTreeBuilder.SearchMatchesUniqueInReference(searchSeqList[i]));
            }

            List<Match> mums = new List<Match>();
            foreach (var a in matches.Values)
            {
                mums.AddRange(a);
            }

            switch (additionalParam)
            {
                case AdditionalParameters.FindUniqueMatches:
                    // Validates the Unique Matches.
                    ApplicationLog.WriteLine("NUCmer P1 : Validating the Unique Matches");
                    Assert.IsTrue(ValidateUniqueMatches(mums, nodeName, isFilePath));
                    Console.WriteLine(
                        "NUCmer P1 : Successfully validated the all the unique matches for the sequences.");
                    break;
                case AdditionalParameters.PerformClusterBuilder:
                    // Validates the Unique Matches.
                    ApplicationLog.WriteLine(
                        "NUCmer P1 : Validating the Unique Matches using Cluster Builder");
                    Assert.IsTrue(ValidateClusterBuilderMatches(mums, nodeName, propParam));
                    Console.WriteLine(
                        "NUCmer P1 : Successfully validated the all the cluster builder matches for the sequences.");
                    break;
                default:
                    break;
            }


            ApplicationLog.WriteLine(
                "NUCmer P1 : Successfully validated the all the unique matches for the sequences.");
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        void ValidateNUCmerAlignGeneralTestCases(string nodeName, bool isFilePath,
            bool isAlignList, bool isAmbiguous)
        {
            ValidateNUCmerAlignGeneralTestCases(nodeName, isFilePath, isAlignList,
                ParserParameters.FastA, isAmbiguous);
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName, bool isFilePath,
            bool isAlignList)
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(nodeName, isFilePath, isAlignList,
                ParserParameters.FastA);
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// <param name="parserParam">Parser type</param>
        void ValidateNUCmerAlignGeneralTestCases(string nodeName, bool isFilePath, bool isAlignList,
            ParserParameters parserParam, bool isAmbiguous)
        {
            ValidateNUCmerAlignGeneralTestCases(nodeName, isFilePath, isAlignList, parserParam,
                AdditionalParameters.Default, PropertyParameters.Default, isAmbiguous);
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// <param name="parserParam">Parser type</param>
        /// <param name="addParam">Additional parameters</param>
        /// <param name="propParam">Property parameters</param>
        /// 1801 is suppressed as this variable would be used later
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parserParam"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateNUCmerAlignGeneralTestCases(string nodeName, bool isFilePath, bool isAlignList,
            ParserParameters parserParam, AdditionalParameters addParam, PropertyParameters propParam, bool isAmbiguous)
        {
            string[] referenceSequences = null;
            string[] searchSequences = null;
            IList<ISequence> refSeqList = new List<ISequence>();
            IList<ISequence> searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer P1 : Successfully validated the File Path '{0}'.", filePath));

                FastAParser fastaparserobj = new FastAParser(filePath);
                IEnumerable<ISequence> referenceSeqList = fastaparserobj.Parse();

                foreach (ISequence seq in referenceSeqList)
                {
                    refSeqList.Add(seq);
                }

                // Gets the query sequence from the FastA file
                string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer P1 : Successfully validated the File Path '{0}'.", queryFilePath));

                FastAParser queryParserobj = new FastAParser(queryFilePath);
                IEnumerable<ISequence> serSeqList = queryParserobj.Parse();

                foreach (ISequence seq in serSeqList)
                {
                    searchSeqList.Add(seq);
                }
            }
            else
            {
                // Gets the reference & search sequences from the configurtion file
                referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.ReferenceSequencesNode);
                searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                  Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                       Constants.AlphabetNameNode));
                IAlphabet ambAlphabet = null;
                if (isAmbiguous)
                {
                    switch (seqAlphabet.Name.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "dna":
                        case "ambiguousdna":
                            ambAlphabet = AmbiguousDnaAlphabet.Instance;
                            break;
                        case "rna":
                        case "ambiguousrna":
                            ambAlphabet = AmbiguousRnaAlphabet.Instance;
                            break;
                        case "protein":
                        case "ambiguousprotein":
                            ambAlphabet = AmbiguousProteinAlphabet.Instance;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ambAlphabet = seqAlphabet;
                }

                for (int i = 0; i < referenceSequences.Length; i++)
                {
                    ISequence referSeq = new Sequence(ambAlphabet,
                        encodingObj.GetBytes(referenceSequences[i]));
                    referSeq.ID = "ref " + i;
                    refSeqList.Add(referSeq);
                }

                for (int i = 0; i < searchSequences.Length; i++)
                {
                    ISequence searchSeq = new Sequence(ambAlphabet,
                        encodingObj.GetBytes(searchSequences[i]));
                    searchSeq.ID = "qry " + i;
                    searchSeqList.Add(searchSeq);
                }
            }
            // Gets the mum length from the xml
            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();
            // Check for additional parameters and update the object accordingly
            switch (addParam)
            {
                case AdditionalParameters.AlignSimilarityMatrix:
                    nucmerObj.SimilarityMatrix = new SimilarityMatrix(
                        SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
                    break;
                default:
                    break;
            }
            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = 0;
            nucmerObj.MinimumScore = 2;
            nucmerObj.SeparationFactor = 0.12f;
            nucmerObj.BreakLength = 2;
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);

            switch (propParam)
            {
                case PropertyParameters.MinimumScore:
                    nucmerObj.MinimumScore = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MinimumScoreNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.MaximumSeparation:
                    nucmerObj.MaximumSeparation = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.FixedSeparation:
                    nucmerObj.FixedSeparation = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.SeparationFactor:
                    nucmerObj.SeparationFactor = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.FixedSeparationAndSeparationFactor:
                    nucmerObj.SeparationFactor = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), (IFormatProvider)null);
                    nucmerObj.FixedSeparation = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.MaximumFixedAndSeparationFactor:
                    nucmerObj.MaximumSeparation = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode), (IFormatProvider)null);
                    nucmerObj.SeparationFactor = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), (IFormatProvider)null);
                    nucmerObj.FixedSeparation = int.Parse(
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), (IFormatProvider)null);
                    break;
                default:
                    break;
            }

            IList<ISequenceAlignment> align = null;

            if (isAlignList)
            {
                List<ISequence> listOfSeq = new List<ISequence>();
                listOfSeq.Add(refSeqList.ElementAt(0));
                listOfSeq.Add(searchSeqList.ElementAt(0));
                align = nucmerObj.Align(listOfSeq);
            }
            else
            {
                align = nucmerObj.Align(refSeqList, searchSeqList);
            }

            string expectedSequences = string.Empty;

            if (isFilePath)
                expectedSequences = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                    Constants.ExpectedSequencesNode);
            else
                expectedSequences = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.ExpectedSequencesNode);
            string[] expSeqArray = expectedSequences.Split(',');



            // Gets all the aligned sequences in comma seperated format
            foreach (IPairwiseSequenceAlignment seqAlignment in align)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    string actualStr = new string(alignedSeq.FirstSequence.Select(a => (char)a).ToArray());
                    Assert.IsTrue(expSeqArray.Contains(actualStr));

                    actualStr = new string(alignedSeq.SecondSequence.Select(a => (char)a).ToArray());
                    Assert.IsTrue(expSeqArray.Contains(actualStr));

                }
            }

            Console.WriteLine("NUCmer P1 : Successfully validated all the aligned sequences.");
            ApplicationLog.WriteLine("NUCmer P1 : Successfully validated all the aligned sequences.");
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// <param name="parserParam">Parser type</param>
        /// 1801 is suppressed as this variable would be used later
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parserParam"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName,
            bool isFilePath, bool isAlignList, ParserParameters parserParam)
        {
            string[] referenceSequences = null;
            string[] searchSequences = null;
            IList<ISequence> refSeqList = new List<ISequence>();
            IList<ISequence> searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer P1 : Successfully validated the File Path '{0}'.", filePath));

                FastAParser fastaparserobj = new FastAParser(filePath);
                IEnumerable<ISequence> referenceSeqList = fastaparserobj.Parse();

                foreach (ISequence seq in referenceSeqList)
                {
                    refSeqList.Add(seq);
                }

                // Gets the query sequence from the FastA file
                string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer P1 : Successfully validated the File Path '{0}'.", queryFilePath));

                FastAParser fastaParserobj = new FastAParser(queryFilePath);
                IEnumerable<ISequence> querySeqList = fastaParserobj.Parse();
                foreach (ISequence seq in querySeqList)
                {
                    searchSeqList.Add(seq);
                }
            }
            else
            {
                // Gets the reference & search sequences from the configurtion file
                referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.ReferenceSequencesNode);
                searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                  Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                       Constants.AlphabetNameNode));

                for (int i = 0; i < referenceSequences.Length; i++)
                {
                    ISequence referSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(referenceSequences[i]));
                    refSeqList.Add(referSeq);
                }

                for (int i = 0; i < searchSequences.Length; i++)
                {
                    ISequence searchSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(searchSequences[i]));
                    searchSeqList.Add(searchSeq);
                }
            }

            // Gets the mum length from the xml
            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.MinimumScore = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.SeparationFactor = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.BreakLength = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.LengthOfMUM = long.Parse(mumLength, (IFormatProvider)null);

            IList<ISequenceAlignment> alignSimple = null;

            if (isAlignList)
            {
                List<ISequence> listOfSeq = new List<ISequence>();
                listOfSeq.Add(refSeqList[0]);
                listOfSeq.Add(searchSeqList[0]);
                alignSimple = nucmerObj.AlignSimple(listOfSeq);
            }

            string expectedSequences = string.Empty;

            if (isFilePath)
                expectedSequences = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                    Constants.ExpectedSequencesNode);
            else
                expectedSequences = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma seperated format
            foreach (IPairwiseSequenceAlignment seqAlignment in alignSimple)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.FirstSequence.Select(a => (char)a).ToArray()));
                    ++j;
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.SecondSequence.Select(a => (char)a).ToArray()));
                    j++;
                }
            }

            Console.WriteLine("NUCmer P1 : Successfully validated all the aligned sequences.");
            ApplicationLog.WriteLine("NUCmer P1 : Successfully validated all the aligned sequences.");
        }

        /// <summary>
        /// Validates the Unique Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Nodes to be read from Text file?</param>
        /// <returns>True, if successfully validated</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        bool ValidateUniqueMatches(IEnumerable<Match> matches,
            string nodeName, bool isFilePath)
        {
            string[] firstSeqOrder = null;
            string[] length = null;
            string[] secondSeqOrder = null;

            if (isFilePath)
            {
                firstSeqOrder = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                    Constants.FirstSequenceMumOrderNode).Split(',');
                length = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                    Constants.LengthNode).Split(',');
                secondSeqOrder = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                    Constants.SecondSequenceMumOrderNode).Split(',');
            }
            else
            {
                firstSeqOrder = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FirstSequenceMumOrderNode).Split(',');
                length = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.LengthNode).Split(',');
                secondSeqOrder = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SecondSequenceMumOrderNode).Split(',');
            }

            int i = 0;

            IList<MatchExtension> meNewObj = new List<MatchExtension>();

            foreach (Match m in matches)
            {
                meNewObj.Add(new MatchExtension(m));
            }

            // Order the mum list with query sequence order and
            // Assign query sequence to the MUM's
            for (int index = 0; index < meNewObj.Count(); index++)
            {
                meNewObj.ElementAt(index).ReferenceSequenceMumOrder = index + 1;
                meNewObj.ElementAt(index).QuerySequenceMumOrder = index + 1;
            }

            // Loops through all the matches and validates the same.
            foreach (MatchExtension match in meNewObj)
            {
                if ((0 != string.Compare(firstSeqOrder[i],
                    match.ReferenceSequenceMumOrder.ToString((IFormatProvider)null), true,
                    CultureInfo.CurrentCulture))
                    || (0 != string.Compare(length[i],
                    match.Length.ToString((IFormatProvider)null), true,
                    CultureInfo.CurrentCulture))
                    || (0 != string.Compare(secondSeqOrder[i],
                    match.QuerySequenceMumOrder.ToString((IFormatProvider)null), true,
                    CultureInfo.CurrentCulture)))
                {
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "NUCmer BVT : Unique match not matching at index '{0}'", i.ToString((IFormatProvider)null)));
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "NUCmer BVT : Unique match not matching at index '{0}'", i.ToString((IFormatProvider)null)));
                    return false;
                }
                i++;
            }
            return true;
        }

        /// <summary>
        /// Validates the Cluster Builder Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="propParam">Property parameters</param>
        /// <returns>True, if successfully validated</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        bool ValidateClusterBuilderMatches(IEnumerable<Match> matches,
            string nodeName, PropertyParameters propParam)
        {
            // Validates the Cluster builder MUMs
            string firstSeqOrderExpected = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ClustFirstSequenceMumOrderNode);
            string lengthExpected = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ClustLengthNode);
            string secondSeqOrderExpected = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ClustSecondSequenceMumOrderNode);

            StringBuilder firstSeqOrderActual = new StringBuilder();
            StringBuilder lengthActual = new StringBuilder();
            StringBuilder secondSeqOrderActual = new StringBuilder();

            ClusterBuilder cbObj = new ClusterBuilder();
            cbObj.MinimumScore = 0;
            switch (propParam)
            {
                case PropertyParameters.MinimumScore:
                    cbObj.MinimumScore = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.MinimumScoreNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.MaximumSeparation:
                    cbObj.MaximumSeparation = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.MaximumSeparationNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.FixedSeparation:
                    cbObj.FixedSeparation = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.FixedSeparationNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.SeparationFactor:
                    cbObj.SeparationFactor = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.SeparationFactorNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.FixedSeparationAndSeparationFactor:
                    cbObj.SeparationFactor = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.SeparationFactorNode), (IFormatProvider)null);
                    cbObj.FixedSeparation = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.FixedSeparationNode), (IFormatProvider)null);
                    break;
                case PropertyParameters.MaximumFixedAndSeparationFactor:
                    cbObj.MaximumSeparation = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.MaximumSeparationNode), (IFormatProvider)null);
                    cbObj.SeparationFactor = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.SeparationFactorNode), (IFormatProvider)null);
                    cbObj.FixedSeparation = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.FixedSeparationNode), (IFormatProvider)null);
                    break;
                default:
                    break;
            }

            List<MatchExtension> meObj = new List<MatchExtension>();

            foreach (Match m in matches)
            {
                meObj.Add(new MatchExtension(m));
            }

            IEnumerable<Cluster> clusts = cbObj.BuildClusters(meObj);

            foreach (Cluster clust in clusts)
            {
                foreach (MatchExtension maxMatchExtension in clust.Matches)
                {
                    firstSeqOrderActual.Append(maxMatchExtension.ReferenceSequenceOffset);
                    secondSeqOrderActual.Append(maxMatchExtension.QuerySequenceOffset);
                    lengthActual.Append(maxMatchExtension.Length);
                }
            }

            if ((0 != string.Compare(firstSeqOrderExpected.Replace(",", ""),
                firstSeqOrderActual.ToString(), true, CultureInfo.CurrentCulture))
                || (0 != string.Compare(lengthExpected.Replace(",", ""),
                lengthActual.ToString(), true, CultureInfo.CurrentCulture))
                || (0 != string.Compare(secondSeqOrderExpected.Replace(",", ""),
                secondSeqOrderActual.ToString(), true, CultureInfo.CurrentCulture)))
            {
                Console.WriteLine("NUCmer P1 : Cluster builder match not matching");
                ApplicationLog.WriteLine("NUCmer P1 : Cluster builder match not matching");
                return false;
            }

            return true;
        }

        #endregion Supported Methods
    }
}
