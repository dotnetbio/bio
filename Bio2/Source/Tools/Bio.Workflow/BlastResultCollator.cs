namespace Bio.Workflow
{
    /// <summary>
    /// BlastResultCollater class collates all the information of the
    /// Blast search for UI rendering.
    /// </summary>
    public class BlastResultCollator
    {
        /// <summary>
        /// Gets or sets the Query Id of the BLAST search result.
        /// </summary>
        public string QueryId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Subject Id of the BLAST search result.
        /// </summary>
        public string SubjectId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Identity of the BLAST search result.
        /// </summary>
        public long Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Alignment found in the BLAST search result.
        /// </summary>
        public long Alignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Length of the BLAST search result.
        /// </summary>
        public long Length
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MisMatches found in the BLAST search result.
        /// </summary>
        public string Mismatches
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GapOpenings found in the BLAST search result.
        /// </summary>
        public string GapOpenings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the QStartof the BLAST search result.
        /// </summary>
        public long QStart
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the QEnd the BLAST search result.
        /// </summary>
        public long QEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SStart the BLAST search result.
        /// </summary>
        public long SStart
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SEnd the BLAST search result.
        /// </summary>
        public long SEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the EValue the BLAST search result.
        /// </summary>
        public double EValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Number of bits in the BLAST search result.
        /// </summary>
        public double Bit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Number of positives in the BLAST search result.
        /// </summary>
        public long Positives
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Query string of the BLAST search result.
        /// </summary>
        public string QueryString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Subject string of the BLAST search result.
        /// </summary>
        public string SubjectString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Accession number of the hit
        /// </summary>
        public string Accession
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Description of the hit
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
