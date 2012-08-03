using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for ProgressiveAligner class
    /// </summary>
    [TestClass]
    public class ProgressiveAlignerTests
    {

        /// <summary>
        /// Test ProgressiveAligner class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestProgressiveAligner()
        {

            MsaUtils.SetProfileItemSets(Alphabets.AmbiguousDNA);

            SimilarityMatrix similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
            int gapOpenPenalty = -8;
            int gapExtendPenalty = -1;
            int kmerLength = 1;

            PAMSAMMultipleSequenceAligner.parallelOption = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            ISequence seqA = new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT");
            ISequence seqB = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG");
            //ISequence seqA = new Sequence(Alphabets.DNA, "G");
            //ISequence seqB = new Sequence(Alphabets.DNA, "G");
            ISequence seqC = new Sequence(Alphabets.DNA, "GGGACAAAATCAG");
            List<ISequence> sequences = new List<ISequence>();
            sequences.Add(seqA);
            sequences.Add(seqB);
            sequences.Add(seqC);

            KmerDistanceMatrixGenerator kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.DNA);

            kmerDistanceMatrixGenerator.GenerateDistanceMatrix(sequences);

            IHierarchicalClustering hierarchicalClustering = new HierarchicalClusteringParallel(kmerDistanceMatrixGenerator.DistanceMatrix);

            BinaryGuideTree tree = new BinaryGuideTree(hierarchicalClustering);

            IProgressiveAligner progressiveAligner = new ProgressiveAligner(ProfileAlignerNames.NeedlemanWunschProfileAligner, similarityMatrix, gapOpenPenalty, gapExtendPenalty);

            progressiveAligner.Align(sequences, tree);

            //ISequence expectedSeqA = new Sequence(Alphabets.DNA, "GGGA---AAAATCAGATT");
            //ISequence expectedSeqB = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG---");
            //ISequence expectedSeqC = new Sequence(Alphabets.DNA, "GGGA--CAAAATCAG---");

            //Assert.AreEqual(expectedSeqA.ToString(), progressiveAligner.AlignedSequences[0].ToString());
            //Assert.AreEqual(expectedSeqB.ToString(), progressiveAligner.AlignedSequences[1].ToString());
            //Assert.AreEqual(expectedSeqC.ToString(), progressiveAligner.AlignedSequences[2].ToString());

            string expectedSeqA = "GGGA---AAAATCAGATT";
            string expectedSeqB = "GGGAATCAAAATCAG---";
            string expectedSeqC = "GGGA--CAAAATCAG---";

            Assert.AreEqual(expectedSeqA, new string(progressiveAligner.AlignedSequences[0].Select(a => (char)a).ToArray()));
            Assert.AreEqual(expectedSeqB, new string(progressiveAligner.AlignedSequences[1].Select(a => (char)a).ToArray()));
            Assert.AreEqual(expectedSeqC, new string(progressiveAligner.AlignedSequences[2].Select(a => (char)a).ToArray()));

            sequences = new List<ISequence>();
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAATCG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCTTATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));

            kmerDistanceMatrixGenerator.GenerateDistanceMatrix(sequences);

            hierarchicalClustering = new HierarchicalClusteringParallel(kmerDistanceMatrixGenerator.DistanceMatrix);

            tree = new BinaryGuideTree(hierarchicalClustering);

            for (int i = 0; i < tree.NumberOfNodes; ++i)
            {
                Console.WriteLine("Node {0} ID: {1}", i, tree.Nodes[i].ID);
            }
            for (int i = 0; i < tree.NumberOfEdges; ++i)
            {
                Console.WriteLine("Edge {0} ID: {1}, length: {2}", i, tree.Edges[i].ID, tree.Edges[i].Length);
            }

            SequenceWeighting sw = new SequenceWeighting(tree);

            for (int i = 0; i < sw.Weights.Length; ++i)
            {
                Console.WriteLine("weights {0} is {1}", i, sw.Weights[i]);
            }

            progressiveAligner = new ProgressiveAligner(ProfileAlignerNames.NeedlemanWunschProfileAligner, similarityMatrix, gapOpenPenalty, gapExtendPenalty);
            progressiveAligner.Align(sequences, tree);
            for (int i = 0; i < progressiveAligner.AlignedSequences.Count; ++i)
            {
                Console.WriteLine(new string(progressiveAligner.AlignedSequences[i].Select(a => (char)a).ToArray()));
            }

            MsaUtils.SetProfileItemSets(Alphabets.AmbiguousProtein);
            string filepath = @"TestUtils\FASTA\Protein\BB11001.tfa";
            FastAParser parser = new FastAParser(filepath);
            IList<ISequence> orgSequences = parser.Parse().ToList();

            sequences = MsaUtils.UnAlign(orgSequences);

            similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62);
            kmerLength = 4;

            gapOpenPenalty = -13;
            gapExtendPenalty = -5;

            kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA);

            kmerDistanceMatrixGenerator.GenerateDistanceMatrix(sequences);

            hierarchicalClustering = new HierarchicalClusteringParallel(kmerDistanceMatrixGenerator.DistanceMatrix);

            tree = new BinaryGuideTree(hierarchicalClustering);

            for (int i = tree.NumberOfLeaves; i < tree.Nodes.Count; ++i)
            {
                Console.WriteLine("Node {0}: leftchildren-{1}, rightChildren-{2}", i, tree.Nodes[i].LeftChildren.ID, tree.Nodes[i].RightChildren.ID);
            }
            progressiveAligner = new ProgressiveAligner(ProfileAlignerNames.NeedlemanWunschProfileAligner, similarityMatrix, gapOpenPenalty, gapExtendPenalty);
            progressiveAligner.Align(sequences, tree);
            for (int i = 0; i < progressiveAligner.AlignedSequences.Count; ++i)
            {
                Console.WriteLine(new string(progressiveAligner.AlignedSequences[i].Select(a => (char)a).ToArray()));
            }

            ((FastAParser)parser).Dispose();
        }
    }
}
