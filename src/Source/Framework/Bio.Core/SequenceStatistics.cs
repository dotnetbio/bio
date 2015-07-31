using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Bio
{
    /// <summary>
    /// SequenceStatistics is used to keep track of the number of occurrences of each symbol within
    /// a sequence.
    /// </summary>
    public class SequenceStatistics
    {
        #region Fields

        private IAlphabet alphabet;
        private Dictionary<char, long> countHash;
        private double totalCount; // double so we don't need to cast when dividing

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs sequence statistics with alphabet and 0 counts.
        /// </summary>
        /// <param name="alphabet">The alphabet for the sequence.</param>
        internal SequenceStatistics(IAlphabet alphabet)
        {
            this.alphabet = alphabet;
            countHash = new Dictionary<char, long>();
            totalCount = 0;
        }

        /// <summary>
        /// Constructs sequence statistics by iterating through a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to construct statistics for.</param>
        public SequenceStatistics(ISequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            this.alphabet = sequence.Alphabet;

            // Counting with an array is way faster than using a dictionary.
            long[] symbolCounts = new long[256];
            foreach (byte item in sequence)
            {
                if (item == 0)
                {
                    continue;
                }

                symbolCounts[item]++;
            }

            LoadFromLongArray(symbolCounts);
        }

        /// <summary>
        /// This method takes an array of symbol counts and loads our dictionary.
        /// It collapses upper/lower case differences.
        /// </summary>
        /// <param name="symbolCounts"></param>
        private void LoadFromLongArray(long[] symbolCounts)
        {
            if (symbolCounts.Length != 256)
            {
                throw new ArgumentOutOfRangeException("symbolCounts", "Array of symbol counts should have length of 256.");
            }

            countHash = new Dictionary<char, long>();
            totalCount = 0;

            for (int i = 0; i < symbolCounts.Length; i++)
            {
                if (symbolCounts[i] > 0)
                {
                    char value = char.ToUpper((char)i);
                    
                    if (countHash.ContainsKey(value))
                        countHash[value] += symbolCounts[i];
                    else
                        countHash.Add(value, symbolCounts[i]);

                    totalCount += symbolCounts[i];
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The total number of elements counted in this statistics set
        /// </summary>
        public long TotalCount
        {
            get { return (long) totalCount; }
        }

        /// <summary>
        /// The alphabet used for the values in this statistics set
        /// </summary>
        public IAlphabet Alphabet
        {
            get { return alphabet; }
        }

        /// <summary>
        /// The set of values counted (so it can be enumerated easily)
        /// </summary>
        public IEnumerable<Tuple<char,long>> SymbolCounts
        {
            get { return countHash.Select(kvp => Tuple.Create(kvp.Key, kvp.Value)); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the number of occurrences of a specific symbol.  This method does not perform
        /// any calculations to group counts of ambiguous symbols with corresponding unambiguous
        /// symbols.  So the minimum G-C content of a DNA sequence would be calculated as
        /// 
        ///     GetCount('G') + GetCount('C') + GetCount('S')
        /// </summary>
        /// <param name="symbol">The char representation of a symbol.</param>
        /// <returns>The number of occurrences of the given symbol.</returns>
        public long GetCount(char symbol)
        {
            long result = 0;

            symbol = char.ToUpper(symbol);
            if (countHash.ContainsKey(symbol))
            {
                result = countHash[symbol];
            }

            return result;
        }

        /// <summary>
        /// Gets the number of occurrences of the specific sequence char.  This method does not perform
        /// any calculations to group counts of ambiguous symbols with corresponding unambiguous
        /// symbols.  So the minimum G-C content of a DNA sequence would be calculated as
        /// 
        ///     GetCount('G') + GetCount('C') + GetCount('S')
        /// </summary>
        /// <param name="item">A byte of sequence.</param>
        /// <returns>The number of occurrences of the given a byte of sequence.</returns>
        public long GetCount(byte item)
        {
            if (item == byte.MinValue)
            {
                throw new ArgumentNullException("item");
            }

            return GetCount((char)item);
        }

        /// <summary>
        /// Gets the fraction of occurrences of a specific symbol.  This method does not perform
        /// any calculations to group counts of ambiguous symbols with corresponding unambiguous
        /// symbols.  So the minimum G-C content of a DNA sequence would be calculated as
        /// 
        ///     GetFraction('G') + GetFraction('C') + GetFraction('S')
        /// </summary>
        /// <param name="symbol">The char representation of a symbol.</param>
        /// <returns>The fraction of occurrences of the given symbol.</returns>
        public double GetFraction(char symbol)
        {
            return GetCount(symbol) / totalCount;
        }

        /// <summary>
        /// Gets the fraction of occurrences of a specific sequence char.  This method does not perform
        /// any calculations to group counts of ambiguous symbols with corresponding unambiguous
        /// symbols.  So the minimum G-C content of a DNA sequence would be calculated as
        /// 
        ///     GetFraction('G') + GetFraction('C') + GetFraction('S')
        /// </summary>
        /// <param name="item">A sequence char.</param>
        /// <returns>The fraction of occurrences of the given sequence char.</returns>
        public double GetFraction(byte item)
        {
            return GetCount(item) / totalCount;
        }

        /// <summary>
        /// Converts the sequence chars with its count to string.
        /// </summary>
        /// <returns>Sequence chars with its count.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<char, long> valuePair in countHash)
            {
                builder.AppendLine(string.Format(CultureInfo.CurrentCulture, Properties.Resource.SequenceStatisticsToStringFormat, valuePair.Key, valuePair.Value));
            }
            return builder.ToString();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Increments the count of the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to add.</param>
        internal void Add(char symbol)
        {
            symbol = char.ToUpper(symbol);

            if (countHash.ContainsKey(symbol))
            {
                countHash[symbol]++;
            }
            else
            {
                countHash.Add(symbol, 1);
            }

            totalCount++;
        }


        /// <summary>
        /// Decrements the count of the given char.
        /// </summary>
        /// <param name="symbol">The char to remove.</param>
        internal void Remove(char symbol)
        {
            if (--countHash[symbol] == 0)
                countHash.Remove(symbol);
            
            totalCount--;
        }
        #endregion
    }
}
