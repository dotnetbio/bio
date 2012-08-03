using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Collections;

namespace Bio
{
    /// <summary>
    /// SparseSequence can hold discontinuous sequence. Use this class for storing the sequence items 
    /// with their known position from a long continuous sequence.  This class uses SortedDictionary to store 
    /// the sequence items with their position. Position is zero based indexes at which a sequence items 
    /// are present in the original continues sequence.
    /// For example: 
    /// To store sequence items at position 10, 101, 200, 1501 this class can be used as shown in the below code.
    /// 
    /// // Create a SparseSequence by specifying the Alphabet.
    /// SparseSequence mySparseSequence= new SparseSequence(Alphabets.DNA);
    /// 
    /// // By default count will be set to zero. To insert a sequence item at a position greater than zero,
    /// // Count has to be set to a value greater than the maximum position value. 
    /// // If try to insert a sequence item at a position greater than the count an exception will occur.
    /// // You can limit the SparseSequence length by setting the count to desired value. In this example it 
    /// will be 1502 as the maximum index is 1501.
    /// mySparseSequence.Count = 1502;
    /// 
    /// // To access the value in a SparseSequence use Indexer or an Enumerator like below.
    ///
    /// // Accessing SparsesSequence using Indexer.
    /// byte seqItem1 = mySparseSequence [10] ;  // this will return sequence item A.
    /// byte seqItem2 = mySparseSequence [1501] ;  // this will return sequence item G.
    /// byte seqItem3 = mySparseSequence [102] ;  // this will return null as there is no sequence item at this position.
    /// 
    /// // Accessing SparsesSequence using Enumerator.
    /// foreach(byte seqItem in mySparseSequence) {…}
    /// </summary>
    public class SparseSequence : ISequence
    {
        #region Field

        /// <summary>
        /// Holds sequence items with their position.
        /// </summary>
        private SortedDictionary<long, byte> sparseSeqItems = new SortedDictionary<long, byte>();

        /// <summary>
        /// Holds size of this sequence.
        /// </summary>
        private long count;

        /// <summary>
        /// Metadata is features or references or related things of a sequence.
        /// </summary>
        private Dictionary<string, object> metadata;

        #endregion Field

        #region Constructors

        /// <summary>
        /// Creates a SparseSequence with no sequence data.
        /// 
        /// Count property of SparseSequence instance created by using this constructor will be set to zero.
        /// 
        /// For working with sequences that never have sequence data, but are
        /// only used for metadata storage (like keeping an ID or various features
        /// but no direct sequence data) consider using the VirtualSequence
        /// class instead.
        /// </summary>
        /// <param name="alphabet"> 
        /// The alphabet the sequence uses (e.g.. Alphabets.DNA or Alphabets.RNA or Alphabets.Protein)
        /// </param>
        public SparseSequence(IAlphabet alphabet)
            : this(alphabet, 0) { }

        /// <summary>
        /// Creates a SparseSequence with no sequence data.
        /// 
        /// Count property of SparseSequence instance created by using this constructor will be 
        /// set a value specified by size parameter.
        /// 
        /// For working with sequences that never have sequence data, but are
        /// only used for metadata storage (like keeping an ID or various features
        /// but no direct sequence data) consider using the VirtualSequence
        /// class instead.
        /// </summary>
        /// <param name="alphabet"> 
        /// The alphabet the sequence uses (e.g.. Alphabets.DNA or Alphabets.RNA or Alphabets.Protein)
        /// </param>
        /// <param name="size">A value indicating the size of this sequence.</param>
        public SparseSequence(IAlphabet alphabet, int size)
        {
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(Properties.Resource.ParameterNameSize, Properties.Resource.ParameterMustNonNegative);
            }

            Count = size;
            Alphabet = alphabet;

            Statistics = new SequenceStatistics(alphabet);
        }

