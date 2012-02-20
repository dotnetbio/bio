using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Bio.Algorithms.Alignment;
using Bio.Util;

namespace Bio.IO.SAM
{
    /// <summary>
    /// Class to hold sequence alignment map (SAM) structure.
    /// </summary>
    [Serializable]
    public class SequenceAlignmentMap : ISequenceAlignment
    {
        /// <summary>
        /// Holds SAM header.
        /// </summary>
        private SAMAlignmentHeader header;

        /// <summary>
        /// holds the metadta.
        /// </summary>
        private Dictionary<string, object> metadata;

        /// <summary>
        /// Holds list of query sequences present in this SAM object.
        /// </summary>
        private IList<SAMAlignedSequence> querySequences;

        /// <summary>
        /// Default constructor.
        /// Creates SequenceAlignmentMap instance.
        /// </summary>
        public SequenceAlignmentMap() : this(new SAMAlignmentHeader()) { }

        /// <summary>
        /// Creates SequenceAlignmentMap instance.
        /// </summary>
        /// <param name="header">SAM header.</param>
        public SequenceAlignmentMap(SAMAlignmentHeader header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            this.header = header;
            metadata = new Dictionary<string, object>();
            metadata.Add(Helper.SAMAlignmentHeaderKey, header);
            querySequences = new List<SAMAlignedSequence>();
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming context.</param>
        protected SequenceAlignmentMap(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            header = (SAMAlignmentHeader)info.GetValue("header", typeof(SAMAlignmentHeader));
            metadata = new Dictionary<string, object>();
            metadata.Add(Helper.SAMAlignmentHeaderKey, header);
            querySequences = (IList<SAMAlignedSequence>)info.GetValue("sequences", typeof(IList<SAMAlignedSequence>));

            if (querySequences == null)
            {
                querySequences = new List<SAMAlignedSequence>();
            }
        }

        /// <summary>
        /// Gets the SAM header.
        /// </summary>
        public SAMAlignmentHeader Header
        {
            get
            {
                return header;
            }
        }

        /// <summary>
        /// Gets the query sequences present in this alignment.
        /// </summary>
        public IList<SAMAlignedSequence> QuerySequences
        {
            get
            {
                return querySequences;
            }
        }

        /// <summary>
        /// Gets list of aligned sequences present in this alignment.
        /// </summary>
        public IList<IAlignedSequence> AlignedSequences
        {
            get
            {
                ReadOnlyAlignedSequenceCollection collection = new ReadOnlyAlignedSequenceCollection(querySequences);

                return collection;
            }
        }

        /// <summary>
        /// Gets list of source sequences present in this alignment.
        /// Note that this method always returns an empty readonly list.
        /// </summary>
        public IList<ISequence> Sequences
        {
            get { return new List<ISequence>().AsReadOnly(); }
        }

        /// <summary>
        /// Gets the metadata of this alignment.
        /// </summary>
        public Dictionary<string, object> Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// Gets documentation object.
        /// </summary>
        public object Documentation
        {
            get;
            set;
        }

        /// <summary>
        /// Returns list of reference sequences present in this header. 
        /// </summary>
        public IList<string> GetRefSequences()
        {
            return header.ReferenceSequences.Select(N => N.Name).ToList();
        }

        /// <summary>
        /// Returns list of SequenceRanges objects which represents reference sequences present in the header. 
        /// </summary>
        public IList<SequenceRange> GetReferenceSequenceRanges()
        {
            return header.GetReferenceSequenceRanges();
        }

        /// <summary>
        /// Gets the paired reads.
        /// </summary>
        /// <returns>List of paired read.</returns>
        public IList<PairedRead> GetPairedReads()
        {
             return GetInMemoryPairedReads(0, 0, true);
         }

        /// <summary>
        /// Gets the paired reads.
        /// </summary>
        /// <param name="libraryName">Name of the library present in CloneLibrary.</param>
        /// <returns>List of paired read.</returns>
        public IList<PairedRead> GetPairedReads(string libraryName)
        {
            if (string.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentNullException("libraryName");
            }

            CloneLibraryInformation libraryInfo = CloneLibrary.Instance.GetLibraryInformation(libraryName);

            if (libraryInfo == null)
            {
                throw new ArgumentOutOfRangeException("libraryName");
            }

            return GetPairedReads(libraryInfo);
        }

        /// <summary>
        /// Gets the paired reads.
        /// </summary>
        /// <param name="libraryInfo">Library information.</param>
        /// <returns>List of paired read.</returns>
        public IList<PairedRead> GetPairedReads(CloneLibraryInformation libraryInfo)
        {
            if (libraryInfo == null)
            {
                throw new ArgumentNullException("libraryInfo");
            }

            return GetPairedReads(libraryInfo.MeanLengthOfInsert, libraryInfo.StandardDeviationOfInsert);
        }

        /// <summary>
        /// Gets the paired reads.
        /// </summary>
        /// <param name="meanLengthOfInsert">Mean of the insert length.</param>
        /// <param name="standardDeviationOfInsert">Standard deviation of insert length.</param>
        /// <returns>List of paired read.</returns>
        public IList<PairedRead> GetPairedReads(float meanLengthOfInsert, float standardDeviationOfInsert)
        {
            return GetInMemoryPairedReads(meanLengthOfInsert, standardDeviationOfInsert);
        }

        /// <summary>
        /// Method for serializing the SAM object.
        /// </summary>
        /// <param name="info">Serialization Info.</param>
        /// <param name="context">Streaming context.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("header", header);
            info.AddValue("sequences", querySequences);
        }

