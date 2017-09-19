using System;
using System.Collections.Generic;

namespace Bio.Extensions
{
    /// <summary>
    /// Alphabet extensions used to supplement the IAlphabet interface without
    /// requiring an implementation by the class.
    /// </summary>
    public static class AlphabetExtensions
    {
        /// <summary>
        /// This returns true/false if the given symbol value is considered a termination
        /// value in the alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to test</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if value is a termination symbol, false if terminations are not supported or value is not.</returns>
        public static bool CheckIsTermination(this IAlphabet alphabet, byte value)
        {
            if (alphabet == null)
                throw new ArgumentNullException("alphabet", "Alphabet must be supplied.");

            // Not supported?
            if (!alphabet.HasTerminations)
                return false;

            // Get the termination set and return true/false on match.
            HashSet<byte> symbols;
            return (alphabet.TryGetTerminationSymbols(out symbols)
                    && symbols.Contains(value));
        }

        /// <summary>
        /// This returns true/false if the given symbol value is considered a termination
        /// value in the alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to test</param>
        /// <param name="value">Value to check</param>
        /// <returns>True if value is a termination symbol, false if terminations are not supported or value is not.</returns>
        public static bool CheckIsTermination(this IAlphabet alphabet, char value)
        {
            return CheckIsTermination(alphabet, (byte) value);
        }

        /// <summary>
        /// Checks if the provided item is a gap character or not
        /// </summary>
        /// <param name="alphabet">Alphabet to test against.</param>
        /// <param name="item">Item to be checked</param>
        /// <returns>True if the specified item is a gap</returns>
        public static bool CheckIsGap(this IAlphabet alphabet, char item)
        {
            if (alphabet == null)
                throw new ArgumentNullException("alphabet", "Alphabet must be supplied.");
            
            return alphabet.CheckIsGap((byte) item);
        }

        /// <summary>
        /// Checks if the provided item is an ambiguous character or not
        /// </summary>
        /// <param name="alphabet">Alphabet to test against.</param>
        /// <param name="item">Item to be checked</param>
        /// <returns>True if the specified item is a ambiguous</returns>
        public static bool CheckIsAmbiguous(this IAlphabet alphabet, char item)
        {
            if (alphabet == null)
                throw new ArgumentNullException("alphabet", "Alphabet must be supplied.");

            return alphabet.CheckIsAmbiguous((byte)item);
        }

        /// <summary>
        /// Gets the friendly name of a given symbol.
        /// </summary>
        /// <param name="alphabet"> </param>
        /// <param name="item">Symbol to find friendly name.</param>
        /// <returns>Friendly name of the given symbol.</returns>
        public static string GetFriendlyName(this IAlphabet alphabet, char item)
        {
            if (alphabet == null)
                throw new ArgumentNullException("alphabet", "Alphabet must be supplied.");

            return alphabet.GetFriendlyName((byte)item);
        }
    }
}
