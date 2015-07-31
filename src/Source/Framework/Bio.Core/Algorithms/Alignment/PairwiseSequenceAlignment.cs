using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// A simple implementation of IPairwiseSequenceAlignment that stores the 
    /// results as list of Aligned Sequences.
    /// </summary>
    public class PairwiseSequenceAlignment : IPairwiseSequenceAlignment
    {
        /// <summary>
        /// Sequence alignment instance.
        /// </summary>
        private readonly SequenceAlignment seqAlignment;

        /// <summary>
        /// List of alignments.
        /// </summary>
        private readonly List<PairwiseAlignedSequence> alignedSequences = new List<PairwiseAlignedSequence>();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the PairwiseSequenceAlignment class
        /// Constructs PairwiseSequenceAlignment with input sequences.
        /// </summary>
        /// <param name="firstSequence">First input sequence.</param>
        /// <param name="secondSequence">Second input sequence.</param>
        public PairwiseSequenceAlignment(ISequence firstSequence, ISequence secondSequence)
        {
            seqAlignment = new SequenceAlignment(new [] { firstSequence, secondSequence });
            alignedSequences = new List<PairwiseAlignedSequence>();
            IsReadOnly = false;  // initializes to false by default, but make it explicit for good style.
        }

        /// <summary>
        /// Initializes a new instance of the PairwiseSequenceAlignment class
        /// Constructs an empty PairwiseSequenceAlignment.
        /// </summary>
        public PairwiseSequenceAlignment()
        {
            seqAlignment = new SequenceAlignment();
            IsReadOnly = false;  // initializes to false by default, but make it explicit for good style.
        }
        
        #endregion

        #region ISequenceAlignment members
        /// <summary>
        /// Gets any additional information about the Alignment.
        /// </summary>
        public IDictionary<string, object> Metadata
        {
            get
            {
                return seqAlignment.Metadata;
            }
        }

        /// <summary>
        /// Gets list of the (output) aligned sequences.
        /// </summary>
        public IList<IAlignedSequence> AlignedSequences
        {
            get
            {
                // get all IPairwiseAlignedSequence as IAlignedSequence.
                return alignedSequences.Cast<IAlignedSequence>().ToList();
            }
        }

        /// <summary>
        /// Gets list of sequences involved in this alignment.
        /// </summary>
        public IList<ISequence> Sequences
        {
            get
            {
                return seqAlignment.Sequences;
            }
        }

        #endregion

        #region IPairwiseSequenceAlignment members

        /// <summary>
        /// Gets the list of alignments.
        /// </summary>
        public IList<PairwiseAlignedSequence> PairwiseAlignedSequences
        {
            get { return alignedSequences; }
        }

        /// <summary>
        /// Gets accessor for the first sequence.
        /// </summary>
        public ISequence FirstSequence
        {
            get
            {
                if (seqAlignment.Sequences.Count == 0)
                {
                    return null;
                }

                return seqAlignment.Sequences[0];
            }
        }

        /// <summary>
        /// Gets accessor for the second sequence.
        /// </summary>
        public ISequence SecondSequence
        {
            get
            {
                if (seqAlignment.Sequences.Count <= 1)
                {
                    return null;
                }

                return seqAlignment.Sequences[1];
            }
        }

        /// <summary>
        /// Gets or sets Documentation object is intended for tracking the history, provenance,
        /// and experimental context of a PairwiseSequenceAlignment. The user can adopt any desired
        /// convention for use of this object.
        /// </summary>
        public object Documentation { get; set; }

        /// <summary>
        /// Gets number of aligned sequence objects in the PairwiseSequenceAlignment.
        /// </summary>
        public int Count
        {
            get
            {
                return alignedSequences.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether PairwiseSequenceAlignment is read-only or not.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Returns the nth aligned sequence in the alignment.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The aligned sequence.</returns>
        public PairwiseAlignedSequence this[int i]
        {
            get
            {
                return alignedSequences[i];
            }
        }

        #endregion

        /// <summary>
        /// Add a new Aligned Sequence Object to the end of the list.
        /// </summary>
        /// <param name="pairwiseAlignedSequence">The sequence to add.</param>
        public void AddSequence(PairwiseAlignedSequence pairwiseAlignedSequence)
        {
            if (IsReadOnly)
                throw new NotSupportedException(Properties.Resource.READ_ONLY_COLLECTION_MESSAGE);

            alignedSequences.Add(pairwiseAlignedSequence);
        }

        #region ICollection<ISequence> Members
        
        /// <summary>
        /// Adds an aligned sequence to the list of aligned sequences in the PairwiseSequenceAlignment.
        /// Throws exception if sequence alignment is read only.
        /// </summary>
        /// <param name="item">PairwiseAlignedSequence to add.</param>
        public void Add(PairwiseAlignedSequence item)
        {
            if (IsReadOnly)
                throw new NotSupportedException(Properties.Resource.READ_ONLY_COLLECTION_MESSAGE);

            alignedSequences.Add(item);
        }

        /// <summary>
        /// Clears the PairwiseSequenceAlignment
        /// Throws exception if PairwiseSequenceAlignment is read only.
        /// </summary>
        public void Clear()
        {
            if (IsReadOnly)
                throw new NotSupportedException(Properties.Resource.READ_ONLY_COLLECTION_MESSAGE);

            alignedSequences.Clear();
        }

        /// <summary>
        /// Returns true if the PairwiseSequenceAlignment contains the aligned sequence in the
        /// list of aligned sequences.
        /// </summary>
        /// <param name="item">PairwiseAlignedSequence object.</param>
        /// <returns>True if contains item, otherwise returns false.</returns>
        public bool Contains(PairwiseAlignedSequence item)
        {
            return alignedSequences.Contains(item);
        }

        /// <summary>
        /// Copies the aligned sequences from the PairwiseSequenceAlignment into an existing aligned sequence array.
        /// </summary>
        /// <param name="array">Array into which to copy the sequences.</param>
        /// <param name="arrayIndex">Starting index in array at which to begin the copy.</param>
        public void CopyTo(PairwiseAlignedSequence[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameArray);
            }

            foreach (PairwiseAlignedSequence seq in alignedSequences)
            {
                array[arrayIndex++] = seq;
            }
        }

        /// <summary>
        /// Removes item from the list of aligned sequences in the PairwiseSequenceAlignment.
        /// Throws exception if PairwiseSequenceAlignment is read only.
        /// </summary>
        /// <param name="item">Aligned sequence object.</param>
        /// <returns>True if item was removed, false if item was not found.</returns>
        public bool Remove(PairwiseAlignedSequence item)
        {
            if (IsReadOnly)
                throw new NotSupportedException(Properties.Resource.READ_ONLY_COLLECTION_MESSAGE);

            return alignedSequences.Remove(item);
        }

        /// <summary>
        /// Converts the Aligned Sequences to string.
        /// </summary>
        /// <returns>Aligned Sequence Data.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (IAlignedSequence seq in AlignedSequences)
            {
                builder.AppendLine(seq.ToString());
            }

            return builder.ToString();
        }

        #endregion

        #region IEnumerable<ISequence> Members

        /// <summary>
        /// Returns an enumerator for the aligned sequences in the PairwiseSequenceAlignment.
        /// </summary>
        /// <returns>Returns the enumerator for PairwiseAlignedSequence.</returns>
        public IEnumerator<PairwiseAlignedSequence> GetEnumerator()
        {
            return alignedSequences.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator for the aligned sequences in the PairwiseSequenceAlignment.
        /// </summary>
        /// <returns>Returns the enumerator for PairwiseAlignedSequence.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return alignedSequences.GetEnumerator();
        }

        #endregion
    }
}
