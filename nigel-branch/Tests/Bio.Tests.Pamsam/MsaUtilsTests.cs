using System;
using System.Collections.Generic;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.SimilarityMatrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for AlignmentScore functions
    /// </summary>
    [TestClass]
    public class AlignmentScoreTests
    {
        /// <summary>
        /// Test AlignmentScore functions
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAlignmentScore()
        {

            ISequence templateSequence = new Sequence(Alphabets.AmbiguousDNA, "ATGCSWRYKMBVHDN-");
            Dictionary<byte, int> itemSet = new Dictionary<byte, int>();
            for (int i = 0; i < templateSequence.Count; ++i)
            {
                itemSet.Add(templateSequence[i], i);

                if (char.IsLetter((char)templateSequence[i]))
                    itemSet.Add((byte)char.ToLower((char)templateSequence[i]), i);
            }
            Profiles.ItemSet = itemSet;

            SimilarityMatrix similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            int gapOpenPenalty = -8;
            int gapExtendPenalty = -1;

            // Test PairWiseScoreFunction
            ISequence seqA = new Sequence(Alphabets.AmbiguousDNA, "ACG");
            ISequence seqB = new Sequence(Alphabets.AmbiguousDNA, "ACG");
            float score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);




            //Assert.AreEqual(15, score);

            seqA = new Sequence(Alphabets.AmbiguousDNA, "ACG");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "ACC");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(6, score);

            seqA = new Sequence(Alphabets.AmbiguousDNA, "AC-");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "ACC");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(2, score);

            seqA = new Sequence(Alphabets.AmbiguousDNA, "AC--");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "ACCG");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(1, score);


            seqA = new Sequence(Alphabets.AmbiguousDNA, "A---");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "A--C");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(-3, score);


            seqA = new Sequence(Alphabets.AmbiguousDNA, "GGGA---AAAATCAGATT");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "GGGA--CAAAATCAG---");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(42, score);

            seqA = new Sequence(Alphabets.AmbiguousDNA, "GGG---AAAAATCAGATT");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "GGGA--CAAAATCAG---");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(33, score);

            seqA = new Sequence(Alphabets.AmbiguousDNA, "GGGA---AAAATCAGATT");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "GGGAATCAAAATCAG---");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(40, score);

            seqA = new Sequence(Alphabets.AmbiguousDNA, "GGGA--CAAAATCAG---");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "GGGAATCAAAATCAG---");

            score = MsaUtils.PairWiseScoreFunction(seqA, seqB, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            //Assert.AreEqual(56, score);

            // Test MultipleAlignmentScoreFunction
            List<ISequence> sequences = new List<ISequence>();
            seqA = new Sequence(Alphabets.AmbiguousDNA, "GGGA---AAAATCAGATT");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "GGGA--CAAAATCAG---");
            sequences.Add(seqA);
            sequences.Add(seqB);
            score = MsaUtils.MultipleAlignmentScoreFunction(sequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty);
            Console.WriteLine("alignment score is: {0}", score);
            for (int i = 0; i < sequences.Count; ++i)
            {
                Console.WriteLine(sequences[i].ToString());
            }
            //Assert.AreEqual(42, score);

            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCAAAATCAG---"));
            score = MsaUtils.MultipleAlignmentScoreFunction(sequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty);
            Console.WriteLine("alignment score is: {0}", score);
            for (int i = 0; i < sequences.Count; ++i)
            {
                Console.WriteLine(sequences[i].ToString());
            }
            //Assert.AreEqual(46, score);

            sequences[0] = new Sequence(Alphabets.AmbiguousDNA, "GGG---AAAAATCAGATT");
            score = MsaUtils.MultipleAlignmentScoreFunction(sequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty);
            for (int i = 0; i < sequences.Count; ++i)
            {
                Console.WriteLine(sequences[i].ToString());
            }
            Console.WriteLine("alignment score is: {0}", score);
            for (int i = 0; i < sequences.Count; ++i)
            {
                Console.WriteLine(sequences[i].ToString());
            }
            //Assert.AreEqual(40, score);

            // Test CalculateOffset
            seqA = new Sequence(Alphabets.AmbiguousDNA, "ABCD");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "ABCD");

            List<int> offset = MsaUtils.CalculateOffset(seqA, seqB);
            Console.WriteLine("offsets are:");
            for (int i = 0; i < offset.Count; ++i)
            {
                Console.Write("{0}\t", offset[i]);
            }

            seqA = new Sequence(Alphabets.AmbiguousDNA, "A-BCD");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "AB-CD");
            offset = MsaUtils.CalculateOffset(seqA, seqB);
            Console.WriteLine("\noffsets are:");
            for (int i = 0; i < offset.Count; ++i)
            {
                Console.Write("{0}\t", offset[i]);
            }

            seqA = new Sequence(Alphabets.AmbiguousDNA, "A-BCD");
            seqB = new Sequence(Alphabets.AmbiguousDNA, "----AB-CD");
            offset = MsaUtils.CalculateOffset(seqA, seqB);
            Console.WriteLine("\noffsets are:");
            for (int i = 0; i < offset.Count; ++i)
            {
                Console.Write("{0}\t", offset[i]);
            }

            sequences.Clear();
            sequences.Add(seqA);
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "ABBCG"));

            List<ISequence> sequencesRef = new List<ISequence>();
            sequencesRef.Add(seqA);
            sequencesRef.Add(new Sequence(Alphabets.AmbiguousDNA, "ABBCG"));

            for (int i = 0; i < sequences.Count; ++i)
            {
                offset = MsaUtils.CalculateOffset(sequences[i], sequencesRef[i]);
                Console.WriteLine("\noffsets are:");
                for (int j = 0; j < offset.Count; ++j)
                {
                    Console.Write("{0}\t", offset[j]);
                }
            }

            Console.WriteLine("Q score is: {0}", MsaUtils.CalculateAlignmentScoreQ(sequences, sequencesRef));
            Console.WriteLine("TC score is: {0}", MsaUtils.CalculateAlignmentScoreTC(sequences, sequencesRef));



            // Test on one example
            sequences.Clear();
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA---A-AAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCA-AAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCA-AAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA---A-A--TC-G---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCA-A--TCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCTTA--TCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA--CA-AAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA---A-AAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCA-AAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA--CA-AAATCAG---"));

            gapOpenPenalty = -4;
            gapExtendPenalty = -1;

            Console.WriteLine("score is: {0}", MsaUtils.MultipleAlignmentScoreFunction(sequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty));

            sequences.Clear();
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA---AAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCAAAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCAAAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA-----AATC-G---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATC--AATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCTTA-TCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA--CAAAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA---AAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGAATCAAAATCAG---"));
            sequences.Add(new Sequence(Alphabets.AmbiguousDNA, "GGGA--CAAAATCAG---"));

            Console.WriteLine("score is: {0}", MsaUtils.MultipleAlignmentScoreFunction(sequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty));

            // Test Quick Sort
            float[] a = new float[5] { 0, 2, 1, 5, 4 };
            int[] aIndex = new int[5] { 0, 1, 2, 3, 4 };
            MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            Console.WriteLine("quicksort");
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(a[i]);
            }
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(aIndex[i]);
            }

            Console.WriteLine("quicksortM");
            a = new float[5] { 0, 2, 1, 5, 4 };
            int[] aIndexB = null;
            MsaUtils.QuickSortM(a, out aIndexB, 0, 4);
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(a[i]);
            }
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(aIndexB[i]);
            }

            Console.WriteLine("quicksort");
            a = new float[5] { 0, 2, 1, 5, 4 };
            int[] aIndexC = MsaUtils.CreateIndexArray(a.Length);
            MsaUtils.QuickSort(a, aIndexC, 0, a.Length - 1);
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(aIndexC[i]);
            }

            a = new float[5] { 1, 0, 0, 0, 0 };
            aIndex = new int[5] { 0, 1, 2, 3, 4 };
            MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(a[i]);
            }
            for (int i = 0; i < a.Length; ++i)
            {
                Console.WriteLine(aIndex[i]);
            }
        }
    }
}
