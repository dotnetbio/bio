using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Padena;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for Step 5 in Parallel De Novo Assembly
    /// This step builds contigs from input graph.
    /// </summary>
    [TestClass]
    public class ContigBuilderTests : ParallelDeNovoAssembler
    {
        /// <summary>
        /// Test Step 5 - Contig Builder Class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestContigBuilder1()
        {
            const int KmerLength = 11;
            const int DangleThreshold = 3;
            const int RedundantThreshold = 10;

            List<ISequence> readSeqs = TestInputs.GetDanglingReads();
            this.SequenceReads.Clear();
            this.SetSequenceReads(readSeqs);
            this.KmerLength = KmerLength;
            DanglingLinksThreshold = DangleThreshold;
            DanglingLinksPurger = new DanglingLinksPurger(DangleThreshold);
            RedundantPathLengthThreshold = RedundantThreshold;
            RedundantPathsPurger = new RedundantPathsPurger(RedundantThreshold);
            ContigBuilder = new SimplePathContigBuilder();

            CreateGraph();
            UnDangleGraph();
            RemoveRedundancy();
            long graphCount = Graph.NodeCount;
            long graphEdges = Graph.GetNodes().Select(n => n.ExtensionsCount).Sum();

            IEnumerable<ISequence> contigs = BuildContigs();
            long contigsBuiltGraphCount = this.Graph.NodeCount;
            long contigsBuilt = Graph.GetNodes().Select(n => n.ExtensionsCount).Sum();

            // Compare the two graphs
            Assert.AreEqual(1, contigs.Count());
            HashSet<string> expectedContigs = new HashSet<string>() 
            { 
                "ATCGCTAGCATCGAACGATCATT" 
            };

            foreach (ISequence contig in contigs)
            {
                string s = new string(contig.Select(a => (char)a).ToArray());
                Assert.IsTrue(expectedContigs.Contains(s));
            }

            Assert.AreEqual(graphCount, contigsBuiltGraphCount);
            Assert.AreEqual(graphEdges, contigsBuilt);
        }

        /// <summary>
        /// Test Step 5 - Contig Builder Class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestContigBuilder2()
        {
            const int KmerLength = 6;
            const int RedundantThreshold = 10;

            List<ISequence> readSeqs = TestInputs.GetRedundantPathReads();
            SequenceReads.Clear();
            this.SetSequenceReads(readSeqs);
            this.KmerLength = KmerLength;
            RedundantPathLengthThreshold = RedundantThreshold;
            RedundantPathsPurger = new RedundantPathsPurger(RedundantThreshold);
            ContigBuilder = new SimplePathContigBuilder();

            CreateGraph();
            RemoveRedundancy();
            long graphCount = Graph.NodeCount;
            long graphEdges = Graph.GetNodes().Select(n => n.ExtensionsCount).Sum();

            IEnumerable<ISequence> contigs = BuildContigs();
            long contigsBuiltGraphCount = Graph.NodeCount;
            long contigsBuilt = Graph.GetNodes().Select(n => n.ExtensionsCount).Sum();

            // Compare the two graphs
            Assert.AreEqual(1, contigs.Count());
            string s = new string(contigs.ElementAt(0).Select(a => (char)a).ToArray());
            Assert.AreEqual("ATGCCTCCTATCTTAGCGATGCGGTGT", s);
            Assert.AreEqual(graphCount, contigsBuiltGraphCount);
            Assert.AreEqual(graphEdges, contigsBuilt);
        }
    }
}
