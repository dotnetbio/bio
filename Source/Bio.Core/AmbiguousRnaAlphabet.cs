using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Bio.Properties;

namespace Bio
{
    /// <summary>
    /// Ambiguous symbols in the RNA.
    /// </summary>
    public class AmbiguousRnaAlphabet : RnaAlphabet
    {
        /// <summary>
        /// New instance of Ambiguous symbol.
        /// </summary>
        public static readonly new AmbiguousRnaAlphabet Instance;

        /// <summary>
        /// Initializes static members of the AmbiguousRnaAlphabet class.
        /// </summary>
        static AmbiguousRnaAlphabet()
        {
            Instance = new AmbiguousRnaAlphabet();
        }

        /// <summary>
        /// Initializes a new instance of the AmbiguousRnaAlphabet class.
        /// </summary>
        protected AmbiguousRnaAlphabet()
        {
            Name = Resource.AmbiguousRnaAlphabetName;
            HasAmbiguity = true;

            this.AC = (byte)'M';
            this.GA = (byte)'R';
            this.GC = (byte)'S';
            this.AU = (byte)'W';
            this.UC = (byte)'Y';
            this.GU = (byte)'K';
            this.GCA = (byte)'V';
            this.ACU = (byte)'H';
            this.GAU = (byte)'D';
            this.GUC = (byte)'B';
            this.Any = (byte)'N';

            AddNucleotide(this.Any, "Any", (byte)'n');
            AddNucleotide(this.AC, "Adenine or Cytosine", (byte)'m');
            AddNucleotide(this.GA, "Guanine or Adenine", (byte)'r');
            AddNucleotide(this.GC, "Guanine or Cytosine", (byte)'s');
            AddNucleotide(this.AU, "Adenine or Uracil", (byte)'w');
            AddNucleotide(this.UC, "Uracil or Cytosine", (byte)'y');
            AddNucleotide(this.GU, "Guanine or Uracil", (byte)'k');
            AddNucleotide(this.GCA, "Guanine or Cytosine or Adenine", (byte)'v');
            AddNucleotide(this.ACU, "Adenine or Cytosine or Uracil", (byte)'h');
            AddNucleotide(this.GAU, "Guanine or Adenine or Uracil", (byte)'d');
            AddNucleotide(this.GUC, "Guanine or Uracil or Cytosine", (byte)'b');

            // map complements.
            MapComplementNucleotide(this.Any, this.Any);
            MapComplementNucleotide(this.AC, this.GU);
            MapComplementNucleotide(this.AU, this.AU);
            MapComplementNucleotide(this.ACU, this.GAU);
            MapComplementNucleotide(this.GA, this.UC);
            MapComplementNucleotide(this.GC, this.GC);
            MapComplementNucleotide(this.GU, this.AC);
            MapComplementNucleotide(this.GAU, this.ACU);
            MapComplementNucleotide(this.GCA, this.GUC);
            MapComplementNucleotide(this.GUC, this.GCA);
            MapComplementNucleotide(this.UC, this.GA);

            // Map ambiguous symbols.
            MapAmbiguousNucleotide(this.Any, new byte[] { A, C, G, U });
            MapAmbiguousNucleotide(this.AC, new byte[] { A, C });
            MapAmbiguousNucleotide(this.GA, new byte[] { G, A });
            MapAmbiguousNucleotide(this.GC, new byte[] { G, C });
            MapAmbiguousNucleotide(this.AU, new byte[] { A, U });
            MapAmbiguousNucleotide(this.UC, new byte[] { U, C });
            MapAmbiguousNucleotide(this.GU, new byte[] { G, U });
            MapAmbiguousNucleotide(this.GCA, new byte[] { G, C, A });
            MapAmbiguousNucleotide(this.ACU, new byte[] { A, C, U });
            MapAmbiguousNucleotide(this.GAU, new byte[] { G, A, U });
            MapAmbiguousNucleotide(this.GUC, new byte[] { G, U, C });
        }

        /// <summary>
        /// Gets Ambiguous symbols A-Adenine C-Cytosine.
        /// </summary>
        public byte AC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols G-Guanine A-Adenine.
        /// </summary>
        public byte GA { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols G-Guanine C-Cytosine.
        /// </summary>
        public byte GC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols A-Adenine U-Uracil.
        /// </summary>
        public byte AU { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols U-Uracil C-Cytosine.
        /// </summary>
        public byte UC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols  G-Guanine U-Uracil.
        /// </summary>
        public byte GU { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols G-Guanine C-Cytosine A-Adenine.
        /// </summary>
        public byte GCA { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols A-Adenine C-Cytosine U-Uracil.
        /// </summary>
        public byte ACU { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols G-Guanine A-Adenine U-Uracil.
        /// </summary>
        public byte GAU { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbols G-Guanine U-Uracil C-Cytosine.
        /// </summary>
        public byte GUC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol Any.
        /// </summary>
        public byte Any { get; private set; }

        /// <summary>
        /// Find the consensus nucleotide for a set of nucleotides.
        /// </summary>
        /// <param name="symbols">Set of sequence items.</param>
        /// <returns>Consensus nucleotide.</returns>
        public override byte GetConsensusSymbol(HashSet<byte> symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

            if (symbols.Count == 0)
            {
                throw new ArgumentException(Resource.SymbolCountZero);
            }

            // Validate that all are valid DNA symbols
            HashSet<byte> validValues = GetValidSymbols();

            HashSet<byte> symbolsInUpperCase = new HashSet<byte>();

            foreach (byte symbol in symbols)
            {
                if (!validValues.Contains(symbol))
                {
                    throw new ArgumentException(string.Format(
                        CultureInfo.CurrentCulture, Resource.INVALID_SYMBOL, (char)symbol, Name));
                }

                byte upperCaseSymbol = symbol;
                if (symbol >= 97 && symbol <= 122)
                {
                    upperCaseSymbol = (byte)(symbol - 32);
                }

                symbolsInUpperCase.Add(upperCaseSymbol);
            }

            // Remove all gap symbols
            HashSet<byte> gapItems = null;
            this.TryGetGapSymbols(out gapItems);

            byte defaultGap = 0;
            this.TryGetDefaultGapSymbol(out defaultGap);

            symbolsInUpperCase.ExceptWith(gapItems);

            if (symbolsInUpperCase.Count == 0)
            {
                // All are gap characters, return default 'Gap'
                return defaultGap;
            }
            if (symbolsInUpperCase.Count == 1)
            {
                return symbols.First();
            }
            
            HashSet<byte> baseSet = new HashSet<byte>();
            foreach (byte n in symbolsInUpperCase)
            {
                HashSet<byte> ambiguousSymbols;
                if (this.TryGetBasicSymbols(n, out ambiguousSymbols))
                {
                    baseSet.UnionWith(ambiguousSymbols);
                }
                else
                {
                    // If not found in ambiguous map, it has to be base / unambiguous character
                    baseSet.Add(n);
                }
            }

            byte returnValue;
            this.TryGetAmbiguousSymbol(baseSet, out returnValue);

            return returnValue;
        }
    }
}
