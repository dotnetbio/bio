namespace BiodexExcel.Visualizations.Common
{
    #region -- User Directive --

    using System;
    using System.Collections.Generic;
    using Bio;
    using Bio.Algorithms.Alignment;

    #endregion -- User Directive --

    /// <summary>
    /// AssemblyInputEventArgs defines input data for assembly algorithm.
    /// The input data will be the list of sequences and the algorithm.
    /// </summary>
    public class AssemblyInputEventArgs : EventArgs
    {
        /// <summary>
        /// Stores the list of sequences which has to be assembled..
        /// </summary>
        private List<ISequence> sequences;

        /// <summary>
        /// Initializes a new instance of the AssemblyInputEventArgs class.
        /// </summary>
        /// <param name="sequences">The list of sequences which has to be assembled.</param>
        /// <param name="selectedAligner">Aligner selected</param>
        public AssemblyInputEventArgs(List<ISequence> sequences, ISequenceAligner selectedAligner)
        {
            this.sequences = sequences;
            this.Aligner = selectedAligner;
        }

        /// <summary>
        /// Gets the aligner to be used
        /// </summary>
        public ISequenceAligner Aligner { get; private set; }

        /// <summary>
        /// Gets the list of sequences which has to be assembled.
        /// </summary>
        public List<ISequence> Sequences
        {
            get
            {
                return this.sequences;
            }
        }

        /// <summary>
        /// Gets or sets the match score.
        /// </summary>
        public int MatchScore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Mismatch score.
        /// </summary>
        public int MismatchScore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Merge Threshold.
        /// </summary>
        public double MergeThreshold
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the consensus Threshold.
        /// </summary>
        public double ConsensusThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the aligner input argument
        /// </summary>
        public AlignerInputEventArgs AlignerInput { get; set; }
    }
}
