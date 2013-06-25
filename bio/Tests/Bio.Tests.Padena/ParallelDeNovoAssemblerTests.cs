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
        /// reverse complement of itself.  This is an interesting case because it forms a loop, but the path out of the node 
        /// is not the same as before.  In deciding how to handle this, one of the two test cases must be incorrectly assembled, as 
        /// occurs in this test with the current simple path algorithm.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void PalindromicAssembleTest()
        {
            const int KmerLength = 19;
            string testSeq = @"TTTTTTCAATTGAAAAAAATCTGTATT";
            string testSeq2 = "T" + testSeq;
            var testSequence = new Sequence(DnaAlphabet.Instance, testSeq);
            var testSequence2 = new Sequence(DnaAlphabet.Instance, testSeq2);
            List<ISequence> seqs = new List<ISequence>();
            
            //two test sequences that are different but assemble to the same sequence
            //only one of these can be done correctly in current algorithmic setup
            //using simple paths, that must be the first one.
            foreach (var curTestSeq in new[] { testSequence, testSequence2 })
            {
                seqs.Clear();
                seqs.Add(curTestSeq);
                using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
                {
                    assembler.KmerLength = KmerLength;
                    assembler.AllowErosion = false;
                    assembler.AllowLowCoverageContigRemoval = false;
                    assembler.ContigCoverageThreshold = 0;
                    assembler.DanglingLinksThreshold = 0;

                    IDeNovoAssembly result = assembler.Assemble(seqs);
                    // Compare the two graphs, ensure that an additional base is not added (which might be inco
                    Assert.AreEqual(1, result.AssembledSequences.Count);
                    bool correctContig = result.AssembledSequences[0].SequenceEqual(testSequence);
                    if (!correctContig)
                        correctContig = result.AssembledSequences[0].GetReverseComplementedSequence().SequenceEqual(testSequence);
                    Assert.IsTrue(correctContig);
                }
            }
            
        }
    }
}
