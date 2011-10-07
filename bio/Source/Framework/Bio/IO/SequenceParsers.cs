using System;
using System.Collections.Generic;
using System.Linq;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
using Bio.Util;

namespace Bio.IO
{
    /// <summary>
    /// SequenceParsers class is an abstraction class which provides instances
    /// and lists of all Parsers currently supported by Bio.
    /// </summary>
    public static class SequenceParsers
    {
        #region Member variables
        /// <summary>
        /// A singleton instance of FastAParser class which is capable of
        /// parsing FASTA format files.
        /// </summary>
        private static FastAParser fasta = new FastAParser();

        /// <summary>
        /// A singleton instance of FastQParser class which is capable of
        /// parsing FASTQ format files.
        /// </summary>
        private static FastQParser fastq = new FastQParser();

        /// <summary>
        /// A singleton instance of GffParser class which is capable of
        /// parsing GFF format files.
        /// </summary>
        private static GffParser gff = new GffParser();

        /// <summary>
        /// A singleton instance of GenBankParser class which is capable of
        /// parsing GenBank format files.
        /// </summary>
        private static GenBankParser genBank = new GenBankParser();

        /// <summary>
        /// List of all supported sequence parsers.
        /// </summary>
        private static List<ISequenceParser> all = new List<ISequenceParser>() { fasta, fastq, gff, genBank };
        #endregion

        #region Constructors

        #if (SILVERLIGHT == false)
		    /// <summary>
            /// Initializes static members of the SequenceParsers class.
            /// </summary>
            static SequenceParsers()
            {
                // get the registered parsers
                IList<ISequenceParser> registeredParsers = GetSequenceParsers(true);

                if (null != registeredParsers && registeredParsers.Count > 0)
                {
                    foreach (ISequenceParser parser in registeredParsers)
                    {
                        if (parser != null && all.FirstOrDefault(IA => string.Compare(
                            IA.Name,
                            parser.Name,
                            StringComparison.OrdinalIgnoreCase) == 0) == null)
                        {
                            all.Add(parser);
                        }
                    }

                    registeredParsers.Clear();
                }
            }  
        #endif
        #endregion

        #region Properties
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
        public static IList<ISequenceParser> All
        {
            get
            {
                return all.AsReadOnly();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Finds a suitable parser that supports the specified file, opens the file and returns the parser.
        /// </summary>
        /// <param name="fileName">File name for which the parser is required.</param>
        /// <returns>If found returns the parser as ISequenceParser else returns null.</returns>
        public static ISequenceParser FindParserByFileName(string fileName)
        {
            ISequenceParser parser = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (IsFasta(fileName))
                {
                    parser = new FastAParser(fileName);
                }
                else if (IsFastQ(fileName))
                {
                    parser = new FastQParser(fileName);
                }
                else if (IsGenBank(fileName))
                {
                    parser = new GenBankParser(fileName);
                }
                else if (fileName.EndsWith(Properties.Resource.GFF_FILEEXTENSION, StringComparison.InvariantCultureIgnoreCase))
                {
                    parser = new GffParser(fileName);
                }
                else
                {
                    parser = null;
                }
            }

            return parser;
        }

        /// <summary>
        /// Returns parser which supports the specified file.
        /// </summary>
        /// <param name="fileName">File name for which the parser is required.</param>
        /// <param name="parserName">Name of the parser to use.</param>
        /// <returns>If found returns the parser as IParser else returns null.</returns>
        public static ISequenceParser FindParserByName(string fileName, string parserName)
        {
            ISequenceParser parser = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (parserName == Properties.Resource.FastAName)
                {
                    parser = new FastAParser(fileName);
                }
                else if (parserName == Properties.Resource.FastQName)
                {
                    parser = new FastQParser(fileName);
                }
                else if (parserName == Properties.Resource.GENBANK_NAME)
                {
                    parser = new GenBankParser(fileName);
                }
                else if (parserName == Properties.Resource.GFF_NAME)
                {
                    parser = new GffParser(fileName);
                }
                else
                {
                    parser = null;
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

        #if (SILVERLIGHT == false)
            /// <summary>
            /// Gets all registered parsers in core folder and addins (optional) folders.
            /// </summary>
            /// <param name="includeAddinFolder">Include add-ins folder or not.</param>
            /// <returns>List of registered parsers.</returns>
            private static IList<ISequenceParser> GetSequenceParsers(bool includeAddinFolder)
            {
                IList<ISequenceParser> registeredParsers = new List<ISequenceParser>();

                if (includeAddinFolder)
                {
                    IList<ISequenceParser> addInParsers;
                    if (null != Bio.Registration.RegisteredAddIn.AddinFolderPath)
                    {
                        addInParsers = Bio.Registration.RegisteredAddIn.GetInstancesFromAssemblyPath<ISequenceParser>(Bio.Registration.RegisteredAddIn.AddinFolderPath, Bio.Registration.RegisteredAddIn.DLLFilter);
                        if (null != addInParsers && addInParsers.Count > 0)
                        {
                            foreach (ISequenceParser parser in addInParsers)
                            {
                                if (parser != null && registeredParsers.FirstOrDefault(IA => string.Compare(
                                    IA.Name, parser.Name, StringComparison.OrdinalIgnoreCase) == 0) == null)
                                {
                                    registeredParsers.Add(parser);
                                }
                            }
                        }
                    }
                }

                return registeredParsers;
            }  
        #endif
        #endregion
    }
}
