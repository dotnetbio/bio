using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Padena;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for Step 3 in Parallel De Novo Assembly
    /// This step performs error correction on the input graph.
    /// It removes dangling links in the graph.
    /// </summary>
    [TestClass]
    public class DanglingLinksPurgerTests : ParallelDeNovoAssembler
    {
        /// <summary>
        /// Test Step 3 - Dangling Link Purger class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestDanglingLinksPurger()
        {
            const int KmerLength = 11;
            const int DangleThreshold = 3;
            List<ISequence> readSeqs = TestInputs.GetDanglingReads();
            SequenceReads.Clear();
            this.SetSequenceReads(readSeqs);
            this.KmerLength = KmerLength;
            DanglingLinksThreshold = DangleThreshold;
            DanglingLinksPurger = new DanglingLinksPurger(DangleThreshold);

            CreateGraph();
            long graphCount = Graph.NodeCount;

            long graphEdges = Graph.GetNodes().Select(n => n.ExtensionsCount).Sum();
            HashSet<string> graphNodes = new HashSet<string>(
            Graph.GetNodes().Select(n => new string(Graph.GetNodeSequence(n).Select(a => (char)a).ToArray())));

            DanglingLinksThreshold = DangleThreshold;
            UnDangleGraph();
            long dangleRemovedGraphCount = Graph.NodeCount;
            long dangleRemovedGraphEdge = Graph.GetNodes().Select(n => n.ExtensionsCount).Sum();
            HashSet<string> dangleRemovedGraphNodes = new HashSet<string>(
            Graph.GetNodes().Select(n => 
                {
                    return new string(Graph.GetNodeSequence(n).Select(a => (char)a).ToArray());
                }));

            // Compare the two graphs
            Assert.AreEqual(2, graphCount - dangleRemovedGraphCount);
            Assert.AreEqual(4, graphEdges - dangleRemovedGraphEdge);
            graphNodes.ExceptWith(dangleRemovedGraphNodes);
            Assert.IsTrue(graphNodes.Contains("TCGAACGATGA"));
            Assert.IsTrue(graphNodes.Contains("ATCGAACGATG"));
        }
    }
}
