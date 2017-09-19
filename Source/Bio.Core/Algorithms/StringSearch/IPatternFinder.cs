using System.Collections.Generic;

namespace Bio.Algorithms.StringSearch
{
    /// <summary>
    /// Interface to be implemented by the class that implement string search algorithm.
    /// </summary>
    public interface IPatternFinder
    {
        /// <summary>
        /// Gets or sets the Minimum start index of the match
        /// </summary>
        int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the match is case sensitive
        /// <remarks>
        /// Note that symbols in Sequence are always Upper case.
        /// </remarks>
        /// </summary>
        bool IgnoreCase { get; set; }

        /// <summary>
        /// Find the matches for given searchStrings in sequence and returns
        /// the matched strings with indices found at.
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="searchPatterns">Strings to be searched.</param>
        /// <returns>Matches found in sequence.</returns>
        IDictionary<string, IList<int>> FindMatch(ISequence sequence, IList<string> searchPatterns);

        /// <summary>
        /// Find the matches for given searchString in sequence and returns
        /// the matched strings with indices found at.
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="searchPattern">Strings to be searched.</param>
        /// <returns>Matches found in sequence.</returns>
        IList<int> FindMatch(ISequence sequence, string searchPattern);
    }
}
