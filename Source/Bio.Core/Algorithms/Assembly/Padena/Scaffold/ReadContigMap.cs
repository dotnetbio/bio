using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Class stores multiple mapping between reads and a contig.
    ///     -------------------         Read Sequence
    /// ------------------------------  Contig Sequence [Full Overlap]
    ///               ----------------  Contig Sequence [Partial Overlap]
    /// The Class stores 
    /// Key: Sequence Id of Read 
    /// Value
    ///     Key: Sequence of Contig
    ///     Value: List of position of Overlaps of contig with read.
    /// </summary>
    public class ReadContigMap : Dictionary<string, Dictionary<ISequence, IList<ReadMap>>>
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the ReadContigMap class.
        /// </summary>
        public ReadContigMap()
        {
        }               

        /// <summary>
        /// Initializes a new instance of the ReadContigMap class with specified reads.
        /// </summary>
        /// <param name="reads">List of reads.</param>
        public ReadContigMap(IEnumerable<ISequence> reads)
        {
            if (reads == null)
            {
                throw new ArgumentNullException("reads");
            }

            foreach (ISequence read in reads)
            {
                this.Add(read.ID, new Dictionary<ISequence, IList<ReadMap>>());
            }
        }
        #endregion
    }
}
