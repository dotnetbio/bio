using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

using Bio.Util;


namespace Bio.IO.SAM
{
    /// <summary>
    /// SAMAlignedSequenceHeader holds aligned sequence headers of the sam file format.
    /// </summary>
    [Serializable]
    public class SAMAlignedSequenceHeader
    {
        #region Fields
        #region Regular expressions
        /// <summary>
        /// Regular expression pattern for QName.
        /// </summary>
        private static char[] QNameIllegalCharacters =  {' ','\t','\n','\r'};

        /// <summary>
        /// Regular expression pattern for RName.
        /// </summary>
        public const string RNameRegxExprPattern = @"[^\s\t\n\r@=]+";

        /// <summary>
        /// Regular expression pattern for CIGAR.
        /// </summary>
        private const string CIGARRegxExprPattern = @"([0-9]+[MIDNSHPX=])+|\*";

        /// <summary>
        ///  Regular expression pattern for MRNM.
        /// </summary>
        private const string MRNMRegxExprPattern = @"[^\s\t\n\r@]+";

        /// <summary>
        /// Represents the largest possible value of POS. This field is constant.
        /// </summary>
        private const int POS_MaxValue = 536870911; // ((int)Math.Pow(2, 29)) - 1

        /// <summary>
        /// Represents the smallest possible value of POS. This field is constant.
        /// </summary>
        private const int POS_MinValue = 0;

        /// <summary>
        /// Represents the largest possible value of MPOS. This field is constant.
        /// </summary>
        private const int MPOS_MaxValue = 536870911; // ((int)Math.Pow(2, 29)) - 1

        /// <summary>
        /// Represents the smallest possible value of MPOS. This field is constant.
        /// </summary>
        private const int MPOS_MinValue = 0;

        /// <summary>
        /// Represents the largest possible value of ISize. This field is constant.
        /// </summary>
        private const int ISize_MaxValue = 536870911; // ((int)Math.Pow(2, 29)) -1

        /// <summary>
        /// Represents the smallest possible value of ISize. This field is constant.
        /// </summary>
        private const int ISize_MinValue = -536870911; // -((int)Math.Pow(2, 29)) + 1

        /// <summary>
        /// Represents the largest possible value of MapQ. This field is constant.
        /// </summary>
        private const int MapQ_MaxValue = 255; // ((int)Math.Pow(2, 8)) - 1

        /// <summary>
        /// Represents the smallest possible value of MapQ. This field is constant.
        /// </summary>
        private const int MapQ_MinValue = 0;

        /// <summary>
        /// Default value for read/query length.
        /// </summary>
        private const int DefaultReadLength = 0;

        /// <summary>
        /// Default value for CIGAR.
        /// </summary>
        private const string DefaultCIGAR = "*";

   
        /// <summary>
        /// Regular Expression object for RName.
        /// </summary>
        private static Regex RNameRegxExpr = new Regex(RNameRegxExprPattern);

        /// <summary>
        /// Regular Expression object for CIGAR.
        /// </summary>
        private static Regex CIGARRegxExpr = new Regex(CIGARRegxExprPattern);

        /// <summary>
        /// Regular Expression object for MRNM.
        /// </summary>
        private static Regex MRNMRegxExpr = new Regex(MRNMRegxExprPattern);

        #endregion

        /// <summary>
        /// Holds Query pair name if paired; or Query name if unpaired.
        /// </summary>
        private string qname;

        /// <summary>
        /// Holds Reference sequence name.
        /// </summary>
        private string rname;

        /// <summary>
        /// Holds left co-ordinate of alignment.
        /// </summary>
        private int pos;

        /// <summary>
        /// Holds MAPping Quality of alignment.
        /// </summary>
        private int mapq;

        /// <summary>
        /// Holds Leftmost mate position of the clipped sequence.
        /// </summary>
        private int mpos;

        /// <summary>
        /// Holds inferred insert size.
        /// </summary>
        private int isize;

        /// <summary>
        /// Holds the length of the alignment determined by the CIGAR value if available or the default read length if not.
        /// </summary>
        private int alignmentLength;

        /// <summary>
        /// Holds CIGAR value.
        /// </summary>
        private string cigar;

        /// <summary>
        /// Holds Mate Reference sequence name (MRNM).
        /// </summary>
        private string mrnm;

