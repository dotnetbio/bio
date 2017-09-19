using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of ProfileAlignment class.
    /// 
    /// The class is basically the same as SequenceAlignment, with the difference 
    /// that it also inherits the new members defined in IProfileAlignment interface,
    /// i.e. new profile class, float alignment score.
    /// 
    /// With the new Profiles member, some methods need to be modified to 
    /// accommodate this change.
    /// 
    /// The class also provides a set of static methods to generate profileAlignment
    /// from profiles, sequences, and other profileAlignments.
    /// 
    /// </summary>
    public class ProfileAlignment : SequenceAlignment, IProfileAlignment
    {
        // The profiles of an alignment

        // The score for the alignment. Higher scores mean better alignments.
        // Internal score

        /// <summary>
        /// The alignment score of this alignment
        /// </summary>
        public new float Score { get; set; }

        /// <summary>
        /// The profiles of this class
        /// </summary>
        public IProfiles ProfilesMatrix { get; set; }

        /// <summary>
        /// The number of sequences associated with this profile alignment
        /// </summary>
        public int NumberOfSequences { get; set; }

        /// <summary>
        /// Modified.
        /// Clears the SequenceAlignment
        /// Throws exception if SequenceAlignment is read only.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            this.Score = 0;
            ProfilesMatrix.Clear();
        }

        /// <summary>
        /// Generate IProfileAlignment from a set of aligned sequences
        /// </summary>
        /// <param name="sequences">aligned sequences</param>
        public static IProfileAlignment GenerateProfileAlignment(ICollection<ISequence> sequences)
        {
            IProfiles profileMatrix = Profiles.GenerateProfiles(sequences);
            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = sequences.Count;
            profileAlignment.ProfilesMatrix = profileMatrix;
            return profileAlignment;
        }

        /// <summary>
        /// Generate IProfileAlignment from a set of aligned sequences
        /// </summary>
        /// <param name="sequences">aligned sequences</param>
        /// <param name="weights">sequence weights</param>
        public static IProfileAlignment GenerateProfileAlignment(ICollection<ISequence> sequences, float[] weights)
        {
            IProfiles profileMatrix = Profiles.GenerateProfiles(sequences, weights);
            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = sequences.Count;
            profileAlignment.ProfilesMatrix = profileMatrix;
            return profileAlignment;
        }

        /// <summary>
        /// Combine two profileAlignments into one if they are aligned already
        /// </summary>
        /// <param name="profileAlignmentA">first profile alignment</param>
        /// <param name="profileAlignmentB">second profile alignment</param>
        public static IProfileAlignment GenerateProfileAlignment(IProfileAlignment profileAlignmentA, IProfileAlignment profileAlignmentB)
        {

            IProfiles profileMatrix = Profiles.GenerateProfiles(
                        profileAlignmentA.ProfilesMatrix, profileAlignmentB.ProfilesMatrix,
                        profileAlignmentA.NumberOfSequences, profileAlignmentB.NumberOfSequences);

            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = profileAlignmentA.NumberOfSequences + profileAlignmentB.NumberOfSequences;
            profileAlignment.ProfilesMatrix = profileMatrix;

            return profileAlignment;
        }

        /// <summary>
        /// Generate a profileAlignment from one single sequence
        /// The set of sequence items of the seq should be the same as 
        /// 'static ItemSet' of the IProfiles.
        /// </summary>
        /// <param name="seq">an input sequence</param>
        public static IProfileAlignment GenerateProfileAlignment(ISequence seq)
        {
            IProfiles profileMatrix = Profiles.GenerateProfiles(seq);
            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = 1;
            profileAlignment.ProfilesMatrix = profileMatrix;
            return profileAlignment;
        }

        /// <summary>
        /// Generate a profileAlignment from one single sequence
        /// The set of sequence items of the seq should be the same as 
        /// 'static ItemSet' of the IProfiles.
        /// </summary>
        /// <param name="seq">an input sequence</param>
        /// <param name="weight">sequence weight</param>
        public static IProfileAlignment GenerateProfileAlignment(ISequence seq, float weight)
        {
            IProfiles profileMatrix = Profiles.GenerateProfiles(seq, weight);
            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = 1;
            profileAlignment.ProfilesMatrix = profileMatrix;
            return profileAlignment;
        }

        /// <summary>
        /// Combine two profileAlignments with alignment operation array from dynamic programming.
        /// The dynamic programming algorithm returns two arrays containing the alignment operations
        /// on the two profiles. This method applies the operation information in the two arrays to 
        /// the two original profiles, and combine them into a new aligned profile, and put into the
        /// newly generated profileAlignment.
        /// </summary>
        /// <param name="profileAlignmentA">first profile alignment</param>
        /// <param name="profileAlignmentB">second profile alignment</param>
        /// <param name="aAligned">aligned integer array generated by dynamic programming</param>
        /// <param name="bAligned">aligned integer array generated by dynamic programming</param>
        /// <param name="gapCode">the gap integer code defined in dynamic programming class</param>
        public static IProfileAlignment GenerateProfileAlignment(
                IProfileAlignment profileAlignmentA,
                IProfileAlignment profileAlignmentB,
                int[] aAligned,
                int[] bAligned,
                int gapCode)
        {
            IProfiles profileMatrix = Profiles.GenerateProfiles(
                profileAlignmentA.ProfilesMatrix, profileAlignmentB.ProfilesMatrix, 
                profileAlignmentA.NumberOfSequences, profileAlignmentB.NumberOfSequences,
                aAligned, bAligned, gapCode);

            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = profileAlignmentA.NumberOfSequences + 
                                                    profileAlignmentB.NumberOfSequences;
            profileAlignment.ProfilesMatrix = profileMatrix;

            return profileAlignment;
        }

        /// <summary>
        /// Combine two profileAlignments with alignment operation array from dynamic programming.
        /// The dynamic programming algorithm returns two arrays containing the alignment operations
        /// on the two profiles. This method applies the operation information in the two arrays to 
        /// the two original profiles, and combine them into a new aligned profile, and put into the
        /// newly generated profileAlignment.
        /// </summary>
        /// <param name="profileAlignmentA">first profile alignment</param>
        /// <param name="profileAlignmentB">second profile alignment</param>
        /// <param name="aAligned">aligned integer array generated by dynamic programming</param>
        /// <param name="bAligned">aligned integer array generated by dynamic programming</param>
        /// <param name="gapCode">the gap integer code defined in dynamic programming class</param>
        /// <param name="weights">the weights of two profileAlignments</param>
        public static IProfileAlignment GenerateProfileAlignment(
                IProfileAlignment profileAlignmentA,
                IProfileAlignment profileAlignmentB,
                int[] aAligned,
                int[] bAligned,
                int gapCode,
                float[] weights)
        {
            IProfiles profileMatrix = Profiles.GenerateProfiles(
                profileAlignmentA.ProfilesMatrix, profileAlignmentB.ProfilesMatrix,
                profileAlignmentA.NumberOfSequences, profileAlignmentB.NumberOfSequences,
                aAligned, bAligned, gapCode, weights);

            IProfileAlignment profileAlignment = new ProfileAlignment();
            profileAlignment.NumberOfSequences = profileAlignmentA.NumberOfSequences +
                                                    profileAlignmentB.NumberOfSequences;
            profileAlignment.ProfilesMatrix = profileMatrix;

            return profileAlignment;
        }

        /// <summary>
        /// The profiles of two subsets is extracted from the current multiple alignment.
        /// Columns containing no residues, i.e. indels only, are discarded.
        /// 
        /// This method is used in alignment refinement, when the guide tree is cut into two,
        /// the sequences (leaf nodes) are separated into two subsets. This method generates
        /// two profileAlignments for the two subtrees by extracting profiles of the two subsets
        /// of sequences.
        /// </summary>
        /// <param name="alignedSequences">a set of aligned sequences</param>
        /// <param name="sequenceIndicesA">the subset sequence indices of subtree A</param>
        /// <param name="sequenceIndicesB">the subset sequence indices of subtree B</param>
        /// <param name="allIndelPositions">the list of all-indel positions that have been removed when constructing</param>
        public static IProfileAlignment[] ProfileExtraction(IList<ISequence> alignedSequences, 
                                        IList<int> sequenceIndicesA, IList<int> sequenceIndicesB,
                                        out List<int>[] allIndelPositions)
        {
            allIndelPositions = new List<int>[2];
            IProfiles profileA = Profiles.GenerateProfiles(alignedSequences, sequenceIndicesA, out allIndelPositions[0]);
            IProfiles profileB = Profiles.GenerateProfiles(alignedSequences, sequenceIndicesB, out allIndelPositions[1]);
            IProfileAlignment profileAlignmentA = new ProfileAlignment();
            IProfileAlignment profileAlignmentB = new ProfileAlignment();
            profileAlignmentA.ProfilesMatrix = profileA;
            profileAlignmentB.ProfilesMatrix = profileB;
            profileAlignmentA.NumberOfSequences = sequenceIndicesA.Count;
            profileAlignmentB.NumberOfSequences = sequenceIndicesB.Count;

            return new IProfileAlignment[2] { profileAlignmentA, profileAlignmentB };
        }
    }
}
