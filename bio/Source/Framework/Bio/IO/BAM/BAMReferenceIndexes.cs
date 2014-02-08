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


        public bool HasMetaData {get;internal set;}

        public ulong MappedReads { get; internal set; }

        public ulong UnMappedReads { get; internal set; }

        //TODO: Currently these are in a pull request on github, see if they show up
        //public FileOffset UnmappedOffSetStart { get; private set; }
        //public FileOffset UnmappedOffSetEnd { get; private set; }


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
