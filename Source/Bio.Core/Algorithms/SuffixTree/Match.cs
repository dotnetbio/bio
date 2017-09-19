using System.Globalization;

namespace Bio.Algorithms.SuffixTree
{
    /// <summary>
    /// Structure to hold the match information.
    /// </summary>
    public struct Match
    {
        /// <summary>
        /// Gets or sets the length of match.
        /// </summary>
        public long Length;

        /// <summary>
        /// Gets or sets the start index of this match in reference sequence.
        /// </summary>
        public long ReferenceSequenceOffset;

        /// <summary>
        /// Gets or sets start index of this match in query sequence.
        /// </summary>
        public long QuerySequenceOffset;

        /// <summary>
        /// Converts RefStart, QueryStart, Length of Match to string.
        /// </summary>
        /// <returns>RefStart, QueryStart, Length.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Properties.Resource.MatchToStringFormat,
                              this.ReferenceSequenceOffset, this.QuerySequenceOffset, this.Length);
        }
    }
}
