using System;
using System.Collections.Generic;
using System.Linq;

using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiSequenceAlignment
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<ISequence> Sequences { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float MsaScore { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfSequences { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfColumns { get; private set; }

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public MultiSequenceAlignment()
        {
            this.MsaScore = 0;
            this.Sequences = new List<ISequence>();
            this.NumberOfSequences = 0;
            this.NumberOfColumns = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequences"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public MultiSequenceAlignment(IList<ISequence> sequences)
        {
            MsaScore = 0;
            if (sequences == null || sequences.Count == 0)
                throw new ArgumentException("Empty input sequences");

            int numberOfColumns = (int) sequences[0].Count;
            if (sequences.Any(t => t.Count != numberOfColumns))
            {
                throw new ArgumentException("Unaligned sequences");
            }

            this.Sequences = sequences;
            this.NumberOfColumns = numberOfColumns;
            this.NumberOfSequences = sequences.Count;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate pairwise score of a pair of aligned sequences.
        /// The score is the sum over all position score given by the similarity matrix.
        /// The positions with only indels, e.g. gaps, are discarded. Gaps in the remaining 
        /// columns are assessed affined score: g + w * e, where g is open penalty, and e
        /// is extension penalty.
        /// </summary>
        /// <param name="sequenceA">aligned sequence</param>
        /// <param name="sequenceB">aligned sequence</param>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="gapOpenPenalty">negative open gap penalty</param>
        /// <param name="gapExtensionPenalty">negative extension gap penalty</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static float PairWiseScoreFunction(ISequence sequenceA, ISequence sequenceB, SimilarityMatrix similarityMatrix,
                                                int gapOpenPenalty, int gapExtensionPenalty)
        {
            if (sequenceA.Count != sequenceB.Count)
            {
                throw new Exception("Unaligned sequences");
            }
            float result = 0;

            bool isGapA = false;
            bool isGapB = false;

            for (int i = 0; i < sequenceA.Count; ++i)
            {
                if (sequenceA.Alphabet.CheckIsGap(sequenceA[i]) && sequenceB.Alphabet.CheckIsGap(sequenceB[i]))
                {
                    continue;
                }
                if (sequenceA.Alphabet.CheckIsGap(sequenceA[i]) && !sequenceB.Alphabet.CheckIsGap(sequenceB[i]))
                {
                    if (isGapB)
                    {
                        isGapB = false;
                    }
                    if (isGapA)
                    {
                        result += gapExtensionPenalty;
                    }
                    else
                    {
                        result += gapOpenPenalty;
                        isGapA = true;
                    }
                    continue;
                }
                if (!sequenceA.Alphabet.CheckIsGap(sequenceA[i]) && sequenceB.Alphabet.CheckIsGap(sequenceB[i]))
                {
                    if (isGapA)
                    {
                        isGapA = false;
                    }
                    if (isGapB)
                    {
                        result += gapExtensionPenalty;
                    }
                    else
                    {
                        result += gapOpenPenalty;
                        isGapB = true;
                    }
                    continue;
                }

                result += similarityMatrix[sequenceA[i], sequenceB[i]];
            }
            return result;
        }

        /// <summary>
        /// Calculate alignment score of a set of aligned sequences.
        /// The score is the average over all pairs of sequences of their pairwise alignment score.
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="gapOpenPenalty">negative open gap penalty</param>
        /// <param name="gapExtensionPenalty">negative extension gap penalty</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static float MultipleAlignmentScoreFunction(List<ISequence> sequences, SimilarityMatrix similarityMatrix,
                                                                int gapOpenPenalty, int gapExtensionPenalty)
        {
            float result = 0;

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    result += PairWiseScoreFunction(sequences[i], sequences[j], similarityMatrix, gapOpenPenalty, gapExtensionPenalty);
                }
            }

            return result /= (float)sequences.Count * (sequences.Count - 1) / 2;
        }

        #endregion
    }
}
