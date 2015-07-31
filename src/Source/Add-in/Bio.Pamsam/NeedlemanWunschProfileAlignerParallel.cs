using System;
using System.Collections.Generic;
using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Profile-profile Needleman-Wunsch algorithm aligns two profiles with NW algorithm.
    /// 
    /// A few modifications are made based on pairwise NW algorithm:
    /// 
    /// When constructing the table, pairwise score is calculated by
    /// profile-profile function instead of looking up similarity matrix.
    /// 
    /// Profile-profile function is float, therefore the corresponding matrix
    /// data types are changed to float.
    /// 
    /// Details can be found in NeedlemanWunschAligner class
    /// 
    /// </summary>
    public class NeedlemanWunschProfileAlignerParallel : DynamicProgrammingProfileAlignerParallel
    {

        // change to float
        private float _optScore = float.MinValue;

        /// <summary>
        /// Constructor for NeedlemanWunschProfileAligner Aligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        public NeedlemanWunschProfileAlignerParallel()
            : base()
        {
        }

        /// <summary>
        /// Constructor for NeedlemanWunschProfile Aligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileScoreFunctionName">enum: profileScoreFunctionName</param>
        /// <param name="gapOpenPenalty">negative integer</param>
        /// <param name="gapExtensionPenalty">negative integer</param>
        /// <param name="numberOfPartitions">positive integer</param>
        public NeedlemanWunschProfileAlignerParallel(SimilarityMatrix similarityMatrix,
                                        ProfileScoreFunctionNames profileScoreFunctionName,
                                        int gapOpenPenalty,
                                        int gapExtensionPenalty,
                                        int numberOfPartitions)
            : base(similarityMatrix, profileScoreFunctionName, gapOpenPenalty, gapExtensionPenalty, numberOfPartitions)
        {
        }

        /// <summary>
        /// Fills matrix cell specifically for NeedlemanWunsch
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        override protected void FillCellSimple(int col, int row)
        {
            float score = SetCellValuesSimple(col, row);
            // Traceback starts at lower right corner.
            // Save the score from this point.
            if (col == _nCols - 1 && row == _nRows - 1)
            {
                _optScore = score;
            }
        }

        /// <summary>
        /// Fills matrix cell specifically for NeedlemanWunsch
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        override protected void FillCellAffine(int col, int row)
        {
            float score = SetCellValuesAffine(col, row);
            // Traceback starts at lower right corner.
            // Save the score from this point.
            if (col == _nCols - 1 && row == _nRows - 1)
            {
                _optScore = score;
            }
        }

        /// <summary>
        /// Sets F matrix boundary conditions for NeedlemanWunsch global alignment.
        /// Uses one gap penalty.
        /// </summary>
        override protected void SetBoundaryConditionSimple()
        {
            // Fill the 0 row and 0 column with i*_gapOpenPenalty for NW.
            int col, row;
            int colLen = _FScore.GetLength(0);
            int rowLen = _FScore.GetLength(1);
            for (col = 0; col < colLen; col++)
            {
                _FScore[col, 0] = col * _gapOpenPenalty;
                _FSource[col, 0] = SourceDirection.Left;
            }
            for (row = 0; row < rowLen; row++)
            {
                _FScore[0, row] = row * _gapOpenPenalty;
                _FSource[0, row] = SourceDirection.Up;

            }
        }

        /// <summary>
        /// Sets matrix boundary conditions for NeedlemanWunsch global alignment.
        /// Uses affine gap penalty.
        /// </summary>
        override protected void SetBoundaryConditionAffine()
        {
            // Fill the 0 row and 0 column with 0 for sw
            int col, row;
            int colLen = _FSource.GetLength(0);
            int rowLen = _FSource.GetLength(1);

            // Note that the BC for Ix and Iy are usually set to -infinity or for integers, int.MinValue.
            // This can lead to underflow when filling the first row or col past the boundary, so use int.MinValue/2.
            // This is still effectively -infinity but avoids underflow.
            for (col = 0; col < colLen; col++)
            {
                //int valueBC = col * _gapOpenPenalty;
                // Terminal gaps have half penalties as internal gaps.
                int valueBC;
                if (col == 0)
                {
                    valueBC = 0;
                }
                else
                {
                    valueBC = (_gapOpenPenalty / 2) + (_gapExtensionPenalty / 2) * (col - 1);
                }
                _M[col, 0] = valueBC;
                _Ix[col, 0] = int.MinValue / 2;
                _Iy[col, 0] = int.MinValue / 2;

                _FSource[col, 0] = SourceDirection.Left; // no source for cells with 0
            }
            for (row = 0; row < rowLen; row++)
            {
                //int valueBC = row * _gapOpenPenalty;
                // Terminal gaps have half penalties as internal gaps.
                int valueBC;
                if (row == 0)
                {
                    valueBC = 0;
                }
                else
                {
                    valueBC = (_gapOpenPenalty / 2) + (_gapExtensionPenalty / 2) * (row - 1);
                }
                _M[0, row] = valueBC;
                _Ix[0, row] = int.MinValue / 2;
                _Iy[0, row] = int.MinValue / 2;

                _FSource[0, row] = SourceDirection.Up; // no source for cells with 0

            }
        }

        /// <summary>
        /// Resets the members used to track optimum score and cell.
        /// </summary>
        override protected void ResetSpecificAlgorithmMemberVariables()
        {
            // Not strictly necessary since this will be set in the FillCell methods, 
            // but it is good practice to initialize correctly.
            _optScore = int.MinValue;
        }


        /// <summary>
        /// Performs traceback for global alignment.
        /// </summary>
        /// <param name="aAlignedOut">Aligned sequences.</param>
        /// <param name="bAlignedOut"></param>
        /// <returns>Optimum score.</returns>
        override protected float Traceback(out int[] aAlignedOut, out int[] bAlignedOut)
        {
            // For NW, aligned sequence will be at least as long as longest input sequence.
            // May be longer if there are gaps in both aligned sequences.
            int guessLen = Math.Max(_a.Length, _b.Length);
            List<int> aAligned = new List<int>(guessLen);
            List<int> bAligned = new List<int>(guessLen);

            // Start at the bottom left element of F and work backwards until we get to upper left
            int col, row;
            col = _nCols - 1;
            row = _nRows - 1;
            while (col > 0 || row > 0) // stop when col and row are both zero
            {
                if (_FSource[col, row] == SourceDirection.Diagonal)
                {
                    // diagonal, no gap, use both sequence residues
                    aAligned.Add(_a[col - 1]);
                    bAligned.Add(_b[row - 1]);
                    col = col - 1;
                    row = row - 1;
                }
                else if (_FSource[col, row] == SourceDirection.Up)
                {
                    // up, gap in a
                    aAligned.Add(_gapCode);
                    bAligned.Add(_b[row - 1]);
                    row = row - 1;
                }
                else if (_FSource[col, row] == SourceDirection.Left)
                {
                    // left, gap in b
                    aAligned.Add(_a[col - 1]);
                    bAligned.Add(_gapCode);
                    col = col - 1;
                }
                else
                {
                    throw new Exception("NeedlemanWunsch, bad source in traceback.");
                }
            }

            // Prepare solution, copy diagnostic data, turn aligned sequences around, etc
            // Be nice, turn aligned solutions around so that they match the input sequences
            int i, j; // utility indices used to reverse aligned sequences
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
