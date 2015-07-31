using System;

namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// Class to get the MUMmer alphabet corresponding to the specified alphabet.
    /// </summary>
    public static class MummerAlphabetExtensions
    {
        /// <summary>
        /// Find corresponding mummer alphabet which supports the concatenation symbol using an existing alphabet
        /// </summary>
        /// <param name="alphabet">Existing alphabet type</param>
        /// <returns>Corresponding mummer alphabet</returns>
        public static IAlphabet GetMummerAlphabet(this IAlphabet alphabet)
        {
            if (alphabet == MummerDnaAlphabet.Instance || alphabet == MummerRnaAlphabet.Instance || alphabet == MummerProteinAlphabet.Instance)
            {
                return alphabet;
            }
            
            if (alphabet == DnaAlphabet.Instance || alphabet == AmbiguousDnaAlphabet.Instance)
            {
                return MummerDnaAlphabet.Instance;
            }
            
            if (alphabet == RnaAlphabet.Instance || alphabet == AmbiguousRnaAlphabet.Instance)
            {
                return MummerRnaAlphabet.Instance;
            }
            
            if (alphabet == ProteinAlphabet.Instance || alphabet == AmbiguousProteinAlphabet.Instance)
            {
                return MummerProteinAlphabet.Instance;
            }
            
            throw new NotSupportedException(Properties.Resource.ParserIncorrectAlphabet);
        }
    }
}
