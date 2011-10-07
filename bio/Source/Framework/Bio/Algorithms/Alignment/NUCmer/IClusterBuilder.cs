using System.Collections.Generic;
using Bio.Algorithms.SuffixTree;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Contract defined to implement class that creates clusters.
    /// Takes list of maximum unique matches as input and creates clusters
    /// </summary>
    public interface IClusterBuilder
    {
        /// <summary>
        /// Gets or sets maximum fixed diagonal difference
        /// </summary>
        int FixedSeparation { get; set; }

        /// <summary>
        /// Gets or sets maximum separation between the adjacent matches in clusters
        /// </summary>
        int MaximumSeparation { get; set; }

        /// <summary>
        /// Gets or sets minimum output score
        /// </summary>
        int MinimumScore { get; set; }

        /// <summary>
        /// Gets or sets separation factor. Fraction equal to 
        /// (diagonal difference / match separation) where higher values
        /// increase the insertion or deletion (indel) tolerance
        /// </summary>
        float SeparationFactor { get; set; }

        /// <summary>
        /// Gets or sets the method to use while calculating score of a cluster.
        /// </summary>
        ClusterScoreMethod ScoreMethod { get; set; }

        /// <summary>
        /// Build the list of clusters for given MUMs
        /// </summary>
        /// <param name="matchExtensions">List of MUMs</param>
        /// <returns>List of Cluster</returns>
        List<Cluster> BuildClusters(List<MatchExtension> matchExtensions);

        /// <summary>
        /// Build the list of clusters for given MUMs
        /// </summary>
        /// <param name="matchExtensions">List of MUMs</param>
        /// <param name="sortedByQuerySequenceIndex">Flag to indicate whether the match 
        /// extensions are already started by query sequence index or not.</param>
        /// <returns>List of Cluster</returns>
        List<Cluster> BuildClusters(List<MatchExtension> matchExtensions, bool sortedByQuerySequenceIndex);
    }

    /// <summary>
    /// Enum to indicate method to use while calculating score of a cluster.
    /// </summary>
    public enum ClusterScoreMethod
    {
        /// <summary>
        /// Use sum of length of matches in a cluster.
        /// </summary>
        MatchLength,

        /// <summary>
        /// Use Maximum(ReferenceStart + Length) - Min(RefereceStart) from matches in a cluster.
        /// </summary>
        ReferenceOffset,
    }
}
