using System;
using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Classes and methods to read and write PacBio data.
/// </summary>
namespace Bio.IO.PacBio
{
    /// <summary>
    /// A class representing the consensus sequence generated from multiple subreads.  This is produced
    /// by the program pbccs and output to a BAM file. 
    /// </summary>
    public class PacBioCCSRead
    {
        /// <summary>
        /// A measure of CCS read quality, currently capped at 99.9% (QV30)
        /// </summary>
        public readonly float ReadQuality;

        /// <summary>
        /// The read group.
        /// </summary>
        public readonly string ReadGroup;

        /// <summary>
        /// The number of subreads that were used to generate the consensus sequence.
        /// </summary>
        public readonly int NumPasses;

        /// <summary>
        /// The outcome counts for results of adding each subread.  Most subreads
        /// should be correctly added, but some may be dropped due to various
        /// exceptions enumerated here.
        /// </summary>
        private readonly int[] statusCounts;

        /// <summary>
        /// Count of subreads successfully added for consensus generation.
        /// </summary>
        public int ReadCountSuccessfullyAdded {
            get{ return statusCounts [0]; }
        }

        /// <summary>
        /// Count of subreads not added to consensus for failing alpha/beta mismatch check.
        /// </summary>
        /// <value>The ReadCount alpha beta mismatch.</value>
        public int ReadCountAlphaBetaMismatch {
            get { return statusCounts [1]; }
        }

        /// <summary>
        /// Count of subreads not added to consensus for allocating too much memory.
        /// </summary>
        /// <value>The ReadCount mem fail.</value>
        public int ReadCountMemFail {
            get { return statusCounts [2]; }
        }

        /// <summary>
        /// Count of subreads not added to consensus for having too low a Z-score.
        /// </summary>
        /// <value>The ReadCount bad zscore.</value>
        public int ReadCountBadZscore {
            get { return statusCounts [3]; }
        }
        public int ReadCountOther {
            get{ return statusCounts [4]; }
        }

        /// <summary>
        /// The SNR of the A Channel
        /// </summary>
        public readonly float SnrA;

        /// <summary>
        /// The SNR of the C Channel
        /// </summary>
        public readonly float SnrC;

        /// <summary>
        /// The SNR of the G Channel
        /// </summary>
        public readonly float SnrG;

        /// <summary>
        /// The SNR of the T Channel
        /// </summary>
        public readonly float SnrT;

        /// <summary>
        /// The name of the movie this CCS read came from.
        /// </summary>
        public string Movie;

        /// <summary>
        /// What is the hole number for the ZMW.
        /// </summary>
        public int HoleNumber;

        /// <summary>
        /// The average Z-score for all subreads.
        /// </summary>
        public float AvgZscore;

        /// <summary>
        /// An array of Z-scores for each subread added.  Subreads that were not added are report as 
        /// Double.NaN values.
        /// </summary>
        public readonly float[] ZScores;

        /// <summary>
        /// The consensus sequence and associated QV values.
        /// </summary>
        public QualitativeSequence Sequence;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bio.IO.PacBio.PacBioCCSRead"/> class. From an initially parsed BAM file.
        /// </summary>
        /// <param name="s">S.</param>
        public PacBioCCSRead (SAMAlignedSequence s)
        {
            /* TODO: Converting from binary to string and back is beyond silly...
             * no performance hit worth worrying about at present, but in the future it might be worth
             * going directly from binary to the type rather than through string intermediates */
            foreach (var v in s.OptionalFields) {
                if (v.Tag == "sn") {
                    var snrs = v.Value.Split (',').Skip (1).Select (x => Convert.ToSingle (x)).ToArray ();
                    SnrA = snrs [0];
                    SnrC = snrs [1];
                    SnrG = snrs [2];
                    SnrT = snrs [3];
                } else if (v.Tag == "zm") {
                    HoleNumber = (int)Convert.ToInt32 (v.Value);
                } else if (v.Tag == "rq") {
                    ReadQuality = Convert.ToInt32 (v.Value) / 1000.0f;
                } else if (v.Tag == "za") {
                    AvgZscore = (float)Convert.ToSingle (v.Value);
                } else if (v.Tag == "rs") {
                    statusCounts = v.Value.Split (',').Skip (1).Select (x => Convert.ToInt32 (x)).ToArray ();
                } else if (v.Tag == "np") {
                    NumPasses = Convert.ToInt32 (v.Value);
                } else if (v.Tag == "RG") {
                    ReadGroup = v.Value;
                } else if (v.Tag == "zs") {
                    ZScores = v.Value.Split (',').Skip (1).Select (x => Convert.ToSingle (x)).ToArray ();
                }
            }
            // TODO: We should use String.Intern here, but not available in PCL...
            // Movie = String.Intern(s.QuerySequence.ID.Split ('/') [0]);
            Movie = s.QuerySequence.ID.Split ('/') [0];
            Sequence = s.QuerySequence as QualitativeSequence;
        }
    }
}

