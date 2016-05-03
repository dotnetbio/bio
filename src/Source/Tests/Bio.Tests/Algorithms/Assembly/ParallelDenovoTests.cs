using System;
using System.Collections.Generic;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.Extensions;
using Bio.SimilarityMatrices;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Assembly
{
    /// <summary>
    ///     Assembly Bvt Test case implementation.
    /// </summary>
    [TestFixture]
    public class PadenaTests
    {
        
        [Test]
        [Category("Padena")]
        public void ValidateIndelsRemoved()
        {
            var seq1 = "ACGTACCCGATAGACACGATACGACAACCCTTTCACGAATCGATACGCAGTACAGATA".Select (x => (byte)x).ToArray ();
            var seq2 = "ACGTACCCGATAGACACGATACGACAACCCT-TCACGAATCGATACGCAGTACAGATA".Where(z=> z != '-').Select (x => (byte)x).ToArray ();
            List<Sequence> data = new List<Sequence> (1000);
            var s1 = new Sequence (DnaAlphabet.Instance, seq1, false);
            var s2 = new Sequence (DnaAlphabet.Instance, seq2, false);
            foreach (var i in Enumerable.Range(0, 600)) {
                data.Add (s1);
            }
            foreach (var i in Enumerable.Range(0, 400)) {
                data.Add (s2);
            }
            ParallelDeNovoAssembler asm = new ParallelDeNovoAssembler ();
            asm.KmerLength = 17;
            asm.RedundantPathLengthThreshold = 50;
            asm.ErosionThreshold = 0;
            asm.DanglingLinksThreshold = 0;
            var contigs = asm.Assemble (data).AssembledSequences.ToList();
            Assert.AreEqual (1, contigs.Count);
            var found_seq = (contigs.First ().GetReverseComplementedSequence() as Sequence).ConvertToString ();
            Assert.AreEqual (s1.ConvertToString (), found_seq);
        }

        [Test]
        [Category("Padena")]
        public void ValidateSNPsRemoved()
        {
            var seq1 = "ACGTACCCGATAGACACGATACGACAACCCTTTCACGAATCGATACGCAGTACAGATA".Select (x => (byte)x).ToArray ();
            var seq2 = "ACGTACCCGATAGACACGATACGACAACCCTATCACGAATCGATACGCAGTACAGATA".Where(z=> z != '-').Select (x => (byte)x).ToArray ();
            List<Sequence> data = new List<Sequence> (1000);
            var s1 = new Sequence (DnaAlphabet.Instance, seq1, false);
            var s2 = new Sequence (DnaAlphabet.Instance, seq2, false);
            foreach (var i in Enumerable.Range(0, 600)) {
                data.Add (s1);
            }
            foreach (var i in Enumerable.Range(0, 400)) {
                data.Add (s2);
            }
            ParallelDeNovoAssembler asm = new ParallelDeNovoAssembler ();
            asm.KmerLength = 17;
            asm.RedundantPathLengthThreshold = 50;
            asm.ErosionThreshold = 0;
            asm.DanglingLinksThreshold = 0;
            var contigs = asm.Assemble (data).AssembledSequences.ToList();
            Assert.AreEqual (1, contigs.Count);
            var found_seq = (contigs.First ().GetReverseComplementedSequence() as Sequence).ConvertToString ();
            Assert.AreEqual (s1.ConvertToString (), found_seq);
        }
       
    }
}