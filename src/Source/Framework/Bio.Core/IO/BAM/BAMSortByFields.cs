namespace Bio.IO.BAM
{
    /// <summary>
    /// Defines list of possible sort option for SequenceAlignmentMap
    /// </summary>
    public enum BAMSortByFields
    {
        /// <summary>
        /// Sort by Positions (Pos)
        /// </summary>
        ChromosomeCoordinates = 0,

        /// <summary>
        /// Sort by Read name (QName)
        /// </summary>
        ReadNames,

        /// <summary>
        /// Sort by Chromosome name (RName) and Positions (Pos)
        /// </summary>
        ChromosomeNameAndCoordinates
    }
}