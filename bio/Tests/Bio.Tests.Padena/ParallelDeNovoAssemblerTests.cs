using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Builds scaffold sequence. 
    /// </summary>
    [TestClass]
    public class ParallelDeNovoAssemblerTests
    {
        static ParallelDeNovoAssemblerTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.tests.log");
            }
        }

        /// <summary>
        /// Test Assembler method in ParallelDeNovoAssembler
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void AssemblerTest()
        {
            const int KmerLength = 11;
            const int DangleThreshold = 3;
            const int RedundantThreshold = 10;

            List<ISequence> readSeqs = TestInputs.GetDanglingReads();
            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.KmerLength = KmerLength;
                assembler.DanglingLinksThreshold = DangleThreshold;
                assembler.RedundantPathLengthThreshold = RedundantThreshold;
                IDeNovoAssembly result = assembler.Assemble(readSeqs);

                // Compare the two graphs
                Assert.AreEqual(1, result.AssembledSequences.Count());
                HashSet<string> expectedContigs = new HashSet<string>() 
            { 
                "ATCGCTAGCATCGAACGATCATT" 
            };

                foreach (ISequence contig in result.AssembledSequences)
                {
                    Assert.IsTrue(expectedContigs.Contains(new string(contig.Select(a => (char)a).ToArray())));
                }
            }
        }
        /// <summary>
        /// Tests a quasi-palindromic sequence, where an 19 bp kmer overlaps at 18 bp with the 
        /// reverse complement of itself, forming a chain, but not a loop. A bad algorithm may not be able 
        /// to reconstruct this.
        /// 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PalindromicAssembleTest()
        {
            const int KmerLength = 19;
            string testSeq = @"GGTTTTTTCAATTGAAAAAAATCTGTATT";
            var testSequence = new Sequence(DnaAlphabet.Instance, testSeq);
            List<ISequence> seqs = new List<ISequence>();
            seqs.Add(testSequence);
            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.KmerLength = KmerLength;
                assembler.AllowErosion = false;
                assembler.AllowLowCoverageContigRemoval = false;
                assembler.ContigCoverageThreshold = 0;
                assembler.DanglingLinksThreshold = 0;
                
                IDeNovoAssembly result = assembler.Assemble(seqs);

                // Compare the two graphs
                Assert.IsTrue(result.AssembledSequences.Count == 1);
                Assert.AreEqual(1, result.AssembledSequences.Count());
                bool correctContig = result.AssembledSequences[0].SequenceEqual(testSequence);
                if (!correctContig)
                    correctContig = result.AssembledSequences[0].GetReverseComplementedSequence().Equals(testSequence);
                Assert.IsTrue(correctContig);
            }
        }
    }
}
