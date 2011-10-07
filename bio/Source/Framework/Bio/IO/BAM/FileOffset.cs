using System;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold offset of a BAM file.
    /// </summary>
    public class FileOffset
    {
        /// <summary>
        /// Gets or sets BGZF block start offset.
        /// </summary>
        public UInt64 CompressedBlockOffset { get; set; }

        /// <summary>
        /// Gets or sets an offset of uncompressed block inside a BGZF block 
        /// from which aligned sequences starts or ends.
        /// </summary>
        public UInt16 UncompressedBlockOffset { get; set; }
    }
}
