using System.Collections.Generic;
using System.Linq;

namespace Bio.Tests
{
    /// <summary>
    /// Contains test inputs for steps in PaDeNA algorithm. 
    /// </summary>
    public static class TestInputs
    {
        /// <summary>
        /// Sequence Reads for unit tests
        /// </summary>
        /// <returns>List of sequences</returns>
        public static List<ISequence> GetTinyReads()
        {
            List<string> reads = new List<string>();
            reads.Add("ATGCC");
            reads.Add("TCCTA");
            reads.Add("CCTATC");
            reads.Add("TGCCTCC");
            reads.Add("CCTCCT");
            return new List<ISequence>(reads.Select(r => new Sequence(Alphabets.DNA, r.Select(a => (byte)a).ToArray())));
        }

        /// <summary>
        /// Sequence Reads for unit tests
        /// </summary>
        /// <returns>List of sequences</returns>
        public static List<ISequence> GetSmallReads()
        {
            // Sequence to assemble: GATGCCTCCTATCGATCGTCGATGC
            List<string> reads = new List<string>();
            reads.Add("GATGCCTCCTAT");
            reads.Add("CCTCCTATCGA");
            reads.Add("TCCTATCGATCGT");
            reads.Add("ATCGTCGATGC");
            reads.Add("TCCTATCGATCGTC");
            reads.Add("TGCCTCCTATCGA");
            reads.Add("TATCGATCGTCGA");
            reads.Add("TCGATCGTCGATGC");
            reads.Add("GCCTCCTATCGA");
            return new List<ISequence>(reads.Select(r => new Sequence(Alphabets.DNA, r.Select(a => (byte)a).ToArray())));
        }

        /// <summary>
        /// Sequence reads for testing dangling links purger
        /// </summary>
        /// <returns>List of sequences</returns>
        public static List<ISequence> GetDanglingReads()
        {
            // Sequence to assemble: ATCGCTAGCATCGAACGATCATTA
            List<string> reads = new List<string>();
            reads.Add("ATCGCTAGCATCG");
            reads.Add("CTAGCATCGAAC");
            reads.Add("CATCGAACGATCATT");
            reads.Add("GCTAGCATCGAAC");
            reads.Add("CGCTAGCATCGAA");
            reads.Add("ATCGAACGATGA"); // ATCGAACGATCA: SNP introduced to create dangling link
            reads.Add("CTAGCATCGAACGATC");
            reads.Add("ATCGCTAGCATCGAA");
            reads.Add("GCTAGCATCGAACGAT");
            reads.Add("AGCATCGAACGATCAT");
            return new List<ISequence>(reads.Select(r => (ISequence)new Sequence(Alphabets.DNA, r.Select(a => (byte)a).ToArray())));
        }

        /// <summary>
        /// Sequence reads for testing redundant paths purger
        /// </summary>
        /// <returns>List of sequence</returns>
        public static List<ISequence> GetRedundantPathReads()
        {
            // Sequence to assemble: ATGCCTCCTATCTTAGCGATGCGGTGT
            List<string> reads = new List<string>();
            reads.Add("ATGCCTCCTAT");
            reads.Add("CCTCCTATCTT");
            reads.Add("TCCTATCTT");
            reads.Add("TGCCTCCTATC");
            reads.Add("GCCTCCTATCTT");
            reads.Add("CTTAGCGATG");
            reads.Add("CTATCTTAGCGAT");
            reads.Add("CTATCTTAGC");
            reads.Add("GCCTCGTATCT"); // GCCTCCTATCT: SNP introduced to create bubble
            reads.Add("AGCGATGCGGTGT");
            reads.Add("TATCTTAGCGATGC");
            reads.Add("ATCTTAGCGATGC");
            reads.Add("TTAGCGATGCGG");
            return new List<ISequence>(reads.Select(r => (ISequence)new Sequence(Alphabets.DNA, r.Select(a => (byte)a).ToArray())));
        }

        /// <summary>
        /// Gets reads required for scaffolds.
        /// </summary>
        public static List<ISequence> GetReadsForScaffolds()
        {
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
            return sequences;
        }
    }
}
