using System.Collections.Generic;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// A sequence alignment algorithm that aligns exactly two 
    /// sequences. This may diverge from ISequenceAligner at some 
    /// point; meanwhile, it's important to maintain the distinction
    /// (e.g., assembly requires a pairwise algorithm).
    /// </summary>
    public interface IPairwiseSequenceAligner : ISequenceAligner
    {
        /// <summary>
        /// A convenience method - we know there are exactly two inputs.
        /// AlignSimple uses a single gap penalty.
        /// </summary>
        /// <param name="sequence1">First input sequence.</param>
        /// <param name="sequence2">Second input sequence.</param>
        /// <returns>List of Aligned Sequences.</returns>
        IList<IPairwiseSequenceAlignment> AlignSimple(ISequence sequence1, ISequence sequence2);

        /// <summary>
        /// A convenience method - we know there are exactly two inputs.
        /// Align uses the affine gap model, which requires a gap open and a gap extension penalty.
        /// </summary>
        /// <param name="sequence1">First input sequence.</param>
        /// <param name="sequence2">Second input sequence.</param>
        /// <returns>List of Aligned Sequences.</returns>
        IList<IPairwiseSequenceAlignment> Align(ISequence sequence1, ISequence sequence2);
    }
}
