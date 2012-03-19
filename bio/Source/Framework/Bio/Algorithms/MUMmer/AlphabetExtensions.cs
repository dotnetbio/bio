using System;
using System.ComponentModel.Composition;

namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// Class to get the MUMmer alphabet corresponding to the specified alphabet.
    /// </summary>
    public static class AlphabetExtensions
    {
        /// <summary>
        /// Find corresponding mummer alphabet which supports the concatenation symbol using an existing alphabet
        /// </summary>
        /// <param name="alphabet">Existing alphabet type</param>
        /// <returns>Corresponding mummer alphabet</returns>
        public static IAlphabet GetMummerAlphabet(IAlphabet alphabet)
        {
            if (alphabet == MummerDnaAlphabet.Instance || alphabet == MummerRnaAlphabet.Instance || alphabet == MummerProteinAlphabet.Instance)
            {
                return alphabet;
            }
            else if (alphabet == DnaAlphabet.Instance || alphabet == AmbiguousDnaAlphabet.Instance)
            {
                return MummerDnaAlphabet.Instance;
            }
            else if (alphabet == RnaAlphabet.Instance || alphabet == AmbiguousRnaAlphabet.Instance)
            {
                return MummerRnaAlphabet.Instance;
            }
            if (alphabet == ProteinAlphabet.Instance || alphabet == AmbiguousProteinAlphabet.Instance)
            {
                return MummerProteinAlphabet.Instance;
            }
            else
            {
                throw new NotSupportedException(Properties.Resource.ParserIncorrectAlphabet);
            }
        }
    }

    /// <summary>
    /// Alphabet for use by MUMmer to support concatenation character '+'
    /// </summary>
    [PartNotDiscoverable]
    public class MummerDnaAlphabet : AmbiguousDnaAlphabet
    {
        /// <summary>
        /// New instance of Ambiguous symbol.
        /// </summary>
        public static readonly new MummerDnaAlphabet Instance = new MummerDnaAlphabet();

        /// <summary>
        /// Initializes a new instance of the AmbiguousDnaAlphabet class.
        /// </summary>
        protected MummerDnaAlphabet()
        {
            Name = "mummerDna";

            this.ConcatenationChar = (byte)'+';

            AddNucleotide(this.ConcatenationChar, "Concatenation");
        }

        /// <summary>
        /// Gets the Concatenation character
        /// </summary>
        public byte ConcatenationChar { get; private set; }
    }

    /// <summary>
    /// Alphabet for use by MUMmer to support concatenation character '+'
    /// </summary>
    [PartNotDiscoverable]
    public class MummerRnaAlphabet : AmbiguousRnaAlphabet
    {
        /// <summary>
        /// New instance of Ambiguous symbol.
        /// </summary>
        public static readonly new MummerRnaAlphabet Instance;

        /// <summary>
        /// Initializes static members of the AmbiguousDnaAlphabet class.
        /// </summary>
        static MummerRnaAlphabet()
        {
            Instance = new MummerRnaAlphabet();
        }

        /// <summary>
        /// Initializes a new instance of the AmbiguousDnaAlphabet class.
        /// </summary>
        protected MummerRnaAlphabet()
        {
            Name = "mummerRna";

            this.ConcatenationChar = (byte)'+';

            AddNucleotide(this.ConcatenationChar, "Concatenation");
        }

        /// <summary>
        /// Gets the Concatenation character
        /// </summary>
        public byte ConcatenationChar { get; private set; }
    }

    /// <summary>
    /// Alphabet for use by MUMmer to support concatenation character '+'
    /// </summary>
    [PartNotDiscoverable]
    public class MummerProteinAlphabet : AmbiguousProteinAlphabet
    {
        /// <summary>
        /// New instance of Ambiguous symbol.
        /// </summary>
        public static readonly new MummerProteinAlphabet Instance;

        /// <summary>
        /// Initializes static members of the AmbiguousDnaAlphabet class.
        /// </summary>
        static MummerProteinAlphabet()
        {
            Instance = new MummerProteinAlphabet();
        }

        /// <summary>
        /// Initializes a new instance of the AmbiguousDnaAlphabet class.
        /// </summary>
        protected MummerProteinAlphabet()
        {
            Name = "mummerProtein";

            this.ConcatenationChar = (byte)'+';

            AddAminoAcid(this.ConcatenationChar, "Concatenation");
        }

        /// <summary>
        /// Gets the Concatenation character
        /// </summary>
        public byte ConcatenationChar { get; private set; }
    }
}
