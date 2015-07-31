using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// The implementation of progressive alignment.
    /// 
    /// Progressive alignment is a heuristic algorithm to align multiple sequences,
    /// with very fast speed. The algorithm contains a series steps of pairwise 
    /// alignment of a pair of sequences or profiles (aligned sequences). The accuracy
    /// performance of progressive alignment highly depends on the order of the steps.
    /// 
    /// The pros of progressive alignment is its fast speed and relatively high accuracy.
    /// The cons are that it is not global optimization, and the the performance is
    /// not good for divergent input sequences.
    /// </summary>
    public interface IProgressiveAligner
    {
        /// <summary>
        /// The pregressive alignment algorithm aligns a set of sequences guided by
        /// a binary tree. 
        /// </summary>
        /// <param name="sequences">input sequences</param>
        /// <param name="tree">a binary guide tree</param>
        void Align(IList<ISequence> sequences, BinaryGuideTree tree);

        /// <summary>
        /// The aligned sequences generated
        /// </summary>
        List<ISequence> AlignedSequences { get; }

        /// <summary>
        /// The name of the progressive aligner.
        /// </summary>
        string Name { get; }
    }
}
