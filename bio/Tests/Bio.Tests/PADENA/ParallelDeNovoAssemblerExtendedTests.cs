using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Assembly.Padena;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Builds scaffold sequence. 
    /// </summary>
    [TestClass]
    public class ParallelDeNovoAssemblerWithScaffoldBuilders
    {
        static ParallelDeNovoAssemblerWithScaffoldBuilders()
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
        public void AssemblerTestWithScaffoldBuilder()
        {
            const int kmerLength = 6;
            const int dangleThreshold = 3;
            const int redundantThreshold = 7;

            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.KmerLength = kmerLength;
                assembler.DanglingLinksThreshold = dangleThreshold;
                assembler.RedundantPathLengthThreshold = redundantThreshold;

                assembler.ScaffoldRedundancy = 0;
                assembler.Depth = 3;
                CloneLibrary.Instance.AddLibrary("abc", (float)5, (float)20);

                PadenaAssembly result = (PadenaAssembly)assembler.Assemble(TestInputs.GetReadsForScaffolds(), true);

                Assert.AreEqual(10, result.ContigSequences.Count());

                HashSet<string> expectedContigs = new HashSet<string>
            {
               "GCGCGC",
               "TTTTTT",
               "TTTTTA",
               "TTTTAA",
               "TTTAAA",
               "ATGCCTCCTATCTTAGC",
               "TTTTAGC",
               "TTAGCGCG",
               "CGCGCCGCGC",
               "CGCGCG"
            };

                foreach (ISequence contig in result.ContigSequences)
                {
                    string contigSeq = new string(contig.Select(a => (char)a).ToArray());
                    Assert.IsTrue(
                        expectedContigs.Contains(contigSeq) ||
                        expectedContigs.Contains(contigSeq.GetReverseComplement(new char[contigSeq.Length])));
                }

                Assert.AreEqual(8, result.Scaffolds.Count());
                HashSet<string> expectedScaffolds = new HashSet<string>
            {
                "ATGCCTCCTATCTTAGCGCGC",
                "TTTTTT",
                "TTTTTA",
                "TTTTAA",
                "TTTAAA",
                "CGCGCCGCGC",
                "TTTTAGC",
                "CGCGCG"
            };

                foreach (ISequence scaffold in result.Scaffolds)
                {
                    string scaffoldSeq = new string(scaffold.Select(a => (char)a).ToArray());
                    Assert.IsTrue(
                        expectedScaffolds.Contains(scaffoldSeq) ||
                        expectedScaffolds.Contains(scaffoldSeq.GetReverseComplement(new char[scaffoldSeq.Length])));
                }
            }
        }
    }
}
