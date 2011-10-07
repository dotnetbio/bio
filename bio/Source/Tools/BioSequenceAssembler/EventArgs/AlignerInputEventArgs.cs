namespace SequenceAssembler
{
    using System;
    using System.Collections.Generic;
    using Bio;
    using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
    using Bio.SimilarityMatrices;

    /// <summary>
    /// Defines list of input parameter for alignment algorithm
    /// </summary>
    public class AlignerInputEventArgs : EventArgs
    {
        public string SelectedAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets minimum length of Maximal Unique Match
        /// </summary>
        public int LengthOfMUM { get; set; }

        /// <summary>
        /// Gets or sets maximum fixed diagonal difference
        /// </summary>
        public int FixedSeparation { get; set; }

        /// <summary>
        /// Gets or sets maximum seperation between the adjacent matches in clusters
        /// </summary>
        public int MaximumSeparation { get; set; }

        /// <summary>
        /// Gets or sets minimum output score
        /// </summary>
        public int MinimumScore { get; set; }

        /// <summary>
        /// Gets or sets the gap cost.
        /// </summary>
        public int GapCost { get; set; }

        /// <summary>
        /// Gets or sets cost of extending an already existing gap
        /// </summary>
        public int GapExtensionCost { get; set; }

        /// <summary>
        /// Gets or sets seperation factor. Fraction equal to 
        /// (diagonal difference / match seperation) where higher values
        /// increase the indel tolerance
        /// </summary>
        public float SeparationFactor { get; set; }

        /// <summary>
        /// Gets or sets number of bases to be extended before stopping alignment
        /// </summary>
        public int BreakLength { get; set; }

        /// <summary>
        /// Gets or sets matrix that determines the score for any possible pair
        /// of symbols
        /// </summary>
        public SimilarityMatrix SimilarityMatrix { get; set; }

        /// <summary>
        /// List of sequences to be aligned
        /// </summary>
        public IList<ISequence> SequencesToAlign { get; set; }

        #region PAMSAM parameters

        public int KmerLength;
        public DistanceFunctionTypes DistanceFunctionName;
        public UpdateDistanceMethodsTypes UpdateDistanceMethodsType;
        public ProfileAlignerNames ProfileAlignerName;
        public ProfileScoreFunctionNames ProfileScoreFunctionName;
        public int GapOpenPenalty;
        public int GapExtendPenalty;
        public int NumberOfPartitions;
        public int DegreeOfParallelism;

        #endregion
    }
}
