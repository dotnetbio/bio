using System.Collections.Generic;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// An ISequenceAlignment is the result of running an alignment algorithm on a set 
    /// of two or more sequences. This could be a pairwise alignment, an MSA (multiple 
    /// sequence alignment), or an overlap alignment of the sort needed for sequence
    /// assembly.
    /// </summary>
    /// <remarks>
    /// this is just a storage object – it’s up to an algorithm object to fill it in.
    /// for efficiency’s sake, we are leaving it up to calling code to keep track of the 
    /// input sequences, if desired.
    /// </remarks>
    public interface ISequenceAlignment
    {
        /// <summary>
        /// Gets list of the IAlignedSequences which contains aligned sequences with score, offset and consensus .
        /// </summary>
        IList<IAlignedSequence> AlignedSequences { get; }

        /// <summary>
        /// Gets list of sequences.
        /// </summary>
        IList<ISequence> Sequences { get; }

        /// <summary>
        /// Gets any additional information about the Alignment.
        /// </summary>
        IDictionary<string, object> Metadata { get; }

        /// <summary>
        /// Gets or sets Documentation object is intended for tracking the history, provenance,
        /// and experimental context of a sequence. The user can adopt any desired
        /// convention for use of this object.
        /// </summary>
        object Documentation { get; set; }
    }
}
