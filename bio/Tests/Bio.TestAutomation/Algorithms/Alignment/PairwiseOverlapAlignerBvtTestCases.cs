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
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;

namespace Bio.TestAutomation.Algorithms.Alignment
{

    /// <summary>
    /// Pairwise Overlap Aligner algorithm Bvt test cases
    /// </summary>
    [TestClass]
    public class PairwiseOverlapAlignerBvtTestCases
    {
        #region Enums

        /// <summary>
        /// Alignment Type Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AlignmentType
        {
            SimpleAlign,
            Align
        };

        /// <summary>
        /// Alignment Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AlignmentParamType
        {
            AlignTwo,
            AlignList,
            AllParam
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
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
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignTwoSequencesFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignTwoSequencesFromXml()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignTwo);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AlignList);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromXml()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignListSequencesFromXm()
        {
            ValidatePairwiseOverlapAlignment(false, AlignmentParamType.AlignList);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is in a text file using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : Text File i.e., Fasta
        /// Validation : Aligned sequence and score.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PairwiseOverlapSimpleAlignAllParamsFromTextFile()
        {
            ValidatePairwiseOverlapAlignment(true, AlignmentParamType.AllParam);
        }

        /// <summary>
        /// Pass a Valid Sequence with valid GapPenalty, Similarity Matrix 
        /// which is passed in code using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : sequence in xml
        /// Validation : Aligned sequence and score.
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
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(sequence1, sequence2) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
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
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(List) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : FastA File
        /// Validation : Aligned sequence and score.
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
        /// Pass a Valid Sequence with valid Gap Open Cost, Gap Extension Cost, Similarity Matrix 
        /// which is in a text file using the method Align(all parameters) 
        /// and validate if the aligned sequence is as expected and 
        /// also validate the score for the same
        /// Input : Text File i.e., Fasta
        /// Validation : Aligned sequence and score.
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
        /// Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        void ValidatePairwiseOverlapAlignment(bool isTextFile, AlignmentParamType alignParam)
        {
            ValidatePairwiseOverlapAlignment(isTextFile, alignParam, AlignmentType.SimpleAlign);
        }

        /// <summary>
        /// Validates PairwiseOverlapAlignment algorithm for the parameters passed.
        /// </summary>
        /// <param name="isTextFile">Is text file an input.</param>
        /// <param name="alignParam">parameter based on which certain validations are done.</param>
        /// <param name="alignType">Is the Align type Simple or Align with Gap Extension cost?</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidatePairwiseOverlapAlignment(bool isTextFile, AlignmentParamType alignParam,
            AlignmentType alignType)
        {
            ISequence aInput = null;
            ISequence bInput = null;

            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(
                Constants.PairwiseOverlapAlignAlgorithmNodeName,
                Constants.AlphabetNameNode));

            if (isTextFile)
            {
                // Read the xml file for getting both the files for aligning.
                string filePath1 = utilityObj.xmlUtil.GetTextValue(
                    Constants.PairwiseOverlapAlignAlgorithmNodeName,
                    Constants.FilePathNode1);
                string filePath2 = utilityObj.xmlUtil.GetTextValue(
                    Constants.PairwiseOverlapAlignAlgorithmNodeName,
                    Constants.FilePathNode2);

                //Parse the files and get the sequence.               

                using (FastAParser parser1 = new FastAParser(filePath1))
                {
                    parser1.Alphabet = alphabet;
                    aInput = parser1.Parse().ElementAt(0);
                }

                using (FastAParser parser2 = new FastAParser(filePath2))
                {
                    parser2.Alphabet = alphabet;
                    bInput = parser2.Parse().ElementAt(0);
                }
            }
            else
            {
                // Read the xml file for getting both the files for aligning.
                string origSequence1 = utilityObj.xmlUtil.GetTextValue(
                    Constants.PairwiseOverlapAlignAlgorithmNodeName,
                    Constants.SequenceNode1);
                string origSequence2 = utilityObj.xmlUtil.GetTextValue(
                    Constants.PairwiseOverlapAlignAlgorithmNodeName,
                    Constants.SequenceNode2);
                aInput = new Sequence(alphabet, origSequence1);
                bInput = new Sequence(alphabet, origSequence2);
            }

