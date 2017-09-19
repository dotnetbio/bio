using System.Collections.Generic;
using Bio.Algorithms.SuffixTree;

namespace Bio.Algorithms.MUMmer.LIS
{
    /// <summary>
    /// This interface defines contract for classes implementing
    ///  Longest increasing subsequence.
    /// </summary>
    public interface ILongestIncreasingSubsequence
    {
        /// <summary>
        /// This method will run greedy version of 
        /// longest increasing subsequence algorithm on the list of Mum.        
        /// </summary>
        /// <param name="sortedMums">List of Sorted Mums.</param>
        /// <returns>Returns the longest subsequence list of Mum.</returns>
        IList<Match> GetLongestSequence(IList<Match> sortedMums);
    }
}
