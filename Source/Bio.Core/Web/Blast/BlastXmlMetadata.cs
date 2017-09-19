namespace Bio.Web.Blast
{
    /// <summary>
    /// Container for the Output segment of the XML BLAST format. This
    /// contains metadata for the search.
    /// </summary>
    public class BlastXmlMetadata
    {
        /// <summary>
        /// The name of the program invoked (blastp, etc.)
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// The BLAST version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Literature reference for BLAST (always the same)
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The database(s) searched
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// The query identifier defined by the service
        /// </summary>
        public string QueryId { get; set; }

        /// <summary>
        /// The query definition line (if any)
        /// </summary>
        public string QueryDefinition { get; set; }

        /// <summary>
        /// The length of the query sequence
        /// </summary>
        public int QueryLength { get; set; }

        /// <summary>
        /// The query sequence (optional, may not be returned)
        /// </summary>
        public string QuerySequence { get; set; }

        // note: the following attributes are nested in the <Parameters>
        // section inside <BlastOutput>.

        /// <summary>
        /// The name of the similarity matrix used
        /// </summary>
        public string ParameterMatrix { get; set; }

        /// <summary>
        /// The Expect value used for the search
        /// </summary>
        public double ParameterExpect { get; set; }

        /// <summary>
        /// The inclusion threshold for a PSI-Blast iteration
        /// </summary>
        public double ParameterInclude { get; set; }

        /// <summary>
        /// The match score for nucleotide-nucleotide comparisons
        /// </summary>
        public int ParameterMatchScore { get; set; }

        /// <summary>
        /// The mismatch score for nucleotide-nucleotide comparisons
        /// </summary>
        public int ParameterMismatchScore { get; set; }

        /// <summary>
        /// The gap open penalty
        /// </summary>
        public int ParameterGapOpen { get; set; }

        /// <summary>
        /// The Gap extension penalty.
        /// </summary>
        public int ParameterGapExtend { get; set; }

        /// <summary>
        /// The filtering options used for the search
        /// </summary>
        public string ParameterFilter { get; set; }

        /// <summary>
        /// The pattern used for PHI-Blast
        /// </summary>
        public string ParameterPattern { get; set; }

        /// <summary>
        /// The ENTREZ query used to limit the search
        /// </summary>
        public string ParameterEntrezQuery { get; set; }
    }
}
