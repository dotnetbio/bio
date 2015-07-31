using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

using Bio.Algorithms.Alignment;
using Bio.Algorithms.MUMmer;
using Bio.Algorithms.SuffixTree;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment.NUCmer
{
    /// <summary>
    ///     NUCmer P1 Test case implementation.
    /// </summary>
    [TestFixture]
    public class NUCmerP1TestCases
    {
        /// <summary>
        ///     Lis Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AdditionalParameters
        {
            FindUniqueMatches,
            PerformClusterBuilder,
            AlignSimilarityMatrix,
            Default
        };

        /// <summary>
        ///     Parameters which are used for different test cases
        ///     based on which the properties are updated.
        /// </summary>
        private enum PropertyParameters
        {
            MaximumSeparation,
            MinimumScore,
            SeparationFactor,
            FixedSeparation,
            FixedSeparationAndSeparationFactor,
            MaximumFixedAndSeparationFactor,
            Default
        };

        private readonly Utility utilityObj = new Utility(@"TestUtils\NUCmerTestsConfig.xml");

        #region Suffix Tree Test Cases

        /// <summary>
        ///     Validate FindMatches() method with one line sequences
        ///     and valid MUM length for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with Valid MUM length
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesOneLineSequenceValidMUMLength()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with DNA sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with Valid MUM length
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesDnaSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.DnaNucmerSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with RNA sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with Valid MUM length
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesRnaSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.RnaNucmerSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with medium sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : Medium size reference and query parameter
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesMediumSizeSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.MediumSizeSequenceNodeName,
                                                    true, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with continous repeating character sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with continous
        ///     repeating characters
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesContinousRepeatingSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineRepeatingCharactersNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with same sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with same characters
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesSameSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.SameSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with overlap sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with overlap
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesWithCrossOverlapSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with no match sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with no match
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesWithNoMatchSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineNoMatchSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with overlap sequences
        ///     for both reference and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with overlap
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesWithOverlapSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithoutCrossOverlapSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with ambiguity characters in
        ///     reference Dna sequence and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with ambiguity
        ///     characters in reference Dna sequence
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityDnaReferenceSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.DnaAmbiguityReferenceSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with ambiguity characters in
        ///     search Dna sequence and reference parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with ambiguity
        ///     characters in search Dna sequence
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityDnaSearchSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.DnaAmbiguitySearchSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with ambiguity characters in
        ///     reference Rna sequence and query parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with ambiguity
        ///     characters in reference Rna sequence
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityRnaReferenceSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.RnaAmbiguityReferenceSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate FindMatches() method with ambiguity characters in
        ///     search Rna sequence and reference parameter and validate
        ///     the unique matches
        ///     Input : One line sequence for both reference and query parameter with ambiguity
        ///     characters in search Rna sequence
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void SuffixTreeFindMatchesAmbiguityRnaSearchSequences()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.RnaAmbiguitySearchSequenceNodeName,
                                                    false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        ///     Validate BuildCluster() method with two unique match
        ///     and without cross over lap and validate the clusters
        ///     Input : Two unique matches without cross overlap
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderTwoUniqueMatchesWithoutCrossOverlap()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithoutCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        ///     Validate BuildCluster() method with two unique match
        ///     and with cross over lap and validate the clusters
        ///     Input : Two unique matches with cross overlap
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderTwoUniqueMatchesWithCrossOverlap()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        ///     Validate BuildCluster() method with two unique match
        ///     and with overlap and no cross overlap and validate the clusters
        ///     Input : Two unique matches with overlap and no cross overlap
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithOverlapNoCrossOverlap()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        ///     Validate BuildCluster() method with Minimum Score set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with minimum score 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithMinimumScoreZero()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.MinimumScore);
        }

        /// <summary>
        ///     Validate BuildCluster() method with MaximumSeparation set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with MaximumSeparation 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithMaximumSeparationZero()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.MaximumSeparation);
        }

        /// <summary>
        ///     Validate BuildCluster() method with SeperationFactor set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with SeperationFactor 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithSeperationFactoreZero()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.SeparationFactor);
        }

        /// <summary>
        ///     Validate BuildCluster() method with FixedSeparation set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with FixedSeparation 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithFixedSeparationZero()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.FixedSeparation);
        }

        /// <summary>
        ///     Validate BuildCluster() method with
        ///     MinimumScore set to greater than MUMlength
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with
        ///     MinimumScore set to greater than MUMlength
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithMinimumScoreGreater()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.MinimumScore);
        }

        /// <summary>
        ///     Validate BuildCluster() method with
        ///     FixedSeparation set to postive value and SeparationFactor=0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with
        ///     FixedSeparation set to postive value and SeparationFactor=0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithFixedSeparationAndSeparationFactor()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.FixedSeparationAndSeparationFactor);
        }

        /// <summary>
        ///     Validate BuildCluster() method with
        ///     MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with
        ///     MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ClusterBuilderWithMaximumFixedAndSeparationFactor()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                                                    false, AdditionalParameters.PerformClusterBuilder,
                                                    PropertyParameters.MaximumFixedAndSeparationFactor);
        }

        #endregion Suffix Tree Test Cases

        #region NUCmer Align Test Cases

        /// <summary>
        ///     Validate Align() method with one line Dna sequence
        ///     and validate the aligned sequences
        ///     Input : One line Dna sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignDnaSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.DnaNucmerSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with one line Rna sequence
        ///     and validate the aligned sequences
        ///     Input : One line Rna sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignRnaSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.RnaNucmerSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with one line list of sequence
        ///     and validate the aligned sequences
        ///     Input : One line list of sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOneLineListOfSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineOneReferenceQuerySequenceNodeName,
                                                false, true, false);
        }

        /// <summary>
        ///     Validate Align() method with small size list of sequence
        ///     and validate the aligned sequences
        ///     Input : small size list of sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSmallSizeListOfSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineOneReferenceQuerySequenceNodeName,
                                                false, true, false);
        }

        /// <summary>
        ///     Validate Align() method with one line Dna list of sequence
        ///     and validate the aligned sequences
        ///     Input : One line Dna list of sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOneLineDnaListOfSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, true, false);
        }

        /// <summary>
        ///     Validate Align() method with one line Rna list of sequence
        ///     and validate the aligned sequences
        ///     Input : One line Rna list of sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOneLineRnaListOfSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, false, true, false);
        }

        /// <summary>
        ///     Validate Align() method with medium size sequence
        ///     and validate the aligned sequences
        ///     Input : Medium size sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignMediumSizeSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.MediumSizeSequenceNodeName, true, false, false);
        }

        /// <summary>
        ///     Validate Align() method with One Line Repeating Characters sequence
        ///     and validate the aligned sequences
        ///     Input : One Line Repeating Characters sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignRepeatingCharactersSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineRepeatingCharactersNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with One Line Alternate Repeating Characters sequence
        ///     and validate the aligned sequences
        ///     Input : One Line Alternate Repeating Characters sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignAlternateRepeatingCharactersSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineAlternateRepeatingCharactersNodeName,
                                                false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with FastA file sequence
        ///     and validate the aligned sequences
        ///     Input : FastA file sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignFastASequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SimpleDnaFastaNodeName, true, false, false);
        }

        /// <summary>
        ///     Validate Align() method with One Line only Repeating Characters sequence
        ///     and validate the aligned sequences
        ///     Input : One Line only Repeating Characters sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOnlyRepeatingCharactersSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineOnlyRepeatingCharactersNodeName,
                                                false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with one reference multi search sequence
        ///     and validate the aligned sequences
        ///     Input : one reference multi search sequence file
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOneRefMultiSearchSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SmallSizeSequenceNodeName, true, false, false);
        }

        /// <summary>
        ///     Validate Align() method with valid MUM length
        ///     and validate the aligned sequences
        ///     Input : One line sequence with valid MUM length
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOneLineSequenceValidMumLength()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with One Line same sequences
        ///     and validate the aligned sequences
        ///     Input : One Line same sequences
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSameSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SameSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with overlap sequences
        ///     and validate the aligned sequences
        ///     Input : One Line overlap sequences
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOverlapMatchSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneOverlapMatchSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with no match sequences
        ///     and validate the aligned sequences
        ///     Input : One Line no match sequences
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignNoMatchSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineNoMatchSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with cross overlap sequences
        ///     and validate the aligned sequences
        ///     Input : One Line cross overlap sequences
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignCrossOverlapMatchSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                                                false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with one line reference Dna sequence with ambiguity
        ///     and validate the aligned sequences
        ///     Input : One line Dna sequence with ambiguity
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignDnaReferenceAmbiguitySequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.DnaAmbiguityReferenceSequenceNodeName,
                                                false, false, true);
        }

        /// <summary>
        ///     Validate Align() method with one line reference Rna sequence with ambiguity
        ///     and validate the aligned sequences
        ///     Input : One line Rna sequence with ambiguity
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignRnaReferenceAmbiguitySequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.RnaAmbiguityReferenceSequenceNodeName,
                                                false, false, true);
        }

        /// <summary>
        ///     Validate Align() method with one line search Dna sequence with ambiguity
        ///     and validate the aligned sequences
        ///     Input : One line Dna sequence with ambiguity
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignDnaSearchAmbiguitySequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.DnaAmbiguitySearchSequenceNodeName, false, false, true);
        }

        /// <summary>
        ///     Validate Align() method with one line search Rna sequence with ambiguity
        ///     and validate the aligned sequences
        ///     Input : One line Rna sequence with ambiguity
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignRnaSearchAmbiguitySequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.RnaAmbiguitySearchSequenceNodeName, false, false, true);
        }

        /// <summary>
        ///     Validate Align() method with one ref. and one query sequence
        ///     and validate the aligned sequences
        ///     Input : one reference and one query sequence
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignOneRefOneQuerySequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with multi reference one search sequence
        ///     and validate the aligned sequences
        ///     Input : multiple reference one search sequence file
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignMultiRefOneSearchSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.MultiRefSingleQueryMatchSequenceNodeName,
                                                false, false, false);
        }

        /// <summary>
        ///     Validate Align() method with multi reference multi search sequence
        ///     and validate the aligned sequences
        ///     Input : multiple reference multi search sequence file
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignMultiRefMultiSearchSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName, false, false, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with Minimum Score set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with minimum score 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithMinimumScoreZero()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.MinimumScore, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with MaximumSeparation set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with MaximumSeparation 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithMaximumSeparationZero()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.MaximumSeparation, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with SeperationFactor set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with SeperationFactor 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithSeperationFactoreZero()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.SeparationFactor, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with FixedSeparation set to 0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with FixedSeparation 0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithFixedSeparationZero()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.FixedSeparation, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with
        ///     MinimumScore set to greater than MUMlength
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with
        ///     MinimumScore set to greater than MUMlength
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithMinimumScoreGreater()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.MinimumScore, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with
        ///     FixedSeparation set to postive value and SeparationFactor=0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with
        ///     FixedSeparation set to postive value and SeparationFactor=0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithFixedSeparationAndSeparationFactor()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.FixedSeparationAndSeparationFactor, false);
        }

        /// <summary>
        ///     Validate BuildCluster() method with
        ///     MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0
        ///     and validate the clusters
        ///     Input : Reference and Search Sequences with
        ///     MaximumSeparation=6, FixedSeparation=7 and SeparationFactor=0
        ///     Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignWithMaximumFixedAndSeparationFactor()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.MinimumScoreGreaterSequenceNodeName,
                                                false, false, AdditionalParameters.Default,
                                                PropertyParameters.MaximumFixedAndSeparationFactor, false);
        }

        /// <summary>
        ///     Validate Align() method with IsAlign set to true
        ///     and with gaps in the reference and query sequence.
        /// </summary>
        [Category("Priority1")]
        public void NUCmerAlignWithIsAlignAndGaps()
        {
            IList<ISequence> seqList = new List<ISequence>();
            seqList.Add(new Sequence(Alphabets.DNA, Encoding.ASCII.GetBytes("CAAAAGGGATTGC---TGTTGGAGTGAATGCCATTACCTACCGGCTAGGAGGAGTAGTACAAAGGAGC")));
            seqList.Add(new Sequence(Alphabets.DNA, Encoding.ASCII.GetBytes("CAAAAGGGATTGC---")));
            seqList.Add(new Sequence(Alphabets.DNA, Encoding.ASCII.GetBytes("TAGTAGTTCTGCTATATACATTTG")));
            seqList.Add(new Sequence(Alphabets.DNA, Encoding.ASCII.GetBytes("GTTATCATGCGAACAATTCAACAGACACTGTAGA")));
            var num = new NucmerPairwiseAligner
                          {
                              BreakLength = 8,
                              FixedSeparation = 0,
                              MinimumScore = 0,
                              MaximumSeparation = 0,
                              SeparationFactor = 0,
                              LengthOfMUM = 8
                          };
            IList<ISequence> sequenceList = seqList;
            IList<ISequenceAlignment> alignmentObj = num.Align(sequenceList);

            var alignedSeqs = (AlignedSequence) alignmentObj[0].AlignedSequences[0];
            Assert.AreEqual("CAAAAGGGATTGC---", new string(alignedSeqs.Sequences[0].Select(a => (char) a).ToArray()));
            Assert.AreEqual("CAAAAGGGATTGC---", new string(alignedSeqs.Sequences[1].Select(a => (char) a).ToArray()));

            ApplicationLog.WriteLine("Successfully validated Align method with IsAlign and Gaps");
        }

        #endregion NUCmer Align Test Cases

        #region NUCmer Align Simple Test Cases

        /// <summary>
        ///     Validate AlignSimple() method with one line Dna list of sequence
        ///     and validate the aligned sequences
        ///     Input : Dna list of sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleDnaListOfSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Rna list of sequence
        ///     and validate the aligned sequences
        ///     Input : Rna list of sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleRnaListOfSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Dna sequence
        ///     and validate the aligned sequences
        ///     Input : Single Reference and Single Query Dna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleSingleRefSingleQueryDnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Rna sequence
        ///     and validate the aligned sequences
        ///     Input : Single Reference and Single Query Rna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleSingleRefSingleQueryRnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Dna sequence
        ///     and validate the aligned sequences
        ///     Input : Single Reference and Multi Query Dna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleSingleRefMultiQueryDnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Rna sequence
        ///     and validate the aligned sequences
        ///     Input : Single Reference and Multi Query Rna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleSingleRefMultiQueryRnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Dna sequence
        ///     and validate the aligned sequences
        ///     Input : Multi Reference and Multi Query Dna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleMultiRefMultiQueryDnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.MultiRefMultiQueryDnaMatchSequence, false, true);
        }

        /// <summary>
        ///     Validate AlignSimple() method with one line Rna sequence
        ///     and validate the aligned sequences
        ///     Input : Multi Reference and Multi Query Rna sequence
        ///     Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void NUCmerAlignSimpleMultiRefMultiQueryRnaSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.MultiRefMultiQueryRnaMatchSequence, false, true);
        }

        #endregion NUCmer Align Simple Test Cases

        #region Supported Methods

        /// <summary>
        ///     Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="additionalParam">LIS action type enum</param>
        private void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath,
                                                             AdditionalParameters additionalParam)
        {
            this.ValidateFindMatchSuffixGeneralTestCases(nodeName, isFilePath, additionalParam,
                                                    PropertyParameters.Default);
        }

        /// <summary>
        ///     Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="additionalParam">LIS action type enum</param>
        /// <param name="propParam">Property parameters</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath,
                                                             AdditionalParameters additionalParam,
                                                             PropertyParameters propParam)
        {
            ISequence referenceSeq;
            var searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format(null, "NUCmer P1 : Successfully validated the File Path '{0}'.", filePath));

                var parser = new FastAParser();
                IEnumerable<ISequence> referenceSeqList = parser.Parse(filePath);

                var byteList = new List<Byte>();
                foreach (ISequence seq in referenceSeqList)
                {
                    byteList.AddRange(seq);
                    byteList.Add((byte) '+');
                }

                referenceSeq = new Sequence(referenceSeqList.First().Alphabet.GetMummerAlphabet(),
                                            byteList.ToArray());

                // Gets the query sequence from the FastA file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format(null, "NUCmer P1 : Successfully validated the File Path '{0}'.", queryFilePath));

                var queryParserObj = new FastAParser();
                IEnumerable<ISequence> querySeqList = queryParserObj.Parse(queryFilePath);
                searchSeqList.AddRange(querySeqList);
            }
            else
            {
                // Gets the reference & search sequences from the configuration file
                string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
                string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);
                IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

                var refSeqList = referenceSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))).Cast<ISequence>().ToList();

                var byteList = new List<Byte>();
                foreach (ISequence seq in refSeqList)
                {
                    byteList.AddRange(seq);
                    byteList.Add((byte) '+');
                }

                referenceSeq = new Sequence(refSeqList.First().Alphabet.GetMummerAlphabet(), byteList.ToArray());
                searchSeqList.AddRange(searchSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))));
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Builds the suffix for the reference sequence passed.           
            var suffixTreeBuilder = new MultiWaySuffixTree(referenceSeq as Sequence)
                                    {
                                        MinLengthOfMatch =
                                            long.Parse(mumLength, null)
                                    };
            var matches = searchSeqList.ToDictionary(t => t, suffixTreeBuilder.SearchMatchesUniqueInReference);

            var mums = new List<Match>();
            foreach (var a in matches.Values)
            {
                mums.AddRange(a);
            }

            switch (additionalParam)
            {
                case AdditionalParameters.FindUniqueMatches:
                    // Validates the Unique Matches.
                    ApplicationLog.WriteLine("NUCmer P1 : Validating the Unique Matches");
                    Assert.IsTrue(this.ValidateUniqueMatches(mums, nodeName, isFilePath));
                    ApplicationLog.WriteLine("NUCmer P1 : Successfully validated the all the unique matches for the sequences.");
                    break;
                case AdditionalParameters.PerformClusterBuilder:
                    // Validates the Unique Matches.
                    ApplicationLog.WriteLine(
                        "NUCmer P1 : Validating the Unique Matches using Cluster Builder");
                    Assert.IsTrue(this.ValidateClusterBuilderMatches(mums, nodeName, propParam));
                    ApplicationLog.WriteLine("NUCmer P1 : Successfully validated the all the cluster builder matches for the sequences.");
                    break;
                default:
                    break;
            }


            ApplicationLog.WriteLine("NUCmer P1 : Successfully validated the all the unique matches for the sequences.");
        }

        /// <summary>
        ///     Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// <param name="isAmbiguous"></param>
        private void ValidateNUCmerAlignGeneralTestCases(string nodeName, bool isFilePath, bool isAlignList, bool isAmbiguous)
        {
            this.ValidateNUCmerAlignGeneralTestCases(nodeName, isFilePath, isAlignList, 
                AdditionalParameters.Default, PropertyParameters.Default, isAmbiguous);
        }

        /// <summary>
        ///     Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// <param name="addParam">Additional parameters</param>
        /// <param name="propParam">Property parameters</param>
        /// <param name="isAmbiguous"></param>
        /// 1801 is suppressed as this variable would be used later
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"),
         SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),
         SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"),
         SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parserParam")]
        private void ValidateNUCmerAlignGeneralTestCases(string nodeName, bool isFilePath, bool isAlignList, AdditionalParameters addParam,
                                                         PropertyParameters propParam, bool isAmbiguous)
        {
            IList<ISequence> refSeqList = new List<ISequence>();
            IList<ISequence> searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format(null, "NUCmer P1 : Successfully validated the File Path '{0}'.", filePath));

                var fastaparserobj = new FastAParser();
                IEnumerable<ISequence> referenceSeqList = fastaparserobj.Parse(filePath);

                foreach (ISequence seq in referenceSeqList)
                {
                    refSeqList.Add(seq);
                }

                // Gets the query sequence from the FastA file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format(null, "NUCmer P1 : Successfully validated the File Path '{0}'.", queryFilePath));

                var queryParserobj = new FastAParser();
                IEnumerable<ISequence> serSeqList = queryParserobj.Parse(queryFilePath);

                foreach (ISequence seq in serSeqList)
                {
                    searchSeqList.Add(seq);
                }
            }
            else
            {
                // Gets the reference & search sequences from the configuration file
                string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
                string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));
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
                                                      Encoding.ASCII.GetBytes(referenceSequences[i]));
                    referSeq.ID = "ref " + i;
                    refSeqList.Add(referSeq);
                }

                for (int i = 0; i < searchSequences.Length; i++)
                {
                    ISequence searchSeq = new Sequence(ambAlphabet,
                                                       Encoding.ASCII.GetBytes(searchSequences[i]));
                    searchSeq.ID = "qry " + i;
                    searchSeqList.Add(searchSeq);
                }
            }
            // Gets the mum length from the xml
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            var nucmerObj = new NucmerPairwiseAligner();
            // Check for additional parameters and update the object accordingly
            switch (addParam)
            {
                case AdditionalParameters.AlignSimilarityMatrix:
                    nucmerObj.SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
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
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MinimumScoreNode), null);
                    break;
                case PropertyParameters.MaximumSeparation:
                    nucmerObj.MaximumSeparation = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode), null);
                    break;
                case PropertyParameters.FixedSeparation:
                    nucmerObj.FixedSeparation = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), null);
                    break;
                case PropertyParameters.SeparationFactor:
                    nucmerObj.SeparationFactor = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), null);
                    break;
                case PropertyParameters.FixedSeparationAndSeparationFactor:
                    nucmerObj.SeparationFactor = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), null);
                    nucmerObj.FixedSeparation = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), null);
                    break;
                case PropertyParameters.MaximumFixedAndSeparationFactor:
                    nucmerObj.MaximumSeparation = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode), null);
                    nucmerObj.SeparationFactor = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), null);
                    nucmerObj.FixedSeparation = int.Parse(
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), null);
                    break;
                default:
                    break;
            }

            IList<ISequenceAlignment> align = null;

            if (isAlignList)
            {
                var listOfSeq = new List<ISequence> {refSeqList.ElementAt(0), searchSeqList.ElementAt(0)};
                align = nucmerObj.Align(listOfSeq);
            }
            else
            {
                align = nucmerObj.Align(refSeqList, searchSeqList);
            }

            string expectedSequences = isFilePath
                    ? this.utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.ExpectedSequencesNode)
                    : this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequencesNode);
            string[] expSeqArray = expectedSequences.Split(',');

            // Gets all the aligned sequences in comma separated format
            foreach (IPairwiseSequenceAlignment seqAlignment in align)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    var actualStr = alignedSeq.FirstSequence.ConvertToString();
                    Assert.IsTrue(expSeqArray.Contains(actualStr));

                    actualStr = alignedSeq.SecondSequence.ConvertToString();
                    Assert.IsTrue(expSeqArray.Contains(actualStr));
                }
            }

            ApplicationLog.WriteLine("NUCmer P1 : Successfully validated all the aligned sequences.");
        }

        /// <summary>
        ///     Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// 1801 is suppressed as this variable would be used later
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"),
         SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parserParam")]
        private void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName, bool isFilePath, bool isAlignList)
        {
            IList<ISequence> refSeqList = new List<ISequence>();
            IList<ISequence> searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format(null, "NUCmer P1 : Successfully validated the File Path '{0}'.", filePath));

                var fastaparserobj = new FastAParser();
                IEnumerable<ISequence> referenceSeqList = fastaparserobj.Parse(filePath);

                foreach (ISequence seq in referenceSeqList)
                {
                    refSeqList.Add(seq);
                }

                // Gets the query sequence from the FastA file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format(null,"NUCmer P1 : Successfully validated the File Path '{0}'.", queryFilePath));

                var fastaParserobj = new FastAParser();
                IEnumerable<ISequence> querySeqList = fastaParserobj.Parse(queryFilePath);
                foreach (ISequence seq in querySeqList)
                {
                    searchSeqList.Add(seq);
                }
            }
            else
            {
                // Gets the reference & search sequences from the configuration file
                string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
                string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

                foreach (Sequence referSeq in referenceSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))))
                {
                    refSeqList.Add(referSeq);
                }

                foreach (Sequence searchSeq in searchSequences.Select(t => new Sequence(seqAlphabet, Encoding.ASCII.GetBytes(t))))
                {
                    searchSeqList.Add(searchSeq);
                }
            }

            // Gets the mum length from the xml
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            var nucmerObj = new NucmerPairwiseAligner
            {
                MaximumSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                MinimumScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null), 
                SeparationFactor = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                BreakLength = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null),
                LengthOfMUM = long.Parse(mumLength, null)
            };

            IList<ISequenceAlignment> alignSimple = null;
            if (isAlignList)
            {
                var listOfSeq = new List<ISequence> {refSeqList[0], searchSeqList[0]};
                alignSimple = nucmerObj.AlignSimple(listOfSeq);
            }

            string expectedSequences = isFilePath
                ? this.utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.ExpectedSequencesNode)
                : this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma separated format
            foreach (PairwiseAlignedSequence alignedSeq in alignSimple.Cast<IPairwiseSequenceAlignment>().SelectMany(seqAlignment => seqAlignment))
            {
                Assert.AreEqual(expSeqArray[j], alignedSeq.FirstSequence.ConvertToString());
                ++j;
                Assert.AreEqual(expSeqArray[j], alignedSeq.SecondSequence.ConvertToString());
                j++;
            }

            ApplicationLog.WriteLine("NUCmer P1 : Successfully validated all the aligned sequences.");
        }

        /// <summary>
        ///     Validates the Unique Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Nodes to be read from Text file?</param>
        /// <returns>True, if successfully validated</returns>
        private bool ValidateUniqueMatches(IEnumerable<Match> matches, string nodeName, bool isFilePath)
        {
            string[] firstSeqOrder;
            string[] length;
            string[] secondSeqOrder;

            if (isFilePath)
            {
                firstSeqOrder = this.utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.FirstSequenceMumOrderNode).Split(',');
                length = this.utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.LengthNode).Split(',');
                secondSeqOrder = this.utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.SecondSequenceMumOrderNode).Split(',');
            }
            else
            {
                firstSeqOrder = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FirstSequenceMumOrderNode).Split(',');
                length = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LengthNode).Split(',');
                secondSeqOrder = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SecondSequenceMumOrderNode).Split(',');
            }

            int i = 0;

            IList<MatchExtension> meNewObj = matches.Select(m => new MatchExtension(m)).ToList();

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
                if ((0 != string.Compare(firstSeqOrder[i], match.ReferenceSequenceMumOrder.ToString((IFormatProvider) null), true, CultureInfo.CurrentCulture))
                 || (0 != string.Compare(length[i], match.Length.ToString((IFormatProvider) null), true, CultureInfo.CurrentCulture))
                 || (0 != string.Compare(secondSeqOrder[i], match.QuerySequenceMumOrder.ToString((IFormatProvider) null), true, CultureInfo.CurrentCulture)))
                {
                    ApplicationLog.WriteLine(string.Format(null, "NUCmer BVT : Unique match not matching at index '{0}'", i));
                    return false;
                }
                i++;
            }
            return true;
        }

        /// <summary>
        ///     Validates the Cluster Builder Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="propParam">Property parameters</param>
        /// <returns>True, if successfully validated</returns>
        private bool ValidateClusterBuilderMatches(IEnumerable<Match> matches, string nodeName, PropertyParameters propParam)
        {
            // Validates the Cluster builder MUMs
            string firstSeqOrderExpected = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustFirstSequenceMumOrderNode);
            string lengthExpected = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustLengthNode);
            string secondSeqOrderExpected = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustSecondSequenceMumOrderNode);

            var firstSeqOrderActual = new StringBuilder();
            var lengthActual = new StringBuilder();
            var secondSeqOrderActual = new StringBuilder();

            var cbObj = new ClusterBuilder {MinimumScore = 0};
            switch (propParam)
            {
                case PropertyParameters.MinimumScore:
                    cbObj.MinimumScore = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MinimumScoreNode), null);
                    break;
                case PropertyParameters.MaximumSeparation:
                    cbObj.MaximumSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode), null);
                    break;
                case PropertyParameters.FixedSeparation:
                    cbObj.FixedSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), null);
                    break;
                case PropertyParameters.SeparationFactor:
                    cbObj.SeparationFactor = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), null);
                    break;
                case PropertyParameters.FixedSeparationAndSeparationFactor:
                    cbObj.SeparationFactor = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), null);
                    cbObj.FixedSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), null);
                    break;
                case PropertyParameters.MaximumFixedAndSeparationFactor:
                    cbObj.MaximumSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode), null);
                    cbObj.SeparationFactor = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode), null);
                    cbObj.FixedSeparation = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode), null);
                    break;
                default:
                    break;
            }

            var meObj = matches.Select(m => new MatchExtension(m)).ToList();

            IEnumerable<Cluster> clusts = cbObj.BuildClusters(meObj);

            foreach (MatchExtension maxMatchExtension in clusts.SelectMany(clust => clust.Matches))
            {
                firstSeqOrderActual.Append(maxMatchExtension.ReferenceSequenceOffset);
                secondSeqOrderActual.Append(maxMatchExtension.QuerySequenceOffset);
                lengthActual.Append(maxMatchExtension.Length);
            }

            if ((0 != string.Compare(firstSeqOrderExpected.Replace(",", ""), firstSeqOrderActual.ToString(), true, CultureInfo.CurrentCulture))
             || (0 != string.Compare(lengthExpected.Replace(",", ""), lengthActual.ToString(), true, CultureInfo.CurrentCulture))
             || (0 != string.Compare(secondSeqOrderExpected.Replace(",", ""), secondSeqOrderActual.ToString(), true, CultureInfo.CurrentCulture)))
            {
                ApplicationLog.WriteLine("NUCmer P1 : Cluster builder match not matching");
                return false;
            }

            return true;
        }

        #endregion Supported Methods
    }
}