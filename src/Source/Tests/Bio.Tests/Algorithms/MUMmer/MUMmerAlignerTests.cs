using System.Collections.Generic;
using System.Linq;
using Bio.Tests.Framework;
using NUnit.Framework;
using Bio.Algorithms.MUMmer;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;

namespace Bio.Tests.Algorithms.MUMmer
{
    /// <summary>
    /// Tests for the MummerAligner class.
    /// </summary>
    [TestFixture]
    public class MUMmerAlignerTests
    {
        #region MUMmer Test Cases

        /// <summary>
        /// Test MUMmer Aligner with extension penalty.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestMUMmerAlignerSingleMum()
        {
            const string reference = "TTAATTTTAG";
            const string search = "AGTTTAGAG";

            ISequence referenceSeq = new Sequence(Alphabets.DNA, reference);
            ISequence searchSeq = new Sequence(Alphabets.DNA, search);

            var searchSeqs = new List<ISequence> {searchSeq};

            MUMmerAligner mummer = new MUMmerAligner
            {
                LengthOfMUM = 3,
                PairWiseAlgorithm = new NeedlemanWunschAligner(),
                GapExtensionCost = -2
            };

            IList<IPairwiseSequenceAlignment> result = mummer.Align(referenceSeq, searchSeqs);

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.DNA, "TTAATTTTAG--"),
                SecondSequence = new Sequence(Alphabets.DNA, "---AGTTTAGAG"),
                Consensus = new Sequence(AmbiguousDnaAlphabet.Instance, "TTAAKTTTAGAG"),
                Score = -6,
                FirstOffset = 0,
                SecondOffset = 3
            };
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// MUMmer 3 test where we get multiple MUMs.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestMUMmerAlignerMultipleMum()
        {
            string reference = "ATGCGCATCCCCTT";
            string search = "GCGCCCCCTA";

            Sequence referenceSeq = null;
            Sequence searchSeq = null;

            referenceSeq = new Sequence(Alphabets.DNA, reference);
            searchSeq = new Sequence(Alphabets.DNA, search);

            List<ISequence> searchSeqs = new List<ISequence>();
            searchSeqs.Add(searchSeq);

            MUMmerAligner mummer = new MUMmerAligner();
            mummer.LengthOfMUM = 4;
            mummer.PairWiseAlgorithm = new NeedlemanWunschAligner();

            IList<IPairwiseSequenceAlignment> result = mummer.AlignSimple(referenceSeq, searchSeqs);

            // Check if output is not null
            Assert.AreNotEqual(null, result);
            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "ATGCGCATCCCCTT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "--GCGC--CCCCTA");
            alignedSeq.Consensus = new Sequence(AmbiguousDnaAlphabet.Instance, "ATGCGCATCCCCTW");
            alignedSeq.Score = -11;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 2;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// MUMmer 3 test where we get multiple MUMs.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestMUMmer3MultipleMumWithCustomMatrix()
        {
            string reference = "ATGCGCATCCCCTT";
            string search = "GCGCCCCCTA";

            Sequence referenceSeq = null;
            Sequence searchSeq = null;

            referenceSeq = new Sequence(Alphabets.DNA, reference);
            searchSeq = new Sequence(Alphabets.DNA, search);

            List<ISequence> searchSeqs = new List<ISequence>();
            searchSeqs.Add(searchSeq);

            int[,] customMatrix = new int[256, 256];

            customMatrix[(byte)'A', (byte)'A'] = 3;
            customMatrix[(byte)'A', (byte)'T'] = -2;
            customMatrix[(byte)'A', (byte)'G'] = -2;
            customMatrix[(byte)'A', (byte)'c'] = -2;

            customMatrix[(byte)'G', (byte)'G'] = 3;
            customMatrix[(byte)'G', (byte)'A'] = -2;
            customMatrix[(byte)'G', (byte)'T'] = -2;
            customMatrix[(byte)'G', (byte)'C'] = -2;

            customMatrix[(byte)'T', (byte)'T'] = 3;
            customMatrix[(byte)'T', (byte)'A'] = -2;
            customMatrix[(byte)'T', (byte)'G'] = -2;
            customMatrix[(byte)'T', (byte)'C'] = -2;

            customMatrix[(byte)'C', (byte)'C'] = 3;
            customMatrix[(byte)'C', (byte)'T'] = -2;
            customMatrix[(byte)'C', (byte)'A'] = -2;
            customMatrix[(byte)'C', (byte)'G'] = -2;

            DiagonalSimilarityMatrix matrix = new DiagonalSimilarityMatrix(3, -2);

            int gapOpenCost = -6;

            MUMmerAligner mummer = new MUMmerAligner();
            mummer.LengthOfMUM = 4;
            mummer.PairWiseAlgorithm = new NeedlemanWunschAligner();
            mummer.SimilarityMatrix = matrix;
            mummer.GapOpenCost = gapOpenCost;
            mummer.GapExtensionCost = -2;

            IList<IPairwiseSequenceAlignment> result = mummer.AlignSimple(referenceSeq, searchSeqs);

            // Check if output is not null
            Assert.AreNotEqual(null, result);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            PairwiseAlignedSequence alignedSeq = new PairwiseAlignedSequence();
            alignedSeq.FirstSequence = new Sequence(Alphabets.DNA, "ATGCGCATCCCCTT");
            alignedSeq.SecondSequence = new Sequence(Alphabets.DNA, "--GCGC--CCCCTA");
            alignedSeq.Consensus = new Sequence(AmbiguousDnaAlphabet.Instance, "ATGCGCATCCCCTW");
            alignedSeq.Score = 1;
            alignedSeq.FirstOffset = 0;
            alignedSeq.SecondOffset = 2;
            align.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(align);
            Assert.IsTrue(CompareAlignment(result, expectedOutput));
        }

        /// <summary>
        /// Test MUMmer 3 Align with RNA.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestMUMmerAlignerSingleMumRNA()
        {
            const string reference = "AUGCUUUUCCCCCCC";
            const string search = "UAUAUUUUGG";

            MUMmerAligner mummer = new MUMmerAligner
            {
                LengthOfMUM = 3,
                PairWiseAlgorithm = new NeedlemanWunschAligner(),
                SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna),
                GapOpenCost = -8,
                GapExtensionCost = -2
            };

            ISequence referenceSeq = new Sequence(Alphabets.RNA, reference);
            List<ISequence> searchSeqs = new List<ISequence> { new Sequence(Alphabets.RNA, search) };
            IList<IPairwiseSequenceAlignment> result = mummer.Align(referenceSeq, searchSeqs);

            // Check if output is not null
            Assert.AreNotEqual(0, result.Count);

            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment align = new PairwiseSequenceAlignment();
            align.PairwiseAlignedSequences.Add(new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(Alphabets.RNA,  "AUGCUUUUCCCCCCC"),
                SecondSequence = new Sequence(Alphabets.RNA, "AUA-UUUUGG-----"),
                Consensus = new Sequence(AmbiguousRnaAlphabet.Instance, "AURCUUUUSSCCCCC"),
                Score = -14,
                FirstOffset = 0,
                SecondOffset = 0
            });
            expectedOutput.Add(align);
            AlignmentHelpers.CompareAlignment(result, expectedOutput);
        }

        #endregion MUMer Test Cases

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
                            if (result[count].PairwiseAlignedSequences[count1].FirstSequence.ToStrings().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].FirstSequence.ToStrings())
                                && result[count].PairwiseAlignedSequences[count1].SecondSequence.ToStrings().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].SecondSequence.ToStrings())
                                && result[count].PairwiseAlignedSequences[count1].Consensus.ToStrings().Equals(
                                    expectedAlignment[count].PairwiseAlignedSequences[count1].Consensus.ToStrings())
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

    internal static class extension
    {
        public static string ToStrings(this ISequence sequence)
        {
            return new string(sequence.ToArray().Select(a => (char)a).ToArray());
        }
    }
}
