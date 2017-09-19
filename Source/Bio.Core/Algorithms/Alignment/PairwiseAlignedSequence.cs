using System.Linq;
using System.Text;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// PairwiseAlignedSequence is a class containing the single aligned unit of pairwise alignment.
    /// </summary>
    public class PairwiseAlignedSequence : AlignedSequence
    {
        #region Private constants
        /// <summary>
        /// Constant string indicating consensus in meta-data.
        /// </summary>
        private const string ConsensusKey = "Consensus";

        /// <summary>
        /// Constant string indicating alignment score in meta-data.
        /// </summary>
        private const string ScoreKey = "Score";

        /// <summary>
        /// Constant string indicating offset of first sequence in alignment.
        /// </summary>
        private const string FirstOffsetKey = "FirstOffset";

        /// <summary>
        /// Constant string indicating offset of second sequence in alignment.
        /// </summary>
        private const string SecondOffsetKey = "SecondOffset";
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the PairwiseAlignedSequence class.
        /// </summary>
        public PairwiseAlignedSequence()
            : base()
        {
            // No impl.
        }

        /// <summary>
        /// Initializes a new instance of the PairwiseAlignedSequence class
        /// Internal constructor for creating new instance of 
        /// PairwiseAlignedSequence from specified IAlignedSequence.
        /// </summary>
        /// <param name="alignedSequence">IAlignedSequence instance.</param>
        internal PairwiseAlignedSequence(IAlignedSequence alignedSequence)
            : base(alignedSequence)
        {
            // Impl.
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Alignment of First Sequence.
        /// </summary>
        public ISequence FirstSequence
        {
            get
            {
                if (Sequences.Count > 0)
                {
                    return Sequences[0];
                }

                return null;
            }

            set
            {
                if (Sequences.Count == 0)
                {
                    Sequences.Add(null);
                }

                Sequences[0] = value;
            }
        }

        /// <summary>
        /// Gets or sets Alignment of Second Sequence.
        /// </summary>
        public ISequence SecondSequence
        {
            get
            {
                if (Sequences.Count > 1)
                {
                    return Sequences[1];
                }

                return null;
            }

            set
            {
                if (Sequences.Count == 0)
                {
                    Sequences.Add(null);
                }

                if (Sequences.Count == 1)
                {
                    Sequences.Add(null);
                }

                Sequences[1] = value;
            }
        }

        /// <summary>
        /// Gets or sets Consensus of FirstSequence and SecondSequence.
        /// </summary>
        public ISequence Consensus
        {
            get
            {
                if (Metadata.ContainsKey(ConsensusKey))
                {
                    return Metadata[ConsensusKey] as ISequence;
                }

                return null;
            }

            set
            {
                Metadata[ConsensusKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets Score of the alignment.
        /// </summary>
        public long Score
        {
            get
            {
                long score = 0;
                if (Metadata.ContainsKey(ScoreKey))
                {
                    if (Metadata[ScoreKey] is long)
                    {
                        score = (long)Metadata[ScoreKey];
                    }
                }

                return score;
            }

            set
            {
                Metadata[ScoreKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets Offset of FirstSequence.
        /// </summary>
        public long FirstOffset
        {
            get
            {
                long score = 0;
                if (Metadata.ContainsKey(FirstOffsetKey))
                {
                    if (Metadata[FirstOffsetKey] is long)
                    {
                        score = (long)Metadata[FirstOffsetKey];
                    }
                }

                return score;
            }

            set
            {
                Metadata[FirstOffsetKey] = value;
            }
        }

        /// <summary>
        /// Gets or sets Offset of SecondSequence.
        /// </summary>
        public long SecondOffset
        {
            get
            {
                long score = 0;
                if (Metadata.ContainsKey(SecondOffsetKey))
                {
                    if (Metadata[SecondOffsetKey] is long)
                    {
                        score = (long)Metadata[SecondOffsetKey];
                    }
                }

                return score;
            }

            set
            {
                Metadata[SecondOffsetKey] = value;
            }
        }
        #endregion
        /// <summary>
        /// Converts the Consensus, First and Second sequences.
        /// </summary>
        /// <returns>Consensus, First and Second sequences.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(this.Consensus.ToString());
            builder.AppendLine(this.FirstSequence.ToString());
            builder.AppendLine(this.SecondSequence.ToString());
            return builder.ToString();
        }
    }
}
