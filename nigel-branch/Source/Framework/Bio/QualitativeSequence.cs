using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using Bio.Util;

namespace Bio
{
    /// <summary>
    /// This class holds quality scores along with the sequence data.
    /// </summary>
    public class QualitativeSequence : ISequence
    {
        #region Member variables
        /// <summary>
        /// Phred minimum quality score: 0
        /// </summary>
        public const int Phred_MinQualityScore = 0;

        /// <summary>
        /// Phred maximum quality score: 93
        /// </summary>
        public const int Phred_MaxQualityScore = 93;

        /// <summary>
        /// Solexa minimum quality score: -5
        /// </summary>
        public const int Solexa_MinQualityScore = -5;

        /// <summary>
        /// Solexa maximum quality score: 62
        /// </summary>
        public const int Solexa_MaxQualityScore = 62;

        /// <summary>
        /// Minimum encoded quality score for Sanger format: 33
        /// </summary>
        public const byte Sanger_MinEncodedQualScore = 33;

        /// <summary>
        /// Maximum encoded quality score for Sanger format: 126
        /// </summary>
        public const byte Sanger_MaxEncodedQualScore = 126;

        /// <summary>
        /// Minimum encoded quality score for Solexa/Illumina v1.0 format: 59
        /// </summary>
        public const byte Solexa_Illumina_v1_0_MinEncodedQualScore = 59;

        /// <summary>
        /// Maximum encoded quality score for Solexa/Illumina v1.0 format: 126
        /// </summary>
        public const byte Solexa_Illumina_v1_0_MaxEncodedQualScore = 126;

        /// <summary>
        /// Minimum encoded quality score for Illumina v1.3 format: 64
        /// </summary>
        public const byte Illumina_v1_3_MinEncodedQualScore = 64;

        /// <summary>
        /// Maximum encoded quality score for Illumina v1.3 format: 126
        /// </summary>
        public const byte Illumina_v1_3_MaxEncodedQualScore = 126;

        /// <summary>
        /// Minimum encoded quality score for Illumina v1.5 format: 64
        /// </summary>
        public const byte Illumina_v1_5_MinEncodedQualScore = 64;

        /// <summary>
        /// Maximum encoded quality score for Illumina v1.5 format: 126
        /// </summary>
        public const byte Illumina_v1_5_MaxEncodedQualScore = 126;

        /// <summary>
        /// Minimum encoded quality score for Illumina v1.8 format: 33
        /// </summary>
        public const byte Illumina_v1_8_MinEncodedQualScore = 33;

        /// <summary>
        /// Maximum encoded quality score for Illumina v1.8 format: 126
        /// </summary>
        public const byte Illumina_v1_8_MaxEncodedQualScore = 126;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Sanger format: 33
        /// </summary>
        private const int Sanger_AsciiBaseValue = 33;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Solexa/Illumina 1.0 format: 64
        /// </summary>
        private const int Solexa_Illumina_v1_0_AsciiBaseValue = 64;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Illumina v1.3 format: 64
        /// </summary>
        private const int Illumina_v1_3_AsciiBaseValue = 64;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Illumina v1.5 format: 64
        /// </summary>
        private const int Illumina_v1_5_AsciiBaseValue = 64;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Illumina 1.8 format: 33
        /// </summary>
        private const int Illumina_v1_8_AsciiBaseValue = 33;

        /// <summary>
        /// Default quality score.
        /// </summary>
        private const int DefaultQualScore = 60;

        /// <summary>
        /// Holds sequence data.
        /// </summary>
        private byte[] sequenceData;

        /// <summary>
        /// Holds decoded quality scores
        /// </summary>
        private sbyte[] qualityScores;

