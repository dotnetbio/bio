using System;
using System.Collections.Generic;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;

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
    public class NeedlemanWunschProfileAlignerSerial : DynamicProgrammingProfileAlignerSerial
    {
        // change to float
        private float _optScore = float.MinValue;

        /// <summary>
        /// Constructor for NeedlemanWunschProfileAligner Aligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        public NeedlemanWunschProfileAlignerSerial()
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
        public NeedlemanWunschProfileAlignerSerial(SimilarityMatrix similarityMatrix,
                                        ProfileScoreFunctionNames profileScoreFunctionName,
                                        int gapOpenPenalty,
                                        int gapExtensionPenalty,
                                        int numberOfPartitions)
            : base(similarityMatrix, profileScoreFunctionName, gapOpenPenalty, gapExtensionPenalty, numberOfPartitions)
        {
        }

        /// <summary>
        /// Fills matrix cell specifically for NeedlemanWunsch
        /// Required because method is abstract in DynamicProgrammingPairwise
        /// To be removed once changes are made in SW, Pairwise algorithms
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        /// <param name="cell">cell number</param>
        protected override void FillCellSimple(int col, int row, int cell)
        {
            _FScore[row] = SetCellValuesSimple(col, row, cell);
        }

        /// <summary>
        /// Sets the score in last cell of _FScore to be the optimal score
        /// </summary>
        protected override void SetOptimalScoreSimple()
        {
            // Traceback starts at lower right corner.
            // Save the score from this point.
            _optScore = _FScore[_nRows - 1];
        }

        /// <summary>
        /// Sets the score in last cell of _MaxScore to be the optimal score
        /// </summary>
        protected override void SetOptimalScoreAffine()
        {
            // Traceback starts at lower right corner.
            // Save the score from this point.
            _optScore = _MaxScore[_nRows - 1];
        }

        /// <summary>
        /// Fills matrix cell specifically for NeedlemanWunsch
        /// Required because method is abstract in DynamicProgrammingPairwise
        /// To be removed once changes are made in SW, Pairwise algorithms
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        /// <param name="cell">cell number</param>
        protected override void FillCellAffine(int col, int row, int cell)
        {
            _MaxScore[row] = SetCellValuesAffine(col, row, cell);
        }

        /// <summary>
        /// Sets F matrix boundary conditions for first row in NeedlemanWunsch global alignment.
        /// Uses single gap penalty.
        /// Required because method is abstract in DynamicProgrammingPairwise
        /// To be removed once changes are made in SW, Pairwise algorithms
        /// </summary>
        protected override void SetRowBoundaryConditionSimple()
        {
            for (int row = 0; row < _nRows; row++)
            {
                _FScore[row] = row * _gapOpenPenalty;
                _FSource[row] = SourceDirection.Up;
            }

            _FScore[0] = _gapOpenPenalty;
        }

        /// <summary>
        /// Sets F matrix boundary conditions for first column in NeedlemanWunsch global alignment.
        /// Uses single gap penalty.
        /// </summary>
        /// <param name="col">Column number of cell</param>
        /// <param name="cell">cell number</param>
        protected override void SetColumnBoundaryConditionSimple(int col, int cell)
        {
            _FScore[0] = col * _gapOpenPenalty; // (col, 0) for F Matrix is set
            _FScoreDiagonal = (col - 1) * _gapOpenPenalty; // _FScoreDiagonal is set to previous row's _FScore[0]
            _FSource[cell] = SourceDirection.Left;
        }

        /// <summary>
        /// Sets matrix boundary conditions for first row in NeedlemanWunsch global alignment.
        /// Uses affine gap penalty.
        /// Required because method is abstract in DynamicProgrammingPairwise
        /// To be removed once changes are made in SW, Pairwise algorithms
        /// </summary>
        protected override void SetRowBoundaryConditionAffine()
        {
            for (int row = 0; row < _nRows; row++)
            {
                _IxGapScore[row] = int.MinValue / 2;
                if (row == 0)
                {
                    _MaxScore[row] = 0;
                }
                else
                {
                    _MaxScore[row] = (_gapOpenPenalty / 2) + (_gapExtensionPenalty / 2) * (row - 1);
                }
                //_MaxScore[row] = row * _gapOpenPenalty; // _gapOpenPenalty + (row - 1) * _gapExtensionPenalty????
                _FSource[row] = SourceDirection.Up;
            }
        }

        /// <summary>
        /// Sets matrix boundary conditions for first column in NeedlemanWunsch global alignment.
        /// Uses affine gap penalty.
        /// </summary>
        /// <param name="col">Column number of cell</param>
        /// <param name="cell">cell number</param>
        protected override void SetColumnBoundaryConditionAffine(int col, int cell)
        {
            _IyGapScore = int.MinValue / 2; // Iy set to -infinity
            _MaxScoreDiagonal = _MaxScore[0]; // stored 0th cell of previous row. 
            if (col == 0)
            {
                _MaxScore[0] = 0;
            }
            else
            {
                _MaxScore[0] = (_gapOpenPenalty / 2) + (_gapExtensionPenalty / 2) * (col - 1);
            }
            //_MaxScore[0] = col * _gapOpenPenalty; // sets (col, 0) for _MaxScore
            ////_gapOpenPenalty + (col - 1) * _gapExtensionPenalty; ????
            _FSource[cell] = SourceDirection.Left;
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
        /// <param name="aAligned">First aligned sequence</param>
        /// <param name="bAligned">Second aligned sequence</param>
        /// <returns>Optimum score.</returns>
        protected override float Traceback(out int[] aAligned, out int[] bAligned)
        {
            // For NW, aligned sequence will be at least as long as longest input sequence.
            // May be longer if there are gaps in both aligned sequences.
            int guessLen = Math.Max(_a.Length, _b.Length);
            List<int> aAlignedList = new List<int>(guessLen);
            List<int> bAlignedList = new List<int>(guessLen);

            // Start at the bottom left element of F and work backwards until we get to upper left
            int col, row, cell;
            col = _nCols - 1;
            row = _nRows - 1;
            cell = (_nCols * _nRows) - 1;

            // stop when col and row are both zero
            while (cell > 0)
            {
                switch (_FSource[cell])
                {
                    case SourceDirection.Diagonal:
                        {
                            // diagonal, no gap, use both sequence residues
                            aAlignedList.Add(_a[col - 1]);
                            bAlignedList.Add(_b[row - 1]);
                            col = col - 1;
                            row = row - 1;
                            cell = cell - _nRows - 1;
                            break;
                        }

                    case SourceDirection.Up:
                        {
                            // up, gap in a
                            aAlignedList.Add(_gapCode);
                            bAlignedList.Add(_b[row - 1]);
                            row = row - 1;
                            cell = cell - 1;
                            break;
                        }

                    case SourceDirection.Left:
                        {
                            // left, gap in b
                            aAlignedList.Add(_a[col - 1]);
                            bAlignedList.Add(_gapCode);
                            col = col - 1;
                            cell = cell - _nRows;
                            break;
                        }

                    default:
                        {
                            string message = "NeedlemanWunschAligner: Bad source in Traceback.";
                            Trace.Report(message);
                            throw new Exception(message);
                        }
                }
            }

            // Prepare solution, copy diagnostic data, turn aligned sequences around, etc
            // Be nice, turn aligned solutions around so that they match the input sequences
            int i, j; // utility indices used to reverse aligned sequences
            int len = aAlignedList.Count;
            aAligned = new int[len];
            bAligned = new int[len];
            for (i = 0, j = len - 1; i < len; i++, j--)
            {
                aAligned[i] = aAlignedList[j];
                bAligned[i] = bAlignedList[j];
            }

            return this._optScore;
        }
    }
}
