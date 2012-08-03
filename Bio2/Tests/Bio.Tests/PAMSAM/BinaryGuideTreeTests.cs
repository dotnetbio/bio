using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for BinaryGuideTree class
    /// </summary>
    [TestClass]
    public class BinaryGuideTreeTests
    {
        /// <summary>
        /// Test BinaryGuideTree class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestBinaryGuideTree()
        {
            int numberOfNodes = 5;
            List<BinaryGuideTreeNode> nodes = new List<BinaryGuideTreeNode>(numberOfNodes);
            for (int i = 0; i < numberOfNodes; ++i)
            {
                nodes.Add(new BinaryGuideTreeNode(i));
            }

            nodes[3].LeftChildren = nodes[0];
            nodes[3].RightChildren = nodes[1];
            nodes[4].LeftChildren = nodes[3];
            nodes[4].RightChildren = nodes[2];

            nodes[0].Parent = nodes[3];
            nodes[1].Parent = nodes[3];
            nodes[2].Parent = nodes[4];
            nodes[3].Parent = nodes[4];


            Assert.IsFalse(nodes[0].IsRoot);
            Assert.IsTrue(nodes[0].IsLeaf);

            Assert.IsFalse(nodes[1].IsRoot);
            Assert.IsTrue(nodes[1].IsLeaf);

            Assert.IsFalse(nodes[2].IsRoot);
            Assert.IsTrue(nodes[2].IsLeaf);

            Assert.IsFalse(nodes[3].IsRoot);
            Assert.IsFalse(nodes[3].IsLeaf);

            Assert.IsTrue(nodes[4].IsRoot);
            Assert.IsFalse(nodes[4].IsLeaf);

            Assert.AreEqual(nodes[3], nodes[0].Parent);


            int numberOfEdges = 4;
            List<BinaryGuideTreeEdge> edges = new List<BinaryGuideTreeEdge>(numberOfEdges);
            for (int i = 0; i < numberOfEdges; ++i)
            {
                edges.Add(new BinaryGuideTreeEdge(i));
            }

            edges[0].ParentNode = nodes[3];
            edges[0].ChildNode = nodes[0];
            edges[1].ParentNode = nodes[3];
            edges[1].ChildNode = nodes[1];

            edges[2].ParentNode = nodes[4];
            edges[2].ChildNode = nodes[2];
            edges[3].ParentNode = nodes[4];
            edges[3].ChildNode = nodes[3];

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

            BinaryGuideTree binaryGuideTree = new BinaryGuideTree(hierarchicalClustering);

            Assert.AreEqual(7, binaryGuideTree.NumberOfNodes);
            Assert.AreEqual(6, binaryGuideTree.NumberOfEdges);
            Assert.AreEqual(4, binaryGuideTree.NumberOfLeaves);

            Assert.IsTrue(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1].IsRoot);

            for (int i = 0; i < binaryGuideTree.Nodes.Count; ++i)
            {
                Console.WriteLine(binaryGuideTree.Nodes[i].ID);
            }

            // Test ExtractSubTreeNodes
            Console.WriteLine("binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[4]).Count : {0}", binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[4]).Count);
            Console.WriteLine("binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1]).Count : {0}", binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1]).Count);
            Console.WriteLine("binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[0]).Count : {0}", binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[0]).Count);
            Assert.AreEqual(3, binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[4]).Count);
            Assert.AreEqual(7, binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1]).Count);
            Assert.AreEqual(1, binaryGuideTree.ExtractSubTreeNodes(binaryGuideTree.Nodes[0]).Count);

            // Test ExtractSubTreeLeafNodes
            Console.WriteLine("binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[4]).Count : {0}", binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[4]).Count);
            Console.WriteLine("binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1]).Coun : {0}", binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1]).Count);
            Console.WriteLine("binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[0]).Count : {0}", binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[0]).Count);
            Assert.AreEqual(2, binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[4]).Count);
            Assert.AreEqual(4, binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1]).Count);
            Assert.AreEqual(1, binaryGuideTree.ExtractSubTreeLeafNodes(binaryGuideTree.Nodes[0]).Count);


            // Test FindSmallestTreeDifference
            BinaryGuideTree binaryGuideTreeB = new BinaryGuideTree(hierarchicalClustering);
            BinaryGuideTreeNode node = BinaryGuideTree.FindSmallestTreeDifference(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1], binaryGuideTreeB.Nodes[binaryGuideTreeB.Nodes.Count - 1]);
            Assert.IsNull(node);
            node = BinaryGuideTree.FindSmallestTreeDifference(binaryGuideTree.Nodes[binaryGuideTree.Nodes.Count - 1], binaryGuideTreeB.Nodes[0]);
            Assert.IsNotNull(node);

            // Test CompareTwoTrees

            for (int i = 0; i < binaryGuideTree.Nodes.Count; ++i)
            {
                Console.Write(binaryGuideTree.Nodes[i].ID);
            }
            for (int i = 0; i < binaryGuideTreeB.Nodes.Count; ++i)
            {
                Console.Write(binaryGuideTreeB.Nodes[i].ID);
            }

            BinaryGuideTree.CompareTwoTrees(binaryGuideTree, binaryGuideTreeB);

            for (int i = 0; i < binaryGuideTree.Nodes.Count; ++i)
            {
                Console.Write(binaryGuideTree.Nodes[i].ID);
                Console.Write(binaryGuideTree.Nodes[i].NeedReAlignment);
                binaryGuideTree.Nodes[i].NeedReAlignment = false;
            }
            for (int i = 0; i < binaryGuideTreeB.Nodes.Count; ++i)
            {
                Console.Write(binaryGuideTreeB.Nodes[i].ID);
                Console.Write(binaryGuideTreeB.Nodes[i].NeedReAlignment);
                binaryGuideTreeB.Nodes[i].NeedReAlignment = false;
            }
            Assert.IsFalse(binaryGuideTree.Nodes[4].NeedReAlignment);
            Assert.IsFalse(binaryGuideTree.Nodes[5].NeedReAlignment);
            Assert.IsFalse(binaryGuideTree.Nodes[6].NeedReAlignment);

            for (int i = binaryGuideTree.NumberOfLeaves; i < binaryGuideTree.NumberOfNodes; ++i)
            {
                Assert.IsFalse(binaryGuideTree.Nodes[i].NeedReAlignment);
            }

            binaryGuideTreeB.Root.ID = 7;
            BinaryGuideTree.CompareTwoTrees(binaryGuideTree, binaryGuideTreeB);

            Assert.IsFalse(binaryGuideTree.Root.NeedReAlignment);

            binaryGuideTreeB.Nodes[5].ID = 8;
            BinaryGuideTree.CompareTwoTrees(binaryGuideTree, binaryGuideTreeB);

            for (int i = 0; i < binaryGuideTree.Nodes.Count; ++i)
            {
                Console.WriteLine(binaryGuideTree.Nodes[i].ID);
                Console.WriteLine(binaryGuideTree.Nodes[i].NeedReAlignment);
            }
            for (int i = 0; i < binaryGuideTreeB.Nodes.Count; ++i)
            {
                Console.WriteLine(binaryGuideTreeB.Nodes[i].ID);
                Console.WriteLine(binaryGuideTreeB.Nodes[i].NeedReAlignment);
            }

            Assert.IsFalse(binaryGuideTree.Nodes[5].NeedReAlignment);
            Assert.IsFalse(binaryGuideTree.Root.NeedReAlignment);

            binaryGuideTreeB.Nodes[5].LeftChildren = binaryGuideTreeB.Nodes[3];
            BinaryGuideTree.CompareTwoTrees(binaryGuideTree, binaryGuideTreeB);
            Assert.IsTrue(binaryGuideTree.Nodes[5].NeedReAlignment);
            Assert.IsTrue(binaryGuideTree.Root.NeedReAlignment);
            Assert.IsFalse(binaryGuideTree.Nodes[4].NeedReAlignment);

            // Test SeparateSequencesByCuttingTree
            List<int>[] newSequences = binaryGuideTree.SeparateSequencesByCuttingTree(3);
            Assert.AreEqual(1, newSequences[0].Count);
            Assert.AreEqual(3, newSequences[1].Count);
            Console.WriteLine("newSequences[0].Count: {0}", newSequences[0].Count);
            Console.WriteLine("newSequences[1].Count: {0}", newSequences[1].Count);


            List<int>[] newSequencesB = binaryGuideTree.SeparateSequencesByCuttingTree(2);
            Assert.AreEqual(2, newSequencesB[0].Count);
            Assert.AreEqual(2, newSequencesB[1].Count);
            Console.WriteLine("newSequences[0].Count: {0}", newSequencesB[0].Count);
            Console.WriteLine("newSequences[1].Count: {0}", newSequencesB[1].Count);

            // Cut tree test
            ISequence seq1 = new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT");
            ISequence seq2 = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG");
            ISequence seq3 = new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG");
            ISequence seq4 = new Sequence(Alphabets.DNA, "GGGAAATCG");
            ISequence seq5 = new Sequence(Alphabets.DNA, "GGGAATCAATCAG");
            ISequence seq6 = new Sequence(Alphabets.DNA, "GGGACAAAATCAG");
            ISequence seq7 = new Sequence(Alphabets.DNA, "GGGAATCTTATCAG");

            List<ISequence> sequences = new List<ISequence>();
            sequences.Add(seq1);
            sequences.Add(seq2);
            sequences.Add(seq3);
            sequences.Add(seq4);
            sequences.Add(seq5);
            sequences.Add(seq6);
            sequences.Add(seq7);

            // Generate DistanceMatrix
            KmerDistanceMatrixGenerator kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(sequences, 2, Alphabets.AmbiguousDNA, DistanceFunctionTypes.EuclideanDistance);

            // Hierarchical clustering
            IHierarchicalClustering hierarcicalClustering = new HierarchicalClusteringParallel(kmerDistanceMatrixGenerator.DistanceMatrix, UpdateDistanceMethodsTypes.Average);

            //// Generate Guide Tree
            binaryGuideTree = new BinaryGuideTree(hierarcicalClustering);

            // CUT Tree
            BinaryGuideTree[] subtrees = binaryGuideTree.CutTree(3);

            Assert.IsNotNull(subtrees);
        }
    }
}
