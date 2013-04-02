namespace Bio.Algorithms.Alignment.Legacy
{
    /// <summary>
    /// Implements the NeedlemanWunsch algorithm for global alignment.
    /// See Chapter 2 in Biological Sequence Analysis; Durbin, Eddy, Krogh and Mitchison; 
    /// Cambridge Press; 1998.
    /// </summary>
    public class LegacyNeedlemanWunschAligner : DynamicProgrammingPairwiseAligner
    {
        /// <summary>
        /// Gets the name of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns the Name of our algorithm i.e 
        /// Needleman-Wunsch algorithm.
        /// </summary>
        public override string Name
        {
            get
            {
                return Properties.Resource.NEEDLEMAN_NAME;
            }
        }

        /// <summary>
        /// Gets the description of the NeedlemanWunsch algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns a simple description of what 
        /// NeedlemanWunschAligner class implements.
        /// </summary>
        public override string Description
        {
            get
            {
                return Properties.Resource.NEEDLEMAN_DESCRIPTION;
            }
        }

        /// <summary>
        /// Creates the Simple aligner job
        /// </summary>
        /// <param name="sequenceA">First aligned sequence</param>
        /// <param name="sequenceB">Second aligned sequence</param>
        /// <returns></returns>
        protected override DynamicProgrammingPairwiseAlignerJob CreateSimpleAlignmentJob(ISequence sequenceA, ISequence sequenceB)
        {
            return new NeedlemanWunschSimpleAlignmentJob(this.SimilarityMatrix, this.GapOpenCost, sequenceA, sequenceB);
        }

        /// <summary>
        /// Creates the Affine aligner job
        /// </summary>
        /// <param name="sequenceA">First aligned sequence</param>
        /// <param name="sequenceB">Second aligned sequence</param>
        /// <returns></returns>
        protected override DynamicProgrammingPairwiseAlignerJob CreateAffineAlignmentJob(ISequence sequenceA, ISequence sequenceB)
        {
            return new NeedlemanWunschAffineAlignmentJob(this.SimilarityMatrix, this.GapOpenCost, this.GapExtensionCost, sequenceA, sequenceB);
        }
    }
}
