namespace Bio.Web.Blast
{
    /// <summary>
    /// a High-scoring Segment Pair.
    /// </summary>
    /// <remarks>
    /// Represents an aligned section of the query and hit sequences with high similarity.
    /// </remarks>
    public class Hsp
    {
        /// <summary>
        /// The score for the pair
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Normalized form of the score
        /// </summary>
        public double BitScore { get; set; }

        /// <summary>
        /// Expectation value
        /// </summary>
        public double EValue { get; set; }

        /// <summary>
        /// The start location of the match in the query sequence
        /// </summary>
        public long QueryStart { get; set; }

        /// <summary>
        /// The end location of the match in the query sequence
        /// </summary>
        public long QueryEnd { get; set; }

        /// <summary>
        /// The start location of the match in the hit sequence
        /// </summary>
        public long HitStart { get; set; }

        /// <summary>
        /// The end location of the match in the hit sequence
        /// </summary>
        public long HitEnd { get; set; }

        /// <summary>
        /// The frame for the query sequence
        /// </summary>
        public long QueryFrame { get; set; }

        /// <summary>
        /// The frame for the hit sequence
        /// </summary>
        public long HitFrame { get; set; }

        /// <summary>
        /// Number of residues that matched exactly
        /// </summary>
        public long IdentitiesCount { get; set; }

        /// <summary>
        /// Number of residues that matched conservatively (for proteins)
        /// </summary>
        public long PositivesCount { get; set; }

        /// <summary>
        /// The length of the local match
        /// </summary>
        public long AlignmentLength { get; set; }

        /// <summary>
        /// The score density
        /// </summary>
        public int Density { get; set; }

        /// <summary>
        /// The local match in the query sequence, as a string
        /// </summary>
        public string QuerySequence { get; set; }

        /// <summary>
        /// The local match in the hit sequence, as a string
        /// </summary>
        public string HitSequence { get; set; }

        /// <summary>
        /// Gets or sets the formating middle line
        /// </summary>
        public string Midline { get; set; }

        /// <summary>
        /// Gets or sets start of PHI-BLAST pattern
        /// </summary>
        public int PatternFrom { get; set; }

        /// <summary>
        /// Gets or sets end of PHI-BLAST pattern
        /// </summary>
        public int PatternTo { get; set; }

        /// <summary>
        /// Gets are sets number of gaps in HSP
        /// </summary>
        public int Gaps { get; set; }
    }
}
