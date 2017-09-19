using System;
using System.Collections.Generic;
using System.Linq;

using Bio;
using Bio.Variant;
using Bio.Algorithms.Alignment;
using Bio.Extensions;
using NUnit.Framework;

namespace Bio.Tests
{
    [TestFixture]
    [Category("VariantCalls")]
    public static class VariantCallTests
    {        
        /// <summary>
        /// Converts the Sequence to a QualitativeSequence in the alignment.
        /// </summary>
        /// <param name="aln">Aln.</param>
        /// <param name="qualScores">Qual scores.</param>
        public static void ConvertAlignedSequenceToQualSeq(IPairwiseSequenceAlignment aln, int[] qualScores) {
            var q = aln.PairwiseAlignedSequences [0].SecondSequence as Sequence;
            var qvs = new int[q.Count];
            int queryPos = 0;
            for (int i = 0; i < qvs.Length; i++) {
                if (q [i] == '-') {
                    qvs [i] = 0;
                } else {
                    qvs [i] = qualScores[queryPos++];
                }            
            }
            var qseq = new QualitativeSequence (DnaAlphabet.Instance, FastQFormatType.Sanger, q.ToArray (), qvs, false);

            aln.PairwiseAlignedSequences [0].SecondSequence = qseq;

        }

        [Test]

