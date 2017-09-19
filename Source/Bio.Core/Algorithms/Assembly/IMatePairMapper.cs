using System.Collections.Generic;
using Bio.Algorithms.Assembly.Padena.Scaffold;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Map Reads to Mate Pairs
    /// Interface can be implemented by classes which map reads to mate pairs
    /// in another input formats.
    /// </summary>
    public interface IMatePairMapper
    {
        /// <summary>
        /// Map Reads to mate pairs.
        /// </summary>
        /// <param name="reads">List of reads.</param>
        /// <returns>List of mate pairs.</returns>
        IList<MatePair> Map(IEnumerable<ISequence> reads);

        /// <summary>
        /// Finds contig pairs having valid mate pairs connection between them.
        /// </summary>
        /// <param name="reads">Input list of reads.</param>
        /// <param name="alignment">Reads con alignment.</param>
        /// <returns>Contig Mate pair map.</returns>
        ContigMatePairs MapContigToMatePairs(IEnumerable<ISequence> reads, ReadContigMap alignment);
    }
}
