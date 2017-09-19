namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// Alphabet for use by MUMmer to support concatenation character '+'
    /// </summary>
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
            this.Name = "mummerDna";

            this.ConcatenationChar = (byte)'+';

            this.AddNucleotide(this.ConcatenationChar, "Concatenation");
        }

        /// <summary>
        /// Gets the Concatenation character
        /// </summary>
        public byte ConcatenationChar { get; private set; }
    }
}