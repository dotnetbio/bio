using System.Collections.Generic;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Implements the NeedlemanWunsch algorithm for global alignment.
    /// See Chapter 2 in Biological Sequence Analysis; Durbin, Eddy, Krogh and Mitchison; 
    /// Cambridge Press; 1998.
    /// </summary>
    public sealed class NeedlemanWunschAligner : PairwiseSequenceAligner
    {
        /// <summary>
        /// Gets the name of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns the Name of our algorithm i.e 
        /// Needleman-Wunsch algorithm.
        /// </summary>
        public override string Name
        {
            get { return Properties.Resource.NEEDLEMAN_NAME; }
        }

        /// <summary>
        /// Gets the description of the NeedlemanWunsch algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns a simple description of what 
        /// NeedlemanWunschAligner class implements.
        /// </summary>
        public override string Description
        {
            get { return Properties.Resource.NEEDLEMAN_DESCRIPTION; }
        }

        /// <summary>
        /// This is step (2) in the dynamic programming model - to fill in the scoring matrix
        /// and calculate the traceback entries. 
        /// </summary>
        protected override IEnumerable<OptScoreMatrixCell> CreateTracebackTable(bool useAffineGapModel)
        {
            if (useAffineGapModel)
                return CreateAffineTracebackTable();

            for (int i = 1; i < Rows; i++)
            {
                for (int j = 1; j < Cols; j++)
                {
                    int aboveIndex = (i - 1) * Cols + j;
                    int leftIndex = i * Cols + (j - 1);
                    int diagonalIndex = (i - 1) * Cols + (j - 1);
                    int currentIndex = i * Cols + j;

                    // Gap in reference sequence
                    int scoreAbove = ScoreTable[aboveIndex] + GapOpenCost;
                    // Gap in query sequence
                    int scoreLeft = ScoreTable[leftIndex] + GapOpenCost;
                    // Match/mismatch score
                    int scoreDiag = ScoreTable[diagonalIndex] + SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];

                    // Get the max S = MAX(diag,above,left) and assign the traceback
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        ScoreTable[currentIndex] = scoreDiag;
                        Traceback[currentIndex] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        ScoreTable[currentIndex] = scoreLeft;
                        Traceback[currentIndex] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        ScoreTable[currentIndex] = scoreAbove;
                        Traceback[currentIndex] = SourceDirection.Up;
                    }
                }
            }

            // Return a single high score which is always the bottom right corner of the matrix.
            return new[]
            {
                new OptScoreMatrixCell
                {
                    Row = Rows-1, 
                    Col = Cols-1, 
                    Score = ScoreTable[ScoreTable.Length-1]
                }
            };
        }

        /// <summary>
        /// This is step (2) in the dynamic programming model - to fill in the scoring matrix
        /// and calculate the traceback entries.  This version is used when the open/extension
        /// gap costs are different (affine gap model).
        /// </summary>
        private IEnumerable<OptScoreMatrixCell> CreateAffineTracebackTable()
        {
            // Horizontal and vertical gap counts.
            int gapStride = Cols + 1;
            int[] hgapLength = new int[(Rows + 1) * gapStride];
            int[] vgapLength = new int[(Rows + 1) * gapStride];
            int[] hgapCost = new int[(Rows + 1) * gapStride];
            int[] vgapCost = new int[(Rows + 1) * gapStride];

            // Initialize the gap extension cost matrices.
            for (int i = 1; i < Rows; i++)
            {
                hgapLength[i * gapStride] = i;
                vgapLength[i * gapStride] = 1;
                hgapCost[i * gapStride] = GapExtensionCost * i;
            }
            for (int j = 1; j < Cols; j++)
            {
                hgapLength[j] = 1;
                vgapLength[j] = j;
                vgapCost[j] = GapExtensionCost * j;
            }

            // Walk the sequences and generate the scoring matrix.
            for (int i = 1; i < Rows; i++)
            {
                for (int j = 1; j < Cols; j++)
                {
                    int aboveIndex = (i - 1) * Cols + j;
                    int leftIndex = i * Cols + (j - 1);
                    int diagonalIndex = (i - 1) * Cols + (j - 1);
                    int currentIndex = i * Cols + j;

                    // Gap in reference sequence
                    int scoreAbove;
                    int scoreAboveOpen = ScoreTable[aboveIndex] + GapOpenCost;
                    int scoreAboveExtend = vgapCost[(i-1) * gapStride + j] + GapExtensionCost;
                    if (scoreAboveOpen > scoreAboveExtend)
                    {
                        scoreAbove = scoreAboveOpen;
                    }
                    else
                    {
                        scoreAbove = scoreAboveExtend;
                        vgapLength[i * gapStride + j] = vgapLength[(i - 1) * gapStride + j] + 1;
                    }

                    // Gap in query sequence
                    int scoreLeft;
                    int scoreLeftOpen = ScoreTable[leftIndex] + GapOpenCost;
                    int scoreLeftExtend = hgapCost[i * gapStride + (j-1)] + GapExtensionCost;
                    if (scoreLeftOpen > scoreLeftExtend)
                    {
                        scoreLeft = scoreLeftOpen;
                    }
                    else
                    {
                        scoreLeft = scoreLeftExtend;
                        hgapLength[i * gapStride + j] = hgapLength[i * gapStride + (j - 1)] + 1;
                    }

                    // Store off the gaps costs for this cell
                    hgapCost[i * gapStride + j] = scoreLeft;
                    vgapCost[i * gapStride + j] = scoreAbove;

                    // Get the exact match/mismatch score
                    int scoreDiag = ScoreTable[diagonalIndex] + SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];

                    // Get the max S = MAX(diag,above,left) and assign the traceback
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        ScoreTable[currentIndex] = scoreDiag;
                        Traceback[currentIndex] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        ScoreTable[currentIndex] = scoreLeft;
                        Traceback[currentIndex] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        ScoreTable[currentIndex] = scoreAbove;
                        Traceback[currentIndex] = SourceDirection.Up;
                    }
                }
            }

            return new[]
            {
                new OptScoreMatrixCell
                {
                    Row = Rows-1, 
                    Col = Cols-1, 
                    Score = ScoreTable[ScoreTable.Length-1]
                }
            };
        }

        /// <summary>
        /// This method is used to determine when the traceback step is complete.
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="col">Column</param>
        /// <returns>True if we are finished with the traceback step, false if not.</returns>
        protected override bool TracebackIsComplete(int row, int col)
        {
            return Traceback[row * Cols + col] == SourceDirection.Stop;
        }

        /// <summary>
        /// Initializes the given cell in the scoring matrix
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="col">Column</param>
        protected override void InitializeScoreTraceback(int row, int col)
        {
            if (row == 0 && col != 0)
            {
                Traceback[col] = SourceDirection.Left;
                ScoreTable[row * Cols + col] = (GapOpenCost != GapExtensionCost) ? col * GapExtensionCost : GapExtensionCost * col;
            }
            else if (col == 0 && row != 0)
            {
                Traceback[row * Cols] = SourceDirection.Up;
                ScoreTable[row * Cols + col] = (GapOpenCost != GapExtensionCost) ? row * GapExtensionCost : GapOpenCost * row;
            }
            else
            {
                Traceback[row * Cols + col] = SourceDirection.Stop;
            }
        }
    }
}