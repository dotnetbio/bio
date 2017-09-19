using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Bio
{
    /// <summary>
    /// Node that tracks difference between the two sequences.
    /// </summary>
    public class DifferenceNode
    {
        /// <summary>
        /// Start position of difference in first sequence 
        /// </summary>
        private int sequence1Start;

        /// <summary>
        /// End position of difference in first sequence 
        /// </summary>
        private int sequence1End;

        /// <summary>
        /// Start position of difference in second sequence 
        /// </summary>
        private int sequence2Start;

        /// <summary>
        /// End position of difference in second sequence 
        /// </summary>
        private int sequence2End;

        /// <summary>
        /// Initializes a new instance of the DifferenceNode class.
        /// </summary>
        /// <param name="startIndex1">Start index in first sequence</param>
        /// <param name="startIndex2">Start index in second sequence</param>
        /// <param name="endIndex1">End index in first sequence</param>
        /// <param name="endIndex2">End index in second sequence</param>
        public DifferenceNode(int startIndex1, int startIndex2, int endIndex1, int endIndex2)
        {
            sequence1Start = startIndex1;
            sequence2Start = startIndex2;
            sequence1End = endIndex1;
            sequence2End = endIndex2;
        }

        /// <summary>
        /// Gets start index of difference in first sequence
        /// </summary>
        public int Sequence1Start
        {
            get { return sequence1Start; }
        }

        /// <summary>
        /// Gets start index of difference in second sequence
        /// </summary>
        public int Sequence2Start
        {
            get { return sequence2Start; }
        }

        /// <summary>
        /// Gets end index of difference in first sequence
        /// </summary>
        public int Sequence1End
        {
            get { return sequence1End; }
        }

        /// <summary>
        /// Gets end index of difference in second sequence
        /// </summary>
        public int Sequence2End
        {
            get { return sequence2End; }
        }

        /// <summary>
        /// Comparison of two word match list nodes
        /// based on the first sequence start indices
        /// </summary>
        /// <param name="n1">First match list node</param>
        /// <param name="n2">Second match list node</param>
        /// <returns>Integer value indicating zero if equal.</returns>
        public static int CompareDifferenceNode(WordMatch n1, WordMatch n2)
        {
            if (n1 == null)
            {
                throw new ArgumentNullException("n1");
            }

            if (n2 == null)
            {
                throw new ArgumentNullException("n2");
            }

            return n1.Sequence1Start - n2.Sequence1Start;
        }

        /// <summary>
        /// Builds difference list from match list
        /// </summary>
        /// <param name="matchList">List of matching segments</param>
        /// <param name="sequence1">First sequence</param>
        /// <param name="sequence2">Second sequence</param>
        /// <returns>List of difference nodes</returns>
        public static List<DifferenceNode> BuildDiffList(List<WordMatch> matchList, ISequence sequence1, ISequence sequence2)
        {
            if (matchList == null)
            {
                throw new ArgumentNullException("matchList");
            }

            if (sequence1 == null)
            {
                throw new ArgumentNullException("sequence1");
            }

            if (sequence2 == null)
            {
                throw new ArgumentNullException("sequence2");
            }

            // Sort match list according to start indices
            matchList.Sort(CompareDifferenceNode);

            List<DifferenceNode> diffList = new List<DifferenceNode>();

            int mismatchStart1, mismatchStart2, mismatchEnd1, mismatchEnd2;
            mismatchStart1 = 0;
            mismatchStart2 = 0;

            // There might be some mutation at starting
            WordMatch first = matchList[0];
            if (first.Sequence1Start > 0 || first.Sequence2Start > 0)
            {
                diffList.Add(new DifferenceNode(0, 0, first.Sequence1Start - 1, first.Sequence2Start - 1));
            }

            // Primer for starting iteration
            matchList.RemoveAt(0);
            mismatchStart1 = first.Sequence1Start + first.Length;
            mismatchStart2 = first.Sequence2Start + first.Length;

            foreach (WordMatch n in matchList)
            {
                mismatchEnd1 = n.Sequence1Start - 1;
                mismatchEnd2 = n.Sequence2Start - 1;
                diffList.Add(new DifferenceNode(mismatchStart1, mismatchStart2, mismatchEnd1, mismatchEnd2));
                mismatchStart1 = n.Sequence1Start + n.Length;
                mismatchStart2 = n.Sequence2Start + n.Length;
            }

            // There might be some mutation at the end
            mismatchEnd1 = (int)sequence1.Count - 1;
            mismatchEnd2 = (int)sequence2.Count - 1;
            if (mismatchStart1 <= mismatchEnd1 || mismatchStart2 <= mismatchEnd2)
            {
                diffList.Add(new DifferenceNode(mismatchStart1, mismatchStart2, mismatchEnd1, mismatchEnd2));
            }

            return diffList;
        }

        /// <summary>
        /// Constructs output from input difference list
        /// </summary>
        /// <param name="diffList">Difference list</param>
        /// <param name="sequence1">First Sequence</param>
        /// <param name="sequence2">Second Sequence</param>
        /// <returns>List of features</returns>
        public static List<CompareFeature> OutputDiffList(List<DifferenceNode> diffList, ISequence sequence1, ISequence sequence2)
        {
            if (diffList == null)
            {
                throw new ArgumentNullException("diffList");
            }

            List<CompareFeature> features = new List<CompareFeature>();
            foreach (DifferenceNode diff in diffList)
            {
                features.AddRange(
                    ComputeFeatures(
                        diff.sequence1Start, 
                        diff.sequence2Start,
                        diff.sequence1End,
                        diff.sequence2End,
                        sequence1, 
                        sequence2,
                        1,
                        2));
                features.AddRange(
                    ComputeFeatures(
                        diff.sequence2Start, 
                        diff.sequence1Start,
                        diff.sequence2End,
                        diff.sequence1End,
                        sequence2, 
                        sequence1,
                        2,
                        1));
            }

            return features;
        }

        /// <summary>
        /// Computes features for current difference
        /// </summary>
        /// <param name="sequence1Start">Start index of difference in first sequence</param>
        /// <param name="sequence2Start">Start index of difference in second sequence</param>
        /// <param name="sequence1End">End index of difference in first sequence</param>
        /// <param name="sequence2End">End index of difference in second sequence</param>
        /// <param name="sequence1">First sequence</param>
        /// <param name="sequence2">Second sequence</param>
        /// <param name="sequence1Index">First sequence index</param>
        /// <param name="sequence2Index">Second sequence index</param>
        /// <returns>List of features</returns>
        private static List<CompareFeature> ComputeFeatures(
            int sequence1Start,
            int sequence2Start,
            int sequence1End,
            int sequence2End,
            ISequence sequence1,
            ISequence sequence2,
            int sequence1Index,
            int sequence2Index)
        {
            List<CompareFeature> features = new List<CompareFeature>();
            string noteStr, replaceStr, conflictStr, sourceStr;

            int sequence1Length = sequence1End - sequence1Start + 1;
            int sequence2Length = sequence2End - sequence2Start + 1;

            if (sequence1Length != 0)
            {
                if (sequence1Length == 1 && sequence2Length == 1)
                {
                    noteStr = string.Format(
                        CultureInfo.CurrentCulture,
                        "SNP in Sequence {0}: {1}",
                        sequence1Index,
                        sequence1.ID);
                }
                else if (sequence2Length == 0)
                {
                    noteStr = string.Format(
                        CultureInfo.CurrentCulture,
                        "Insertion of {0} bases in {1} {2}",
                        sequence1Length,
                        sequence1Index,
                        sequence1.ID);
                }
                else
                {
                    noteStr = string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} {1}",
                        sequence2Index,
                        sequence2.ID);
                }

                // Add feature
                features.Add(new CompareFeature(sequence1Start, sequence1End, "NOTE", noteStr));

                if (sequence2Length > 0)
                {
                    replaceStr = new string(sequence2.Select(a => (char)a).ToArray()).Substring(sequence2Start, sequence2Length);
                }
                else
                {
                    replaceStr = string.Empty;
                }

                if (sequence1.Alphabet == Alphabets.Protein)
                {                    
                    if (replaceStr.Length > 0)
                    {
                        string seqstring = new string(sequence1.Select(a => (char)a).ToArray());
                        sourceStr = seqstring .Substring(sequence1Start, sequence1Length);
                        conflictStr = string.Format(CultureInfo.CurrentCulture, "{0} -> {1}", sourceStr, replaceStr);
                    }
                    else
                    {
                        conflictStr = "MISSING";
                    }

                    // Add feature
                    features.Add(new CompareFeature(sequence1Start, sequence1End, "NOTE", conflictStr));
                }
                else
                {
                    // Add feature
                    features.Add(new CompareFeature(sequence1Start, sequence1End, "REPLACE", replaceStr));
                }
            }

            return features;
        }

        /// <summary>
        /// Constructs a user-friendly string representation.
        /// Used for debug purpose.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return string.Format(
                "{0} : {1} : {2} : {3}",
                sequence1Start,
                sequence2Start,
                sequence1End,
                sequence2End);
        }

        /// <summary>
        /// Structure that maintains node structure for feature list.
        /// </summary>
        public struct CompareFeature
        {
            /// <summary>
            /// Difference start position
            /// </summary>
            private int start;

            /// <summary>
            /// Difference end position
            /// </summary>
            private int end;

            /// <summary>
            /// Type of feature
            /// </summary>
            private string featureType;

            /// <summary>
            /// Feature Description
            /// </summary>
            private string feature;

            /// <summary>
            /// Initializes a new instance of the CompareFeature class.
            /// </summary>
            /// <param name="startPosition">Difference start position</param>
            /// <param name="endPosition">Difference end position</param>
            /// <param name="featureType">Type of feature</param>
            /// <param name="feature">Feature details</param>
            public CompareFeature(int startPosition, int endPosition, string featureType, string feature)
            {
                this.start = startPosition;
                this.end = endPosition;
                this.featureType = featureType;
                this.feature = feature;
            }

            /// <summary>
            /// Gets value of start index 
            /// </summary>
            public int Start
            {
                get { return start; }
            }

            /// <summary>
            /// Gets value of end index 
            /// </summary>
            public int End
            {
                get { return end; }
            }

            /// <summary>
            /// Gets value of feature type
            /// </summary>
            public string FeatureType
            {
                get { return featureType; }
            }

            /// <summary>
            /// Gets feature description
            /// </summary>
            public string Feature
            {
                get { return feature; }
            }
        }
    }
}
