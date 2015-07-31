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
        protected override IEnumerable<OptScoreMatrixCell> CreateTracebackTable()
        {
            if (usingAffineGapModel)
                return CreateAffineTracebackTable();

            int[][] matrix = SimilarityMatrix.Matrix;
            int[] scoreLastRow = new int[Cols];
            int[] scoreRow = new int[Cols];

            // Initialize the high score cell to an invalid value
            var highScoreCells = new List<OptScoreMatrixCell> { new OptScoreMatrixCell { Score = Int32.MinValue }};

            // Walk the sequences and generate the scores.
            for (int i = 1; i < Rows; i++)
            {
                sbyte[] traceback = new sbyte[Cols]; // Initialized to STOP

                if (i > 1)
                {
                    // Move current row to last row
                    scoreLastRow = scoreRow;
                    scoreRow = new int[Cols];
                }

                for (int j = 1; j < Cols; j++)
                {
                    // Gap in reference sequence
                    int scoreAbove = scoreLastRow[j] + GapOpenCost;

                    // Gap in query sequence
                    int scoreLeft = scoreRow[j - 1] + GapOpenCost;

                    // Match/mismatch score
                    int mScore = (matrix != null) ? matrix[QuerySequence[i - 1]][ReferenceSequence[j - 1]] : SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];
                    int scoreDiag = scoreLastRow[j - 1] + mScore;

                    // Calculate the current cell score and trackback
                    // M[i,j] = MAX(M[i-1,j-1] + S[i,j], M[i,j-1] + gapCost, M[i-1,j] + gapCost)
                    int score;
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        score = scoreDiag;
                        traceback[j] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        score = scoreLeft;
                        traceback[j] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        score = scoreAbove;
                        traceback[j] = SourceDirection.Up;
                    }

                    // Do not allow for negative scores in the local alignment.
                    if (score <= 0)
                    {
                        score = 0;
                        traceback[j] = SourceDirection.Stop;
                    }

                    // Keep track of the highest scoring cell for our final score.
                    if (score >= highScoreCells[0].Score)
                    {
                        if (score > highScoreCells[0].Score)
                            highScoreCells.Clear();
                        highScoreCells.Add(new OptScoreMatrixCell { Row = i, Col = j, Score = score });
                    }

                    scoreRow[j] = score;
                }

                // Save off the traceback row
                Traceback[i] = traceback;

                if (IncludeScoreTable)
                {
                    Array.Copy(scoreRow, 0, ScoreTable, i * Cols, Cols);
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

            // Score matrix - we just track the current row and prior row for better memory utilization
            int[] scoreLastRow = new int[Cols];
            int[] scoreRow = new int[Cols];
            int[][] matrix = SimilarityMatrix.Matrix;

            // Horizontal and vertical gap counts.
            int gapStride = Cols + 1;
            h_Gap_Length = new int[(Rows + 1) * gapStride];
            v_Gap_Length = new int[(Rows + 1) * gapStride];
            int[] hgapCost = new int[(Rows + 1) * gapStride];
            int[] vgapCost = new int[(Rows + 1) * gapStride];

            // Initialize the gap extension cost matrices.
            for (int i = 1; i < Rows; i++)
            {
                h_Gap_Length[i * gapStride] = i;
                v_Gap_Length[i * gapStride] = 1;
                hgapCost[i * gapStride] = GapExtensionCost * (i-1)+GapOpenCost;
            }
            for (int j = 1; j < Cols; j++)
            {
                h_Gap_Length[j] = 1;
                v_Gap_Length[j] = j;
                vgapCost[j] = GapExtensionCost * (j-1)+GapOpenCost;
            }

            // Walk the sequences and generate the scoring/traceback matrix
            for (int i = 1; i < Rows; i++)
            {
                sbyte[] traceback = new sbyte[Cols]; // Initialized to STOP

                if (i > 1)
                {
                    // Move current row to last row
                    scoreLastRow = scoreRow;
                    scoreRow = new int[Cols];
                }

                for (int j = 1; j < Cols; j++)
                {
                    // Gap in sequence #1 (reference)
                    int scoreAbove;
                    int scoreAboveOpen = scoreLastRow[j] + GapOpenCost;
                    int scoreAboveExtend = vgapCost[(i-1) * gapStride + j] + GapExtensionCost;
                    if (scoreAboveOpen > scoreAboveExtend)
                    {
                        scoreAbove = scoreAboveOpen;
                        v_Gap_Length[i * gapStride + j] = 1;
                    }
                    else
                    {
                        scoreAbove = scoreAboveExtend;
                        v_Gap_Length[i * gapStride + j] = v_Gap_Length[(i - 1) * gapStride + j] + 1;
                    }

                    // Gap in sequence #2 (query)
                    int scoreLeft;
                    int scoreLeftOpen = scoreRow[j-1] + GapOpenCost;
                    int scoreLeftExtend = hgapCost[i * gapStride + (j-1)] + GapExtensionCost;
                    if (scoreLeftOpen > scoreLeftExtend)
                    {
                        scoreLeft = scoreLeftOpen;
                        h_Gap_Length[i * gapStride + j] = 1;
                    }
                    else
                    {
                        scoreLeft = scoreLeftExtend;
                        h_Gap_Length[i * gapStride + j] = h_Gap_Length[i * gapStride + (j - 1)] + 1;
                    }

                    // Store off the gaps costs for this cell
                    hgapCost[i * gapStride + j] = scoreLeft;
                    vgapCost[i * gapStride + j] = scoreAbove;

                    // Match score (diagonal)
                    int mScore = (matrix != null) ? matrix[QuerySequence[i - 1]][ReferenceSequence[j - 1]] : SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];
                    int scoreDiag = scoreLastRow[j - 1] + mScore;

                    // Calculate the current cell score and trackback
                    // M[i,j] = MAX(M[i-1,j-1] + S[i,j], M[i,j-1] + gapCost, M[i-1,j] + gapCost)
                    int score;
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        score = scoreDiag;
                        traceback[j] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        score = scoreLeft;
                        traceback[j] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        score = scoreAbove;
                        traceback[j] = SourceDirection.Up;
                    }

                    // Do not allow for negative scores in the local alignment.
                    if (score <= 0)
                    {
                        score = 0;
                        traceback[j] = SourceDirection.Stop;
                    }

                    // Keep track of the highest scoring cell for our final score.
                    if (score >= highScoreCells[0].Score)
                    {
                        if (score > highScoreCells[0].Score)
                            highScoreCells.Clear();
                        highScoreCells.Add(new OptScoreMatrixCell { Row = i, Col = j, Score = score });
                    }

                    scoreRow[j] = score;
                }

                // Save off the traceback row
                Traceback[i] = traceback;

                if (IncludeScoreTable)
                {
                    Array.Copy(scoreRow, 0, ScoreTable, i * Cols, Cols);
                }
            }
            return highScoreCells;
        }
    }
}