using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bio.SimilarityMatrices;
using System;
using Bio.Util.Logging;
using Trace = Bio.Util.Logging.Trace;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Base class for our pair-wise sequence aligners. This implements the core shared 
    /// portions of the Smith-Waterman and Needleman-Wunsch aligners.
    /// </summary>
    public abstract class PairwiseSequenceAligner : IPairwiseSequenceAligner
    {
        #region Data
        /// <summary>
        /// Traceback table built during the matrix creation step
        /// </summary>
        protected sbyte[][] Traceback;

        /// <summary>
        /// Generated score table - this is filled in with the scoring matrix when debugging
        /// </summary>
        protected int[] ScoreTable;

        /// <summary>
        /// Rows in ScoreTable
        /// </summary>
        protected int Rows;

        /// <summary>
        /// Columns in ScoreTable
        /// </summary>
        protected int Cols;

        /// <summary>
        /// A variable to keep track of whether the traceback table was constructed with an affine gap model.
        /// </summary>
        protected bool usingAffineGapModel;

        /// <summary>
        /// This array keeps track of the length of gaps up to a point along the horizontal axis.
        /// Only used with the affine gap model
        /// </summary>
        protected int[] h_Gap_Length;
        
        /// <summary>
        /// This array keeps track of the length of gaps up to a point along the vertical axis.
        /// nly used with the affine gap model.
        /// </summary>
        protected int[] v_Gap_Length;

        /// <summary>
        /// The reference sequence being aligned (sequence #1)
        /// </summary>
        protected byte[] ReferenceSequence;

        /// <summary>
        /// The query sequence being aligned (sequence #2)
        /// </summary>
        protected byte[] QuerySequence;

        /// <summary>
        /// The gap character being used for the shared alphabet between the reference and query sequence.
        /// </summary>
        protected byte _gap;

        /// <summary>
        /// Original sequence
        /// </summary>
        protected ISequence _sequence1;

        /// <summary>
        /// Original sequence #2
        /// </summary>
        protected ISequence _sequence2;
        #endregion

        /// <summary>
        /// Gets the name of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the Description of the current Alignment algorithm used.
        /// This is a overridden property from the abstract parent.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// True to include the score table and matrix as part of the output.
        /// This is placed into the Metadata for the alignment. It is turned off by
        /// default due to the expense of generating it.
        /// </summary>
        public bool IncludeScoreTable { get; set; }

        /// <summary>
        /// Gets or sets value of similarity matrix
        /// The similarity matrix determines the score for any possible pair
        /// of symbols that are encountered at a common location across the 
        /// sequences being aligned.
        /// </summary>
        public SimilarityMatrix SimilarityMatrix { get; set; }

        /// <summary>
        /// Gets or sets value of GapOpenCost
        /// The GapOpenCost is the cost of inserting a gap character into 
        /// a sequence.
        /// </summary>
        public int GapOpenCost
        {
            get { return pGapOpenCost; }
            set
            {
                // Hard constrain on GapOpen/Extension costs, some programs have inputs of these be positive 
                // and then flip them internally so it is confusing.  Our users will know the difference right away.
                if (value >= 0)
                {
                    throw new ArgumentOutOfRangeException("GapOpenCost", "Gap Open Cost must be less than 0");
                }
                pGapOpenCost = value;
            }
        }
        private int pGapOpenCost;

        /// <summary>
        /// Gets or sets value of GapExtensionCost 
        /// The GapExtensionCost is the cost of extending an already existing gap.
        /// This is only used in the affine gap model
        /// </summary>
        public int GapExtensionCost
        {
            get 
            {
                return pGapExtensionCost;
            
            }
            set
            {
                if (value > 0)
                {
                    throw new ArgumentOutOfRangeException("GapExtensionCost", "Gap Extension Cost must be less than 0");
                }
                pGapExtensionCost = value;
            }
        } 
        private int pGapExtensionCost;

        /// <summary>
        /// Gets or sets the object that will be used to compute the alignment's consensus.
        /// </summary>
        public IConsensusResolver ConsensusResolver { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected PairwiseSequenceAligner()
        {
            SimilarityMatrix = new DiagonalSimilarityMatrix(2, -2);
            GapOpenCost = -8;
            GapExtensionCost = -1;
            IncludeScoreTable = false;
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>List of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(IEnumerable<ISequence> inputSequences)
        {
            if (inputSequences == null)
                throw new ArgumentNullException("inputSequences");

            var listOfSequences = inputSequences.ToList();
            if (listOfSequences.Count != 2)
            {
                string message = String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.PairwiseAlignerWrongArgumentCount,
                        listOfSequences.Count);
                Trace.Report(message);
                throw new ArgumentException(message, "inputSequences");
            }

            return Align(listOfSequences[0], listOfSequences[1]);
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>List of sequence alignments.</returns>
        IList<ISequenceAlignment> ISequenceAligner.Align(IEnumerable<ISequence> inputSequences)
        {
            return this.Align(inputSequences).Cast<ISequenceAlignment>().ToList();
        }

        /// <summary>
        /// AlignSimple aligns the set of input sequences using the linear gap model (one gap penalty), 
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>List of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            if (inputSequences == null)
                throw new ArgumentNullException("inputSequences");

            var listOfSequences = inputSequences.ToList();
            if (listOfSequences.Count != 2)
            {
                string message = String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.PairwiseAlignerWrongArgumentCount,
                        inputSequences.Count());
                Trace.Report(message);
                throw new ArgumentException(message, "inputSequences");
            }

            return AlignSimple(listOfSequences[0], listOfSequences[1]);
        }

        /// <summary>
        /// AlignSimple aligns the set of input sequences using the linear gap model (one gap penalty), 
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>List of sequence alignments.</returns>
        IList<ISequenceAlignment> ISequenceAligner.AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            return this.AlignSimple(inputSequences).Cast<ISequenceAlignment>().ToList();
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="sequence1">First sequence</param>
        /// <param name="sequence2">Second sequence</param>
        /// <returns>List of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(ISequence sequence1, ISequence sequence2)
        {
            return DoAlign(sequence1, sequence2, true);
        }

        /// <summary>
        /// AlignSimple aligns the set of input sequences using the linear gap model (one gap penalty), 
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="sequence1">First sequence</param>
        /// <param name="sequence2">Second sequence</param>
        /// <returns>List of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(ISequence sequence1, ISequence sequence2)
        {
            return DoAlign(sequence1, sequence2, false);
        }

        /// <summary>
        /// Pairwise alignment of two sequences using a linear gap penalty.  The various algorithms in derived classes (NeedlemanWunsch, 
        /// SmithWaterman, and PairwiseOverlap) all use this general engine for alignment with a linear gap penalty.
        /// </summary>
        /// <param name="localSimilarityMatrix">Scoring matrix.</param>
        /// <param name="gapPenalty">Gap penalty (by convention, use a negative number for this.).</param>
        /// <param name="inputA">First input sequence.</param>
        /// <param name="inputB">Second input sequence.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(SimilarityMatrix localSimilarityMatrix, int gapPenalty, ISequence inputA, ISequence inputB)
        {
            this.SimilarityMatrix = localSimilarityMatrix;
            this.GapOpenCost = gapPenalty;
            return DoAlign(inputA, inputB,false);
        }

        /// <summary>
        /// Pairwise alignment of two sequences using an affine gap penalty.  The various algorithms in derived classes (NeedlemanWunsch, 
        /// SmithWaterman, and PairwiseOverlap) all use this general engine for alignment with an affine gap penalty.
        /// </summary>
        /// <param name="localSimilarityMatrix">Scoring matrix.</param>
        /// <param name="gapOpenPenalty">Gap open penalty (by convention, use a negative number for this.).</param>
        /// <param name="gapExtensionPenalty">Gap extension penalty (by convention, use a negative number for this.).</param>
        /// <param name="inputA">First input sequence.</param>
        /// <param name="inputB">Second input sequence.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(
            SimilarityMatrix localSimilarityMatrix,
            int gapOpenPenalty,
            int gapExtensionPenalty,
            ISequence inputA,
            ISequence inputB)
        {
            this.SimilarityMatrix = localSimilarityMatrix;
            this.GapOpenCost = gapOpenPenalty;
            this.GapExtensionCost = gapExtensionPenalty;
            return DoAlign(inputA, inputB, true);
        }

        /// <summary>
        /// Method which performs the alignment work.
        /// </summary>
        /// <param name="sequence1">First sequence</param>
        /// <param name="sequence2">Second sequence</param>
        /// <param name="useAffineGapModel">True to use affine gap model (separate open vs. extension cost)</param>
        /// <returns></returns>
        private IList<IPairwiseSequenceAlignment> DoAlign(ISequence sequence1, ISequence sequence2, bool useAffineGapModel)
        {
            usingAffineGapModel = useAffineGapModel;
            if (sequence1 == null)
                throw new ArgumentNullException("sequence1");
            if (sequence2 == null)
                throw new ArgumentNullException("sequence2");

            if (!Alphabets.CheckIsFromSameBase(sequence1.Alphabet, sequence2.Alphabet))
            {
                Trace.Report(Properties.Resource.InputAlphabetsMismatch);
                throw new ArgumentException(Properties.Resource.InputAlphabetsMismatch);
            }

            if (SimilarityMatrix == null)
            {
                Trace.Report(Properties.Resource.SimilarityMatrixCannotBeNull);
                throw new ArgumentException(Properties.Resource.SimilarityMatrixCannotBeNull);
            }

            if (!SimilarityMatrix.ValidateSequence(sequence1))
            {
                Trace.Report(Properties.Resource.FirstInputSequenceMismatchSimilarityMatrix);
                throw new ArgumentException(Properties.Resource.FirstInputSequenceMismatchSimilarityMatrix);
            }

            if (!SimilarityMatrix.ValidateSequence(sequence2))
            {
                Trace.Report(Properties.Resource.SecondInputSequenceMismatchSimilarityMatrix);
                throw new ArgumentException(Properties.Resource.SecondInputSequenceMismatchSimilarityMatrix);
            }           


            if (GapOpenCost > GapExtensionCost)
            {
                Trace.Report(Properties.Resource.GapOpenGreaterThanGapExtension);               
                throw new ArgumentException(Properties.Resource.GapOpenGreaterThanGapExtension);
            }

            _sequence1 = sequence1;
            _sequence2 = sequence2;
            _gap = Alphabets.CheckIsFromSameBase(Alphabets.Protein, sequence1.Alphabet) ? Alphabets.Protein.Gap : Alphabets.DNA.Gap;

            ReferenceSequence = GetByteArrayFromSequence(_sequence1);
            QuerySequence = GetByteArrayFromSequence(_sequence2);

            // Assign consensus resolver if it was not assigned already.
            IAlphabet alphabet = sequence1.Alphabet;
            if (ConsensusResolver == null)
                ConsensusResolver = new SimpleConsensusResolver(alphabet.HasAmbiguity ? alphabet : Alphabets.AmbiguousAlphabetMap[sequence1.Alphabet]);
            else
                ConsensusResolver.SequenceAlphabet = alphabet.HasAmbiguity ? alphabet : Alphabets.AmbiguousAlphabetMap[sequence1.Alphabet];

            return new List<IPairwiseSequenceAlignment> { Process() };
        }

        /// <summary>
        /// Retrieve or copy the sequence array
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static byte[] GetByteArrayFromSequence(ISequence sequence)
        {
            var realSequence = sequence as Sequence;
            return realSequence != null 
                // Very fast grab internal array
                ? realSequence.GetInternalArray()
                // Much slower copy
                : sequence.ToArray();
        }

        /// <summary>
        /// This method performs the pairwise alignment between two sequences (reference and query).
        /// It does this using the standard Dynamic Programming model:
        /// 1. Initialization of the scoring matrix (Rs.Length x Qs.Length)
        /// 2. Filling of the scoring matrix and traceback table
        /// 3. Traceback (alignment)
        /// </summary>
        /// <returns>Aligned sequences</returns>
        private PairwiseSequenceAlignment Process()
        {
            // Step 1: Initialize
            Initialize();

            // Step 2: Matrix fill (scoring)
            var scores = CreateTracebackTable();

            // Step 3: Traceback (alignment)
            return CreateAlignment(scores);
        }

        /// <summary>
        /// This is step (1) in the dynamic programming model - to initialize the default values
        /// for the scoring matrix and traceback tables.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected void Initialize()
        {
            // Check the bounds
            if (QuerySequence.Length == Int32.MaxValue)
                throw new ArgumentException("Reference sequence contains too many residues (cannot exceed " + Int32.MaxValue + " values).");
            if (ReferenceSequence.Length == Int32.MaxValue)
                throw new ArgumentException("Query sequence contains too many residues (cannot exceed " + Int32.MaxValue + " values).");

            // Track rows/cols
            Rows = QuerySequence.Length + 1;
            Cols = ReferenceSequence.Length + 1;

            // Attempt to keep the scoring table if requested. For performance/memory we use a single
            // array here, but it limits the size dramatically so see if we can actually hold it.
            if (IncludeScoreTable)
            {
                long maxIndex = (long) Rows*Cols;
                if (maxIndex > Int32.MaxValue)
                {
                    IncludeScoreTable = false;
                }
                else
                {
                    try
                    {
                        ScoreTable = new int[maxIndex];
                    }
                    catch (OutOfMemoryException)
                    {
                        ScoreTable = null;
                        IncludeScoreTable = false;
                    }
                }
            }

            // Allocate the first pass of the traceback
            Traceback = new sbyte[Rows][];
            Traceback[0] = new sbyte[Cols]; // Initialized to STOP
        }

        /// <summary>
        /// This is step (2) in the dynamic programming model - to fill in the scoring matrix
        /// and calculate the traceback entries.  This is algorithm specific and so is left
        /// as an abstract method.
        /// </summary>
        protected abstract IEnumerable<OptScoreMatrixCell> CreateTracebackTable();

        /// <summary>
        /// This is step (3) in the dynamic programming model - to walk the traceback/scoring
        /// matrix and generate the alignment.
        /// </summary>
        private PairwiseSequenceAlignment CreateAlignment(IEnumerable<OptScoreMatrixCell> startingCells)
        {
            // Generate each alignment.
            var alignment = new PairwiseSequenceAlignment(_sequence1, _sequence2);
            foreach (var startingCell in startingCells)
                alignment.PairwiseAlignedSequences.Add(CreateAlignmentFromCell(startingCell));

            // Include the scoring table if requested.
            if (IncludeScoreTable)
                alignment.Metadata["ScoreTable"] = GetScoreTable();

            return alignment;
        }

        /// <summary>
        /// This takes a specific starting location in the scoring matrix and generates
        /// an alignment from it using the traceback scores.
        /// </summary>
        /// <param name="startingCell">Starting point</param>
        /// <returns>Pairwise alignment</returns>
        protected PairwiseAlignedSequence CreateAlignmentFromCell(OptScoreMatrixCell startingCell)
        {
            int gapStride = Cols + 1;
            //Using list to avoid allocation issues
            int estimatedLength = (int)( 1.1*Math.Max(ReferenceSequence.Length,QuerySequence.Length));
            var firstAlignment = new List<byte>(estimatedLength);
            var secondAlignment = new List<byte>(estimatedLength);

            // Get the starting cell position and record the optimal score found there.
            int i = startingCell.Row;
            int j = startingCell.Col;
            var finalScore = startingCell.Score;

            long rowGaps = 0, colGaps = 0, identicalCount = 0, similarityCount = 0;

            // Walk the traceback matrix and build the alignments.
            while (!TracebackIsComplete(i, j))
            {
                sbyte tracebackDirection = Traceback[i][j];
                // Walk backwards through the trace back
                int gapLength;
                switch (tracebackDirection)
                {
                    case SourceDirection.Diagonal:
                        byte n1 = ReferenceSequence[j - 1];
                        byte n2 = QuerySequence[i - 1];
                        firstAlignment.Add(n1);
                        secondAlignment.Add(n2);
                        i--;
                        j--;
                        // Track some useful statistics
                        if (n1 == n2 && n1 != _gap)
                        {
                            identicalCount++;
                            similarityCount++;
                        }
                        else if (SimilarityMatrix[n2, n1] > 0)
                            similarityCount++;
                        break;
                    case SourceDirection.Left:
                        //Add 1 because this only counts number of extensions
                        if (usingAffineGapModel)
                        {
                            gapLength = h_Gap_Length[i * gapStride + j];
                            for (int k = 0; k < gapLength; k++)
                            {
                                firstAlignment.Add(ReferenceSequence[--j]);
                                secondAlignment.Add(_gap);
                                rowGaps++;
                            }
                        }
                        else
                        {
                            firstAlignment.Add(ReferenceSequence[--j]);
                            secondAlignment.Add(_gap);
                            rowGaps++;
                        }
                        break;
                    case SourceDirection.Up:
                        //add 1 because this only counts number of extensions.
                        if (usingAffineGapModel)
                        {
                            gapLength = v_Gap_Length[i * gapStride + j];
                            for (int k = 0; k < gapLength; k++)
                            {
                                firstAlignment.Add(_gap);
                                colGaps++;
                                secondAlignment.Add(QuerySequence[--i]);
                            }
                        }
                        else
                        {
                            secondAlignment.Add(QuerySequence[--i]);
                            firstAlignment.Add(_gap);
                            colGaps++;
                        }
                        break;
                    default:
                        break;
                }
            }

            // We build the alignments in reverse since we were
            // walking backwards through the matrix table. To create
            // the proper alignments we need to resize and reverse
            // both underlying arrays.
            firstAlignment.Reverse();
            secondAlignment.Reverse();
            // Create the Consensus sequence
            byte[] consensus = new byte[Math.Min(firstAlignment.Count, secondAlignment.Count)];
            for (int n = 0; n < consensus.Length; n++)
            {
                consensus[n] = ConsensusResolver.GetConsensus(new[] { firstAlignment[n], secondAlignment[n] });
            }

            // Create the result alignment
            var pairwiseAlignedSequence = new PairwiseAlignedSequence
            {
                Score = finalScore,
                FirstSequence = new Sequence(_sequence1.Alphabet, firstAlignment.ToArray()) { ID = _sequence1.ID },
                SecondSequence = new Sequence(_sequence2.Alphabet, secondAlignment.ToArray()) { ID = _sequence2.ID },
                Consensus = new Sequence(ConsensusResolver.SequenceAlphabet, consensus),
            };

            // Offset is start of alignment in input sequence with respect to other sequence.
            if (i >= j)
            {
                pairwiseAlignedSequence.FirstOffset = i - j;
                pairwiseAlignedSequence.SecondOffset = 0;
            }
            else
            {
                pairwiseAlignedSequence.FirstOffset = 0;
                pairwiseAlignedSequence.SecondOffset = j - i;
            }


            // Add in ISequenceAlignment metadata
            pairwiseAlignedSequence.Metadata["Score"] = pairwiseAlignedSequence.Score;
            pairwiseAlignedSequence.Metadata["FirstOffset"] = pairwiseAlignedSequence.FirstOffset;
            pairwiseAlignedSequence.Metadata["SecondOffset"] = pairwiseAlignedSequence.SecondOffset;
            pairwiseAlignedSequence.Metadata["Consensus"] = pairwiseAlignedSequence.Consensus;
            pairwiseAlignedSequence.Metadata["StartOffsets"] = new List<long> { j, i };
            pairwiseAlignedSequence.Metadata["EndOffsets"] = new List<long> { startingCell.Col - 1, startingCell.Row - 1 };
            pairwiseAlignedSequence.Metadata["Insertions"] = new List<long> { colGaps, rowGaps }; // ref, query insertions
            pairwiseAlignedSequence.Metadata["IdenticalCount"] = identicalCount;
            pairwiseAlignedSequence.Metadata["SimilarityCount"] = similarityCount;

            return pairwiseAlignedSequence;

        }

        /// <summary>
        /// This method is used to determine when the traceback step is complete.
        /// It is algorithm specific.
        /// </summary>
        /// <param name="row">Current row</param>
        /// <param name="col">Current column</param>
        /// <returns>True if we are finished with the traceback step, false if not.</returns>
        protected virtual bool TracebackIsComplete(int row, int col)
        {
            return Traceback[row][col] == SourceDirection.Stop;
        }
        
        /// <summary>
        /// This method generates a textual representation of the scoring/traceback matrix
        /// for diagnostic purposes.
        /// </summary>
        /// <returns>String</returns>
        private string GetScoreTable()
        {
            if (ScoreTable == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < QuerySequence.Length + 2; i++)
            {
                for (int j = 0; j < ReferenceSequence.Length + 2; j++)
                {
                    if (i == 0)
                    {
                        if (j == 0 || j == 1)
                        {
                            sb.Append(' ');
                        }
                        else
                        {
                            sb.Append(j == 2 ? "           " : "    ");
                            sb.Append((char)ReferenceSequence[j - 2]);
                        }
                    }
                    else if (j == 0)
                    {
                        if (i == 1)
                        {
                            sb.Append("    ");
                        }
                        else
                        {
                            sb.Append("   " + (char)QuerySequence[i - 2]);
                        }
                    }
                    else
                    {
                        char ch;
                        switch (Traceback[i-1][j-1])
                        {
                            case SourceDirection.Diagonal:
                                ch = '\\';
                                break;
                            case SourceDirection.Left:
                                ch = '<';
                                break;
                            case SourceDirection.Up:
                                ch = '^';
                                break;
                            case SourceDirection.Stop:
                                ch = '*';
                                break;
                            default:
                                ch = ' ';
                                break;
                        }
                        sb.AppendFormat(" {0}{1,3}", ch, ScoreTable[(i-1) * Cols + (j-1)]);
                    }

                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Optimum score maxtrix cell
        /// </summary>
        protected sealed class OptScoreMatrixCell
        {
            /// <summary>
            /// Position (y) of this cell
            /// </summary>
            internal int Row;

            /// <summary>
            /// Position (x) of this cell
            /// </summary>
            internal int Col;

            /// <summary>
            /// Score at this position
            /// </summary>
            internal int Score;
        }
    }
}