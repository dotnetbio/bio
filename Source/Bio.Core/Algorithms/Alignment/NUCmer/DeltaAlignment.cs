using System;
using System.Collections.Generic;
using System.Globalization;
using Bio.Algorithms.SuffixTree;
using Bio.Core.Extensions;
using Bio.Properties;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Represents a alignment object in terms of delta.
    /// Delta is an encoded representation of alignments between input sequences.
    /// It contains the start and end indices of alignment in reference and
    /// query sequence followed by error values and list of integer in 
    /// following lines. Each integer represent an insertion (+ve) in reference
    /// sequence and deletion (-ve) in reference sequence.
    /// This class represents such alignment with required properties and
    /// utility methods.
    /// </summary>
    public class DeltaAlignment
    {
        /// <summary>
        /// List of integers that pointing the insertion and deletion indices
        /// </summary>
        private readonly IList<long> internalDeltas;

        /// <summary>
        /// The query direction
        /// </summary>
        private string queryDirection;

        ///<summary>
        /// Initializes a new instance of the DeltaAlignment class
        /// </summary>
        /// <param name="referenceSequence">Reference Sequence</param>
        /// <param name="querySequence">Query Sequence</param>
        public DeltaAlignment(ISequence referenceSequence, ISequence querySequence)
        {
            this.internalDeltas = new List<long>();
            ReferenceSequence = referenceSequence;
            QuerySequence = querySequence;
            QueryDirection = Cluster.ForwardDirection;
        }

        /// <summary>
        /// Id for this Delta alignment.
        /// Note: As file position is used as ID in case of comparative assembly steps, this value may vary from step to step.
        ///       For example: if 0 is the id for a delta alignment in 2nd step then the same delta alignment
        ///                    can have 500 as id in 3rd step.
        ///       This change in id may happen due to the sorting of delta or ignoring some other delta alignments etc.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the query sequence direction FORWARD or REVERSE
        /// When this is reverse, the second offsets will be reversed in output
        /// </summary>
        public string QueryDirection
        {
            get { return this.queryDirection; }
            set
            {
                if (value != Cluster.ForwardDirection && value != Cluster.ReverseDirection)
                    throw new ArgumentOutOfRangeException("value", Resource.InvalidQueryDirection);
                this.queryDirection = value;
            }
        }

        /// <summary>
        /// Returns TRUE if this is a REVERSE query sequence direction
        /// </summary>
        public bool IsReverseQueryDirection
        {
            get { return QueryDirection == Cluster.ReverseDirection; }
        }

        /// <summary>
        /// Gets or sets the start index of first sequence
        /// </summary>
        public long FirstSequenceStart { get; set; }

        /// <summary>
        /// Gets or sets the end index of first sequence
        /// </summary>
        public long FirstSequenceEnd { get; set; }

        /// <summary>
        /// Gets or sets the start index of second sequence
        /// </summary>
        public long SecondSequenceStart { get; set; }

        /// <summary>
        /// Gets or sets the end index of second sequence
        /// </summary>
        public long SecondSequenceEnd { get; set; }

        /// <summary>
        /// Gets or sets errors
        /// </summary>
        public int Errors { get; set; }

        /// <summary>
        /// Gets or sets similarity errors
        /// </summary>
        public int SimilarityErrors { get; set; }

        /// <summary>
        /// Gets or sets number of non alphabets encountered during alignment
        /// </summary>
        public int NonAlphas { get; set; }

        /// <summary>
        /// Gets or sets the value of delta reference position
        /// </summary>
        public int DeltaReferencePosition { get; set; }

        /// <summary>
        /// Gets list of integers that pointing the insertion and 
        /// deletion indices
        /// </summary>
        public IList<long> Deltas
        {
            get { return this.internalDeltas; }
        }

        /// <summary>
        /// Gets reference sequence
        /// </summary>
        public ISequence ReferenceSequence { get; set; }

        /// <summary>
        /// Gets query sequence id
        /// </summary>
        public ISequence QuerySequence { get; set; }

        /// <summary>
        /// Create a new delta alignment
        /// </summary>
        /// <param name="referenceSequence">Reference sequence</param>
        /// <param name="querySequence">Query sequence</param>
        /// <param name="cluster">Cluster object</param>
        /// <param name="match">Match object</param>
        /// <returns>Newly created DeltaAlignment object</returns>
        internal static DeltaAlignment NewAlignment(
                ISequence referenceSequence,
                ISequence querySequence,
                Cluster cluster,
                MatchExtension match)
        {
            DeltaAlignment deltaAlignment = new DeltaAlignment(referenceSequence, querySequence)
            {
                FirstSequenceStart = match.ReferenceSequenceOffset,
                SecondSequenceStart = match.QuerySequenceOffset,
                FirstSequenceEnd = match.ReferenceSequenceOffset + match.Length - 1,
                SecondSequenceEnd = match.QuerySequenceOffset + match.Length - 1,
                QueryDirection = cluster.QueryDirection
            };

            return deltaAlignment;
        }

        /// <summary>
        /// Convert the delta alignment object to its sequence representation
        /// </summary>
        /// <returns>Reference sequence alignment at 0th index and
        /// Query sequence alignment at 1st index</returns>
        public PairwiseAlignedSequence ConvertDeltaToSequences()
        {
            PairwiseAlignedSequence alignedSequence = new PairwiseAlignedSequence();
            int gap = 0;
            List<long> startOffsets = new List<long>(2);
            List<long> endOffsets = new List<long>(2);
            List<long> insertions = new List<long>(2);

            startOffsets.Add(FirstSequenceStart);
            startOffsets.Add(SecondSequenceStart);
            endOffsets.Add(FirstSequenceEnd);
            endOffsets.Add(SecondSequenceEnd);

            insertions.Add(0);
            insertions.Add(0);

            // Create the new sequence object with given start and end indices
            List<byte> referenceSequence = new List<byte>();
            for (long index = this.FirstSequenceStart; index <= this.FirstSequenceEnd; index++)
            {
                referenceSequence.Add(this.ReferenceSequence[index]);
            }

            List<byte> querySequence = new List<byte>();
            for (long index = this.SecondSequenceStart; index <= this.SecondSequenceEnd; index++)
            {
                querySequence.Add(this.QuerySequence[index]);
            }
            // Insert the Alignment character at delta position
            // +ve delta: Insertion in reference sequence
            // -ve delta: Insertion in query sequence (deletion in reference sequence)
            foreach (int delta in Deltas)
            {
                gap += Math.Abs(delta);
                if (delta < 0)
                {
                    referenceSequence.Insert(gap - 1, DnaAlphabet.Instance.Gap);
                    insertions[0]++;
                }
                else
                {
                    querySequence.Insert(gap - 1, DnaAlphabet.Instance.Gap);
                    insertions[1]++;
                }
            }

            byte[] refSeq = referenceSequence.ToArray();
            IAlphabet alphabet = Alphabets.AutoDetectAlphabet(refSeq, 0, refSeq.GetLongLength(), null);
            alignedSequence.FirstSequence = new Sequence(alphabet, refSeq, false)
            {
                ID = ReferenceSequence.ID,
                Metadata = new Dictionary<string, object>(ReferenceSequence.Metadata)
            };

            byte[] querySeq = querySequence.ToArray();
            alphabet = Alphabets.AutoDetectAlphabet(querySeq, 0, querySeq.GetLongLength(), QuerySequence.Alphabet);
            alignedSequence.SecondSequence = new Sequence(alphabet, querySeq, false)
            {
                ID = QuerySequence.ID,
                Metadata = new Dictionary<string, object>(QuerySequence.Metadata)
            };

            alignedSequence.Metadata["StartOffsets"] = startOffsets;
            alignedSequence.Metadata["EndOffsets"] = endOffsets;
            alignedSequence.Metadata["Insertions"] = insertions;

            return alignedSequence;
        }

        /// <summary>
        /// Converts Ref ID, Query Id, Ref start, Ref End, Query start, Query End to string.
        /// </summary>
        /// <returns>Ref ID, Query Id, Ref start, Ref End, Query start, Query End.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Properties.Resource.DeltaAlignmentToStringFormat,
                          this.ReferenceSequence.ID, this.QuerySequence.ID, this.FirstSequenceStart, this.FirstSequenceEnd,
                          this.SecondSequenceStart, this.SecondSequenceEnd, this.QueryDirection);
        }
    }
}
