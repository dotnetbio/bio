using System.Collections.Generic;
using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// A profile alignment algorithm that aligns two profiles. 
    /// 
    /// Profile-profile alignment and scoring is analogous to 
    /// pairwise sequence alignment, except that the objects 
    /// being compared are profiles: matrices of distributions, 
    /// representing the frequencies of sequence items at each 
    /// position in a corresponding multiple sequence alignment.
    /// 
    /// </summary>
    public interface IProfileAligner
    {
        /// <summary>
        /// AlignSimple uses a single gap penalty.
        /// </summary>
        /// <param name="profileA">first input profile</param>
        /// <param name="profileB">second input profile</param>
        IProfileAlignment AlignSimple(IProfileAlignment profileA, IProfileAlignment profileB);

        /// <summary>
        /// Align uses the affine gap model, which requires a gap open and a gap extension penalty.
        /// </summary>
        /// <param name="profileA">first input profile</param>
        /// <param name="profileB">second input profile</param>
        IProfileAlignment Align(IProfileAlignment profileA, IProfileAlignment profileB);

        /// <summary>
        /// The similarity matrix determines the score for any possible pair
        /// of symbols that are encountered at a common location across the 
        /// sequences being aligned.
        /// </summary>
        SimilarityMatrix SimilarityMatrix { set; get; }

        /// <summary>
        /// The GapOpenCost is the cost of inserting a gap character into 
        /// a sequence.
        /// </summary>
        /// <remarks>
        /// In the simple gap model, all gaps use this cost. In the affine gap
        /// model, the GapExtensionCost below is also used.
        /// </remarks>
        int GapOpenCost { set; get; }

        /// <summary>
        /// The GapExtensionCost is the cost of extending an already existing gap.
        /// This is used for the affine gap model, not used for the simple gap model.
        /// </summary>
        int GapExtensionCost { set; get; }

        /// <summary>
        /// Defined in MUSCLE (Edgar 2004) paper.
        /// eString stores the operation of the child node sequence to become
        /// the aligned sequence in its parent node.
        /// 
        /// eString is a vector of integers: positive integer n means 
        /// skip n letters; negative integer -n means insert n indels 
        /// at the current position.
        /// 
        /// The alignment path of sequence (leaf node) is the series of 
        /// eString through internal nodes from this leaf to root (including
        /// leaf node).
        /// 
        /// with eString, there's no need to adjust the sequences until
        /// progressive alignment is finished.
        /// </summary>
        /// <param name="aligned">aligned integer array</param>
        List<int> GenerateEString(int[] aligned);

        /// <summary>
        /// Apply alignment operation in eString to the sequence to
        /// generate aligned sequence
        /// </summary>
        /// <param name="eString">estring with alignment path</param>
        /// <param name="seq">a sequece to aligned</param>
        Sequence GenerateSequenceFromEString(List<int> eString, ISequence seq);

        /// <summary>
        /// Return aligned sequence as a vector of integer:
        /// positive integer N means it's original the Nth letter;
        /// negative integer -1 means it's an indel.
        /// 
        /// These two vectors are used to convert to eString (alignment paths)
        /// and combine two profiles/profileAlignments.
        /// 
        /// AlignedA is the aligned integer of the first sequence
        /// </summary>
        int[] AlignedA { get; }

        /// <summary>
        /// AlignedB is the aligned integer of the first sequence
        /// </summary>
        int[] AlignedB { get; }

        /// <summary>
        /// 
        /// </summary>
        float[] Weights { get; set; }
    }

    #region Enum ProfileAlignerNames
    /// <summary>
    /// The enum of profile-profile aligner names
    /// </summary>
    public enum ProfileAlignerNames
    {
        /// <summary>
        /// Profile Smith Waterman alignment algorithm
        /// </summary>
        SmithWatermanProfileAligner,

        /// <summary>
        /// profile Needleman Wunsch alignment algorithm
        /// </summary>
        NeedlemanWunschProfileAligner
    }
    #endregion
}
