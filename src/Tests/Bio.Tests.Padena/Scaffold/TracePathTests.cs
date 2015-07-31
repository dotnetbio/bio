using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph;
using Bio.Extensions;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Traverse Contig Overlap graph using Breadth First Search.
    /// </summary>
    [TestClass]
    public class TracePathTests : ParallelDeNovoAssembler
    {
        /// <summary>
        /// Initializes static members of the TracePathTests class.
        /// </summary>
        static TracePathTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.tests.log");
            }
        }

        /// <summary>
        /// Traverse Graph with palindromic contig.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TracePathTestWithPalindromicContig()
        {
            const int kmerLengthConst = 5;
            const int dangleThreshold = 3;
            const int redundantThreshold = 6;

            var sequences = new List<ISequence>()
            {
                new Sequence(Alphabets.DNA, "ATGCCTC") {ID = "0"},
                new Sequence(Alphabets.DNA, "CCTCCTAT") {ID = "1"},
                new Sequence(Alphabets.DNA, "TCCTATC") {ID = "2"},
                new Sequence(Alphabets.DNA, "TGCCTCCT") {ID = "3"},
                new Sequence(Alphabets.DNA, "ATCTTAGC") {ID = "4"},
                new Sequence(Alphabets.DNA, "CTATCTTAG") {ID = "5"},
                new Sequence(Alphabets.DNA, "CTTAGCG") {ID = "6"},
                new Sequence(Alphabets.DNA, "GCCTCCTAT") {ID = "7"},
                new Sequence(Alphabets.DNA, "TAGCGCGCTA") {ID = "8"},
                new Sequence(Alphabets.DNA, "AGCGCGC") {ID = "9"},
                new Sequence(Alphabets.DNA, "TTTTTT") {ID = "10"},
                new Sequence(Alphabets.DNA, "TTTTTAAA") {ID = "11"},
                new Sequence(Alphabets.DNA, "TAAAAA") {ID = "12"},
                new Sequence(Alphabets.DNA, "TTTTAG") {ID = "13"},
                new Sequence(Alphabets.DNA, "TTTAGC") {ID = "14"},
                new Sequence(Alphabets.DNA, "GCGCGCCGCGCG") {ID = "15"},
            };

            KmerLength = kmerLengthConst;
            SequenceReads.Clear();
            
            SetSequenceReads(sequences);
            CreateGraph();
            
            DanglingLinksThreshold = dangleThreshold;
            DanglingLinksPurger = new DanglingLinksPurger(dangleThreshold);
            RedundantPathLengthThreshold = redundantThreshold;
            RedundantPathsPurger = new RedundantPathsPurger(redundantThreshold);
            
            UnDangleGraph();
            RemoveRedundancy();

            IList<ISequence> contigs = BuildContigs().ToList();
            ReadContigMapper mapper = new ReadContigMapper();

            ReadContigMap maps = mapper.Map(contigs, sequences, kmerLengthConst);
            MatePairMapper builder = new MatePairMapper();
            CloneLibrary.Instance.AddLibrary("abc", 5, 15);
            ContigMatePairs pairedReads = builder.MapContigToMatePairs(sequences, maps);

            OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
            
            ContigMatePairs overlap = filter.FilterPairedReads(pairedReads, 0);
            DistanceCalculator dist = new DistanceCalculator(overlap);
            
            overlap = dist.CalculateDistance();
            ContigGraph graph = new ContigGraph();
            graph.BuildContigGraph(contigs, this.KmerLength);
            TracePath path = new TracePath();
            IList<ScaffoldPath> paths = path.FindPaths(graph, overlap, kmerLengthConst, 3);

            Assert.AreEqual(paths.Count, 3);
            Assert.AreEqual(paths.First().Count, 3);
            ScaffoldPath scaffold = paths.First();

            Assert.AreEqual("ATGCCTCCTATCTTAGC", graph.GetNodeSequence(scaffold[0].Key).ConvertToString());
            Assert.AreEqual("TTAGCGCG", graph.GetNodeSequence(scaffold[1].Key).ConvertToString());
            Assert.AreEqual("GCGCGC", graph.GetNodeSequence(scaffold[2].Key).ConvertToString());
        }
    }
}