        public static void Test1BPInsertionCall()
        {
            string seq1seq = "ATA-CCCTT".Replace("-", String.Empty);
            string seq2seq = "ATACCCCTT";
            int[] seq2qual = new int[] { 30, 30, 30, 4, 30, 30, 30, 30, 30 };
            var refseq = new Sequence(AmbiguousDnaAlphabet.Instance, seq1seq, false);
            var query = new Sequence (AmbiguousDnaAlphabet.Instance, seq2seq, false);
            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            // Need to add in the QV Values.
            ConvertAlignedSequenceToQualSeq(aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (4, variant.QV);
            Assert.AreEqual (2, variant.StartPosition);
            Assert.AreEqual (VariantType.INDEL, variant.Type);
            var vi = variant as IndelVariant;
            Assert.AreEqual ("C", vi.InsertedOrDeletedBases);
            Assert.AreEqual ('C', vi.HomopolymerBase);
            Assert.AreEqual (3, vi.HomopolymerLengthInReference);
            Assert.AreEqual (true, vi.InHomopolymer);
            Assert.AreEqual (vi.InsertionOrDeletion, IndelType.Insertion);
        }

        [Test]
        public static void Test1BPDeletionCall()
        {
            string seq1seq = "ATACCCCTT";
            string seq2seq = "ATA-CCCTT".Replace("-", String.Empty);
            int[] seq2qual = new int[] { 30, 30, 30, 2, 30, 30, 30, 30 };
            var refseq = new Sequence(AmbiguousDnaAlphabet.Instance, seq1seq, false);
            var query = new Sequence (AmbiguousDnaAlphabet.Instance, seq2seq, false);
            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            // Need to add in the QV Values.
            ConvertAlignedSequenceToQualSeq(aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (2, variant.QV);
            Assert.AreEqual (2, variant.StartPosition);
            Assert.AreEqual (VariantType.INDEL, variant.Type);
            var vi = variant as IndelVariant;
            Assert.AreEqual ("C", vi.InsertedOrDeletedBases);
            Assert.AreEqual ('C', vi.HomopolymerBase);
            Assert.AreEqual (4, vi.HomopolymerLengthInReference);
            Assert.AreEqual (true, vi.InHomopolymer);
            Assert.AreEqual (vi.InsertionOrDeletion, IndelType.Deletion);
        }

        [Test]
        public static void TestSNPCall()
        {
            string seq1seq = "ATCCCCCTT";
            string seq2seq = "ATCCCTCTT";
            int[] seq2qual = new int[] { 30, 30, 30, 30, 5, 3, 30, 30, 30 };
            var refseq = new Sequence(DnaAlphabet.Instance, seq1seq);
            var query = new Sequence (DnaAlphabet.Instance, seq2seq);

            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            ConvertAlignedSequenceToQualSeq (aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (3, variant.QV);
            Assert.AreEqual (5, variant.StartPosition);
            Assert.AreEqual (variant.Type, VariantType.SNP);
            var vi = variant as SNPVariant;
            Assert.AreEqual (1, vi.Length);
            Assert.AreEqual ('T', vi.AltBP);
            Assert.AreEqual ('C', vi.RefBP);
            Assert.AreEqual (VariantType.SNP, vi.Type);
            Assert.AreEqual (false, vi.AtEndOfAlignment);
        } 

        [Test]
        public static void TestSNPCallAtEnd()
        {
            string seq1seq = "ATCCCCCTC";
            string seq2seq = "ATCCCCCTT";
            int[] seq2qual = new int[] { 30, 30, 30, 30, 5, 3, 30, 30, 10 };
            var refseq = new Sequence(DnaAlphabet.Instance, seq1seq);
            var query = new Sequence (DnaAlphabet.Instance, seq2seq);

            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            ConvertAlignedSequenceToQualSeq (aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (10, variant.QV);
            Assert.AreEqual (8, variant.StartPosition);
            Assert.AreEqual (variant.Type, VariantType.SNP);
            var vi = variant as SNPVariant;
            Assert.AreEqual (1, vi.Length);
            Assert.AreEqual ('T', vi.AltBP);
            Assert.AreEqual ('C', vi.RefBP);
            Assert.AreEqual (VariantType.SNP, vi.Type);
            Assert.AreEqual (true, vi.AtEndOfAlignment);
        } 

        [Test]
        public static void TestSNPCallAtStart()
        {
            string seq1seq = "CTCCCCCTT";
            string seq2seq = "TTCCCCCTT";
            int[] seq2qual = new int[] { 10, 30, 30, 30, 5, 3, 30, 30, 10 };
            var refseq = new Sequence(DnaAlphabet.Instance, seq1seq);
            var query = new Sequence (DnaAlphabet.Instance, seq2seq);

            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            ConvertAlignedSequenceToQualSeq (aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (10, variant.QV);
            Assert.AreEqual (0, variant.StartPosition);
            Assert.AreEqual (variant.Type, VariantType.SNP);
            var vi = variant as SNPVariant;
            Assert.AreEqual (1, vi.Length);
            Assert.AreEqual ('T', vi.AltBP);
            Assert.AreEqual ('C', vi.RefBP);
            Assert.AreEqual (VariantType.SNP, vi.Type);
            Assert.AreEqual (true, vi.AtEndOfAlignment);
        } 

        [Test]
        public static void TestLeftAlignmentStep() {
            var refseq =   "ACAATAAAAGCGCGCGCGCGTTACGTATAT--ATGGATAT";
            var queryseq = "ACAATAA-AGC--GCGC--GTTACGTATATATATGGATAT";

            var r = new Sequence (DnaAlphabet.Instance, refseq);
            var q = new Sequence (DnaAlphabet.Instance, queryseq);
            var aln = new PairwiseSequenceAlignment (r, q);
            var pas = new PairwiseAlignedSequence ();
            pas.FirstSequence = r;
            pas.SecondSequence = q;
            aln.Add (pas);
            var tpl = VariantCaller.LeftAlignIndelsAndCallVariants (aln, true);

            // Check the left alignment
            aln = tpl.Item1 as PairwiseSequenceAlignment;
            var lar = aln.PairwiseAlignedSequences [0].FirstSequence.ConvertToString();
            var laq = aln.PairwiseAlignedSequences [0].SecondSequence.ConvertToString();
            var exprefseq =   "ACAATAAAAGCGCGCGCGCGTTACG--TATATATGGATAT";
            var expqueryseq = "ACAAT-AAA----GCGCGCGTTACGTATATATATGGATAT";
            Assert.AreEqual (exprefseq, lar);
            Assert.AreEqual (expqueryseq, laq);

            // And it's hard, so we might as well check the variants
            var variants = tpl.Item2;
            Assert.AreEqual (3, variants.Count);
            string[] bases = new string[] { "A", "GCGC", "TA" };
            char[] hpbases = new char[] { 'A', 'G', 'T' };
            bool[] inHp = new bool[] { true, false, false };
            int[] lengths = new int[] { 1, 4, 2 };
            int[] starts = new int[] { 4, 8, 24 };
            IndelType[] types = new IndelType[] { IndelType.Deletion, IndelType.Deletion, IndelType.Insertion };
            for (int i = 0; i < 3; i++) {
                Assert.AreEqual (VariantType.INDEL, variants [i].Type);
                var vi = variants [i] as IndelVariant;
                Assert.AreEqual (hpbases[i], vi.HomopolymerBase);
                Assert.AreEqual (starts [i], vi.StartPosition);
                Assert.AreEqual (lengths [i], vi.Length);
                Assert.AreEqual (bases [i], vi.InsertedOrDeletedBases);
                Assert.AreEqual (inHp [i], vi.InHomopolymer);
                Assert.AreEqual (types [i], vi.InsertionOrDeletion);

            }
        
        }

        [Test]
        public static void TestReverseComplement1BPIndelCall() {

            string seq1seq = "ATACCCCTTGCGC";
            string seq2seq = "ATA-CCCTTGCGC".Replace("-", String.Empty);
            int[] seq2qual = new int[] { 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30 };
            var refseq = new Sequence(DnaAlphabet.Instance, seq1seq);
            var query = new Sequence (DnaAlphabet.Instance, seq2seq);

            var s1rc = refseq.GetReverseComplementedSequence ();
            var s2rc = query.GetReverseComplementedSequence ();

            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (s1rc, s2rc).First();
            VariantCallTests.ConvertAlignedSequenceToQualSeq (aln, seq2qual.Reverse ().ToArray ());
            aln.PairwiseAlignedSequences [0].Sequences [1].MarkAsReverseComplement ();
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (2, variant.QV);
            Assert.AreEqual (5, variant.StartPosition);
            Assert.AreEqual (VariantType.INDEL, variant.Type);
            var vi = variant as IndelVariant;
            Assert.AreEqual (IndelType.Deletion, vi.InsertionOrDeletion);
            Assert.AreEqual ('G', vi.HomopolymerBase);
            Assert.AreEqual (1, vi.Length);
            Assert.AreEqual (4, vi.HomopolymerLengthInReference);
            Assert.AreEqual (true, vi.InHomopolymer);
            Assert.AreEqual ("G", vi.InsertedOrDeletedBases);
            Assert.AreEqual (false, vi.AtEndOfAlignment);
            Assert.AreEqual (6, vi.EndPosition);
        }


        [Test]
        public static void TestTrickyQVInversions() {
            // This will be hard because normally flip the QV value for a homopolymer, but in this case we won't. 
            // Note the whole notion of flipping is poorly defined.
            string seq1seq = "ATTGC";
            string seq2seq = "ATAGC";
            int[] seq2qual = new int[] { 30, 30, 2, 30, 30 };
            var refseq = new Sequence(DnaAlphabet.Instance, seq1seq);
            var query = new Sequence (DnaAlphabet.Instance, seq2seq);

            var s1rc = refseq.GetReverseComplementedSequence ();
            var s2rc = query.GetReverseComplementedSequence ();

            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (s1rc, s2rc).First();
            VariantCallTests.ConvertAlignedSequenceToQualSeq (aln, seq2qual.Reverse ().ToArray ());
            aln.PairwiseAlignedSequences [0].Sequences [1].MarkAsReverseComplement ();
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (1, variants.Count);
            var variant = variants.First ();
            Assert.AreEqual (VariantType.SNP, variant.Type);
            Assert.AreEqual (2, variant.QV);

            var vs = variant as SNPVariant; 
            Assert.AreEqual ('T', vs.AltBP);
            Assert.AreEqual ('A', vs.RefBP);
        }

        [Test]
        public static void TestInsertionAtEndofHP() {
            string seq1seq = "ATA-CCC".Replace("-", String.Empty);
            string seq2seq = "ATACCCC";
            int[] seq2qual = new int[] { 30, 30, 30, 4, 30, 30, 30 };
            var refseq = new Sequence(AmbiguousDnaAlphabet.Instance, seq1seq, false);
            var query = new Sequence (AmbiguousDnaAlphabet.Instance, seq2seq, false);
            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            // Need to add in the QV Values.
            ConvertAlignedSequenceToQualSeq(aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (4, variant.QV);
            Assert.AreEqual (2, variant.StartPosition);
            Assert.AreEqual (VariantType.INDEL, variant.Type);
            var vi = variant as IndelVariant;
            Assert.AreEqual ("C", vi.InsertedOrDeletedBases);
            Assert.AreEqual ('C', vi.HomopolymerBase);
            Assert.AreEqual (true, vi.AtEndOfAlignment);
            Assert.AreEqual (3, vi.HomopolymerLengthInReference);
            Assert.AreEqual (true, vi.InHomopolymer);
            Assert.AreEqual (vi.InsertionOrDeletion, IndelType.Insertion);
        }


        [Test]
        public static void TestDeletionAtEndofHP() {
            string seq1seq = "ATACCCC";
            string seq2seq = "ATA-CCC".Replace("-", String.Empty);
            int[] seq2qual = new int[] { 30, 30, 30, 4, 30, 30 };
            var refseq = new Sequence(AmbiguousDnaAlphabet.Instance, seq1seq, false);
            var query = new Sequence (AmbiguousDnaAlphabet.Instance, seq2seq, false);
            NeedlemanWunschAligner aligner = new NeedlemanWunschAligner ();
            var aln = aligner.Align (refseq, query).First();
            // Need to add in the QV Values.
            ConvertAlignedSequenceToQualSeq(aln, seq2qual);
            var variants = VariantCaller.CallVariants (aln);
            Assert.AreEqual (variants.Count, 1);
            var variant = variants.First ();
            Assert.AreEqual (4, variant.QV);
            Assert.AreEqual (2, variant.StartPosition);
            Assert.AreEqual (VariantType.INDEL, variant.Type);
            var vi = variant as IndelVariant;
            Assert.AreEqual ("C", vi.InsertedOrDeletedBases);
            Assert.AreEqual ('C', vi.HomopolymerBase);
            Assert.AreEqual (true, vi.AtEndOfAlignment);
            Assert.AreEqual (4, vi.HomopolymerLengthInReference);
            Assert.AreEqual (true, vi.InHomopolymer);
            Assert.AreEqual (vi.InsertionOrDeletion, IndelType.Deletion);
        }

        [Test]
        public static void TestExceptionThrownForUnclippedAlignment() {
            var refseq =   "ACAATATA";
            var queryseq = "ACAATAT-";

            var r = new Sequence (DnaAlphabet.Instance, refseq);
            var q = new Sequence (DnaAlphabet.Instance, queryseq);
            var aln = new PairwiseSequenceAlignment (r, q);
            var pas = new PairwiseAlignedSequence ();
            pas.FirstSequence = r;
            pas.SecondSequence = q;
            aln.Add (pas);
            Assert.Throws<FormatException> (() => VariantCaller.LeftAlignIndelsAndCallVariants (aln, true));

            refseq =   "AAACAATATA";
            queryseq = "AA-CAATATA";

            r = new Sequence (DnaAlphabet.Instance, refseq);
            q = new Sequence (DnaAlphabet.Instance, queryseq);
            aln = new PairwiseSequenceAlignment (r, q);
            pas = new PairwiseAlignedSequence ();
            pas.FirstSequence = r;
            pas.SecondSequence = q;
            aln.Add (pas);
            Assert.Throws<FormatException> (() => VariantCaller.LeftAlignIndelsAndCallVariants (aln, true));
        }
    }
}

