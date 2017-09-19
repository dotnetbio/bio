namespace Bio.IO.SAM
{
    /// <summary>
    /// SAM Dna Alphabet, Supports "=ACMGRSVTWYHKDBN." symbols.
    /// </summary>
    public class SAMDnaAlphabet : AmbiguousDnaAlphabet
    {
        /// <summary>
        /// Singleton instance of SAMDnaAlphabet.
        /// </summary>
        public static readonly new SAMDnaAlphabet Instance = new SAMDnaAlphabet();

        /// <summary>
        /// Initializes a new instance of the SAMDnaAlphabet class.
        /// </summary>
        protected SAMDnaAlphabet()
        {
            Name = "SAMDna";
            Equal = (byte)'=';
            Dot = (byte)'.';

            AddNucleotide(this.Equal, "Equal to reference");
            AddNucleotide(this.Dot, "Space holder to represent Intron");

            // map complements.
            MapComplementNucleotide(this.Dot, this.Dot);    
            MapComplementNucleotide(this.Equal, this.Equal);
        }

        /// <summary>
        /// Checks if the provided item is an ambiguous character or not
        /// Note: for '=' and '.' symbols this method returns false.
        /// </summary>
        /// <param name="item">Item to be checked</param>
        /// <returns>True if the specified item is a ambiguous</returns>
        public override bool CheckIsAmbiguous(byte item)
        {
            if (item == this.Equal || item == this.Dot)
            {
                return false;
            }

            return base.CheckIsAmbiguous(item);
        }

        /// <summary>
        /// Gets the SAM symbol "=" (Equal to reference symbol)
        /// </summary>
        public byte Equal { get; private set; }

        /// <summary>
        /// Gets symbol "." (Space holder to represent Intron).
        /// As per the Specification Version 0.1.2-draft (20090820) and 1.4-r985
        /// In a split alignment "..." represents Intron.
        /// </summary>
        public byte Dot { get; private set; }
    }
}
