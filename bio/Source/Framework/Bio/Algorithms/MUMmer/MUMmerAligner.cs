using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.MUMmer.LIS;
using Bio.Algorithms.SuffixTree;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;

namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// This class is used for Align MUMs.
    /// </summary>
    public class MUMmerAligner : ISequenceAligner
    {
        #region -- Member Variables --

        /// <summary>
        /// Alignment Char.
        /// </summary>
        private const byte AlignmentChar = 45;

        /// <summary>
        /// Boolean indicating whether the MUMs generated 
        /// during alignment are to be stored for later access.
        /// </summary>
        private bool storeMUMs = false;

        /// <summary>
        /// Stores list of MUMs.
        /// </summary>
        private IDictionary<ISequence, IEnumerable<Match>> mums;

        #endregion -- Member Variables --

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MUMmerAligner class.
        /// Constructor for all the pairwise aligner 
        /// (NeedlemanWunsch, SmithWaterman, Overlap).
        /// Sets default similarity matrix and gap penalty.
        /// Users will typically reset these using parameters 
        /// specific to their particular sequences
        /// and needs.
        /// </summary>
        public MUMmerAligner()
        {
            // Set default similarity matrix and gap penalty.
            // User will typically choose their own parameters, these
            // defaults are reasonable for many cases.

            // Default is set to 20
            this.LengthOfMUM = 20;

            SimilarityMatrix = null;

            this.GapOpenCost = -13; // 5, -4 diagonal matrix for Dna

            // default affine gap is -1
            this.GapExtensionCost = -8;

            // Set the default alignment algorithm to NeedlemanWunsch
            this.PairWiseAlgorithm = new NeedlemanWunschAligner();

            this.MaximumMatchEnabled = false;

            this.AmbigiousMatchesAllowed = false;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Gets or sets the length of MUM.
        /// </summary>
        public long LengthOfMUM { get; set; }

        /// <summary>
        /// Gets or sets similarity matrix for use in alignment algorithms.
        /// </summary>
        public SimilarityMatrix SimilarityMatrix { get; set; }

        /// <summary> 
        /// Gets or sets gap open penalty for use in alignment algorithms. 
        /// For alignments using a linear gap penalty, this is the gap penalty.
        /// For alignments using an affine gap, this is the penalty to open a new gap.
        /// This is a negative number, for example GapOpenCost = -8, not +8.
        /// </summary>
        public int GapOpenCost { get; set; }

        /// <summary> 
        /// Gets or sets gap extension penalty for use in alignment algorithms. 
        /// Not used for alignments using a linear gap penalty.
        /// For alignments using an affine gap, this is the penalty to
        /// extend an existing gap.
        /// This is a negative number, for example GapExtensionCost = -2, not +2.
        /// </summary>
        public int GapExtensionCost { get; set; }

        /// <summary>
        /// Gets or sets the object that will be used to compute the alignment's consensus.
        /// </summary>
        public IConsensusResolver ConsensusResolver { get; set; }

        /// <summary>
        /// Gets or sets the pair wise aligner which will be executed 
        /// by end of Mummer.
        /// </summary>
        public IPairwiseSequenceAligner PairWiseAlgorithm { get; set; }

        /// <summary>
        /// Gets the name of the Aligner. Intended to be filled in 
        /// by classes deriving from DynamicProgrammingPairwiseAligner class
        /// with the exact name of the Alignment algorithm.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.MUMmerAlignerName; }
        }

        /// <summary>
        /// Gets the description of the Aligner. Intended to be filled in 
        /// by classes deriving from DynamicProgrammingPairwiseAligner class
        /// with the exact details of the Alignment algorithm.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.MUMmerAlignerDescription; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Boolean value indicating 
        /// whether MUMs generated are to be stored or not.
        /// Set to false by default.
        /// Note: Storing MUMs incur memory overhead.
        /// </summary>
        public bool StoreMUMs
        {
            get { return this.storeMUMs; }
            set { this.storeMUMs = value; }
        }

        /// <summary>
        /// Gets the list of MUMs after applying Longest Increasing Subsequence
        /// algorithm to order and merge MUMs, for each query sequence.
        /// </summary>
        public IDictionary<ISequence, IEnumerable<Match>> MUMs
        {
            get { return this.mums; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ambiguous matches are allowed or not.
        /// </summary>
        public bool AmbigiousMatchesAllowed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether MaxMatch option should be enabled or not.
        /// If this property is set to true, then mums are generated irrespective of 
        /// uniqueness in query and reference sequences.
        /// By default this property is set to false, indicating that matches are unique in reference sequence only.
        /// </summary>
        public bool MaximumMatchEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to run Align or AlignSimple.
        /// </summary>
        private bool UseGapExtensionCost { get; set; }

        #endregion -- Properties --

        #region -- Public Method(s) --

        /// <summary>
        /// Align the list of input sequences using linear gap model.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>A list of sequence alignments.</returns>
        IList<ISequenceAlignment> ISequenceAligner.AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            return this.AlignSimple(inputSequences).ToList().ConvertAll(SA => SA as ISequenceAlignment);
        }

        /// <summary>
        /// Align the list of input sequences using linear gap model.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            this.UseGapExtensionCost = false;
            return this.AlignSimple(inputSequences.First(), inputSequences);
        }

        /// <summary>
        /// Align the reference sequence and query sequences using linear gap model.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequenceList">List of query sequence.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> AlignSimple(
               ISequence referenceSequence,
               IEnumerable<ISequence> querySequenceList)
        {
            this.UseGapExtensionCost = false;
            return this.Alignment(referenceSequence, querySequenceList);
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model 
        /// (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>A list of sequence alignments.</returns>
        IList<ISequenceAlignment> ISequenceAligner.Align(IEnumerable<ISequence> inputSequences)
        {
            return this.Align(inputSequences).ToList().ConvertAll(SA => SA as ISequenceAlignment);
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model 
        /// (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(IEnumerable<ISequence> inputSequences)
        {
            this.UseGapExtensionCost = true;
            return this.Align(inputSequences.First(), inputSequences);
        }

        /// <summary>
        ///  Align aligns the reference sequence with query sequences using the affine gap model 
        ///  (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequenceList">List of query sequence.</param>
        /// <returns>A list of sequence alignments.</returns>
        public IList<IPairwiseSequenceAlignment> Align(
                ISequence referenceSequence,
                IEnumerable<ISequence> querySequenceList)
        {
            this.UseGapExtensionCost = true;
            return this.Alignment(referenceSequence, querySequenceList);
        }

        #endregion -- Public Method(s) --

        /// <summary>
        /// Validate the inputs.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequenceList">List of input sequences.</param>
        /// <returns>Are inputs valid.</returns>
        private bool Validate(
                ISequence referenceSequence,
                IEnumerable<ISequence> querySequenceList)
        {
            bool isValidLength = false;

            if (null == referenceSequence)
            {
                string message = Properties.Resource.ReferenceSequenceCannotBeNull;
                Trace.Report(message);
                throw new ArgumentNullException("referenceSequence");
            }

            if (null == querySequenceList)
            {
                string message = Properties.Resource.QueryListCannotBeNull;
                Trace.Report(message);
                throw new ArgumentNullException("querySequenceList");
            }

            if ((referenceSequence.Alphabet != Alphabets.DNA) && (referenceSequence.Alphabet != Alphabets.RNA))
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resource.OnlyDNAOrRNAInput,
                    "MUMmer");
                Trace.Report(message);
                throw new ArgumentException(message, "referenceSequence");
            }

            // setting default similarity matrix based on DNA or RNA
            if (SimilarityMatrix == null)
            {
                if (referenceSequence.Alphabet == Alphabets.RNA)
                {
                    SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna);
                }
                else if (referenceSequence.Alphabet == Alphabets.DNA)
                {
                    SimilarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna);
                }
            }

            if (!SimilarityMatrix.ValidateSequence(referenceSequence))
            {
                string message = Properties.Resource.FirstInputSequenceMismatchSimilarityMatrix;
                Trace.Report(message);
                throw new ArgumentException(message, "referenceSequence");
            }

            if (referenceSequence.Count < this.LengthOfMUM)
            {
                string message = String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.InputSequenceMustBeGreaterThanMUM,
                        this.LengthOfMUM);
                Trace.Report(message);
                throw new ArgumentException(message, "referenceSequence");
            }

            foreach (ISequence querySequence in querySequenceList)
            {
                if (null == querySequence)
                {
                    string message = Properties.Resource.QuerySequenceCannotBeNull;
                    Trace.Report(message);
                    throw new ArgumentNullException("querySequenceList", message);
                }

                if (referenceSequence.Alphabet != querySequence.Alphabet)
                {
                    string message = Properties.Resource.InputAlphabetsMismatch;
                    Trace.Report(message);
                    throw new ArgumentException(message);
                }

                if (!SimilarityMatrix.ValidateSequence(querySequence))
                {
                    string message = Properties.Resource.SecondInputSequenceMismatchSimilarityMatrix;
                    Trace.Report(message);
                    throw new ArgumentException(message, "querySequenceList");
                }

                if (querySequence.Count >= this.LengthOfMUM)
                {
                    isValidLength = true;
                }
            }

            if (!isValidLength)
            {
                string message = String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.InputSequenceMustBeGreaterThanMUM,
                        this.LengthOfMUM);
                Trace.Report(message);
                throw new ArgumentException(message, "querySequenceList");
            }

            if (1 > this.LengthOfMUM)
            {
                string message = Properties.Resource.MUMLengthTooSmall;
                Trace.Report(message);
                throw new ArgumentException(message);
            }

            return true;
        }

        #region -- Private Methods --

        /// <summary>
        /// Generates a list of MUMs for computing LIS.
        /// </summary>
        /// <param name="mums">MUMs generated by the MUMmer.</param>
        /// <returns>List of MUMs.</returns>
        private static IList<Match> GetMumsForLIS(IEnumerable<Match> mums)
        {
            IList<Match> querySortedMum2 = new List<Match>(mums);
            return querySortedMum2;
        }

        /// <summary>
        /// Create a default gap sequence of given length, pad the symbol - in sequence.
        /// </summary>
        /// <param name="length">Length of gap.</param>
        /// <returns>Hyphen padded sequence.</returns>
        private static byte[] CreateDefaultGap(long length)
        {
            byte[] gap = new byte[length];
            for (long index = 0; index < length; index++)
            {
                gap[index] = AlignmentChar;
            }

            return gap;
        }

        /// <summary>
        /// This method is considered as main execute method which defines the
        /// step by step algorithm. Derived class flows the defined flow by this
        /// method.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequenceList">List of input sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        private IList<IPairwiseSequenceAlignment> Alignment(
                ISequence referenceSequence,
                IEnumerable<ISequence> querySequenceList)
        {
            // Initializations
            if (this.ConsensusResolver == null)
            {
                this.ConsensusResolver = new SimpleConsensusResolver(referenceSequence.Alphabet);
            }
            else
            {
                this.ConsensusResolver.SequenceAlphabet = referenceSequence.Alphabet;
            }

            if (this.StoreMUMs)
            {
                return this.AlignmentWithAccumulatedMUMs(referenceSequence, querySequenceList);
            }
            else
            {
                return this.AlignmentWithoutAccumulatedMUMs(referenceSequence, querySequenceList);
            }
        }

        /// <summary>
        /// This method is considered as main execute method which defines the
        /// step by step algorithm. Derived class flows the defined flow by this
        /// method. Does not store MUMs, processes MUMs and gaps to find 
        /// alignment directly. 
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequenceList">List of input sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        private IList<IPairwiseSequenceAlignment> AlignmentWithoutAccumulatedMUMs(
                ISequence referenceSequence,
                IEnumerable<ISequence> querySequenceList)
        {
            IList<IPairwiseSequenceAlignment> results = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment alignment = null;
            IEnumerable<Match> mum;
            if (this.Validate(referenceSequence, querySequenceList))
            {
                // Safety check for public methods to ensure that null 
                // inputs are handled.
                if (referenceSequence == null || querySequenceList == null)
                {
                    return null;
                }

                Sequence seq = referenceSequence as Sequence;
                if (seq == null)
                {
                    throw new ArgumentException(Properties.Resource.OnlySequenceClassSupported);
                }

                MUMmer mummer = new MUMmer(seq);
                mummer.LengthOfMUM = this.LengthOfMUM;
                mummer.NoAmbiguity = this.AmbigiousMatchesAllowed;
                foreach (ISequence sequence in querySequenceList)
                {
                    if (sequence.Equals(referenceSequence))
                    {
                        continue;
                    }

                    alignment = new PairwiseSequenceAlignment(referenceSequence, sequence);

                    // Step2 : streaming process is performed with the query sequence
                    if (this.MaximumMatchEnabled)
                    {
                        mum = mummer.GetMatches(sequence);
                    }
                    else
                    {
                        mum = mummer.GetMatchesUniqueInReference(sequence);
                    }

                    // Step3(a) : sorted mum list based on reference sequence
                    LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
                    IList<Match> sortedMumList = lis.SortMum(GetMumsForLIS(mum));

                    if (sortedMumList.Count > 0)
                    {
                        // Step3(b) : LIS using greedy cover algorithm
                        IList<Match> finalMumList = lis.GetLongestSequence(sortedMumList);

                        if (finalMumList.Count > 0)
                        {
                            // Step 4 : get all the gaps in each sequence and call 
                            // pairwise alignment
                            alignment.PairwiseAlignedSequences.Add(
                                this.ProcessGaps(referenceSequence, sequence, finalMumList));
                        }

                        results.Add(alignment);
                    }
                    else
                    {
                        IList<IPairwiseSequenceAlignment> sequenceAlignment = this.RunPairWise(
                                referenceSequence,
                                sequence);

                        foreach (IPairwiseSequenceAlignment pairwiseAlignment in sequenceAlignment)
                        {
                            results.Add(pairwiseAlignment);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// This method is considered as main execute method which defines the
        /// step by step algorithm. Derived class flows the defined flow by this
        /// method. Store generated MUMs in properties MUMs, SortedMUMs.
        /// Alignment first finds MUMs for all the query sequence, and then 
        /// runs pairwise algorithm on gaps to produce alignments.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequenceList">List of input sequences.</param>
        /// <returns>A list of sequence alignments.</returns>
        private IList<IPairwiseSequenceAlignment> AlignmentWithAccumulatedMUMs(
                ISequence referenceSequence,
                IEnumerable<ISequence> querySequenceList)
        {
            // Get MUMs
            this.mums = new Dictionary<ISequence, IEnumerable<Match>>();
            IList<IPairwiseSequenceAlignment> results = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment alignment = null;
            IEnumerable<Match> mum;
            if (this.Validate(referenceSequence, querySequenceList))
            {
                // Safety check for public methods to ensure that null 
                // inputs are handled.
                if (referenceSequence == null || querySequenceList == null)
                {
                    return null;
                }

                Sequence seq = referenceSequence as Sequence;
                if (seq == null)
                {
                    throw new ArgumentException(Properties.Resource.OnlySequenceClassSupported);
                }

                MUMmer mummer = new MUMmer(seq);
                mummer.LengthOfMUM = this.LengthOfMUM;
                mummer.NoAmbiguity = this.AmbigiousMatchesAllowed;
                foreach (ISequence sequence in querySequenceList)
                {
                    if (sequence.Equals(referenceSequence))
                    {
                        continue;
                    }

                    alignment = new PairwiseSequenceAlignment(referenceSequence, sequence);

                    // Step2 : streaming process is performed with the query sequence
                    if (this.MaximumMatchEnabled)
                    {
                        mum = mummer.GetMatches(sequence);
                    }
                    else
                    {
                        mum = mummer.GetMatchesUniqueInReference(sequence);
                    }

                    this.mums.Add(sequence, mum);

                    // Step3(a) : sorted mum list based on reference sequence
                    LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
                    IList<Match> sortedMumList = lis.SortMum(GetMumsForLIS(mum));

                    if (sortedMumList.Count > 0)
                    {
                        // Step3(b) : LIS using greedy cover algorithm
                        IList<Match> finalMumList = lis.GetLongestSequence(sortedMumList);

                        if (finalMumList.Count > 0)
                        {
                            // Step 4 : get all the gaps in each sequence and call 
                            // pairwise alignment
                            alignment.PairwiseAlignedSequences.Add(
                                this.ProcessGaps(referenceSequence, sequence, finalMumList));
                        }

                        results.Add(alignment);
                    }
                    else
                    {
                        IList<IPairwiseSequenceAlignment> sequenceAlignment = this.RunPairWise(
                                referenceSequence,
                                sequence);

                        foreach (IPairwiseSequenceAlignment pairwiseAlignment in sequenceAlignment)
                        {
                            results.Add(pairwiseAlignment);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Get the alignment using pair wise.
        /// </summary>
        /// <param name="seq1">Sequence 1.</param>
        /// <param name="seq2">Sequence 2.</param>
        /// <returns>A list of sequence alignments.</returns>
        private IList<IPairwiseSequenceAlignment> RunPairWise(ISequence seq1, ISequence seq2)
        {
            IList<IPairwiseSequenceAlignment> sequenceAlignment = null;

            if (this.PairWiseAlgorithm == null)
            {
                this.PairWiseAlgorithm = new NeedlemanWunschAligner();
            }

            this.PairWiseAlgorithm.SimilarityMatrix = SimilarityMatrix;
            this.PairWiseAlgorithm.GapOpenCost = this.GapOpenCost;
            this.PairWiseAlgorithm.ConsensusResolver = this.ConsensusResolver;

            if (this.UseGapExtensionCost)
            {
                this.PairWiseAlgorithm.GapExtensionCost = this.GapExtensionCost;
                sequenceAlignment = this.PairWiseAlgorithm.Align(seq1, seq2);
            }
            else
            {
                sequenceAlignment = this.PairWiseAlgorithm.AlignSimple(seq1, seq2);
            }

            // MUMmer does not support other aligners. Throw exception.
            return sequenceAlignment;
        }

        /// <summary>
        /// Get all the gaps in each sequence and call pairwise alignment.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="sequence">Query sequence.</param>
        /// <param name="mums">List of MUMs.</param>
        /// <returns>Aligned sequences.</returns>
        private PairwiseAlignedSequence ProcessGaps(
                ISequence referenceSequence,
                ISequence sequence,
                IList<Match> mums)
        {
            List<byte> sequenceResult1 = new List<byte>();
            List<byte> sequenceResult2 = new List<byte>();
            List<byte> consensusResult = new List<byte>();
            PairwiseAlignedSequence alignedSequence = new PairwiseAlignedSequence();
            Match mum1;
            Match mum2;

            // Run the alignment for gap before first MUM
            List<long> insertions = new List<long>(2);
            insertions.Add(0);
            insertions.Add(0);

            List<long> gapInsertions;
            mum1 = mums.First();
            alignedSequence.Score += this.AlignGap(
                    referenceSequence,
                    sequence,
                    sequenceResult1,
                    sequenceResult2,
                    consensusResult,
                    new Match() { Length = 0 }, // Here the first MUM does not exist
                    mum1,
                    out gapInsertions);

            insertions[0] += gapInsertions[0];
            insertions[1] += gapInsertions[1];

            // Run the alignment for all the gaps between MUM
            for (int index = 1; index < mums.Count; index++)
            {
                mum2 = mums[index];

                alignedSequence.Score += this.AlignGap(
                        referenceSequence,
                        sequence,
                        sequenceResult1,
                        sequenceResult2,
                        consensusResult,
                        mum1,
                        mum2,
                        out gapInsertions);

                insertions[0] += gapInsertions[0];
                insertions[1] += gapInsertions[1];

                mum1 = mum2;
            }

            // Run the alignment for gap after last MUM
            alignedSequence.Score += this.AlignGap(
                    referenceSequence,
                    sequence,
                    sequenceResult1,
                    sequenceResult2,
                    consensusResult,
                    mum1,
                    new Match() { Length = 0 },
                    out gapInsertions);

            insertions[0] += gapInsertions[0];
            insertions[1] += gapInsertions[1];

            byte[] result1 = sequenceResult1.ToArray();
            IAlphabet alphabet = Alphabets.AutoDetectAlphabet(result1, 0, result1.LongLength, referenceSequence.Alphabet);
            alignedSequence.FirstSequence = new Sequence(
                alphabet,
                result1)
                {
                    ID = referenceSequence.ID,
                    // Do not shallow copy dictionary
                    //Metadata = referenceSequence.Metadata
                };

            byte[] result2 = sequenceResult2.ToArray();
            alphabet = Alphabets.AutoDetectAlphabet(result2, 0, result2.LongLength, sequence.Alphabet);

            alignedSequence.SecondSequence = new Sequence(
                alphabet,
                result2)
                {
                    ID = sequence.ID,
                    // Do not shallow copy dictionary
                    //Metadata = sequence.Metadata
                };

            byte[] consensus = consensusResult.ToArray();
            alphabet = Alphabets.AutoDetectAlphabet(consensus, 0, consensus.LongLength, referenceSequence.Alphabet);
            alignedSequence.Consensus = new Sequence(
                alphabet,
                consensus);

            // Offset is not required as Smith Waterman will  fragmented alignment. 
            // Offset is the starting position of alignment of sequence1 with respect to sequence2.
            if (this.PairWiseAlgorithm is NeedlemanWunschAligner)
            {
                alignedSequence.FirstOffset = alignedSequence.FirstSequence.IndexOfNonGap() -
                    referenceSequence.IndexOfNonGap();
                alignedSequence.SecondOffset = alignedSequence.SecondSequence.IndexOfNonGap() -
                    sequence.IndexOfNonGap();
            }

            List<long> startOffsets = new List<long>(2);
            List<long> endOffsets = new List<long>(2);
            startOffsets.Add(0);
            startOffsets.Add(0);

            endOffsets.Add(referenceSequence.Count - 1);
            endOffsets.Add(sequence.Count - 1);

            alignedSequence.Metadata["StartOffsets"] = startOffsets;
            alignedSequence.Metadata["EndOffsets"] = endOffsets;
            alignedSequence.Metadata["Insertions"] = insertions;

            // return the aligned sequence
            return alignedSequence;
        }

        /// <summary>
        /// Align the Gap by executing pairwise alignment.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequence">Query Sequence.</param>
        /// <param name="sequenceResult1">Editable sequence containing alignment first result.</param>
        /// <param name="sequenceResult2">Editable sequence containing alignment second result.</param>
        /// <param name="consensusResult">Editable sequence containing consensus sequence.</param>
        /// <param name="mum1">First MUM of Gap.</param>
        /// <param name="mum2">Second MUM of Gap.</param>
        /// <param name="insertions">Insertions made to the aligned sequences.</param>
        /// <returns>Score of alignment.</returns>
        private long AlignGap(
                ISequence referenceSequence,
                ISequence querySequence,
                List<byte> sequenceResult1,
                List<byte> sequenceResult2,
                List<byte> consensusResult,
                Match mum1,
                Match mum2,
                out List<long> insertions)
        {
            long score = 0;
            ISequence sequence1 = null;
            ISequence sequence2 = null;
            IList<IPairwiseSequenceAlignment> sequenceAlignment = null;
            byte[] mum1String;
            byte[] mum2String;

            insertions = new List<long>(2);
            insertions.Add(0);
            insertions.Add(0);

            long mum1ReferenceStartIndex = 0;
            long mum1QueryStartIndex = 0;
            long mum1Length = 0;
            long mum2ReferenceStartIndex = 0;
            long mum2QueryStartIndex = 0;
            long mum2Length = 0;

            if (mum1.Length != 0)
            {
                mum1ReferenceStartIndex = mum1.ReferenceSequenceOffset;
                mum1QueryStartIndex = mum1.QuerySequenceOffset;
                mum1Length = mum1.Length;
            }

            if (mum2.Length != 0)
            {
                mum2ReferenceStartIndex = mum2.ReferenceSequenceOffset;
                mum2QueryStartIndex = mum2.QuerySequenceOffset;
                mum2Length = mum2.Length;
            }
            else
            {
                mum2ReferenceStartIndex = referenceSequence.Count;
                mum2QueryStartIndex = querySequence.Count;
            }

            long referenceGapStartIndex = mum1ReferenceStartIndex + mum1Length;
            long queryGapStartIndex = mum1QueryStartIndex + mum1Length;

            if (mum2ReferenceStartIndex > referenceGapStartIndex
                && mum2QueryStartIndex > queryGapStartIndex)
            {
                sequence1 = referenceSequence.GetSubSequence(
                    referenceGapStartIndex,
                    mum2ReferenceStartIndex - referenceGapStartIndex);
                sequence2 = querySequence.GetSubSequence(
                    queryGapStartIndex,
                    mum2QueryStartIndex - queryGapStartIndex);

                sequenceAlignment = this.RunPairWise(sequence1, sequence2);

                if (sequenceAlignment != null)
                {
                    foreach (IPairwiseSequenceAlignment pairwiseAlignment in sequenceAlignment)
                    {
                        foreach (PairwiseAlignedSequence alignment in pairwiseAlignment.PairwiseAlignedSequences)
                        {
                            sequenceResult1.InsertRange(
                                    sequenceResult1.Count,
                                    alignment.FirstSequence);
                            sequenceResult2.InsertRange(
                                    sequenceResult2.Count,
                                    alignment.SecondSequence);
                            consensusResult.InsertRange(
                                consensusResult.Count,
                                    alignment.Consensus);

                            score += alignment.Score;

                            if (alignment.Metadata.ContainsKey("Insertions"))
                            {
                                List<int> gapinsertions = alignment.Metadata["Insertions"] as List<int>;
                                if (gapinsertions != null)
                                {
                                    if (gapinsertions.Count > 0)
                                    {
                                        insertions[0] += gapinsertions[0];
                                    }

                                    if (gapinsertions.Count > 1)
                                    {
                                        insertions[1] += gapinsertions[1];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (mum2ReferenceStartIndex > referenceGapStartIndex)
            {
                sequence1 = referenceSequence.GetSubSequence(
                    referenceGapStartIndex,
                    mum2ReferenceStartIndex - referenceGapStartIndex);

                sequenceResult1.InsertRange(sequenceResult1.Count, sequence1);
                sequenceResult2.InsertRange(sequenceResult2.Count, CreateDefaultGap(sequence1.Count));
                consensusResult.InsertRange(consensusResult.Count, sequence1);

                insertions[1] += sequence1.Count;

                if (this.UseGapExtensionCost)
                {
                    score = this.GapOpenCost + ((sequence1.Count - 1) * this.GapExtensionCost);
                }
                else
                {
                    score = sequence1.Count * this.GapOpenCost;
                }
            }
            else if (mum2QueryStartIndex > queryGapStartIndex)
            {
                sequence2 = querySequence.GetSubSequence(
                    queryGapStartIndex,
                    mum2QueryStartIndex - queryGapStartIndex);

                sequenceResult1.InsertRange(sequenceResult1.Count, CreateDefaultGap(sequence2.Count));
                sequenceResult2.InsertRange(sequenceResult2.Count, sequence2);
                consensusResult.InsertRange(consensusResult.Count, sequence2);

                insertions[0] += sequence2.Count;

                if (this.UseGapExtensionCost)
                {
                    score = this.GapOpenCost + ((sequence2.Count - 1) * this.GapExtensionCost);
                }
                else
                {
                    score = sequence2.Count * this.GapOpenCost;
                }
            }

            // Add the MUM to the result
            if (0 < mum2Length)
            {
                mum1String = referenceSequence.GetSubSequence(
                        mum2ReferenceStartIndex,
                        mum2Length).ToArray();
                sequenceResult1.InsertRange(sequenceResult1.Count, mum1String);

                mum2String = querySequence.GetSubSequence(
                        mum2QueryStartIndex,
                        mum2Length).ToArray();
                sequenceResult2.InsertRange(sequenceResult2.Count, mum2String);
                consensusResult.InsertRange(consensusResult.Count, mum1String);

                foreach (byte index in mum1String)
                {
                    score += SimilarityMatrix[index, index];
                }
            }

            return score;
        }

        #endregion -- Private Methods --
    }
}
