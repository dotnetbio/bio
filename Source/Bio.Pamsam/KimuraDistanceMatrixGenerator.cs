using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment.Properties;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implemetation of Distance Matrix Generator class via Kimura method.
    /// 
    /// Generates a distance matrix from a set of *aligned* sequences by Kimura distance 
    /// method. See paper:
    /// MUSCLE: a multiple sequence alignment method with reduced time and space complexity. 
    /// Edgar, 2004, for details.
    /// 
    /// An additive distance measure is defined so d(A,B) = d(A,C) + d(B,C). 
    /// The mutation distance is trivially additive. Given the fractional identity D
    /// (percent identity), the mutation distance can be approximated as 1-D.
    /// As sequences diverge, there is an increasing probability of multiple mutations
    /// at a single site. Kimura correction is used here to correct this problem.
    /// 
    /// The generator is an O(N^2) algorithm. In the double loop, the generator calculates
    /// percent identity of each pair, and converts it to kimura distance score. These two 
    /// functions are defined in class KimuraDistanceScoreCalculator.
    /// 
    /// The distances will be stored in a symmetric square distance matrix, where rows and 
    /// cols are sequences, and elements are distance scores. The distance matrix is then 
    /// used to generate binary guide tree by hierarchical clustering method.
    /// </summary>
    public class KimuraDistanceMatrixGenerator : IDistanceMatrixGenerator
    {
        #region Field

        // The distance matrix generated
        private IDistanceMatrix _distanceMatrix;

        // The score calculator class
        private KimuraDistanceScoreCalculator _kimuraDistanceScoreCalculator;
        #endregion

        #region Properties

        /// <summary>
        /// The distance matrix generated in this class
        /// </summary>
        public IDistanceMatrix DistanceMatrix 
        { 
            get { return _distanceMatrix; } 
        }

        /// <summary>
        /// The method name of this class
        /// </summary>
        public string Name
        {
            get { return Resource.KimuraDistanceMatrixGeneratorName; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct DistanceMatrix via Kimura method.
        /// The two functions are defined in KimuraDistanceScoreCalculator class.
        /// </summary>
        public KimuraDistanceMatrixGenerator()
        {
            _kimuraDistanceScoreCalculator = new KimuraDistanceScoreCalculator();
        }
        #endregion

        #region Interface Methods
        /// <summary>
        /// Generate a symmetric distance matrix from a set of aligned sequences.
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        public void GenerateDistanceMatrix(IList<ISequence> sequences)
        {
            if (sequences.Count <= 0)
            {
                throw new ArgumentException("empty sequence dataset");
            }

            // Construct a symmetric distance matrix
            _distanceMatrix = new SymmetricDistanceMatrix(sequences.Count);

            // Fill in values
            Parallel.For(1, sequences.Count, PAMSAMMultipleSequenceAligner.ParallelOption, row =>
            {
                for (int col = 0; col < row; ++col)
                {
                    float distanceScore = KimuraDistanceScoreCalculator.CalculateDistanceScore(sequences[row], sequences[col]);
                    _distanceMatrix[row, col] = distanceScore;
                    _distanceMatrix[col, row] = distanceScore;
                }
            });
        }
        #endregion
    }
}
