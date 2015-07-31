using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Stores information about Contig - Contig mate pair map.
    /// Forward Contig     Reverse Contig
    /// ---------------) (---------------
    ///    -------)           (------
    ///    Forward              Reverse
    ///    read                 read    
    /// Key: Sequence of Forward Contig
    /// Value:
    ///     Key: Sequence of reverse contig
    ///     Value: List of mate pair between two contigs.
    /// </summary>
    public class ContigMatePairs : Dictionary<ISequence, Dictionary<ISequence, IList<ValidMatePair>>>
    {
        /// <summary>
        /// Initializes a new instance of the ContigMatePairs class.
        /// </summary>
        public ContigMatePairs()
        {
        }        

        /// <summary>
        /// Initializes a new instance of the ContigMatePairs class with specified contigs.
        /// </summary>
        /// <param name="contigs">List of contigs.</param>
        public ContigMatePairs(IEnumerable<ISequence> contigs)
        {
            if (contigs == null)
            {
                throw new ArgumentNullException("contigs");
            }

            foreach (ISequence contig in contigs)
            {
                Add(contig, new Dictionary<ISequence, IList<ValidMatePair>>());
            }
        }
    }
}
