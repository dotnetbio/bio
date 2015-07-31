using System;
using System.Collections.Generic;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implements the SmithWaterman algorithm for partial alignment.
    /// See Chapter 2 in Biological Sequence Analysis; Durbin, Eddy, Krogh and Mitchison; 
    /// Cambridge Press; 1998.
    /// </summary>
    public class SmithWatermanProfileAlignerSerial : DynamicProgrammingProfileAlignerSerial
    {
        // SW begins traceback at cell with optimum score.  Use these variables
        // to track this in FillCell overrides.

        /// <summary>
        /// Column number of cell with optimal score
        /// </summary>
        private int _optScoreCol = int.MinValue;

        /// <summary>
        /// Row number of cell with optimal score
        /// </summary>
        private int _optScoreRow = int.MinValue;

        /// <summary>
        /// Cell number of cell with optimal score
        /// </summary>
        private int _optScoreCell = int.MinValue;

        /// <summary>
        /// Tracks optimal score for alignment
        /// </summary>
        private float _optScore = float.MinValue;

        /// <summary>
        /// Constructor for SmithWatermanProfileAligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        public SmithWatermanProfileAlignerSerial()
            : base()
        {
        }

        /// <summary>
        /// Constructor for SmithWatermanProfileAligner.
        /// Sets default similarity matrix, gap penalties, and profile function name.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileScoreFunctionName">enum: profileScoreFunctionName</param>
        /// <param name="gapOpenPenalty">negative integer</param>
        /// <param name="gapExtensionPenalty">negative integer</param>
        /// <param name="numberOfPartitions">positive integer</param>
        public SmithWatermanProfileAlignerSerial(SimilarityMatrix similarityMatrix,
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
        /// <param name="cell">cell number</param>
        protected override void FillCellSimple(int col, int row, int cell)
        {
            float score = SetCellValuesSimple(col, row, cell);

            // SmithWaterman does not use negative scores, instead, if score is <0
            // set scores to 0 and stop the alignment at that point.
            if (score < 0)
            {
                score = 0;
                _FSource[cell] = SourceDirection.Stop;
            }

            _FScore[row] = score;

            // SmithWaterman traceback begins at cell with optimum score, save it here.
            if (score >= _optScore)
            {
                _optScore = score;
                _optScoreCol = col;
                _optScoreRow = row;
                _optScoreCell = cell;
            }
        }

        /// <summary>
        /// Fills matrix cell specifically for SmithWaterman
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        /// <param name="cell">cell number</param>
        protected override void FillCellAffine(int col, int row, int cell)
        {
            float score = SetCellValuesAffine(col, row, cell);

            // SmithWaterman does not use negative scores, instead, if score is < 0
            // set score to 0 and stop the alignment at that point.
            if (score < 0)
            {
                score = 0;
                _FSource[cell] = SourceDirection.Stop;
            }

            _MaxScore[row] = score;

            // SmithWaterman traceback begins at cell with optimum score, save it here.
            if (score >= _optScore)
            {
                _optScore = score;
                _optScoreCol = col;
                _optScoreRow = row;
                _optScoreCell = cell;
            }
        }

        /// <summary>
        /// Sets F matrix boundary conditions for first row in SmithWaterman alignment.
        /// Uses one gap penalty.
        /// </summary>
        protected override void SetRowBoundaryConditionSimple()
        {
            for (int row = 0; row < _nRows; row++)
            {
                _FScore[row] = 0;
                _FSource[row] = SourceDirection.Stop; // no source for cells with 0
            }

            // Optimum score can be on a boundary.
            // These all have the same score, 0.
            // Pick first col, last row arbitrarily.  There is no prefered optimum score cell.
            _optScore = 0;
            _optScoreCol = 0;
            _optScoreRow = _nRows - 1;
            _optScoreCell = _nRows - 1;
        }

        /// <summary>
        /// Sets F matrix boundary conditions for first column in SmithWaterman alignment.
        /// Uses one gap penalty.
        /// </summary>
        /// <param name="col">Column number of cell</param>
        /// <param name="cell">cell number</param>
        protected override void SetColumnBoundaryConditionSimple(int col, int cell)
        {
            _FScoreDiagonal = _FScore[0];
            _FSource[cell] = SourceDirection.Stop; // no source for cells with 0
        }

        /// <summary>
        /// Sets matrix boundary conditions for first row in SmithWaterman alignment.
        /// Uses affine gap penalty.
        /// </summary>
        protected override void SetRowBoundaryConditionAffine()
        {
            for (int row = 0; row < _nRows; row++)
            {
                _IxGapScore[row] = float.MinValue / 2;
                _MaxScore[row] = 0;
                _FSource[row] = SourceDirection.Stop; // no source for cells with 0
            }

            // Optimum score can be on a boundary.
            // These all have the same score, 0.
            // Pick first col, last row arbitrarily.  There is no prefered optimum score cell.
            _optScore = 0;
            _optScoreCol = 0;
            _optScoreRow = _nRows - 1;
            _optScoreCell = _nRows - 1;
        }

        /// <summary>
        /// Sets matrix boundary conditions for first row in SmithWaterman alignment.
        /// Uses affine gap penalty.
        /// </summary>
        /// <param name="col">Column number of cell</param>
        /// <param name="cell">cell number</param>
        protected override void SetColumnBoundaryConditionAffine(int col, int cell)
        {
            _IyGapScore = float.MinValue / 2;
            _MaxScoreDiagonal = _MaxScore[0];
            _FSource[cell] = SourceDirection.Stop; // no source for cells with 0
        }

        /// <summary>
        /// Resets the members used to track optimum score and cell.
        /// </summary>
        protected override void ResetSpecificAlgorithmMemberVariables()
        {
            _optScoreCol = int.MinValue;
            _optScoreRow = int.MinValue;
            _optScoreCell = int.MinValue;
            _optScore = int.MinValue;
        }

        /// <summary>
        /// Optimal score updated in FillCellSimple. 
        /// So nothing to be done here
        /// </summary>
        protected override void SetOptimalScoreSimple() { }

        /// <summary>
        /// Optimal score updated in FillCellAffine. 
        /// So nothing to be done here
        /// </summary>
        protected override void SetOptimalScoreAffine() { }

        /// <summary>
        /// Performs traceback for SmithWaterman partial alignment.
        /// </summary>
        /// <param name="aAligned">First aligned sequence</param>
        /// <param name="bAligned">Second aligned sequence</param>
        /// <returns>Optimum score.</returns>
        protected override float Traceback(out int[] aAligned, out int[] bAligned)
        {
            // need an array we can extend if necessary
            // aligned array will be backwards, may be longer than original sequence due to gaps
            int guessLen = Math.Max(_a.Length, _b.Length);
            List<int> aAlignedList = new List<int>(guessLen);
            List<int> bAlignedList = new List<int>(guessLen);

            int col, row, cell;

            // Start at the optimum element of F and work backwards
            col = _optScoreCol;
            row = _optScoreRow;
            cell = _optScoreCell;

            bool done = false;
            while (!done)
            {
                // if next cell has score 0, we're done
                switch (_FSource[cell])
                {
                    case SourceDirection.Stop:
                        {
                            done = true;
                            break;
                        }

                    case SourceDirection.Diagonal:
                        {
                            // Diagonal, Aligned
                            aAlignedList.Add(_a[col - 1]);
                            bAlignedList.Add(_b[row - 1]);
                            col = col - 1;
                            row = row - 1;
                            cell = cell - _nRows - 1;
                            break;
                        }

                    case SourceDirection.Up:
                        {
                            // up, gap in A
                            aAlignedList.Add(_gapCode);
                            bAlignedList.Add(_b[row - 1]);
                            row = row - 1;
                            cell = cell - 1;
                            break;
                        }

                    case SourceDirection.Left:
                        {
                            // left, gap in B
                            aAlignedList.Add(_a[col - 1]);
                            bAlignedList.Add(_gapCode);
                            col = col - 1;
                            cell = cell - _nRows;
                            break;
                        }

                    default:
                        {
                            // error condition, should never see this
                            string message = "SmithWaterman Traceback error.";
                            Trace.Report(message);
                            throw new Exception(message);
                        }
                }
            }

            // prepare solution, copy diagnostic data, turn aligned sequences around, etc
            // Be nice, turn aligned solutions around so that they match the input sequences
            int i, j; // utility indices used to invert aligned sequences
            int len = aAlignedList.Count;
            aAligned = new int[len];
            bAligned = new int[len];
            for (i = 0, j = len - 1; i < len; i++, j--)
            {
                aAligned[i] = aAlignedList[j];
                bAligned[i] = bAlignedList[j];
            }

            return _optScore;
        }
    }
}
