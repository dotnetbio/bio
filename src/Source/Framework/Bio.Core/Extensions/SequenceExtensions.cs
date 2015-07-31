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
        /// Suffix added to indicate a reversed sequence
        /// </summary>
        public const string ReverseIdSuffix = " Reverse";

        /// <summary>
        /// Tag for a reversed sequence (metadata)
        /// </summary>
        public const string ReversedSequenceMetadataKey = @"+isReversed";

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

            // Quick check for empty sequence
            if (sequence.Count == 0)
                return string.Empty;

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

        /// <summary>
        /// This adds a key to the Metadata to indicate this is a reversed sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static void MarkAsReverseComplement(this ISequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence", "Sequence cannot be null.");

            if (!sequence.Metadata.ContainsKey(ReversedSequenceMetadataKey))
                sequence.Metadata.Add(ReversedSequenceMetadataKey, true);
            if (!sequence.ID.EndsWith(ReverseIdSuffix, StringComparison.OrdinalIgnoreCase))
                sequence.ID += ReverseIdSuffix;
        }

        /// <summary>
        /// This checks for a sequence marker to determine if the given ISequence was generated from
        /// a reverse complement.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static bool IsMarkedAsReverseComplement(this ISequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence", "Sequence cannot be null.");

            // We look for two things - if the ID ends with "Reverse" then it probably came from MUMmer or NUCmer which
            // do this when adding the reverse query sequence into the set.  Or, we look for a metadata key indicating it 
            // was reversed.
            return (sequence.ID.EndsWith(ReverseIdSuffix, StringComparison.OrdinalIgnoreCase)
                || sequence.Metadata.ContainsKey(ReversedSequenceMetadataKey));
        }
    }
}
