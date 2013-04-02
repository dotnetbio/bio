using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Implements the SmithWaterman algorithm for partial alignment.
    /// See Chapter 2 in Biological Sequence Analysis; Durbin, Eddy, Krogh and Mitchison; 
    /// Cambridge Press; 1998.
    /// </summary>
    public sealed class SmithWatermanAligner : PairwiseSequenceAligner
    {
        /// <summary>
        /// Gets the name of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns the Name of our algorithm i.e 
        /// Smith-Waterman algorithm.
        /// </summary>
        public override string Name
        {
            get { return Properties.Resource.SMITH_NAME; }
        }

        /// <summary>
        /// Gets the Description of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// This property returns a simple description of what 
        /// SmithWatermanAligner class implements.
        /// </summary>
        public override string Description
        {
            get { return Properties.Resource.SMITH_DESCRIPTION; }
        }

        /// <summary>
        /// This is step (2) in the dynamic programming model - to fill in the scoring matrix
        /// and calculate the traceback entries.  In the Smith-Waterman algorithm, we track the
        /// highest scoring cell during the algorithm - this is often NOT the bottom/right cell as
        /// it would be in a global alignment.
        /// </summary>
        protected override IEnumerable<OptScoreMatrixCell> CreateTracebackTable(bool useAffineGapModel)
        {
            if (useAffineGapModel)
                return CreateAffineTracebackTable();

            // Initialize the high score cell to an invalid value
            var highScoreCells = new List<OptScoreMatrixCell> { new OptScoreMatrixCell { Score = Int32.MinValue }};

            // Walk the sequences and generate the scores.
            for (int i = 1; i < Rows; i++)
            {
                for (int j = 1; j < Cols; j++)
                {
                    int aboveIndex = (i - 1)*Cols + j;
                    int leftIndex = i*Cols + (j - 1);
                    int diagonalIndex = (i - 1)*Cols + (j - 1);
                    int currentIndex = i*Cols + j;

                    // Gap in sequence #1
                    int scoreAbove = ScoreTable[aboveIndex] + GapOpenCost;

                    // Gap in sequence #2
                    int scoreLeft = ScoreTable[leftIndex] + GapOpenCost;

                    // Match score (diagonal)
                    int scoreDiag = ScoreTable[diagonalIndex] + SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];

                    // Calculate the current cell score and trackback
                    // M[i,j] = MAX(M[i-1,j-1] + S[i,j], M[i,j-1] + gapCost, M[i-1,j] + gapCost)
                    int score;
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        score = scoreDiag;
                        Traceback[currentIndex] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        score = scoreLeft;
                        Traceback[currentIndex] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        score = scoreAbove;
                        Traceback[currentIndex] = SourceDirection.Up;
                    }

                    // Do not allow for negative scores in the local alignment.
                    if (score < 0)
                        score = 0;

                    // Keep track of the highest scoring cell for our final score.
                    if (score >= highScoreCells[0].Score)
                    {
                        if (score > highScoreCells[0].Score)
                            highScoreCells.Clear();
                        highScoreCells.Add(new OptScoreMatrixCell { Row = i, Col = j, Score = score });
                    }

                    ScoreTable[currentIndex] = score;
                }
            }

            return highScoreCells;
        }

        /// <summary>
        /// This method is used to create the traceback/scoring table when an affine gap model 
        /// is being used - this is where the open cost is different than the extension cost for a gap
        /// and generally will produce a better alignment.
        /// </summary>
        private IEnumerable<OptScoreMatrixCell> CreateAffineTracebackTable()
        {
            // Initialize the high score cell to an invalid value
            var highScoreCells = new List<OptScoreMatrixCell> { new OptScoreMatrixCell { Score = Int32.MinValue } };

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

            // Walk the sequences and generate the scoring/traceback matrix
            for (int i = 1; i < Rows; i++)
            {
                for (int j = 1; j < Cols; j++)
                {
                    int aboveIndex = (i - 1) * Cols + j;
                    int leftIndex = i * Cols + (j - 1);
                    int diagonalIndex = (i - 1) * Cols + (j - 1);
                    int currentIndex = i * Cols + j;

                    // Gap in sequence #1 (reference)
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

                    // Gap in sequence #2 (query)
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

                    // Match score (diagonal)
                    int scoreDiag = ScoreTable[diagonalIndex] + SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];

                    // Calculate the current cell score and trackback
                    // M[i,j] = MAX(M[i-1,j-1] + S[i,j], M[i,j-1] + gapCost, M[i-1,j] + gapCost)
                    int score;
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        score = scoreDiag;
                        Traceback[currentIndex] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        score = scoreLeft;
                        Traceback[currentIndex] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        score = scoreAbove;
                        Traceback[currentIndex] = SourceDirection.Up;
                    }

                    // Do not allow for negative scores in the local alignment.
                    if (score < 0)
                        score = 0;

                    // Keep track of the highest scoring cell for our final score.
                    if (score >= highScoreCells[0].Score)
                    {
                        if (score > highScoreCells[0].Score)
                            highScoreCells.Clear();
                        highScoreCells.Add(new OptScoreMatrixCell { Row = i, Col = j, Score = score });
                    }

                    ScoreTable[currentIndex] = score;
                }
            }

            return highScoreCells;
        }

        /// <summary>
        /// This method is used to determine when the traceback step is complete.
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="col">Column</param>
        /// <returns>True if we are finished with the traceback step, false if not.</returns>
        protected override bool TracebackIsComplete(int row, int col)
        {
            return ScoreTable[row * Cols + col] == 0;
        }
    }
}