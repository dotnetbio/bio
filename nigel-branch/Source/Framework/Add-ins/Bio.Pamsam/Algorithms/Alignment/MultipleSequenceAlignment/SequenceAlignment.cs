using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Bio.Util.Logging;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// A simple implementation of ISequenceAlignment that stores the 
    /// results as new sequences
    /// </summary>
    [Serializable]
    public class SequenceAlignment : ISequenceAlignment
    {
        #region Fields

        private ISequence consensus;
        private List<ISequence> sequences = new List<ISequence>();
        private int score;

        #endregion

        /// <summary>
        /// Constructs an empty SequenceAlignment
        /// </summary>
        public SequenceAlignment()
        {
            IsReadOnly = false;  // initializes to false by default, but make it explicit for good style
        }

        #region ISequenceAlignment members

        /// <summary>
        /// A list of the (usually modified) output sequences, in the same order
        /// that the inputs were passed to the alignment algorithm.
        /// </summary>
        public ICollection<ISequence> Sequences
        {
            set
            {
                sequences = (List<ISequence>)value;
            }
            get
            {
                return sequences;
            }
        }

        /// <summary>
        /// A consensus sequence representing the alignment.
        /// </summary>
        public ISequence Consensus
        {
            set
            {
                consensus = value;
            }
            get
            {
                return consensus;
            }
        }

        /// <summary>
        /// Returns the ith sequence in the alignment.
        /// </summary>
        /// <param name="iSequence">The index.</param>
        /// <returns>The sequence.</returns>
        public ISequence this[int iSequence]
        {
            get
            {
                return sequences[iSequence];
            }
        }

        /// <summary>
        /// Add a new sequence to the end of the sequence collection.
        /// </summary>
        /// <param name="sequence">The sequence to add.</param>
        public void AddSequence(ISequence sequence)
        {
            if (!IsReadOnly)
            {
                sequences.Add(sequence);
            }
        }

        /// <summary>
        /// The score for the alignment. Higher scores mean better alignments.
        /// The score is determined by the alignment algorithm used.
        /// </summary>
        public int Score
        {
            set
            {
                score = value;
            }
            get
            {
                return score;
            }
        }

        /// <summary>
        /// Offset is the starting position of alignment of sequence1
        /// with respect to sequence2.
        /// </summary>
        public IList<int> Offsets { get; set; }

        /// <summary>
        /// The Documentation object is intended for tracking the history, provenance,
        /// and experimental context of a sequence. The user can adopt any desired
        /// convention for use of this object.
        /// </summary>
        public Object Documentation { get; set; }

        #endregion

        #region ICollection<ISequence> Members

        /// <summary>
        /// Adds an ISequence to the list of sequences in the SequenceAlignment.
        /// Throws exception if SequenceAlignment is read only.
        /// </summary>
        /// <param name="item">ISequence to add.</param>
        public void Add(ISequence item)
        {
            if (IsReadOnly)
            {
                Trace.Report(Resource.READ_ONLY_COLLECTION_MESSAGE);
                throw new NotSupportedException(Resource.READ_ONLY_COLLECTION_MESSAGE);
            }

            sequences.Add(item);
        }

        /// <summary>
        /// Clears the SequenceAlignment
        /// Throws exception if SequenceAlignment is read only.
        /// </summary>
        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(Resource.READ_ONLY_COLLECTION_MESSAGE);
            }

            sequences.Clear();
            consensus = null;
            score = 0;
        }

        /// <summary>
        /// Returns true if the SequenceAlignment contains the sequence in the
        /// list of sequences. (The consensus sequence is not checked.) Does not do a deep comparison,
        /// the input sequence must be exactly the same sequence as the one in the
        /// SequenceAlignment.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if contains item, otherwise returns false.</returns>
        public bool Contains(ISequence item)
        {
            return sequences.Contains<ISequence>(item);
        }

        /// <summary>
        /// Copies the sequences from the SequenceAlignment into an existing ISequence array.
        /// </summary>
        /// <param name="array">Array into which to copy the sequences.</param>
        /// <param name="arrayIndex">Starting index in array at which to begin the copy.</param>
        public void CopyTo(ISequence[] array, int arrayIndex)
        {
            foreach (ISequence seq in sequences)
            {
                array[arrayIndex++] = seq;
            }
        }

        /// <summary>
        /// Number of sequences in the SequenceAlignment.
        /// </summary>
        public int Count
        {
            get
            {
                return sequences.Count;
            }

        }

        /// <summary>
        /// true if SequenceAlignment is read-only, false if data can be changed.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Removes item from the list of sequences in the SequenceAlignment.
        /// Throws exception if SequenceAlignment is read only.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if item was removed, false if item was not found.</returns>
        public bool Remove(ISequence item)
        {
            if (IsReadOnly)
            {
                Trace.Report(Resource.READ_ONLY_COLLECTION_MESSAGE);
                throw new NotSupportedException(Resource.READ_ONLY_COLLECTION_MESSAGE);
            }

            return sequences.Remove(item);
        }

        #endregion

        #region IEnumerable<ISequence> Members

        /// <summary>
        /// Returns an enumerator for the sequences in the SequenceAlignment.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ISequence> GetEnumerator()
        {
            return sequences.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator for the sequences in the SequenceAlignment.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return sequences.GetEnumerator();
        }

        #endregion

        #region ISerializable Members
        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming context.</param>
        internal SequenceAlignment(SerializationInfo info, StreamingContext context)
        {
            consensus = (ISequence)info.GetValue("SequenceAlignment:Consensus", typeof(ISequence));
            sequences = (List<ISequence>)info.GetValue("SequenceAlignment:Sequences", typeof(List<ISequence>));
            score = info.GetInt32("SequenceAlignment:Score");
            IsReadOnly = info.GetBoolean("SequenceAlignment:IsReadOnly");
            Documentation = info.GetValue("SequenceAlignment:Documentation", typeof(object));
        }

        /// <summary>
        /// Method for serializing the SequenceAlignment.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SequenceAlignment:Consensus", consensus);
            info.AddValue("SequenceAlignment:Sequences", sequences);
            info.AddValue("SequenceAlignment:Score", score);
            info.AddValue("SequenceAlignment:IsReadOnly", IsReadOnly);

            if (Documentation != null && ((Documentation.GetType().Attributes &
                System.Reflection.TypeAttributes.Serializable) == System.Reflection.TypeAttributes.Serializable))
            {
                info.AddValue("SequenceAlignment:Documentation", Documentation);
            }
            else
            {
                info.AddValue("SequenceAlignment:Documentation", null);
            }
        }

        #endregion
    }
}
