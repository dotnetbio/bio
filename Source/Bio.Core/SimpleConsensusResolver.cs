using System;
using System.Collections.Generic;

namespace Bio
{
    /// <summary>
    /// Calculate the consensus for a list of symbols using simple frequency fraction method.
    /// Normal (non-gap) symbols are given a weight of 100. 
    /// The confidence of a symbol is the sum of weights for that symbol, 
    /// divided by the total number of symbols occurring at that position. 
    /// If symbols have confidence >= threshold, symbol corresponding 
    /// to set of these high confidence symbols is used.
    /// If no symbol meets the threshold, symbol corresponding 
    /// to set of all the symbols at that position is used.
    /// <para>
    /// For ambiguous symbols, the corresponding set of base symbols are retrieved.
    /// And for frequency calculation, each base symbol is given a weight of 
    /// (100 / number of base symbols).
    /// </para>
    /// </summary>
    public class SimpleConsensusResolver : IConsensusResolver
    {
        /// <summary>
        /// Holds the current alphabet type
        /// </summary>
        private IAlphabet alphabetType;

        /// <summary>
        /// Initializes a new instance of the SimpleConsensusResolver class.
        /// Sets user parameter threshold.
        /// </summary>
        /// <param name="threshold">Threshold Value.</param>
        public SimpleConsensusResolver(double threshold)
        {
            this.Threshold = threshold;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleConsensusResolver class.
        /// </summary>
        /// <param name="seqAlphabet">Sequence Alphabet.</param>
        /// <param name="threshold">Threshold Value.</param>
        public SimpleConsensusResolver(IAlphabet seqAlphabet, double threshold)
        {
            this.SequenceAlphabet = seqAlphabet;
            this.Threshold = threshold;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleConsensusResolver class.
        /// Sets default value for threshold.
        /// </summary>
        /// <param name="seqAlphabet">Sequence Alphabet.</param>
        public SimpleConsensusResolver(IAlphabet seqAlphabet)
        {
            this.SequenceAlphabet = seqAlphabet;
            this.Threshold = 99;
        }

        /// <summary>
        /// Gets or sets sequence alphabet
        /// </summary>
        public IAlphabet SequenceAlphabet
        {
            get
            {
                return alphabetType;
            }
            set
            {
                if (value == AmbiguousDnaAlphabet.Instance || value == AmbiguousRnaAlphabet.Instance || value == AmbiguousProteinAlphabet.Instance)
                {
                    alphabetType = value;
                }
                else if (value == DnaAlphabet.Instance || value == RnaAlphabet.Instance || value == ProteinAlphabet.Instance)
                {
                    alphabetType = Alphabets.AmbiguousAlphabetMap[value];
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Gets or sets threshold value - used when generating consensus symbol
        /// The confidence level for a position must equal or exceed Threshold for
        /// a non-gap symbol to appear in the consensus at that position.
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Gets consensus symbols for the input list, 
        /// using frequency fraction method.
        /// Refer class summary for more details.
        /// </summary>
        /// <param name="items">List of input symbols.</param>
        /// <returns>Consensus Symbol.</returns>
        public byte GetConsensus(byte[] items)
        {
            if (this.SequenceAlphabet == null)
            {
                throw new ArgumentNullException(Properties.Resource.ALPHABET_NULL);
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Length == 0)
            {
                throw new ArgumentException(Properties.Resource.LIST_EMPTY);
            }

            Dictionary<byte, double> symbolFrequency = new Dictionary<byte, double>();
            int symbolsCount = 0;

            HashSet<byte> gapSymbols = null;
            this.SequenceAlphabet.TryGetGapSymbols(out gapSymbols);

            HashSet<byte> ambiguousSymbols = this.SequenceAlphabet.GetAmbiguousSymbols();
            HashSet<byte> basicSymbols = null;

            byte defaultGap;
            this.SequenceAlphabet.TryGetDefaultGapSymbol(out defaultGap);

            foreach (byte item in items)
            {
                if (gapSymbols.Contains(item))
                {
                    // ignore gaps
                    continue;
                }

                symbolsCount++;

                if (ambiguousSymbols.Contains(item))
                {
                    this.SequenceAlphabet.TryGetBasicSymbols(item, out basicSymbols);

                    double baseProbability = 1 / (double)basicSymbols.Count;
                    foreach (byte s in basicSymbols)
                    {
                        symbolFrequency[s] =
                            (symbolFrequency.ContainsKey(s) ? symbolFrequency[s] : 0) + baseProbability;
                    }
                }
                else
                {
                    symbolFrequency[item] =
                    (symbolFrequency.ContainsKey(item) ? symbolFrequency[item] : 0) + 1;
                }
            }

            if (symbolsCount == 0)
            {
                // All symbols were gaps
                return defaultGap;
            }

            // Check which characters are above threshold
            HashSet<byte> aboveThresholdSymbols = new HashSet<byte>();

            foreach (KeyValuePair<byte, double> item in symbolFrequency)
            {
                double frequency = (item.Value * 100) / symbolsCount;
                if (frequency > this.Threshold)
                {
                    aboveThresholdSymbols.Add(item.Key);
                }
            }

            // If there are characters above threshold, consider those characters for consensus
            // Else, consider all characters 
            return this.SequenceAlphabet.GetConsensusSymbol(aboveThresholdSymbols.Count > 0 
                ? aboveThresholdSymbols 
                : new HashSet<byte>(symbolFrequency.Keys));
        }
    }
}
