using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.SimilarityMatrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for ParallelDynamicProgramming class
    /// </summary>
    [TestClass]
    public class ParallelDynamicProgrammingTests
    {
        /// <summary>
        /// Test ParallelDynamicProgramming class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestParallelDynamicProgramming()
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

            NeedlemanWunschProfileAlignerSerial profileAligner = new NeedlemanWunschProfileAlignerSerial();
            SimilarityMatrix similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            int gapOpenPenalty = -8;
            int gapExtendPenalty = -1;

            profileAligner.SimilarityMatrix = similarityMatrix;
            profileAligner.GapOpenCost = gapOpenPenalty;
            profileAligner.GapExtensionCost = gapExtendPenalty;



            int numberOfRows = 8, numberOfCols = 7;
            int numberOfPartitions = 4;

            int startPosition = 1, endPosition = 100; Dictionary<int, List<int[]>> parallelIndexMaster = profileAligner.ParallelIndexMasterGenerator(numberOfRows, numberOfCols, numberOfPartitions);
            foreach (var pair in parallelIndexMaster)
            {
                Console.Write("{0} ->: ", pair.Key);
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    Console.WriteLine("iteration: {0}: {1}-{2};", i, pair.Value[i][0], pair.Value[i][1]);
                }
            }

            for (int partitionIndex = 0; partitionIndex < numberOfPartitions; ++partitionIndex)
            {
                int[] indexPositions = profileAligner.IndexLocator(startPosition, endPosition, numberOfPartitions, partitionIndex);
                Console.Write("Index number: {0}: {1}-{2}", partitionIndex, indexPositions[0], indexPositions[1]);
            }

            int numberOfIterations = numberOfPartitions * 2 - 1;

            for (int i = 0; i < numberOfIterations; ++i)
            {
                foreach (var pair in parallelIndexMaster)
                {
                    List<int[]> indexPositions = parallelIndexMaster[pair.Key];

                    // Parallel in anti-diagonal direction
                    Parallel.ForEach(indexPositions, indexPosition =>
                    {
                        int[] rowPositions = profileAligner.IndexLocator(1, 100, numberOfPartitions, indexPosition[0]);
                        int[] colPositions = profileAligner.IndexLocator(1, 200, numberOfPartitions, indexPosition[0]);
                        Console.Write("row positions: {0}-{1}", rowPositions[0], rowPositions[1]);
                        Console.Write("col positions: {0}-{1}", colPositions[0], colPositions[1]);
                    });
                }
            }
        }
    }
}
