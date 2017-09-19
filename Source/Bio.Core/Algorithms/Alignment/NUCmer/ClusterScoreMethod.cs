namespace Bio.Algorithms.Alignment
{
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