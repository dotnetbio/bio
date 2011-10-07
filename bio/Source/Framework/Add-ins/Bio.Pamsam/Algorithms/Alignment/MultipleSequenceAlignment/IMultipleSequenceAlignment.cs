using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Multiple sequences alignment interface.
    /// Multiple sequence alignment (MSA) is used to align three or  
    /// more sequences in preparation for further analysis.  
    /// More info on MSA can be found at 
    /// http://en.wikipedia.org/wiki/Multiple_sequence_alignment)
    /// /// </summary>
    public interface IMultipleSequenceAlignment
    {
        /// <summary>
        /// Aligned sequences with equal length by inserting gaps '-' at
        /// appropriate positions so that the alignment score is optimized.
        /// </summary>
        List<ISequence> AlignedSequences { get; }

        /// <summary>
        /// The alignment score of the multiple sequence alignment.
        /// A typical score is the summation of pairwise alignment scores.
        /// </summary>
        float AlignmentScore { get; }

        /// <summary>
        /// The method to align multiple sequences.
        /// The gap penalty is affine gap score.
        /// </summary>
        /// <param name="sequences">a set of unaligned sequences</param>
        void Align(List<ISequence> sequences);

        /// <summary>
        /// The name of multiple sequence alignment method.
        /// </summary>
        string Name { get; }
    }
}
