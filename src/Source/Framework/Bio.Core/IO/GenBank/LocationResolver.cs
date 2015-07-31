using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// This is the default implementation of ILocationResolver.
    /// This class resolves the start and end positions of a location.
    /// 
    /// Please see the following table for how this class resolves the ambiguities in start and end data.
    /// 
    /// Start/End Data		Resolved Start		Resolved End
    /// 12.30               12      			30
    /// &gt;30	            30			        30
    /// &lt;30 	            30			        30
    /// 23^24		        23	                24
    /// 100^1		        1000    			1	
    /// </summary>
    public class LocationResolver : ILocationResolver
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocationResolver()
        {
            // No implementation.
        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// Returns the new LocationResolver instance that is a copy of this instance.
        /// </summary>
        public LocationResolver Clone()
        {
            return new LocationResolver();
        }
        #endregion Public Methods

        #region ILocationResolver Members

        /// <summary>
        /// Returns the start position by resolving the start-data present in the specified location.
        /// If unable to resolve start-data then an exception will occur.
        /// </summary>
        /// <param name="location">Location instance.</param>
        public int GetStart(ILocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            // If sub-locations are there, then get the minimum start position from the sub-locations.
            if (location.SubLocations.Count > 0)
            {
                return location.SubLocations.OrderBy(L => L.LocationStart).ToList()[0].LocationStart;
            }

            if (string.IsNullOrEmpty(location.StartData))
            {
                throw new ArgumentException(Properties.Resource.StartDataCannotBeNull);
            }

            return ResolveStart(location.StartData);
        }

        /// <summary>
        /// Returns the end position by resolving the end-data present in the specified location.
        /// If unable to resolve end-data then an exception will occur.
        /// </summary>
        /// <param name="location">Location instance.</param>
        public int GetEnd(ILocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            // If sub-locations are there, then get the max end position from the sub-locations.
            if (location.SubLocations.Count > 0)
            {
                return location.SubLocations.OrderByDescending(L => L.LocationEnd).ToList()[0].LocationEnd;
            }

            if (string.IsNullOrEmpty(location.EndData))
            {
                throw new InvalidOperationException(Properties.Resource.EndDataCannotBeNull);
            }

            return ResolveEnd(location.EndData);
        }

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequence as specified by the location.
        /// If the location of a feature and sequence in which the feature is present is 
        /// specified then this method returns a sequence which contains the bases of the specified feature.
        /// 
        /// Please note that,
        /// 1. If Accession of the location is not null or empty then an exception will occur.
        /// 2. If the location contains "order" operator then this method uses SegmentedSequence class to construct the sequence.
        ///    For example, order(100..200,300..450) will result in a SegmentedSequence which internally contains two sequences, 
        ///    first one created from 100 to 200 bases, and second one created from 300 to 450 bases.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="sequence">Sequence from which the sub sequence has to be returned.</param>
        public ISequence GetSubSequence(ILocation location, ISequence sequence)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            if (sequence == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameSequence);
            }

            return GetSubSequence(location, sequence, null);
        }

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequence as specified by the location.
        /// If the location contains accession then the sequence from the referredSequences which matches the 
        /// accession of the location will be considered.
        /// 
        /// For example,
        /// if location is "join(100..200, J00089.1:10..50, J00090.2:30..40)"
        /// then bases from 100 to 200 will be considered from the sequence parameter and referredSequences will
        /// be searched for the J00089.1 and J00090.2 accession if found then those sequences will be considered 
        /// for constructing the output sequence.
        /// If the referred sequence is not found in the referredSequences then an exception will occur.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="sequence">Sequence instance from which the sub sequence has to be returned.</param>
        /// <param name="referredSequences">A dictionary containing Accession numbers as keys and Sequences as values, this will be used when
        /// the location or sub-locations contains accession.</param>
        public ISequence GetSubSequence(ILocation location, ISequence sequence, Dictionary<string, ISequence> referredSequences)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            if (sequence == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameSequence);
            }

            DerivedSequence basicDerSeq;

            if (location.Operator == LocationOperator.Complement)
            {
                if (location.SubLocations.Count > 1)
                {
                    throw new ArgumentException(Properties.Resource.ComplementWithMorethanOneSubLocs);
                }

                if (location.SubLocations.Count > 0)
                {
                    basicDerSeq = new DerivedSequence(location.SubLocations[0].GetSubSequence(sequence, referredSequences), false, true);
                }
                else
                {
                    basicDerSeq = new DerivedSequence(GetSubSequence(location.LocationStart, location.LocationEnd, location.Accession, location.Separator, sequence, referredSequences), false, true);
                }

                byte[] tempSeqData = new byte[basicDerSeq.Count];
                for (int i = 0; i < basicDerSeq.Count; i++)
                {
                    tempSeqData[i] = basicDerSeq[i];
                }

                return new Sequence(sequence.Alphabet, tempSeqData);
            }

            if (location.Operator == LocationOperator.Order)
            {
                List<ISequence> subSequences = new List<ISequence>();
                if (location.SubLocations.Count > 0)
                {
                    foreach (ILocation loc in location.SubLocations)
                    {
                        subSequences.Add(loc.GetSubSequence(sequence, referredSequences));
                    }
                }
                else
                {
                    basicDerSeq = new DerivedSequence(GetSubSequence(location.LocationStart, location.LocationEnd, location.Accession, location.Separator, sequence, referredSequences), false, false);
                    byte[] seqData = new byte[basicDerSeq.Count];
                    for (long i = 0; i < basicDerSeq.Count; i++)
                    {
                        seqData[i] = basicDerSeq[i];
                    }

                    subSequences.Add(new Sequence(sequence.Alphabet, seqData));
                }

                long totalSubSequenceLength = 0;
                long j = 0;
                foreach (ISequence seq in subSequences)
                {
                    totalSubSequenceLength += seq.Count;
                }
                byte[] tempSeqData = new byte[totalSubSequenceLength];
                totalSubSequenceLength = 0;
                IAlphabet alphabet = null;
                int m = 0;
                foreach (ISequence seq in subSequences)
                {
                    totalSubSequenceLength += seq.Count;
                    while (j < totalSubSequenceLength)
                    {
                        tempSeqData[j] = seq[m];
                        j++;
                        m++;
                    }
                    m = 0;
                    alphabet = seq.Alphabet;
                    
                }

                //return Segmented sequence.
                return new Sequence(alphabet, tempSeqData);
            }

            if (location.Operator == LocationOperator.Join || location.Operator == LocationOperator.Bond)
            {
                if (location.SubLocations.Count > 0)
                {
                    List<ISequence> subSequences = new List<ISequence>();
                    foreach (ILocation loc in location.SubLocations)
                    {
                        subSequences.Add(loc.GetSubSequence(sequence, referredSequences));
                    }


                    long i = 0;
                    long subSeqLength = 0;
                    foreach (ISequence subSeq in subSequences)
                    {
                        subSeqLength += subSeq.Count;
                    }
                    byte[] seqData = new byte[subSeqLength];
                    subSeqLength = 0;
                    int m = 0;
                    foreach (ISequence subSeq in subSequences)
                    {
                        subSeqLength += subSeq.Count;
                        while (i < subSeqLength)
                        {
                            seqData[i] = subSeq[m];
                            i++; 
                            m++;
                        }
                        m = 0;
                    }

                    Sequence seq = new Sequence(sequence.Alphabet,seqData);

                    return seq;
                }
                else
                {
                    return GetSubSequence(location.LocationStart, location.LocationEnd, location.Accession, location.Separator, sequence, referredSequences);
                }
            }

            if (location.SubLocations.Count > 0)
            {
                throw new ArgumentException(Properties.Resource.NoneWithSubLocs);
            }

            return GetSubSequence(location.LocationStart, location.LocationEnd, location.Accession, location.Separator, sequence, referredSequences);
        }

        /// <summary>
        /// Return true if the specified position is within the start position.
        /// For example,
        /// if the start-data of a location is "23.40", this method will 
        /// return true for the position values ranging from 23 to 40.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="position">Position to be verified.</param>
        /// <returns>Returns true if the specified position is with in the start position else returns false.</returns>
        public bool IsInStart(ILocation location, int position)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            List<ILocation> leafLocations = location.GetLeafLocations();
            foreach (ILocation loc in leafLocations)
            {
                int minStart = ResolveStart(loc.StartData);
                int maxStart = ResolveEnd(loc.StartData);
                if (position >= minStart && position <= maxStart)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return true if the specified position is within the end position.
        /// For example,
        /// if the end-data of a location is "23.40", this method will 
        /// return true for the position values ranging from 23 to 40.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="position">Position to be verified.</param>
        /// <returns>Returns true if the specified P\position is with in the end position else returns false.</returns>
        public bool IsInEnd(ILocation location, int position)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            List<ILocation> leafLocations = location.GetLeafLocations();
            foreach (ILocation loc in leafLocations)
            {
                int maxStart = ResolveEnd(loc.EndData);
                int minStart = ResolveStart(loc.EndData);

                if (position >= minStart && position <= maxStart)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the specified position is with in the start and end positions.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="position">Position to be verified.</param>
        /// <returns>Returns true if the specified position is with in the start and end positions else returns false.</returns>
        public bool IsInRange(ILocation location, int position)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            List<ILocation> leafLocations = location.GetLeafLocations();
            foreach (ILocation loc in leafLocations)
            {
                if (position >= loc.LocationStart && position <= loc.LocationEnd)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new ILocationResolver that is a copy of the current ILocationResolver.
        /// </summary>
        /// <returns>A new ILocationResolver that is a copy of this ILocationResolver.</returns>
        ILocationResolver ILocationResolver.Clone()
        {
            return Clone();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Resolves and returns the start position.
        /// </summary>
        /// <param name="str">Start data.</param>
        private static int ResolveStart(string str)
        {
            int value;
            if (int.TryParse(str, out value))
            {
                return value;
            }
            else
            {
                if (str.StartsWith(">", StringComparison.OrdinalIgnoreCase))
                {
                    int firstIndex = str.IndexOf(">", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf(">", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(str.Substring(1));
                }
                else if (str.StartsWith("<", StringComparison.OrdinalIgnoreCase))
                {
                    int firstIndex = str.IndexOf("<", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf("<", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(str.Substring(1));
                }
                else if (str.Contains("^"))
                {
                    int firstIndex = str.IndexOf("^", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf("^", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                        throw new FormatException(msgStr);
                    }

                    string[] values = str.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length != 2)
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(values[0]);
                }
                else if (str.Contains("."))
                {
                    int firstIndex = str.IndexOf(".", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf(".", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                        throw new FormatException(msgStr);
                    }

                    string[] values = str.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length != 2)
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(values[0]);
                }
                else
                {
                    string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidStartData, str);
                    throw new FormatException(msgStr);
                }
            }
        }

        /// <summary>
        /// Resolves and returns the end position.
        /// </summary>
        /// <param name="str">End data.</param>
        private static int ResolveEnd(string str)
        {
            int value;
            if (int.TryParse(str, out value))
            {
                return value;
            }
            else
            {
                if (str.StartsWith(">", StringComparison.OrdinalIgnoreCase))
                {
                    int firstIndex = str.IndexOf(">", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf(">", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(str.Substring(1));
                }
                else if (str.StartsWith("<", StringComparison.OrdinalIgnoreCase))
                {
                    int firstIndex = str.IndexOf("<", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf("<", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(str.Substring(1));
                }
                else if (str.Contains("^"))
                {
                    int firstIndex = str.IndexOf("^", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf("^", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                        throw new FormatException(msgStr);
                    }

                    string[] values = str.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 2)
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(values[values.Length - 1]);
                }
                else if (str.Contains("."))
                {
                    int firstIndex = str.IndexOf(".", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != str.LastIndexOf(".", StringComparison.OrdinalIgnoreCase))
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                        throw new FormatException(msgStr);
                    }

                    string[] values = str.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 2)
                    {
                        string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                        throw new FormatException(msgStr);
                    }

                    return ResolveStart(values[values.Length - 1]);
                }
                else
                {
                    string msgStr = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidEndData, str);
                    throw new FormatException(msgStr);
                }
            }
        }

        /// <summary>
        /// Returns the sequence for the specified start and end positions.
        /// If the accession is null or empty then the source sequence is used to construct the output sequence,
        /// otherwise appropriate sequence from the referred sequence is used to construct output sequence.
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="end">End position.</param>
        /// <param name="accession">Accession number.</param>
        /// <param name="sepataror">Start and End separator.</param>
        /// <param name="source">Source sequence.</param>
        /// <param name="referredSequences">Referred Sequences.</param>
        private static ISequence GetSubSequence(int start, int end, string accession, string sepataror, ISequence source, Dictionary<string, ISequence> referredSequences)
        {
            if (string.Compare(sepataror, "^", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new Sequence(source.Alphabet, string.Empty);
            }

            if (string.Compare(sepataror, "..", StringComparison.OrdinalIgnoreCase) != 0 &&
                string.Compare(sepataror, ".", StringComparison.OrdinalIgnoreCase) != 0 &&
                !string.IsNullOrEmpty(sepataror))
            {
                string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidSeparator, sepataror);
                throw new ArgumentException(str);
            }

            if (!string.IsNullOrEmpty(accession) && (referredSequences == null || !referredSequences.ContainsKey(accession)))
            {
                string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.AccessionSequenceNotFound, accession);
                throw new ArgumentException(str);
            }

            if (!string.IsNullOrEmpty(accession))
            {
                if (source.Alphabet != referredSequences[accession].Alphabet)
                {
                    string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidReferredAlphabet, accession);
                    throw new ArgumentException(str);
                }

                source = referredSequences[accession];
            }

            // as location.start is one based where as Range accepts zero based index.
            start = start - 1;
            int length = end - start;

            if (string.IsNullOrEmpty(sepataror) || string.Compare(sepataror, ".", StringComparison.OrdinalIgnoreCase) == 0)
            {
                length = 1;
            }

            ISequence newSequence = source.GetSubSequence(start, length);
            byte[] seqData = new byte[newSequence.Count];
            for (long i = 0; i < newSequence.Count; i++)
            {
                seqData[i] = newSequence[i];
            }

            return new Sequence(source.Alphabet, seqData);
        }
        #endregion Private Methods
    }
}
