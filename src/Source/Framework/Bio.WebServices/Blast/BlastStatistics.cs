namespace Bio.Web.Blast
{
    /// <summary>
    /// Container for the Statistics segment of the XML BLAST format.
    /// </summary>
    public class BlastStatistics
    {
        /// <summary>
        /// The number of sequences in the iteration
        /// </summary>
        public int SequenceCount { get; set; }

        /// <summary>
        /// Database size, for correction
        /// </summary>
        public long DatabaseLength { get; set; }

        /// <summary>
        /// Effective HSP length
        /// </summary>
        public long HspLength { get; set; }

        /// <summary>
        /// Effective search space
        /// </summary>
        public double EffectiveSearchSpace { get; set; }

        /// <summary>
        /// Karlin-Altschul parameter K
        /// </summary>
        public double Kappa { get; set; }

        /// <summary>
        /// Karlin-Altschul parameter Lambda
        /// </summary>
        public double Lambda { get; set; }

        /// <summary>
        /// Karlin-Altschul parameter H
        /// </summary>
        public double Entropy { get; set; }
    }
}
