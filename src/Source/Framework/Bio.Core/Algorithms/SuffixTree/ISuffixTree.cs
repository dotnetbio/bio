using System.Collections.Generic;

namespace Bio.Algorithms.SuffixTree
{
    /// <summary>
    /// This interface defines the contract to be implemented by suffix tree class.
    /// </summary>
    public interface ISuffixTree
    {
        /// <summary>
        /// Gets the Name of the suffix tree.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets sequence of Suffix Tree.
        /// </summary>
        ISequence Sequence { get; }

        /// <summary>
        /// Gets or sets Minimum length of match required.
        /// </summary>
        long MinLengthOfMatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to match
        /// basic symbols only by ignoring the ambiguous symbols or not.
        /// </summary>
        bool NoAmbiguity { get; set; }

        /// <summary>
        /// Gets the matches who's length are greater than or equal to the MinLengthOfMatch.
        /// </summary>
        /// <param name="searchSequence">Query sequence.</param>
        /// <returns>Returns IEnumerable of matches.</returns>
        IEnumerable<Match> SearchMatches(ISequence searchSequence);

        /// <summary>
        /// Gets the matches unique in reference sequence whos are greater than or equal to the MinLengthOfMatch.
        /// </summary>
        /// <param name="searchSequence">Query sequence.</param>
        /// <returns>Returns IEnumerable of matches.</returns>
        IEnumerable<Match> SearchMatchesUniqueInReference(ISequence searchSequence);
    }
}
