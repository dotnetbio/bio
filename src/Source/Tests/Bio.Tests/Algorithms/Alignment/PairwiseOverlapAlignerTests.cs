using System;
using System.Collections.Generic;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment
{
    /// <summary>
    /// Tests for the PairwiseOverlapAligner class.
    /// </summary>
    [TestFixture]
    public class PairwiseOverlapAlignerTests
    {
        #region PairwiseOverlap Aligner

        /// <summary>
        /// Test PairwiseOverlapAligner using Protein sequence and Simple Gap Penalty Function.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapProteinSeqSimpleGap()
        {
            string sequenceString1 = "HEAGAWGHEE";
            string sequenceString2 = "PAWHEAE";

            Sequence sequence1 = new Sequence(Alphabets.Protein, sequenceString1);
            Sequence sequence2 = new Sequence(Alphabets.Protein, sequenceString2);

            SimilarityMatrix sm = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
            int gapPenalty = -8;

            PairwiseOverlapAligner overlap = new PairwiseOverlapAligner();
            overlap.SimilarityMatrix = sm;
            overlap.GapOpenCost = gapPenalty;
            IList<IPairwiseSequenceAlignment> result = overlap.AlignSimple(sequence1, sequence2);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "{0}, Simple; Matrix {1}; GapOpenCost {2}", overlap.Name, overlap.SimilarityMatrix.Name, overlap.GapOpenCost));
            foreach (IPairwiseSequenceAlignment sequenceResult in result)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "score {0}", sequenceResult.PairwiseAlignedSequences[0].Score));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 0     {0}", sequenceResult.FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 1     {0}", sequenceResult.SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 0    {0}", sequenceResult.PairwiseAlignedSequences[0].FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 1    {0}", sequenceResult.PairwiseAlignedSequences[0].SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "consesus    {0}", sequenceResult.PairwiseAlignedSequences[0].Consensus.ToString()));
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.Protein, "GAWGHEE");
            alignedSeq.SecondSequence = new Sequence(Alphabets.Protein, "PAW-HEA");
            alignedSeq.Consensus = new Sequence(Alphabets.AmbiguousProtein, "XAWGHEX");
            alignedSeq.Score = 25;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 3;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test PairwiseOverlapAligner using Protein sequence and Affine Gap Penalty Function.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapProteinSeqAffineGap()
        {
            string sequenceString1 = "HEAGAWGHEE";
            string sequenceString2 = "PAWHEAE";

            Sequence sequence1 = new Sequence(Alphabets.Protein, sequenceString1);
            Sequence sequence2 = new Sequence(Alphabets.Protein, sequenceString2);

            SimilarityMatrix sm = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum50);
            int gapPenalty = -8;

            PairwiseOverlapAligner overlap = new PairwiseOverlapAligner();
            overlap.SimilarityMatrix = sm;
            overlap.GapOpenCost = gapPenalty;
            overlap.GapExtensionCost = -1;
            IList<IPairwiseSequenceAlignment> result = overlap.Align(sequence1, sequence2);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
            "{0}, Affine; Matrix {1}; GapOpenCost {2}; GapExtenstionCost {3}",
            overlap.Name, overlap.SimilarityMatrix.Name, overlap.GapOpenCost, overlap.GapExtensionCost));
            foreach (IPairwiseSequenceAlignment sequenceResult in result)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "score {0}", sequenceResult.PairwiseAlignedSequences[0].Score));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 0     {0}", sequenceResult.FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 1     {0}", sequenceResult.SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 0    {0}", sequenceResult.PairwiseAlignedSequences[0].FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 1    {0}", sequenceResult.PairwiseAlignedSequences[0].SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "consesus    {0}", sequenceResult.PairwiseAlignedSequences[0].Consensus.ToString()));
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.Protein, "GAWGHEE");
            alignedSeq.SecondSequence = new Sequence(Alphabets.Protein, "PAW-HEA");
            alignedSeq.Consensus = new Sequence(Alphabets.AmbiguousProtein, "XAWGHEX");
            alignedSeq.Score = 25;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 3;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test PairwiseOverlapAligner using Protein sequence with zero overlap
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapProteinSeqWithZeroOverlap()
        {
            Sequence sequence1 = new Sequence(Alphabets.Protein, "ACDEF");
            Sequence sequence2 = new Sequence(Alphabets.Protein, "TUVWY");
            SimilarityMatrix sm = new DiagonalSimilarityMatrix(5, -5);
            int gapPenalty = -10;
            PairwiseOverlapAligner overlap = new PairwiseOverlapAligner();
            overlap.SimilarityMatrix = sm;
            overlap.GapOpenCost = gapPenalty;
            overlap.GapExtensionCost = -1;
            IList<IPairwiseSequenceAlignment> result = overlap.Align(sequence1, sequence2);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "{0}, Simple; Matrix {1}; GapOpenCost {2}", overlap.Name, overlap.SimilarityMatrix.Name, overlap.GapOpenCost));
            foreach (IPairwiseSequenceAlignment sequenceResult in result)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "score {0}", sequenceResult.PairwiseAlignedSequences[0].Score));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 0     {0}", sequenceResult.FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 1     {0}", sequenceResult.SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 0    {0}", sequenceResult.PairwiseAlignedSequences[0].FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 1    {0}", sequenceResult.PairwiseAlignedSequences[0].SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "consesus    {0}", sequenceResult.PairwiseAlignedSequences[0].Consensus.ToString()));
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test PairwiseOverlapAligner for cases where it returns multiple alignments
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PairwiseOverlapMultipleAlignments()
        {
            Sequence sequence1 = new Sequence(Alphabets.DNA, "CCCAACCC");
            Sequence sequence2 = new Sequence(Alphabets.DNA, "CCC");
            SimilarityMatrix sm = new DiagonalSimilarityMatrix(5, -20);
            int gapPenalty = -10;
            PairwiseOverlapAligner overlap = new PairwiseOverlapAligner();
            overlap.SimilarityMatrix = sm;
            overlap.GapOpenCost = gapPenalty;
            IList<IPairwiseSequenceAlignment> result = overlap.AlignSimple(sequence1, sequence2);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "{0}, Simple; Matrix {1}; GapOpenCost {2}", overlap.Name, overlap.SimilarityMatrix.Name, overlap.GapOpenCost));
            foreach (IPairwiseSequenceAlignment sequenceResult in result)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "score {0}", sequenceResult.PairwiseAlignedSequences[0].Score));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 0     {0}", sequenceResult.FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "input 1     {0}", sequenceResult.SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 0    {0}", sequenceResult.PairwiseAlignedSequences[0].FirstSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "result 1    {0}", sequenceResult.PairwiseAlignedSequences[0].SecondSequence.ToString()));
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "consesus    {0}", sequenceResult.PairwiseAlignedSequences[0].Consensus.ToString()));
            }

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();

            // First alignment
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "CCC");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "CCC");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "CCC");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 0;
            align.PairwiseAlignedSequences.Add(alignedSeq);

            // Second alignment
            alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "CCC");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "CCC");
            alignedSeq.Consensus = new Sequence(Alphabets.DNA, "CCC");
            alignedSeq.Score = 15;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 5;
            align.PairwiseAlignedSequences.Add(alignedSeq);

            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }
        #endregion PairwiseOverlap Aligner

        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="result">output of Aligners</param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        private static bool CompareAlignment(
                IList<IPairwiseSequenceAlignment> result,
                IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            bool output = true;
            if (result.Count == expectedAlignment.Count)
            {
                for (int count = 0; count < result.Count; count++)
                {
                    if (result[count].PairwiseAlignedSequences.Count == expectedAlignment[count].PairwiseAlignedSequences.Count)
                    {
                        for (int count1 = 0; count1 < result[count].PairwiseAlignedSequences.Count; count1++)
                        {
                            if (result[count].PairwiseAlignedSequences[count1].FirstSequence.ToString().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].FirstSequence.ToString())
                                && result[count].PairwiseAlignedSequences[count1].SecondSequence.ToString().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].SecondSequence.ToString())
                                && result[count].PairwiseAlignedSequences[count1].Consensus.ToString().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].Consensus.ToString())
                                && result[count].PairwiseAlignedSequences[count1].FirstOffset ==
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].FirstOffset
                                && result[count].PairwiseAlignedSequences[count1].SecondOffset ==
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].SecondOffset
                                && result[count].PairwiseAlignedSequences[count1].Score ==
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].Score)
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
    }
}
