using System;

namespace Bio.Algorithms.Translation
{
    /// <summary>
    /// Provides the means of translating an RNA sequence into an Protein
    /// sequence of amino acids.
    /// </summary>
    public static class ProteinTranslation
    {
        /// <summary>
        /// Translates the RNA sequence passed in as source into a Protein
        /// sequence of amino acids. Works on the entire source sequence
        /// starting from the first triplet of nucleotides.
        /// </summary>
        /// <param name="source">The source sequence which needs to be translated.</param>
        /// <returns>The translated sequence.</returns>
        public static ISequence Translate(ISequence source)
        {
            return Translate(source, 0);
        }

        /// <summary>
        /// Translates the RNA sequence passed in as a source into a Protein
        /// sequence of amino acids. Allows the setting of a particular index
        /// into the source sequence for the start of translation.
        /// For instance if you wanted to translate all the phases of an RNA
        /// sequence you could perform the following:
        /// Sequence rnaSeq = new Sequence(Alphabets.RNA), "AUGCGCCCG");
        /// Sequence phase1 = ProteinTranslation.Translate(rnaSeq, 0);
        /// Sequence phase2 = ProteinTranslation.Translate(rnaSeq, 1);
        /// Sequence phase3 = ProteinTranslation.Translate(rnaSeq, 2);
        /// </summary>
        /// <param name="source">The source RNA sequence to translate from</param>
        /// <param name="nucleotideOffset">
        /// An offset into the source sequence from which to begin translation.
        /// Note that this offset begins counting from 0. Set this parameter to
        /// 0 to translate the entire source sequence. Set it to 1 to ignore the
        /// first nucleotide in the source sequence, etc.
        /// </param>
        /// <returns>The translated sequence.</returns>
        public static ISequence Translate(ISequence source, int nucleotideOffset)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (nucleotideOffset > source.Count || nucleotideOffset < 0)
                throw new ArgumentOutOfRangeException(Properties.Resource.OffsetInvalid, "nucleotideOffset");

            var sourceAlphabet = source.Alphabet;
            if (sourceAlphabet != Alphabets.RNA && sourceAlphabet != Alphabets.AmbiguousRNA)
                throw new InvalidOperationException(Properties.Resource.InvalidRNASequenceInput);

            long size = (source.Count - nucleotideOffset)/3;
            byte[] translatedResult = new byte[size];
            long counter = 0;

            for (int i = nucleotideOffset; i < source.Count - 2; i += 3)
            {
                byte aminoAcid;
                if (!Codons.TryLookup(source, i, out aminoAcid))
                {
                    if (source.Alphabet != Alphabets.RNA)
                    {
                        aminoAcid = AmbiguousProteinAlphabet.Instance.X;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            Properties.Resource.OnlyAmbiguousRnaCanContainAmbiguousSymbolsOnTranslation);
                    }
                }

                translatedResult[counter] = aminoAcid;
                ++counter;
            }

            var alphabet = sourceAlphabet == Alphabets.RNA ? Alphabets.Protein : Alphabets.AmbiguousProtein;
            return new Sequence(alphabet, translatedResult) {ID = "AA: " + source.ID};
        }
    }
}