        /// <summary>
        /// Metadata is features or references or related things of a sequence.
        /// </summary>
        private Dictionary<string, object> metadata;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// byte array representing symbols and encoded quality scores.
        /// Sequence and quality scores are validated with the specified alphabet and specified fastq format respectively.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">An array of bytes representing the symbols.</param>
        /// <param name="encodedQualityScores">An array of bytes representing the encoded quality scores.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, byte[] sequence, byte[] encodedQualityScores)
            : this(alphabet, fastQFormatType, sequence, encodedQualityScores, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// byte array representing symbols and encoded quality scores.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">An array of bytes representing the symbols.</param>
        /// <param name="encodedQualityScores">An array of bytes representing the encoded quality scores.</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, byte[] sequence, byte[] encodedQualityScores, bool validate)
        {
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (encodedQualityScores == null)
            {
                throw new ArgumentNullException("encodedQualityScores");
            }

            this.Alphabet = alphabet;
            this.ID = string.Empty;
            this.FormatType = fastQFormatType;

            if (validate)
            {
                if (sequence.LongLength() != encodedQualityScores.LongLength())
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                                Properties.Resource.DifferenceInSequenceAndQualityScoresLengthMessage,
                                                sequence.LongLength(),
                                                encodedQualityScores.LongLength());
                    throw new ArgumentException(message);
                }

                // Validate sequence data
                if (!this.Alphabet.ValidateSequence(sequence, 0, sequence.LongLength()))
                {
                    throw new ArgumentOutOfRangeException("sequence");
                }

                byte invalidEncodedQualityScore;
                // Validate quality scores
                if (!ValidateQualScores(encodedQualityScores, this.FormatType, out invalidEncodedQualityScore))
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                        Properties.Resource.InvalidEncodedQualityScoreFound,
                                        (char)invalidEncodedQualityScore,
                                        this.FormatType);
                    throw new ArgumentOutOfRangeException("encodedQualityScores", message);
                }
            }

            this.sequenceData = new byte[sequence.LongLength()];
            this.qualityScores = new sbyte[encodedQualityScores.LongLength()];

            Helper.Copy(sequence, this.sequenceData, sequence.LongLength());
            this.qualityScores = GetDecodedQualScoresInSignedBytes(encodedQualityScores, this.FormatType);

            this.Count = this.sequenceData.LongLength();
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// string representing symbols and encoded quality scores.
        /// Sequence and quality scores are validated with the specified alphabet and specified fastq format respectively.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">A string representing the symbols.</param>
        /// <param name="encodedQualityScores">A string representing the encoded quality scores.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, string sequence, string encodedQualityScores)
            : this(alphabet, fastQFormatType, sequence, encodedQualityScores, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// string representing symbols and encoded quality scores.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">A string representing the symbols.</param>
        /// <param name="encodedQualityScores">A string representing the encoded quality scores.</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, string sequence, string encodedQualityScores, bool validate)
        {
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            this.Alphabet = alphabet;
            this.ID = string.Empty;

            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (encodedQualityScores == null)
            {
                throw new ArgumentNullException("encodedQualityScores");
            }

            this.FormatType = fastQFormatType;
            this.sequenceData = UTF8Encoding.UTF8.GetBytes(sequence);
            byte[] encodedQualityScoresarray = UTF8Encoding.UTF8.GetBytes(encodedQualityScores);

            if (validate)
            {
                if (this.sequenceData.LongLength() != encodedQualityScoresarray.LongLength())
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                                Properties.Resource.DifferenceInSequenceAndQualityScoresLengthMessage,
                                                 this.sequenceData.LongLength(),
                                                encodedQualityScoresarray.LongLength());
                    throw new ArgumentException(message);
                }

                // Validate sequence data
                if (!this.Alphabet.ValidateSequence(this.sequenceData, 0, this.sequenceData.LongLength()))
                {
                    throw new ArgumentOutOfRangeException("sequence");
                }

                byte invalidEncodedQualityScore;
                // Validate quality scores
                if (!ValidateQualScores(encodedQualityScoresarray, this.FormatType, out invalidEncodedQualityScore))
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                            Properties.Resource.InvalidEncodedQualityScoreFound,
                                            (char)invalidEncodedQualityScore,
                                            this.FormatType);
                    throw new ArgumentOutOfRangeException("encodedQualityScores", message);
                }
            }

            this.qualityScores = GetDecodedQualScoresInSignedBytes(encodedQualityScoresarray, this.FormatType);
            this.Count = this.sequenceData.LongLength();
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// byte array representing symbols and signed byte array representing base quality scores 
        /// (Phred or Solexa base according to the FastQ format type).
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">An array of bytes representing the symbols.</param>
        /// <param name="qualityScores">An array of signed bytes representing the base quality scores 
        /// (Phred or Solexa base according to the FastQ format type).</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, byte[] sequence, sbyte[] qualityScores, bool validate)
        {
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (qualityScores == null)
            {
                throw new ArgumentNullException("qualityScores");
            }

            this.Alphabet = alphabet;
            this.ID = string.Empty;
            this.FormatType = fastQFormatType;
            if (validate)
            {
                if (sequence.LongLength() != qualityScores.LongLength())
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                                Properties.Resource.DifferenceInSequenceAndQualityScoresLengthMessage,
                                                sequence.LongLength(),
                                                qualityScores.LongLength());
                    throw new ArgumentException(message);
                }

                // Validate sequence data
                if (!this.Alphabet.ValidateSequence(sequence, 0, sequence.LongLength()))
                {
                    throw new ArgumentOutOfRangeException("sequence");
                }

                sbyte invalidQualityScore;

                // Validate quality scores
                if (!ValidateQualScores(qualityScores, this.FormatType, out invalidQualityScore))
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                            Properties.Resource.InvalidQualityScoreFound,
                                            invalidQualityScore,
                                            this.FormatType);
                    throw new ArgumentOutOfRangeException("qualityScores", message);
                }
            }

            this.sequenceData = new byte[sequence.LongLength()];
            this.qualityScores = new sbyte[qualityScores.LongLength()];
            Helper.Copy(sequence, this.sequenceData, sequence.LongLength());
            Helper.Copy(qualityScores, this.qualityScores, qualityScores.LongLength());

            this.Count = this.sequenceData.LongLength();
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// byte array representing symbols and integer array representing base quality scores 
        /// (Phred or Solexa base according to the FastQ format type).
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">An array of bytes representing the symbols.</param>
        /// <param name="qualityScores">An array of integers representing the base quality scores 
        /// (Phred or Solexa base according to the FastQ format type).</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, byte[] sequence, int[] qualityScores, bool validate)
        {
            if (alphabet == null)
            {
                throw new ArgumentNullException("alphabet");
            }

            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (qualityScores == null)
            {
                throw new ArgumentNullException("qualityScores");
            }

            this.Alphabet = alphabet;
            this.ID = string.Empty;
            this.FormatType = fastQFormatType;
            if (validate)
            {
                if (sequence.LongLength() != qualityScores.LongLength())
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                                Properties.Resource.DifferenceInSequenceAndQualityScoresLengthMessage,
                                                sequence.LongLength(),
                                                qualityScores.LongLength());
                    throw new ArgumentException(message);
                }

                // Validate sequence data
                if (!this.Alphabet.ValidateSequence(sequence, 0, sequence.LongLength()))
                {
                    throw new ArgumentOutOfRangeException("sequence");
                }

                int invalidQualityScore;

                // Validate quality scores
                if (!ValidateQualScores(qualityScores, this.FormatType, out invalidQualityScore))
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                            Properties.Resource.InvalidQualityScoreFound,
                                            invalidQualityScore,
                                            this.FormatType);
                    throw new ArgumentOutOfRangeException("qualityScores", message);
                }
            }

            long len = qualityScores.LongLength();
            this.sequenceData = new byte[sequence.LongLength()];
            this.qualityScores = new sbyte[len];
            Helper.Copy(sequence, this.sequenceData, sequence.LongLength());

            for (long i = 0; i < len; i++)
            {
                this.qualityScores[i] = (sbyte)qualityScores[i];
            }

            this.Count = this.sequenceData.LongLength();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Identifier.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets the alphabet to which symbols in this sequence belongs to.
        /// </summary>
        public IAlphabet Alphabet { get; private set; }

        /// <summary>
        /// Gets the number of bytes contained in the Sequence.
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Gets the quality scores format type.
        /// Ex: Illumina/Solexa/Sanger.
        /// </summary>
        public FastQFormatType FormatType { get; private set; }

        /// <summary>
        /// <para>
        /// Gets or sets the metadata for this qualitative sequence.
        /// </para>
        /// <para>
        /// Many sequence representations when saved to file also contain
        /// information about that sequence. Unfortunately there is no standard
        /// around what that data may be from format to format. This property
        /// allows a place to put structured metadata that can be accessed by
        /// a particular key.
        /// </para>
        /// <para>
        /// For example, if species information is stored in a particular Species
        /// class, you could add it to the dictionary by:
        /// </para>
        /// <para>
        /// mySequence.Metadata["SpeciesInfo"] = mySpeciesInfo;
        /// </para>
        /// To fetch the data you would use:
        /// <para>
        /// Species mySpeciesInfo = mySequence.Metadata["SpeciesInfo"];
        /// </para>
        /// Particular formats may create their own data model class for information
        /// unique to their format as well. Such as:
        /// <para>
        /// GenBankMetadata genBankData = new GenBankMetadata();
        /// </para>
        /// <para>
        /// // ... add population code
        /// </para>
        /// <para>
        /// mySequence.MetaData["GenBank"] = genBankData;.
        /// </para>
        /// </summary>
        public Dictionary<string, object> Metadata
        {
            get
            {
                if (this.metadata == null)
                {
                    this.metadata = new Dictionary<string, object>();
                }

                return this.metadata;
            }

            set
            {
                this.metadata = value;
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Returns the byte which represents the symbol found at the specified index if within bounds. Note 
        /// that the index value starts at 0.
        /// </summary>
        /// <param name="index">Index at which the symbol is required.</param>
        /// <returns>Symbol at the given index.</returns>
        public byte this[long index]
        {
            get { return this.sequenceData[index]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts Quality score from to specified format.
        /// </summary>
        /// <param name="fromFormatType">from fastq format.</param>
        /// <param name="toFormatType">to fastq format.</param>
        /// <param name="qualScore">Quality score.</param>
        public static int ConvertQualityScore(FastQFormatType fromFormatType, FastQFormatType toFormatType, int qualScore)
        {
            int result;
            int invalidQualScore;

            if (!ValidateQualScores(new int[] { qualScore }, fromFormatType, out invalidQualScore))
            {
                string message = string.Format(CultureInfo.CurrentUICulture, Properties.Resource.InvalidQualityScore, invalidQualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }


            if (fromFormatType == toFormatType)
            {
                result = qualScore;
            }
            else
            {
                BaseQualityScoreType fromQualityType = GetQualityScoreType(fromFormatType);
                BaseQualityScoreType toQualityType = GetQualityScoreType(toFormatType);
                if (fromQualityType == toQualityType)
                {
                    result = qualScore;
                }
                else
                {
                    result = Convert(fromQualityType, toQualityType, qualScore);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts Quality scores from to specified format.
        /// </summary>
        /// <param name="fromFormatType">from fastq format.</param>
        /// <param name="toFormatType">to fastq format.</param>
        /// <param name="qualScores">Quality scores.</param>
        public static int[] ConvertQualityScores(FastQFormatType fromFormatType, FastQFormatType toFormatType, int[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int invalidQualScore;
            if (!ValidateQualScores(qualScores, fromFormatType, out invalidQualScore))
            {
                string message = string.Format(CultureInfo.CurrentUICulture, Properties.Resource.InvalidQualityScore, invalidQualScore);
                throw new ArgumentOutOfRangeException("qualScores", message);
            }

            int[] result;
            if (fromFormatType == toFormatType)
            {
                result = new int[qualScores.LongLength()];
                Helper.Copy(qualScores, result, qualScores.LongLength());
            }
            else
            {
                BaseQualityScoreType fromQualityType = GetQualityScoreType(fromFormatType);
                BaseQualityScoreType toQualityType = GetQualityScoreType(toFormatType);
                if (fromQualityType == toQualityType)
                {
                    result = new int[qualScores.LongLength()];
                    Helper.Copy(qualScores, result, qualScores.LongLength());
                }
                else
                {
                    result = Convert(fromQualityType, toQualityType, qualScores);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts Quality scores from to specified format.
        /// </summary>
        /// <param name="fromFormatType">from fastq format.</param>
        /// <param name="toFormatType">to fastq format.</param>
        /// <param name="qualScores">Quality scores.</param>
        public static sbyte[] ConvertQualityScores(FastQFormatType fromFormatType, FastQFormatType toFormatType, sbyte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            sbyte invalidQualScore;
            if (!ValidateQualScores(qualScores, fromFormatType, out invalidQualScore))
            {
                string message = string.Format(CultureInfo.CurrentUICulture, Properties.Resource.InvalidQualityScore, invalidQualScore);
                throw new ArgumentOutOfRangeException("qualScores", message);
            }

            sbyte[] result;
            if (fromFormatType == toFormatType)
            {
                result = new sbyte[qualScores.LongLength()];
                Helper.Copy(qualScores, result, qualScores.LongLength());
            }
            else
            {
                BaseQualityScoreType fromQualityType = GetQualityScoreType(fromFormatType);
                BaseQualityScoreType toQualityType = GetQualityScoreType(toFormatType);
                if (fromQualityType == toQualityType)
                {
                    result = new sbyte[qualScores.LongLength()];
                    Helper.Copy(qualScores, result, qualScores.LongLength());
                }
                else
                {
                    result = Convert(fromQualityType, toQualityType, qualScores);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts Encoded quality score from to specified format.
        /// </summary>
        /// <param name="fromFormatType">from fastq format.</param>
        /// <param name="toFormatType">to fastq format.</param>
        /// <param name="encodedqualScore">Encoded quality score.</param>
        public static byte ConvertEncodedQualityScore(FastQFormatType fromFormatType, FastQFormatType toFormatType, byte encodedqualScore)
        {
            byte result;
            byte invalidQualScore;
            if (!ValidateQualScores(new byte[] {encodedqualScore}, fromFormatType, out invalidQualScore))
            {
                string message = string.Format(CultureInfo.CurrentUICulture, Properties.Resource.InvalidQualityScore, (char)invalidQualScore);
                throw new ArgumentOutOfRangeException("encodedqualScore", message);
            }

            if (fromFormatType == toFormatType)
            {
                result = encodedqualScore;
            }
            else
            {
                int fromQualScore = GetDecodedQualScore(encodedqualScore, fromFormatType);
                int toQualScore = ConvertQualityScore(fromFormatType, toFormatType, fromQualScore);
                result = GetEncodedQualScore(toQualScore, toFormatType);
            }

            return result;
        }

        /// <summary>
        /// Converts Encoded quality scores from to specified format.
        /// </summary>
        /// <param name="fromFormatType">from fastq format.</param>
        /// <param name="toFormatType">to fastq format.</param>
        /// <param name="encodedqualScores">Encoded quality scores.</param>
        public static byte[] ConvertEncodedQualityScore(FastQFormatType fromFormatType, FastQFormatType toFormatType, byte[] encodedqualScores)
        {
            if (encodedqualScores == null)
            {
                throw new ArgumentNullException("encodedqualScores");
            }
            byte invalidQualScore;
            if (!ValidateQualScores(encodedqualScores, fromFormatType, out invalidQualScore))
            {
                string message = string.Format(CultureInfo.CurrentUICulture, Properties.Resource.InvalidQualityScore,(char) invalidQualScore);
                throw new ArgumentOutOfRangeException("encodedqualScores", message);
            }

            byte[] result;
            if (fromFormatType == toFormatType)
            {
                result = new byte[encodedqualScores.LongLength()];
                Helper.Copy(encodedqualScores, result, encodedqualScores.LongLength());
            }
            else
            {
                int[] fromQualScore = GetDecodedQualScores(encodedqualScores, fromFormatType);
                int[] toQualScore = ConvertQualityScores(fromFormatType, toFormatType, fromQualScore);
                result = GetEncodedQualScores(toQualScore, toFormatType);
            }

            return result;
        }

        /// <summary>
        /// Gets the default quality score for the specified FastQFormatType.
        /// </summary>
        ///  /// <param name="type">FastQ format type.</param>
        /// <returns>Quality score.</returns>
        public static byte GetDefaultQualScore(FastQFormatType type)
        {
            return (byte)(GetEncodedQualScore(DefaultQualScore, type));
        }

        /// <summary>
        /// Gets the maximum encoded quality score for the specified FastQFormatType.
        /// </summary>
        ///  /// <param name="formatType">FastQ format type.</param>
        /// <returns>Quality score.</returns>
        public static byte GetMaxEncodedQualScore(FastQFormatType formatType)
        {
            byte result;
            switch (formatType)
            {
                case FastQFormatType.Sanger:
                    result = Sanger_MaxEncodedQualScore;
                    break;
                case FastQFormatType.Solexa_Illumina_v1_0:
                    result = Solexa_Illumina_v1_0_MaxEncodedQualScore;
                    break;
                case FastQFormatType.Illumina_v1_3:
                    result = Illumina_v1_3_MaxEncodedQualScore;
                    break;
                case FastQFormatType.Illumina_v1_5:
                    result = Illumina_v1_5_MaxEncodedQualScore;
                    break;
                default:
                    result = Illumina_v1_8_MaxEncodedQualScore;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the minimum encoded quality score for the specified FastQFormatType.
        /// </summary>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>Quality score.</returns>
        public static byte GetMinEncodedQualScore(FastQFormatType formatType)
        {
            byte result;
            switch (formatType)
            {
                case FastQFormatType.Sanger:
                    result = Sanger_MinEncodedQualScore;
                    break;
                case FastQFormatType.Solexa_Illumina_v1_0:
                    result = Solexa_Illumina_v1_0_MinEncodedQualScore;
                    break;
                case FastQFormatType.Illumina_v1_3:
                    result = Illumina_v1_3_MinEncodedQualScore;
                    break;
                case FastQFormatType.Illumina_v1_5:
                    result = Illumina_v1_5_MinEncodedQualScore;
                    break;
                default:
                    result = Illumina_v1_8_MinEncodedQualScore;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the encoded quality score found at the specified index if within bounds. Note that the index value start at 0.
        /// </summary>
        /// <param name="index">Index at which the symbol is required.</param>
        /// <returns>Quality Score at the given index.</returns>
        public byte GetEncodedQualityScore(long index)
        {
            return GetEncodedQualScore(this.qualityScores[index], this.FormatType);
        }

        /// <summary>
        /// Gets the encoded quality scores.
        /// </summary>
        public byte[] GetEncodedQualityScores()
        {
            return GetEncodedQualScores(this.qualityScores, this.FormatType);
        }

        /// <summary>
        /// Returns base quality scores at specified index.
        /// Returns Solexa quality scores if the FastQFormat type of this instance is Solexa Illumina v1.0,
        /// else returns Phred quality scores.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetQualityScore(long index)
        {
            return this.qualityScores[index];
        }

        /// <summary>
        /// Returns base quality scores.
        /// Returns Solexa quality scores if the FastQFormat type of this instance is Solexa Illumina v1.0,
        /// else returns Phred quality scores.
        /// </summary>
        public int[] GetQualityScores()
        {
            long count = this.qualityScores.LongLength();
            int[] baseQualityScores = new int[count];
            Helper.Copy(this.qualityScores, baseQualityScores, count);

            return baseQualityScores;
        }

        /// <summary>
        /// Gets the Phred base quality score.
        /// </summary>
        /// <param name="index">Index of the required score.</param>
        /// <returns>Returns an integer value representing Phred quality score.</returns>
        public int GetPhredQualityScore(long index)
        {
            int phredQualityScore;
            if (this.FormatType == FastQFormatType.Solexa_Illumina_v1_0)
            {
                int solexaQualityScore = this.qualityScores[index];
                phredQualityScore = ConvertSolexaToPhred(solexaQualityScore);
            }
            else
            {
                phredQualityScore = this.qualityScores[index];
            }

            return phredQualityScore;
        }

        /// <summary>
        /// Gets the Phred base quality scores.
        /// </summary>
        public int[] GetPhredQualityScores()
        {
            long count = this.qualityScores.LongLength();
            int[] phredQualityScores = new int[count];
            BaseQualityScoreType fromQualityScoreType = GetQualityScoreType(this.FormatType);
            BaseQualityScoreType toQualityScoreType = BaseQualityScoreType.PhredBaseQualityScore;
            if (fromQualityScoreType == toQualityScoreType)
            {
                Helper.Copy(this.qualityScores, phredQualityScores, count);
            }
            else
            {
                for (long i = 0; i < count; i++)
                {
                    phredQualityScores[i] = Convert(fromQualityScoreType, toQualityScoreType, this.qualityScores[i]);
                }
            }

            return phredQualityScores;
        }

        /// <summary>
        /// Gets the Solexa base quality score.
        /// </summary>
        /// <param name="index">Index of the required score.</param>
        /// <returns>Returns an integer value representing Solexa quality score.</returns>
        public int GetSolexaQualityScore(long index)
        {
            int solexaQualityScore;
            if (this.FormatType == FastQFormatType.Solexa_Illumina_v1_0)
            {
                solexaQualityScore = this.qualityScores[index];
            }
            else
            {
                int pharedQualityScore = this.qualityScores[index];
                solexaQualityScore = ConvertPhredToSolexa(pharedQualityScore);
            }

            return solexaQualityScore;
        }

        /// <summary>
        /// Gets the solexa base quality scores.
        /// </summary>
        public int[] GetSolexaQualityScores()
        {
            long count = this.qualityScores.LongLength();
            int[] solexaQualityScores = new int[count];
            BaseQualityScoreType fromQualityScoreType = GetQualityScoreType(this.FormatType);
            BaseQualityScoreType toQualityScoreType = BaseQualityScoreType.SolexaBaseQualityScore;
            if (fromQualityScoreType == toQualityScoreType)
            {
                Helper.Copy(this.qualityScores, solexaQualityScores, count);
            }
            else
            {
                for (long i = 0; i < count; i++)
                {
                    solexaQualityScores[i] = Convert(fromQualityScoreType, toQualityScoreType, this.qualityScores[i]);
                }
            }

            return solexaQualityScores;
        }

        /// <summary>
        /// Converts the current instance to the specified FastQ format type 
        /// and returns a new instance of QualitativeSequence.
        /// </summary>
        /// <param name="formatType">FastQ format type to convert.</param>
        public QualitativeSequence ConvertTo(FastQFormatType formatType)
        {
            sbyte[] convertedQualityScores = ConvertQualityScores(this.FormatType, formatType, this.qualityScores);

            QualitativeSequence seq = new QualitativeSequence(this.Alphabet, formatType, this.sequenceData, convertedQualityScores, false);
            seq.ID = this.ID;
            seq.metadata = this.metadata;

            return seq;
        }

        /// <summary>
        /// Return a new QualitativeSequence representing this QualitativeSequence with the orientation reversed.
        /// </summary>
        public ISequence GetReversedSequence()
        {
            byte[] newSequenceData = new byte[this.sequenceData.LongLength()];
            sbyte[] newQualityScores = new sbyte[this.qualityScores.LongLength()];

            for (long index = 0; index < this.sequenceData.LongLength(); index++)
            {
                newSequenceData[index] = this.sequenceData[this.sequenceData.LongLength() - index - 1];
                newQualityScores[index] = (this.qualityScores[this.qualityScores.LongLength() - index - 1]);
            }

            QualitativeSequence seq = new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores, false);
            seq.ID = this.ID;
            seq.metadata = this.metadata;

            return seq;
        }

        /// <summary>
        /// Return a new QualitativeSequence representing the complement of this QualitativeSequence.
        /// </summary>
        public ISequence GetComplementedSequence()
        {
            byte[] newSequenceData = new byte[this.sequenceData.LongLength()];
            sbyte[] newQualityScores = this.qualityScores;

            for (long index = 0; index < this.sequenceData.LongLength(); index++)
            {
                byte complementedSymbol;
                byte symbol = this.sequenceData[index];
                if (!this.Alphabet.TryGetComplementSymbol(symbol, out complementedSymbol))
                {
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentUICulture, Properties.Resource.ComplementNotSupportedByalphabet, (char)symbol, this.Alphabet.Name));
                }

                newSequenceData[index] = complementedSymbol;
            }

            QualitativeSequence seq = new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores, false);
            seq.ID = this.ID;
            seq.metadata = this.metadata;

            return seq;
        }

        /// <summary>
        /// Return a new QualitativeSequence representing the reverse complement of this QualitativeSequence.
        /// </summary>
        public ISequence GetReverseComplementedSequence()
        {
            byte[] newSequenceData = new byte[this.sequenceData.LongLength()];
            sbyte[] newQualityScores = new sbyte[this.qualityScores.LongLength()];

            for (long index = 0; index < this.sequenceData.LongLength(); index++)
            {
                byte complementedSymbol;
                byte symbol = this.sequenceData[this.sequenceData.LongLength() - index - 1];

                if (!this.Alphabet.TryGetComplementSymbol(symbol, out complementedSymbol))
                {
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentUICulture, Properties.Resource.ComplementNotSupportedByalphabet, (char)symbol, this.Alphabet.Name));
                }

                newSequenceData[index] = complementedSymbol;

                newQualityScores[index] = this.qualityScores[this.qualityScores.LongLength() - index - 1];
            }

            QualitativeSequence seq = new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores, false);
            seq.ID = this.ID;
            seq.metadata = this.metadata;

            return seq;
        }

        /// <summary>
        /// Return a new QualitativeSequence representing a range (subsequence) of this QualitativeSequence.
        /// </summary>
        /// <param name="start">The index of the first symbol in the range.</param>
        /// <param name="length">The number of symbols in the range.</param>
        /// <returns>The sub-sequence.</returns>
        public ISequence GetSubSequence(long start, long length)
        {
            if (start >= this.sequenceData.LongLength())
            {
                throw new ArgumentOutOfRangeException("start");
            }

            if (start + length > this.sequenceData.LongLength())
            {
                throw new ArgumentOutOfRangeException("length");
            }

            byte[] newSequenceData = new byte[length];
            sbyte[] newQualityScores = new sbyte[length];

            for (long index = 0; index < length; index++)
            {
                newSequenceData[index] = this.sequenceData[start + index];
                newQualityScores[index] = this.qualityScores[start + index];
            }

            return new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores, false)
            {
                ID = this.ID, 
                metadata = this.metadata
            };
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
            if (startPos >= this.sequenceData.LongLength())
            {
                throw new ArgumentOutOfRangeException("startPos");
            }

            HashSet<byte> gapSymbols;
            if (!this.Alphabet.TryGetGapSymbols(out gapSymbols))
            {
                return startPos;
            }

            byte[] aliasSymbolsMap = this.Alphabet.GetSymbolValueMap();
            for (long index = startPos; index < this.sequenceData.LongLength(); index++)
            {
                byte symbol = aliasSymbolsMap[this.sequenceData[index]];
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
                byte symbol = aliasSymbolsMap[this.sequenceData[index]];
                if (!gapSymbols.Contains(symbol))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a string representation of the Qualitative sequence data. This representation
        /// will come from the symbols in the alphabet defined for the sequence.
        /// Also their Quality scores.
        /// </summary>
        public override string ToString()
        {
            if (this.Count > Helper.AlphabetsToShowInToString)
            {
                return string.Format(CultureInfo.CurrentCulture, Properties.Resource.QualitativeSequenceToStringFormatForLongSequence,
                                     new string(this.sequenceData.Take(Helper.AlphabetsToShowInToString).Select((a => (char)a)).ToArray()),
                                     (this.Count - Helper.AlphabetsToShowInToString),
                                     new string(this.qualityScores.Take(Helper.AlphabetsToShowInToString).Select(a => (char)GetEncodedQualScore(a, this.FormatType)).ToArray()),
                                     (this.Count - Helper.AlphabetsToShowInToString));
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, Properties.Resource.QualitativeSequenceToStringFormatForSmallSequence,
                                     new string(this.sequenceData.Take(this.sequenceData.Length).Select((a => (char)a)).ToArray()),
                                     new string(this.qualityScores.Take(this.qualityScores.Length).Select(a => (char)GetEncodedQualScore(a, this.FormatType)).ToArray()));
            }
        }

        /// <summary>
        /// Gets an enumerator to the bytes present symbols in this sequence.
        /// </summary>
        /// <returns>An IEnumerator of bytes.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            for (long index = 0; index < this.sequenceData.LongLength(); index++)
            {
                yield return this.sequenceData[index];
            }
        }

        /// <summary>
        /// Gets an enumerator to the bytes present symbols in this sequence.
        /// </summary>
        /// <returns>An IEnumerator of bytes.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the Ascii base value for the specified format.
        /// </summary>
        /// <param name="formatType">FastQ format.</param>
        private static int GetAsciiBaseValue(FastQFormatType formatType)
        {
            int result;
            switch (formatType)
            {
                case FastQFormatType.Sanger:
                    result = Sanger_AsciiBaseValue;
                    break;
                case FastQFormatType.Solexa_Illumina_v1_0:
                    result = Solexa_Illumina_v1_0_AsciiBaseValue;
                    break;
                case FastQFormatType.Illumina_v1_3:
                    result = Illumina_v1_3_AsciiBaseValue;
                    break;
                case FastQFormatType.Illumina_v1_5:
                    result = Illumina_v1_5_AsciiBaseValue;
                    break;
                default:
                    result = Illumina_v1_8_AsciiBaseValue;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the quality score type for the specified format.
        /// </summary>
        /// <param name="formatType">FastQ format.</param>
        private static BaseQualityScoreType GetQualityScoreType(FastQFormatType formatType)
        {
            BaseQualityScoreType result;
            switch (formatType)
            {
                case FastQFormatType.Solexa_Illumina_v1_0:
                    result = BaseQualityScoreType.SolexaBaseQualityScore;
                    break;
                default:
                    result = BaseQualityScoreType.PhredBaseQualityScore;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the decoded quality score from the ASCII encoded quality score.
        /// </summary>
        /// <param name="encodedQualScore">ASCII Encoded quality score.</param>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>Returns quality score.</returns>
        private static int GetDecodedQualScore(byte encodedQualScore, FastQFormatType formatType)
        {
            return DecodeQualityScore(encodedQualScore, GetAsciiBaseValue(formatType));
        }

        /// <summary>
        /// Gets the decoded quality scores from the ASCII encoded quality score.
        /// </summary>
        /// <param name="encodedQualScores">ASCII Encoded quality score.</param>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>Returns quality scores.</returns>
        private static int[] GetDecodedQualScores(byte[] encodedQualScores, FastQFormatType formatType)
        {
            int baseValue = GetAsciiBaseValue(formatType);
            long count = encodedQualScores.LongLength();
            int[] result = new int[count];
            for (long i = 0; i < count; i++)
            {
                result[i] = DecodeQualityScore(encodedQualScores[i], baseValue);
            }

            return result;
        }

        /// <summary>
        /// Gets the decoded quality scores from the ASCII encoded quality score.
        /// </summary>
        /// <param name="encodedQualScores">ASCII Encoded quality score.</param>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>Returns quality scores.</returns>
        private static sbyte[] GetDecodedQualScoresInSignedBytes(byte[] encodedQualScores, FastQFormatType formatType)
        {
            int baseValue = GetAsciiBaseValue(formatType);
            long count = encodedQualScores.LongLength();
            sbyte[] result = new sbyte[count];
            for (long i = 0; i < count; i++)
            {
                result[i] = (sbyte)DecodeQualityScore(encodedQualScores[i], baseValue);
            }

            return result;
        }

        /// <summary>
        /// Decodes the specified encoded quality score using base value.
        /// </summary>
        /// <param name="encodedQualityScore">Encoded quality score.</param>
        /// <param name="baseValue">Base value used for encoding.</param>
        private static int DecodeQualityScore(byte encodedQualityScore, int baseValue)
        {
            return encodedQualityScore - baseValue;
        }

        /// <summary>
        /// Encodes the specified quality score using base value.
        /// </summary>
        /// <param name="qualityScore">Quality score.</param>
        /// <param name="baseValue">Base value to use for encoding.</param>
        private static byte EncodeQualityScore(int qualityScore, int baseValue)
        {
            return (byte)(qualityScore + baseValue);
        }

        /// <summary>
        /// Gets the ASCII encoded quality score for the given quality score.
        /// </summary>
        /// <param name="qualScore">Quality Score.</param>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>ASCII encoded quality score.</returns>
        private static byte GetEncodedQualScore(int qualScore, FastQFormatType formatType)
        {
            return EncodeQualityScore(qualScore, GetAsciiBaseValue(formatType));
        }

        /// <summary>
        /// Gets the ASCII encoded quality scores for the given quality score.
        /// </summary>
        /// <param name="qualScores">Quality Score.</param>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>ASCII encoded quality scores.</returns>
        private static byte[] GetEncodedQualScores(int[] qualScores, FastQFormatType formatType)
        {
            int baseValue = GetAsciiBaseValue(formatType);
            long count = qualScores.LongLength();
            byte[] result = new byte[count];
            for (long i = 0; i < count; i++)
            {
                result[i] = EncodeQualityScore(qualScores[i], baseValue);
            }

            return result;
        }

        /// <summary>
        /// Gets the ASCII encoded quality scores for the given quality score.
        /// </summary>
        /// <param name="qualScores">Quality Score.</param>
        /// <param name="formatType">FastQ format type.</param>
        /// <returns>ASCII encoded quality scores.</returns>
        private static byte[] GetEncodedQualScores(sbyte[] qualScores, FastQFormatType formatType)
        {
            int baseValue = GetAsciiBaseValue(formatType);
            long count = qualScores.LongLength();
            byte[] result = new byte[count];
            for (long i = 0; i < count; i++)
            {
                result[i] = EncodeQualityScore(qualScores[i], baseValue);
            }

            return result;
        }

        /// <summary>
        /// Converts quality score from fromQualityScore type to toQualityScore type
        /// Ex: Phred to Solexa or Solexa to Phred
        /// </summary>
        /// <param name="fromQualityScoreType">from quality score type.</param>
        /// <param name="toQualityScoreType">to quality score type.</param>
        /// <param name="qualScore">Quality score</param>
        private static int Convert(BaseQualityScoreType fromQualityScoreType, BaseQualityScoreType toQualityScoreType, int qualScore)
        {
            int result;
            if (fromQualityScoreType == toQualityScoreType)
            {
                result = qualScore;
            }
            else
            {
                if (fromQualityScoreType == BaseQualityScoreType.PhredBaseQualityScore)
                {
                    result = ConvertPhredToSolexa(qualScore);
                }
                else
                {
                    result = ConvertSolexaToPhred(qualScore);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts quality scores from fromQualityScore type to toQualityScore type
        /// Ex: Phred to Solexa or Solexa to Phred
        /// </summary>
        /// <param name="fromQualityScoreType">from quality score type.</param>
        /// <param name="toQualityScoreType">to quality score type.</param>
        /// <param name="qualScores">Quality scores</param>
        private static sbyte[] Convert(BaseQualityScoreType fromQualityScoreType, BaseQualityScoreType toQualityScoreType, sbyte[] qualScores)
        {
            long count = qualScores.LongLength();
            sbyte[] result = new sbyte[count];
            if (fromQualityScoreType == toQualityScoreType)
            {
                Helper.Copy(qualScores, result, qualScores.LongLength());
            }
            else
            {
                if (fromQualityScoreType == BaseQualityScoreType.PhredBaseQualityScore)
                {
                    for (long i = 0; i < count; i++)
                    {
                        result[i] = (sbyte)ConvertPhredToSolexa(qualScores[i]);
                    }
                }
                else
                {
                    for (long i = 0; i < count; i++)
                    {
                        result[i] = (sbyte)ConvertSolexaToPhred(qualScores[i]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Converts quality scores from fromQualityScore type to toQualityScore type
        /// Ex: Phred to Solexa or Solexa to Phred
        /// </summary>
        /// <param name="fromQualityScoreType">from quality score type.</param>
        /// <param name="toQualityScoreType">to quality score type.</param>
        /// <param name="qualScores">Quality scores</param>
        private static int[] Convert(BaseQualityScoreType fromQualityScoreType, BaseQualityScoreType toQualityScoreType, int[] qualScores)
        {
            long count = qualScores.LongLength();
            int[] result = new int[count];
            if (fromQualityScoreType == toQualityScoreType)
            {
                Helper.Copy(qualScores, result, qualScores.LongLength());
            }
            else
            {
                if (fromQualityScoreType == BaseQualityScoreType.PhredBaseQualityScore)
                {
                    for (long i = 0; i < count; i++)
                    {
                        result[i] = ConvertPhredToSolexa(qualScores[i]);
                    }
                }
                else
                {
                    for (long i = 0; i < count; i++)
                    {
                        result[i] = ConvertSolexaToPhred(qualScores[i]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Converts Phred quality score to Solexa quality score.
        /// </summary>
        /// <param name="phredQualityScore">Phred quality score.</param>
        private static int ConvertPhredToSolexa(int phredQualityScore)
        {
            int result;
            if (phredQualityScore == Phred_MinQualityScore)
            {
                result = Solexa_MinQualityScore;
            }
            else
            {
                result = Math.Min(Solexa_MaxQualityScore, Math.Max(Solexa_MinQualityScore, (int)Math.Round(10 * Math.Log10(Math.Pow(10, (phredQualityScore / 10.0)) - 1), 0)));
            }

            return result;
        }

        /// <summary>
        /// Converts Solexa quality score to Phred quality score.
        /// </summary>
        /// <param name="solexaQualityScore">Solexa quality score.</param>
        private static int ConvertSolexaToPhred(int solexaQualityScore)
        {
            int result;
            if (solexaQualityScore == Solexa_MinQualityScore)
            {
                result = Phred_MinQualityScore;
            }
            else
            {
                result = Math.Min(Phred_MaxQualityScore, Math.Max(Phred_MinQualityScore, (int)Math.Round(10 * Math.Log10(Math.Pow(10, (solexaQualityScore / 10.0)) + 1), 0)));
            }

            return result;
        }

        /// <summary>
        /// Validates whether the specified encoded quality scores are within the FastQFormatType limit or not.
        /// </summary>
        /// <param name="encodedQualScore">Encoded quality scores.</param>
        /// <param name="formatType">Fastq format type.</param>
        /// <param name="invalidQualScore">returns invalid encoded quality score if found.</param>
        /// <returns>Returns true if the specified encoded quality scores are with in the limit, otherwise false.</returns>
        private static bool ValidateQualScores(byte[] encodedQualScore, FastQFormatType formatType, out byte invalidQualScore)
        {
            bool result = true;
            invalidQualScore = 0;
            int minScore = GetMinEncodedQualScore(formatType);
            int maxScore = GetMaxEncodedQualScore(formatType);
            long count = encodedQualScore.LongLength();
            for (long index = 0; index < count; index++)
            {
                byte qualScore = encodedQualScore[index];
                if (qualScore < minScore || qualScore > maxScore)
                {
                    result = false;
                    invalidQualScore = qualScore;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Validates whether the specified quality scores are within the FastQFormatType limit or not.
        /// </summary>
        /// <param name="qualScores">Quality scores in base type.</param>
        /// <param name="formatType">Fastq format type.</param>
        /// <param name="invalidQualScore">returns invalid quality score if found.</param>
        /// <returns>Returns true if the specified quality scores are with in the limit, otherwise false.</returns>
        private static bool ValidateQualScores(sbyte[] qualScores, FastQFormatType formatType, out sbyte invalidQualScore)
        {
            bool result = true;
            invalidQualScore = 0;
            int minScore = GetDecodedQualScore(GetMinEncodedQualScore(formatType), formatType);
            int maxScore = GetDecodedQualScore(GetMaxEncodedQualScore(formatType), formatType);
            long count = qualScores.LongLength();
            for (long index = 0; index < count; index++)
            {
                sbyte qualScore = qualScores[index];
                if (qualScore < minScore || qualScore > maxScore)
                {
                    result = false;
                    invalidQualScore = qualScore;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Validates whether the specified quality scores are within the FastQFormatType limit or not.
        /// </summary>
        /// <param name="qualScores">Quality scores in base type.</param>
        /// <param name="formatType">Fastq format type.</param>
        /// <param name="invalidQualScore">returns invalid quality score if found.</param>
        /// <returns>Returns true if the specified quality scores are with in the limit, otherwise false.</returns>
        private static bool ValidateQualScores(int[] qualScores, FastQFormatType formatType, out int invalidQualScore)
        {
            bool result = true;
            invalidQualScore = 0;
            int minScore = GetDecodedQualScore(GetMinEncodedQualScore(formatType), formatType);
            int maxScore = GetDecodedQualScore(GetMaxEncodedQualScore(formatType), formatType);
            long count = qualScores.LongLength();
            for (long index = 0; index < count; index++)
            {
                int qualScore = qualScores[index];
                if (qualScore < minScore || qualScore > maxScore)
                {
                    result = false;
                    invalidQualScore = qualScore;
                    break;
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Base quality score type used in FastQ formats
        /// </summary>
        private enum BaseQualityScoreType
        {
            /// <summary>
            /// Phred base quality score.
            /// </summary>
            PhredBaseQualityScore = 0,

            /// <summary>
            /// Solexa base quality score
            /// </summary>
            SolexaBaseQualityScore
        }

    }
}
