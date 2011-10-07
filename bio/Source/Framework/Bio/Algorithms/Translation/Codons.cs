using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Bio.Algorithms.Translation
{
    /// <summary>
    /// A class which stores a table of mappings from triplets of RNA nucleotides
    /// to Amino Acids. This mapping comes from the standard Axiom of Genetics
    /// triplet rule. This class provides the basic lookup functionality from the
    /// codons. The ProteinTranslation class provides methods for translating
    /// whole RNA sequences.
    /// In order to perform mapping from DNA, it is suggested that you first
    /// use the Transcription class to create the RNA sequence
    /// </summary>
    public static class Codons
    {
        /// <summary>
        /// The mapping dictionary.
        /// </summary>
        private static Dictionary<string, byte> codonMap = new Dictionary<string, byte>();

        /// <summary>
        /// Lookup an amino acid based on a triplet of nucleotides. U U U for instance
        /// will result in Phenylalanine.
        /// </summary>
        /// <param name="n1">The first character.</param>
        /// <param name="n2">The second character.</param>
        /// <param name="n3">The third character.</param>
        /// <returns>The mapped RNA.</returns>
        public static byte Lookup(byte n1, byte n2, byte n3)
        {
            StringBuilder source = new StringBuilder();
            source.Append(char.ToUpper((char)n1, CultureInfo.InvariantCulture));
            source.Append(char.ToUpper((char)n2, CultureInfo.InvariantCulture));
            source.Append(char.ToUpper((char)n3, CultureInfo.InvariantCulture));
            return codonMap[source.ToString()];
        }

        /// <summary>
        /// Lookup an amino acid within a sequence starting a certian offset.
        /// </summary>
        /// <param name="sequence">The source sequence to lookup from.</param>
        /// <param name="offset">
        /// The offset within the sequence from which to look at the next three
        /// nucleotides. Note that this offset begins its count at zero. Thus
        /// looking at a sequence described by "AUGGCG" and using a offset of 0
        /// would lookup the amino acid for codon "AUG" while using a offset of 1
        /// would lookup the amino acid for codon "UGG".
        /// </param>
        /// <returns>An amino acid from the protein alphabet</returns>
        public static byte Lookup(ISequence sequence, int offset)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (offset >= sequence.Count - 2)
            {
                throw new ArgumentException(Properties.Resource.OffsetOverflow, "offset");
            }

            return Lookup(sequence[offset], sequence[offset + 1], sequence[offset + 2]);
        }

        /// <summary>
        /// Initializes the Codon map dictionary.
        /// </summary>
        static Codons()
        {
            // Initialize the basic codon map from mRNA codes to Amino Acids
            codonMap.Add("UUU", Alphabets.Protein.F);
            codonMap.Add("UUC", Alphabets.Protein.F);
            codonMap.Add("UUA", Alphabets.Protein.L);
            codonMap.Add("UUG", Alphabets.Protein.L);

            codonMap.Add("UCU", Alphabets.Protein.S);
            codonMap.Add("UCC", Alphabets.Protein.S);
            codonMap.Add("UCA", Alphabets.Protein.S);
            codonMap.Add("UCG", Alphabets.Protein.S);

            codonMap.Add("UAU", Alphabets.Protein.Y);
            codonMap.Add("UAC", Alphabets.Protein.Y);
            codonMap.Add("UAA", Alphabets.Protein.Ter);
            codonMap.Add("UAG", Alphabets.Protein.Ter);

            codonMap.Add("UGU", Alphabets.Protein.C);
            codonMap.Add("UGC", Alphabets.Protein.C);
            codonMap.Add("UGA", Alphabets.Protein.Ter);
            codonMap.Add("UGG", Alphabets.Protein.W);

            codonMap.Add("CUU", Alphabets.Protein.L);
            codonMap.Add("CUC", Alphabets.Protein.L);
            codonMap.Add("CUA", Alphabets.Protein.L);
            codonMap.Add("CUG", Alphabets.Protein.L);

            codonMap.Add("CCU", Alphabets.Protein.P);
            codonMap.Add("CCC", Alphabets.Protein.P);
            codonMap.Add("CCA", Alphabets.Protein.P);
            codonMap.Add("CCG", Alphabets.Protein.P);

            codonMap.Add("CAU", Alphabets.Protein.H);
            codonMap.Add("CAC", Alphabets.Protein.H);
            codonMap.Add("CAA", Alphabets.Protein.Q);
            codonMap.Add("CAG", Alphabets.Protein.Q);

            codonMap.Add("CGU", Alphabets.Protein.R);
            codonMap.Add("CGC", Alphabets.Protein.R);
            codonMap.Add("CGA", Alphabets.Protein.R);
            codonMap.Add("CGG", Alphabets.Protein.R);

            codonMap.Add("AUU", Alphabets.Protein.I);
            codonMap.Add("AUC", Alphabets.Protein.I);
            codonMap.Add("AUA", Alphabets.Protein.I);
            codonMap.Add("AUG", Alphabets.Protein.M);

            codonMap.Add("ACU", Alphabets.Protein.T);
            codonMap.Add("ACC", Alphabets.Protein.T);
            codonMap.Add("ACA", Alphabets.Protein.T);
            codonMap.Add("ACG", Alphabets.Protein.T);

            codonMap.Add("AAU", Alphabets.Protein.N);
            codonMap.Add("AAC", Alphabets.Protein.N);
            codonMap.Add("AAA", Alphabets.Protein.K);
            codonMap.Add("AAG", Alphabets.Protein.K);

            codonMap.Add("AGU", Alphabets.Protein.S);
            codonMap.Add("AGC", Alphabets.Protein.S);
            codonMap.Add("AGA", Alphabets.Protein.R);
            codonMap.Add("AGG", Alphabets.Protein.R);

            codonMap.Add("GUU", Alphabets.Protein.V);
            codonMap.Add("GUC", Alphabets.Protein.V);
            codonMap.Add("GUA", Alphabets.Protein.V);
            codonMap.Add("GUG", Alphabets.Protein.V);

            codonMap.Add("GCU", Alphabets.Protein.A);
            codonMap.Add("GCC", Alphabets.Protein.A);
            codonMap.Add("GCA", Alphabets.Protein.A);
            codonMap.Add("GCG", Alphabets.Protein.A);

            codonMap.Add("GAU", Alphabets.Protein.D);
            codonMap.Add("GAC", Alphabets.Protein.D);
            codonMap.Add("GAA", Alphabets.Protein.E);
            codonMap.Add("GAG", Alphabets.Protein.E);

            codonMap.Add("GGU", Alphabets.Protein.G);
            codonMap.Add("GGC", Alphabets.Protein.G);
            codonMap.Add("GGA", Alphabets.Protein.G);
            codonMap.Add("GGG", Alphabets.Protein.G);
            codonMap.Add("---", Alphabets.Protein.Gap);
        }
    }
}
