namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// Alphabet for use by MUMmer to support concatenation character '+'
    /// </summary>
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
            this.Name = "mummerProtein";

            this.ConcatenationChar = (byte)'+';

            this.AddAminoAcid(this.ConcatenationChar, "Concatenation");
        }

        /// <summary>
        /// Gets the Concatenation character
        /// </summary>
        public byte ConcatenationChar { get; private set; }
    }
}