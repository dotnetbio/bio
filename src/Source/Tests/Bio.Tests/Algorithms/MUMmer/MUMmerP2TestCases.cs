using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.MUMmer;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Tests.Framework;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.TestAutomation.Algorithms.MUMmer
{
    /// <summary>
    ///     MUMmer Priority Two Test case implementation.
    /// </summary>
    [TestFixture]
    public class MUMmerP2TestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\MUMmerTestsConfig.xml");

        #region MUMmer Align Test Cases

        /// <summary>
        ///     Validate Align(reference, query) method with 1000 BP Dna sequence
        ///     and validate the aligned sequences
        ///     Input : Dna sequence with 1000 BP.
        ///     Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void MUMmerAlignDnaSequenceWith1000BP()
        {
            ValidateMUMmerAlignGeneralTestCases(Constants.Dna1000BPSequenceNodeName);
        }

        #endregion MUMmer Align Test Cases

        #region Supported Methods

        /// <summary>
        ///     Validates the Mummer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void ValidateMUMmerAlignGeneralTestCases(string nodeName)
        {
            // Gets the reference sequence from the configuration file
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

            Assert.IsNotNull(filePath);
            ApplicationLog.WriteLine(string.Format(null, "MUMmer P2 : Successfully validated the File Path '{0}'.", filePath));

            var fastaParserObj = new FastAParser();
            IEnumerable<ISequence> referenceSeqs = fastaParserObj.Parse(filePath);

            ISequence referenceSeq = referenceSeqs.ElementAt(0);

            // Gets the reference sequence from the configuration file
            string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);

            Assert.IsNotNull(queryFilePath);
            ApplicationLog.WriteLine(string.Format(null, "MUMmer P2 : Successfully validated the Search File Path '{0}'.", queryFilePath));

            var fastaParserObj1 = new FastAParser();
            IEnumerable<ISequence> querySeqs = fastaParserObj1.Parse(queryFilePath);

            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            var mum = new MUMmerAligner
            {
                LengthOfMUM = long.Parse(mumLength, null),
                StoreMUMs = true,
                PairWiseAlgorithm = new NeedlemanWunschAligner(),
                GapOpenCost = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode), null)
            };

            IList<IPairwiseSequenceAlignment> align = mum.Align(referenceSeq, querySeqs);

            // Validate FinalMUMs and MUMs Properties.
            Assert.IsNotNull(mum.MUMs);

            string expectedScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ScoreNodeName);

            string[] expectedSequences = utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ExpectedSequencesNode);
            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment seqAlign = new PairwiseSequenceAlignment();
            var alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(referenceSeq.Alphabet, expectedSequences[0]),
                SecondSequence = new Sequence(referenceSeq.Alphabet, expectedSequences[1]),
                Score = Convert.ToInt32(expectedScore, null),
                FirstOffset = Int32.MinValue,
                SecondOffset = Int32.MinValue,
            };
            seqAlign.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(seqAlign);
            Assert.IsTrue(AlignmentHelpers.CompareAlignment(align, expectedOutput));

            ApplicationLog.WriteLine("MUMmer P2 : Successfully validated the aligned sequences.");
        }

        #endregion Supported Methods
    }
}