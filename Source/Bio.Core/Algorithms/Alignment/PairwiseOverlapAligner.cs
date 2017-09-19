namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Implements the pair-wise overlap alignment algorithm described in Chapter 2 of
    /// Biological Sequence Analysis; Durbin, Eddy, Krogh and Mitchison; Cambridge Press; 1998.
    /// </summary>
    public class PairwiseOverlapAligner : DynamicProgrammingPairwiseAligner
    {
        /// <summary>
        /// Gets the name of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns the Name of our algorithm i.e 
        /// pair-wise-Overlap algorithm.
        /// </summary>
        public override string Name
        {
            get { return Properties.Resource.PAIRWISE_NAME; }
        }

        /// <summary>
        /// Gets the description of the pair-wise-Overlap algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns a simple description of what 
        /// PairwiseOverlapAligner class implements.
        /// </summary>
        public override string Description
        {
            get { return Properties.Resource.PAIRWISE_DESCRIPTION; }
        }

        /// <summary>
        /// Creates the Simple aligner job
        /// </summary>
        /// <param name="sequenceA">First aligned sequence</param>
        /// <param name="sequenceB">Second aligned sequence</param>
        /// <returns></returns>
        protected override DynamicProgrammingPairwiseAlignerJob CreateSimpleAlignmentJob(ISequence sequenceA, ISequence sequenceB)
        {
            return new PairwiseOverlapSimpleAlignmentJob(this.SimilarityMatrix, this.GapOpenCost, sequenceA, sequenceB);
        }

        /// <summary>
        /// Creates the Affine aligner job
        /// </summary>
        /// <param name="sequenceA">First aligned sequence</param>
        /// <param name="sequenceB">Second aligned sequence</param>
        /// <returns></returns>
        protected override DynamicProgrammingPairwiseAlignerJob CreateAffineAlignmentJob(ISequence sequenceA, ISequence sequenceB)
        {
            return new PairwiseOverlapAffineAlignmentJob(this.SimilarityMatrix, this.GapOpenCost, this.GapExtensionCost, sequenceA, sequenceB);
        }
    }
}
