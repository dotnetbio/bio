using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of profiles class.
    /// 
    /// Profile is a multiple alignment treated as a sequence by 
    /// regarding each column as an alignable symbol. Thus two sets
    /// of sequences can be aligned by aligning profiles.
    /// 
    /// The symbol is a distribution vector, recording the frequencies
    /// of items in the column.
    /// 
    /// The class defines such a matrix of profiles, and provides a 
    /// set of static methods to generate profiles from one single 
    /// sequence, a set of aligned sequences, or other profiles.
    /// 
    /// </summary>
    public class Profiles : IProfiles
    {
        #region Fields

        // The list of profiles
        private List<float[]> _profilesMatrix = null;

        // The row dimension of profiles matrix
        private int _rowSize;

        // The column dimension of profiles matrix
        private int _colSize;

        private static Dictionary<byte, int> internalItemSet;
        #endregion

        #region Properties
        /// <summary>
        /// The column vector is a distribution vector of sequence item 
        /// frequencies. The sequence item order is kept in this ItemSet
        /// dictionary, so that the profile vectors are consistent for a 
        /// certain set of sequences.
        /// </summary>
        public static Dictionary<byte, int> ItemSet
        {
            get
            {
                return internalItemSet;
            }
            set
            {
                internalItemSet = value;
                ItemSetSmallInverted = new Dictionary<int, byte>();

                foreach (var item in internalItemSet.Where(a => !char.IsLower((char)a.Key)))
                {
                    Profiles.ItemSetSmallInverted.Add(item.Value, item.Key);
                }
            }
        }

        /// <summary>
        /// Similar to item set but will contain only the uppercase charectors.
        /// </summary>
        public static Dictionary<int, byte> ItemSetSmallInverted
        {
            get;
            set;
        }

        /// <summary>
        /// Map ambiguous a character to the list of original characters.
        /// </summary>
        public static Dictionary<byte, List<byte>> AmbiguousCharactersMap
        {
            get;
            set;
        }

        /// <summary>
        /// The number of basic characters
        /// </summary>
        public static int NumberOfBasicCharacters
        {
            get;
            set;
        }
        #endregion

        #region Interface methods

        /// <summary>
        /// The row dimension of profile matrix
        /// </summary>
        public int RowSize 
        { 
            get { return _rowSize; }
            set { _rowSize = value; }
        }

        /// <summary>
        /// The column dimension of profile matrix
        /// </summary>
        public int ColumnSize 
        { 
            get { return _colSize; }
            set { _colSize = value; }
        }

        /// <summary>
        /// The profiles of this class
        /// </summary>
        public List<float[]> ProfilesMatrix
        { 
            get { return _profilesMatrix; }
            set { _profilesMatrix = value; }
        }

        /// <summary>
        /// Access column vector (profile)
        /// </summary>
        /// <param name="col">zero-based column index</param>
        public float[] this[int col] 
        {
            get
            {
                if (col < _rowSize && col >= 0)
                {
                    return _profilesMatrix[col];
                }
                else
                {
                    throw new ArgumentException("Invalid column index");
                }
            }

            set
            {
                if (col < _rowSize && col >= 0)
                {
                    if (value.Length == _colSize)
                    {
                        _profilesMatrix[col] = value;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid input profile vector");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid column index");
                }
            }
        }

        /// <summary>
        /// Clear profiles.
        /// Once the profiles of two children nodes are aligned in the tree,
        /// the aligned profiles is assigned to the parent node, then the profiles
        /// of the children nodes can be cleared.
        /// </summary>
        public void Clear()
        {
            _profilesMatrix.Clear();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construction with empty profiles
        /// </summary>
        public Profiles()
        {
            _profilesMatrix = new List<float[]>();
        }

        /// <summary>
        /// Allocate space in memory
        /// </summary>
        /// <param name="rowSize">zero-based row index</param>
        /// <param name="colSize">zero-based col index</param>
        public Profiles(int rowSize, int colSize)
        {
            _rowSize = rowSize;
            _colSize = colSize;
            try
            {
                _profilesMatrix = new List<float[]>(_rowSize);
                for (int i = 0; i < _rowSize; ++i)
                {
                    _profilesMatrix.Add(new float[_colSize]);
                }
            }
            catch (OutOfMemoryException ex)
            {
                throw new Exception("Out of memory when creating profiles", ex.InnerException);
            }
        }

        /// <summary>
        /// Copy from an existing profiles
        /// </summary>
        /// <param name="p">an existing profile class</param>
        public Profiles(IProfiles p)
        {
            _rowSize = p.RowSize;
            _colSize = p.ColumnSize;
            _profilesMatrix = p.ProfilesMatrix;
        }
        #endregion

        #region Static methods: these methods generate IProfiles

        /// <summary>
        /// Generate profiles from one single sequence
        /// The set of sequence items of the seq should be the same as 
        /// 'static ItemSet' of this class.
        /// </summary>
        /// <param name="seq">an input sequence</param>
        public static IProfiles GenerateProfiles(ISequence seq)
        {
            IProfiles profiles;
            int sequenceLength = (int)seq.Count;

            int colSize = (ItemSet.Count + 1) / 2;
            profiles = new Profiles(sequenceLength, colSize);

            for (int i = 0; i < sequenceLength; ++i)
            {
                try
                {
                    if (seq.Alphabet.CheckIsAmbiguous(seq[i]))
                    {
                        for (int b = 0; b < AmbiguousCharactersMap[seq[i]].Count; ++b)
                        {
                            ++(profiles[i][ItemSet[AmbiguousCharactersMap[seq[i]][b]]]);
                        }
                    }
                    else
                    {
                        ++(profiles[i][ItemSet[seq[i]]]);
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new Exception("Invalid alphabet", ex.InnerException);
                }
                //MsaUtils.Normalize(profiles[i]);
            }
            profiles.ColumnSize = colSize;
            profiles.RowSize = sequenceLength;
            return profiles;
        }

        /// <summary>
        /// Generate profiles from one single sequence
        /// The set of sequence items of the seq should be the same as 
        /// 'static ItemSet' of this class.
        /// </summary>
        /// <param name="seq">an input sequence</param>
        /// <param name="weight">sequence weight</param>
        public static IProfiles GenerateProfiles(ISequence seq, float weight)
        {
            int sequenceLength = (int)seq.Count;
            int colSize = (ItemSet.Count + 1) / 2;
            IProfiles profiles = new Profiles(sequenceLength, colSize);

            for (int i = 0; i < sequenceLength; ++i)
            {
                try
                {
                    if (seq.Alphabet.CheckIsAmbiguous(seq[i]))
                    {
                        for (int b = 0; b < AmbiguousCharactersMap[seq[i]].Count; ++b)
                        {
                            //profiles[i][ItemSet[AmbiguousCharactersMap[seq[i]][b]]] += weight;
                            ++(profiles[i][ItemSet[AmbiguousCharactersMap[seq[i]][b]]]);
                        }
                    }
                    else
                    {
                        //profiles[i][ItemSet[seq[i]]] += weight;
                        ++(profiles[i][ItemSet[seq[i]]]);
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new Exception("Invalid alphabet", ex.InnerException);
                }
                //MsaUtils.Normalize(profiles[i]);
            }
            profiles.ColumnSize = colSize;
            profiles.RowSize = sequenceLength;
            return profiles;
        }
        /// <summary>
        /// Generate IProfiles from a set of aligned sequences
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        public static IProfiles GenerateProfiles(ICollection<ISequence> sequences)
        {
            IEnumerator<ISequence> enumeratorSeq = sequences.GetEnumerator();
            enumeratorSeq.MoveNext();
            int sequenceLength = (int)enumeratorSeq.Current.Count;
            IAlphabet alphabet = enumeratorSeq.Current.Alphabet;
            while (enumeratorSeq.MoveNext())
            {
                if (enumeratorSeq.Current.Count!=sequenceLength)
                {
                    throw new ArgumentException("Input sequences are not aligned");
                }
                if (enumeratorSeq.Current.Alphabet != alphabet)
                {
                    throw new ArgumentException("Input sequences use different alphabets");
                }
            }

            int colSize = (ItemSet.Count + 1) / 2;
            IProfiles profiles = new Profiles(sequenceLength, colSize);

            for (int i = 0; i < sequenceLength; ++i)
            {
                enumeratorSeq.Reset();
                while (enumeratorSeq.MoveNext())
                {
                    if (enumeratorSeq.Current.Alphabet.CheckIsAmbiguous(enumeratorSeq.Current[i]))
                    {
                        for (int b = 0; b < AmbiguousCharactersMap[enumeratorSeq.Current[i]].Count; ++b)
                        {
                            ++(profiles[i][ItemSet[AmbiguousCharactersMap[enumeratorSeq.Current[i]][b]]]);
                        }
                    }
                    else
                    {
                        ++(profiles[i][ItemSet[enumeratorSeq.Current[i]]]);
                    }
                }
                MsaUtils.Normalize(profiles[i]);
            }
            profiles.ColumnSize = colSize;
            profiles.RowSize = sequenceLength;
            return profiles;
        }

        /// <summary>
        /// Generate IProfiles from a set of aligned sequences
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        /// <param name="weights">sequence weights</param>
        public static IProfiles GenerateProfiles(ICollection<ISequence> sequences, float[] weights)
        {
            if (sequences.Count != weights.Length)
            {
                throw new ArgumentException("Invalid inputs");
            }

            MsaUtils.Normalize(weights);

            IEnumerator<ISequence> enumeratorSeq = sequences.GetEnumerator();
            enumeratorSeq.MoveNext();
            int sequenceLength = (int)enumeratorSeq.Current.Count;
            IAlphabet alphabet = enumeratorSeq.Current.Alphabet;
            while (enumeratorSeq.MoveNext())
            {
                if (enumeratorSeq.Current.Count != sequenceLength)
                {
                    throw new ArgumentException("Input sequences are not aligned");
                }
                if (enumeratorSeq.Current.Alphabet != alphabet)
                {
                    throw new ArgumentException("Input sequences use different alphabets");
                }
            }

            // each row is a column; each column is a profile
            int colSize = (ItemSet.Count + 1) / 2;
            IProfiles profiles = new Profiles(sequenceLength, colSize);

            for (int i = 0; i < sequenceLength; ++i)
            {
                enumeratorSeq.Reset();
                while (enumeratorSeq.MoveNext())
                {
                    if (!enumeratorSeq.Current.Alphabet.CheckIsAmbiguous(enumeratorSeq.Current[i])) // IsAmbiguous
                    {
                        for (int b = 0; b < AmbiguousCharactersMap[enumeratorSeq.Current[i]].Count; ++b)
                        {
                            profiles[i][ItemSet[AmbiguousCharactersMap[enumeratorSeq.Current[i]][b]]] += weights[i];
                        }
                    }
                    else
                    {
                        profiles[i][ItemSet[enumeratorSeq.Current[i]]] += weights[i];
                    }
                }
                MsaUtils.Normalize(profiles[i]);
            }
            profiles.ColumnSize = colSize;
            profiles.RowSize = sequenceLength;
            return profiles;
        }

        /// <summary>
        /// Generate IProfiles from a subset of aligned sequences.
        /// In the subset of sequences, those columns containing no residues, 
        /// i.e. indels only, are discarded.
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        /// <param name="sequenceIndices">the subset indices of the aligned sequences</param>
        /// <param name="allIndelPositions">the list of all-indel positions that have been removed when constructing</param>
        public static IProfiles GenerateProfiles(IList<ISequence> sequences, IList<int> sequenceIndices, out List<int> allIndelPositions)
        {
            IProfiles profiles;
            if (sequences.Count <= 0)
            {
                throw new ArgumentException("Empty input sequences");
            }
            if (sequenceIndices.Count > sequences.Count)
            {
                throw new ArgumentException("Invalid subset indices");
            }

            try
            {
                int sequenceLength = (int)sequences[sequenceIndices[0]].Count;
                IAlphabet alphabet = sequences[sequenceIndices[0]].Alphabet;

                foreach (int i in sequenceIndices)
                {
                    if (sequences[i].Count != sequenceLength)
                    {
                        throw new ArgumentException("Input sequences are not aligned");
                    }
                    if (sequences[i].Alphabet != alphabet)
                    {
                        throw new ArgumentException("Input sequences use different alphabets");
                    }
                }

                allIndelPositions = new List<int>();

                profiles = new Profiles();
                int colSize = (ItemSet.Count + 1) / 2;

                // Discard all indels columns.
                for (int col = 0; col < sequenceLength; ++col)
                {
                    float[] vector = new float[colSize];
                    bool isAllIndels = true;
                    foreach (int i in sequenceIndices)
                    {
                        if (!sequences[i].Alphabet.CheckIsGap(sequences[i][col]))
                        {
                            isAllIndels = false;
                        }
                        if (sequences[i].Alphabet.CheckIsAmbiguous(sequences[i][col]))
                        {
                            for (int b = 0; b < AmbiguousCharactersMap[sequences[i][col]].Count; ++b)
                            {
                                ++(vector[ItemSet[AmbiguousCharactersMap[sequences[i][col]][b]]]);
                            }
                        }
                        else
                        {
                            ++(vector[ItemSet[sequences[i][col]]]);
                        }
                    }
                    if (!isAllIndels)
                    {
                        MsaUtils.Normalize(vector);
                        profiles.ProfilesMatrix.Add(vector);
                    }
                    else
                    {
                        allIndelPositions.Add(col);
                    }
                }
                profiles.ColumnSize = colSize;
                profiles.RowSize = profiles.ProfilesMatrix.Count;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception("Invalid index", ex.InnerException);
            }
            return profiles;
        }



        /// <summary>
        /// Generate IProfiles from a subset of aligned sequences.
        /// In the subset of sequences, those columns containing no residues, 
        /// i.e. indels only, are discarded.
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        /// <param name="sequenceIndices">the subset indices of the aligned sequences</param>
        /// <param name="allIndelPositions">the list of all-indel positions that have been removed when constructing</param>
        /// <param name="weights">sequence weights</param>
        public static IProfiles GenerateProfiles(List<ISequence> sequences, List<int> sequenceIndices, out List<int> allIndelPositions, float[] weights)
        {
            IProfiles profiles;
            if (sequences.Count <= 0)
            {
                throw new ArgumentException("Empty input sequences");
            }
            if (sequenceIndices.Count > sequences.Count)
            {
                throw new ArgumentException("Invalid subset indices");
            }

            MsaUtils.Normalize(weights);

            try
            {
                int sequenceLength = (int)sequences[sequenceIndices[0]].Count;
                IAlphabet alphabet = sequences[sequenceIndices[0]].Alphabet;

                foreach (int i in sequenceIndices)
                {
                    if (sequences[i].Count != sequenceLength)
                    {
                        throw new ArgumentException("Input sequences are not aligned");
                    }
                    if (sequences[i].Alphabet != alphabet)
                    {
                        throw new ArgumentException("Input sequences use different alphabets");
                    }
                }

                allIndelPositions = new List<int>();

                profiles = new Profiles();
                int colSize = (ItemSet.Count + 1) / 2;

                // Discard all indels columns.
                for (int col = 0; col < sequenceLength; ++col)
                {
                    float[] vector = new float[colSize];
                    bool isAllIndels = true;
                    foreach (int i in sequenceIndices)
                    {
                        if (!sequences[i].Alphabet.CheckIsGap(sequences[i][col]))
                        {
                            isAllIndels = false;
                        }
                        if (sequences[i].Alphabet.CheckIsAmbiguous(sequences[i][col]))
                        {
                            //Console.WriteLine("residue {0} is {1}, ambiguous? {2}", i, seq[i].Symbol, seq[i].IsAmbiguous);
                            for (int b = 0; b < AmbiguousCharactersMap[sequences[i][col]].Count; ++b)
                            {
                                vector[ItemSet[AmbiguousCharactersMap[sequences[i][col]][b]]] += weights[i];
                            }
                        }
                        else
                        {
                            vector[ItemSet[sequences[i][col]]] += weights[i];
                        }
                    }
                    if (!isAllIndels)
                    {
                        MsaUtils.Normalize(vector);
                        profiles.ProfilesMatrix.Add(vector);
                    }
                    else
                    {
                        allIndelPositions.Add(col);
                    }
                }
                profiles.ColumnSize = colSize;
                profiles.RowSize = profiles.ProfilesMatrix.Count;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception("Invalid index", ex.InnerException);
            }
            return profiles;
        }

        /// <summary>
        /// Combine two profiles into one.
        /// The frequencies in the two profiles are weighted by the number of sequences.
        /// The new frequencies are defined as:
        /// (frequencyA * numberOfSequenceA + frequencyB * numberOfSequenceB) / (numberOfSequenceA + numberOfSequenceB)
        /// </summary>
        /// <param name="profileA">first profile alignment</param>
        /// <param name="profileB">second profile alignment</param>
        /// <param name="numberOfSequencesA">the number of sequences in the first profile</param>
        /// <param name="numberOfSequencesB">the number of sequences in the second profile</param>
        public static IProfiles GenerateProfiles(IProfiles profileA, IProfiles profileB, int numberOfSequencesA, int numberOfSequencesB)
        {
            if (profileA.RowSize != profileB.RowSize
                || profileA.ColumnSize != profileB.ColumnSize)
            {
                throw new Exception("different profiles sizes");
            }

            IProfiles profiles;

            profiles = new Profiles(profileA);

            for (int i = 0; i < profiles.RowSize; ++i)
            {
                for (int j = 0; j < profiles.ColumnSize; ++j)
                {
                    profiles[i][j] = (profileA[i][j] * numberOfSequencesA + profileB[i][j] * numberOfSequencesB)
                                                / (numberOfSequencesA + numberOfSequencesB);
                }
            }
            return profiles;
        }


        /// <summary>
        /// Combine two profiles with alignment array results from dynamic programming algorithm.
        /// The dynamic programming algorithm returns two arrays containing the alignment operations
        /// on the two profiles. This method applies the operation information in the two arrays to 
        /// the two original profiles, and combine them into a new aligned profile.
        /// </summary>
        /// <param name="profileA">first profile</param>
        /// <param name="profileB">second profile</param>
        /// <param name="numberOfSequencesA">the number of sequences in the first profile</param>
        /// <param name="numberOfSequencesB">the number of sequences in the second profile</param>
        /// <param name="aAligned">aligned interger array generated by dynamic programming</param>
        /// <param name="bAligned">aligned interger array generated by dynamic programming</param>
        /// <param name="gapCode">the gap integer code defined in dynamic programming class</param>
        public static IProfiles GenerateProfiles(
                IProfiles profileA,
                IProfiles profileB,
                int numberOfSequencesA,
                int numberOfSequencesB,
                int[] aAligned,
                int[] bAligned,
                int gapCode)
        {
            if (aAligned.Length != bAligned.Length)
            {
                throw new ArgumentException("not aligned sequences");
            }
            IProfiles profiles = new Profiles(aAligned.Length, profileA.ColumnSize);

            // a profile with gap only
            float[] gapProfile = new float[profiles.ColumnSize];
            gapProfile[gapProfile.Length - 1] = 1;

            for (int i = 0; i < aAligned.Length; ++i)
            {
                if (aAligned[i] == gapCode && bAligned[i] == gapCode)
                {
                    throw new Exception("Both positions are gap between two sets of sequences");
                }
                if (aAligned[i] == gapCode)
                {
                    for (int j = 0; j < profiles.ColumnSize; ++j)
                    {
                        profiles[i][j] = ((gapProfile[j] * numberOfSequencesA) + (profileB[bAligned[i]][j] * numberOfSequencesB))
                                                / (numberOfSequencesA + numberOfSequencesB);
                    }
                }
                else if (bAligned[i] == gapCode)
                {
                    for (int j = 0; j < profiles.ColumnSize; ++j)
                    {
                        profiles[i][j] = ((gapProfile[j] * numberOfSequencesA) + (profileA[aAligned[i]][j] * numberOfSequencesB))
                                                / (numberOfSequencesA + numberOfSequencesB);
                    }
                }
                else
                {
                    for (int j = 0; j < profiles.ColumnSize; ++j)
                    {
                        profiles[i][j] = ((profileA[aAligned[i]][j] * numberOfSequencesA) + (profileB[bAligned[i]][j] * numberOfSequencesB))
                                                / (numberOfSequencesA + numberOfSequencesB);
                    }
                }
            }
            return profiles;
        }

        /// <summary>
        /// Combine two profiles with alignment array results from dynamic programming algorithm.
        /// The dynamic programming algorithm returns two arrays containing the alignment operations
        /// on the two profiles. This method applies the operation information in the two arrays to 
        /// the two original profiles, and combine them into a new aligned profile.
        /// </summary>
        /// <param name="profileA">first profile</param>
        /// <param name="profileB">second profile</param>
        /// <param name="numberOfSequencesA">the number of sequences in the first profile</param>
        /// <param name="numberOfSequencesB">the number of sequences in the second profile</param>
        /// <param name="aAligned">aligned interger array generated by dynamic programming</param>
        /// <param name="bAligned">aligned interger array generated by dynamic programming</param>
        /// <param name="gapCode">the gap integer code defined in dynamic programming class</param>
        /// <param name="weights">the weights of two profiles</param>
        public static IProfiles GenerateProfiles(
                IProfiles profileA,
                IProfiles profileB,
                int numberOfSequencesA,
                int numberOfSequencesB,
                int[] aAligned,
                int[] bAligned,
                int gapCode,
                float[] weights)
        {
            if (aAligned.Length != bAligned.Length)
            {
                throw new ArgumentException("not aligned sequences");
            }
            IProfiles profiles = new Profiles(aAligned.Length, profileA.ColumnSize);

            MsaUtils.Normalize(weights);

            // a profile with gap only
            float[] gapProfile = new float[profiles.ColumnSize];
            gapProfile[gapProfile.Length - 1] = 1;

            for (int i = 0; i < aAligned.Length; ++i)
            {
                if (aAligned[i] == gapCode && bAligned[i] == gapCode)
                {
                    throw new Exception("Both positions are gap between two sets of sequences");
                }
                if (aAligned[i] == gapCode)
                {
                    for (int j = 0; j < profiles.ColumnSize; ++j)
                    {
                        profiles[i][j] = ((gapProfile[j] * numberOfSequencesA * weights[0]) + (profileB[bAligned[i]][j] * numberOfSequencesB * weights[1]))
                                                / (numberOfSequencesA + numberOfSequencesB);
                    }
                }
                else if (bAligned[i] == gapCode)
                {
                    for (int j = 0; j < profiles.ColumnSize; ++j)
                    {
                        profiles[i][j] = ((gapProfile[j] * numberOfSequencesA * weights[0]) + (profileA[aAligned[i]][j] * numberOfSequencesB * weights[1]))
                                                / (numberOfSequencesA + numberOfSequencesB);
                    }
                }
                else
                {
                    for (int j = 0; j < profiles.ColumnSize; ++j)
                    {
                        profiles[i][j] = ((profileA[aAligned[i]][j] * numberOfSequencesA * weights[0]) + (profileB[bAligned[i]][j] * numberOfSequencesB * weights[1]))
                                                / (numberOfSequencesA + numberOfSequencesB);
                    }
                }
            }
            return profiles;
        }
        #endregion
    }
}
