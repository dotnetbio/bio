using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Bio.Properties;

namespace Bio
{
    /// <summary>
    /// Ambiguous characters in the Protein.
    /// </summary>
    public class AmbiguousProteinAlphabet : ProteinAlphabet
    {
        /// <summary>
        /// New instance of Ambiguous character.
        /// </summary>
        public static readonly new AmbiguousProteinAlphabet Instance;

        /// <summary>
        /// Initializes static members of the AmbiguousProteinAlphabet class.
        /// </summary>
        static AmbiguousProteinAlphabet()
        {
            Instance = new AmbiguousProteinAlphabet();
        }

        /// <summary>
        /// Initializes a new instance of the AmbiguousProteinAlphabet class.
        /// </summary>
        protected AmbiguousProteinAlphabet()
        {
            Name = Resource.AmbiguousProteinAlphabetName;
            HasAmbiguity = true;

            this.X = (byte)'X';
            this.Z = (byte)'Z';
            this.B = (byte)'B';
            this.J = (byte)'J';

            AddAminoAcid(this.X, "Undetermined or atypical", (byte)'x');
            AddAminoAcid(this.Z, "Glutamic or Glutamine", (byte)'z');
            AddAminoAcid(this.B, "Aspartic or Asparagine", (byte)'b');
            AddAminoAcid(this.J, "Leucine or Isoleucine", (byte)'j');

            // Map ambiguous symbols.
            MapAmbiguousAminoAcid(this.B, new byte[] { D, N });
            MapAmbiguousAminoAcid(this.Z, new byte[] { Q, E });
            MapAmbiguousAminoAcid(this.J, new byte[] { L, I });
            MapAmbiguousAminoAcid(this.X, new byte[] { A, C, D, E, F, G, H, I, K, L, M, N, O, P, Q, R, S, T, U, V, W, Y });
        }

        /// <summary>
        /// Gets X - Xxx - Undetermined or atypical.
        /// </summary>
        public byte X { get; private set; }
        
        /// <summary>
        /// Gets Z - Glx - Glutamic Acid or Glutamine.
        /// </summary>
        public byte Z { get; private set; }

        /// <summary>
        /// Gets the Aspartic Acid or Asparagine.
        /// </summary>
        public byte B { get; private set; }

        /// <summary>
        /// Gets the Leucine or Isoleucine.
        /// </summary>
        public byte J { get; private set; }

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

            // Validate that all are valid protein symbols
            HashSet<byte> validValues = GetValidSymbols();
            HashSet<byte> symbolsInUpperCase = new HashSet<byte>();

            foreach (byte symbol in symbols)
            {
                if (!validValues.Contains(symbol))
                {
                    throw new ArgumentException(string.Format(
                        CultureInfo.CurrentCulture, Resource.INVALID_SYMBOL, symbol, Name));
                }

                byte upperCaseSymbol = symbol;
                if (symbol >= 97 && symbol <= 122)
                {
                    upperCaseSymbol = (byte)(symbol - 32);
                }

                symbolsInUpperCase.Add(upperCaseSymbol);
            }

            if (symbols.Contains(this.X))
            {
                return this.X;
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
                return symbols.First();
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
                if (TryGetAmbiguousSymbol(baseSet, out returnValue))
                {
                    return returnValue;
                }
                else
                {
                    return this.X;
                }
            }
        }
    }
}
