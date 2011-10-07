using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bio.IO.SAM
{
    /// <summary>
    /// Class to hold Paired reads.
    /// </summary>
    public class PairedRead
    {
        #region Fields
        /// <summary>
        /// holds reads.
        /// </summary>
        private List<SAMAlignedSequence> _alignedSequences;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the list of paired reads.
        /// </summary>
        public virtual IList<SAMAlignedSequence> Reads
        {
            get
            {
                if (_alignedSequences == null)
                {
                    _alignedSequences = new List<SAMAlignedSequence>();
                }

                return _alignedSequences;
            }
        }

        /// <summary>
        /// First aligned sequence or read.
        /// </summary>
        public virtual SAMAlignedSequence Read1
        {
            get
            {
                if (_alignedSequences == null || _alignedSequences.Count == 0)
                {
                    return null;
                }

                return _alignedSequences[0];
            }
            set
            {
                if (_alignedSequences != null && _alignedSequences.Count > 0)
                {
                    _alignedSequences[0] = value;
                }
                else
                {
                    if (value != null)
                    {
                        Reads.Add(value);
                    }
                }
            }
        }

        /// <summary>
        /// Second aligned sequence or read.
        /// </summary>
        public virtual SAMAlignedSequence Read2
        {
            get
            {
                if (_alignedSequences == null || _alignedSequences.Count <= 1)
                {
                    return null;
                }

                return _alignedSequences[1];
            }
            set
            {
                if (_alignedSequences != null && _alignedSequences.Count > 1)
                {
                    _alignedSequences[1] = value;
                }
                else
                {
                    if (value != null)
                    {
                        int count = Reads.Count;
                        if (count == 0)
                        {
                            Reads.Add(null);
                        }

                        Reads.Add(value);
                    }
                }
            }
        }

        /// <summary>
        /// Paired type <see cref="PairedReadType"/>
        /// </summary>
        public virtual PairedReadType PairedType { get; set; }

        /// <summary>
        /// Gets or sets the insert length.
        /// </summary>
        public virtual int InsertLength { get; set; }
        #endregion Properties

        #region Public Static Methods
        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="pairedRead">Paired read.</param>
        /// <param name="libraryName">library name.</param>
        public static PairedReadType GetPairedReadType(PairedRead pairedRead, string libraryName)
        {
            return GetPairedReadType(pairedRead, libraryName, false);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="pairedRead">Paired read.</param>
        /// <param name="libraryName">library name.</param>
        /// <param name="useInsertLengthOfReads">
        /// If this flag is set to true then insert length will be calculated from read1 and read2,
        /// else InsertLength in spcified paired read will be used.
        /// </param>
        public static PairedReadType GetPairedReadType(PairedRead pairedRead, string libraryName, bool useInsertLengthOfReads)
        {
            if (pairedRead == null)
            {
                throw new ArgumentNullException("pairedRead");
            }

            if (string.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentNullException("libraryName");
            }

            CloneLibraryInformation libraryInfo = CloneLibrary.Instance.GetLibraryInformation(libraryName);

            if (libraryInfo == null)
            {
                throw new ArgumentOutOfRangeException("libraryName");
            }

            return GetPairedReadType(pairedRead, libraryInfo, useInsertLengthOfReads);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="pairedRead">Paired read.</param>
        /// <param name="libraryInfo">Library information.</param>
        public static PairedReadType GetPairedReadType(PairedRead pairedRead, CloneLibraryInformation libraryInfo)
        {
            return GetPairedReadType(pairedRead, libraryInfo, false);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="pairedRead">Paired read.</param>
        /// <param name="libraryInfo">Library information.</param>
        /// <param name="useInsertLengthOfReads">
        /// If this flag is set to true then insert length will be calculated from read1 and read2,
        /// else InsertLength in spcified paired read will be used.
        /// </param>
        public static PairedReadType GetPairedReadType(PairedRead pairedRead, CloneLibraryInformation libraryInfo, bool useInsertLengthOfReads)
        {
            if (pairedRead == null)
            {
                throw new ArgumentNullException("pairedRead");
            }

            if (libraryInfo == null)
            {
                throw new ArgumentNullException("libraryInfo");
            }

            return GetPairedReadType(pairedRead, libraryInfo.MeanLengthOfInsert, libraryInfo.StandardDeviationOfInsert, useInsertLengthOfReads);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="pairedRead">Paired read.</param>
        /// <param name="meanLengthOfInsert">Mean of the insertion length.</param>
        /// <param name="standardDeviationOfInsert">Standard deviation of insertion length.</param>
        public static PairedReadType GetPairedReadType(PairedRead pairedRead, float meanLengthOfInsert, float standardDeviationOfInsert)
        {
            return GetPairedReadType(pairedRead, meanLengthOfInsert, standardDeviationOfInsert, false);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="pairedRead">Paired read.</param>
        /// <param name="meanLengthOfInsert">Mean of the insertion length.</param>
        /// <param name="standardDeviationOfInsert">Standard deviation of insertion length.</param>
        /// <param name="useInsertLengthOfReads">
        /// If this flag is set to true then insert length will be calculated from read1 and read2,
        /// else InsertLength in spcified paired read will be used.
        /// By default this will be set to false.
        /// </param>
        public static PairedReadType GetPairedReadType(PairedRead pairedRead, float meanLengthOfInsert, float standardDeviationOfInsert, bool useInsertLengthOfReads)
        {
            if (pairedRead == null)
            {
                throw new ArgumentNullException("pairedRead");
            }

            if (pairedRead._alignedSequences.Count > 2)
                return PairedReadType.MultipleHits;

            if (useInsertLengthOfReads)
            {
                return GetPairedReadType(pairedRead.Read1, pairedRead.Read2, meanLengthOfInsert, standardDeviationOfInsert);
            }
            else
            {
                PairedReadType type = GetPairedReadType(pairedRead.Read1, pairedRead.Read2, meanLengthOfInsert, standardDeviationOfInsert);

                if (type == PairedReadType.Normal || type == PairedReadType.LengthAnomaly)
                {
                    int insertLength = pairedRead.InsertLength;
                    // µ + 3σ
                    float upperLimit = meanLengthOfInsert + (3*standardDeviationOfInsert);
                    // µ - 3σ
                    float lowerLimit = meanLengthOfInsert - (3*standardDeviationOfInsert);
                    if (insertLength > upperLimit || insertLength < lowerLimit)
                    {
                        type = PairedReadType.LengthAnomaly;
                    }
                    else
                    {
                        type = PairedReadType.Normal;
                    }
                }

                return type;
            }
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="read1">First aligned sequence.</param>
        /// <param name="read2">Second aligned sequence.</param>
        /// <param name="libraryName">library name.</param>
        public static PairedReadType GetPairedReadType(SAMAlignedSequence read1, SAMAlignedSequence read2, string libraryName)
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

            return GetPairedReadType(read1, read2, libraryInfo);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="read1">First aligned sequence.</param>
        /// <param name="read2">Second aligned sequence.</param>
        /// <param name="libraryInfo">Library information.</param>
        public static PairedReadType GetPairedReadType(SAMAlignedSequence read1, SAMAlignedSequence read2, CloneLibraryInformation libraryInfo)
        {
            if (libraryInfo == null)
            {
                throw new ArgumentNullException("libraryInfo");
            }

            return GetPairedReadType(read1, read2, libraryInfo.MeanLengthOfInsert, libraryInfo.StandardDeviationOfInsert);
        }

        /// <summary>
        /// Gets the paired reads type.
        /// </summary>
        /// <param name="read1">First aligned sequence.</param>
        /// <param name="read2">Second aligned sequence.</param>
        /// <param name="meanLengthOfInsert">Mean of the insertion length.</param>
        /// <param name="standardDeviationOfInsert">Standard deviation of insertion length.</param>
        public static PairedReadType GetPairedReadType(SAMAlignedSequence read1, SAMAlignedSequence read2, float meanLengthOfInsert, float standardDeviationOfInsert)
        {
            PairedReadType type = PairedReadType.Normal;
            if (read1 == null)
            {
                throw new ArgumentNullException("read1");
            }

            if (read2 == null)
            {
                return PairedReadType.Orphan;
            }

            if (string.IsNullOrEmpty(read2.RName)
                || read2.RName.Equals("*")
                || ((read2.Flag & SAMFlags.UnmappedQuery) == SAMFlags.UnmappedQuery))
            {
                type = PairedReadType.Orphan;
            }
            else if (!read2.RName.Equals(read1.RName))
            {
                type = PairedReadType.Chimera;
            }
            else
            {
                bool isBothforwardReads = IsForwardRead(read1) && IsForwardRead(read2);
                bool isBothReverseReads = IsReverseRead(read1) && IsReverseRead(read2);

                if (isBothforwardReads || isBothReverseReads)
                {
                    type = PairedReadType.StructuralAnomaly;
                }
                else
                {
                    int forwardReadStartPos = 0;
                    int reverseReadStartPos = 0;

                    if (IsForwardRead(read1))
                    {
                        forwardReadStartPos = read1.Pos;
                        reverseReadStartPos = read2.Pos;
                    }
                    else
                    {
                        forwardReadStartPos = read2.Pos;
                        reverseReadStartPos = read1.Pos;
                    }

                    if (forwardReadStartPos > reverseReadStartPos)
                    {
                        type = PairedReadType.StructuralAnomaly;
                    }
                    else
                    {

                        int insertLength = GetInsertLength(read1, read2);

                        // µ + 3σ
                        float upperLimit = meanLengthOfInsert + (3*standardDeviationOfInsert);
                        // µ - 3σ
                        float lowerLimit = meanLengthOfInsert - (3*standardDeviationOfInsert);
                        if (insertLength > upperLimit || insertLength < lowerLimit)
                        {
                            type = PairedReadType.LengthAnomaly;
                        }
                    }
                }
            }

            return type;
        }

        /// <summary>
        /// Gets the insert length of reads.
        /// </summary>
        /// <param name="read1">First read.</param>
        /// <param name="read2">Second read.</param>
        public static int GetInsertLength(SAMAlignedSequence read1, SAMAlignedSequence read2)
        {
            return GetInsertLength(read1, read2, false);
        }

        /// <summary>
        /// Gets the insert length of reads.
        /// </summary>
        /// <param name="read1">First read.</param>
        /// <param name="read2">Second read.</param>
        /// <param name="validate">Validates the reads before calculating the insert length.</param>
        public static int GetInsertLength(SAMAlignedSequence read1, SAMAlignedSequence read2, bool validate)
        {
            //                      reference chromosome
            //5'                         -->                      3'
            //----------------------------------------------------- F strand
            // 
            //3'                         <--                       5'
            //----------------------------------------------------- R strand
            //        read1                         read2
            //    5'             3'             3'            5'
            //         -->                          <--   
            //    |--------------               --------------|
            //    |<----------insert length------------------>|

            if (read1 == null)
            {
                throw new ArgumentNullException("read1");
            }

            if (read2 == null)
            {
                return 0;
            }


            if (validate)
            {
                PairedReadType type = GetPairedReadType(read1, read2, 0, 0);
                if (type != PairedReadType.Normal && type != PairedReadType.LengthAnomaly)
                {
                    return 0;
                }
            }

            if (read1.ISize == -read2.ISize)
            {
                return read1.ISize >= 0 ? read1.ISize : -read1.ISize;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a boolean value which indicates that whether the specified read is forward oriented or not.
        /// </summary>
        /// <param name="read">Aligned Sequence.</param>
        public static bool IsForwardRead(SAMAlignedSequence read)
        {
            if (read == null)
            {
                throw new ArgumentNullException("read");
            }

            return (read.Flag & SAMFlags.QueryOnReverseStrand) == 0;
        }

        /// <summary>
        /// Gets a boolean value which indicates that whether the specified read is reverse oriented or not.
        /// </summary>
        /// <param name="read">Aligned Sequence.</param>
        public static bool IsReverseRead(SAMAlignedSequence read)
        {
            if (read == null)
            {
                throw new ArgumentNullException("read");
            }

            return !IsForwardRead(read);
        }
        #endregion Public Static Methods
    }

    /// <summary>
    /// Specifies the type of paired read.
    /// </summary>
    public enum PairedReadType
    {
        /// <summary>
        /// Normal - Reads are aligning to same reference sequence 
        ///         and insertion length is with in the limit. 
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Orphan - One read is not aligned to any reference sequence.
        /// </summary>
        Orphan,

        /// <summary>
        /// Chimera - Reads are not aligning to same reference sequence.
        /// </summary>
        Chimera,

        /// <summary>
        /// StructuralAnomaly - Reads are not in proper orientation.
        /// </summary>
        StructuralAnomaly,

        /// <summary>
        /// LengthAnomaly - Insertion length is either too short or too long.
        /// </summary>
        LengthAnomaly,

        /// <summary>
        /// MultipleHits - A mapped read pair is stored in more than two aligned sequences.
        /// </summary>
        MultipleHits
    }
}
