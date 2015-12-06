using System;
using System.Collections.Generic;
using System.Linq;

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
        /// A element holding the match and gap scores, 
        /// allows us to store three matrices in one.
        /// </summary>
        internal struct ScoreElement {
            /// <summary>
            /// The score for a diagonal move.
            /// </summary>
            internal int MatchScore;
            /// <summary>
            /// The score for a horizontal move.
            /// </summary>
            internal int HorizontalGapScore;
            /// <summary>
            /// The score for a vertical move.
            /// </summary>
            internal int VerticalGapScore;

            /// <summary>
            /// Gets the best score for this cell, used to compute the match score 
            /// which is the max of (A + MatchScore, B + MatchScore, C + VerticalGapScore).
            /// Since the max was already computed earlier to determine the traceback,
            /// in theory we can store that value rather than recompute the max, but
            /// this is expected to be fast enough.
            /// </summary>
            /// <value>The best score.</value>
            internal int BestScore {
                get 
                {
                    return Math.Max (Math.Max (MatchScore, HorizontalGapScore), VerticalGapScore);
                }
            }
        }

        /// <summary>
        /// This is step (2) in the dynamic programming model - to fill in the scoring matrix
        /// and calculate the traceback entries. 
        /// </summary>
        protected override IEnumerable<OptScoreMatrixCell> CreateTracebackTable()
        {
            // The affine gap model (Gotoh) is unrolled into a duplicate routine since this is really
            // a massive loop.  We could combine most of the logic with delegates, but that has a severe
            // penalty in performance due to the number of times they are called.
            if (usingAffineGapModel)
                return CreateAffineTracebackTable();

            int[] scoreLastRow = new int[Cols];
            int[] scoreRow = new int[Cols];
            int[][] matrix = SimilarityMatrix.Matrix;

            // Initialize the first row of the TB table
            for (int index = 1; index < Cols; index++)
                Traceback[0][index] = SourceDirection.Left;

            for (int i = 1; i < Rows; i++)
            {
                // Create the next TB row
                sbyte[] traceback = new sbyte[Cols];
                traceback[0] = SourceDirection.Up;

                if (i > 1)
                {
                    // Move current row to last row
                    Array.Copy(scoreRow, scoreLastRow, Cols);

                    // Initialize the next scoring row
                    scoreRow[0] = GapOpenCost*i;
                    for (int index = 1; index < Cols; index++)
                        scoreRow[index] = GapOpenCost*index;
                }
                else // first iteration
                {
                    scoreRow[0] = GapOpenCost;

                    // Initialize the first row
                    for (int index = 0; index < Cols; index++)
                    {
                        int initialScore = GapOpenCost * index;
                        scoreLastRow[index] = initialScore;
                        if (IncludeScoreTable)
                            ScoreTable[index] = initialScore;
                    }
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

                    // Get the max S = MAX(diag,above,left) and assign the traceback
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        scoreRow[j] = scoreDiag;
                        traceback[j] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        scoreRow[j] = scoreLeft;
                        traceback[j] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        scoreRow[j] = scoreAbove;
                        traceback[j] = SourceDirection.Up;
                    }
                }

                // Save off the traceback step
                Traceback[i] = traceback;

                if (IncludeScoreTable)
                {
                    Array.Copy(scoreRow, 0, ScoreTable, i*Cols, Cols);
                }
            }

            // Return a single high score which is always the bottom right corner of the matrix.
            return new[]
            {
                new OptScoreMatrixCell
                {
                    Row = Rows-1, 
                    Col = Cols-1, 
                    Score = scoreRow[Cols-1],
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
            /* We will need a low constant number that is not Int32.MinValue 
             * for the boundaries of the matrix (if we used Int32.MinValue it would 
             * over flow when added to the move score to a very high value and become the max
             * rather than the min).
             */
            int EDGE_MIN_VALUE = Int32.MinValue + Math.Max(Rows, Cols) * Math.Abs (GapOpenCost);


            // Horizontal and vertical gap counts.
            int gapStride = Cols + 1;

            /* These are equivalent to traceback matrices for the 
             * horizontal and vertical "gap" matrices.  However, rather than store
             * an arrow that points in a direction, we store the size of moves,
             * which allows us to transition into and out of the gap matrices in one
             * step, rather than following paths through them.
             */
            h_Gap_Length = new int[(Rows + 1) * gapStride];
            v_Gap_Length = new int[(Rows + 1) * gapStride];

            /* As we progress through, we only need to know about 
             * the current row and the previous row for the recursions,
             * so we only use these two, rather than a full matrix.
             */
            ScoreElement[] previousScoreRow = new ScoreElement[Cols];
            ScoreElement[] currentScoreRow = new ScoreElement[Cols];
            int[][] matrix = SimilarityMatrix.Matrix;

            // Upper left element is initalized to 0
            currentScoreRow [0] = new ScoreElement () {
                HorizontalGapScore = EDGE_MIN_VALUE,
                VerticalGapScore = EDGE_MIN_VALUE,
                MatchScore = 0
            };
            if (IncludeScoreTable) {
                ScoreTable[0] = 0;

            }
            // Initialize first row
            sbyte[] traceback = Traceback [0];
            for (int j = 1; j < traceback.Length; j++)
            {
                // always have to go left from top
                traceback[j] = SourceDirection.Left;
                h_Gap_Length[j] = j;
                var initialScore = (j - 1) * GapExtensionCost + GapOpenCost;
                currentScoreRow [j] = new ScoreElement () {
                    HorizontalGapScore = initialScore,
                    VerticalGapScore = EDGE_MIN_VALUE,
                    MatchScore = EDGE_MIN_VALUE
                };
                if (IncludeScoreTable)
                   ScoreTable[j] = initialScore;
            }

            for (int i = 1; i < Rows; i++)
            {
                // Create the next TB row
                traceback = new sbyte[Cols];
                Traceback [i] = traceback;
                // Make the current row the last row,
                // reuse the current rows memory for the new row
                SwapArrays(ref currentScoreRow, ref previousScoreRow);
              
                /* Initialize first column
                 * We can't move further back horizontally here (we are at 
                 * the edge), so we set the score to something so low we will
                 * never go this direction on the highest scoring path, and so 
                 * can avoid explicitly checking for an edge
                 */
                currentScoreRow[0] = new ScoreElement() {
                    MatchScore = EDGE_MIN_VALUE, 
                    HorizontalGapScore = EDGE_MIN_VALUE, 
                    VerticalGapScore = (i - 1) * GapExtensionCost + GapOpenCost};
                traceback[0] = SourceDirection.Up;
                v_Gap_Length[i * gapStride] = i;

                for (int j = 1; j < Cols; j++)
                {
                    // Get the three important values
                    var cellAbove = previousScoreRow[j];
                    var cellDiagAbove = previousScoreRow [j - 1];
                    var cellLeft = currentScoreRow [j - 1];

                    // Gap in reference sequence
                    int scoreAbove;
                    int scoreAboveOpen = cellAbove.BestScore + GapOpenCost;
                    int scoreAboveExtend = cellAbove.VerticalGapScore + GapExtensionCost;
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

                    // Gap in query sequence
                    int scoreLeft;
                    int scoreLeftOpen = cellLeft.MatchScore + GapOpenCost;
                    int scoreLeftExtend = cellLeft.HorizontalGapScore + GapExtensionCost;
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

                    // Get the exact match/mismatch score
                    int mScore = (matrix != null) ? matrix[QuerySequence[i - 1]][ReferenceSequence[j - 1]] : SimilarityMatrix[QuerySequence[i - 1], ReferenceSequence[j - 1]];
                    /* Since all possible previous states have the same match score applied, 
                     * we can just add to the previous max */
                    int scoreDiag = cellDiagAbove.BestScore + mScore;

                    // Store the scores for this cell
                    currentScoreRow[j] = new ScoreElement() { HorizontalGapScore = scoreLeft,
                                                              VerticalGapScore = scoreAbove,
                                                              MatchScore = scoreDiag };
                                                              

                    // Get the max S = MAX(diag,above,left) and assign the traceback
                    if ((scoreDiag > scoreAbove) && (scoreDiag > scoreLeft))
                    {
                        traceback[j] = SourceDirection.Diagonal;
                    }
                    else if (scoreLeft > scoreAbove)
                    {
                        traceback[j] = SourceDirection.Left;
                    }
                    else //if (scoreAbove > scoreLeft)
                    {
                        traceback[j] = SourceDirection.Up;
                    }
                }

                if (IncludeScoreTable)
                {
                    Array.Copy(currentScoreRow.Select(x => x.BestScore).ToArray(), 0, ScoreTable, i * Cols, Cols);
                }
            }

            return new[]
            {
                new OptScoreMatrixCell
                {
                    Row = Rows-1, 
                    Col = Cols-1, 
                    Score = currentScoreRow[Cols-1].BestScore,
                }
            };
        }
       
        private void SwapArrays(ref ScoreElement[] previousRow, ref ScoreElement[] currentRow) {
            var tmp = previousRow;
            previousRow = currentRow;
            currentRow = tmp;
        }
    }
}