using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.SimilarityMatrices;
using Bio.Util;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Pairwise Overlap alignment implementation using affine gap model.
    /// </summary>
    public class PairwiseOverlapAffineAlignmentJob : PairwiseOverlapSimpleAlignmentJob
    {
        /// <summary>
        /// Inializes a new alignment job
        /// </summary>
        /// <param name="similarityMatrix"></param>
        /// <param name="gapOpenCost"></param>
        /// <param name="gapExtensionCost"></param>
        /// <param name="aInput"></param>
        /// <param name="bInput"></param>
        public PairwiseOverlapAffineAlignmentJob(SimilarityMatrix similarityMatrix, int gapOpenCost, int gapExtensionCost, ISequence aInput, ISequence bInput)
            : base(similarityMatrix, gapOpenCost, gapExtensionCost, aInput, bInput) {}

        /// <summary>
        /// Computes weights for all blocks of the grid except the lower-right corner one.
        /// Assumes the grid cache to the left and top of the block has already been filled.
        /// Weights on the bottom and right edge of the block are written back to the grid.
        /// </summary>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected override void ComputeIntermediateBlock(int blockRow, int blockCol, int lastRow, int lastCol)
        {
            ComputeBlock(
                (int i, int j, int Iij, int Dij, int Cij) =>
                {
                    int weight = Cij;

                    if (weight < Dij)
                    {
                        weight = Dij;
                    }

                    if (weight < Iij)
                    {
                        weight = Iij;
                    }

                    if ((i == lastRow) || (j == lastCol))
                    {
                        if (weight >= optScore)
                        {
                            if (weight > optScore)
                            {
                                optScore = weight;
                                optScoreCells.Clear();
                            }

                            long globalRow = Helper.BigMul(blockRow, gridStride) + i;
                            long globalCol = Helper.BigMul(blockCol, gridStride) + j;

                            optScoreCells.Add(new Tuple<long, long>(globalRow, globalCol));
                        }
                    }

                    return weight;

                },
            blockRow,
            blockCol,
            lastRow,
            lastCol);
        }

        /// <summary>
        /// Computes the lower-right corner block of the grid.
        /// Combines the forward and traceback passes for performance.
        /// This is the only block computation that takes place for smaller-than-block alignments
        /// </summary>
        /// <param name="trace">Array of traceback pointers</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected override void ComputeCornerBlock(int blockRow, int blockCol, int lastRow, int lastCol, sbyte[][] trace)
        {
            SetBoundaryCondition(trace, blockRow, blockCol, lastRow, lastCol);

            ComputeBlock(
            (int i, int j, int Iij, int Dij, int Cij) =>
            {
                int weight = Cij;
                sbyte direction = SourceDirection.Diagonal;

                if (weight < Dij)
                {
                    weight = Dij;
                    direction = SourceDirection.Up;
                }

                if (weight < Iij)
                {
                    weight = Iij;
                    direction = SourceDirection.Left;
                }

                if ((i == lastRow) || (j == lastCol))
                {
                    if (weight >= optScore)
                    {
                        if (weight > optScore)
                        {
                            optScore = weight;
                            optScoreCells.Clear();
                        }

                        long globalRow = Helper.BigMul(blockRow, gridStride) + i;
                        long globalCol = Helper.BigMul(blockCol, gridStride) + j;

                        optScoreCells.Add(new Tuple<long, long>(globalRow, globalCol));
                    }
                }

                trace[i][j] = direction;

                return weight;

            },
            blockRow,
            blockCol,
            lastRow,
            lastCol);
        }

        /// <summary>
        /// Computes the traceback pointers for the block.
        /// Assumes the grid cache has been already filled during the forward pass
        /// </summary>
        /// <param name="trace">Array of traceback pointers</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected override void ComputeTraceBlock(int blockRow, int blockCol, int lastRow, int lastCol, sbyte[][] trace)
        {
            SetBoundaryCondition(trace, blockRow, blockCol, lastRow, lastCol);

            ComputeBlock(
            (int i, int j, int Iij, int Dij, int Cij) =>
            {
                int weight = Cij;
                sbyte direction = SourceDirection.Diagonal;

                if (weight < Dij)
                {
                    weight = Dij;
                    direction = SourceDirection.Up;
                }

                if (weight < Iij)
                {
                    weight = Iij;
                    direction = SourceDirection.Left;
                }

                trace[i][j] = direction;

                return weight;

            },
            blockRow,
            blockCol,
            lastRow,
            lastCol);
        }

        /// <summary>
        /// Initializes grid cache for the algorithm.      
        /// </summary>
        protected override void InitializeCache()
        {
            InitializeCacheAffineZero();
        }

        /// <summary>
        /// Forward pass for the block.
        /// </summary>
        /// <param name="weightFunction"></param>
        /// <param name="blockRow"></param>
        /// <param name="blockCol"></param>
        /// <param name="lastRow"></param>
        /// <param name="lastCol"></param>
        protected override void ComputeBlock(WeightFunction weightFunction, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            ComputeBlockAffine(weightFunction, blockRow, blockCol, lastRow, lastCol);
        }
    }
}
