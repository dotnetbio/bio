using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.IO.Xsv;

namespace Bio.IO.Snp
{
    /// <summary>
    /// Implements common methods for parsing SNPs from a SnpReader into ISequences. 
    /// This reads Snp items from the SnpReader and stores either of the two alleles
    /// in a sparse sequence at the same position as the chromosome position.
    /// Extending classes have to implement the 
    /// SnpReader GetSnpReader(TextReader reader) method that returns a
    /// SnpReader for the given TextReader.
    /// </summary>
    public abstract class SnpParser : ISequenceParser
    {
        /// <summary>
        /// The alphabet to use for parsed ISequence objects.
        /// </summary>
        public IAlphabet Alphabet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the parser. Intended to be filled in 
        /// by classes deriving from BasicSequenceParser class
        /// with the exact name of the parser type.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the description of the parser. Intended to be filled in 
        /// by classes deriving from BasicSequenceParser class
        /// with the exact details of the parser.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        #region Properties introduced by SnpParser

        /// <summary>
        /// If set to false, this will parse AlleleTwo. If true, this will parse AlleleOne from the SnpReader.
        /// </summary>
        public bool ParseAlleleOne
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        #endregion Properties introduced by SnpParser

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SnpParser class.
        /// </summary>
        /// <param name="alphabet">The Alphabet.</param>
        protected SnpParser(IAlphabet alphabet)
        {
            this.Alphabet = alphabet;
            this.ParseAlleleOne = true;
        }

        /// <summary>
        /// Initializes a new instance of the SnpParser class.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        protected SnpParser(string filename)
        {
            this.Filename = filename;
            this.ParseAlleleOne = true;
        }

        #endregion Constructor

        #region Methods implementing ISequenceParser

        /// <summary>
        /// Gets the filetypes supported by the parser. Intended to be filled in 
        /// by classes deriving from BasicSequenceParser class
        /// with the exact details of the filetypes supported.
        /// </summary>
        public abstract string SupportedFileTypes
        {
            get;
        }

        /// <summary>
        /// Parses a list of sparse sequences from the reader, one per contiguous 
        /// chromosome present in the reader. There is one SequenceItem per SnpItem with 
        /// either of the two alleles in the SnpItem (determined by the ParseAlleleOne property)
        /// and at the same position in the sequence as the SnpItem.Position.
        /// </summary>
        /// <returns>Returns a list of sparse sequences containing Snp items that were read 
        /// from the reader, one sequence per contiguous chromosome number and
        /// retaining the same position in the sequence as the chromosome position.</returns>
        public IEnumerable<ISequence> Parse()
        {
            using (StreamReader txtReader = new StreamReader(this.Filename))
            {
                return this.Parse(txtReader);
            }
        }

        /// <summary>
        /// Parses a list of sparse sequences from the reader, one per contiguous 
        /// chromosome present in the reader. There is one SequenceItem per SnpItem with 
        /// either of the two alleles in the SnpItem (determined by the ParseAlleleOne property)
        /// and at the same position in the sequence as the SnpItem.Position.
        /// </summary>
        /// <param name="reader">Stream to be parsed.</param>
        /// <returns>Returns a list of sparse sequences containing Snp items that were read 
        /// from the reader, one sequence per contiguous chromosome number and
        /// retaining the same position in the sequence as the chromosome position.</returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            if (this.Filename == null)
            {
                throw new FieldAccessException("Filename");
            }


            if (this.Alphabet == null)
            {
                this.Alphabet = AmbiguousDnaAlphabet.Instance;
            }

            ISnpReader snpReader = new XsvSnpReader(reader, new[] { '\t' }, true, true, 0, 1, 2, 3);
            snpReader.MoveNext();

            List<ISequence> sequenceList = new List<ISequence>();
            while (snpReader.Current != null)
                sequenceList.Add(ParseOne(snpReader));
            return sequenceList;
        }

        /// <summary>
        /// Opens the stream for the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            // if the file is already open throw invalid 
            if (!string.IsNullOrEmpty(this.Filename))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.FileAlreadyOpen, this.Filename));
            }

            // Validate the file - by try to open.
            using (new StreamReader(filename))
            {
            }

            this.Filename = filename;
        }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        public void Close()
        {
            this.Filename = null;
        }

        /// <summary>
        /// Disposes the reader.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        #endregion Methods implementing ISequenceParser

        #region Protected Methods of SnpParser
        /// <summary>
        /// The common ParseOne method called for parsing SNPs
        /// NOTE: The snpReader.MoveNext must have already been called and 
        /// the ISnpReader.Current have the first SnpItem to parse into the sequence
        /// </summary>
        /// <param name="snpReader">The ISnpReader to read a Snp chromosome sequence from</param>
        /// <returns>Returns a SparseSequence containing Snp items from the first contiguous 
        /// chromosome number read from the snp reader.</returns>
        protected ISequence ParseOne(ISnpReader snpReader)
        {
            // Check input arguments
            if (snpReader == null) 
            {
                throw new ArgumentNullException("snpReader",Properties.Resource.snpTextReaderNull);
            }

            if (snpReader.Current == null)
                return new SparseSequence(Alphabet)
                {
                    ID = "Empty"
                };

            int sequenceChromosome = snpReader.Current.Chromosome;
            SparseSequence sequence = new SparseSequence(Alphabet);
            sequence.ID = ("Chr" + sequenceChromosome);

            do
            {
                SnpItem snp = snpReader.Current;
                // increase the size of the sparse sequence
                if (sequence.Count <= snp.Position)
                    sequence.Count = snp.Position + 1;
                sequence[snp.Position] = ParseAlleleOne
                             ? (byte)snp.AlleleOne
                             : (byte)snp.AlleleTwo;
            } while (snpReader.MoveNext() && snpReader.Current.Chromosome == sequenceChromosome);

            return sequence;
        }

        #endregion Protected Methods of SnpParser
    }
}
