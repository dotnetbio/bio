using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// The delegate function of profile-profile score function.
    /// The function takes two profile vectors, and calculate the distance score.
    /// 
    /// The two profile vectors should be normalized (by definition) and same in size.
    /// </summary>
    /// <param name="similarityMatrix">similarity matrix</param>
    /// <param name="profileIndexA">the first profile vector (normalized)</param>
    /// <param name="profileIndexB">the second profile vector (normalized)</param>
    public delegate float ProfileScoreFunctionSelector(
                SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB);

    /// <summary>
    /// The delegate function of cashing function.
    /// </summary>
    /// <param name="similarityMatrix">similarity matrix</param>
    /// <param name="profileAlignmentA">profile alignment</param>
    /// <param name="profileAlignmentB">profile alignment</param>
    public delegate void CachingFunctionSelector(
                SimilarityMatrix similarityMatrix, IProfileAlignment profileAlignmentA, IProfileAlignment profileAlignmentB);

    /// <summary>
    /// The enum of profile-profile score function
    /// a complete list can be found from paper:
    /// A comprarison of scoring functions for protein sequence
    /// profile alignment, Edgar 2003.
    /// </summary>
    public enum ProfileScoreFunctionNames
    {
        /// <summary>
        /// Inner-profuct of two observed profile vectors
        /// </summary>
        InnerProduct,

        /// <summary>
        /// Fast Version Inner-profuct of two observed profile vectors
        /// </summary>
        InnerProductFast,

        /// <summary>
        /// Weighted inner-profuct by similarity matrix
        /// </summary>
        WeightedInnerProduct,

        /// <summary>
        /// Fast Version Weighted inner-profuct by similarity matrix
        /// </summary>
        WeightedInnerProductFast,

        /// <summary>
        /// Cached Version Weighted inner-profuct by similarity matrix
        /// </summary>
        WeightedInnerProductCached,

        /// <summary>
        /// Weighted inner-profuct by shifted similarity matrix
        /// </summary>
        WeightedInnerProductShifted,

        /// <summary>
        /// Fast Version Weighted inner-profuct by shifted similarity matrix
        /// </summary>
        WeightedInnerProductShiftedFast,

        /// <summary>
        /// Correlation of observation vectors
        /// </summary>
        PearsonCorrelation,

        /// <summary>
        /// Weighted Euclidean distance of observation vectors
        /// </summary>
        WeightedEuclideanDistance,

        /// <summary>
        /// Fast Version Weighted Euclidean distance of observation vectors
        /// </summary>
        WeightedEuclideanDistanceFast,

        /// <summary>
        /// Log of Weighted inner-product by exponential of similarity matrix
        /// </summary>
        LogExponentialInnerProduct,

        /// <summary>
        /// Fast Version Log of Weighted inner-product by exponential of similarity matrix
        /// </summary>
        LogExponentialInnerProductFast,

        /// <summary>
        /// Log of Weighted inner-product by exponential of shifted similarity matrix
        /// </summary>
        LogExponentialInnerProductShifted,

        /// <summary>
        /// Fast Version Log of Weighted inner-product by exponential of shifted similarity matrix
        /// </summary>
        LogExponentialInnerProductShiftedFast,

        /// <summary>
        /// Symmetrized entropy of observation vectors
        /// </summary>
        SymmetrizedEntropy,

        /// <summary>
        /// Jensen-Shannon divergence of observation vectors
        /// </summary>
        JensenShannonDivergence
    }
}
