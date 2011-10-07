using System;
using System.Linq;
using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.MUMmer;
using Bio.IO;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.IO.FastA;
using Bio;

namespace Bio.TestAutomation.Algorithms.MUMmer
{
    /// <summary>
    /// MUMmer Priority Two Test case implementation.
    /// </summary>
    [TestClass]
    public class MUMmerP2TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\MUMmerTestsConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static MUMmerP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region MUMmer Align Test Cases

        /// <summary>
        /// Validate Align(reference, query) method with 1000 BP Dna sequence 
        /// and validate the aligned sequences
        /// Input : Dna sequence with 1000 BP.
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void MUMmerAlignDnaSequenceWith1000BP()
        {
            ValidateMUMmerAlignGeneralTestCases(Constants.Dna1000BPSequenceNodeName);
        }

        #endregion MUMmer Align Test Cases

        #region Supported Methods

        /// <summary>
        /// Validates the Mummer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        /// <param name="parserParam">Parser Parameter</param>
        /// <param name="addParam">Additional parameter</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateMUMmerAlignGeneralTestCases(string nodeName)
        {
            ISequence referenceSeq = null;
            IEnumerable<ISequence> querySeqs = null;
            IEnumerable<ISequence> referenceSeqs = null;

            // Gets the reference sequence from the configurtion file
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsNotNull(filePath);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "MUMmer P2 : Successfully validated the File Path '{0}'.",
                filePath));

            FastAParser fastaParserObj = new FastAParser(filePath);
            referenceSeqs = fastaParserObj.Parse();

            referenceSeq = referenceSeqs.ElementAt(0);

            // Gets the reference sequence from the configurtion file
            string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SearchSequenceFilePathNode);

            Assert.IsNotNull(queryFilePath);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "MUMmer P2 : Successfully validated the Search File Path '{0}'.",
                queryFilePath));

            FastAParser fastaParserObj1 = new FastAParser(queryFilePath);
            querySeqs = fastaParserObj1.Parse();

            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            MUMmerAligner mum = new MUMmerAligner();
            mum.LengthOfMUM = long.Parse(mumLength, null);
            mum.StoreMUMs = true;

            mum.PairWiseAlgorithm = new NeedlemanWunschAligner();
            mum.GapOpenCost =
                int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode),
                (IFormatProvider)null);
            IList<IPairwiseSequenceAlignment> align = null;

            align = mum.Align(referenceSeq, querySeqs);

            // Validate FinalMUMs and MUMs Properties.
            Assert.IsNotNull(mum.MUMs);

            string expectedScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ScoreNodeName);

            string[] expectedSequences =
                utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ExpectedSequencesNode);
            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment seqAlign = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(referenceSeq.Alphabet, expectedSequences[0]);
            alignedSeq.SecondSequence = new Sequence(referenceSeq.Alphabet, expectedSequences[1]);
            alignedSeq.Score = Convert.ToInt32(expectedScore, (IFormatProvider)null);
            seqAlign.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(seqAlign);
            Assert.IsTrue(CompareAlignment(align, expectedOutput));

            Console.WriteLine("MUMmer P2 : Successfully validated the aligned sequences.");
            ApplicationLog.WriteLine("MUMmer P2 : Successfully validated the aligned sequences.");
        }

        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="actualAlignment">actual alignment</param>
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
                    if (actualAlignment[resultCount].PairwiseAlignedSequences.Count ==
                        expectedAlignment[resultCount].PairwiseAlignedSequences.Count)
                    {
                        for (int alignSeqCount = 0; alignSeqCount <
                            actualAlignment[resultCount].PairwiseAlignedSequences.Count; alignSeqCount++)
                        {
                            // Validates the First Sequence, Second Sequence and Score
                            if (actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].FirstSequence.ToString().Equals(
                                    expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].FirstSequence.ToString())
                                && actualAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].SecondSequence.ToString().Equals(
                                    expectedAlignment[resultCount].PairwiseAlignedSequences[alignSeqCount].SecondSequence.ToString())
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

        #endregion Supported Methods
    }
}
