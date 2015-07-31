using System;
using System.Collections.Generic;
using Bio.SimilarityMatrices;


namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Profile-profile SmithWaterman algorithm aligns two profiles.
    /// 
    /// A few modifications are made based on pairwise SW algorithm:
    /// 
    /// When constructing the table, pairwise score is calculated by
    /// profile-profile function instead of looking up similarity matrix.
    /// 
    /// profile-profile function is float, therefore the corresponding matrix
    /// data types are changed to float.
    /// 
    /// </summary>
    public class SmithWatermanProfileAlignerParallel : DynamicProgrammingProfileAlignerParallel
    {
        // SW begins traceback at cell with optimum score.  Use these variables
        // to track this in FillCell overrides.
        private int _optScoreCol = int.MinValue;
        private int _optScoreRow = int.MinValue;
        // modified, convert int to float
        private float _optScore = float.MinValue;

        /// <summary>
        /// Constructor for SmithWatermanProfile Aligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        public SmithWatermanProfileAlignerParallel()
            : base()
        {
        }

        /// <summary>
        /// Constructor for SmithWatermanProfileAligner Aligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileScoreFunctionName">enum: profileScoreFunctionName</param>
        /// <param name="gapOpenPenalty">negative integer</param>
        /// <param name="gapExtensionPenalty">negative integer</param>
        /// <param name="numberOfPartitions">positive integer</param>
        public SmithWatermanProfileAlignerParallel(SimilarityMatrix similarityMatrix,
                                        ProfileScoreFunctionNames profileScoreFunctionName,
                                        int gapOpenPenalty,
                                        int gapExtensionPenalty,
                                        int numberOfPartitions)
            : base(similarityMatrix, profileScoreFunctionName, gapOpenPenalty, gapExtensionPenalty, numberOfPartitions)
        {
        }

        /// <summary>
        /// Fills matrix cell specifically for SmithWaterman
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        override protected void FillCellSimple(int col, int row)
        {
            // convert int to float
            float score = SetCellValuesSimple(col, row);

            // SmithWaterman does not use negative scores, instead, if score is <0
            // set scores to 0 and stop the alignment at that point.
            if (score < 0)
            {
                _FScore[col, row] = 0;
                _FSource[col, row] = SourceDirection.Stop;
            }

            // SmithWaterman traceback begins at cell with optimum score, save it here.
            if (score >= _optScore)
            {
                _optScore = score;
                _optScoreCol = col;
                _optScoreRow = row;
            }
        }

        /// <summary>
        /// Fills matrix cell specifically for SmithWaterman
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        override protected void FillCellAffine(int col, int row)
        {
            // convert int to float
            float score = SetCellValuesAffine(col, row);

            // SmithWaterman does not use negative scores, instead, if score is < 0
            // set score to 0 and stop the alignment at that point.
            if (score < 0)
            {
                score = 0;
                _M[col, row] = 0;
                _FSource[col, row] = SourceDirection.Stop;
            }

            // SmithWaterman traceback begins at cell with optimum score, save it here.
            if (score >= _optScore)
            {
                _optScore = score;
                _optScoreCol = col;
                _optScoreRow = row;
            }
        }

        /// <summary>
        /// Sets F matrix boundary conditions for SmithWaterman partial alignment.
        /// Uses one gap penalty.
        /// </summary>
        override protected void SetBoundaryConditionSimple()
        {
            // Fill the 0 row and 0 column with 0 for sw
            int col, row;

            for (col = 0; col < _nCols; col++)
            {
                _FScore[col, 0] = 0;
                _FSource[col, 0] = SourceDirection.Stop; // no source for cells with 0
            }
            for (row = 0; row < _nRows; row++)
            {
                _FScore[0, row] = 0;
                _FSource[0, row] = SourceDirection.Stop; // no source for cells with 0

            }
            // Optimum score can be on a boundary.
            // These all have the same score, 0.
            // Pick first row, last col arbitrarily.  There is no prefered optimum score cell.
            _optScore = 0;
            _optScoreCol = _nCols - 1;
            _optScoreRow = 0;

        }

        /// <summary>
        /// Sets matrix boundary conditions for SmithWaterman partial alignment.
        /// Uses affine gap penalty.
        /// </summary>
        override protected void SetBoundaryConditionAffine()
        {
            // Fill the 0 row and 0 column with 0 for sw
            int col, row;

            for (col = 0; col < _nCols; col++)
            {
                _M[col, 0] = 0;
                _Ix[col, 0] = float.MinValue / 2;  // should be -infinity, this value avoids underflow problems
                _Iy[col, 0] = float.MinValue / 2;

                _FSource[col, 0] = SourceDirection.Stop; // no source for cells with 0
            }
            for (row = 0; row < _nRows; row++)
            {
                _M[0, row] = 0;
                _Ix[0, row] = float.MinValue / 2;
                _Iy[0, row] = float.MinValue / 2;

                _FSource[0, row] = SourceDirection.Stop; // no source for cells with 0

            }
            // Optimum score can be on a boundary.
            // These all have the same score, 0.
            // Pick first row, last col arbitrarily.  There is no prefered optimum score cell.
            _optScore = 0;
            _optScoreCol = _nCols - 1;
            _optScoreRow = 0;

        }

        /// <summary>
        /// Resets the members used to track optimum score and cell.
        /// </summary>
        override protected void ResetSpecificAlgorithmMemberVariables()
        {
            _optScoreCol = int.MinValue;
            _optScoreRow = int.MinValue;
            _optScore = float.MinValue;
        }

        /// <summary>
        /// Performs traceback for SmithWaterman partial alignment.
        /// </summary>
        /// <param name="aAlignedOut">Aligned sequences.</param>
        /// <param name="bAlignedOut"></param>
        /// <returns>Optimum score.</returns>
        override protected float Traceback(out int[] aAlignedOut, out int[] bAlignedOut)
        {
            // need an array we can extend if necessary
            int guessLen = Math.Max(_a.Length, _b.Length);
            List<int> aAligned = new List<int>(guessLen);
            List<int> bAligned = new List<int>(guessLen);
            // aligned array will be backwards, may be longer then original sequence due to gaps

            int col, row;

            // Start at the optimum element of F and work backwards
            col = _optScoreCol;
            row = _optScoreRow;
            bool done = false;
            while (!done)
            {
                if (_FSource[col, row] == SourceDirection.Diagonal)
                {
                    // Diagonal, Aligned
                    aAligned.Add(_a[col - 1]);
                    bAligned.Add(_b[row - 1]);
                    col = col - 1;
                    row = row - 1;
                }
                else if (_FSource[col, row] == SourceDirection.Up)
                {
                    // up, gap in A
                    aAligned.Add(_gapCode);
                    bAligned.Add(_b[row - 1]);
                    row = row - 1;
                }
                else if (_FSource[col, row] == SourceDirection.Left)
                {
                    // left, gap in B
                    aAligned.Add(_a[col - 1]);
                    bAligned.Add(_gapCode);
                    col = col - 1;
                }
                else
                {
                    // error condition, should never see this
                    throw new Exception("SmithWaterman Traceback error");
                }

                // if next cell has score 0, we're done
                //if (_FScore[col, row] <= 0) done = true;
                // If next cell has stop flag set, we're done
                if (_FSource[col, row] == SourceDirection.Stop) done = true;
            }

            // prepare solution, copy diagnostic data, turn aligned sequences around, etc
            // Be nice, turn aligned solutions around so that they match the input sequences
            int i, j; // utility indices used to invert aligned sequences
            int len = aAligned.Count;
            aAlignedOut = new int[len];
            bAlignedOut = new int[len];
            for (i = 0, j = len - 1; i < len; i++, j--)
            {
                aAlignedOut[i] = aAligned[j];
                bAlignedOut[i] = bAligned[j];
            }
            return _optScore;
        }
    }
}
