using System.Collections.Generic;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Interface is used by classes that maps mate pairs to contigs. 
    /// </summary>
    public interface IReadContigMapper
    {
        /// <summary>
        /// Map reads to contigs.
        /// Reads are aligned to contigs for distance calculation between
        /// contigs using mate pair library information, which will aid in scaffold building. 
        /// </summary>
        /// <param name="contigs">List of contig sequences.</param>
        /// <param name="reads">List of paired reads to be mapped.</param>
        /// <param name="kmerLength">Length of kmer.</param>
        /// <returns>Read contig Map.</returns>
        ReadContigMap Map(IList<ISequence> contigs, IEnumerable<ISequence> reads, int kmerLength);
    }
}
