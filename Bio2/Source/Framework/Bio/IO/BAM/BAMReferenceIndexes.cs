using System.Collections.Generic;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold BAM file index information related to a reference sequence.
    /// Holds bin and linear index information for a reference sequence.
    /// </summary>
    public class BAMReferenceIndexes
    {
        /// <summary>
        /// Gets list of Bin index information.
        /// </summary>
        public IList<Bin> Bins { get; private set; }

        /// <summary>
        /// Gets list of Linear file offsets.
        /// </summary>
        public IList<FileOffset> LinearOffsets { get; private set; }

        /// <summary>
        /// Creats new instance of BAMReferenceIndexes class.
        /// </summary>
        public BAMReferenceIndexes()
        {
            Bins = new List<Bin>();
            LinearOffsets = new List<FileOffset>();
        }
    }
}
