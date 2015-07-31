using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Bio.Core.Extensions;
using Bio.Extensions;
using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Base class for dynamic programming alignment algorithms, including NeedlemanWunsch, 
    /// SmithWaterman and PairwiseOverlap.
    /// The basic reference for this code (including NW, SW and Overlap) is Chapter 2 in 
    /// Biological Sequence Analysis; Durbin, Eddy, Krogh and Mitchison; Cambridge Press; 1998
    /// The variable names in these classes follow the notation in Durbin et al.
    /// </summary>
    public abstract class DynamicProgrammingPairwiseAligner : IPairwiseSequenceAligner
    {
        /// <summary> 
        /// Similarity matrix for use in alignment algorithms. 
        /// </summary>
        protected SimilarityMatrix InternalSimilarityMatrix;

        /// <summary>
        /// First input sequence.
        /// </summary>
        protected ISequence FirstInputSequence;

        /// <summary>
        /// Second input sequence.
        /// </summary>
        protected ISequence SecondInputSequence;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DynamicProgrammingPairwiseAligner class.
        /// Constructor for all the pairwise aligner (NeedlemanWunsch, SmithWaterman, Overlap).
        /// Sets default similarity matrix and gap penalties.
        /// Users will typically reset these using parameters specific to their particular sequences and needs.
        /// </summary>
        protected DynamicProgrammingPairwiseAligner()
        {
            // Set default similarity matrix and gap penalty.
            // User will typically choose their own parameters, these defaults are reasonable for many cases.
            // Molecule type is set to protein, since this will also work for DNA and RNA in the
            // special case of a diagonal similarity matrix.
            this.InternalSimilarityMatrix = new DiagonalSimilarityMatrix(2, -2);
            GapOpenCost = -8;
            GapExtensionCost = -1;
        }

        #endregion

        #region Properties

        /// <summary> Gets or sets similarity matrix for use in alignment algorithms. </summary>
        public SimilarityMatrix SimilarityMatrix
        {
            get { return this.InternalSimilarityMatrix; }
            set { this.InternalSimilarityMatrix = value; }
        }

        /// <summary> 
        /// Gets or sets gap open penalty for use in alignment algorithms. 
        /// For alignments using a linear gap penalty, this is the gap penalty.
        /// For alignments using an affine gap, this is the penalty to open a new gap.
        /// This is a negative number, for example GapOpenCost = -8, not +8.
        /// </summary>
        public int GapOpenCost
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets or sets gap extension penalty for use in alignment algorithms. 
        /// Not used for alignments using a linear gap penalty.
        /// For alignments using an affine gap, this is the penalty to extend an existing gap.
        /// This is a negative number, for example GapExtensionCost = -2, not +2.
        /// </summary>
        public int GapExtensionCost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the object that will be used to compute the alignment's consensus.
        /// </summary>
        public IConsensusResolver ConsensusResolver { get; set; }

        /// <summary>
        /// Gets the name of the Aligner. Intended to be filled in 
        /// by classes deriving from DynamicProgrammingPairwiseAligner class
        /// with the exact name of the Alignment algorithm.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of the Aligner. Intended to be filled in 
        /// by classes deriving from DynamicProgrammingPairwiseAligner class
        /// with the exact details of the Alignment algorithm.
        /// </summary>
        public abstract string Description { get; }
        #endregion

        #region Methods

        /// <summary>
        /// Aligns two sequences using a linear gap parameter, using existing gap open cost and similarity matrix.
        /// Set these using GapOpenCost and SimilarityMatrix properties before calling this method.
        /// </summary>
        /// <param name="sequence1">First input sequence.</param>
        /// <param name="sequence2">Second input sequence.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(ISequence sequence1, ISequence sequence2)
        {
            return AlignSimple(this.InternalSimilarityMatrix, GapOpenCost, sequence1, sequence2);
        }

        /// <summary>
        /// Aligns two sequences using a linear gap parameter, using existing gap open cost and similarity matrix.
        /// Set these using GapOpenCost and SimilarityMatrix properties before calling this method.
        /// </summary>
        /// <param name="inputSequences">List of sequences to align.  Must contain exactly two sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            if (inputSequences.Count() != 2)
            {
                string message = String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.PairwiseAlignerWrongArgumentCount,
                        inputSequences.Count());
                Debug.WriteLine(message);
                throw new ArgumentException(message, "inputSequences");
            }

            ISequence firstSequence = null;
            ISequence secondSequence = null;

            firstSequence = inputSequences.ElementAt(0);
            secondSequence = inputSequences.ElementAt(1);

            return AlignSimple(this.InternalSimilarityMatrix, GapOpenCost, firstSequence, secondSequence);
        }

        /// <summary>
        /// Aligns two sequences using a linear gap parameter, using existing gap open cost and similarity matrix.
        /// Set these using GapOpenCost and SimilarityMatrix properties before calling this method.
        /// </summary>
        /// <param name="inputSequences">List of sequences to align.  Must contain exactly two sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        IList<ISequenceAlignment> ISequenceAligner.AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            return this.AlignSimple(inputSequences).Cast<ISequenceAlignment>().ToList();
        }

        /// <summary>
        /// Aligns two sequences using the affine gap metric, a gap open penalty and a gap extension penalty.
        /// This method uses the existing gap open and extension penalties and similarity matrix.
        /// Set these using GapOpenCost, GapExtensionCost and SimilarityMatrix properties before calling this method.
        /// </summary>
        /// <param name="sequence1">First input sequence.</param>
        /// <param name="sequence2">Second input sequence.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(ISequence sequence1, ISequence sequence2)
        {
            return Align(this.InternalSimilarityMatrix, GapOpenCost, GapExtensionCost, sequence1, sequence2);
        }

        /// <summary>
        /// Aligns two sequences using the affine gap metric, a gap open penalty and a gap extension penalty.
        /// This method uses the existing gap open and extension penalties and similarity matrix.
        /// Set these using GapOpenCost, GapExtensionCost and SimilarityMatrix properties before calling this method.
        /// </summary>
        /// <param name="inputSequences">List of sequences to align.  Must contain exactly two sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(IEnumerable<ISequence> inputSequences)
        {
            if (inputSequences.Count() != 2)
            {
                string message = String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.PairwiseAlignerWrongArgumentCount,
                        inputSequences.Count());
                Debug.WriteLine(message);
                throw new ArgumentException(message, "inputSequences");
            }

            ISequence firstSequence = null;
            ISequence secondSequence = null;

            firstSequence = inputSequences.ElementAt(0);
            secondSequence = inputSequences.ElementAt(1);

            return Align(this.InternalSimilarityMatrix, GapOpenCost, GapExtensionCost, firstSequence, secondSequence);
        }

        /// <summary>
        /// Aligns two sequences using the affine gap metric, a gap open penalty and a gap extension penalty.
        /// This method uses the existing gap open and extension penalties and similarity matrix.
        /// Set these using GapOpenCost, GapExtensionCost and SimilarityMatrix properties before calling this method.
        /// </summary>
        /// <param name="inputSequences">List of sequences to align.  Must contain exactly two sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        IList<ISequenceAlignment> ISequenceAligner.Align(IEnumerable<ISequence> inputSequences)
        {
            return this.Align(inputSequences).ToList().ConvertAll(SA => SA as ISequenceAlignment);
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
             // Initialize and perform validations for simple alignment
            SimpleAlignPrimer(localSimilarityMatrix, gapPenalty, inputA, inputB);

            DynamicProgrammingPairwiseAlignerJob alignerJob = this.CreateSimpleAlignmentJob(inputA, inputB);
            IList<IPairwiseSequenceAlignment> result = alignerJob.Align();

            foreach (IPairwiseSequenceAlignment alignment in result)
            {
                foreach (PairwiseAlignedSequence sequence in alignment.AlignedSequences)
                {
                    AddSimpleConsensusToResult(sequence);
                }
            }

            return result;
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
            // Initialize and perform validations for alignment
            // In addition, initialize gap extension penalty.
            SimpleAlignPrimer(localSimilarityMatrix, gapOpenPenalty, inputA, inputB);
            GapExtensionCost = gapExtensionPenalty;

            DynamicProgrammingPairwiseAlignerJob alignerJob = this.CreateAffineAlignmentJob(inputA, inputB);
            IList<IPairwiseSequenceAlignment> result = alignerJob.Align();

            foreach (IPairwiseSequenceAlignment alignment in result)
            {
                foreach (PairwiseAlignedSequence sequence in alignment.AlignedSequences)
                {
                    AddSimpleConsensusToResult(sequence);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates the Simple aligner job
        /// </summary>
        /// <param name="sequenceA">First aligned sequence</param>
        /// <param name="sequenceB">Second aligned sequence</param>
        /// <returns></returns>
        protected abstract DynamicProgrammingPairwiseAlignerJob CreateSimpleAlignmentJob(ISequence sequenceA, ISequence sequenceB);

        /// <summary>
        /// Creates the Affine aligner job
        /// </summary>
        /// <param name="sequenceA">First aligned sequence</param>
        /// <param name="sequenceB">Second aligned sequence</param>
        /// <returns></returns>
        protected abstract DynamicProgrammingPairwiseAlignerJob CreateAffineAlignmentJob(ISequence sequenceA, ISequence sequenceB);
        /// <summary>
        /// Validates input sequences and gap penalties.
        /// Checks that input sequences use the same alphabet.
        /// Checks that each symbol in the input sequences exists in the similarity matrix.
        /// Checks that gap penalties are less than or equal to 0.
        /// Throws exception if sequences fail these checks.
        /// Writes warning to ApplicationLog if gap penalty or penalties are positive.
        /// </summary>
        /// <param name="inputA">First input sequence.</param>
        /// <param name="inputB">Second input sequence.</param>
        protected void ValidateAlignInput(ISequence inputA, ISequence inputB)
        {
            if (inputA == null)
            {
                throw new ArgumentNullException("inputA");
            }

            if (inputB == null)
            {
                throw new ArgumentNullException("inputB");
            }

            if (!Alphabets.CheckIsFromSameBase(inputA.Alphabet, inputB.Alphabet))
                throw new ArgumentException(Properties.Resource.InputAlphabetsMismatch);

            if (null == this.InternalSimilarityMatrix)
                throw new ArgumentException(Properties.Resource.SimilarityMatrixCannotBeNull);

            if (!this.InternalSimilarityMatrix.ValidateSequence(inputA))
                throw new ArgumentException(Properties.Resource.FirstInputSequenceMismatchSimilarityMatrix);

            if (!this.InternalSimilarityMatrix.ValidateSequence(inputB))
                throw new ArgumentException(Properties.Resource.SecondInputSequenceMismatchSimilarityMatrix);
        }
            /// <summary>
        /// Initializations to be done before aligning sequences.
        /// Sets consensus resolver property to correct alphabet.
        /// </summary>
        /// <param name="inputSequence">Input sequence.</param>
        private void InitializeAlign(ISequence inputSequence)
        {
            // Initializations
            if (ConsensusResolver == null)
            {
                ConsensusResolver = new SimpleConsensusResolver(Alphabets.AmbiguousAlphabetMap[inputSequence.Alphabet]);
            }
            else
            {
                ConsensusResolver.SequenceAlphabet = Alphabets.AmbiguousAlphabetMap[inputSequence.Alphabet];
            }
        }

        /// <summary>
        /// Performs initializations and validations required 
        /// before carrying out sequence alignment.
        /// Initializes only gap open penalty. Initialization for
        /// gap extension, if required, has to be done separately. 
        /// </summary>
        /// <param name="similarityMatrix">Scoring matrix.</param>
        /// <param name="gapPenalty">Gap open penalty (by convention, use a negative number for this.).</param>
        /// <param name="inputA">First input sequence.</param>
        /// <param name="inputB">Second input sequence.</param>
        private void SimpleAlignPrimer(SimilarityMatrix similarityMatrix, int gapPenalty, ISequence inputA, ISequence inputB)
        {
            InitializeAlign(inputA);

            // Set Gap Penalty and Similarity Matrix
            GapOpenCost = gapPenalty;

            // note that _gapExtensionCost is not used for linear gap penalty
            this.InternalSimilarityMatrix = similarityMatrix;

            ValidateAlignInput(inputA, inputB);  // throws exception if input not valid

            // Convert input strings to 0-based int arrays using similarity matrix mapping
            this.FirstInputSequence = inputA;
            this.SecondInputSequence = inputB;
        }

        /// <summary>
        /// Adds consensus to the alignment result.  At this point, it is a very simple algorithm
        /// which puts an ambiguity character where the two aligned sequences do not match.
        /// Uses X and N for protein and DNA/RNA alignments, respectively.
        /// </summary>
        /// <param name="alignment">
        /// Alignment to which to add the consensus.  This is the result returned by the main Align
        /// or AlignSimple method, which contains the aligned sequences but not yet a consensus sequence.
        /// </param>
        private void AddSimpleConsensusToResult(PairwiseAlignedSequence alignment)
        {
            ISequence seq0 = alignment.FirstSequence;
            ISequence seq1 = alignment.SecondSequence;

            byte[] consensus = new byte[seq0.Count];
            for (int i = 0; i < seq0.Count; i++)
            {
                consensus[i] = ConsensusResolver.GetConsensus(
                        new byte[] { seq0[i], seq1[i] });
            }

            IAlphabet consensusAlphabet = Alphabets.AutoDetectAlphabet(consensus, 0, consensus.GetLongLength(), seq0.Alphabet);
            alignment.Consensus = new Sequence(consensusAlphabet, consensus, false);
        }

        #region Nested Enums, Structs and Classes
        /// <summary>
        /// Position details of cell with best score.
        /// </summary>
        protected struct OptScoreCell
        {
            /// <summary>
            /// Column number of cell with optimal score.
            /// </summary>
            public long Column;

            /// <summary>
            /// Row number of cell with optimal score.
            /// </summary>
            public long Row;

            /// <summary>
            /// Cell number of cell with optimal score.
            /// </summary>
            public long Cell;

            /// <summary>
            /// Initializes a new instance of the OptScoreCell struct.
            /// Creates best score cell with the input position values.
            /// </summary>
            /// <param name="row">Row Number.</param>
            /// <param name="column">Column Number.</param>
            public OptScoreCell(long row, long column)
            {
                Row = row;
                Column = column;
                Cell = 0;
            }

            /// <summary>
            /// Initializes a new instance of the OptScoreCell struct.
            /// Creates best score cell with the input position values.
            /// </summary>
            /// <param name="row">Row Number.</param>
            /// <param name="column">Column Number.</param>
            /// <param name="cell">Cell Number.</param>
            public OptScoreCell(long row, long column, long cell)
                : this(row, column)
            {
                Cell = cell;
            }

            /// <summary>
            /// Overrides == Operator.
            /// </summary>
            /// <param name="cell1">First cell.</param>
            /// <param name="cell2">Second cell.</param>
            /// <returns>Result of comparison.</returns>
            public static bool operator ==(OptScoreCell cell1, OptScoreCell cell2)
            {
                return
                    cell1.Row == cell2.Row &&
                    cell1.Column == cell2.Column &&
                    cell1.Cell == cell2.Cell;
            }

            /// <summary>
            /// Overrides != Operator.
            /// </summary>
            /// <param name="cell1">First cell.</param>
            /// <param name="cell2">Second cell.</param>
            /// <returns>Result of comparison.</returns>
            public static bool operator !=(OptScoreCell cell1, OptScoreCell cell2)
            {
                return !(cell1 == cell2);
            }

            /// <summary>
            /// Override Equals method.
            /// </summary>
            /// <param name="obj">Object for comparison.</param>
            /// <returns>Result of comparison.</returns>
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                OptScoreCell other = (OptScoreCell)obj;
                return this == other;
            }

            /// <summary>
            /// Returns the Hash code.
            /// </summary>
            /// <returns>Hash code of OptScoreCell.</returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        #endregion

        #endregion
    }
}
