using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.IO.FastA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for Hierarchical Clustering Serial Algorithm
    /// </summary>
    [TestClass]
    public class HierarchicalClusteringSerialTests
    {

        /// <summary>
        /// Test Hierarcical clustering algorithm
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestHierarchicalClusteringSerial()
        {
            int dimension = 4;
            IDistanceMatrix distanceMatrix = new SymmetricDistanceMatrix(dimension);
            for (int i = 0; i < distanceMatrix.Dimension - 1; ++i)
            {
                for (int j = i + 1; j < distanceMatrix.Dimension; ++j)
                {
                    distanceMatrix[i, j] = i + j;
                    distanceMatrix[j, i] = i + j;
                }
            }

            PAMSAMMultipleSequenceAligner.parallelOption = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            IHierarchicalClustering hierarchicalClustering = new HierarchicalClusteringParallel(distanceMatrix);

            Assert.AreEqual(7, hierarchicalClustering.Nodes.Count);
            for (int i = 0; i < dimension * 2 - 1; ++i)
            {
                Assert.AreEqual(i, hierarchicalClustering.Nodes[i].ID);
            }

            for (int i = dimension; i < hierarchicalClustering.Nodes.Count; ++i)
            {
                Console.WriteLine(hierarchicalClustering.Nodes[i].LeftChildren.ID);
                Console.WriteLine(hierarchicalClustering.Nodes[i].RightChildren.ID);
            }

            // Test on sequences
            ISequence seqA = new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT");
            ISequence seqB = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG");
            ISequence seqC = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG");
            List<ISequence> sequences = new List<ISequence>();
            sequences.Add(seqA);
            sequences.Add(seqB);
            sequences.Add(seqC);
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAATCG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAATCAG"));

            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCTTATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));

            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));

            int kmerLength = 4;
            KmerDistanceMatrixGenerator kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA);

            //Console.WriteLine(kmerDistanceMatrixGenerator.Name);
            kmerDistanceMatrixGenerator.GenerateDistanceMatrix(sequences);
            //Console.WriteLine(kmerDistanceMatrixGenerator.DistanceMatrix);

            for (int i = 0; i < kmerDistanceMatrixGenerator.DistanceMatrix.Dimension - 1; ++i)
            {
                for (int j = i + 1; j < kmerDistanceMatrixGenerator.DistanceMatrix.Dimension; ++j)
                {
                    Console.WriteLine("{0}-{1}: {2}", i, j, kmerDistanceMatrixGenerator.DistanceMatrix[i, j]);
                }
            }

            hierarchicalClustering = new HierarchicalClusteringParallel(kmerDistanceMatrixGenerator.DistanceMatrix);
            for (int i = 0; i < hierarchicalClustering.Nodes.Count; ++i)
            {
                Assert.AreEqual(true, hierarchicalClustering.Nodes[i].NeedReAlignment);
            }

            BinaryGuideTree tree = new BinaryGuideTree(hierarchicalClustering);
            for (int i = 0; i < tree.Nodes.Count; ++i)
            {
                Assert.AreEqual(true, tree.Nodes[i].NeedReAlignment);
            }


            // SimilarityMatrix similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            //Assert.AreEqual(0, hierarchicalClustering.Nodes[4].LeftChildren.ID);
            //Assert.AreEqual(1, hierarchicalClustering.Nodes[4].RightChildren.ID);
            //Assert.AreEqual(2, hierarchicalClustering.Nodes[5].LeftChildren.ID);
            //Assert.AreEqual(4, hierarchicalClustering.Nodes[5].RightChildren.ID);
            //Assert.AreEqual(3, hierarchicalClustering.Nodes[6].LeftChildren.ID);
            //Assert.AreEqual(5, hierarchicalClustering.Nodes[6].RightChildren.ID);


            // Test on larger dataset
            string filepath = @"TestUtils\FASTA\RV11_BBS_all.afa";
            FastAParser parser = new FastAParser(filepath);
            IList<ISequence> orgSequences = parser.Parse().ToList();

            sequences = MsaUtils.UnAlign(orgSequences);

            kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA);

            kmerDistanceMatrixGenerator.GenerateDistanceMatrix(sequences);

            hierarchicalClustering = new HierarchicalClusteringParallel(kmerDistanceMatrixGenerator.DistanceMatrix);

            for (int i = sequences.Count; i < hierarchicalClustering.Nodes.Count; ++i)
            {
                Console.WriteLine("Node {0}: leftchildren-{1}, rightChildren-{2}", i, hierarchicalClustering.Nodes[i].LeftChildren.ID, hierarchicalClustering.Nodes[i].RightChildren.ID);
            }

            ((FastAParser)parser).Dispose();
        }
    }
}
