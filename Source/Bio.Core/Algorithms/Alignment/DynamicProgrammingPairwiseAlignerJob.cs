using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bio.Core.Extensions;
using Bio.SimilarityMatrices;
using Bio.Util;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Base class for all aligner algorithms.
    /// Provides storage and read/write operations for the grid.
    /// </summary>
    public abstract class DynamicProgrammingPairwiseAlignerJob
    {
        /// <summary>
        /// Base class for alignment implementation.
        /// Provides building blocks for alignments using either simple or affine gap models.
        /// </summary>
        protected const int gridStride = 512;

        // SequenceI - determines the columnt height
        /// <summary>
        /// 
        /// </summary>
        private ISequence sequenceI;
        // SequenceJ - determines the row width
        /// <summary>
        /// 
        /// </summary>
        private ISequence sequenceJ;

        /// <summary>
        /// 
        /// </summary>
        private long colHeight;

        /// <summary>
        /// 
        /// </summary>
        private long rowWidth;

        /// <summary>
        /// 
        /// </summary>
        private SimilarityMatrix similarityMatrix;
        /// <summary>
        /// 
        /// </summary>
        private int gapOpenCost;
        /// <summary>
        /// 
        /// </summary>
        private int gapExtensionCost;

        /// <summary>
        /// 
        /// </summary>
        private int gridRows;
        /// <summary>
        /// 
        /// </summary>
        private int gridCols;

        /// <summary>
        /// Match cache rows
        /// </summary>
        private int[][] cachedRowsC;

        /// <summary>
        /// Match cache columns
        /// </summary>
        private int[][] cachedColsC;

        /// <summary>
        /// Insertion cost cache rows
        /// </summary>
        private int[][] cachedRowsI;

        /// <summary>
        /// Insertion cost cache columns
        /// </summary>
        private int[][] cachedColsI;

        /// <summary>
        /// Deletion cost cache rows
        /// </summary>
        private int[][] cachedRowsD;

        /// <summary>
        /// Deletion cost cache columns
        /// </summary>
        private int[][] cachedColsD;

        /// <summary>
        /// Stores details of all cells with best score.
        /// </summary>
        internal List<Tuple<long, long>> optScoreCells = new List<Tuple<long, long>>();

        /// <summary>
        /// 
        /// </summary>
        public int optScore = 1;

        /// <summary>
        /// Signifies gap in aligned sequence (stored as int[]) during trace back.
        /// </summary>
        protected byte gapCode = 255;

        /// <summary>
        /// Algorith-specific weight function
        /// </summary>
        /// <param name="i">First index within the block</param>
        /// <param name="j">Second index within the block</param>
        /// <param name="Iij">Cost of Insertion</param>
        /// <param name="Dij">Cost of Deletion</param>
        /// <param name="Cij">Cost of Match</param>
        /// <returns></returns>
        protected delegate int WeightFunction(int i, int j, int Iij, int Dij, int Cij);

                /// <summary>
        /// 
        /// </summary>
        /// <param name="similarityMatrix"></param>
        /// <param name="gapOpenCost"></param>
        /// <param name="gapExtensionCost"></param>
        /// <param name="aInput"></param>
        /// <param name="bInput"></param>
        protected DynamicProgrammingPairwiseAlignerJob(SimilarityMatrix similarityMatrix, int gapOpenCost, int gapExtensionCost, ISequence aInput, ISequence bInput)
        {
            if (aInput == null)
            {
                throw new ArgumentNullException("aInput");
            }

            aInput.Alphabet.TryGetDefaultGapSymbol(out gapCode);

            // Set Gap Penalty and Similarity Matrix
            this.gapOpenCost = gapOpenCost;
            this.gapExtensionCost = gapExtensionCost;

            // note that _gapExtensionCost is not used for linear gap penalty
            this.similarityMatrix = similarityMatrix;

            // Convert input strings to 0-based int arrays using similarity matrix mapping
            this.sequenceI = aInput;
            this.sequenceJ = bInput;

            colHeight = sequenceI.Count + 1;
            rowWidth = sequenceJ.Count + 1;

            gridCols = (int)((rowWidth - 1) / gridStride) + 1;
            gridRows = (int)((colHeight - 1) / gridStride) + 1;
        }

        /// <summary>
        /// Initializes grid cache for the algorithm.      
        /// </summary>
        protected abstract void InitializeCache();

        /// <summary>
        /// Sets the boundary values tor traceback
        /// </summary>
        /// <param name="trace">Array of traceback pointers</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected abstract void SetBoundaryCondition(sbyte[][] trace, int blockRow, int blockCol, int lastRow, int lastCol);

        /// <summary>
        /// Computes weights for all blocks of the grid except the lower-right corner one.
        /// Assumes the grid cache to the left and top of the block has already been filled.
        /// Weights on the bottom and right edge of the block are written back to the grid.
        /// </summary>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected abstract void ComputeIntermediateBlock(int blockRow, int blockCol, int lastRow, int lastCol);

        /// <summary>
        /// Computes the lower-right corner block of the grid.
        /// Combines the forward and traceback passes for performance.
        /// This is the only block computation that takes place for smaller-than-block alignments
        /// </summary>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        /// <param name="trace">Array of traceback pointers</param>
        protected abstract void ComputeCornerBlock(int blockRow, int blockCol, int lastRow, int lastCol, sbyte[][] trace);

        /// <summary>
        /// Computes the traceback pointers for the block.
        /// Assumes the grid cache has been already filled during the forward pass
        /// </summary>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        /// <param name="trace">Array of traceback pointers</param>
        protected abstract void ComputeTraceBlock(int blockRow, int blockCol, int lastRow, int lastCol, sbyte[][] trace);

        /// <summary>
        /// Forward pass for the block.
        /// Assumes the blocks to the top and right have already been processed.
        /// </summary>
        /// <param name="weightFunction"></param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected abstract void ComputeBlock(WeightFunction weightFunction, int blockRow, int blockCol, int lastRow, int lastCol);

        /// <summary>
        /// Launches the alignment algorithm
        /// </summary>
        public virtual List<IPairwiseSequenceAlignment> Align()
        {
            InitializeCache();

            // Grid
            for (int diagonal = 0; diagonal < gridCols + gridRows - 2; diagonal++)
            {
                for (int blockRow = 0; blockRow < gridRows; blockRow++)
                {
                    int blockCol = diagonal - blockRow;

                    if ((blockCol >= 0) && (blockCol < gridCols))
                    {
                        int lastRow = (blockRow == gridRows - 1) ? (int)(colHeight - Helper.BigMul(blockRow, gridStride) - 1) : gridStride;
                        int lastCol = (blockCol == gridCols - 1) ? (int)(rowWidth - Helper.BigMul(blockCol, gridStride) - 1) : gridStride;

                        ComputeIntermediateBlock(blockRow, blockCol, lastRow, lastCol);
                    }
                }
            }

            sbyte[][] trace = new sbyte[gridStride + 1][];
            for (int i = 0; i <= gridStride; i++)
            {
                trace[i] = new sbyte[gridStride + 1];
            }

            // Last Block - grid calculation and Traceback combined
            int completeTraceRow = gridRows - 1;
            int completeTraceCol = gridCols - 1;

            int completeLastRow = (int)(colHeight - Helper.BigMul(completeTraceRow, gridStride) - 1);
            int completeLastCol = (int)(rowWidth - Helper.BigMul(completeTraceCol, gridStride) - 1);

            ComputeCornerBlock(completeTraceRow, completeTraceCol, completeLastRow, completeLastCol, trace);

            //Traceback
            if (optScoreCells.Count == 0)
            {
                return new List<IPairwiseSequenceAlignment>();
            }
            else
            {
                PairwiseSequenceAlignment alignment = new PairwiseSequenceAlignment(sequenceI, sequenceJ);

                for (int alignmentCount = 0; alignmentCount < optScoreCells.Count; alignmentCount++)
                {
                    PairwiseAlignedSequence result = new PairwiseAlignedSequence();
                    result.Score = optScore;

                    long alignmentRow = optScoreCells[alignmentCount].Item1;
                    long alignmentCol = optScoreCells[alignmentCount].Item2;

                    int blockRow = (int)(alignmentRow / gridStride);
                    int blockCol = (int)(alignmentCol / gridStride);

                    int lastRow = (int)(alignmentRow - Helper.BigMul(blockRow, gridStride));
                    int lastCol = (int)(alignmentCol - Helper.BigMul(blockCol, gridStride));

                    result.Metadata["EndOffsets"] = new List<long> { alignmentRow - 1, alignmentCol - 1 };

                    long alignmentLength = 0;
                    byte[] sequence1 = new byte[colHeight + rowWidth];
                    byte[] sequence2 = new byte[colHeight + rowWidth];

                    int colGaps = 0;
                    int rowGaps = 0;

                    while ((blockRow >= 0) && (blockCol >= 0))
                    {
                        if ((blockRow != completeTraceRow) || (blockCol != completeTraceCol) || (lastRow > completeLastRow) || (lastCol > completeLastCol))
                        {
                            ComputeTraceBlock(blockRow, blockCol, lastRow, lastCol, trace);

                            completeTraceRow = blockRow;
                            completeTraceCol = blockCol;

                            completeLastRow = lastRow;
                            completeLastCol = lastCol;
                        }

                        long startPositionI = blockRow * gridStride - 1;
                        long startPositionJ = blockCol * gridStride - 1;

                        while ((trace[lastRow][lastCol] != SourceDirection.Stop) && (trace[lastRow][lastCol] != SourceDirection.Block))
                        {
                            switch (trace[lastRow][lastCol])
                            {
                                case SourceDirection.Diagonal:
                                    // diagonal, no gap, use both sequence residues
                                    sequence1[alignmentLength] = sequenceI[startPositionI + lastRow];
                                    sequence2[alignmentLength] = sequenceJ[startPositionJ + lastCol];
                                    alignmentLength++;
                                    lastRow--;
                                    lastCol--;
                                    break;

                                case SourceDirection.Up:
                                    // up, gap in J
                                    sequence1[alignmentLength] = sequenceI[startPositionI + lastRow];
                                    sequence2[alignmentLength] = this.gapCode;
                                    alignmentLength++;
                                    lastRow--;
                                    colGaps++;
                                    break;

                                case SourceDirection.Left:
                                    // left, gap in I
                                    sequence1[alignmentLength] = this.gapCode;
                                    sequence2[alignmentLength] = sequenceJ[startPositionJ + lastCol];
                                    alignmentLength++;
                                    lastCol--;
                                    rowGaps++;
                                    break;
                            }
                        }

                        if (trace[lastRow][lastCol] == SourceDirection.Stop)
                        {

                            // Be nice, turn aligned solutions around so that they match the input sequences
                            byte[] alignedA = new byte[alignmentLength];
                            byte[] alignedB = new byte[alignmentLength];
                            for (long i = 0, j = alignmentLength - 1; i < alignmentLength; i++, j--)
                            {
                                alignedA[i] = sequence1[j];
                                alignedB[i] = sequence2[j];
                            }

                            // If alphabet of inputA is DnaAlphabet then alphabet of alignedA may be Dna or AmbiguousDna.
                            IAlphabet alphabet = Alphabets.AutoDetectAlphabet(alignedA, 0, alignedA.GetLongLength(), sequenceI.Alphabet);
                            Sequence seq = new Sequence(alphabet, alignedA, false);
                            seq.ID = sequenceI.ID;
                            // seq.DisplayID = aInput.DisplayID;
                            result.FirstSequence = seq;

                            alphabet = Alphabets.AutoDetectAlphabet(alignedB, 0, alignedB.GetLongLength(), sequenceJ.Alphabet);
                            seq = new Sequence(alphabet, alignedB, false);
                            seq.ID = sequenceJ.ID;
                            // seq.DisplayID = bInput.DisplayID;
                            result.SecondSequence = seq;

                            // Offset is start of alignment in input sequence with respect to other sequence.
                            if (lastCol >= lastRow)
                            {
                                result.FirstOffset = lastCol - lastRow;
                                result.SecondOffset = 0;
                            }
                            else
                            {
                                result.FirstOffset = 0;
                                result.SecondOffset = lastRow - lastCol;
                            }
                            result.Metadata["StartOffsets"] = new List<long> { lastRow, lastCol };
                            result.Metadata["Insertions"] = new List<long> { rowGaps, colGaps };
                            alignment.PairwiseAlignedSequences.Add(result);

                            break;
                        }
                        else
                        {
                            if (lastRow == 0 && lastCol == 0)
                            {
                                blockRow--;
                                blockCol--;
                                lastRow = gridStride;
                                lastCol = gridStride;
                            }
                            else
                            {
                                if (lastRow == 0)
                                {
                                    blockRow--;
                                    lastRow = gridStride;
                                }
                                else
                                {
                                    blockCol--;
                                    lastCol = gridStride;
                                }
                            }
                        }
                    }
                }

                return new List<IPairwiseSequenceAlignment>() { alignment };
            }
        }

        /// <summary>
        /// Initializes grid cache for the simple alignment.
        /// </summary>
        protected void InitializeCacheSimple()
        {
            // Preallocate row cache
            cachedRowsC = new int[gridRows][];
            for (int i = 0; i < gridRows; i++)
            {
                cachedRowsC[i] = new int[rowWidth];
            }

            // Fill the first row
            cachedRowsC[0][0] = 0;

            for (int j = 1, t = 0; j < rowWidth; j++)
            {
                t += gapOpenCost;
                cachedRowsC[0][j] = t;
            }

            // Preallocate column cache
            cachedColsC = new int[gridCols][];
            for (int j = 0; j < gridCols; j++)
            {
                cachedColsC[j] = new int[colHeight];
            }

            // Fill the first column
            cachedColsC[0][0] = 0;

            for (int i = 1, t = 0; i < colHeight; i++)
            {
                t += gapOpenCost;
                cachedColsC[0][i] = t;
            }
        }

        /// <summary>
        /// Initializes grid cache for the simple alignment.
        /// Sets the boundary weights to zero.
        /// </summary>
        protected void InitializeCacheSimpleZero()
        {
            // Preallocate row cache
            cachedRowsC = new int[gridRows][];
            for (int i = 0; i < gridRows; i++)
            {
                cachedRowsC[i] = new int[rowWidth];
            }

            // Fill the first row
            for (int j = 0; j < rowWidth; j++)
            {
                cachedRowsC[0][j] = 0;
            }

            // Preallocate column cache
            cachedColsC = new int[gridCols][];
            for (int j = 0; j < gridCols; j++)
            {
                cachedColsC[j] = new int[colHeight];
            }

            // Fill the first column
            for (int i = 0; i < colHeight; i++)
            {
                cachedColsC[0][i] = 0;
            }
        }

        /// <summary>
        /// Initializes grid cache for the affine alignment.
        /// </summary>
        protected void InitializeCacheAffine()
        {
            // Preallocate row cache
            cachedRowsC = new int[gridRows][];
            cachedRowsD = new int[gridRows][];
            cachedRowsI = new int[gridRows][];
            for (int i = 0; i < gridRows; i++)
            {
                cachedRowsC[i] = new int[rowWidth];
                cachedRowsD[i] = new int[rowWidth];
                cachedRowsI[i] = new int[rowWidth];
            }

            // Fill the first row
            cachedRowsC[0][0] = 0;
            cachedRowsD[0][0] = int.MinValue / 2;
            cachedRowsI[0][0] = int.MinValue / 2;

            for (int j = 1, t = 0; j < rowWidth; j++)
            {
                cachedRowsC[0][j] = t + gapOpenCost;
                cachedRowsD[0][j] = int.MinValue / 2;
                cachedRowsI[0][j] = int.MinValue / 2;
                t += gapExtensionCost;
            }

            // Preallocate column cache
            cachedColsC = new int[gridCols][];
            cachedColsD = new int[gridCols][];
            cachedColsI = new int[gridCols][];
            for (int j = 0; j < gridCols; j++)
            {
                cachedColsC[j] = new int[colHeight];
                cachedColsD[j] = new int[colHeight];
                cachedColsI[j] = new int[colHeight];
            }

            // Fill the first column
            cachedColsC[0][0] = 0;
            cachedColsD[0][0] = int.MinValue / 2;
            cachedColsI[0][0] = int.MinValue / 2;

            for (int i = 1, t = 0; i < colHeight; i++)
            {
                cachedColsC[0][i] = t + gapOpenCost;
                cachedColsD[0][i] = int.MinValue / 2;
                cachedColsI[0][i] = int.MinValue / 2;
                t += gapExtensionCost;
            }
        }

        /// <summary>
        /// Initializes grid cache for the affine alignment.
        /// Sets the boundary weights to zero.
        /// </summary>
        protected void InitializeCacheAffineZero()
        {
            // Preallocate row cache
            cachedRowsC = new int[gridRows][];
            cachedRowsD = new int[gridRows][];
            cachedRowsI = new int[gridRows][];
            for (int i = 0; i < gridRows; i++)
            {
                cachedRowsC[i] = new int[rowWidth];
                cachedRowsD[i] = new int[rowWidth];
                cachedRowsI[i] = new int[rowWidth];
            }

            // Fill the first row
            for (int j = 0; j < rowWidth; j++)
            {
                cachedRowsC[0][j] = 0;
                cachedRowsD[0][j] = int.MinValue / 2;
                cachedRowsI[0][j] = int.MinValue / 2;
            }

            // Preallocate column cache
            cachedColsC = new int[gridCols][];
            cachedColsD = new int[gridCols][];
            cachedColsI = new int[gridCols][];
            for (int j = 0; j < gridCols; j++)
            {
                cachedColsC[j] = new int[colHeight];
                cachedColsD[j] = new int[colHeight];
                cachedColsI[j] = new int[colHeight];
            }

            // Fill the first column
            for (int i = 0; i < colHeight; i++)
            {
                cachedColsC[0][i] = 0;
                cachedColsD[0][i] = int.MinValue / 2;
                cachedColsI[0][i] = int.MinValue / 2;
            }
        }

        /// <summary>
        /// Implementation of the linear programming model for the block using the simple gap cost model
        /// </summary>
        /// <param name="weightFunction">Algorith-specific weight function</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected void ComputeBlockSimple(WeightFunction weightFunction, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            int[] foldC = new int[2 * gridStride + 1];

            PopulateFold(cachedRowsC, cachedColsC, foldC, blockRow, blockCol, lastRow, lastCol);

            long startPositionI = Helper.BigMul(blockRow, gridStride) - 1;
            long startPositionJ = Helper.BigMul(blockCol, gridStride) - 1;

            for (int i = 1; i <= lastRow; i++)
            {
                long globalI = startPositionI + i;

                for (int j = 1, f = gridStride - i, Cij = foldC[f++]; j <= lastCol; j++, f++)
                {
                    long globalJ = startPositionJ + j;

                    // I
                    //int Iij = fold[f - 1] + gapOpenCost;
                    int Iij = Cij + gapOpenCost;

                    // D
                    int Dij = foldC[f + 1] + gapOpenCost;

                    // C
                    Cij = foldC[f] + similarityMatrix[sequenceI[globalI], sequenceJ[globalJ]];

                    Cij = weightFunction(i, j, Iij, Dij, Cij);

                    foldC[f] = Cij;
                }
            }

            WritebackFold(cachedRowsC, cachedColsC, foldC, blockRow, blockCol, lastRow, lastCol);
        }

        /// <summary>
        /// Implementation of the linear programming model for the block using the affine gap cost model
        /// </summary>
        /// <param name="weightFunction">Algorith-specific weight function</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected void ComputeBlockAffine(WeightFunction weightFunction, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            int[] foldC = new int[2 * gridStride + 1];
            int[] foldD = new int[2 * gridStride + 1];
            int[] foldI = new int[2 * gridStride + 1];

            PopulateFold(cachedRowsC, cachedColsC, foldC, blockRow, blockCol, lastRow, lastCol);
            PopulateFold(cachedRowsD, cachedColsD, foldD, blockRow, blockCol, lastRow, lastCol);
            PopulateFold(cachedRowsI, cachedColsI, foldI, blockRow, blockCol, lastRow, lastCol);

            long startPositionI = Helper.BigMul(blockRow, gridStride) - 1;
            long startPositionJ = Helper.BigMul(blockCol, gridStride) - 1;

            for (int i = 1; i <= lastRow; i++)
            {
                long globalI = startPositionI + i;

                for (int j = 1, f = gridStride - i, Cij = foldC[f++]; j <= lastCol; j++, f++)
                {
                    long globalJ = startPositionJ + j;

                    // I
                    int Iij = Math.Max(foldI[f - 1] + gapExtensionCost, Cij + gapOpenCost);

                    // D
                    int Dij = Math.Max(foldD[f + 1] + gapExtensionCost, foldC[f + 1] + gapOpenCost);

                    // C
                    Cij = foldC[f] + similarityMatrix[sequenceI[globalI], sequenceJ[globalJ]];

                    Cij = weightFunction(i, j, Iij, Dij, Cij);

                    foldC[f] = Cij;
                    foldD[f] = Dij;
                    foldI[f] = Iij;
                }
            }

            WritebackFold(cachedRowsC, cachedColsC, foldC, blockRow, blockCol, lastRow, lastCol);
            WritebackFold(cachedRowsD, cachedColsD, foldD, blockRow, blockCol, lastRow, lastCol);
            WritebackFold(cachedRowsI, cachedColsI, foldI, blockRow, blockCol, lastRow, lastCol);
        }

        /// <summary>
        /// Fills the fold data structure for the block from the grid cache
        /// </summary>
        /// <param name="cachedRows">Cache for the grid rows</param>
        /// <param name="cachedCols">Cache for the grid columns</param>
        /// <param name="fold">Fold data structure</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected void PopulateFold(int[][] cachedRows, int[][] cachedCols, int[] fold, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            //Fill Column
            for (long k = 0, cellIndex = Helper.BigMul(blockRow, gridStride), foldIndexx = gridStride; k <= lastRow; k++, cellIndex++, foldIndexx--)
            {
                fold[foldIndexx] = cachedCols[blockCol][cellIndex];
            }

            //Fill Row
            for (long k = 0, cellIndex = Helper.BigMul(blockCol, gridStride), foldIndexx = gridStride; k <= lastCol; k++, cellIndex++, foldIndexx++)
            {
                fold[foldIndexx] = cachedRows[blockRow][cellIndex];
            }
        }

        /// <summary>
        /// Writes the fold data structure for the block back to the grid cache
        /// </summary>
        /// <param name="cachedRows">Cache for the grid rows</param>
        /// <param name="cachedCols">Cache for the grid columns</param>
        /// <param name="fold">Fold data structure</param>
        /// <param name="blockRow">First index of the block within the grid</param>
        /// <param name="blockCol">Second index of the block within the grid</param>
        /// <param name="lastRow">Last valid row index within the block; rows beyond this index stay uninitialized</param>
        /// <param name="lastCol">Last valid column index within the block; columns beyond this index stay uninitialized</param>
        protected void WritebackFold(int[][] cachedRows, int[][] cachedCols, int[] fold, int blockRow, int blockCol, int lastRow, int lastCol)
        {
            // Writeback Row
            int bottomNeighbor = blockRow + 1;
            if (bottomNeighbor < gridRows)
            {
                for (long k = 0, cellIndex = Helper.BigMul(blockCol, gridStride), foldIndexx = 0; k <= lastCol; k++, cellIndex++, foldIndexx++)
                {
                    cachedRows[bottomNeighbor][cellIndex] = fold[foldIndexx];
                }
            }

            // Writeback Column
            int rightNeighbor = blockCol + 1;
            if (rightNeighbor < gridCols)
            {
                for (long k = 0, cellIndex = Helper.BigMul(blockRow, gridStride), foldIndexx = 2 * gridStride; k <= lastRow; k++, cellIndex++, foldIndexx--)
                {
                    cachedCols[rightNeighbor][cellIndex] = fold[foldIndexx];
                }
            }
        }
    }
}
