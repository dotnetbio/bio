using System.Collections.Generic;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Interface to hold single aligned unit of alignment.
    /// </summary>
    public interface IAlignedSequence
    {
        /// <summary>
        /// Gets information about the AlignedSequence, like score, offsets, consensus, etc..
        /// </summary>
        IDictionary<string, object> Metadata { get; }

        /// <summary>
        /// Gets list of sequences, aligned as part of an alignment.
        /// </summary>
        IList<ISequence> Sequences { get; }
    }
}
