using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
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
        /// A list of the (usually modified) output sequences, in the same order
        /// that the inputs were passed to the alignment algorithm.
        /// </summary>
        IList<ISequence> Sequences { set; get; }

        /// <summary>
        /// Offset is the starting position of alignment of sequence1 
        /// with respect to sequence2.
        /// </summary>
        IList<int> Offsets { set; get; }

        /// <summary>
        /// A consensus sequence representing the alignment.
        /// </summary>
        ISequence Consensus { set; get; }

        /// <summary>
        /// The score for the alignment. Higher scores mean better alignments.
        /// The score is determined by the alignment algorithm used.
        /// </summary>
        int Score { set; get; }

        /// <summary>
        /// The Documentation object is intended for tracking the history, provenance,
        /// and experimental context of a sequence. The user can adopt any desired
        /// convention for use of this object.
        /// </summary>
        object Documentation { set; get; }

        /// <summary>
        /// Clears the alignment
        /// </summary>
        void Clear();
    }
}
