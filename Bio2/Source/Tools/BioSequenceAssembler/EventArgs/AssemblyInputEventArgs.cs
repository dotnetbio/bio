namespace SequenceAssembler
{
    #region -- User Directive --

    using System;
    using System.Collections.Generic;
    using Bio;

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
        private IList<ISequence> sequences;

        /// <summary>
        /// Alignment algorithm name.
        /// </summary>
        private string algorithm;

        /// <summary>
        /// Initializes a new instance of the AssemblyInputEventArgs class.
        /// </summary>
        /// <param name="sequences">The list of sequences which has to be assembled.</param>
        /// <param name="algorithm">Algorithm name.</param>
        public AssemblyInputEventArgs(IList<ISequence> sequences, string algorithm)
        {
            this.sequences = sequences;
            this.algorithm = algorithm;
        }

        /// <summary>
        /// Gets the name of the alignment algorithm.
        /// </summary>
        public string Algorithm
        {
            get 
            {
                return this.algorithm; 
            }
        }

        /// <summary>
        /// Gets the list of sequences which has to be assembled.
        /// </summary>
        public IList<ISequence> Sequences
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

        #region Parameters for Padena

        /// <summary>
        /// Gets or sets the kmer length
        /// </summary>
        public int KmerLength { get; set; }

        /// <summary>
        /// Gets or sets the threshold length for dangling link purger.
        /// </summary>
        public int DanglingLinksThreshold { get; set; }

        /// <summary>
        /// Gets or sets the length threshold for redundant paths purger.
        /// </summary>
        public int RedundantPathLengthThreshold { get; set; }

        /// <summary>
        /// Gets or sets the Depth for graph traversal in scaffold builder step.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets or sets value of redundancy for building scaffolds.
        /// </summary>
        public int ScaffoldRedundancy { get; set; }

        /// <summary>
        /// Gets or Sets which assembler is selected by user
        /// </summary>
        public AssemblerType AssemblerUsed { get; set; }

        /// <summary>
        /// Use scaffold generation while assembling
        /// </summary>
        public bool GenerateScaffolds { get; set; }

        /// <summary>
        /// Gets or sets whether erosion is enabled.
        /// </summary>
        public bool ErosionEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether low coverage contig removal enabled.
        /// </summary>
        public bool LowCoverageContigRemovalEnabled { get; set; }

        /// <summary>
        /// Gets or sets value of erosion threshold.
        /// </summary>
        public double ErosionThreshold { get; set; }

        /// <summary>
        /// Gets or sets value of low coverage contig removal threshold.
        /// </summary>
        public double LowCoverageContigRemovalThreshold { get; set; }

        #endregion
    }
}
