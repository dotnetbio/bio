using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// A simple implementation of ISequenceAlignment that stores the 
    /// results as new sequences
    /// </summary>
    public class SequenceAlignment : ISequenceAlignment
    {
        /// <summary>
        /// A list of the (usually modified) output sequences, in the same order
        /// that the inputs were passed to the alignment algorithm.
        /// </summary>
        public IList<ISequence> Sequences { get; set; }

        /// <summary>
        /// A consensus sequence representing the alignment.
        /// </summary>
        public ISequence Consensus { get; set; }

        /// <summary>
        /// The score for the alignment. Higher scores mean better alignments.
        /// The score is determined by the alignment algorithm used.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Offset is the starting position of alignment of sequence1
        /// with respect to sequence2.
        /// </summary>
        public IList<int> Offsets { get; set; }

        /// <summary>
        /// The Documentation object is intended for tracking the history, provenance,
        /// and experimental context of a sequence. The user can adopt any desired
        /// convention for use of this object.
        /// </summary>
        public object Documentation { get; set; }

        /// <summary>
        /// Clear the sequence alignment
        /// </summary>
        public virtual void Clear()
        {
            Consensus = null;
            Score = 0;
            Offsets.Clear();
            Sequences.Clear();
        }
    }
}
