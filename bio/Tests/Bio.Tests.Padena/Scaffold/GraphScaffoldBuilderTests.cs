using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Tests.Framework;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Builds scaffold sequence. 
    /// </summary>
    [TestClass]
    public class GraphScaffoldBuilderTests : ParallelDeNovoAssembler
    {
        static GraphScaffoldBuilderTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.tests.log");
            }
        }

        /// <summary>
        /// Test Class Scaffold Builder using 9 contigs.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void BuildScaffold()
        {
            const int kmerLength = 6;
            const int dangleThreshold = 3;
            const int redundantThreshold = 7;
            var sequences = new List<ISequence>(TestInputs.GetReadsForScaffolds());

            KmerLength = kmerLength;
            SequenceReads.Clear();
            this.SetSequenceReads(sequences);
            CreateGraph();
            
            DanglingLinksThreshold = dangleThreshold;
            DanglingLinksPurger = new DanglingLinksPurger(dangleThreshold);
            RedundantPathLengthThreshold = redundantThreshold;
            RedundantPathsPurger = new RedundantPathsPurger(redundantThreshold);
            UnDangleGraph();
            RemoveRedundancy();

            IList<ISequence> contigs = BuildContigs().ToList();
            CloneLibrary.Instance.AddLibrary("abc", 5, 20);

            using (GraphScaffoldBuilder scaffold = new GraphScaffoldBuilder())
            {
                IEnumerable<ISequence> scaffoldSeq = scaffold.BuildScaffold(sequences, contigs, this.KmerLength, 3, 0);
                HashSet<string> expected = new HashSet<string>
                {
                    "GCGCGCTAAGATAGGAGGCAT", "CGCGCG", "TTTTAAA", "TTTTAGC", "TTTTTA", "TTTTTT", "GCGCGGCGCG" 
                };
                AlignmentHelpers.CompareSequenceLists(expected, scaffoldSeq);
            }

        }
    }
}
