using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bio.Extensions;
using Bio.Util;

namespace Bio
{
    /// <summary>
    /// This is the standard implementation of the ISequence interface. It contains
    /// the raw data that defines the contents of a sequence. Since Sequence uses 
    /// enumerable of bytes that can be accessed as follows:
    /// Sequence mySequence = new Sequence(Alphabets.DNA, "GATTC");
    /// foreach (Nucleotide nucleotide in mySequence) { ... }
    /// The results will be based on the Alphabet associated with the
    /// sequence. Common alphabets include those for DNA, RNA, and Amino Acids.
    /// For users who wish to get at the underlying data directly, Sequence provides
    /// a means to do this as well. This may be useful for those writing algorithms
    /// against the sequence where performance is especially important. For these
    /// advanced users access is provided to the encoding classes associated with the
    /// sequence.
    /// </summary>
    public sealed class Sequence : ISequence
    {
        #region Member variables

        /// <summary>
        /// Holds the sequence data.
        /// </summary>
        private byte[] _sequenceData;

        /// <summary>
        /// Metadata is features or references or related things of a sequence.
        /// </summary>
        private Dictionary<string, object> _metadata;

        #endregion Member variables

        #region Constructors
        /// <summary>
        /// Constructor used when copying an existing sequence internally for reverse/complement usage
        /// to avoid double-copying the buffer.
        /// </summary>
        private Sequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with specified alphabet and string sequence.
        /// Symbols in the sequence are validated with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this class should conform.</param>
        /// <param name="sequence">The sequence in string form.</param>
        public Sequence(IAlphabet alphabet, string sequence)
            : this(alphabet, sequence, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with specified alphabet and string sequence.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this class should conform.</param>
        /// <param name="sequence">The sequence in string form.</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public Sequence(IAlphabet alphabet, string sequence, bool validate)
        {
            // validate the inputs
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            this.Alphabet = alphabet;
            this.ID = string.Empty;
            byte[] values = Encoding.UTF8.GetBytes(sequence);

            if (validate)
            {
                // Validate sequence data
                if (!alphabet.ValidateSequence(values, 0, values.LongLength()))
                {
                    throw Helper.GenerateAlphabetCheckFailureException(alphabet, values);
                }
            }

            this._sequenceData = values;
            this.Count = this._sequenceData.LongLength();
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with specified alphabet and bytes.
        /// Bytes representing Symbols in the values are validated with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="values">An array of bytes representing the symbols.</param>
        public Sequence(IAlphabet alphabet, byte[] values)
            : this(alphabet, values, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with specified alphabet and bytes.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="values">An array of bytes representing the symbols.</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public Sequence(IAlphabet alphabet, byte[] values, bool validate)
        {
            // validate the inputs
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (validate)
            {
                // Validate sequence data
                if (!alphabet.ValidateSequence(values, 0, values.LongLength()))
                {
                    throw Helper.GenerateAlphabetCheckFailureException(alphabet, values);
                }
            }

            this._sequenceData = new byte[values.LongLength()];
            this.ID = string.Empty;

#if (SILVERLIGHT == false)
            Array.Copy(values, this._sequenceData, values.LongLength);
#else
            Array.Copy(values, this._sequenceData, values.Length);  
#endif

            this.Alphabet = alphabet;
            this.Count = this._sequenceData.LongLength();
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with passed new Sequence. Creates a copy of the sequence.
        /// </summary>
        /// <param name="newSequence">The New sequence for which the copy has to be made.</param>
        public Sequence(ISequence newSequence)
        {
            if (newSequence == null)
            {
                throw new ArgumentNullException("newSequence");
            }

            this.ID = newSequence.ID;
            this.Alphabet = newSequence.Alphabet;
            this.Count = newSequence.Count;
            this._metadata = new Dictionary<string, object>(newSequence.Metadata);

            Sequence realSequence = newSequence as Sequence;
            if (realSequence != null)
            {
                this._sequenceData = new byte[newSequence.Count];
                Array.Copy(realSequence._sequenceData, this._sequenceData, realSequence._sequenceData.LongLength);
            }
            else
            {
                this._sequenceData = newSequence.ToArray();
            }
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets an identifier for this instance of sequence class.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets the number of bytes contained in the Sequence.
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Gets or sets the alphabet to which symbols in this sequence belongs to.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Gets or sets the Metadata of this instance.
        /// Many sequence representations when saved to file also contain
        /// information about that sequence. Unfortunately there is no standard
        /// around what that data may be from format to format. This property
        /// allows a place to put structured metadata that can be accessed by
        /// a particular key.
        /// <para>
        /// For example, if species information is stored in a particular Species
        /// class, you could add it to the dictionary by:
        /// </para>
        /// <para>
        /// mySequence.Metadata["SpeciesInfo"] = mySpeciesInfo;
        /// </para>
        /// <para>
        /// To fetch the data you would use:
        /// </para>
        /// <para>
        /// Species mySpeciesInfo = mySequence.Metadata["SpeciesInfo"];
        /// </para>
        /// <para>
        /// Particular formats may create their own data model class for information
        /// unique to their format as well. Such as:
        /// </para>
        /// <para>
        /// GenBankMetadata genBankData = new GenBankMetadata();
        /// // ... add population code
        /// mySequence.MetaData["GenBank"] = genBankData;.
        /// </para>
        /// </summary>
        public Dictionary<string, object> Metadata
        {
            get { return this._metadata ?? (this._metadata = new Dictionary<string, object>()); }
            set { this._metadata = value; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Returns the byte found at the specified index if within bounds. Note 
        /// that the index value starts at 0.
        /// </summary>
        /// <param name="index">Index at which the symbol is required.</param>
        /// <returns>Byte value at the given index.</returns>
        public byte this[long index]
        {
            get
            {
                return this._sequenceData[index];
            }
        }

        /// <summary>
        /// Return a new sequence representing this sequence with the orientation reversed.
        /// </summary>
        public ISequence GetReversedSequence()
        {
            byte[] values = new byte[this.Count];

#if (SILVERLIGHT == false)
            Array.Copy(this._sequenceData, values, this._sequenceData.LongLength);
#else
            Array.Copy(this._sequenceData, values, this._sequenceData.Length);  
#endif

            Array.Reverse(values);
            Sequence seq = new Sequence { _sequenceData = values, Alphabet = this.Alphabet, ID = this.ID, Count = this.Count };
            if (this._metadata != null)
                seq._metadata = new Dictionary<string, object>(this._metadata);

            return seq;
        }

        /// <summary>
        /// Return a new sequence representing the complement of this sequence.
        /// </summary>
        public ISequence GetComplementedSequence()
        {
            if (!this.Alphabet.IsComplementSupported)
            {
                throw new InvalidOperationException(Properties.Resource.ComplementNotFound);
            }

            byte[] complemented = new byte[this.Count];
            this.Alphabet.TryGetComplementSymbol(this._sequenceData, out complemented);

            Sequence seq = new Sequence { _sequenceData = complemented, Alphabet = this.Alphabet, ID = this.ID, Count = this.Count };
            if (this._metadata != null)
                seq._metadata = new Dictionary<string, object>(this._metadata);

            return seq;
        }

        /// <summary>
        /// Return a new sequence representing the reverse complement of this sequence.
        /// </summary>
        public ISequence GetReverseComplementedSequence()
        {
            if (!this.Alphabet.IsComplementSupported)
            {
                throw new InvalidOperationException(Properties.Resource.ComplementNotFound);
            }

            byte[] reverseComplemented = new byte[this.Count];
            this.Alphabet.TryGetComplementSymbol(this._sequenceData, out reverseComplemented);
            Array.Reverse(reverseComplemented);
            Sequence seq = new Sequence { _sequenceData = reverseComplemented, Alphabet = this.Alphabet, ID = this.ID, Count = this.Count };
            if (this._metadata != null)
                seq._metadata = new Dictionary<string, object>(this._metadata);

            return seq;
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
                subSequence[index] = this._sequenceData[start + index];
            }

            Sequence seq = new Sequence { _sequenceData = subSequence, Alphabet = this.Alphabet, ID = this.ID, Count = subSequence.Length };
            if (this._metadata != null)
                seq._metadata = new Dictionary<string, object>(this._metadata);
            return seq;
        }

        /// <summary>
        /// Gets the index of first non-gap symbol.
        /// </summary>
        /// <returns>If found returns a zero based index of the first non-gap symbol, otherwise returns -1.</returns>
        public long IndexOfNonGap()
        {
            return this.IndexOfNonGap(0);
        }

        /// <summary>
        /// Returns the position of the first symbol beyond startPos that does not 
        /// have a Gap symbol.
        /// </summary>
        /// <param name="startPos">Index value beyond which the non-gap symbol is searched for.</param>
        /// <returns>If found returns a zero based index of the first non-gap symbol, otherwise returns -1.</returns>
        public long IndexOfNonGap(long startPos)
        {
            if (startPos >= this._sequenceData.LongLength())
            {
                throw new ArgumentOutOfRangeException("startPos");
            }

            HashSet<byte> gapSymbols;
            if (!this.Alphabet.TryGetGapSymbols(out gapSymbols))
            {
                return startPos;
            }

            byte[] aliasSymbolsMap = this.Alphabet.GetSymbolValueMap();

            for (long index = startPos; index < this.Count; index++)
            {
                byte symbol = aliasSymbolsMap[this._sequenceData[index]];
                if (!gapSymbols.Contains(symbol))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of last non-gap symbol.
        /// </summary>
        /// <returns>If found returns a zero based index of the last non-gap symbol, otherwise returns -1.</returns>
        public long LastIndexOfNonGap()
        {
            return this.LastIndexOfNonGap(this.Count - 1);
        }

        /// <summary>
        /// Returns the index of last non-gap symbol before the specified end position.
        /// </summary>
        /// <param name="endPos">Index value up to which the non-Gap symbol is searched for.</param>
        /// <returns>If found returns a zero based index of the last non-gap symbol, otherwise returns -1.</returns>
        public long LastIndexOfNonGap(long endPos)
        {
            HashSet<byte> gapSymbols;

            if (!this.Alphabet.TryGetGapSymbols(out gapSymbols))
            {
                return endPos;
            }

            byte[] aliasSymbolsMap = this.Alphabet.GetSymbolValueMap();
            for (long index = endPos; index >= 0; index--)
            {
                byte symbol = aliasSymbolsMap[this._sequenceData[index]];
                if (!gapSymbols.Contains(symbol))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets an enumerator to the bytes present in this sequence.
        /// </summary>
        /// <returns>An IEnumerator of bytes.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            for (long index = 0; index < this.Count; index++)
            {
                yield return this._sequenceData[index];
            }
        }

        /// <summary>
        /// Returns a string representation of the sequence data. This representation
        /// will come from the symbols in the alphabet defined for the sequence.
        /// 
        /// Thus a Sequence whose Alphabet is Alphabets.DNA may return a value like
        /// 'GATTCCA'
        /// </summary>
        public override string ToString()
        {
            if (this.Count > Helper.AlphabetsToShowInToString)
            {
                return string.Format(CultureInfo.CurrentCulture, Properties.Resource.ToStringFormat,
                                     new string(this._sequenceData.Take(Helper.AlphabetsToShowInToString).Select((a => (char)a)).ToArray()),
                                     (this.Count - Helper.AlphabetsToShowInToString));
            }
            else
            {
                return new string(this._sequenceData.Take(this._sequenceData.Length).Select((a => (char)a)).ToArray());
            }
        }

        /// <summary>
        /// Converts part of the sequence to a string.
        /// </summary>
        /// <param name="startIndex">Start position of the sequence.</param>
        /// <param name="length">Number of symbols to return.</param>
        /// <returns>Part of the sequence in string format.</returns>
        public string ConvertToString(long startIndex, long length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "<= 0");
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", "> 0");
            }

            StringBuilder sb = new StringBuilder();
            try
            {
                for (long index = startIndex; index < startIndex + length; index++)
                {
                    sb.Append((char)this._sequenceData[index]);
                }
            }
            catch (IndexOutOfRangeException rangeEx)
            {
                throw new ArgumentOutOfRangeException("length", rangeEx.Message);
            }

            return sb.ToString();
        }

#if !SILVERLIGHT
        /// <summary>
        /// This is used by some of the built-in algorithms which access the data in a read-only fashion
        /// to quickly grab a sequence of data without copying it.  It cannot be used outside Bio.dll
        /// For outside users, use the CopyTo method.
        /// </summary>
        /// <returns></returns>
        internal byte[] GetInternalArray()
        {
            return this._sequenceData;
        }
#endif

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

            if ((start + count) > this.Count)
            {
                throw new ArgumentException(Properties.Resource.DestArrayNotLargeEnough);
            }

            if (start < 0)
            {
                throw new ArgumentException(Properties.Resource.StartCannotBeLessThanZero);
            }

            if (count < 0)
            {
                throw new ArgumentException(Properties.Resource.CountCannotBeLessThanZero);
            }

#if !SILVERLIGHT
            Array.Copy(this._sequenceData, start, byteArray, 0, count);
#else
            Array.Copy(this._sequenceData, (int)start, byteArray, 0, (int)count);
#endif
        }

        /// <summary>
        /// Gets an enumerator to the bytes present in this sequence.
        /// </summary>
        /// <returns>An IEnumerator of bytes.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._sequenceData.GetEnumerator();
        }
        #endregion
    }
}
