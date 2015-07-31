namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Overlap between Read and Contig.
    /// </summary>
    public enum ContigReadOverlapType
    {
        /// <summary>
        /// FullOverlap.
        /// ------------- Contig
        ///    ------     Read
        /// </summary>
        FullOverlap,

        /// <summary>
        /// PartialOverlap.
        /// -------------       Contig
        ///            ------   Read
        /// </summary>
        PartialOverlap
    }
}
