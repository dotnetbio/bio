using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph;
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
            const int KmerLengthConst = 6;
            const int DangleThreshold = 3;
            const int RedundantThreshold = 7;
            List<ISequence> sequences = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "ATGCCTC".Select(a => (byte)a).ToArray());
            seq.ID = ">10.x1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CCTCCTAT".Select(a => (byte)a).ToArray());
            seq.ID = "1";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TCCTATC".Select(a => (byte)a).ToArray());
            seq.ID = "2";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TGCCTCCT".Select(a => (byte)a).ToArray());
            seq.ID = "3";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "ATCTTAGC".Select(a => (byte)a).ToArray());
            seq.ID = "4";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CTATCTTAG".Select(a => (byte)a).ToArray());
            seq.ID = "5";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CTTAGCG".Select(a => (byte)a).ToArray());
            seq.ID = "6";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "GCCTCCTAT".Select(a => (byte)a).ToArray());
            seq.ID = ">8.x1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TAGCGCGCTA".Select(a => (byte)a).ToArray());
            seq.ID = ">8.y1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "AGCGCGC".Select(a => (byte)a).ToArray());
            seq.ID = ">9.x1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTTT".Select(a => (byte)a).ToArray());
            seq.ID = "7";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTTAAA".Select(a => (byte)a).ToArray());
            seq.ID = "8";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TAAAAA".Select(a => (byte)a).ToArray());
            seq.ID = "9";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTAG".Select(a => (byte)a).ToArray());
            seq.ID = "10";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTAGC".Select(a => (byte)a).ToArray());
            seq.ID = "11";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "GCGCGCCGCGCG".Select(a => (byte)a).ToArray());
            seq.ID = "12";
            sequences.Add(seq);

            KmerLength = KmerLengthConst;
            SequenceReads.Clear();
            SetSequenceReads(sequences);
            CreateGraph();
            DanglingLinksThreshold = DangleThreshold;
            DanglingLinksPurger = new DanglingLinksPurger(DangleThreshold);
            RedundantPathLengthThreshold = RedundantThreshold;
            RedundantPathsPurger = new RedundantPathsPurger(RedundantThreshold);
            UnDangleGraph();
            RemoveRedundancy();

            IList<ISequence> contigs = BuildContigs().ToList();
            ReadContigMapper mapper = new ReadContigMapper();

            ReadContigMap maps = mapper.Map(contigs, sequences, KmerLengthConst);
            MatePairMapper builder = new MatePairMapper();
            CloneLibrary.Instance.AddLibrary("abc", (float)5, (float)15);
            ContigMatePairs pairedReads = builder.MapContigToMatePairs(sequences, maps);

            ContigMatePairs overlap;
            OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
            overlap = filter.FilterPairedReads(pairedReads, 0);
            DistanceCalculator dist = new DistanceCalculator(overlap);
            overlap = dist.CalculateDistance();
            ContigGraph graph = new ContigGraph();
            graph.BuildContigGraph(contigs, this.KmerLength);
            TracePath path = new TracePath();
            IList<ScaffoldPath> paths = path.FindPaths(graph, overlap, KmerLengthConst, 3);

            Assert.AreEqual(paths.Count, 3);
            Assert.AreEqual(paths.First().Count, 3);
            ScaffoldPath scaffold = paths.First();
            Assert.IsTrue(new string(graph.GetNodeSequence(scaffold[0].Key).Select(a => (char)a).ToArray()).Equals("ATGCCTCCTATCTTAGC"));
            Assert.IsTrue(new string(graph.GetNodeSequence(scaffold[1].Key).Select(a => (char)a).ToArray()).Equals("TTAGCGCG"));
            Assert.IsTrue(new string(graph.GetNodeSequence(scaffold[2].Key).Select(a => (char)a).ToArray()).Equals("GCGCGC"));
        }
    }
}