        /// <summary>
        /// Creates a sparse sequence based on the specified parameters.
        /// 
        /// The item parameter must contain an alphabet as specified in the alphabet parameter,
        /// else an exception will occur.
        /// 
        /// The index parameter value must be a non negative value.
        /// Count property of an instance created by this constructor will be set to value of index + 1.
        /// </summary>
        /// <param name="alphabet">
        /// The alphabet the sequence uses (e.g. Alphabets.DNA or Alphabets.RNA or Alphabets.Protein)</param>
        /// <param name="index">Position of the specified sequence item.</param>
        /// <param name="item">A sequence item which is known by the alphabet.</param>
        public SparseSequence(IAlphabet alphabet, int index, byte item)
            : this(alphabet)
        {

            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (index < 0 || index == int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNameIndex,
                    Properties.Resource.SparseSequenceConstructorIndexOutofRange);
            }

            if (!alphabet.ValidateSequence(new[] { item }, 0, 1))
            {
                throw new ArgumentException(
                    string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resource.InvalidSymbol,
                    item));
            }

            Statistics = new SequenceStatistics(alphabet);

            sparseSeqItems.Add(index, item);
            Statistics.Add((char)item);

            Count = index + 1;
        }

        /// <summary>
        /// Creates a sparse sequence based on the specified parameters.
        /// The sequenceItems parameter must contain sequence items known by the specified alphabet,
        /// else an exception will occur.
        /// 
        /// The index parameter value must be a non negative. 
        /// </summary>
        /// <param name="alphabet">
        /// The alphabet the sequence uses (e.g.. Alphabets.DNA or Alphabets.RNA or Alphabets.Protein)</param>
        /// <param name="index">A non negative value which indicates the start position of the specified sequence items.</param>
        /// <param name="sequenceItems">
        /// A sequence which contain items known by the alphabet.</param>
        public SparseSequence(IAlphabet alphabet, int index, IEnumerable<byte> sequenceItems)
            : this(alphabet)
        {
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (index < 0 || index == int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNameIndex,
                    Properties.Resource.SparseSequenceConstructorIndexOutofRange);
            }

            if (sequenceItems == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameSequenceItems);
            }

            var sequenceArray = sequenceItems.ToArray();
            if (!alphabet.ValidateSequence(sequenceArray, 0, sequenceArray.LongLength))
            {
                throw new ArgumentOutOfRangeException("sequenceItems");
            }

            Statistics = new SequenceStatistics(alphabet);

            int position = index;
            foreach (byte sequenceItem in sequenceItems)
            {
                sparseSeqItems.Add(position, sequenceItem);
                Statistics.Add((char)sequenceItem);
                position++;
            }

            if (sequenceItems.Count() > 0)
            {
                Count = index + sequenceItems.Count();
            }
        }

        /// <summary>
        /// Creates a sparse sequence based on the new passed sequence.
        /// </summary>
        /// <param name="newSequence">The New sequence for which the copy has to be made</param>
        public SparseSequence(ISequence newSequence):this(newSequence.Alphabet, 0, newSequence)
        {
        }
        
        #endregion Constructors

        #region Properties
        /// <summary>
        /// An identification provided to distinguish the sequence to others
        /// being worked with.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The number of sequence items contained in the Sequence.
        /// </summary>
        public long Count
        {
            get
            {
                return count;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(Properties.Resource.ParameterNameValue, Properties.Resource.ParameterMustNonNegative);
                }

                count = value;
            }
        }

        /// <summary>
        /// The alphabet to which string representations of the sequence should
        /// conform.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Keeps track of the number of occurrences of each symbol within a sequence.
        /// </summary>
        public SequenceStatistics Statistics { get; private set; }
        

        /// <summary>
        /// Many sequence representations when saved to file also contain
        /// information about that sequence. Unfortunately there is no standard
        /// around what that data may be from format to format. This property
        /// allows a place to put structured metadata that can be accessed by
        /// a particular key.
        /// 
        /// For example, if species information is stored in a particular Species
        /// class, you could add it to the dictionary by:
        /// 
        /// mySequence.Metadata["SpeciesInfo"] = mySpeciesInfo;
        /// 
        /// To fetch the data you would use:
        /// 
        /// Species mySpeciesInfo = mySequence.Metadata["SpeciesInfo"];
        /// 
        /// Particular formats may create their own data model class for information
        /// unique to their format as well. Such as:
        /// 
        /// GenBankMetadata genBankData = new GenBankMetadata();
        /// // ... add population code
        /// mySequence.MetaData["GenBank"] = genBankData;
        /// </summary>
        public Dictionary<string, object> Metadata
        {
            get
            {
                if (metadata == null)
                {
                    metadata = new Dictionary<string, object>();
                }

                return metadata;
            }
        }

        #endregion Properties

        #region Methods
        /// <summary>
        /// Allows the sequence to function like an array, gets
        /// the sequence item at the specified index. Note that the
        /// index value starts its count at 0.
        /// </summary>
        public byte this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(
                         Properties.Resource.ParameterNameIndex,
                         Properties.Resource.ParameterMustLessThanCount);

                byte item = byte.MinValue;
                if (sparseSeqItems.ContainsKey(index))
                {
                    item = sparseSeqItems[index];
                }

                return item;
            }

            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(
                         Properties.Resource.ParameterNameIndex,
                         Properties.Resource.ParameterMustLessThanCount);

                Replace(index, value);
            }
        }

        /// <summary>
        /// Return a sequence representing this sequence with the orientation reversed.
        /// </summary>
        /// <returns>The reversed sequence.</returns>
        public ISequence GetReversedSequence()
        {
            // reversed = true, complemented = false, range is a no-op.
            return new DerivedSequence(this, true, false);
        }

        /// <summary>
        /// Return a sequence representing the complement of this sequence.
        /// </summary>
        /// <returns>The complemented sequence.</returns>
        public ISequence GetComplementedSequence()
        {
            // reversed = false, complemented = true, range is a no-op.
            return new DerivedSequence(this, false, true);
        }

        /// <summary>
        /// Return a sequence representing the reverse complement of this sequence.
        /// </summary>
        /// <returns>The reverse complemented sequence.</returns>
        public ISequence GetReverseComplementedSequence()
        {
            // reversed = true, complemented = true, range is a no-op.
            return new DerivedSequence(this, true, true);
        }

        /// <summary>
        /// Return a new sequence representing a range (subsequence) of this sequence.
        /// </summary>
        /// <param name="start">The index of the first symbol in the range.</param>
        /// <param name="length">The number of symbols in the range.</param>
        /// <returns>The sub-sequence.</returns>
        public ISequence GetSubSequence(long start, long length)
        {
            if (start >= this.Count)
            {
                throw new ArgumentOutOfRangeException("start");
            }

            if (start + length > this.Count)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            byte[] subSequence = new byte[length];
            for (long index = 0; index < length; index++)
            {
                subSequence[index] = this[start + index];
            }

            return new Sequence(this.Alphabet, subSequence, false);
        }

        /// <summary>
        /// Copies all items from the sequence to a pre allocated array.
        /// </summary>
        /// <param name="byteArray">Array to fill the items to.</param>
        /// <param name="start">Index at which the filling starts.</param>
        /// <param name="count">Total numbers of elements to be copied.</param>
        public void CopyTo(byte[] byteArray, long start, long count)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameArray);
            }

            if ((start + count) > this.count)
            {
                throw new ArgumentException(Properties.Resource.DestArrayNotLargeEnough);
            }

            Sequence seq = (Sequence)this.GetSubSequence(0, this.sparseSeqItems.Count);
            seq.CopyTo(byteArray, start, count);
        }

        /// <summary>
        /// Gets the index of first non gap character.
        /// </summary>
        /// <returns>If found returns an zero based index of the first non gap character, otherwise returns -1.</returns>
        public long IndexOfNonGap()
        {
            if (Count > 0)
            {
                return IndexOfNonGap(0);
            }

            return -1;
        }

        /// <summary>
        /// Returns the position of the first item from startPos that does not 
        /// have a Gap character.
        /// </summary>
        /// <param name="startPos">Index value above which to search for non-Gap character.</param>
        /// <returns>If found returns an zero based index of the first non gap character, otherwise returns -1.</returns>
        public long IndexOfNonGap(long startPos)
        {
            if (startPos < 0 || startPos >= Count)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNameStartPos,
                    Properties.Resource.ParameterMustLessThanCount);
            }

            try
            {
                HashSet<byte> gapSymbols;

                if (!this.Alphabet.TryGetGapSymbols(out gapSymbols))
                {
                    return startPos;
                }

                byte[] aliasSymbolsMap = this.Alphabet.GetSymbolValueMap();

                for (long index = startPos; index < this.Count; index++)
                {
                    byte symbol = aliasSymbolsMap[this.sparseSeqItems[index]];
                    if (!gapSymbols.Contains(symbol))
                    {
                        return index;
                    }
                }

                return -1;
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the index of last non gap character.
        /// </summary>
        /// <returns>If found returns an zero based index of the last non gap character, otherwise returns -1.</returns>
        public long LastIndexOfNonGap()
        {
            if (Count > 0)
            {
                return LastIndexOfNonGap(Count - 1);
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of last non gap character within the specified end position.
        /// </summary>
        /// <param name="endPos">Index value below which to search for non-Gap character.</param>
        /// <returns>If found returns an zero based index of the last non gap character, otherwise returns -1.</returns>
        public long LastIndexOfNonGap(long endPos)
        {
            if (endPos < 0 || endPos >= Count)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNameEndPos,
                    Properties.Resource.ParameterMustLessThanCount);
            }

            try
            {
                HashSet<byte> gapSymbols;

                if (!this.Alphabet.TryGetGapSymbols(out gapSymbols))
                {
                    return endPos;
                }

                byte[] aliasSymbolsMap = this.Alphabet.GetSymbolValueMap();

                for (long index = endPos; index >= 0; index--)
                {
                    byte symbol = aliasSymbolsMap[this.sparseSeqItems[index]];
                    if (!gapSymbols.Contains(symbol))
                    {
                        return index;
                    }
                }

                return -1;
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns known sequence items with their position as ReadOnlyCollection of IndexedSequenceItem.
        /// </summary>
        /// <returns>Sequence items with their position as ReadOnlyCollection of IndexedSequenceItem.</returns>
        public IList<IndexedItem<byte>> GetKnownSequenceItems()
        {
            List<IndexedItem<byte>> indexedSeqItems = sparseSeqItems.Keys.Select(key => new IndexedItem<byte>(key, sparseSeqItems[key])).ToList();

            return indexedSeqItems.AsReadOnly();
        }

        /// <summary>
        /// Gets an enumerator to the bytes present in this sequence.
        /// </summary>
        /// <returns>An IEnumerator of bytes.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            for (long index = 0; index < this.Count; index++)
            {
                yield return this[index];
            }
        }

        /// <summary>
        /// Gets an enumerator to the bytes present in this sequence.
        /// </summary>
        /// <returns>An IEnumerator of bytes.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Replaces the sequence item present in the specified position in this sequence with the specified sequence item. 
        /// </summary>
        /// <param name="position">Position at which the sequence item has to be replaced.</param>
        /// <param name="item">Sequence item to be placed at the specified position.</param>
        private void Replace(long position, byte item)
        {
            if (position < 0 || position >= Count)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNamePosition,
                    Properties.Resource.ParameterMustLessThanCount);
            }

            if (item == 0)
            {
                if (sparseSeqItems.ContainsKey(position))
                {
                    Statistics.Remove((char)sparseSeqItems[position]);
                    sparseSeqItems.Remove(position);
                }
            }
            else
            {
                if (!Alphabet.ValidateSequence(new[] { item }, 0, 1))
                {
                    throw new ArgumentException(
                        string.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resource.InvalidSymbol,
                        item));
                }

                if (sparseSeqItems.ContainsKey(position))
                {
                    Statistics.Remove((char)sparseSeqItems[position]);
                    sparseSeqItems[position] = item;
                }
                else
                {
                    sparseSeqItems.Add(position, item);
                }

                Statistics.Add((char)item);
            }
        }
        #endregion Methods
    }
}
