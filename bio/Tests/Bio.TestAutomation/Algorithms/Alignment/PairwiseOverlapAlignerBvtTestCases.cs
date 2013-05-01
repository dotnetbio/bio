/****************************************************************************
 * PairwiseOverlapAlignerBvtTestCases.cs
 * 
 *   This file contains the PairwiseOverlapAligner Bvt Test Cases.
 * 
***************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Tests.Framework;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    ///     Pairwise Overlap Aligner algorithm Bvt test cases
    /// </summary>
    [TestClass]
    public class PairwiseOverlapAlignerBvtTestCases
    {
        #region Enums

        /// <summary>
        ///     Alignment Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignmentParamType
        {
            AlignTwo,
            AlignList,
            AllParam
        };

        /// <summary>
        ///     Alignment Type Parameters which are used for different test cases
        ///     based on which the test cases are executed.
        /// </summary>
        private enum AlignmentType
        {
            SimpleAlign,
            Align
        };

        #endregion Enums

        #region Global Variables

        private readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static PairwiseOverlapAlignerBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region PairwiseOverlapAligner BVT Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignTwoSequencesFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignTwoSequencesFromXml()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromXml()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromXm()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignAllParamsFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignAllParamsFromXml()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AllParam);
        }

        #region Gap Extension Cost inclusion Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapAlignTwoSequencesFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignTwo,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapAlignListSequencesFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignList,
                                             AlignmentType.Align);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapAlignAllParamsFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AllParam,
                                             AlignmentType.Align);
        }

        #endregion Gap Extension Cost inclusion Test cases

        #endregion PairwiseOverlapAligner BVT Test cases

        #region Supporting Methods

        /// <summary>
        ///     Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        private void ValidatePairwiseOverlapAlignment(bool isTextFile, AlignmentParamType alignParam)
        {
            ValidatePairwiseOverlapAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        ///     Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        private void ValidatePairwiseOverlapAlignment(bool isTextFile, AlignmentParamType alignParam, AlignmentType alignType)
        {
            ISequence aInput;
            ISequence bInput;

            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.FilePathNode1);
                string filePath2 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.FilePathNode2);

                //Parse the files and get the sequence.               
                using (var parser1 = new FastAParser(filePath1))
                {
                    parser1.Alphabet = alphabet;
                    aInput = parser1.Parse().ElementAt(0);
                }

                using (var parser2 = new FastAParser(filePath2))
                {
                    parser2.Alphabet = alphabet;
                    bInput = parser2.Parse().ElementAt(0);
                }
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.SequenceNode1);
                string origSequence2 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.SequenceNode2);
                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            var aInputString = aInput.ConvertToString();
            var bInputString = bInput.ConvertToString();

            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : First sequence used is '{0}'.", aInputString));
            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : Second sequence used is '{0}'.", bInputString));

            string blosumFilePath = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.BlosumFilePathNode);

            var sm = new SimilarityMatrix(blosumFilePath);
            int gapOpenCost = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.GapOpenCostNode), null);
            int gapExtensionCost = int.Parse(utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.GapExtensionCostNode), null);

            var pairwiseOverlapObj = new PairwiseOverlapAligner();
            if (AlignmentParamType.AllParam != alignParam)
            {
                pairwiseOverlapObj.SimilarityMatrix = sm;
                pairwiseOverlapObj.GapOpenCost = gapOpenCost;
            }

            IList<IPairwiseSequenceAlignment> result = null;

            switch (alignParam)
            {
                case AlignmentParamType.AlignList:
                    var sequences = new List<ISequence> {aInput, bInput};
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = pairwiseOverlapObj.Align(sequences);
                            break;
                        default:
                            result = pairwiseOverlapObj.AlignSimple(sequences);
                            break;
                    }
                    break;
                case AlignmentParamType.AlignTwo:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = pairwiseOverlapObj.Align(aInput, bInput);
                            break;
                        default:
                            result = pairwiseOverlapObj.AlignSimple(aInput, bInput);
                            break;
                    }
                    break;
                case AlignmentParamType.AllParam:
                    switch (alignType)
                    {
                        case AlignmentType.Align:
                            result = pairwiseOverlapObj.Align(sm, gapOpenCost,
                                                              gapExtensionCost, aInput, bInput);
                            break;
                        default:
                            result = pairwiseOverlapObj.AlignSimple(sm, gapOpenCost, aInput, bInput);
                            break;
                    }
                    break;
                default:
                    break;
            }

            // Read the xml file for getting both the files for aligning.
            string expectedSequence1;
            string expectedSequence2;
            string expectedScore;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedSequenceNode1);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedSequenceNode2);
                    break;
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            var seperators = new[] {';'};
            string[] expectedSequences1 = expectedSequence1.Split(seperators);
            string[] expectedSequences2 = expectedSequence2.Split(seperators);

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            for (int i = 0; i < expectedSequences1.Length; i++)
            {
                PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence
                {
                    FirstSequence = new Sequence(alphabet, expectedSequences1[i]),
                    SecondSequence = new Sequence(alphabet, expectedSequences2[i]),
                    Score = Convert.ToInt32(expectedScore, null)
                };
                align.PairwiseAlignedSequences.Add(alignedSeq);
            }

            expectedOutput.Add(align);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(result, expectedOutput));

            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : Aligned First Sequence is '{0}'.", expectedSequence1));
            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : Aligned Second Sequence is '{0}'.", expectedSequence2));
        }

        #endregion Supporting Methods
    }
}