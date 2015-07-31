using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Bio.Properties;

namespace Bio
{
    /// <summary>
    /// Ambiguous symbol in the DNA.
    /// </summary>
    public class AmbiguousDnaAlphabet : DnaAlphabet
    {
        /// <summary>
        /// New instance of Ambiguous symbol.
        /// </summary>
        public static readonly new AmbiguousDnaAlphabet Instance;

        /// <summary>
        /// Initializes static members of the AmbiguousDnaAlphabet class.
        /// </summary>
        static AmbiguousDnaAlphabet()
        {
            Instance = new AmbiguousDnaAlphabet();
        }

        /// <summary>
        /// Initializes a new instance of the AmbiguousDnaAlphabet class.
        /// </summary>
        protected AmbiguousDnaAlphabet()
        {
            Name = Resource.AmbiguousDnaAlphabetName;
            HasAmbiguity = true;

            this.AC = (byte)'M';
            this.GA = (byte)'R';
            this.GC = (byte)'S';
            this.AT = (byte)'W';
            this.TC = (byte)'Y';
            this.GT = (byte)'K';
            this.GCA = (byte)'V';
            this.ACT = (byte)'H';
            this.GAT = (byte)'D';
            this.GTC = (byte)'B';
            this.Any = (byte)'N';

            AddNucleotide(this.AC, "Adenine or Cytosine", (byte)'m');
            AddNucleotide(this.GA, "Guanine or Adenine", (byte)'r');
            AddNucleotide(this.GC, "Guanine or Cytosine", (byte)'s');
            AddNucleotide(this.AT, "Adenine or Thymine", (byte)'w');
            AddNucleotide(this.TC, "Thymine or Cytosine", (byte)'y');
            AddNucleotide(this.GT, "Guanine or Thymine", (byte)'k');
            AddNucleotide(this.GCA, "Guanine or Cytosine or Adenine", (byte)'v');
            AddNucleotide(this.ACT, "Adenine or Cytosine or Thymine", (byte)'h');
            AddNucleotide(this.GAT, "Guanine or Adenine or Thymine", (byte)'d');
            AddNucleotide(this.GTC, "Guanine or Thymine or Cytosine", (byte)'b');
            AddNucleotide(this.Any, "Any", (byte)'n');

            // map complements.
            MapComplementNucleotide(this.Any, this.Any);
            MapComplementNucleotide(this.AC, this.GT);
            MapComplementNucleotide(this.AT, this.AT);
            MapComplementNucleotide(this.ACT, this.GAT);
            MapComplementNucleotide(this.GA, this.TC);
            MapComplementNucleotide(this.GC, this.GC);
            MapComplementNucleotide(this.GT, this.AC);
            MapComplementNucleotide(this.GAT, this.ACT);
            MapComplementNucleotide(this.GCA, this.GTC);
            MapComplementNucleotide(this.GTC, this.GCA);
            MapComplementNucleotide(this.TC, this.GA);

            // Map ambiguous symbols.
            MapAmbiguousNucleotide(this.Any, new byte[] { A, C, G, T });
            MapAmbiguousNucleotide(this.AC, new byte[] { A, C });
            MapAmbiguousNucleotide(this.GA, new byte[] { G, A });
            MapAmbiguousNucleotide(this.GC, new byte[] { G, C });
            MapAmbiguousNucleotide(this.AT, new byte[] { A, T });
            MapAmbiguousNucleotide(this.TC, new byte[] { T, C });
            MapAmbiguousNucleotide(this.GT, new byte[] { G, T });
            MapAmbiguousNucleotide(this.GCA, new byte[] { G, C, A });
            MapAmbiguousNucleotide(this.ACT, new byte[] { A, C, T });
            MapAmbiguousNucleotide(this.GAT, new byte[] { G, A, T });
            MapAmbiguousNucleotide(this.GTC, new byte[] { G, T, C });
        }

        /// <summary>
        /// Gets Ambiguous symbol A-Adenine C-Cytosine.
        /// </summary>
        public byte AC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol G-Guanine A-Adenine.
        /// </summary>
        public byte GA { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol G-Guanine C-Cytosine.
        /// </summary>
        public byte GC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol A-Adenine T-Thymine.
        /// </summary>
        public byte AT { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol T-Thymine C-Cytosine.
        /// </summary>
        public byte TC { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol  G-Guanine T-Thymine.
        /// </summary>
        public byte GT { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol G-Guanine C-Cytosine A-Adenine.
        /// </summary>
        public byte GCA { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol A-Adenine C-Cytosine T-Thymine.
        /// </summary>
        public byte ACT { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol G-Guanine A-Adenine T-Thymine.
        /// </summary>
        public byte GAT { get; private set; }

        /// <summary>
        /// Gets Ambiguous symbol G-Guanine T-Thymine C-Cytosine.
        /// </summary>
        public byte GTC { get; private set; }

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

            HashSet<byte> symbolsInUpperCase = new HashSet<byte>();
            
            // Validate that all are valid DNA symbols
            HashSet<byte> validValues = GetValidSymbols();

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
            else if (symbolsInUpperCase.Count == 1)
            {
                return symbolsInUpperCase.First();
            }
            else
            {
                HashSet<byte> baseSet = new HashSet<byte>();
                HashSet<byte> ambiguousSymbols;

                foreach (byte n in symbolsInUpperCase)
                {
                    ambiguousSymbols = null;
                    if (TryGetBasicSymbols(n, out ambiguousSymbols))
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
                TryGetAmbiguousSymbol(baseSet, out returnValue);

                return returnValue;
            }
        }
    }
}
