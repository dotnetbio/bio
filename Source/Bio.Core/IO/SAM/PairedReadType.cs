namespace Bio.IO.SAM
{
    /// <summary>
    /// Specifies the type of paired read.
    /// </summary>
    public enum PairedReadType
    {
        /// <summary>
        /// Normal - Reads are aligning to same reference sequence 
        ///         and insertion length is with in the limit. 
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Orphan - One read is not aligned to any reference sequence.
        /// </summary>
        Orphan,

        /// <summary>
        /// Chimera - Reads are not aligning to same reference sequence.
        /// </summary>
        Chimera,

        /// <summary>
        /// StructuralAnomaly - Reads are not in proper orientation.
        /// </summary>
        StructuralAnomaly,

        /// <summary>
        /// LengthAnomaly - Insertion length is either too short or too long.
        /// </summary>
        LengthAnomaly,

        /// <summary>
        /// MultipleHits - A mapped read pair is stored in more than two aligned sequences.
        /// </summary>
        MultipleHits
    }
}