            string aInputString = new string(aInput.Select(a => (char)a).ToArray());
            string bInputString = new string(bInput.Select(a => (char)a).ToArray());


            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : First sequence used is '{0}'.",
                aInputString));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Second sequence used is '{0}'.",
                bInputString));

            Console.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : First sequence used is '{0}'.",
                aInputString));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Second sequence used is '{0}'.",
                bInputString));

            string blosumFilePath = utilityObj.xmlUtil.GetTextValue(
                Constants.PairwiseOverlapAlignAlgorithmNodeName,
                Constants.BlosumFilePathNode);

            SimilarityMatrix sm = new SimilarityMatrix(blosumFilePath);
            int gapOpenCost = int.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.PairwiseOverlapAlignAlgorithmNodeName,
                Constants.GapOpenCostNode), (IFormatProvider)null);

            int gapExtensionCost = int.Parse(utilityObj.xmlUtil.GetTextValue(
                Constants.PairwiseOverlapAlignAlgorithmNodeName,
                Constants.GapExtensionCostNode), (IFormatProvider)null);

            PairwiseOverlapAligner pairwiseOverlapObj = new PairwiseOverlapAligner();
            if (AlignmentParamType.AllParam != alignParam)
            {
                pairwiseOverlapObj.SimilarityMatrix = sm;
                pairwiseOverlapObj.GapOpenCost = gapOpenCost;
            }

            IList<IPairwiseSequenceAlignment> result = null;

            switch (alignParam)
            {
                case AlignmentParamType.AlignList:
                    List<ISequence> sequences = new List<ISequence>();
                    sequences.Add(aInput);
                    sequences.Add(bInput);
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

            pairwiseOverlapObj = null;
            aInput = null;
            bInput = null;
            sm = null;

            // Read the xml file for getting both the files for aligning.
            string expectedSequence1 = string.Empty;
            string expectedSequence2 = string.Empty;
            string expectedScore = string.Empty;
            aInput = null;
            bInput = null;
            sm = null;

            switch (alignType)
            {
                case AlignmentType.Align:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(
                        Constants.PairwiseOverlapAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                        Constants.PairwiseOverlapAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence1Node);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                        Constants.PairwiseOverlapAlignAlgorithmNodeName,
                        Constants.ExpectedGapExtensionSequence2Node);
                    break;
                default:
                    expectedScore = utilityObj.xmlUtil.GetTextValue(
                        Constants.PairwiseOverlapAlignAlgorithmNodeName,
                        Constants.ExpectedScoreNode);
                    expectedSequence1 = utilityObj.xmlUtil.GetTextValue(
                        Constants.PairwiseOverlapAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode1);
                    expectedSequence2 = utilityObj.xmlUtil.GetTextValue(
                        Constants.PairwiseOverlapAlignAlgorithmNodeName,
                        Constants.ExpectedSequenceNode2);
                    break;
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            string[] expectedSequences1, expectedSequences2;
            char[] seperators = new char[1] { ';' };
            expectedSequences1 = expectedSequence1.Split(seperators);
            expectedSequences2 = expectedSequence2.Split(seperators);

            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq;
            for (int i = 0; i < expectedSequences1.Length; i++)
            {
                alignedSeq = new PairwiseAlignedSequence();
                alignedSeq.FirstSequence = new Sequence(alphabet, expectedSequences1[i]);
                alignedSeq.SecondSequence = new Sequence(alphabet, expectedSequences2[i]);
                alignedSeq.Score = Convert.ToInt32(expectedScore, (IFormatProvider)null);
                align.PairwiseAlignedSequences.Add(alignedSeq);
            }

            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Final Score '{0}'.", expectedScore));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Aligned First Sequence is '{0}'.",
                expectedSequence1));
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Aligned Second Sequence is '{0}'.",
                expectedSequence2));

            Console.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Final Score '{0}'.", expectedScore));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Aligned First Sequence is '{0}'.",
                expectedSequence1));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "PairwiseOverlapAligner BVT : Aligned Second Sequence is '{0}'.",
                expectedSequence2));
        }


        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="result">output of Aligners</param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        private static bool CompareAlignment(IList<IPairwiseSequenceAlignment> actualAlignment,
             IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            bool output = true;

            if (actualAlignment.Count == expectedAlignment.Count)
            {
                for (int resultCount = 0; resultCount < actualAlignment.Count; resultCount++)
                {
                    if (actualAlignment[resultCount].PairwiseAlignedSequences.Count == expectedAlignment[resultCount].PairwiseAlignedSequences.Count)
                    {
                        for (int alignSeqCount = 0; alignSeqCount < actualAlignment[resultCount].PairwiseAlignedSequences.Count; alignSeqCount++)
                        {
                            // Validates the First Sequence, Second Sequence and Score                            
                            if (new string(actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].FirstSequence.Select(a => (char)a).ToArray()).Equals(
                                new string(expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].FirstSequence.Select(a => (char)a).ToArray()))
                            && new string(actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].SecondSequence.Select(a => (char)a).ToArray()).Equals(
                               new string(expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].SecondSequence.Select(a => (char)a).ToArray()))
                            && actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].Score ==
                                expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].Score)
                            {
                                output = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return output;
        }

        #endregion Supporting Methods
    }
}