        /// <summary>
        /// Holds bin number.
        /// </summary>
        private int bin;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new SAMAlignedSequenceHeader instance.
        /// </summary>
        public SAMAlignedSequenceHeader()
        {
            OptionalFields = new List<SAMOptionalField>();
            cigar = DefaultCIGAR;
            alignmentLength = DefaultReadLength;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Query pair name if paired; or Query name if unpaired.
        /// </summary>  
        public string QName
        {
            get
            {
                return qname;
            }
            set
            {
                string message = IsValidQName(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                qname = value;
            }
        }

        /// <summary>
        /// SAM flags.
        /// <see cref="SAMFlags"/>
        /// </summary>
        public SAMFlags Flag { get; set; }

        /// <summary>
        /// Reference sequence name.
        /// </summary>
        public string RName
        {
            get
            {
                return rname;
            }
            set
            {
                string message = IsValidRName(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                rname = value;
            }
        }
        /// <summary>
        /// Set the name of the reference without validating it by a regular expression, useful
        /// when the same value is being repeatedly used.
        /// </summary>
        /// <param name="rName"></param>
        public void SetPreValidatedRName(string rName)
        {
            rname = rName;
        }

        /// <summary>
        /// One-based leftmost position/coordinate of the aligned sequence.
        /// </summary>
        public int Pos
        {
            get
            {
                return pos;
            }
            set
            {
                string message = IsValidPos(value);

                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                pos = value;
                bin = GetBin();
            }
        }

        /// <summary>
        /// Mapping quality (phred-scaled posterior probability that the 
        /// mapping position of this read is incorrect).
        /// </summary>
        public int MapQ
        {
            get
            {
                return mapq;
            }
            set
            {
                string message = IsValidMapQ(value);

                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                mapq = value;
            }
        }

        /// <summary>
        /// Extended CIGAR string.
        /// </summary>
        public string CIGAR
        {
            get
            {
                return cigar;
            }
            set
            {
                string message = IsValidCIGAR(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                cigar = value;
                alignmentLength = GetRefSeqAlignmentLengthFromCIGAR();
                bin = GetBin();
            }
        }
        /// <summary>
        /// Sets the value of an extended CIGAR string known to be valid
        /// </summary>
        /// <param name="value"></param>
        public void SetPreValidatedCIGAR(string value)
        {
            cigar = value;
            alignmentLength = GetRefSeqAlignmentLengthFromCIGAR();
            bin = GetBin();
        }

        /// <summary>
        /// Mate reference sequence name. 
        /// </summary>
        public string MRNM
        {
            get
            {
                return mrnm;
            }
            set
            {
                string message = IsValidMRNM(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                mrnm = value;
            }
        }
        /// <summary>
        /// Set the MRNM value without validating that it is a valid value
        /// </summary>
        /// <param name="value"></param>
        public void SetPreValidatedMRNM(string value)
        {
            mrnm = value;
        }
        /// <summary>
        /// One-based leftmost mate position of the clipped sequence.
        /// </summary>
        public int MPos
        {
            get
            {
                return mpos;
            }
            set
            {
                string message = IsValidMPos(value);

                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                mpos = value;
            }
        }

        /// <summary>
        /// Inferred insert size.
        /// </summary>
        public int ISize
        {
            get
            {
                return isize;
            }
            set
            {
                string message = IsValidISize(value);

                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                isize = value;
            }
        }

        /// <summary>
        /// Gets the Bin depending on the POS and CIGAR.
        /// </summary>
        public int Bin
        {
            get
            {
                return bin;
            }

            internal set
            {
                bin = value;
            }
        }

        /// <summary>
        /// Gets one based alignment end position of reference sequence depending on CIGAR Value.
        /// </summary>
        public int RefEndPos
        {
            get
            {
                return Pos+alignmentLength-1;
            }
            //TODO: Not clear that we should allow this field to be set from outside of this class, as alignment length, cigar string and RefEndPos need
            //to be consistent with each other, one should set the RefEndPosition by changing the cigar string.
            //internal set
            //{
            //    alignmentLength = value;
            //}
        }

        /// <summary>
        /// Optional fields.
        /// </summary>
        public IList<SAMOptionalField> OptionalFields { get; private set; }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Gets the SAMFlag for the specified integer value.
        /// </summary>
        /// <param name="value">Value for which the SAMFlag is required.</param>
        public static SAMFlags GetFlag(int value)
        {
            return (SAMFlags)value;
        }

        /// <summary>
        /// Gets the SAMFlag for the specified string value.
        /// </summary>
        /// <param name="value">Value for which the SAMFlag is required.</param>
        public static SAMFlags GetFlag(string value)
        {
            return GetFlag(int.Parse(value));
        }

        /// <summary>
        /// Gets Bin for the specified region.
        /// Note that this method returns zero for negative values.
        /// </summary>
        /// <param name="start">Zero based start co-ordinate of alignment.</param>
        /// <param name="end">Zero based end co-ordinate of the alignment.</param>
        public static int RegionToBin(int start, int end)
        {
            if (start < 0 || end <= 0)
            {
                return 0;
            }

            --end;
            if (start >> 14 == end >> 14) return ((1 << 15) - 1) / 7 + (start >> 14);
            if (start >> 17 == end >> 17) return ((1 << 12) - 1) / 7 + (start >> 17);
            if (start >> 20 == end >> 20) return ((1 << 9) - 1) / 7 + (start >> 20);
            if (start >> 23 == end >> 23) return ((1 << 6) - 1) / 7 + (start >> 23);
            if (start >> 26 == end >> 26) return ((1 << 3) - 1) / 7 + (start >> 26);
            return 0;
        }
        #endregion

        #region Private Methods
      
        /// <summary>
        /// Validates QName.
        /// </summary>
        /// <param name="qname">QName value to validate.</param>
        private static string IsValidQName(string qname)
        {

            // Validate for the regx.
            // validate length.
            if (qname.Length > 255)
            {
                return Properties.Resource.InvalidQNameLength;
            }
            bool badName = Helper.StringContainsIllegalCharacters(qname,QNameIllegalCharacters);
            if (!badName)
            {
                return String.Empty;
            }
            else
            {
                return "Query name: " + qname + " contains illegal characters";
            }

        }

        /// <summary>
        /// Validates RName.
        /// </summary>
        /// <param name="rname">RName value to validate.</param>
        private static string IsValidRName(string rname)
        {
            string headerName = "RName";
            // Validate for the regx.
            return Helper.IsValidPatternValue(headerName, rname, RNameRegxExpr);
        }

        /// <summary>
        /// Validates Pos.
        /// </summary>
        /// <param name="pos">Position value to validate.</param>
        private static string IsValidPos(int pos)
        {
            string headerName = "Pos";
            return Helper.IsValidRange(headerName, pos, POS_MinValue, POS_MaxValue);
        }

        /// <summary>
        /// Validates MapQ.
        /// </summary>
        /// <param name="mapq">MapQ value to validate.</param>
        private static string IsValidMapQ(int mapq)
        {
            string headerName = "MapQ";
            return Helper.IsValidRange(headerName, mapq, MapQ_MinValue, MapQ_MaxValue);
        }

        /// <summary>
        /// Validates CIGAR.
        /// </summary>
        /// <param name="cigar">CIGAR value to validate.</param>
        private static string IsValidCIGAR(string cigar)
        {
            string headerName = "CIGAR";
            // Validate for the regx.
            return Helper.IsValidPatternValue(headerName, cigar, CIGARRegxExpr);
        }

        /// <summary>
        /// Validates MRNM.
        /// </summary>
        /// <param name="mrnm">MRNM value to validate.</param>
        private static string IsValidMRNM(string mrnm)
        {
            string headerName = "MRNM";
            // Validate for the regx.
            return Helper.IsValidPatternValue(headerName, mrnm, MRNMRegxExpr);
        }

        /// <summary>
        /// Validates MPos.
        /// </summary>
        /// <param name="mpos">MPOS value to validate.</param>
        private static string IsValidMPos(int mpos)
        {
            string headerName = "MPos";
            return Helper.IsValidRange(headerName, mpos, MPOS_MinValue, MPOS_MaxValue);
        }

        /// <summary>
        /// Validates ISize.
        /// </summary>
        /// <param name="isize">ISIZE value to validate.</param>
        private static string IsValidISize(int isize)
        {
            string headerName = "ISize";
            return Helper.IsValidRange(headerName, isize, ISize_MinValue, ISize_MaxValue);
        }

        /// <summary>
        /// Returns the bin number.
        /// </summary>
        private int GetBin()
        {
            // As SAM stores 1 based position and to calculte BAM Bin, zero based positions are required.
            int start = Pos - 1;
            int end = start + alignmentLength - 1;
            return RegionToBin(start, end);
        }

        /// <summary>
        /// Gets the reference sequence alignment length depending on the CIGAR value.
        /// </summary>
        /// <returns>Length of the alignment.</returns>
        private int GetRefSeqAlignmentLengthFromCIGAR()
        {
            if (string.IsNullOrWhiteSpace(CIGAR) || CIGAR.Equals("*"))
            {
                return DefaultReadLength;
            }

            List<KeyValuePair<char,int>> charsAndPositions = new List<KeyValuePair<char,int>>();
            
            for (int i = 0; i < CIGAR.Length; i++)
            {
                char ch = CIGAR[i];
                if (Char.IsDigit(ch))
                {
                    continue;
                }
                charsAndPositions.Add(new KeyValuePair<char,int>(ch,i));
            }
            string CIGARforClen = "MDNX=";
            int len = 0;
            for (int i = 0; i < charsAndPositions.Count; i++)
            {
                char ch = charsAndPositions[i].Key;
                int start = 0;
                int end = 0;
                if (CIGARforClen.Contains(ch))
                {
                    if (i == 0)
                    {
                        start = 0;
                    }
                    else
                    {
                        start = charsAndPositions[i - 1].Value + 1;
                    }

                    end = charsAndPositions[i].Value - start;

                    len += int.Parse(CIGAR.Substring(start, end), CultureInfo.InvariantCulture);
                }
            }
            return len;
        }
        #endregion
    }
}
