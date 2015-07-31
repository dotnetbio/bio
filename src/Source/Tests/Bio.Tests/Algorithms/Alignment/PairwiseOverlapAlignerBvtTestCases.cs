using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Tests.Framework;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    ///     Pairwise Overlap Aligner algorithm Bvt test cases
    /// </summary>
    [TestFixture]
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

        #region PairwiseOverlapAligner BVT Test cases

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignTwoSequencesFromTextFile()
        {
            this.ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(sequence1, sequence2)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignTwoSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : FastA File
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromTextFile()
        {
            this.ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(List)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromXm()
        {
            this.ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is in a text file using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : Text File i.e., Fasta
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignAllParamsFromTextFile()
        {
            this.ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        ///     Pass a Valid Sequence with valid GapPenalty, Similarity Matrix
        ///     which is passed in code using the method Align(all parameters)
        ///     and validate if the aligned sequence is as expected and
        ///     also validate the score for the same
        ///     Input : sequence in xml
        ///     Validation : Aligned sequence and score.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapSimpleAlignAllParamsFromXml()
        {
            this.ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AllParam);
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
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapAlignTwoSequencesFromTextFile()
        {
            this.ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignTwo,
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
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapAlignListSequencesFromTextFile()
        {
            this.ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignList,
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
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapAlignAllParamsFromTextFile()
        {
            this.ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AllParam,
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
            this.ValidatePairwiseOverlapAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
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

            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.FilePathNode1);
                string filePath2 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.FilePathNode2);

                //Parse the files and get the sequence.               
                var parser = new FastAParser { Alphabet = alphabet };
                aInput = parser.Parse(filePath1).ElementAt(0);
                bInput = parser.Parse(filePath2).ElementAt(0);
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.SequenceNode1);
                string origSequence2 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.SequenceNode2);
                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            var aInputString = aInput.ConvertToString();
            var bInputString = bInput.ConvertToString();

            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : First sequence used is '{0}'.", aInputString));
            ApplicationLog.WriteLine(string.Format(null, "PairwiseOverlapAligner BVT : Second sequence used is '{0}'.", bInputString));

            string blosumFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.BlosumFilePathNode);

            var sm = new SimilarityMatrix(new StreamReader(blosumFilePath));
            int gapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.GapOpenCostNode), null);
            int gapExtensionCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.GapExtensionCostNode), null);

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
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedScoreNode);
                    expectedSequence1 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedSequenceNode1);
                    expectedSequence2 = this.utilityObj.xmlUtil.GetTextValue(Constants.PairwiseOverlapAlignAlgorithmNodeName, Constants.ExpectedSequenceNode2);
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