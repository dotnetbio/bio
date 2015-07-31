using System.Collections.Generic;
using SM = Bio.SimilarityMatrices.SimilarityMatrix;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// This class implements IAlignmentAttributes interface and defines all the 
    /// parameters required to run any pairwise algorithm.
    /// </summary>
    public class PairwiseAlignmentAttributes : IAlignmentAttributes
    {
        /// <summary>
        /// Describes matrix that determines the score for any possible pair
        /// of symbols
        /// </summary>
        public const string SimilarityMatrix = "SIMILARITYMATRIX";

        /// <summary>
        /// Describes cost of inserting a gap character into a sequence.
        /// </summary>
        public const string GapOpenCost = "GAPOPENCOST";

        /// <summary>
        /// Describes cost of extending an already existing gap.
        /// </summary>
        public const string GapExtensionCost = "GAPEXTENSIONCOST";

        /// <summary>
        /// List of Parameters required to run NUCmer
        /// </summary>
        private readonly Dictionary<string, AlignmentInfo> attributes;

        /// <summary>
        /// Initializes a new instance of the PairwiseAlignmentAttributes class.
        /// </summary>
        public PairwiseAlignmentAttributes()
        {
            attributes = new Dictionary<string, AlignmentInfo>();

            StringListValidator similarityMatrixList = new StringListValidator(
                Properties.Resource.SimilarityMatrix_DiagonalSM,
                SM.StandardSimilarityMatrix.AmbiguousDna.ToString(),
                SM.StandardSimilarityMatrix.AmbiguousRna.ToString(),
                SM.StandardSimilarityMatrix.Blosum45.ToString(),
                SM.StandardSimilarityMatrix.Blosum50.ToString(),
                SM.StandardSimilarityMatrix.Blosum62.ToString(),
                SM.StandardSimilarityMatrix.Blosum80.ToString(),
                SM.StandardSimilarityMatrix.Blosum90.ToString(),
                SM.StandardSimilarityMatrix.DiagonalScoreMatrix.ToString(),
                SM.StandardSimilarityMatrix.Pam250.ToString(),
                SM.StandardSimilarityMatrix.Pam30.ToString(),
                SM.StandardSimilarityMatrix.Pam70.ToString());

            AlignmentInfo alignmentAttribute = new AlignmentInfo(
                Properties.Resource.SIMILARITY_MATRIX_NAME,
                Properties.Resource.SIMILARITY_MATRIX_DESCRIPTION,
                true,
                SM.StandardSimilarityMatrix.AmbiguousDna.ToString(),
                AlignmentInfo.StringListType,
                similarityMatrixList);
            attributes.Add(SimilarityMatrix, alignmentAttribute);
            
            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.GAP_COST_NAME,
                    Properties.Resource.GAP_COST_DESCRIPTION,
                    true,
                    "-10",
                    AlignmentInfo.IntType,
                    null);
            attributes.Add(GapOpenCost, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.GAP_EXTENSION_COST_NAME,
                    Properties.Resource.GAP_EXTENSION_COST_DESCRIPTION,
                    true,
                    "-8",
                    AlignmentInfo.IntType,
                    null);
            attributes.Add(GapExtensionCost, alignmentAttribute);
        }

        /// <summary>
        /// Gets list of attributes
        /// </summary>
        public Dictionary<string, AlignmentInfo> Attributes
        {
            get { return attributes; }
        }
    }
}
