using System;
using System.Linq;

namespace Bio.Extensions
{
    /// <summary>
    /// Additional methods added to ISequence interface
    /// </summary>
    public static class SequenceExtensions
    {
        /// <summary>
        /// Converts the sequence to a string.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="startIndex">Start position of the sequence.</param>
        /// <param name="length">Number of symbols to return.</param>
        /// <returns>Part of the sequence in string format.</returns>
        public static string ConvertToString(this ISequence sequence, long startIndex = 0, long length = Int32.MaxValue)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence", "Sequence cannot be null.");
            if (startIndex < 0 || startIndex >= sequence.Count)
                throw new ArgumentOutOfRangeException("startIndex");

            if (length == Int32.MaxValue)
                length = sequence.Count - startIndex;

            if (length <= 0 || startIndex + length > sequence.Count)
                throw new ArgumentOutOfRangeException("length");

            return sequence is Sequence
                       ? ((Sequence) sequence).ConvertToString(startIndex, length)
                       : new string(sequence.Skip((int) startIndex)
                                        .Select(a => (char) a).ToArray());
        }
    }
}
