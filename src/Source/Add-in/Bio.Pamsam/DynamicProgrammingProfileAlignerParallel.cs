using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Base class for dynamic programming (DP) profile alignment algorithms, including 
    /// *Profile* NeedlemanWunsch, SmithWaterman and PairwiseOverlap.
    /// 
    /// Unlike traditional dynamic programming, profile DP takes two profiles to do the alignment.
    /// Correspondingly, the similarity score of two profiles (two distribution of items)
    /// needs to be calculated instead of looking up 'SimilarityMatrix'.
    /// 
    /// Modifications based on pairwise Dynamic Programming:
    /// 
    /// - data type changes to float to accomodate function score
    /// - bytearray _a, and _b store position index and gap code instead of encodings
    /// - aligned bytearray aAligned, bAligned are class members
    /// - gap code is -1 (255 before)
    /// - aligned bytearray is then used to constructed aligned profiles (not sequences)
    /// - aligned pairwise sequences are not created after DP, 
    ///   instead eStrings are created for each pairwise alignment, storing alignment operations
    ///   which will be converted to aligned sequences at the final stage when all sequences
    ///   are aligned
    /// - consensus sequence: at each column, use the majority item to represent the positin
    /// - profile-profile function is a parameter
    /// 
    /// </summary>
    abstract public class DynamicProgrammingProfileAlignerParallel : IProfileAligner
    {
        #region Member variables

        /// <summary>
        /// The delegate of profile-profile score function.
        /// </summary>
        protected ProfileScoreFunctionSelector _profileProfileScoreFunction;

        /// <summary>
        /// The delegate of caching function.
        /// </summary>
        protected CachingFunctionSelector _cachingFunction;

        // Whether do caching.
        private bool _doCaching = false;

        /// <summary>
        /// Check out DynamicProgrammingPairwiseAligner.
        /// change to float
        /// </summary>
        protected float[,] _FScore;

        /// <summary>
        /// Check out DynamicProgrammingPairwiseAligner.
        /// change to float
        /// </summary>
        protected sbyte[,] _FSource; // source for cell

        /// <summary>
        /// Check out DynamicProgrammingPairwiseAligner.
        /// change to float
        /// </summary>
        protected float[,] _M;

        /// <summary>
        /// Check out DynamicProgrammingPairwiseAligner.
        /// change to float
        /// </summary>
        protected float[,] _Ix;

        /// <summary>
        /// Check out DynamicProgrammingPairwiseAligner.
        /// change to float
        /// </summary>
        protected float[,] _Iy;

        /// <summary>
        /// Check out DynamicProgrammingPairwiseAligner.
        /// change to float
        /// </summary>
        protected int[] _a, _b;


        /// <summary>
        /// Similarity matrix used in scoring function. Set using property SimilarityMatrix below.
        /// </summary>
        protected SimilarityMatrix _similarityMatrix;

        /// <summary>
        /// Gap open penalty for use in alignment algorithms. 
        /// For alignments using a single gap penalty, this is the gap penalty.
        /// For alignments using an affine gap, this is the penalty to open a new gap.
        /// This is a negative number, for example _gapOpenPenalty = -8, not +8.
        /// Set when calling full AlignSimple method, or use property GapOpenCost below.
        /// </summary>
        protected int _gapOpenPenalty;

        /// <summary>
        /// Gap extension penalty for use in alignment algorithms. 
        /// Not used for alignments using a single gap penalty.
        /// For alignments using an affine gap, this is the penalty to extend an existing gap.
        /// This is a negative number, for example _gapExtensionPenalty = -2, not +2.
        /// Set when calling full Align method, or use property GapExtensionCost below.
        /// </summary>
        protected int _gapExtensionPenalty;

        /// <summary>
        /// Number of rows and columns in the dynamic programming matrix.
        /// </summary>
        protected int _nCols, _nRows;

        /// <summary>
        /// Signifies gap in aligned sequence (stored as int[]) during traceback.
        /// </summary>
        protected const int _gapCode = -1;  // Used in internal sequences.  ISequenceItem uses byte, so cannot use -1.        

        // Return aligned sequence as a vector of integer:
        // positive integer N means it's original the Nth letter;
        // negative integer -1 means it's an indel.
        // 
        // These two vectors are used to convert to eString
        private int[] _alignedA, _alignedB;

        // The profileAlignments to be aligned
        private IProfileAlignment _profileAlignmentA, _profileAlignmentB;

        // The number of partitions for parallelization
        // It is set to 1 for non-parallelization, and set to the number of processor cores for parallelization.
        private int _numberOfPartitions = 1;

        // The weights of the two sets of profiles
        private float[] _weights;

        // Used to record profile index when sorting
        private int[] _indexA, _indexB;

        // Cache profile index when sorting
        private int[][] _indexAs, _indexBs;

        // Cached the multification of one profile alignment and the similarity matrix
        private float[][] _cachedMatrix;

        /// <summary>
        /// The weights of the two sets of profiles
        /// </summary>
        public float[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <summary>
        /// The aligned vector of the first sequence indices of this class
        /// </summary>
        public int[] AlignedA
        {
            get { return _alignedA; }
        }
        /// <summary>
        /// The aligned vector of the second sequence indices of this class
        /// </summary>
        public int[] AlignedB
        {
            get { return _alignedB; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for all the pairwise aligner (NeedlemanWunsch, SmithWaterman, Overlap).
        /// Sets default similarity matrix and gap penalties.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// 
        /// The default numberOfPartitions is the number of cores in the system.
        /// The default profile function is WeightedInnerProduct.
        /// </summary>
        public DynamicProgrammingProfileAlignerParallel()
            : this(new DiagonalSimilarityMatrix(2, -2),
                        ProfileScoreFunctionNames.WeightedInnerProduct,
                        -8, -1, Environment.ProcessorCount)
        {
            // Set default similarity matrix and gap penalty.
            // User will typically choose their own parameters, these defaults are reasonable for many cases.
            // Molecule type is set to protein, since this will also work for DNA and RNA in the
            // special case of a diagonal similarity matrix.


            //_similarityMatrix = new DiagonalSimilarityMatrix(2, -2, MoleculeType.DNA);
            //_profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedInnerProduct);
            //_gapOpenPenalty = -8;
            //_gapExtensionPenalty = -1;
        }

        /// <summary>
        /// Constructor for all the pairwise aligner (NeedlemanWunsch, SmithWaterman, Overlap).
        /// Sets default similarity matrix and gap penalties.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// 
        /// This constructor is for non-parallel version.
        /// </summary>
        public DynamicProgrammingProfileAlignerParallel(
                                SimilarityMatrix similarityMatrix,
                                ProfileScoreFunctionNames profileScoreFunctionName,
                                int gapOpenPenalty,
                                int gapExtensionPenalty)
            : this(similarityMatrix, profileScoreFunctionName, gapOpenPenalty, gapExtensionPenalty, 1)
        {
        }

        /// <summary>
        /// Constructor for all the pairwise aligner (NeedlemanWunsch, SmithWaterman, Overlap).
        /// Sets default similarity matrix and gap penalties.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        public DynamicProgrammingProfileAlignerParallel(
                                SimilarityMatrix similarityMatrix,
                                ProfileScoreFunctionNames profileScoreFunctionName,
                                int gapOpenPenalty,
                                int gapExtensionPenalty,
                                int numberOfCores)
        {
            // Set default similarity matrix and gap penalty.
            // User will typically choose their own parameters, these defaults are reasonable for many cases.
            // Molecule type is set to protein, since this will also work for DNA and RNA in the
            // special case of a diagonal similarity matrix.
            _similarityMatrix = similarityMatrix;

            _gapOpenPenalty = gapOpenPenalty;
            _gapExtensionPenalty = gapExtensionPenalty;

            switch (profileScoreFunctionName)
            {
                case (ProfileScoreFunctionNames.InnerProduct):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(InnerProduct);
                    break;
                case (ProfileScoreFunctionNames.WeightedInnerProduct):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedInnerProduct);
                    break;
                case (ProfileScoreFunctionNames.WeightedInnerProductShifted):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedInnerProductShifted);
                    break;
                case (ProfileScoreFunctionNames.InnerProductFast):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(InnerProductFast);
                    break;
                case (ProfileScoreFunctionNames.WeightedInnerProductFast):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedInnerProductFast);
                    break;
                case (ProfileScoreFunctionNames.WeightedInnerProductShiftedFast):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedInnerProductShiftedFast);
                    break;
                case (ProfileScoreFunctionNames.PearsonCorrelation):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(PearsonCorrelation);
                    break;
                case (ProfileScoreFunctionNames.WeightedEuclideanDistance):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedEuclideanDistance);
                    break;
                case (ProfileScoreFunctionNames.LogExponentialInnerProduct):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(LogExponentialInnerProduct);
                    break;
                case (ProfileScoreFunctionNames.LogExponentialInnerProductShifted):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(LogExponentialInnerProductShifted);
                    break;
                case (ProfileScoreFunctionNames.WeightedEuclideanDistanceFast):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedEuclideanDistanceFast);
                    break;
                case (ProfileScoreFunctionNames.LogExponentialInnerProductFast):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(LogExponentialInnerProductFast);
                    break;
                case (ProfileScoreFunctionNames.LogExponentialInnerProductShiftedFast):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(LogExponentialInnerProductShiftedFast);
                    break;
                case (ProfileScoreFunctionNames.SymmetrizedEntropy):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(SymmetrizedEntropy);
                    break;
                case (ProfileScoreFunctionNames.JensenShannonDivergence):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(JensenShannonDivergence);
                    break;
                case (ProfileScoreFunctionNames.WeightedInnerProductCached):
                    _profileProfileScoreFunction = new ProfileScoreFunctionSelector(WeightedInnerProductCached);
                    _cachingFunction = new CachingFunctionSelector(CachingWeightedInnerProduct);
                    _doCaching = true;
                    break;

                default:
                    throw new Exception("Invalid profile function name");
            }

            if (numberOfCores <= 0)
            {
                throw new ArgumentException("Invalid number of cores parameter");
            }
            _numberOfPartitions = numberOfCores;

        }
        #endregion

        #region Methods
        /// <summary>
        /// Modified AlignSimple to align two profiles with constant gap panelty.
        /// </summary>
        /// <param name="profileA">First input sequence.</param>
        /// <param name="profileB">Second input sequence.</param>
        /// <returns>Object containing the alignment.</returns>
        public IProfileAlignment AlignSimple(IProfileAlignment profileA, IProfileAlignment profileB)
        {
            IProfileAlignment result;
            result = (ProfileAlignment)AlignSimple(_similarityMatrix, _gapOpenPenalty, profileA, profileB);
            return result;
        }

        /// <summary>
        /// Modified Align to align two profiles with affine gap panelty.
        /// </summary>
        /// <param name="profileA">first input profile</param>
        /// <param name="profileB">second input profile</param>
        public IProfileAlignment Align(IProfileAlignment profileA, IProfileAlignment profileB)
        {
            IProfileAlignment result;
            result = (ProfileAlignment)Align(_similarityMatrix, _gapOpenPenalty, _gapExtensionPenalty, profileA, profileB);
            return result;
        }

        /// <summary>
        /// Pairwise alignment of two sequences using a single gap penalty.  The various algorithms in derived classes (NeedlemanWunsch, 
        /// SmithWaterman, and PairwiseOverlap) all use this general engine for alignment with a single gap penalty.
        /// </summary>
        /// <param name="similarityMatrix">Scoring matrix.</param>
        /// <param name="gapPenalty">Gap penalty (by convention, use a negative number for this.)</param>
        /// <param name="profileAlignmentA">First input profileAlignment.</param>
        /// <param name="profileAlignmentB">Second input profileAlignment.</param>
        /// <returns>Aligned sequences and other information as SequenceAlignment object.</returns>
        public IProfileAlignment AlignSimple(
            SimilarityMatrix similarityMatrix,
            int gapPenalty,
            IProfileAlignment profileAlignmentA,
            IProfileAlignment profileAlignmentB)
        {
            if (profileAlignmentA == null)
            {
                throw new ArgumentNullException("profileAlignmentA");
            }

            if (profileAlignmentB == null)
            {
                throw new ArgumentNullException("profileAlignmentB");
            }

            _profileAlignmentA = profileAlignmentA;
            _profileAlignmentB = profileAlignmentB;

            ResetSpecificAlgorithmMemberVariables();
            // Set Gap Penalty and Similarity Matrix
            GapOpenCost = gapPenalty;
            // note that GapExtensionCost is not used for simple gap penalty
            SimilarityMatrix = similarityMatrix;

            ValidateAlignInput(profileAlignmentA, profileAlignmentB);  // throws exception if input not valid

            // Convert input strings to 0-based int arrays using similarity matrix mapping
            _a = MsaUtils.CreateIndexArray(profileAlignmentA.ProfilesMatrix.RowSize);
            _b = MsaUtils.CreateIndexArray(profileAlignmentB.ProfilesMatrix.RowSize);

            if (_doCaching)
            {
                _cachingFunction(similarityMatrix, profileAlignmentA, profileAlignmentB);
            }
            else
            {
                _indexAs = CachingIndex(profileAlignmentA);
                _indexBs = CachingIndex(profileAlignmentB);
            }

            FillMatrixSimple();

            //DumpF();  // Writes F-matrix to application log, used for development and testing

            float optScore = Traceback(out _alignedA, out _alignedB);

            #region Convert aligned sequences back to Sequence objects, load output SequenceAlignment object
            ProfileAlignment results = null;
            if (PAMSAMMultipleSequenceAligner.UseWeights)
            {
                results = (ProfileAlignment)ProfileAlignment.GenerateProfileAlignment(profileAlignmentA, profileAlignmentB, _alignedA, _alignedB, _gapCode, _weights);
            }
            else
            {
                results = (ProfileAlignment)ProfileAlignment.GenerateProfileAlignment(profileAlignmentA, profileAlignmentB, _alignedA, _alignedB, _gapCode);
            }
            results.Score = optScore;
            //AddSimpleConsensusToResult(results);
            #endregion

            return results;
        }

        /// <summary>
        /// Pairwise alignment of two sequences using an affine gap penalty.  The various algorithms in derived classes (NeedlemanWunsch, 
        /// SmithWaterman, and PairwiseOverlap) all use this general engine for alignment with an affine gap penalty.
        /// </summary>
        /// <param name="similarityMatrix">Scoring matrix.</param>
        /// <param name="gapOpenPenalty">Gap open penalty (by convention, use a negative number for this.)</param>
        /// <param name="gapExtensionPenalty">Gap extension penalty (by convention, use a negative number for this.)</param>
        /// <param name="profileAlignmentA">First input profileAlignment</param>
        /// <param name="profileAlignmentB">Second input profileAlignment</param>
        public IProfileAlignment Align(
            SimilarityMatrix similarityMatrix,
            int gapOpenPenalty,
            int gapExtensionPenalty,
            IProfileAlignment profileAlignmentA,
            IProfileAlignment profileAlignmentB)
        {
            if (profileAlignmentA == null)
            {
                throw new ArgumentNullException("profileAlignmentA");
            }

            if (profileAlignmentB == null)
            {
                throw new ArgumentNullException("profileAlignmentB");
            }

            _profileAlignmentA = profileAlignmentA;
            _profileAlignmentB = profileAlignmentB;

            ResetSpecificAlgorithmMemberVariables();
            // Set Gap Penalty and Similarity Matrix
            GapOpenCost = gapOpenPenalty;
            GapExtensionCost = gapExtensionPenalty;
            SimilarityMatrix = similarityMatrix;

            ValidateAlignInput(profileAlignmentA, profileAlignmentB);  // throws exception if input not valid

            // Convert input strings to 0-based int arrays using similarity matrix mapping
            _a = MsaUtils.CreateIndexArray(profileAlignmentA.ProfilesMatrix.RowSize);
            _b = MsaUtils.CreateIndexArray(profileAlignmentB.ProfilesMatrix.RowSize);

            if (_doCaching)
            {
                _cachingFunction(similarityMatrix, _profileAlignmentA, _profileAlignmentB);
            }

            // Sort profileA
            _indexAs = CachingIndex(_profileAlignmentA);

            FillMatrixAffine();

            //DumpF();  // Writes matrix to application log, used for development and testing
            //DumpAffine(); // Writes matrix to application log in great detail.  Useful only for small cases.

            float optScore = Traceback(out _alignedA, out _alignedB);

            #region Convert aligned sequences back to Sequence objects, load output SequenceAlignment object
            ProfileAlignment results = null;

            //AddSimpleConsensusToResult(results);
            if (PAMSAMMultipleSequenceAligner.UseWeights)
            {
                results = (ProfileAlignment)ProfileAlignment.GenerateProfileAlignment(_profileAlignmentA, _profileAlignmentB, _alignedA, _alignedB, _gapCode, _weights);
            }
            else
            {
                results = (ProfileAlignment)ProfileAlignment.GenerateProfileAlignment(_profileAlignmentA, _profileAlignmentB, _alignedA, _alignedB, _gapCode);
            }
            results.Score = optScore;
            #endregion

            return results;
        }

        /// <summary>
        /// Performs traceback step for the relevant algorithm.  Each algorithm must override this
        /// since the traceback differs for the different algorithms.
        /// </summary>
        /// <param name="aAligned">First aligned sequence (output)</param>
        /// <param name="bAligned">Second aligned sequenc (output)</param>
        /// <returns>Optimum score for this alignment</returns>
        abstract protected float Traceback(out int[] aAligned, out int[] bAligned); // perform algorithm's traceback step

        /// <summary>
        /// Fills F matrix for single gap penalty implementation.
        /// </summary>
        protected void FillMatrixSimple()
        {
            _nCols = _a.Length + 1;
            _nRows = _b.Length + 1;
            try
            {
                _FScore = new float[_nCols, _nRows];
                _FSource = new sbyte[_nCols, _nRows];
                _M = null;  // affine matrices not used for simple model
                _Ix = null;
                _Iy = null;
            }
            catch (OutOfMemoryException ex)
            {
                string msg = BuildOutOfMemoryMessage(ex, false);
                ApplicationLog.WriteLine(msg);
                throw new Exception(msg);
            }

            // Set matrix bc along top row and left column.
            SetBoundaryConditionSimple();

            // Fill by columns
            // Parallel version
            int numberOfIterations = _numberOfPartitions * 2 - 1;

            Dictionary<int, List<int[]>> parallelIndexMaster = ParallelIndexMasterGenerator(_nRows, _nCols, _numberOfPartitions);

            for (int i = 0; i < numberOfIterations; ++i)
            {
                foreach (var pair in parallelIndexMaster)
                {
                    List<int[]> indexPositions = parallelIndexMaster[pair.Key];

                    // Parallel in anti-diagonal direction
                    Parallel.ForEach(indexPositions, PAMSAMMultipleSequenceAligner.ParallelOption, indexPosition =>
                    {
                        int[] rowPositions = IndexLocator(1, _nRows, _numberOfPartitions, indexPosition[0]);
                        int[] colPositions = IndexLocator(1, _nCols, _numberOfPartitions, indexPosition[0]);
                        for (int col = colPositions[0]; col < colPositions[1]; col++)
                        {
                            for (int row = rowPositions[0]; row < rowPositions[1]; row++)
                            {
                                FillCellSimple(col, row);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Fills matrix data for affine gap penalty implementation.
        /// </summary>
        protected void FillMatrixAffine()
        {
            _nCols = _a.Length + 1;
            _nRows = _b.Length + 1;
            try
            {
                _FScore = null; // not used for affine model
                _FSource = new sbyte[_nCols, _nRows];

                _M = new float[_nCols, _nRows];
                _Ix = new float[_nCols, _nRows];
                _Iy = new float[_nCols, _nRows];

            }
            catch (OutOfMemoryException ex)
            {
                string msg = BuildOutOfMemoryMessage(ex, true);
                ApplicationLog.WriteLine(msg);
                throw new Exception(msg);
            }

            // Set matrix bc along top row and left column.
            SetBoundaryConditionAffine();

            // Fill by columns
            // Parallel version
            int numberOfIterations = _numberOfPartitions * 2 - 1;

            Dictionary<int, List<int[]>> parallelIndexMaster = ParallelIndexMasterGenerator(_nRows, _nCols, _numberOfPartitions);

            for (int i = 0; i < numberOfIterations; ++i)
            {
                List<int[]> indexPositions = parallelIndexMaster[i];

                // Parallel in anti-diagonal direction
                Parallel.ForEach(indexPositions, PAMSAMMultipleSequenceAligner.ParallelOption, indexPosition =>
                {
                    int[] rowPositions = IndexLocator(1, _nRows, _numberOfPartitions, indexPosition[0]);
                    int[] colPositions = IndexLocator(1, _nCols, _numberOfPartitions, indexPosition[1]);
                    for (int col = colPositions[0]; col < colPositions[1]; col++)
                    {
                        for (int row = rowPositions[0]; row < rowPositions[1]; row++)
                        {
                            FillCellAffine(col, row);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Modified for profiles.
        /// </summary>
        /// <param name="aInput">First input sequence.</param>
        /// <param name="bInput">Second input sequence.</param>
        protected void ValidateAlignInput(IProfileAlignment aInput, IProfileAlignment bInput)
        {
            if (aInput.ProfilesMatrix.ColumnSize != bInput.ProfilesMatrix.ColumnSize)
            {
                throw new Exception("Input profiles have different column sizes");
            }
            // Warning if gap penalty > 0
            if (_gapOpenPenalty > 0)
            {
                ApplicationLog.WriteLine("Gap Open Penalty {0} > 0, possible error", _gapOpenPenalty);
            }
            if (_gapExtensionPenalty > 0)
            {
                ApplicationLog.WriteLine("Gap Extension Penalty {0} > 0, possible error", _gapExtensionPenalty);
            }
        }

        /// <summary>
        /// Builds detailed error message for OutOfMemory exception.
        /// </summary>
        /// <param name="ex">Exception.</param>
        /// <param name="isAffine">True for affine case, false for one gap penalty.</param>
        /// <returns>Message to send to user.</returns>
        private string BuildOutOfMemoryMessage(Exception ex, bool isAffine)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            // memory required is about 5 * N*M for simple gap penalty, 13 * N*M for affine gap
            sb.AppendFormat("Sequence lengths are {0:N0} and {1:N0}.", _a.Length, _b.Length);
            sb.AppendLine();
            sb.AppendLine("Dynamic programming algorithms are order NxM in memory use, with N and M the sequence lengths.");
            // Large sequences can easily overflow an int.  Use intermediate variables to avoid hard-to-read casts.
            long factor = (isAffine) ? 13 : 5;
            long estimatedMemory = (long)_nCols * (long)_nRows * factor;
            double estimatedGig = (estimatedMemory) / 1073741824.0;
            sb.AppendFormat("Current problem requires about {0:N0} bytes (approx {1:N2} Gbytes) of free memory.", estimatedMemory, estimatedGig);
            sb.AppendLine();
            return sb.ToString();
        }

        /*
        /// <summary>
        /// Improved consensus generation.
        /// Find the majority of the column, and use it as the consensus charactor
        /// </summary>
        /// <param name="alignment">profile alignment</param>
        private void AddSimpleConsensusToResult(IProfileAlignment alignment)
        {
            Sequence consensusSeq = new Sequence(alignment.Consensus.Alphabet);
            for (int i = 0; i < alignment.ProfilesMatrix.RowSize; ++i)
            {
                //consensusSeq.Add(alignment.ProfilesMatrix.ItemSet[MsaUtils.FindMaxIndex<float>(alignment.ProfilesMatrix[i])]);
            }
            alignment.Consensus = consensusSeq;
        }
        */
        /// <summary>
        /// Modified.
        /// Sets general case cell score and source for one gap parameter.
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        /// <returns>score for cell</returns>
        protected float SetCellValuesSimple(int col, int row)
        {
            float diagScore = _FScore[col - 1, row - 1] + _profileProfileScoreFunction(_similarityMatrix, _a[col - 1], _b[row - 1]);
            float upScore = _FScore[col, row - 1] + _gapOpenPenalty;
            float leftScore = _FScore[col - 1, row] + _gapOpenPenalty;
            if (diagScore >= upScore)
            {
                if (diagScore >= leftScore)
                {
                    // use diag
                    _FScore[col, row] = diagScore;
                    _FSource[col, row] = 0;
                }
                else
                {
                    // use left
                    _FScore[col, row] = leftScore;
                    _FSource[col, row] = 2;
                }
            }
            else if (upScore >= leftScore)
            {
                // use up
                _FScore[col, row] = upScore;
                _FSource[col, row] = 1;
            }
            else
            {
                // use left
                _FScore[col, row] = leftScore;
                _FSource[col, row] = 2;
            }
            return _FScore[col, row];
        }

        /// <summary>
        /// Modified.
        /// Sets general case cell score and matrix elements for general affine gap case.
        /// </summary>
        /// <param name="col">col of cell</param>
        /// <param name="row">row of cell</param>
        /// <returns>score for cell</returns>
        protected float SetCellValuesAffine(int col, int row)
        {
            if (col == 0 || row == 0) throw new Exception("Illegal col or row in SetCellAffineValues");

            float score;
            float M, Ix, Iy;

            // M is max of [(M, Ix, Iy) + sm] at col-1, row-1 FIX
            M = (float)Math.Max(_Ix[col - 1, row - 1], _Iy[col - 1, row - 1]);

            // Gap close penalty
            /*
            if (row == 1 || col == 1)
            {
                M = (float)Math.Max(_Ix[col - 1, row - 1] + (_gapOpenPenalty - GapExtensionCost) / 2, _Iy[col - 1, row - 1] + (_gapOpenPenalty - GapExtensionCost) / 2);
            }
            else
            {
                M = (float)Math.Max(_Ix[col - 1, row - 1] + _gapOpenPenalty - GapExtensionCost, _Iy[col - 1, row - 1] + _gapOpenPenalty - GapExtensionCost);
            }
            */
            M = (float)Math.Max(M, _M[col - 1, row - 1]);
            M += _profileProfileScoreFunction(_similarityMatrix, _a[col - 1], _b[row - 1]);

            // Terminal gaps (both ends) have half penalty as internal gaps
            if (row == _nRows - 1)
            {
                Ix = (float)Math.Max(_M[col - 1, row] + (_gapOpenPenalty / 2), _Ix[col - 1, row] + (_gapExtensionPenalty / 2));
            }
            else
            {
                Ix = (float)Math.Max(_M[col - 1, row] + _gapOpenPenalty, _Ix[col - 1, row] + _gapExtensionPenalty);
            }
            if (col == _nCols - 1)
            {
                Iy = (float)Math.Max(_M[col, row - 1] + (_gapOpenPenalty / 2), _Iy[col, row - 1] + (_gapExtensionPenalty / 2));
            }
            else
            {
                Iy = (float)Math.Max(_M[col, row - 1] + _gapOpenPenalty, _Iy[col, row - 1] + _gapExtensionPenalty);
            }

            // Score for this cell is max(M, Ix, Iy)
            score = (float)Math.Max(Ix, Iy);
            score = (float)Math.Max(score, M);
            /*
            sbyte source = SourceDirection.Invalid;
            if (score == M)
            {
                source = SourceDirection.Diagonal;
            }
            else if (score == Ix)
            {
                source = SourceDirection.Left;
            }
            else if (score == Iy)
            {
                source = SourceDirection.Up;
            }
            */
            sbyte source = SourceDirection.Invalid;
            if (score == Iy)
            {
                source = SourceDirection.Up;
            }
            else if (score == Ix)
            {
                source = SourceDirection.Left;
            }
            else if (score == M)
            {
                source = SourceDirection.Diagonal;
            }

            // Fill the various saved quantities
            _FSource[col, row] = source;
            _M[col, row] = M;
            _Ix[col, row] = Ix;
            _Iy[col, row] = Iy;

            return score;
        }

        // These routines will be different for the various variations --SmithWaterman, NeedlemanWunsch, etc.
        // Additonal variations can be built by modifying these functions.

        /// <summary>
        /// Sets cell (col,row) of the F matrix.  Different algorithms will use different scoring
        /// and traceback methods and therefore will override this method.
        /// </summary>
        /// <param name="col">col of cell to fill</param>
        /// <param name="row">row of cell to fill</param>
        abstract protected void FillCellSimple(int col, int row);

        /// <summary>
        /// Resets member variables that are unique to a specific algorithm.
        /// These must be reset for each alignment, initialization in the constructor
        /// only works for the first call to AlignSimple.  This routine is called at the beginning
        /// of each AlignSimple method.
        /// </summary>
        abstract protected void ResetSpecificAlgorithmMemberVariables();

        /// <summary>
        /// Sets cell (col,row) of the matrix for affine gap implementation.  Different algorithms will use different scoring
        /// and traceback methods and therefore will override this method.
        /// </summary>
        /// <param name="col">col of cell to fill</param>
        /// <param name="row">row of cell to fill</param>
        abstract protected void FillCellAffine(int col, int row);

        /// <summary>
        /// Sets boundary conditions in the F matrix for the one gap penalty case.  
        /// As in the FillCell methods, different algorithms will use different 
        /// boundary conditions and will override this method.
        /// </summary>
        abstract protected void SetBoundaryConditionSimple();
        /// <summary>
        /// Sets boundary conditions for the dynamic programming matrix for the affine gap penalty case.  
        /// As in the FillCell methods, different algorithms will use different 
        /// boundary conditions and will override this method.
        /// </summary>
        abstract protected void SetBoundaryConditionAffine();

        #endregion

        #region Nested Enums, Structs and Classes
        /// <summary> Direction to source of cell value, used during traceback. </summary>
        protected static class SourceDirection
        {
            // This is coded as a set of consts rather than using an enum.  Enums are ints and 
            // referring to these in the code requires casts to and from (sbyte), which makes
            // the code more difficult to read.

            /// <summary> Source was up and left from current cell. </summary>
            public const sbyte Diagonal = 0;
            /// <summary> Source was up from current cell. </summary>
            public const sbyte Up = 1;
            /// <summary> Source was left of current cell. </summary>
            public const sbyte Left = 2;
            /// <summary> During traceback, stop at this cell (used by SmithWaterman). </summary>
            public const sbyte Stop = -1;
            /// <summary> Error code, if cell has code Invalid an error has occurred. </summary>
            public const sbyte Invalid = -2;
        }
        #endregion

        #region Properties

        /// <summary> Sets similarity matrix for use in alignment algorithms. </summary>
        public SimilarityMatrix SimilarityMatrix
        {
            get
            {
                return _similarityMatrix;
            }
            set
            {
                _similarityMatrix = value;
            }
        }

        /// <summary> 
        /// Set gap open penalty for use in alignment algorithms. 
        /// For alignments using a single gap penalty, this is the gap penalty.
        /// For alignments using an affine gap, this is the penalty to open a new gap.
        /// This is a negative number, for example GapOpenCost = -8, not +8.
        /// </summary>
        public int GapOpenCost
        {
            get
            {
                return _gapOpenPenalty;
            }
            set
            {
                _gapOpenPenalty = value;
            }
        }

        /// <summary> 
        /// Set gap extension penalty for use in alignment algorithms. 
        /// Not used for alignments using a single gap penalty.
        /// For alignments using an affine gap, this is the penalty to extend an existing gap.
        /// This is a negative number, for example GapExtensionCost = -2, not +2.
        /// </summary>
        public int GapExtensionCost
        {
            get
            {
                return _gapExtensionPenalty;
            }
            set
            {
                _gapExtensionPenalty = value;
            }
        }
        #endregion

        #region EString related methods
        /// <summary>
        /// Defined in MUSCLE (Edgar 2004) paper.
        /// eString stores the operation of the child node sequence to become
        /// the aligned sequence in its parent node.
        /// 
        /// eString is a vector of integers: positive integer n means 
        /// skip n letters; negative integer -n means insert n indels 
        /// at the current position.
        /// 
        /// The alignment path of sequence (leaf node) is the series of 
        /// eString through internal nodes from this leaf to root (including
        /// leaf node).
        /// 
        /// with eString, there's no need to adjust the sequences until
        /// progressive alignment is finished.
        /// </summary>
        /// <param name="aligned">aligned integer array</param>
        /// <returns></returns>
        public List<int> GenerateEString(int[] aligned)
        {
            // generate eString
            List<int> eString = new List<int>();
            int counter = 0;

            for (int i = 0; i < aligned.Length; ++i)
            {
                if (aligned[i] == _gapCode)
                {
                    if (counter > 0)
                    {
                        eString.Add(counter);
                        counter = -1;
                    }
                    else
                    {
                        --counter;
                    }
                }
                else
                {
                    if (counter >= 0)
                    {
                        ++counter;
                    }
                    else
                    {
                        eString.Add(counter);
                        counter = 1;
                    }
                }
            }
            eString.Add(counter);
            return eString;
        }

        /// <summary>
        /// Apply alignment operations in eString to the sequence in order to
        /// generate an aligned sequence
        /// </summary>
        /// <param name="eString">estring with alignment path</param>
        /// <param name="seq">a sequece to aligned</param>
        public Sequence GenerateSequenceFromEString(List<int> eString, ISequence seq)
        {
            List<byte> seqbytes = new List<byte>();
            int x = 0, n;
            for (int i = 0; i < eString.Count; ++i)
            {
                n = eString[i];
                if (n > 0)
                {
                    while (n > 0)
                    {
                        seqbytes.Add(seq[x++]);
                        //++x;
                        --n;
                    }
                }
                else
                {
                    while (n < 0)
                    {
                        // Add Gaps at the end
                        seqbytes.Insert(seqbytes.Count, (byte)'-');
                        //result.Add((ISequenceItem)'-');
                        ++n;
                    }
                }
            }
            return  new Sequence(seq.Alphabet, seqbytes.ToArray())
            {
                ID = seq.ID,
                // Do not shallow copy dictionary
                //Metadata = seq.Metadata
            };

        }
        #endregion

        #region Caching

        /// <summary>
        /// Cache the multification of similarity matrix and one profiles.
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileAlignmentA">profile alignment A</param>
        /// <param name="profileAlignmentB">profile alignment B</param>
        public void CachingWeightedInnerProduct(SimilarityMatrix similarityMatrix, IProfileAlignment profileAlignmentA, IProfileAlignment profileAlignmentB)
        {
            if (profileAlignmentA == null)
            {
                throw new ArgumentNullException("profileAlignmentA");
            }

            if (profileAlignmentB == null)
            {
                throw new ArgumentNullException("profileAlignmentB");
            }

            int rowSize = profileAlignmentB.ProfilesMatrix.RowSize;
            int colSize = profileAlignmentB.ProfilesMatrix.ColumnSize;

            _cachedMatrix = new float[rowSize][];

            //for (int row = 0; row < rowSize; ++row)
            Parallel.For(0, rowSize, PAMSAMMultipleSequenceAligner.ParallelOption, row =>
            {
                _cachedMatrix[row] = new float[colSize];
                for (int i = 0; i < colSize; ++i)
                {
                    for (int j = 0; j < colSize; ++j)
                    {
                        _cachedMatrix[row][i] += profileAlignmentB.ProfilesMatrix[row][j] * similarityMatrix[i, j];
                    }
                }
            });
            //}
            //_indexAs = CachingIndex(profileAlignmentA);
        }

        /// <summary>
        /// Caching Indexes
        /// </summary>
        /// <param name="profileAlignment">profileAlignment</param>
        public int[][] CachingIndex(IProfileAlignment profileAlignment)
        {
            if (profileAlignment == null)
            {
                throw new ArgumentNullException("profileAlignment");
            }

            int rowSize = profileAlignment.ProfilesMatrix.RowSize;
            int colSize = profileAlignment.ProfilesMatrix.ColumnSize;
            int[][] _indexAs = new int[rowSize][];
            //for (int i = 0; i < rowSize; ++i)
            Parallel.For(0, rowSize, PAMSAMMultipleSequenceAligner.ParallelOption, i =>
            {
                MsaUtils.QuickSortM(profileAlignment.ProfilesMatrix[i], out _indexAs[i],
                                0, colSize - 1);
            });
            //}
            return _indexAs;
        }

        #endregion

        #region Debug and test methods
        /// <summary>
        /// Writes F matrix to application log.  Used for test and debugging.
        /// </summary>
        protected void DumpF()
        {
            int col, row;
            if (_FScore != null)
            {
                ApplicationLog.WriteLine("_FScore");
                for (row = 0; row < _nRows; row++)
                {
                    for (col = 0; col < _nCols; col++)
                    {
                        ApplicationLog.Write("{0,4}   ", _FScore[col, row]);
                    }
                    ApplicationLog.WriteLine("");
                }
            }
            if (_FSource != null)
            {
                ApplicationLog.WriteLine("");
                ApplicationLog.WriteLine("_FSource");

                for (row = 0; row < _nRows; row++)
                {
                    for (col = 0; col < _nCols; col++)
                    {
                        ApplicationLog.Write("{0,4} ", _FSource[col, row]);
                    }
                    ApplicationLog.WriteLine("");
                }
            }
            if (_M != null)
            {
                ApplicationLog.WriteLine("");
                ApplicationLog.WriteLine("_M");
                for (row = 0; row < _nRows; row++)
                {
                    for (col = 0; col < _nCols; col++)
                    {
                        ApplicationLog.Write("{0,4} ", _M[col, row]);
                    }
                    ApplicationLog.WriteLine("");
                }
            }
            if (_Ix != null)
            {
                ApplicationLog.WriteLine("");
                ApplicationLog.WriteLine("_Ix");
                for (row = 0; row < _nRows; row++)
                {
                    for (col = 0; col < _nCols; col++)
                    {
                        ApplicationLog.Write("{0,4} ", _Ix[col, row]);
                    }
                    ApplicationLog.WriteLine("");
                }
            }
            if (_Iy != null)
            {
                ApplicationLog.WriteLine("");
                ApplicationLog.WriteLine("_Iy");
                for (row = 0; row < _nRows; row++)
                {
                    for (col = 0; col < _nCols; col++)
                    {
                        ApplicationLog.Write("{0,4} ", _Iy[col, row]);
                    }
                    ApplicationLog.WriteLine("");
                }
            }

        }
        
        #endregion

        #region Profile function score
        // Check out enum ProfileScoreFunctionNames

        /// <summary>
        /// Weighted inner-profuct by similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedInnerProductCached(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            int dimension = profileA.Length - 1;
            float result = 0;

            _indexA = _indexAs[profileIndexA];

            for (int i = 0; i < dimension; ++i)
            {
                int ii = _indexA[i];
                if (profileA[ii] == 0)
                {
                    break;
                }
                result += profileA[ii] * _cachedMatrix[profileIndexB][ii];
            }
            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return result;
        }

        /// <summary>
        /// Weighted inner-profuct by similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedInnerProduct(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            int dimension = profileA.Length - 1;

            float[] cachedW = new float[dimension];

            for (int i = 0; i < dimension; ++i)
            //Parallel.For(0, dimension, PAMSAMMultipleSequenceAligner.parallelOption, i =>
            {
                for (int j = 0; j < dimension; ++j)
                {
                    cachedW[i] += similarityMatrix[i, j] * profileB[j];
                }
                //});
            }
            float result = 0;
            for (int i = 0; i < dimension; ++i)
            {
                result += profileA[i] * cachedW[i];
            }

            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return result;
        }

        /// <summary>
        /// Weighted inner-profuct by similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedInnerProductFast(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            int dimension = profileA.Length - 1;

            _indexA = MsaUtils.CreateIndexArray(dimension);
            _indexB = MsaUtils.CreateIndexArray(dimension);

            //MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            MsaUtils.QuickSort(profileA, _indexA, 0, dimension - 1);
            MsaUtils.QuickSort(profileB, _indexB, 0, dimension - 1);

            float result = 0;

            for (int i = 0; i < dimension; ++i)
            {
                if (profileA[_indexA[i]] == 0)
                {
                    break;
                }
                for (int j = 0; j < dimension; ++j)
                {
                    if (profileB[_indexB[j]] == 0)
                    {
                        break;
                    }
                    result += similarityMatrix[i, j] * profileB[j] * profileA[i];
                }
            }
            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return result;
        }

        /// <summary>
        /// Weighted Euclidean distance of observation vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedEuclideanDistance(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            int dimension = profileA.Length;
            float result = 0;
            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    result += (float)Math.Pow(profileA[i] - profileB[j], 2) * similarityMatrix[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Weighted Euclidean distance of observation vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedEuclideanDistanceFast(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }

            int dimension = profileA.Length;
            float result = 0;

            _indexA = MsaUtils.CreateIndexArray(dimension);
            _indexB = MsaUtils.CreateIndexArray(dimension);

            //MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            MsaUtils.QuickSort(profileA, _indexA, 0, dimension - 1);
            MsaUtils.QuickSort(profileB, _indexB, 0, dimension - 1);

            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    if (profileA[_indexA[i]] == 0 && profileB[_indexB[i]] == 0)
                    {
                        break;
                    }
                    result += (float)Math.Pow(profileA[i] - profileB[j], 2) * similarityMatrix[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Inner-profuct of two observed profile vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float InnerProduct(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            int dimension = profileA.Length;
            float result = 0;

            for (int i = 0; i < dimension; ++i)
            {
                result += profileA[i] * profileB[i] * similarityMatrix[i, i];
            }
            result *= (1 - profileA[profileA.Length - 1]) * (1 - profileB[profileB.Length - 1]);
            return result;
        }

        /// <summary>
        /// Inner-profuct of two observed profile vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float InnerProductFast(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }

            int dimension = profileA.Length;
            float result = 0;

            int[] _indexA = _indexAs[profileIndexA];
            for (int i = 0; i < dimension; ++i)
            {
                int ii = _indexA[i];
                if (profileA[ii] == 0)
                {
                    break;
                }
                result += profileA[ii] * profileB[ii];
            }
            return result;
        }

        /// <summary>
        /// Weighted inner-profuct by shifted similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedInnerProductShifted(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }

            int dimension = profileA.Length - 1;
            float[] cachedW = new float[dimension];

            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    cachedW[i] += (similarityMatrix[i, j] + (float)0.5) * profileB[j];
                }
            }
            float result = 0;
            for (int i = 0; i < dimension; ++i)
            {
                result += profileA[i] * cachedW[i];
            }
            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return result;
        }
        /// <summary>
        /// Weighted inner-profuct by shifted similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float WeightedInnerProductShiftedFast(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }

            int dimension = profileA.Length - 1;

            _indexA = MsaUtils.CreateIndexArray(dimension);
            _indexB = MsaUtils.CreateIndexArray(dimension);

            //MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            MsaUtils.QuickSort(profileA, _indexA, 0, dimension - 1);
            MsaUtils.QuickSort(profileB, _indexB, 0, dimension - 1);

            float result = 0;

            for (int i = 0; i < dimension; ++i)
            {
                if (profileA[_indexA[i]] == 0)
                {
                    break;
                }
                for (int j = 0; j < dimension; ++j)
                {
                    if (profileB[_indexB[j]] == 0)
                    {
                        break;
                    }
                    result += (similarityMatrix[i, j] + (float)0.5) * profileB[j] * profileA[i];
                }
            }
            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return result;
        }

        /// <summary>
        /// Correlation of observation vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float PearsonCorrelation(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            return MsaUtils.Correlation(profileA, profileB);
        }

        /// <summary>
        /// Log of Weighted inner-product by exponential of similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float LogExponentialInnerProduct(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            int dimension = profileA.Length - 1;
            float result = 0;
            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    result += profileA[i] * profileB[j] * (float)Math.Pow(2, similarityMatrix[i, j]);
                }
            }
            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return (float)Math.Log(result, 2);
        }

        /// <summary>
        /// Log of Weighted inner-product by exponential of similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float LogExponentialInnerProductFast(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }

            int dimension = profileA.Length - 1;
            float result = 0;

            _indexA = MsaUtils.CreateIndexArray(dimension);
            _indexB = MsaUtils.CreateIndexArray(dimension);

            //MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            MsaUtils.QuickSort(profileA, _indexA, 0, dimension - 1);
            MsaUtils.QuickSort(_profileAlignmentB.ProfilesMatrix[profileIndexB], _indexB, 0, dimension - 1);


            for (int i = 0; i < dimension; ++i)
            {
                if (profileA[_indexA[i]] == 0)
                {
                    break;
                }
                for (int j = 0; j < dimension; ++j)
                {
                    if (profileB[_indexB[j]] == 0)
                    {
                        break;
                    }
                    result += profileB[j] * profileA[i] * (float)Math.Pow(2, similarityMatrix[i, j]);
                }
            }

            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return (float)Math.Log(result, 2);
        }

        /// <summary>
        /// Log of Weighted inner-product by exponential of shifted similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float LogExponentialInnerProductShifted(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }
            int dimension = profileA.Length - 1;
            float result = 0;
            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    result += profileA[i] * profileB[j] * (float)Math.Pow(2, similarityMatrix[i, j] + 0.5);
                }
            }
            result *= (profileA[dimension]) * (1 - profileB[dimension]);
            return (float)Math.Log(result, 2);
        }

        /// <summary>
        /// Log of Weighted inner-product by exponential of shifted similarity matrix
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float LogExponentialInnerProductShiftedFast(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            float[] profileA = _profileAlignmentA.ProfilesMatrix[profileIndexA];
            float[] profileB = _profileAlignmentB.ProfilesMatrix[profileIndexB];
            if (profileA.Length != profileB.Length)
            {
                throw new ArgumentException("Unequal length profiles");
            }

            int dimension = profileA.Length - 1;
            float result = 0;

            _indexA = MsaUtils.CreateIndexArray(dimension);
            _indexB = MsaUtils.CreateIndexArray(dimension);

            //MsaUtils.QuickSort(a, aIndex, 0, a.Length - 1);
            MsaUtils.QuickSort(profileA, _indexA, 0, dimension - 1);
            MsaUtils.QuickSort(profileB, _indexB, 0, dimension - 1);


            for (int i = 0; i < dimension; ++i)
            {
                if (profileA[_indexA[i]] == 0)
                {
                    break;
                }
                for (int j = 0; j < dimension; ++j)
                {
                    if (profileB[_indexB[j]] == 0)
                    {
                        break;
                    }
                    result += profileB[j] * profileA[i] * (float)Math.Pow(2, similarityMatrix[i, j] + 0.5);
                }
            }

            result *= (1 - profileA[dimension]) * (1 - profileB[dimension]);
            return (float)Math.Log(result, 2);
        }
        /// <summary>
        /// Symmetrized entropy of observation vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float SymmetrizedEntropy(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            return MsaUtils.SymmetrizedEntropy(_profileAlignmentA.ProfilesMatrix[profileIndexA], _profileAlignmentB.ProfilesMatrix[profileIndexB]);
        }

        /// <summary>
        /// Jensen-Shannon divergence of observation vectors
        /// </summary>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="profileIndexA">the first profile vector (normalized)</param>
        /// <param name="profileIndexB">the second profile vector (normalized)</param>
        protected float JensenShannonDivergence(SimilarityMatrix similarityMatrix, int profileIndexA, int profileIndexB)
        {
            return 1 - MsaUtils.JensenShannonDivergence(_profileAlignmentA.ProfilesMatrix[profileIndexA], _profileAlignmentB.ProfilesMatrix[profileIndexB]);
        }

        #endregion

        #region Parallel Related Methods
        /// <summary>
        /// Divide a table into [numberOfPartitions x numberOfPartitions] blocks, and index them in anti-diagonal direction.
        /// Each block is indexed by [row, column] in the block table.
        /// 
        /// In the returned dictionary, the key is the index of a line of anti-diagonal blocks, and the value is the corresponding
        /// block index (row and column index).
        /// </summary>
        /// <param name="numberOfRows">the number of rows in the original table</param>
        /// <param name="numberOfCols">the number of columns in the original table</param>
        /// <param name="numberOfPartitions">the number of partitions in the row and column direction</param>
        /// <returns>a dictionary with anti-diagonal index and block indices</returns>
        public Dictionary<int, List<int[]>> ParallelIndexMasterGenerator(int numberOfRows, int numberOfCols, int numberOfPartitions)
        {
            if (numberOfRows <= 0 || numberOfCols <= 0 || numberOfPartitions <= 0)
            {
                throw new ArgumentException("Invalid numbers");
            }

            int numberOfIterations = numberOfPartitions * 2 - 1;

            Dictionary<int, List<int[]>> parallelIndexMaster = new Dictionary<int, List<int[]>>(numberOfIterations);

            for (int i = 0; i < numberOfIterations; ++i)
            {
                List<int[]> indexBlocks = new List<int[]>();

                for (int j = 0; j <= i; ++j)
                {
                    if (j < numberOfPartitions && i - j < numberOfPartitions)
                    {
                        indexBlocks.Add(new int[2] { j, i - j });
                    }
                }

                parallelIndexMaster.Add(i, indexBlocks);
            }

            return parallelIndexMaster;
        }

        /// <summary>
        /// Divide a range of numbers and return the start and end positions of one partition.
        /// </summary>
        /// <param name="startPosition">zero-based start position</param>
        /// <param name="endPosition">zero-based end position</param>
        /// <param name="numberOfPartitions">the number of partitions</param>
        /// <param name="index">the ith partition</param>
        /// <returns>return the start position and end position of the selected partition</returns>
        public int[] IndexLocator(int startPosition, int endPosition, int numberOfPartitions, int index)
        {
            if (startPosition < 0 || endPosition < 0 || index < 0 || numberOfPartitions < 0)
            {
                throw new ArgumentException("Negative numbers are not good");
            }
            if (endPosition < startPosition)
            {
                throw new ArgumentException("End position should be larger than the starting position");
            }

            int positionA = ((endPosition - startPosition) * index / numberOfPartitions) + startPosition;
            int positionB = ((endPosition - startPosition) * (index + 1) / numberOfPartitions) + startPosition;

            return new int[2] { positionA, positionB };
        }

        #endregion


    }
}
