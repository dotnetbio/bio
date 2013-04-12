using System.Collections.Generic;
using Bio.Algorithms.Assembly.Padena;
using Bio.Tests.Framework;
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
                CloneLibrary.Instance.AddLibrary("abc", 5, 20);

                PadenaAssembly result = (PadenaAssembly)assembler.Assemble(TestInputs.GetReadsForScaffolds(), true);

                HashSet<string> expectedContigs = new HashSet<string>
                {
                    "GCGCGC", "CGCGCG", "TTAGCGCG", "GCTAAGATAGGAGGCAT", "TTTTAAA", "TTTTAGC", "TTTTTA", "TTTTTT", "GCGCGGCGCG"
                };

                AlignmentHelpers.CompareSequenceLists(expectedContigs, result.ContigSequences);

                HashSet<string> expectedScaffolds = new HashSet<string>
                {
                    "GCGCGCTAAGATAGGAGGCAT", "TTTTAGC", "GCGCGGCGCG", "TTTTTA", "TTTTTT", "CGCGCG", "TTTTAAA", 
                };

                AlignmentHelpers.CompareSequenceLists(expectedScaffolds, result.Scaffolds);
            }
        }
    }
}
