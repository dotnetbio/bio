using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Bio.Algorithms.StringSearch
{
    /// <summary>
    /// Implements IPatternFinder interface.
    /// This class contains the implementation of Boyer-Moore string search algorithm.
    /// Reference: http://www-igm.univ-mlv.fr/~lecroq/string/node14.html
    /// </summary>
    public class BoyerMoore : IPatternFinder
    {
        /// <summary>
        /// Wildcard character
        /// </summary>
        private const char Wildcard = '*';

        #region IPatternFinder Members
        /// <summary>
        /// Gets or sets the Minimum start index of the match
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the match is case sensitive
        /// <remarks>
        /// Note that symbols in Sequence are always Upper case.
        /// </remarks>
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Find the matches for given searchStrings in sequence and returns
        /// the matched strings with indices found at.
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="searchPatterns">Strings to be searched.</param>
        /// <returns>Matches found in sequence.</returns>
        public IDictionary<string, IList<int>> FindMatch(ISequence sequence, IList<string> searchPatterns)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (searchPatterns == null)
            {
                throw new ArgumentNullException("searchPatterns");
            }

            // Create tasks
            IList<Task<KeyValuePair<string, IList<int>>>> tasks = searchPatterns.Select(
                    searchString => Task<KeyValuePair<string, IList<int>>>.Factory.StartNew(
                            t => new KeyValuePair<string, IList<int>>(searchString, FindMatch(sequence, searchString)),
                            TaskCreationOptions.None)).ToList();

            // Wait for all the task
            Task.WaitAll(tasks.ToArray());

            IDictionary<string, IList<int>> results = new Dictionary<string, IList<int>>();
            foreach (Task<KeyValuePair<string, IList<int>>> task in tasks)
            {
                results.Add(task.Result.Key, task.Result.Value);
            }

            return results;
        }

        /// <summary>
        /// Find the matches for given searchString in sequence and returns
        /// the matched strings with indices found at.
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="searchPattern">String to be searched.</param>
        /// <returns>Matches found in sequence.</returns>
        public IList<int> FindMatch(ISequence sequence, string searchPattern)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException("searchPattern");
            }

            IList<int> result = new List<int>();
            int patternIndex = 0, patternfoundAt = 0, fullStringLength, searchStringLength, foundAt;
            bool leftAngle = (searchPattern[0] == '<');
            bool righttAngle = (searchPattern[searchPattern.Length - 1] == '>');

            if (leftAngle)
            {
                searchPattern = searchPattern.Substring(1);
            }

            if (righttAngle)
            {
                searchPattern = searchPattern.Substring(0, searchPattern.Length - 1);
            }

            if (IgnoreCase)
            {
                searchPattern = searchPattern.ToUpperInvariant();
            }

            fullStringLength = (int)sequence.Count;
            searchStringLength = searchPattern.Length;

            // Find the Good suffix shifts
            int[] goodSuffixes = GetGoodSuffixShift(searchPattern);

            // Find the Bad character shifts
            int[] badCharacters = GetBadCharacterShift(searchPattern);

            // Start search
            foundAt = 0;
            while (foundAt <= fullStringLength - searchStringLength)
            {
                string[] subPattern = searchPattern.Split(Wildcard);

                for (int index = subPattern.Length - 1; index >= 0; index--)
                {
                    string containsWildcard = subPattern[index];
                    FindMismatchIndex(sequence, containsWildcard, foundAt, out patternIndex, out patternfoundAt);

                    if (index > 0 && patternIndex > 0)
                    {
                        if ((containsWildcard[patternIndex - 1] == sequence[patternfoundAt + foundAt]))
                        {
                            do
                            {
                                --patternfoundAt;
                            }
                            while (containsWildcard[patternIndex - 1] == sequence[patternfoundAt + foundAt]
                                && (patternfoundAt + foundAt > 0));
                        }

                        // Account for '*' and repeated character.
                        patternIndex -= 2;
                    }
                }

                // if match is found
                if (patternIndex < 0)
                {
                    // Check if the string ends with correct character
                    if (righttAngle)
                    {
                        if (foundAt + searchPattern.Length == sequence.Count)
                        {
                            result.Add(foundAt);
                        }

                        break;
                    }
                    // Check if the string start with correct character
                    else if (leftAngle)
                    {
                        if (foundAt == 0)
                        {
                            result.Add(foundAt);
                        }
                    }
                    // Check if the match starts at / after StartIndex
                    else if (foundAt >= StartIndex)
                    {
                        result.Add(foundAt);
                    }

                    foundAt += goodSuffixes[0];
                }
                else
                {
                    foundAt += Math.Max(goodSuffixes[patternIndex], badCharacters[sequence[patternfoundAt + foundAt]] - searchStringLength + 1 + patternIndex);
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Find the index at which mismatch occurs
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="searchPattern">String to be searched.</param>
        /// <param name="foundAt">current Found at index.</param>
        /// <param name="patternIndex">current positon in Input sequence.</param>
        /// <param name="patternfoundAt">Current posistion in string to be searched.</param>
        private static void FindMismatchIndex(ISequence sequence,
                string searchPattern,
                int foundAt,
                out int patternIndex,
                out int patternfoundAt)
        {
            int searchStringLength = searchPattern.Length;

            // Break at the index where character do not match.
            for (patternfoundAt = patternIndex = searchStringLength - 1; patternIndex >= 0; --patternIndex, --patternfoundAt)
            {
                if (searchPattern[patternIndex] != sequence[patternfoundAt + foundAt])
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Get the good suffix heuristics. The pattern is shifted by the longest of the 
        /// two distances that are given by the bad character and the good suffix heuristics.
        /// </summary>
        /// <param name="searchString">Input string.</param>
        /// <returns>List of good suffix shift.</returns>
        private static int[] GetGoodSuffixShift(string searchString)
        {
            int rightIndex, leftIndex, length = searchString.Length;
            int[] goodSuffixes = new int[length];

            int[] suffixes = Suffixes(searchString);

            for (rightIndex = 0; rightIndex < length; ++rightIndex)
            {
                goodSuffixes[rightIndex] = length;
            }

            leftIndex = 0;
            for (rightIndex = length - 1; rightIndex >= 0; --rightIndex)
            {
                if (suffixes[rightIndex] == rightIndex + 1)
                {
                    for (; leftIndex < length - 1 - rightIndex; ++leftIndex)
                    {
                        if (goodSuffixes[leftIndex] == length)
                        {
                            goodSuffixes[leftIndex] = length - 1 - rightIndex;
                        }
                    }
                }
            }

            for (rightIndex = 0; rightIndex <= length - 2; ++rightIndex)
            {
                goodSuffixes[length - 1 - suffixes[rightIndex]] = length - 1 - rightIndex;
            }

            return goodSuffixes;
        }

        /// <summary>
        /// Get the bad character heuristics. The text symbol that causes a mismatch, 
        /// occurs somewhere else in the pattern. Then the pattern can be shifted 
        /// so that it is aligned to this text symbol.
        /// </summary>
        /// <param name="searchString">Input string.</param>
        /// <returns>List of bad character shifts.</returns>
        private static int[] GetBadCharacterShift(string searchString)
        {
            int[] badCharacters = new int[256];
            int index, length = searchString.Length;

            for (index = 0; index < badCharacters.Length; ++index)
            {
                badCharacters[index] = length;
            }

            for (index = 0; index < length - 1; ++index)
            {
                badCharacters[searchString[index]] = length - index - 1;
            }

            return badCharacters;
        }

        /// <summary>
        /// Get the good suffix from search string
        /// </summary>
        /// <param name="searchString">Input string.</param>
        /// <returns>Good suffixes</returns>
        private static int[] Suffixes(string searchString)
        {
            int length = searchString.Length;
            int[] suffixes = new int[length];
            int leftIndex = 0, rightIndex, index;

            suffixes[length - 1] = length;
            rightIndex = length - 1;
            for (index = length - 2; index >= 0; --index)
            {
                if (index > rightIndex && suffixes[index + length - 1 - leftIndex] < index - rightIndex)
                {
                    suffixes[index] = suffixes[index + length - 1 - leftIndex];
                }
                else
                {
                    if (index < rightIndex)
                    {
                        rightIndex = index;
                    }

                    leftIndex = index;
                    while (rightIndex >= 0 && searchString[rightIndex] == searchString[rightIndex + length - 1 - leftIndex])
                    {
                        --rightIndex;
                    }

                    suffixes[index] = leftIndex - rightIndex;
                }
            }

            return suffixes;
        }
    }
}
