using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
using Bio.Registration;
using Bio.Util;

namespace Bio.IO
{
    /// <summary>
    /// SequenceParsers class is an abstraction class which provides instances
    /// and lists of all Parsers currently supported by Bio.
    /// </summary>
    public static class SequenceParsers
    {
        /// <summary>
        /// A singleton instance of FastAParser class which is capable of
        /// parsing FASTA format files.
        /// </summary>
        private static readonly FastAParser fasta = new FastAParser();

        /// <summary>
        /// A singleton instance of FastQParser class which is capable of
        /// parsing FASTQ format files.
        /// </summary>
        private static readonly FastQParser fastq = new FastQParser();

        /// <summary>
        /// A singleton instance of GffParser class which is capable of
        /// parsing GFF format files.
        /// </summary>
        private static readonly GffParser gff = new GffParser();

        /// <summary>
        /// A singleton instance of GenBankParser class which is capable of
        /// parsing GenBank format files.
        /// </summary>
        private static readonly GenBankParser genBank = new GenBankParser();

        /// <summary>
        /// List of all supported sequence parsers.
        /// </summary>
        private static readonly List<ISequenceParser> allParsers = new List<ISequenceParser> { fasta, new SequenceParserDecorator<FastQParser,IQualitativeSequence>(fastq), gff, genBank };

        /// <summary>
        /// Initializes static members of the SequenceParsers class.
        /// </summary>
        static SequenceParsers()
		{
            // get the registered parsers
            IEnumerable<ISequenceParser> registeredParsers = GetSequenceParsers();
            if (null != registeredParsers)
            {
                foreach (ISequenceParser parser in registeredParsers.Where(
                    parser => parser != null && All.All(sp => string.Compare(sp.Name, parser.Name, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    allParsers.Add(parser);
                }
            }
        }

        /// <summary>
        /// Gets an instance of FastaParser class which is capable of
        /// parsing FASTA format files.
        /// </summary>
        public static FastAParser Fasta
        {
            get
            {
                return fasta;
            }
        }

        /// <summary>
        /// Gets an instance of FastQParser class which is capable of
        /// parsing FASTQ format files.
        /// </summary>
        public static FastQParser FastQ
        {
            get
            {
                return fastq;
            }
        }

        /// <summary>
        /// Gets an instance of GffParser class which is capable of
        /// parsing GFF format files.
        /// </summary>
        public static GffParser Gff
        {
            get
            {
                return gff;
            }
        }

        /// <summary>
        /// Gets an instance of GenBankParser class which is capable of
        /// parsing GenBank format files.
        /// </summary>
        public static GenBankParser GenBank
        {
            get
            {
                return genBank;
            }
        }

        /// <summary>
        /// Gets the list of all parsers which is supported by the framework.
        /// </summary>
        public static IReadOnlyList<ISequenceParser> All
        {
            get { return allParsers; }
        }

        /// <summary>
        /// Finds a suitable parser that supports the specified file, opens the file and returns the parser.
        /// </summary>
        /// <param name="fileName">File name for which the parser is required.</param>
        /// <returns>If found returns the open parser as ISequenceParser else returns null.</returns>
        public static ISequenceParser FindParserByFileName(string fileName)
        {
            ISequenceParser parser = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (IsFasta(fileName))
                {
                    parser = fasta;
                }
                else if (IsFastQ(fileName))
                {
                    parser = new SequenceParserDecorator<FastQParser, IQualitativeSequence>(fastq);
                }
                else if (IsGenBank(fileName))
                {
                    parser = genBank;
                }
                else if (fileName.EndsWith(Properties.Resource.GFF_FILEEXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    parser = gff;
                }
                else
                {
                    // Do a search through the known parsers to pick up custom parsers added through add-in.
                    string fileExtension = Path.GetExtension(fileName);
                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        parser = All.FirstOrDefault(p => p.SupportedFileTypes.Contains(fileExtension));
                    }
                }
            }

            return parser;
        }

        /// <summary>
        /// Returns parser which supports the specified file.
        /// </summary>
        /// <param name="fileName">File name for which the parser is required.</param>
        /// <param name="parserName">Name of the parser to use.</param>
        /// <returns>If found returns the open parser as ISequenceParser else returns null.</returns>
        public static ISequenceParser FindParserByName(string fileName, string parserName)
        {
            ISequenceParser parser = null;

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(parserName))
            {
                if (parserName == Properties.Resource.FastAName)
                {
                    parser = fasta;
                }
                else if (parserName == Properties.Resource.FastQName)
                {
                    parser = new SequenceParserDecorator<FastQParser, IQualitativeSequence>(fastq);
                }
                else if (parserName == Properties.Resource.GENBANK_NAME)
                {
                    parser = genBank;
                }
                else if (parserName == Properties.Resource.GFF_NAME)
                {
                    parser = gff;
                }
                else
                {
                    // Do a search through the known parsers to pick up custom parsers added through add-in.
                    parser = All.FirstOrDefault(p => p.Name == parserName);
                }
            }

            return parser;
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for FASTA formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid fasta file extension.
        /// False : if it is a in-valid fasta file extension.
        /// </returns>
        public static bool IsFasta(string fileName)
        {
            return Helper.IsFasta(fileName);
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for FastQ formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid fastq file extension.
        /// False : if it is a in-valid fastq file extension.
        /// </returns>
        public static bool IsFastQ(string fileName)
        {
            return Helper.IsFastQ(fileName);
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for GenBank formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid GenBank file extension.
        /// False : if it is a in-valid GenBank file extension.
        /// </returns>
        public static bool IsGenBank(string fileName)
        {
            return Helper.IsGenBank(fileName);
        }

        /// <summary>
        /// Gets all registered parsers in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered parsers.</returns>
        private static IEnumerable<ISequenceParser> GetSequenceParsers()
        {
            var registeredParsers = new List<ISequenceParser>();
            var implementations = BioRegistrationService.LocateRegisteredParts<ISequenceParser>();

            foreach (var impl in implementations)
            {
                try
                {
                    ISequenceParser parser = Activator.CreateInstance(impl) as ISequenceParser;
                    if (parser != null)
                        registeredParsers.Add(parser);
                }
                catch
                {
                    // Cannot create - no default ctor?
                }
            }
            return registeredParsers;
        }  
    }
}
