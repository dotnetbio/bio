using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.Framework
{
    /// <summary>
    /// Alignment helpers class
    /// </summary>
    public static class AlignmentHelpers
    {
        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="result">output of Aligners</param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        public static bool CompareAlignment(IList<IPairwiseSequenceAlignment> result, IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            Assert.AreEqual(expectedAlignment.Count, result.Count, "Different number of alignments generated.");

            for (int count = 0; count < result.Count; count++)
            {
                Assert.AreEqual(expectedAlignment[count].PairwiseAlignedSequences.Count, result[count].PairwiseAlignedSequences.Count, 
                                "Different count of sequences on alignment " + (count+1) + "generated");

                for (int count1 = 0; count1 < result[count].PairwiseAlignedSequences.Count; count1++)
                {
                    // Compare the sequences
                    var s1 = result[count].PairwiseAlignedSequences[count1].FirstSequence;
                    var s2 = expectedAlignment[count].PairwiseAlignedSequences[count1].FirstSequence;
                    Assert.AreEqual(s2.ConvertToString(), s1.ConvertToString(), "First sequence did not match.");

                    s1 = result[count].PairwiseAlignedSequences[count1].SecondSequence;
                    s2 = expectedAlignment[count].PairwiseAlignedSequences[count1].SecondSequence;
                    Assert.AreEqual(s2.ConvertToString(), s1.ConvertToString(), "Second sequence did not match.");

                    s1 = result[count].PairwiseAlignedSequences[count1].Consensus;
                    s2 = expectedAlignment[count].PairwiseAlignedSequences[count1].Consensus;
                    Assert.AreEqual(s2.ConvertToString(), s1.ConvertToString(), "Consensus did not match.");

                    long offset1 = result[count].PairwiseAlignedSequences[count1].FirstOffset;
                    long offset2 = expectedAlignment[count].PairwiseAlignedSequences[count1].FirstOffset;
                    Assert.AreEqual(offset2, offset1, "FirstOffset does not match.");

                    offset1 = result[count].PairwiseAlignedSequences[count1].SecondOffset;
                    offset2 = expectedAlignment[count].PairwiseAlignedSequences[count1].SecondOffset;
                    Assert.AreEqual(offset2, offset1, "SecondOffset does not match.");

                    long score1 = result[count].PairwiseAlignedSequences[count1].Score;
                    long score2 = expectedAlignment[count].PairwiseAlignedSequences[count1].Score;
                    Assert.AreEqual(score2, score1, "Score does not match.");
                }
            }

            return true;
        }

        /// <summary>
        /// Logs results to current Application log
        /// </summary>
        /// <param name="psa"></param>
        /// <param name="result"></param>
        public static void LogResult(IPairwiseSequenceAligner psa, IEnumerable<IPairwiseSequenceAlignment> result)
        {
            ApplicationLog.WriteLine("{0}, Matrix {1}; GapOpenCost {2}, GapExtensionCost {3}", psa.Name, psa.SimilarityMatrix.Name, psa.GapOpenCost, psa.GapExtensionCost);
            foreach (IPairwiseSequenceAlignment sequenceResult in result)
            {
                ApplicationLog.WriteLine("Input 0     {0}", sequenceResult.FirstSequence);
                ApplicationLog.WriteLine("Input 1     {0}", sequenceResult.SecondSequence);
                int count = 1;
                foreach (var oneResult in sequenceResult.PairwiseAlignedSequences)
                {
                    ApplicationLog.WriteLine(" #" + count++);
                    ApplicationLog.WriteLine("  Score       {0}", oneResult.Score);
                    ApplicationLog.WriteLine("  Result 0    {0}", oneResult.FirstSequence);
                    ApplicationLog.WriteLine("  Result 1    {0}", oneResult.SecondSequence);
                    ApplicationLog.WriteLine("  Offset 0    {0}", oneResult.FirstOffset);
                    ApplicationLog.WriteLine("  Offset 1    {0}", oneResult.SecondOffset);
                    ApplicationLog.WriteLine("  Consensus   {0}", oneResult.Consensus);
                }
            }
        }
    }
}