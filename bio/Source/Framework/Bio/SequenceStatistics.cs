using System;
using System.Collections.Generic;
using System.Globalization;
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
        private Dictionary<char, int> countHash;
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

            countHash = new Dictionary<char, int>();
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

            alphabet = sequence.Alphabet;

            // Counting with an array is way faster than using a dictionary.
            int[] symbolCounts = new int[256];
            foreach (byte item in sequence)
            {
                if (item == 0)
                {
                    continue;
                }

                symbolCounts[item]++;
            }

            LoadFromIntArray(symbolCounts);
        }

        private void LoadFromIntArray(int[] symbolCounts)
        {
            if (symbolCounts.Length != 256)
            {
                throw new ArgumentOutOfRangeException("symbolCounts", "Array of symbol counts should have length of 256.");
            }

            countHash = new Dictionary<char, int>();
            totalCount = 0;

            for (int i = 0; i < symbolCounts.Length; i++)
            {
                if (symbolCounts[i] > 0)
                {
                    countHash.Add((char)i, symbolCounts[i]);

                    totalCount += symbolCounts[i];
                }
            }
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
        public int GetCount(char symbol)
        {
            int result = 0;

            symbol = char.ToUpper(symbol, CultureInfo.InvariantCulture);

            if (countHash.ContainsKey(symbol))
            {
                result = countHash[symbol];
            }

            symbol = char.ToLower(symbol, CultureInfo.InvariantCulture);

            if (countHash.ContainsKey(symbol))
            {
                result += countHash[symbol];
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
        public int GetCount(byte item)
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
            foreach (KeyValuePair<char, int> valuePair in countHash)
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
            symbol = char.ToUpper(symbol, CultureInfo.InvariantCulture);

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
            countHash[symbol]--;
            totalCount--;
        }
        #endregion
    }
}
