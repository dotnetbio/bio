namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold start and end offsets of a BAM file chunk related to a bin.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// Gets or sets start offset of this chunk.
        /// </summary>
        public FileOffset ChunkStart { get; set; }

        /// <summary>
        /// Gets or sets end offset of this chunk.
        /// </summary>
        public FileOffset ChunkEnd { get; set; }
    }
}
