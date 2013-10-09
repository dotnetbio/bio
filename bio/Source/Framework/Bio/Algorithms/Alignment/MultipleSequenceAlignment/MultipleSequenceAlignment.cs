using System;
using System.Collections.Generic;
using System.Threading;
using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public class MultipleSequenceAlignment
    {
        #region Fields

        //
        private List<ISequence> _sequences = null;

        //
        private float _msaScore = 0;

        //
        private int _numberOfSequences;

        //
        private int _numberOfColumns;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<ISequence> Sequences
        {
            get { return _sequences; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float MsaScore
        {
            get { return _msaScore; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfSequences
        {
            get { return _numberOfSequences; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfColumns
        {
            get { return _numberOfColumns; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public MultipleSequenceAlignment()
        {
            _sequences = new List<ISequence>();
            _numberOfSequences = 0;
            _numberOfColumns = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequences"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public MultipleSequenceAlignment(List<ISequence> sequences)
        {
            if (sequences.Count == 0)
            {
                throw new ArgumentException("Empty input sequences");
            }
            int numberOfColumns = (int)sequences[0].Count;
            for (int i = 0; i < sequences.Count; ++i)
            {
                if (sequences[i].Count != numberOfColumns)
                {
                    throw new ArgumentException("Unaligned sequences");
                }
            }

            _sequences = sequences;
            _numberOfColumns = numberOfColumns;
            _numberOfSequences = sequences.Count;
            
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

            //Parallel.For(0, sequences.Count - 1, i =>
            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    result += PairWiseScoreFunction(sequences[i], sequences[j], similarityMatrix, gapOpenPenalty, gapExtensionPenalty);
                }
            }
            //});

            return result /= sequences.Count * (sequences.Count - 1) / 2;
        }

        #endregion
    }
}