        /// <summary>
        /// This Method calculates mean and standard deviation from the available reads
        /// and then using this information updates the type of reads.
        /// </summary>
        /// <param name="allreads">All reads.</param>
        /// <param name="sum">Pre calculated sum of insert length of reads 
        /// (invavled in calculation mean and std deviation) if available, else pass 0.</param>
        /// <param name="count">Pre calculated count of reads (invavled in calculation mean and std deviation)
        /// if available, else pass 0.</param>
        private static void UpdateType(IList<PairedRead> allreads, double sum, int count)
        {
            // Calculate the Mean of insert lengths. 
            // Note: In case MultipleHits, Orphan, Chimera, StructuralAnomaly we can't calculate insert length.
            IEnumerable<PairedRead> reads = allreads.Where(R => R.PairedType == PairedReadType.Normal || R.PairedType == PairedReadType.LengthAnomaly);
            if (reads.Count() == 0)
            {
                return;
            }

            if (sum == 0 || count == 0)
            {
                sum = reads.Sum(PE => PE.InsertLength);
                count = reads.Count();
            }

            float mean = (float)(sum / count);
            sum = 0;
            foreach (PairedRead pairedRead in reads)
            {
                sum += Math.Pow((pairedRead.InsertLength - mean), 2);
            }

            float stddeviation = (float)Math.Sqrt(sum / count);
            // µ + 3σ
            float upperLimit = mean + (3 * stddeviation);
            // µ - 3σ
            float lowerLimit = mean - (3 * stddeviation);
            foreach (PairedRead pairedRead in reads)
            {

                if (pairedRead.InsertLength > upperLimit || pairedRead.InsertLength < lowerLimit)
                {
                    pairedRead.PairedType = PairedReadType.LengthAnomaly;
                }
                else
                {
                    pairedRead.PairedType = PairedReadType.Normal;
                }
            }
        }

        /// <summary>
        /// Gets the paired reads when SAMAligned sequences are in memory.
        /// </summary>
        /// <param name="meanLengthOfInsert">Mean of the insert length.</param>
        /// <param name="standardDeviationOfInsert">Standard deviation of insert length.</param>
        /// <param name="calculate">If this flag is set then mean and standard deviation will
        /// be calculated from the paired reads instead of specified.</param>
        /// <returns>List of paired read.</returns>
        private IList<PairedRead> GetInMemoryPairedReads(float meanLengthOfInsert, float standardDeviationOfInsert, bool calculate = false)
        {
            // Dictionary helps to get the information at one pass of alinged sequence list.
            Dictionary<string, PairedRead> pairedReads = new Dictionary<string, PairedRead>();
            double sum = 0;
            int count = 0;

            for (int i = 0; i < QuerySequences.Count; i++)
            {
                PairedRead pairedRead;
                SAMAlignedSequence read = QuerySequences[i];
                if ((read.Flag & SAMFlags.PairedRead) == SAMFlags.PairedRead)
                {
                    if (pairedReads.TryGetValue(read.QName, out pairedRead))
                    {
                        if (pairedRead.Read2 == null || pairedRead.Read1 == null)
                        {
                            if (pairedRead.Read2 == null)
                            {
                                pairedRead.Read2 = read;
                            }
                            else
                            {
                                pairedRead.Read1 = read;
                            }

                            pairedRead.PairedType = PairedRead.GetPairedReadType(pairedRead.Read1, pairedRead.Read2, meanLengthOfInsert, standardDeviationOfInsert);
                            if (pairedRead.PairedType == PairedReadType.Normal || pairedRead.PairedType == PairedReadType.LengthAnomaly)
                            {
                                pairedRead.InsertLength = PairedRead.GetInsertLength(pairedRead.Read1, pairedRead.Read2);
                                if (calculate)
                                {
                                    sum += pairedRead.InsertLength;
                                    count++;
                                }
                            }
                        }
                        else
                        {
                            pairedRead.InsertLength = 0;
                            if (calculate)
                            {
                                sum -= pairedRead.InsertLength;
                                count--;
                            }

                            pairedRead.Reads.Add(read);
                            pairedRead.PairedType = PairedReadType.MultipleHits;
                        }
                    }
                    else
                    {
                        pairedRead = new PairedRead();
                        if (!string.IsNullOrEmpty(read.RName) && !read.RName.Equals("*"))
                        {
                            pairedRead.Read1 = read;
                        }
                        else
                        {
                            pairedRead.Read2 = read;
                        }

                        pairedRead.PairedType = PairedReadType.Orphan;
                        pairedRead.InsertLength = 0;
                        pairedReads.Add(read.QName, pairedRead);
                    }
                }
            }

            List<PairedRead> allreads = pairedReads.Values.ToList();
            pairedReads = null;
            if (calculate && count > 0)
            {
                UpdateType(allreads, sum, count);
            }

            return allreads;
        }
    }
}
