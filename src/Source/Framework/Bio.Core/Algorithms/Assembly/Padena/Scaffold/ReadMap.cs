namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    ///  Class storing information of a single map between read and contig.
    /// </summary>
    public class ReadMap
    {
        /// <summary>
        /// Gets or sets start position of contig.
        /// </summary>
        public long StartPositionOfContig { get; set; }

        /// <summary>
        /// Gets or sets start position of read. 
        /// </summary>
        public long StartPositionOfRead { get; set; }

        /// <summary>
        /// Gets or sets length of map between read and contig.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets overlap of read and contig.
        /// FullOverlap
        /// ------------- Contig
        ///    ------     Read
        /// PartialOverlap
        /// -------------       Contig
        ///            ------   Read
        /// </summary>
        public ContigReadOverlapType ReadOverlap { get; set; }
    }
}
