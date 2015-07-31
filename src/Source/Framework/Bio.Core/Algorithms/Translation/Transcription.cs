using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Translation
{
    /// <summary>
    /// Provides basic nucleotide transcription across DNA and RNA sequences.
    /// Static methods in the class provide lookup for single nucleotide
    /// complements as well as creating RNA (transcription) or DNA (reverse
    /// transcription) from a DNA or RNA sequence, respectively.
    /// </summary>
    public static class Transcription
    {
        /// <summary>
        /// The DNA to RNA mapping dictionary.
        /// </summary>
        private static Dictionary<byte, byte> dnaToRna = new Dictionary<byte, byte>();

        /// <summary>
        /// The RNA to DNA mapping dictionary.
        /// </summary>
        private static Dictionary<byte, byte> rnaToDna = new Dictionary<byte, byte>();
       
        /// <summary>
        /// Returns the complement nucleotide from DNA to RNA. This also
        /// respects ambiguous characters in the DNA and RNA alphabet.
        /// </summary>
        /// <param name="dnaSource">The DNA source.</param>
        /// <returns>The complement RNA character.</returns>
        public static byte GetRnaComplement(byte dnaSource)
        {
            return dnaToRna[dnaSource];
        }

        /// <summary>
        /// Returns the complement nucleotide from RNA to DNA. This also
        /// respects ambiguous characters in the DNA and RNA alphabet.
        /// </summary>
        /// <param name="rnaSource">The RNA source.</param>
        /// <returns>The complement DNA character.</returns>
        public static byte GetDnaComplement(byte rnaSource)
        {
            return rnaToDna[rnaSource];
        }

        /// <summary>
        /// Transcribes a DNA sequence into an RNA sequence. The length
        /// of the resulting sequence will equal the length of the source
        /// sequence. Gap and ambiguous characters will also be transcribed.
        /// For example:
        /// Sequence dna = new Sequence(Alphabets.DNA, "TACCGC");
        /// Sequence rna = Transcription.Transcribe(dna);
        /// rna.ToString() would produce "AUGGCG"
        /// </summary>
        /// <param name="dnaSource">The dna source sequence to be transcribed.</param>
        /// <returns>The transcribed RNA sequence.</returns>
        public static ISequence Transcribe(ISequence dnaSource)
        {
            if (dnaSource == null)
            {
                throw new ArgumentNullException("dnaSource");
            }

            if (dnaSource.Alphabet != Alphabets.DNA && dnaSource.Alphabet != Alphabets.AmbiguousDNA)
            {
                throw new InvalidOperationException(Properties.Resource.InvalidAlphabetType);
            }

            byte[] transcribedResult = new byte[dnaSource.Count];
            long counter = 0;

            foreach (byte n in dnaSource)
            {
                transcribedResult[counter] = GetRnaComplement(n);
                counter++;
            }

            var alphabet = dnaSource.Alphabet == Alphabets.DNA ? Alphabets.RNA : Alphabets.AmbiguousRNA;
            Sequence result = new Sequence(alphabet, transcribedResult);
            result.ID = "Complement: " + dnaSource.ID;

            return result;
        }

        /// <summary>
        /// Does reverse transcription from an RNA sequence into an DNA sequence.
        /// The length of the resulting sequence will equal the length of the source
        /// sequence. Gap and ambiguous characters will also be transcribed.
        /// For example:
        /// Sequence rna = new Sequence(Alphabets.RNA, "UACCGC");
        /// Sequence dna = Transcription.ReverseTranscribe(rna);
        /// dna.ToString() would produce "ATGGCG"
        /// </summary>
        /// <param name="rnaSource">The RNA source sequence to be reverse transcribed.</param>
        /// <returns>The reverse transcribed DNA sequence.</returns>
        public static ISequence ReverseTranscribe(ISequence rnaSource)
        {
            if (rnaSource == null)
            {
                throw new ArgumentNullException("rnaSource");
            }

            if (rnaSource.Alphabet != Alphabets.RNA && rnaSource.Alphabet != Alphabets.AmbiguousRNA)
            {
                throw new InvalidOperationException(Properties.Resource.InvalidAlphabetType);
            }

            byte[] transcribedResult = new byte[rnaSource.Count];
            long counter = 0;

            foreach (byte n in rnaSource)
            {
                transcribedResult[counter] = GetDnaComplement(n);
                counter++;
            }

            var alphabet = rnaSource.Alphabet == Alphabets.RNA ? Alphabets.DNA : Alphabets.AmbiguousDNA;
            Sequence result = new Sequence(alphabet, transcribedResult);
            result.ID = "Complement: " + rnaSource.ID;
            return result;
        }

        /// <summary>
        /// Initializes the transcription dictionary.
        /// </summary>
        static Transcription()
        {
            // Fill the transcription map
            AddToTranscriptionMap(Alphabets.DNA.A, Alphabets.RNA.A);
            AddToTranscriptionMap(Alphabets.DNA.T, Alphabets.RNA.U);
            AddToTranscriptionMap(Alphabets.DNA.C, Alphabets.RNA.C);
            AddToTranscriptionMap(Alphabets.DNA.G, Alphabets.RNA.G);
            AddToTranscriptionMap(Alphabets.DNA.Gap, Alphabets.RNA.Gap);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.Any, Alphabets.AmbiguousRNA.Any);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.AC, Alphabets.AmbiguousRNA.AC);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.AT, Alphabets.AmbiguousRNA.AU);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.ACT, Alphabets.AmbiguousRNA.ACU);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.GA, Alphabets.AmbiguousRNA.GA);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.GC, Alphabets.AmbiguousRNA.GC);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.GT, Alphabets.AmbiguousRNA.GU);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.GAT, Alphabets.AmbiguousRNA.GAU);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.GCA, Alphabets.AmbiguousRNA.GCA);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.GTC, Alphabets.AmbiguousRNA.GUC);
            AddToTranscriptionMap(Alphabets.AmbiguousDNA.TC, Alphabets.AmbiguousRNA.UC);
        }

        private static void AddToTranscriptionMap(byte dnaSymbol, byte rnaSymbol)
        {
            dnaToRna.Add(dnaSymbol, rnaSymbol);
            rnaToDna.Add(rnaSymbol, dnaSymbol);

            if (char.IsLetter((char)dnaSymbol))
            {
                // Add lowecase symbols as well
                dnaSymbol = (byte)char.ToLowerInvariant((char)dnaSymbol);
                rnaSymbol = (byte)char.ToLowerInvariant((char)rnaSymbol);

                dnaToRna.Add(dnaSymbol, rnaSymbol);
                rnaToDna.Add(rnaSymbol, dnaSymbol);
            }
        }
    }
}
