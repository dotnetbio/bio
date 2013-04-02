using System;
using System.Collections.Generic;
using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment.Legacy
{
    /// <summary>
    /// Needleman-Wunsch alignment implementation using simple gap model.
    /// </summary>
    public class NeedlemanWunschSimpleAlignmentJob : DynamicProgrammingPairwiseAlignerJob
    {
        /// <summary>
        /// Inializes a new alignment job
        /// </summary>
        /// <param name="similarityMatrix"></param>
        /// <param name="gapOpenCost"></param>
        /// <param name="aInput"></param>
        /// <param name="bInput"></param>
        public NeedlemanWunschSimpleAlignmentJob(SimilarityMatrix similarityMatrix, int gapOpenCost, ISequence aInput, ISequence bInput)
            : base(similarityMatrix, gapOpenCost, 0, aInput, bInput) { }

        /// <summary>
        /// Inializes a new alignment job
        /// </summary>
        /// <param name="similarityMatrix"></param>
        /// <param name="gapOpenCost"></param>
        /// <param name="gapExtensionCost"></param>
        /// <param name="aInput"></param>
        /// <param name="bInput"></param>
        protected NeedlemanWunschSimpleAlignmentJob(SimilarityMatrix similarityMatrix, int gapOpenCost, int gapExtensionCost, ISequence aInput, ISequence bInput)
            : base(similarityMatrix, gapOpenCost, gapExtensionCost, aInput, bInput) { }

        /// <summary>
        /// Launches the alignment algorithm
        /// </summary>
        public override List<IPairwiseSequenceAlignment> Align()
        {
            List<IPairwiseSequenceAlignment> result = base.Align();

            foreach (IPairwiseSequenceAlignment alignment in result)
            {
                foreach (PairwiseAlignedSequence sequence in alignment.AlignedSequences)
                {
                    sequence.FirstOffset = GetOffset(sequence.FirstSequence) - GetOffset(alignment.Sequences[0]);
                    sequence.SecondOffset = GetOffset(sequence.SecondSequence) - GetOffset(alignment.Sequences[0]);
                }
            }

            return result;
        }

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

                    if (weight < Iij)
                    {
                        weight = Iij;
                    }

                    if (weight < Dij)
                    {
                        weight = Dij;
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

                if (weight < Iij)
                {
                    weight = Iij;
                    direction = SourceDirection.Left;
                }

                if (weight < Dij)
                {
                    weight = Dij;
                    direction = SourceDirection.Up;
                }

                if ((i == lastRow) && (j == lastCol))
                {
                    optScore = weight;
                    long globalRow = Math.BigMul(blockRow, gridStride) + i;
                    long globalCol = Math.BigMul(blockCol, gridStride) + j;

                    optScoreCells.Add(new Tuple<long, long>(globalRow, globalCol));
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

                if (weight < Iij)
                {
                    weight = Iij;
                    direction = SourceDirection.Left;
                }

                if (weight < Dij)
                {
                    weight = Dij;
                    direction = SourceDirection.Up;
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
            InitializeCacheSimple();
        }

        /// <summary>
        /// Forward pass for the block.
        /// </summary>
        /// <param name="weightFunction"></param>
        /// 
        /// <param name="blockRow"></param>
        /// <param name="blockCol"></param>
        /// <param name="lastRow"></param>
        /// <param name="lastCol"></param>
        protected override void ComputeBlock(WeightFunction weightFunction, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            ComputeBlockSimple(weightFunction, blockRow, blockCol, lastRow, lastCol);
        }

        /// <summary>
        /// Sets the boundary values tor traceback
        /// </summary>
        /// <param name="trace">Array of traceback pointers</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected override void SetBoundaryCondition(sbyte[][] trace, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            // First row of pointers
            if (blockRow == 0)
            {
                trace[0][0] = SourceDirection.Stop;

                for (int cellIndex = 1; cellIndex <= lastCol; cellIndex++)
                {
                    trace[0][cellIndex] = SourceDirection.Left;
                }
            }
            else
            {
                for (int cellIndex = 0; cellIndex <= lastCol; cellIndex++)
                {
                    trace[0][cellIndex] = SourceDirection.Block;
                }
            }

            // First column of pointers
            if (blockCol == 0)
            {
                trace[0][0] = SourceDirection.Stop;

                for (int cellIndex = 1; cellIndex <= lastRow; cellIndex++)
                {
                    trace[cellIndex][0] = SourceDirection.Up;
                }
            }
            else
            {
                for (int cellIndex = 0; cellIndex <= lastRow; cellIndex++)
                {
                    trace[cellIndex][0] = SourceDirection.Block;
                }
            }
        }

        /// <summary>
        /// Return the starting position of alignment of sequence1 with respect to sequence2.
        /// </summary>
        /// <param name="aligned">Aligned sequence.</param>
        /// <returns>The number of initial gap characters.</returns>
        protected int GetOffset(IEnumerable<byte> aligned)
        {
            if (aligned == null)
            {
                throw new ArgumentNullException("aligned");
            }

            int ret = 0;
            foreach (byte item in aligned)
            {
                if (item != gapCode)
                {
                    return ret;
                }

                ++ret;
            }

            return ret;
        }
    }
}
