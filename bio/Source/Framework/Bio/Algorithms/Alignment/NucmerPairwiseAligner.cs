using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bio.Algorithms.MUMmer;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Class which uses nucmer to perform an alignment.
    /// </summary>
    public class NucmerPairwiseAligner : ISequenceAligner
    {

        /// <summary>
        /// Default minimum length of Matches to be searched in streaming process
        /// </summary>
        private const int DefaultLengthOfMUM = 20;

        /// <summary>
        /// Default gap open penalty for use in alignment algorithms
        /// </summary>
        private const int DefaultGapOpenCost = -13;

        /// <summary>
        /// Default gap extension penalty for use in alignment algorithms
        /// </summary>
        private const int DefaultGapExtensionCost = -8;

        /// <summary>
        /// 
        /// </summary>
        public NucmerPairwiseAligner()
        {
            // Set the default Similarity Matrix
            SimilarityMatrix = new SimilarityMatrix(
                SimilarityMatrix.StandardSimilarityMatrix.DiagonalScoreMatrix);

            // Set the defaults
            GapOpenCost = DefaultGapOpenCost;
            GapExtensionCost = DefaultGapExtensionCost;
            LengthOfMUM = DefaultLengthOfMUM;

            // Set the ClusterBuilder properties to defaults
            FixedSeparation = ClusterBuilder.DefaultFixedSeparation;
            MaximumSeparation = ClusterBuilder.DefaultMaximumSeparation;
            MinimumScore = ClusterBuilder.DefaultMinimumScore;
            SeparationFactor = ClusterBuilder.DefaultSeparationFactor;
            BreakLength = ModifiedSmithWaterman.DefaultBreakLength;
        }

        // Nucmer algorithm.
        private NUCmer nucmerAlgo;

        /// <summary>
        /// Gets or Sets the name of the algorithm.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.NUCMER; }
        }

        /// <summary>
        /// Gets or Sets the description of the algorithm.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.NUCMERDESC; }
        }

        /// <summary>
        /// Gets or sets maximum fixed diagonal difference.
        /// </summary>
        public int FixedSeparation { get; set; }

        /// <summary>
        /// Gets or sets number of bases to be extended before stopping alignment.
        /// </summary>
        public int BreakLength { get; set; }

        /// <summary>
        /// Gets or sets minimum output score.
        /// </summary>
        public int MinimumScore { get; set; }

        /// <summary>
        /// Gets or sets separation factor. Fraction equal to 
        /// (diagonal difference / match separation) where higher values
        /// increase the insertion or deletion (indel) tolerance.
        /// </summary>
        public float SeparationFactor { get; set; }

        /// <summary>
        /// Gets or sets minimum length of Match that can be considered as MUM.
        /// </summary>
        public long LengthOfMUM { get; set; }

        /// <summary>
        /// Gets or sets maximum separation between the adjacent matches in clusters.
        /// </summary>
        public int MaximumSeparation { get; set; }

        /// <summary>
        /// Gets or sets the consensus resolver attached with this aligner.
        /// </summary>
        public IConsensusResolver ConsensusResolver { get; set; }

        /// <summary>
        /// Gets or sets the similarity matrix associated with this aligner.
        /// </summary>
        public SimilarityMatrix SimilarityMatrix { get; set; }

        /// <summary>
        /// Gets or sets the Gap Open Cost.
        /// </summary>
        public int GapOpenCost { get; set; }

        /// <summary>
        /// Gets or sets the Gap Extension Cost.
        /// </summary>
        public int GapExtensionCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to run Align or AlignSimple.
        /// </summary>
        protected bool IsAlign { get; set; }

        #region public methods
        /// <summary>
        /// AlignSimple aligns the set of input sequences using the linear gap model (one gap penalty), 
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        /// <returns>List of sequence alignments.</returns>
        public IList<ISequenceAlignment> AlignSimple(IEnumerable<ISequence> inputSequences)
        {
            IsAlign = false;

            if (inputSequences.Count() < 2)
            {
                throw new ArgumentException(Properties.Resource.MinimumTwoSequences);
            }

            return Alignment(new List<ISequence> { GetReferenceSequence(inputSequences, 0) }, GetQuerySequences(inputSequences, 0)).ToList().ConvertAll(SA => SA as ISequenceAlignment);
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="inputSequences">The sequences to align.</param>
        public IList<ISequenceAlignment> Align(IEnumerable<ISequence> inputSequences)
        {
            IsAlign = true;

            if (inputSequences.Count() < 2)
            {
                throw new ArgumentException(Properties.Resource.MinimumTwoSequences);
            }

            return Alignment(new List<ISequence> { GetReferenceSequence(inputSequences, 0) }, GetQuerySequences(inputSequences, 0)).ToList().ConvertAll(SA => SA as ISequenceAlignment);
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="referenceList">Reference sequences.</param>
        /// <param name="queryList">Query sequences.</param>
        public IList<ISequenceAlignment> Align(IEnumerable<ISequence> referenceList, IEnumerable<ISequence> queryList)
        {
            IsAlign = true;

            if (referenceList.Count() + queryList.Count() < 2)
            {
                throw new ArgumentException(Properties.Resource.MinimumTwoSequences);
            }

            return Alignment(referenceList, queryList).ToList().ConvertAll(SA => SA as ISequenceAlignment);
        }

        /// <summary>
        /// Align aligns the set of input sequences using the affine gap model (gap open and gap extension penalties)
        /// and returns the best alignment found.
        /// </summary>
        /// <param name="reference">Reference sequence.</param>
        /// <param name="querySequences">Query sequences.</param>
        public IList<ISequenceAlignment> Align(ISequence reference, IEnumerable<ISequence> querySequences)
        {
            IsAlign = true;

            if (querySequences.Count() < 1)
            {
                throw new ArgumentException(Properties.Resource.MinimumTwoSequences);
            }

            return Alignment(new List<ISequence> { reference }, querySequences).ToList().ConvertAll(SA => SA as ISequenceAlignment);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Concat all the sequences into one sequence with special character.
        /// </summary>
        /// <param name="sequences">List of reference sequence.</param>
        /// <returns>Concatenated sequence.</returns>
        protected static ISequence ConcatSequence(IEnumerable<ISequence> sequences)
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            // Add the Concatenating symbol to every sequence in reference list
            // Note that, as of now protein sequence is being created out 
            // of input sequence add  concatenation character "X" to it to 
            // simplify implementation.
            List<byte> referenceSequence = null;
            foreach (ISequence sequence in sequences)
            {
                if (null == referenceSequence)
                {
                    referenceSequence = new List<byte>(sequence);
                }
                else
                {
                    referenceSequence.AddRange(sequence);
                }

                // Add concatenating symbol.
                referenceSequence.Add((byte)'+');
            }

            return new Sequence(sequences.First().Alphabet.GetMummerAlphabet(), referenceSequence.ToArray(), false);
        }

        /// <summary>
        /// Analyze the given sequences and store a consensus into its Consensus property.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequence">Query sequence.</param>
        /// <returns>Consensus of sequences.</returns>
        protected ISequence MakeConsensus(
                ISequence referenceSequence,
                ISequence querySequence)
        {
            if (referenceSequence == null)
            {
                throw new ArgumentNullException("referenceSequence");
            }

            if (querySequence == null)
            {
                throw new ArgumentNullException("querySequence");
            }

            // For each pair of symbols (characters) in reference and query sequence
            // get the consensus symbol and append it.
            byte[] consensus = new byte[referenceSequence.Count];
            for (int index = 0; index < referenceSequence.Count; index++)
            {
                consensus[index] = ConsensusResolver.GetConsensus(
                    new byte[] { referenceSequence[index], querySequence[index] });
            }

            IAlphabet alphabet = Alphabets.AutoDetectAlphabet(consensus, 0, consensus.LongLength, referenceSequence.Alphabet);
            return new Sequence(alphabet, consensus, false);
        }

        /// <summary>
        /// Calculate the score of alignment.
        /// </summary>
        /// <param name="referenceSequence">Reference sequence.</param>
        /// <param name="querySequence">Query sequence.</param>
        /// <returns>Score of the alignment.</returns>
        protected int CalculateScore(
                ISequence referenceSequence,
                ISequence querySequence)
        {
            if (referenceSequence == null)
            {
                throw new ArgumentNullException("referenceSequence");
            }

            if (querySequence == null)
            {
                throw new ArgumentNullException("querySequence");
            }

            int index;

            int score = 0;

            // For each pair of symbols (characters) in reference and query sequence
            // 1. If the character are different and not alignment character "-", 
            //      then find the cost form Similarity Matrix
            // 2. If Gap Extension cost needs to be used
            //      a. Find how many gaps exists in appropriate sequence (reference / query)
            //          and calculate the score
            // 3. Add the gap open cost
            for (index = 0; index < referenceSequence.Count; index++)
            {
                byte referenceCharacter = referenceSequence[index];
                byte queryCharacter = querySequence[index];

                if (DnaAlphabet.Instance.Gap != referenceCharacter
                    && DnaAlphabet.Instance.Gap != queryCharacter)
                {
                    score += SimilarityMatrix[referenceCharacter, queryCharacter];
                }
                else
                {
                    if (IsAlign)
                    {
                        int gapCount;
                        if (DnaAlphabet.Instance.Gap == referenceCharacter)
                        {
                            gapCount = FindExtensionLength(referenceSequence, index);
                        }
                        else
                        {
                            gapCount = FindExtensionLength(querySequence, index);
                        }

                        score += GapOpenCost + (gapCount * GapExtensionCost);

                        // move the index pointer to end of extension
                        index = index + gapCount - 1;
                    }
                    else
                    {
                        score += GapOpenCost;
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Find the index of extension.
        /// </summary>
        /// <param name="sequence">Sequence object.</param>
        /// <param name="index">Position at which extension starts.</param>
        /// <returns>Last index of extension.</returns>
        private static int FindExtensionLength(ISequence sequence, int index)
        {
            // Find the number of alignment characters "-" in the given sequence 
            // from position index
            int gapCounter = index;

            while (gapCounter < sequence.Count
                    && DnaAlphabet.Instance.Gap == sequence[gapCounter])
            {
                gapCounter++;
            }

            return gapCounter - index;
        }

        /// <summary>
        /// This method is considered as main execute method which defines the
        /// step by step algorithm. Derived class flows the defined flow by this
        /// method.
        /// </summary>
        /// <param name="referenceSequenceList">Reference sequence.</param>
        /// <param name="querySequenceList">List of input sequences.</param>
        /// <returns>A list of sequence alignment.</returns>
        private IEnumerable<IPairwiseSequenceAlignment> Alignment(IEnumerable<ISequence> referenceSequenceList, IEnumerable<ISequence> querySequenceList)
        {
            ConsensusResolver = new SimpleConsensusResolver(referenceSequenceList.ElementAt(0).Alphabet);

            IList<IPairwiseSequenceAlignment> results = new List<IPairwiseSequenceAlignment>();
            IPairwiseSequenceAlignment sequenceAlignment;
            IList<PairwiseAlignedSequence> alignments;

            List<DeltaAlignment> deltas = new List<DeltaAlignment>();

            foreach (ISequence refSequence in referenceSequenceList)
            {
                this.nucmerAlgo = new NUCmer((Sequence)refSequence);

                if (GapOpenCost != DefaultGapOpenCost) this.nucmerAlgo.GapOpenCost = GapOpenCost;
                if (GapExtensionCost != DefaultGapExtensionCost) this.nucmerAlgo.GapExtensionCost = GapExtensionCost;
                if (LengthOfMUM != DefaultLengthOfMUM) this.nucmerAlgo.LengthOfMUM = LengthOfMUM;

                // Set the ClusterBuilder properties to defaults
                if (FixedSeparation != ClusterBuilder.DefaultFixedSeparation) this.nucmerAlgo.FixedSeparation = FixedSeparation;
                if (MaximumSeparation != ClusterBuilder.DefaultMaximumSeparation) this.nucmerAlgo.MaximumSeparation = MaximumSeparation;
                if (MinimumScore != ClusterBuilder.DefaultMinimumScore) this.nucmerAlgo.MinimumScore = MinimumScore;
                if (SeparationFactor != ClusterBuilder.DefaultSeparationFactor) this.nucmerAlgo.SeparationFactor = SeparationFactor;
                if (BreakLength != ModifiedSmithWaterman.DefaultBreakLength) this.nucmerAlgo.BreakLength = BreakLength;

                this.nucmerAlgo.ConsensusResolver = ConsensusResolver;
                if (SimilarityMatrix != null) this.nucmerAlgo.SimilarityMatrix = SimilarityMatrix;

                foreach (ISequence querySequence in querySequenceList)
                {
                    IEnumerable<DeltaAlignment> deltaAlignment = this.nucmerAlgo.GetDeltaAlignments(querySequence);
                    deltas.AddRange(deltaAlignment);
                }
            }

            if (deltas.Count > 0)
            {
                ISequence concatReference = referenceSequenceList.ElementAt(0);
                //// concat all the sequences into one sequence
                if (referenceSequenceList.Count() > 1)
                {
                    concatReference = ConcatSequence(referenceSequenceList);
                }

                foreach (ISequence querySequence in querySequenceList)
                {
                    List<DeltaAlignment> qDelta = deltas.Where(d => d.QuerySequence.Equals(querySequence)).ToList();
                    sequenceAlignment = new PairwiseSequenceAlignment(concatReference, querySequence);

                    // Convert delta alignments to sequence alignments
                    alignments = ConvertDeltaToAlignment(qDelta);

                    if (alignments.Count > 0)
                    {
                        foreach (PairwiseAlignedSequence align in alignments)
                        {
                            // Calculate the score of alignment
                            align.Score = CalculateScore(
                                    align.FirstSequence,
                                    align.SecondSequence);

                            // Make Consensus
                            align.Consensus = MakeConsensus(
                                    align.FirstSequence,
                                    align.SecondSequence);

                            sequenceAlignment.PairwiseAlignedSequences.Add(align);
                        }
                    }

                    results.Add(sequenceAlignment);
                }
            }

            return results;
        }

        /// <summary>
        /// Convert to delta alignments to sequence alignments.
        /// </summary>
        /// <param name="alignments">List of delta alignments.</param>
        /// <returns>List of Sequence alignment.</returns>
        private static IList<PairwiseAlignedSequence> ConvertDeltaToAlignment(
                IEnumerable<DeltaAlignment> alignments)
        {
            if (alignments == null)
            {
                throw new ArgumentNullException("alignments");
            }

            PairwiseAlignedSequence alignedSequence;

            IList<PairwiseAlignedSequence> alignedSequences = new List<PairwiseAlignedSequence>();
            foreach (DeltaAlignment deltaAlignment in alignments)
            {
                alignedSequence = deltaAlignment.ConvertDeltaToSequences();

                // Find the offsets
                long referenceStart = deltaAlignment.FirstSequenceStart;
                long queryStart = deltaAlignment.SecondSequenceStart;
                long difference = referenceStart - queryStart;
                if (0 < difference)
                {
                    alignedSequence.FirstOffset = 0;
                    alignedSequence.SecondOffset = difference;
                }
                else
                {
                    alignedSequence.FirstOffset = -1 * difference;
                    alignedSequence.SecondOffset = 0;
                }

                alignedSequences.Add(alignedSequence);
            }

            return alignedSequences;
        }

        /// <summary>
        /// Gets the sequence at specified index.
        /// </summary>
        /// <param name="sequences">IEnumerable of Sequences.</param>
        /// <param name="referenceIndex">Reference sequence index.</param>
        private static ISequence GetReferenceSequence(IEnumerable<ISequence> sequences, long referenceIndex)
        {
            long index = 0;
            foreach (ISequence sequence in sequences)
            {
                if (index == referenceIndex)
                {
                    return sequence;
                }

                index++;
            }

            return null;
        }

        /// <summary>
        /// Gets the sequences except the sequence at specified index.
        /// </summary>
        /// <param name="sequences">IEnumerable of Sequences.</param>
        /// <param name="referenceIndex">Reference sequence index.</param>
        private static IEnumerable<ISequence> GetQuerySequences(IEnumerable<ISequence> sequences, long referenceIndex)
        {
            long index = 0;
            foreach (ISequence sequence in sequences)
            {
                if (index != referenceIndex)
                {
                    yield return sequence;
                }

                index++;
            }
        }

        #endregion
    }
}


