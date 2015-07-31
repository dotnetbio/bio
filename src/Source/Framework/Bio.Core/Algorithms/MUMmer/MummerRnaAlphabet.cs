namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// Alphabet for use by MUMmer to support concatenation character '+'
    /// </summary>
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
            this.Name = "mummerRna";

            this.ConcatenationChar = (byte)'+';

            this.AddNucleotide(this.ConcatenationChar, "Concatenation");
        }

        /// <summary>
        /// Gets the Concatenation character
        /// </summary>
        public byte ConcatenationChar { get; private set; }
    }
}