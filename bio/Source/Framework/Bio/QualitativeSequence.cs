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
        /// Minimum quality score for Sanger type.
        /// </summary>
        public const byte SangerMinQualScore = 33;

        /// <summary>
        /// Minimum quality score for Solexa type.
        /// </summary>
        public const byte SolexaMinQualScore = 59;

        /// <summary>
        /// Minimum quality score for Illumina type.
        /// </summary>
        public const byte IlluminaMinQualScore = 64;

        /// <summary>
        /// Maximum quality score for Sanger type.
        /// </summary>
        public const byte SangerMaxQualScore = 126;

        /// <summary>
        /// Maximum quality score for Solexa type.
        /// </summary>
        public const byte SolexaMaxQualScore = 126;

        /// <summary>
        /// Maximum quality score for Illumina type.
        /// </summary>
        public const byte IlluminaMaxQualScore = 126;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Sanger format.
        /// </summary>
        private const int SangerAsciiBaseValue = 33;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Solexa/Illumina 1.0 format.
        /// </summary>
        private const int SolexaAsciiBaseValue = 64;

        /// <summary>
        /// ASCII Base value for encoding quality scores in Illumina 1.3 format.
        /// </summary>
        private const int IlluminaAsciiBaseValue = 64;

        /// <summary>
        /// Default quality score.
        /// </summary>
        private const int DefaultQualScore = 60;

        /// <summary>
        /// Holds sequence data.
        /// </summary>
        private byte[] sequenceData;

        /// <summary>
        /// Holds quality scores.
        /// </summary>
        private byte[] qualityScores;

        /// <summary>
        /// Metadata is features or references or related things of a sequence.
        /// </summary>
        private Dictionary<string, object> metadata;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// byte array representing symbols and quality scores.
        /// Sequence and quality scores are validated with the specified alphabet and specified fastq format respectively.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">An array of bytes representing the symbols.</param>
        /// <param name="qualityScores">An array of bytes representing the quality scores.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, byte[] sequence, byte[] qualityScores)
            : this(alphabet, fastQFormatType, sequence, qualityScores, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// byte array representing symbols and quality scores.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">An array of bytes representing the symbols.</param>
        /// <param name="qualityScores">An array of bytes representing the quality scores.</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, byte[] sequence, byte[] qualityScores, bool validate)
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
                // Validate sequence data
                if (!this.Alphabet.ValidateSequence(sequence, 0, sequence.LongLength()))
                {
                    throw new ArgumentOutOfRangeException("sequence");
                }

                // Validate quality scores
                if (!ValidateQualScore(qualityScores, this.FormatType))
                {
                    throw new ArgumentOutOfRangeException("qualityScores");
                }
            }

            this.sequenceData = new byte[sequence.LongLength()];
            this.qualityScores = new byte[qualityScores.LongLength()];

            #if (SILVERLIGHT == false)
		        Array.Copy(sequence, this.sequenceData, sequence.LongLength);
                Array.Copy(qualityScores, this.qualityScores, qualityScores.LongLength);  
            #else   
                Array.Copy(sequence, this.sequenceData, sequence.Length);
                Array.Copy(qualityScores, this.qualityScores, qualityScores.Length);  
            #endif

            this.Count = this.sequenceData.LongLength();
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// string representing symbols and quality scores.
        /// Sequence and quality scores are validated with the specified alphabet and specified fastq format respectively.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">A string representing the symbols.</param>
        /// <param name="qualityScores">A string representing the quality scores.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, string sequence, string qualityScores)
            : this(alphabet, fastQFormatType, sequence, qualityScores, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the QualitativeSequence class with specified alphabet, quality score type,
        /// string representing symbols and quality scores.
        /// </summary>
        /// <param name="alphabet">Alphabet to which this instance should conform.</param>
        /// <param name="fastQFormatType">FastQ format type.</param>
        /// <param name="sequence">A string representing the symbols.</param>
        /// <param name="qualityScores">A string representing the quality scores.</param>
        /// <param name="validate">If this flag is true then validation will be done to see whether the data is valid or not,
        /// else validation will be skipped.</param>
        public QualitativeSequence(IAlphabet alphabet, FastQFormatType fastQFormatType, string sequence, string qualityScores, bool validate)
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

            if (qualityScores == null)
            {
                throw new ArgumentNullException("qualityScores");
            }

            this.FormatType = fastQFormatType;
            this.sequenceData = UTF8Encoding.UTF8.GetBytes(sequence);
            this.qualityScores = UTF8Encoding.UTF8.GetBytes(qualityScores);

            if (validate)
            {
                // Validate sequence data
                if (!this.Alphabet.ValidateSequence(this.sequenceData, 0, this.sequenceData.LongLength()))
                {
                    throw new ArgumentOutOfRangeException("sequence");
                }

                // Validate quality scores
                if (!ValidateQualScore(this.qualityScores, this.FormatType))
                {
                    throw new ArgumentOutOfRangeException("qualityScores");
                }
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
        /// Gets an enumerator to the bytes representing quality scores in this sequence.
        /// </summary>
        public IEnumerable<byte> QualityScores
        {
            get
            {
                return this.qualityScores;
            }
        }

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
        /// Converts Sanger quality score to Illumina quality score.
        /// </summary>
        /// <param name="qualScore">Sanger quality score.</param>
        /// <returns>Returns Illumina quality score.</returns>
        public static byte ConvertFromSangerToIllumina(byte qualScore)
        {
            if (!ValidateQualScore(qualScore, FastQFormatType.Sanger))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }

            return (byte)Math.Min(
                IlluminaMaxQualScore,
                GetEncodedQualScore(GetQualScore(qualScore, FastQFormatType.Sanger), FastQFormatType.Illumina));
        }

        /// <summary>
        /// Converts Sanger quality score to Solexa quality score.
        /// </summary>
        /// <param name="qualScore">Sanger quality score.</param>
        /// <returns>Returns Solexa quality score.</returns>
        public static byte ConvertFromSangerToSolexa(byte qualScore)
        {
            if (!ValidateQualScore(qualScore, FastQFormatType.Sanger))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }

            return (byte)Math.Min(
                SolexaMaxQualScore,
              GetEncodedQualScore(ConvertFromPhredToSolexa(GetQualScore(qualScore, FastQFormatType.Sanger)), FastQFormatType.Solexa));
        }

        /// <summary>
        /// Converts Solexa quality score to Sanger quality score.
        /// </summary>
        /// <param name="qualScore">Solexa quality score.</param>
        /// <returns>Returns Sanger quality score.</returns>
        public static byte ConvertFromSolexaToSanger(byte qualScore)
        {
            if (!ValidateQualScore(qualScore, FastQFormatType.Solexa))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }

            return (byte)Math.Min(
                SangerMaxQualScore,
              GetEncodedQualScore(ConvertFromSolexaToPhared(GetQualScore(qualScore, FastQFormatType.Solexa)), FastQFormatType.Sanger));
        }

        /// <summary>
        /// Converts Solexa quality score to Illumina quality score.
        /// </summary>
        /// <param name="qualScore">Solexa quality score.</param>
        /// <returns>Returns Illumina quality score.</returns>
        public static byte ConvertFromSolexaToIllumina(byte qualScore)
        {
            if (!ValidateQualScore(qualScore, FastQFormatType.Solexa))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }

            return (byte)Math.Min(
                IlluminaMaxQualScore,
              GetEncodedQualScore(ConvertFromSolexaToPhared(GetQualScore(qualScore, FastQFormatType.Solexa)), FastQFormatType.Illumina));
        }

        /// <summary>
        /// Converts Illumina quality score to Sanger quality score.
        /// </summary>
        /// <param name="qualScore">Illumina quality score.</param>
        /// <returns>Returns Sanger quality score.</returns>
        public static byte ConvertFromIlluminaToSanger(byte qualScore)
        {
            if (!ValidateQualScore(qualScore, FastQFormatType.Illumina))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }

            return (byte)Math.Min(
                SangerMaxQualScore,
              GetEncodedQualScore(GetQualScore(qualScore, FastQFormatType.Illumina), FastQFormatType.Sanger));
        }

        /// <summary>
        /// Converts Illumina quality score to Solexa quality score.
        /// </summary>
        /// <param name="qualScore">Illumina quality score.</param>
        /// <returns>Returns Solexa quality score.</returns>
        public static byte ConvertFromIlluminaToSolexa(byte qualScore)
        {
            if (!ValidateQualScore(qualScore, FastQFormatType.Illumina))
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                throw new ArgumentOutOfRangeException("qualScore", message);
            }

            return (byte)Math.Min(
                SolexaMaxQualScore,
              GetEncodedQualScore(ConvertFromPhredToSolexa(GetQualScore(qualScore, FastQFormatType.Illumina)), FastQFormatType.Solexa));
        }

        /// <summary>
        /// Converts Sanger quality scores to Solexa quality scores.
        /// </summary>
        /// <param name="qualScores">Sanger quality scores.</param>
        /// <returns>Returns Solexa quality scores.</returns>
        public static byte[] ConvertFromSangerToSolexa(byte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int length = qualScores.Length;
            byte[] newQualScores = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte qualScore = qualScores[i];
                if (!ValidateQualScore(qualScore, FastQFormatType.Sanger))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    throw new ArgumentOutOfRangeException("qualScores", message);
                }

                newQualScores[i] = ConvertFromSangerToSolexa(qualScore);
            }

            return newQualScores;
        }

        /// <summary>
        /// Converts Sanger quality scores to Illumina quality scores.
        /// </summary>
        /// <param name="qualScores">Sanger quality scores.</param>
        /// <returns>Returns Illumina quality scores.</returns>
        public static byte[] ConvertFromSangerToIllumina(byte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int length = qualScores.Length;
            byte[] newQualScores = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte qualScore = qualScores[i];
                if (!ValidateQualScore(qualScore, FastQFormatType.Sanger))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    throw new ArgumentOutOfRangeException("qualScores", message);
                }

                newQualScores[i] = ConvertFromSangerToIllumina(qualScore);
            }

            return newQualScores;
        }

        /// <summary>
        /// Converts Solexa quality scores to Sanger quality scores.
        /// </summary>
        /// <param name="qualScores">Solexa quality scores.</param>
        /// <returns>Returns Sanger quality scores.</returns>
        public static byte[] ConvertFromSolexaToSanger(byte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int length = qualScores.Length;
            byte[] newQualScores = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte qualScore = qualScores[i];
                if (!ValidateQualScore(qualScore, FastQFormatType.Solexa))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    throw new ArgumentOutOfRangeException("qualScores", message);
                }

                newQualScores[i] = ConvertFromSolexaToSanger(qualScore);
            }

            return newQualScores;
        }

        /// <summary>
        /// Converts Solexa quality scores to Illumina quality scores.
        /// </summary>
        /// <param name="qualScores">Solexa quality scores.</param>
        /// <returns>Returns Illumina quality scores.</returns>
        public static byte[] ConvertFromSolexaToIllumina(byte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int length = qualScores.Length;
            byte[] newQualScores = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte qualScore = qualScores[i];
                if (!ValidateQualScore(qualScore, FastQFormatType.Solexa))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    throw new ArgumentOutOfRangeException("qualScores", message);
                }

                newQualScores[i] = ConvertFromSolexaToIllumina(qualScore);
            }

            return newQualScores;
        }

        /// <summary>
        /// Converts Illumina quality scores to Sanger quality scores.
        /// </summary>
        /// <param name="qualScores">Illumina quality scores.</param>
        /// <returns>Returns Sanger quality scores.</returns>
        public static byte[] ConvertFromIlluminaToSanger(byte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int length = qualScores.Length;
            byte[] newQualScores = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte qualScore = qualScores[i];
                if (!ValidateQualScore(qualScore, FastQFormatType.Illumina))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    throw new ArgumentOutOfRangeException("qualScores", message);
                }

                newQualScores[i] = ConvertFromIlluminaToSanger(qualScore);
            }

            return newQualScores;
        }

        /// <summary>
        /// Converts Illumina quality scores to Solexa quality scores.
        /// </summary>
        /// <param name="qualScores">Illumina quality scores.</param>
        /// <returns>Returns Solexa quality scores.</returns>
        public static byte[] ConvertFromIlluminaToSolexa(byte[] qualScores)
        {
            if (qualScores == null)
            {
                throw new ArgumentNullException("qualScores");
            }

            int length = qualScores.Length;
            byte[] newQualScores = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte qualScore = qualScores[i];
                if (!ValidateQualScore(qualScore, FastQFormatType.Illumina))
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    throw new ArgumentOutOfRangeException("qualScores", message);
                }

                newQualScores[i] = ConvertFromIlluminaToSolexa(qualScore);
            }

            return newQualScores;
        }

        /// <summary>
        /// Gets the default quality score for the specified FastQFormatType.
        /// </summary>
        ///  /// <param name="type">FastQ format type.</param>
        /// <returns>Quality score.</returns>
        public static byte GetDefaultQualScore(FastQFormatType type)
        {
            if (type == FastQFormatType.Sanger)
            {
                return (byte)(SangerAsciiBaseValue + DefaultQualScore);
            }
            else if (type == FastQFormatType.Solexa)
            {
                return (byte)(SolexaAsciiBaseValue + DefaultQualScore);
            }
            else
            {
                return (byte)(IlluminaAsciiBaseValue + DefaultQualScore);
            }
        }

        /// <summary>
        /// Gets the maximum quality score for the specified FastQFormatType.
        /// </summary>
        ///  /// <param name="type">FastQ format type.</param>
        /// <returns>Quality score.</returns>
        public static byte GetMaxQualScore(FastQFormatType type)
        {
            if (type == FastQFormatType.Solexa)
            {
                return SolexaMaxQualScore;
            }
            else if (type == FastQFormatType.Sanger)
            {
                return SangerMaxQualScore;
            }
            else
            {
                return IlluminaMaxQualScore;
            }
        }

        /// <summary>
        /// Gets the minimum quality score for the specified FastQFormatType.
        /// </summary>
        /// <param name="type">FastQ format type.</param>
        /// <returns>Quality score.</returns>
        public static byte GetMinQualScore(FastQFormatType type)
        {
            if (type == FastQFormatType.Solexa)
            {
                return SolexaMinQualScore;
            }
            else if (type == FastQFormatType.Sanger)
            {
                return SangerMinQualScore;
            }
            else
            {
                return IlluminaMinQualScore;
            }
        }

        /// <summary>
        /// Gets the quality score found at the specified index if within bounds. Note that the index value start at 0.
        /// </summary>
        /// <param name="index">Index at which the symbol is required.</param>
        /// <returns>Quality Score at the given index.</returns>
        public byte GetQualityScore(long index)
        {
            return this.qualityScores[index];
        }

        /// <summary>
        /// Converts the current instance to the specified FastQ format type 
        /// and returns a new instance of QualitativeSequence.
        /// </summary>
        /// <param name="type">FastQ format type to convert.</param>
        public QualitativeSequence ConvertTo(FastQFormatType type)
        {
            byte[] convertedQualityScores = null;

            if (this.FormatType == type)
            {
                convertedQualityScores = this.qualityScores;
            }

            if (this.FormatType == FastQFormatType.Sanger)
            {
                if (type == FastQFormatType.Illumina)
                {
                    // Sanger to Illumina.
                    convertedQualityScores = ConvertFromSangerToIllumina(this.qualityScores);
                }
                else if (type == FastQFormatType.Solexa)
                {
                    // Sanger To Solexa.
                    convertedQualityScores = ConvertFromSangerToSolexa(this.qualityScores);
                }
            }
            else if (this.FormatType == FastQFormatType.Solexa)
            {
                if (type == FastQFormatType.Sanger)
                {
                    // Solexa to Sanger.
                    convertedQualityScores = ConvertFromSolexaToSanger(this.qualityScores);
                }
                else if (type == FastQFormatType.Illumina)
                {
                    // Solexa to Illumina.
                    convertedQualityScores = ConvertFromSolexaToIllumina(this.qualityScores);
                }
            }
            else
            {
                if (type == FastQFormatType.Sanger)
                {
                    // Illumina to Sanger.
                    convertedQualityScores = ConvertFromIlluminaToSanger(this.qualityScores);
                }
                else if (type == FastQFormatType.Solexa)
                {
                    // Illumina to Solexa.
                    convertedQualityScores = ConvertFromIlluminaToSolexa(this.qualityScores);
                }
            }

            return new QualitativeSequence(this.Alphabet, type, this.sequenceData, convertedQualityScores, false);
        }

        /// <summary>
        /// Return a new QualitativeSequence representing this QualitativeSequence with the orientation reversed.
        /// </summary>
        public ISequence GetReversedSequence()
        {
            byte[] newSequenceData = new byte[this.sequenceData.LongLength()];
            byte[] newQualityScores = new byte[this.qualityScores.LongLength()];

            for (long index = 0; index < this.sequenceData.LongLength(); index++)
            {
                newSequenceData[index] = this.sequenceData[this.sequenceData.LongLength() - index - 1];
                newQualityScores[index] = this.qualityScores[this.qualityScores.LongLength() - index - 1];
            }

            return new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores);
        }

        /// <summary>
        /// Return a new QualitativeSequence representing the complement of this QualitativeSequence.
        /// </summary>
        public ISequence GetComplementedSequence()
        {
            byte[] newSequenceData = new byte[this.sequenceData.LongLength()];
            byte[] newQualityScores = this.qualityScores;

            for (long index = 0; index < this.sequenceData.LongLength(); index++)
            {
                byte complementedSymbol;
                byte symbol = this.sequenceData[index];
                if (!this.Alphabet.TryGetComplementSymbol(symbol, out complementedSymbol))
                {
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentUICulture, "Complement for the symbol {0} Alphabet is not supported.", (char)symbol));
                }

                newSequenceData[index] = complementedSymbol;
            }

            return new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores);
        }

        /// <summary>
        /// Return a new QualitativeSequence representing the reverse complement of this QualitativeSequence.
        /// </summary>
        public ISequence GetReverseComplementedSequence()
        {
            byte[] newSequenceData = new byte[this.sequenceData.LongLength()];
            byte[] newQualityScores = new byte[this.qualityScores.LongLength()];

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

            return new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores);
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
            byte[] newQualityScores = new byte[length];

            for (long index = 0; index < length; index++)
            {
                newSequenceData[index] = this.sequenceData[start + index];
                newQualityScores[index] = this.qualityScores[start + index];
            }

            return new QualitativeSequence(this.Alphabet, this.FormatType, newSequenceData, newQualityScores);
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
                                     new string(this.qualityScores.Take(Helper.AlphabetsToShowInToString).Select(a => (char)a).ToArray()),
                                     (this.Count - Helper.AlphabetsToShowInToString));
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, Properties.Resource.QualitativeSequenceToStringFormatForSmallSequence,
                                     new string(this.sequenceData.Take(this.sequenceData.Length).Select((a => (char) a)).ToArray()),
                                     new string(this.qualityScores.Take(this.qualityScores.Length).Select(a => (char) a).ToArray()));
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
        /// Gets the quality score from the ASCII encoded quality score.
        /// </summary>
        /// <param name="qualScore">ASCII Encoded quality score.</param>
        /// <param name="type">FastQ format type.</param>
        /// <returns>Returns quality score.</returns>
        private static int GetQualScore(byte qualScore, FastQFormatType type)
        {
            if (type == FastQFormatType.Sanger)
            {
                return qualScore - SangerAsciiBaseValue;
            }
            else if (type == FastQFormatType.Solexa)
            {
                return qualScore - SolexaAsciiBaseValue;
            }
            else
            {
                return qualScore - IlluminaAsciiBaseValue;
            }
        }

        /// <summary>
        /// Gets the ASCII encoded quality score for the given quality score.
        /// </summary>
        /// <param name="qualScore">Quality Score.</param>
        /// <param name="type">FastQ format type.</param>
        /// <returns>ASCII encoded quality score.</returns>
        private static byte GetEncodedQualScore(int qualScore, FastQFormatType type)
        {
            if (type == FastQFormatType.Sanger)
            {
                return (byte)(qualScore + SangerAsciiBaseValue);
            }
            else if (type == FastQFormatType.Solexa)
            {
                return (byte)(qualScore + SolexaAsciiBaseValue);
            }
            else
            {
                return (byte)(qualScore + IlluminaAsciiBaseValue);
            }
        }

        /// <summary>
        /// Converts Phred quality score to Solexa quality score.
        /// </summary>
        /// <param name="qualScore">Quality score to be converted.</param>
        /// <returns>Solexa quality score.</returns>
        private static int ConvertFromPhredToSolexa(int qualScore)
        {
            if (qualScore == 0)
            {
                return -5;
            }

            int minQualvalue = GetQualScore(SolexaMinQualScore, FastQFormatType.Solexa);
            int maxQualValue = GetQualScore(SolexaMaxQualScore, FastQFormatType.Solexa);

            return Math.Min(maxQualValue, Math.Max(minQualvalue, (int)Math.Round(10 * Math.Log10(Math.Pow(10, (qualScore / 10.0)) - 1), 0)));
        }

        /// <summary>
        /// Converts Solexa quality score to Phred quality score.
        /// </summary>
        /// <param name="qualScore">Quality score to be converted.</param>
        /// <returns>Phred quality score.</returns>
        private static int ConvertFromSolexaToPhared(int qualScore)
        {
            if (qualScore == -5)
            {
                return 0;
            }

            int minQualvalue = GetQualScore(SangerMinQualScore, FastQFormatType.Sanger);
            int maxQualValue = GetQualScore(SangerMaxQualScore, FastQFormatType.Sanger);

            return Math.Min(maxQualValue, Math.Max(minQualvalue, (int)Math.Round(10 * Math.Log10(Math.Pow(10, (qualScore / 10.0)) + 1), 0)));
        }

        /// <summary>
        /// Validates whether the specified quality score is within the FastQFormatType limit or not.
        /// </summary>
        /// <param name="qualScore">Quality score.</param>
        /// <param name="type">Fastq format type.</param>
        /// <returns>Returns true if the specified quality score is with in the limit, otherwise false.</returns>
        private static bool ValidateQualScore(byte qualScore, FastQFormatType type)
        {
            if (type == FastQFormatType.Sanger)
            {
                return qualScore >= SangerMinQualScore && qualScore <= SangerMaxQualScore;
            }
            else if (type == FastQFormatType.Solexa)
            {
                return qualScore >= SolexaMinQualScore && qualScore <= SolexaMaxQualScore;
            }
            else
            {
                return qualScore >= IlluminaMinQualScore && qualScore <= IlluminaMaxQualScore;
            }
        }

        /// <summary>
        /// Validates whether the specified quality scores are within the FastQFormatType limit or not.
        /// </summary>
        /// <param name="qualScores">Quality scores.</param>
        /// <param name="type">Fastq format type.</param>
        /// <returns>Returns true if the specified quality scores are with in the limit, otherwise false.</returns>
        private static bool ValidateQualScore(byte[] qualScores, FastQFormatType type)
        {
            bool result = true;

            switch (type)
            {
                case FastQFormatType.Sanger:
                    for (long index = 0; index < qualScores.LongLength(); index++)
                    {
                        byte qualScore = qualScores[index];
                        if (qualScore < SangerMinQualScore || qualScore > SangerMaxQualScore)
                        {
                            result = false;
                            break;
                        }
                    }

                    break;
                case FastQFormatType.Solexa:
                    for (long index = 0; index < qualScores.LongLength(); index++)
                    {
                        byte qualScore = qualScores[index];
                        if (qualScore < SolexaMinQualScore || qualScore > SolexaMaxQualScore)
                        {
                            result = false;
                            break;
                        }
                    }

                    break;
                case FastQFormatType.Illumina:
                    for (long index = 0; index < qualScores.LongLength(); index++)
                    {
                        byte qualScore = qualScores[index];
                        if (qualScore < IlluminaMinQualScore || qualScore > IlluminaMaxQualScore)
                        {
                            result = false;
                            break;
                        }
                    }

                    break;
            }

            return result;
        }
        #endregion
    }
}
