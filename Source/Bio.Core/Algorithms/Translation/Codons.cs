using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<string, byte> CodonMap = new Dictionary<string, byte>();

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
            byte value;
            if (TryLookup(n1, n2, n3, out value))
                return value;

            throw new InvalidOperationException(
                string.Format(null, "({0},{1},{2}) not found.", n1, n2, n3));
        }

        /// <summary>
        /// Lookup an amino acid based on a triplet of nucleotides. U U U for instance
        /// will result in Phenylalanine.  If the values cannot be
        /// found in the lookup table, <c>false</c> will be returned.
        /// </summary>
        /// <param name="n1">The first character.</param>
        /// <param name="n2">The second character.</param>
        /// <param name="n3">The third character.</param>
        /// <param name="aminoAcid">Mapped RNA value</param>
        /// <returns>True/False if the value exists</returns>
        public static bool TryLookup(byte n1, byte n2, byte n3, out byte aminoAcid)
        {
            var codon = new char[3];
            codon[0] = char.ToUpperInvariant((char)n1);
            codon[1] = char.ToUpperInvariant((char)n2);
            codon[2] = char.ToUpperInvariant((char)n3);
            return CodonMap.TryGetValue(new string(codon), out aminoAcid);
        }

        /// <summary>
        /// Lookup an amino acid within a sequence starting a certain offset.
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
            byte value;
            if (TryLookup(sequence, offset, out value))
                return value;

            throw new InvalidOperationException(
                string.Format(null, "({0},{1},{2}) not found.", (char)sequence[offset], (char)sequence[offset + 1], (char)sequence[offset + 2]));
        }

        /// <summary>
        /// Tries to lookup an amino acid within a sequence starting a certain offset.
        /// </summary>
        /// <param name="sequence">The source sequence to lookup from.</param>
        /// <param name="offset">
        /// The offset within the sequence from which to look at the next three
        /// nucleotides. Note that this offset begins its count at zero. Thus
        /// looking at a sequence described by "AUGGCG" and using a offset of 0
        /// would lookup the amino acid for codon "AUG" while using a offset of 1
        /// would lookup the amino acid for codon "UGG".
        /// </param>
        /// <param name="aminoAcid">An amino acid from the protein alphabet</param>
        /// <returns><c>true</c>, if the triplet of nucleotides could
        /// be mapped, <c>false</c> otherwise</returns>
        public static bool TryLookup(ISequence sequence, int offset, out byte aminoAcid)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");
            if (offset >= sequence.Count - 2)
                throw new ArgumentException(Properties.Resource.OffsetOverflow, "offset");

            return TryLookup(sequence[offset], sequence[offset + 1], sequence[offset + 2], out aminoAcid);
        }

        /// <summary>
        /// Initializes the Codon map dictionary.
        /// </summary>
        static Codons()
        {
            // Initialize the basic codon map from mRNA codes to Amino Acids
            CodonMap.Add("UUU", Alphabets.Protein.F);
            CodonMap.Add("UUC", Alphabets.Protein.F);
            CodonMap.Add("UUA", Alphabets.Protein.L);
            CodonMap.Add("UUG", Alphabets.Protein.L);

            CodonMap.Add("UCU", Alphabets.Protein.S);
            CodonMap.Add("UCC", Alphabets.Protein.S);
            CodonMap.Add("UCA", Alphabets.Protein.S);
            CodonMap.Add("UCG", Alphabets.Protein.S);

            CodonMap.Add("UAU", Alphabets.Protein.Y);
            CodonMap.Add("UAC", Alphabets.Protein.Y);
            CodonMap.Add("UAA", Alphabets.Protein.Ter);
            CodonMap.Add("UAG", Alphabets.Protein.Ter);

            CodonMap.Add("UGU", Alphabets.Protein.C);
            CodonMap.Add("UGC", Alphabets.Protein.C);
            CodonMap.Add("UGA", Alphabets.Protein.Ter);
            CodonMap.Add("UGG", Alphabets.Protein.W);

            CodonMap.Add("CUU", Alphabets.Protein.L);
            CodonMap.Add("CUC", Alphabets.Protein.L);
            CodonMap.Add("CUA", Alphabets.Protein.L);
            CodonMap.Add("CUG", Alphabets.Protein.L);

            CodonMap.Add("CCU", Alphabets.Protein.P);
            CodonMap.Add("CCC", Alphabets.Protein.P);
            CodonMap.Add("CCA", Alphabets.Protein.P);
            CodonMap.Add("CCG", Alphabets.Protein.P);

            CodonMap.Add("CAU", Alphabets.Protein.H);
            CodonMap.Add("CAC", Alphabets.Protein.H);
            CodonMap.Add("CAA", Alphabets.Protein.Q);
            CodonMap.Add("CAG", Alphabets.Protein.Q);

            CodonMap.Add("CGU", Alphabets.Protein.R);
            CodonMap.Add("CGC", Alphabets.Protein.R);
            CodonMap.Add("CGA", Alphabets.Protein.R);
            CodonMap.Add("CGG", Alphabets.Protein.R);

            CodonMap.Add("AUU", Alphabets.Protein.I);
            CodonMap.Add("AUC", Alphabets.Protein.I);
            CodonMap.Add("AUA", Alphabets.Protein.I);
            CodonMap.Add("AUG", Alphabets.Protein.M);

            CodonMap.Add("ACU", Alphabets.Protein.T);
            CodonMap.Add("ACC", Alphabets.Protein.T);
            CodonMap.Add("ACA", Alphabets.Protein.T);
            CodonMap.Add("ACG", Alphabets.Protein.T);

            CodonMap.Add("AAU", Alphabets.Protein.N);
            CodonMap.Add("AAC", Alphabets.Protein.N);
            CodonMap.Add("AAA", Alphabets.Protein.K);
            CodonMap.Add("AAG", Alphabets.Protein.K);

            CodonMap.Add("AGU", Alphabets.Protein.S);
            CodonMap.Add("AGC", Alphabets.Protein.S);
            CodonMap.Add("AGA", Alphabets.Protein.R);
            CodonMap.Add("AGG", Alphabets.Protein.R);

            CodonMap.Add("GUU", Alphabets.Protein.V);
            CodonMap.Add("GUC", Alphabets.Protein.V);
            CodonMap.Add("GUA", Alphabets.Protein.V);
            CodonMap.Add("GUG", Alphabets.Protein.V);

            CodonMap.Add("GCU", Alphabets.Protein.A);
            CodonMap.Add("GCC", Alphabets.Protein.A);
            CodonMap.Add("GCA", Alphabets.Protein.A);
            CodonMap.Add("GCG", Alphabets.Protein.A);

            CodonMap.Add("GAU", Alphabets.Protein.D);
            CodonMap.Add("GAC", Alphabets.Protein.D);
            CodonMap.Add("GAA", Alphabets.Protein.E);
            CodonMap.Add("GAG", Alphabets.Protein.E);

            CodonMap.Add("GGU", Alphabets.Protein.G);
            CodonMap.Add("GGC", Alphabets.Protein.G);
            CodonMap.Add("GGA", Alphabets.Protein.G);
            CodonMap.Add("GGG", Alphabets.Protein.G);
            CodonMap.Add("---", Alphabets.Protein.Gap);
        }
    }
}
