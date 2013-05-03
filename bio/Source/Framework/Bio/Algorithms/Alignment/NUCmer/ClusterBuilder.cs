using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.SuffixTree;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Clustering is a process in which individual matches are grouped in larger
    /// set called Cluster. The matches in cluster are decided based on paramters 
    /// like fixed difference allowed, maximum difference allowed, minimum score
    /// and separation factor that should be satisfied.
    /// This class implements IClusterBuilder interface.
    /// </summary>
    public class ClusterBuilder : IClusterBuilder
    {
        /// <summary>
        /// Default fixed Separation
        /// </summary>
        internal const int DefaultFixedSeparation = 5;

        /// <summary>
        /// Default Maximum Separation
        /// </summary>
        internal const int DefaultMaximumSeparation = 1000;

        /// <summary>
        /// Default Minimum Output Score
        /// </summary>
        internal const int DefaultMinimumScore = 200;

        /// <summary>
        /// Default separation factor
        /// </summary>
        internal const float DefaultSeparationFactor = 0.05f;

        /// <summary>
        /// Property referring to Second sequence start in MUM
        /// </summary>
        private const string SecondSequenceStart = "SecondSequenceStart";

        /// <summary>
        /// Property referring to ID of Cluster
        /// </summary>
        private const string ClusterID = "ClusterID";

        /// <summary>
        /// This is a list of number which are used to generate the ID of cluster
        /// </summary>
        private long[] unionFind;

        /// <summary>
        /// Initializes a new instance of the ClusterBuilder class
        /// </summary>
        public ClusterBuilder()
        {
            FixedSeparation = DefaultFixedSeparation;
            MaximumSeparation = DefaultMaximumSeparation;
            MinimumScore = DefaultMinimumScore;
            SeparationFactor = DefaultSeparationFactor;
            ScoreMethod = ClusterScoreMethod.MatchLength;
        }

        /// <summary>
        /// Gets or sets maximum fixed diagonal difference
        /// </summary>
        public int FixedSeparation { get; set; }

        /// <summary>
        /// Gets or sets maximum separation between the adjacent matches in clusters
        /// </summary>
        public int MaximumSeparation { get; set; }

        /// <summary>
        /// Gets or sets minimum output score
        /// </summary>
        public int MinimumScore { get; set; }

        /// <summary>
        /// Gets or sets separation factor. Fraction equal to 
        /// (diagonal difference / match separation) where higher values
        /// increase the insertion or deletion (indel) tolerance
        /// </summary>
        public float SeparationFactor { get; set; }

        /// <summary>
        /// Gets or sets the method to use while calculating score of a cluster.
        /// Default is MatchLength.
        /// </summary>
        public ClusterScoreMethod ScoreMethod { get; set; }

        /// <summary>
        /// Get the Cluster from given inputs of matches.
        /// Steps are as follows:
        ///     1. Sort MUMs based on query sequence start.
        ///     2. Removing overlapping MUMs (in both sequences) and MUMs with same 
        ///         diagonal offset (usually adjacent)
        ///     3. Check for  separation between two MUMs
        ///     4. Check the diagonal separation
        ///     5. If MUMs passes above conditions merge them in one cluster.
        ///     6. Sort MUMs using cluster id
        ///     7. Process clusters (Joining clusters)</summary>
        /// <param name="matchExtensions">List of maximum unique matches</param>
        /// <returns>List of Cluster</returns>
        public List<Cluster> BuildClusters(List<MatchExtension> matchExtensions)
        {
            return this.BuildClusters(matchExtensions, false);
        }

        /// <summary>
        /// Get the Cluster from given inputs of matches.
        /// Steps are as follows:
        ///     1. Sort MUMs based on query sequence start (if sortedByQuerySequenceIndex is false)
        ///     2. Removing overlapping MUMs (in both sequences) and MUMs with same 
        ///         diagonal offset (usually adjacent)
        ///     3. Check for  separation between two MUMs
        ///     4. Check the diagonal separation
        ///     5. If MUMs passes above conditions merge them in one cluster.
        ///     6. Sort MUMs using cluster id
        ///     7. Process clusters (Joining clusters)</summary>
        /// <param name="matchExtensions">List of maximum unique matches</param>
        /// <param name="sortedByQuerySequenceIndex">Flag to indicate whether the match 
        /// extensions are already started by query sequence index or not.</param>
        /// <returns>List of Cluster</returns>
        public List<Cluster> BuildClusters(List<MatchExtension> matchExtensions, bool sortedByQuerySequenceIndex)
        {
            // Validate the input
            if (null == matchExtensions)
            {
                return null;
            }

            int extnCount = matchExtensions.Count;
            if (0 == extnCount)
            {
                return null;
            }

            unionFind = new long[extnCount];

            // populate unionFind
            for (int index = 0; index < extnCount; index++)
            {
                unionFind[index] = -1;
            }

            // Get the cluster and return it
            return GetClusters(matchExtensions, sortedByQuerySequenceIndex);
        }

        /// <summary>
        /// Removes the duplicate and overlapping maximal unique matches.
        /// </summary>
        /// <param name="matches">List of matches</param>
        private static void FilterMatches(List<MatchExtension> matches)
        {
            int counter1, counter2;

            for (counter1 = 0; counter1 < matches.Count; counter1++)
            {
                matches[counter1].IsGood = true;
            }

            for (counter1 = 0; counter1 < matches.Count - 1; counter1++)
            {
                long diagonalIndex, endIndex;

                matches[counter1].IsGood = false;

                diagonalIndex = matches[counter1].QuerySequenceOffset
                        - matches[counter1].ReferenceSequenceOffset;
                endIndex = matches[counter1].QuerySequenceOffset
                        + matches[counter1].Length;

                for (counter2 = counter1 + 1;
                        counter2 < matches.Count &&
                            matches[counter2].QuerySequenceOffset <= endIndex;
                        counter2++)
                {
                    long overlap;
                    long diagonalj;

                    // this is always true as the matches are sorted on QuerySequenceOffset.
                    if (matches[counter1].QuerySequenceOffset <= matches[counter2].QuerySequenceOffset)
                    {
                        diagonalj = matches[counter2].QuerySequenceOffset
                                - matches[counter2].ReferenceSequenceOffset;
                        if (diagonalIndex == diagonalj)
                        {
                            long extentj;

                            extentj = matches[counter2].Length
                                    + matches[counter2].QuerySequenceOffset
                                    - matches[counter1].QuerySequenceOffset;
                            if (extentj > matches[counter1].Length)
                            {
                                matches[counter1].Length = extentj;
                                endIndex = matches[counter1].QuerySequenceOffset
                                        + extentj;
                            }

                            // match lies on the same diagonal, this match cannot be part of
                            // any cluster

                            // remove the match and decrement the count to continue with the next item.
                            matches.RemoveAt(counter2);
                            counter2--;
                        }
                        else if (matches[counter1].ReferenceSequenceOffset == matches[counter2].ReferenceSequenceOffset)
                        {
                            // look for overlaps in second(query) sequence
                            overlap = matches[counter1].QuerySequenceOffset
                                    + matches[counter1].Length
                                    - matches[counter2].QuerySequenceOffset;

                            if (matches[counter1].Length < matches[counter2].Length)
                            {
                                if (overlap >= matches[counter1].Length / 2)
                                {
                                    // match is overlapping, this match cannot be part of 
                                    // any cluster
                                    // remove the match and decrement the count to continue with the next item.
                                    matches.RemoveAt(counter1);
                                    counter1--;
                                    break;
                                }
                            }
                            else if (matches[counter2].Length < matches[counter1].Length)
                            {
                                if (overlap >= matches[counter2].Length / 2)
                                {
                                    // match is overlapping, this match cannot be part of 
                                    // any cluster
                                    // remove the match and decrement the count to continue with the next item.
                                    matches.RemoveAt(counter2);
                                    counter2--;
                                }
                            }
                            else
                            {
                                if (overlap >= matches[counter1].Length / 2)
                                {
                                    matches[counter2].IsTentative = true;
                                    if (matches[counter1].IsTentative)
                                    {
                                        // match is overlapping, this match cannot be part of 
                                        // any cluster
                                        // remove the match and decrement the count to continue with the next item.
                                        matches.RemoveAt(counter1);
                                        counter1--;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (matches[counter1].QuerySequenceOffset == matches[counter2].QuerySequenceOffset)
                        {
                            // look for overlaps in first(reference) sequence
                            overlap = matches[counter1].ReferenceSequenceOffset
                                    + matches[counter1].Length
                                    - matches[counter2].ReferenceSequenceOffset;

                            if (matches[counter1].Length < matches[counter2].Length)
                            {
                                if (overlap >= matches[counter1].Length / 2)
                                {
                                    // match is overlapping, this match cannot be part of 
                                    // any cluster
                                    // remove the match and decrement the count to continue with the next item.
                                    matches.RemoveAt(counter1);
                                    counter1--;
                                    break;
                                }
                            }
                            else if (matches[counter2].Length < matches[counter1].Length)
                            {
                                if (overlap >= matches[counter2].Length / 2)
                                {
                                    // match is overlapping, this match cannot be part of 
                                    // any cluster
                                    // remove the match and decrement the count to continue with the next item.
                                    matches.RemoveAt(counter2);
                                    counter2--;
                                }
                            }
                            else
                            {
                                if (overlap >= matches[counter1].Length / 2)
                                {
                                    matches[counter2].IsTentative = true;
                                    if (matches[counter1].IsTentative)
                                    {
                                        // match is overlapping, this match cannot be part of 
                                        // any cluster
                                        // remove the match and decrement the count to continue with the next item.
                                        matches.RemoveAt(counter1);
                                        counter1--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sort by Cluster by specified column
        /// </summary>
        /// <param name="matches">List of matches</param>
        /// <param name="sortBy">Column to be sorted by</param>
        /// <returns>Sorted list of cluster</returns>
        private static List<MatchExtension> Sort(
                List<MatchExtension> matches,
                string sortBy)
        {
            IEnumerable<MatchExtension> sortedMatches = null;

            switch (sortBy)
            {
                case SecondSequenceStart:
                    sortedMatches = from match in matches
                                    orderby match.QuerySequenceOffset, match.ReferenceSequenceOffset
                                    select match;
                    break;

                case ClusterID:
                    sortedMatches = from match in matches
                                    orderby match.ID,
                                     match.QuerySequenceOffset,
                                     match.ReferenceSequenceOffset
                                    select match;
                    break;

                default:
                    break;
            }

            return sortedMatches.ToList();
        }

        /// <summary>
        /// Process the matches and create clusters
        /// </summary>
        /// <param name="matches">List of matches</param>
        /// <param name="sortedByQuerySequenceIndex">Flag to indicate whether the match 
        /// extensions are already started by query sequence index or not.</param>
        /// <returns>List of clusters</returns>
        private List<Cluster> GetClusters(List<MatchExtension> matches, bool sortedByQuerySequenceIndex)
        {
            long separation, firstMatchIndex, secondMatchIndex;
            int clusterSize;
            int counter1, counter2;
            List<Cluster> clusters = new List<Cluster>();

            // Sort matches by second sequence start
            if (!sortedByQuerySequenceIndex)
            {
                matches = Sort(matches, SecondSequenceStart);
            }

            // Remove overlapping and duplicate matches in cluster
            FilterMatches(matches);

            // Fnd the diagonal distance
            // If diagonal distance is less than user defined, they are clustered together
            for (counter1 = 0; counter1 < matches.Count - 1; counter1++)
            {
                long endIndex = matches[counter1].QuerySequenceOffset
                        + matches[counter1].Length;
                long diagonalIndex = matches[counter1].QuerySequenceOffset
                        - matches[counter1].ReferenceSequenceOffset;

                for (counter2 = counter1 + 1; counter2 < matches.Count; counter2++)
                {
                    long diagonalDifference;

                    separation = matches[counter2].QuerySequenceOffset - endIndex;
                    if (separation > MaximumSeparation)
                    {
                        break;
                    }

                    diagonalDifference = Math.Abs(
                            (matches[counter2].QuerySequenceOffset - matches[counter2].ReferenceSequenceOffset)
                            - diagonalIndex);
                    if (diagonalDifference <= Math.Max(FixedSeparation, SeparationFactor * separation))
                    {
                        firstMatchIndex = Find(counter1);
                        secondMatchIndex = Find(counter2);
                        if (firstMatchIndex != secondMatchIndex)
                        {
                            // add both the matches to the cluster
                            Union((int)firstMatchIndex, (int)secondMatchIndex);
                        }
                    }
                }
            }

            // Set the cluster id of each match
            for (counter1 = 0; counter1 < matches.Count; counter1++)
            {
                matches[counter1].ID = Find(counter1);
            }

            // Sort the matches by cluster id
            matches = Sort(matches, ClusterID);

            for (counter1 = 0; counter1 < matches.Count; counter1 += clusterSize)
            {
                counter2 = counter1 + 1;
                while (counter2 < matches.Count
                        && matches[counter1].ID == matches[counter2].ID)
                {
                    counter2++;
                }

                clusterSize = counter2 - counter1;
                ProcessCluster(
                        clusters,
                        matches,
                        counter1,
                        clusterSize);
            }

            return clusters;
        }

        /// <summary>
        /// Return the id of the set containing "a" in Union-Find.
        /// </summary>
        /// <param name="matchIndex">Index of the maximal unique match in UnionFind</param>
        /// <returns>Cluster id</returns>
        private long Find(int matchIndex)
        {
            long clusterId, counter1, counter2;

            if (unionFind[matchIndex] < 0)
            {
                return matchIndex;
            }

            for (clusterId = matchIndex; unionFind[clusterId] > 0; )
            {
                clusterId = unionFind[clusterId];
            }

            for (counter1 = matchIndex; unionFind[counter1] != clusterId; counter1 = counter2)
            {
                counter2 = unionFind[counter1];
                unionFind[counter1] = clusterId;
            }

            return clusterId;
        }

        /// <summary>
        /// Group the matches in Union
        /// </summary>
        /// <param name="firstMatchIndex">Id of first cluster</param>
        /// <param name="secondMatchIndex">Id of second cluster</param>
        private void Union(int firstMatchIndex, int secondMatchIndex)
        {
            if (unionFind[firstMatchIndex] < 0 && unionFind[secondMatchIndex] < 0)
            {
                if (unionFind[firstMatchIndex] < unionFind[secondMatchIndex])
                {
                    unionFind[firstMatchIndex] += unionFind[secondMatchIndex];
                    unionFind[secondMatchIndex] = firstMatchIndex;
                }
                else
                {
                    unionFind[secondMatchIndex] += unionFind[firstMatchIndex];
                    unionFind[firstMatchIndex] = secondMatchIndex;
                }
            }
        }

        /// <summary>
        /// Process the clusters
        /// </summary>
        /// <param name="clusters">List of clusters</param>
        /// <param name="matches">List of matches</param>
        /// <param name="indexToSkip">Start index upto which match extension to be ignored.</param>
        /// <param name="clusterSize">Size of cluster</param>
        private void ProcessCluster(
                List<Cluster> clusters,
                List<MatchExtension> matches,
                int indexToSkip,
                int clusterSize)
        {
            List<MatchExtension> clusterMatches;
            long total, endIndex, startIndex, score;
            int counter1, counter2, counter3, best;

            do
            {
                // remove cluster overlaps
                for (counter1 = 0; counter1 < clusterSize; counter1++)
                {
                    matches[indexToSkip + counter1].Score = matches[indexToSkip + counter1].Length;
                    matches[indexToSkip + counter1].Adjacent = 0;
                    matches[indexToSkip + counter1].From = -1;

                    for (counter2 = 0; counter2 < counter1; counter2++)
                    {
                        long cost, overlap, overlap1, overlap2;

                        overlap1 = matches[indexToSkip + counter2].ReferenceSequenceOffset
                                + matches[indexToSkip + counter2].Length
                                - matches[indexToSkip + counter1].ReferenceSequenceOffset;
                        overlap = Math.Max(0, overlap1);
                        overlap2 = matches[indexToSkip + counter2].QuerySequenceOffset
                                + matches[indexToSkip + counter2].Length -
                                matches[indexToSkip + counter1].QuerySequenceOffset;
                        overlap = Math.Max(overlap, overlap2);

                        // cost matches which are not on same diagonal
                        cost = overlap
                                + Math.Abs((matches[indexToSkip + counter1].QuerySequenceOffset - matches[indexToSkip + counter1].ReferenceSequenceOffset)
                                - (matches[indexToSkip + counter2].QuerySequenceOffset - matches[indexToSkip + counter2].ReferenceSequenceOffset));

                        if (matches[indexToSkip + counter2].Score + matches[indexToSkip + counter1].Length - cost > matches[indexToSkip + counter1].Score)
                        {
                            matches[indexToSkip + counter1].From = counter2;
                            matches[indexToSkip + counter1].Score = matches[indexToSkip + counter2].Score
                                    + matches[indexToSkip + counter1].Length
                                    - cost;
                            matches[indexToSkip + counter1].Adjacent = overlap;
                        }
                    }
                }

                // Find the match which has highest score
                best = 0;
                for (counter1 = 1; counter1 < clusterSize; counter1++)
                {
                    if (matches[indexToSkip + counter1].Score > matches[indexToSkip + best].Score)
                    {
                        best = counter1;
                    }
                }

                total = 0;
                endIndex = int.MinValue;
                startIndex = int.MaxValue;

                // TODO: remove below cast
                for (counter1 = best; counter1 >= 0; counter1 = (int)matches[indexToSkip + counter1].From)
                {
                    matches[indexToSkip + counter1].IsGood = true;
                    total += matches[indexToSkip + counter1].Length;
                    if (matches[indexToSkip + counter1].ReferenceSequenceOffset + matches[indexToSkip + counter1].Length > endIndex)
                    {
                        // Set the cluster end index
                        endIndex = matches[indexToSkip + counter1].ReferenceSequenceOffset + matches[indexToSkip + counter1].Length;
                    }

                    if (matches[indexToSkip + counter1].ReferenceSequenceOffset < startIndex)
                    {
                        // Set the cluster start index
                        startIndex = matches[indexToSkip + counter1].ReferenceSequenceOffset;
                    }
                }

                if (this.ScoreMethod == ClusterScoreMethod.MatchLength)
                {
                    score = total;
                }
                else 
                {
                    score = endIndex - startIndex;
                }

                // If the current score exceeds the minimum score
                // and the matches to cluster
                if (score >= this.MinimumScore)
                {
                    clusterMatches = new List<MatchExtension>();

                    for (counter1 = 0; counter1 < clusterSize; counter1++)
                    {
                        if (matches[indexToSkip + counter1].IsGood)
                        {
                            MatchExtension match = matches[indexToSkip + counter1];
                            if (matches[indexToSkip + counter1].Adjacent != 0)
                            {
                                match = new MatchExtension();
                                matches[indexToSkip + counter1].CopyTo(match);
                                match.ReferenceSequenceOffset += match.Adjacent;
                                match.QuerySequenceOffset += match.Adjacent;
                                match.Length -= match.Adjacent;
                            }

                            clusterMatches.Add(match);
                        }
                    }

                    // adding the cluster to list
                    if (0 < clusterMatches.Count)
                    {
                        clusters.Add(new Cluster(clusterMatches));
                    }
                }

                // Correcting the cluster indices
                for (counter1 = counter3 = 0; counter1 < clusterSize; counter1++)
                {
                    if (!matches[indexToSkip + counter1].IsGood)
                    {
                        if (counter1 != counter3)
                        {
                            matches[indexToSkip + counter3] = matches[indexToSkip + counter1];
                        }

                        counter3++;
                    }
                }

                clusterSize = counter3;
            }
            while (clusterSize > 0);
        }
    }
}
