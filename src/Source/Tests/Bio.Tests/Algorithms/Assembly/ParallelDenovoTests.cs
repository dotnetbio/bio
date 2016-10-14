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
        // Make tests deterministic
        public static Random rng = new Random (564676416);

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


        private Sequence SimulateRead (byte [] forward, byte [] reverse, int readLength)
        {
            // Simulation parameters (cumulative probabilities)
            double mismatch = 0.02;
            double insert = 0.04;
            double deletion = 0.06;

            byte [] template = rng.NextDouble () <= 0.5 ? forward : reverse;

            // Now to simulate a read
            List<byte> data = new List<byte> (readLength + 10);
            int pos = rng.Next (0, template.Length - readLength);
            while (pos < template.Length && data.Count < readLength) {
                double rv = rng.NextDouble ();
                byte cur = template [pos];
                if (rv <= mismatch) {
                    data.Add (GetMismatchedBase (cur));
                    pos++;
                } else if (rv <= insert) {
                    data.Add (GetRandomBase ());
                } else if (rv <= deletion) {
                    pos++;
                } else {
                    data.Add (cur);
                    pos++;
                }
            }
            return new Sequence (DnaAlphabet.Instance, data.ToArray (), false);
        }

        private byte GetMismatchedBase (byte bp)
        {
            byte cur = bp;
            byte next = cur;
            while (cur == next) {
                double rv = rng.NextDouble ();
                if (rv <= 0.25) next = (byte)'A';
                else if (rv <= 0.5) next = (byte)'C';
                else if (rv <= 0.75) next = (byte)'G';
                else next = (byte)'T';
            }
            return next;
        }

        private byte GetRandomBase ()
        {
            double rv = rng.NextDouble ();
            if (rv <= 0.25) return (byte)'A';
            else if (rv <= 0.5) return (byte)'C';
            else if (rv <= 0.75) return (byte)'G';
            else return (byte)'T';
        }

        private Sequence GetRandomCircularSequence (int templateLength, int readLength)
        {
            byte[] data = new byte [templateLength + readLength];
            for (int i = 0; i < templateLength; i++) {
                data [i] = GetRandomBase ();
            }
            // Now tack on the front bases to make sure it is circular
            for (int i = 0; i < readLength; i++) {
                data [templateLength + i] = data [i];
            }
            return new Sequence (DnaAlphabet.Instance, data, false);
        }

        /// <summary>
        /// Given a random string of DNA, we should be able to completely reassemble
        /// the sequence even in the presence of errors.  This generates a random
        /// sequence and verifies that we can.
        /// 
        /// </summary>
        /// <returns>The ability to assemble random sequence.</returns>
        [Test]
        [Category ("Padena")]
        public void TestAbilityToAssembleRandomCircularSequence ()
        {
            int tlength = 5000;
            int rlength = 50;
            int depth = (int)Math.Ceiling((500 * tlength) / (double) rlength);
            var template = GetRandomCircularSequence (tlength, rlength);
            var farr = template.ToArray ();
            var rarr = template.GetReverseComplementedSequence ().ToArray ();
            List<Sequence> reads = new List<Sequence> (depth);
            for (int i = 0; i < depth; i++) {
                reads.Add (SimulateRead (farr, rarr, rlength));
            }

            // Assemble without any coverage based filtering
            ParallelDeNovoAssembler asm = new ParallelDeNovoAssembler ();
            asm.KmerLength = 11;
            asm.RedundantPathLengthThreshold = rlength + 20;
            asm.DanglingLinksThreshold = rlength + 20;
            asm.AllowErosion = true;

            var contigs = asm.Assemble (reads).AssembledSequences.ToList ();
            Assert.AreEqual (1, contigs.Count);

            // Now verify we assembled it right, and to do this we have to align 
            // it in it's original orientation.
            var truth = template.ConvertToString ();
            var f_seq = (contigs.First ().GetReverseComplementedSequence () as Sequence).ConvertToString ();
            var r_seq = (contigs.First () as Sequence).ConvertToString ();
            // Note if the break occurs in the middle of this seed, we may not find it
            var seed = truth.Substring (0, asm.KmerLength);
            string contig;
            if (f_seq.Contains (seed)) {
                contig = f_seq;
            } else if (r_seq.Contains (seed)) {
                contig = r_seq;
            } else {
                // This can only happen if the break occured in the middle of the seed, which should be very rare
                f_seq = f_seq.Substring (50) + f_seq.Substring (0, 50);
                r_seq = r_seq.Substring (50) + r_seq.Substring (0, 50);
                if (f_seq.Contains (seed)) {
                    contig = f_seq;
                } else if (r_seq.Contains (seed)) {
                    contig = r_seq;
                } else {
                    throw new Exception ("Failed to find the seed string in the contig");
                }
            }

            var ind = contig.IndexOf (seed);
            var new_contig = contig.Substring (ind) + contig.Substring (0, ind);
            Assert.AreEqual (truth, new_contig);
        }       
    }
}