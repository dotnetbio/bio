using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Bio.IO.FastA;
using Bio.IO.FastQ;

namespace Bio.IO
{
    /// <summary>
    /// Support for reading .fa.gz files.
    /// </summary>
    public class GZipFastAParser : GZipSequenceParser<FastAParser, ISequence>
    {
    }

    /// <summary>
    /// Support for reading .fastq.gz files.
    /// </summary>
    public class GZipFastQParser : GZipSequenceParser<FastQParser, IQualitativeSequence>
    {
    }

    /// <summary>
    /// .GZ based ISequenceParser
    /// </summary>
    /// <typeparam name="T">Parser type</typeparam>
    /// <typeparam name="TItem">Item type</typeparam>
    public class GZipSequenceParser<T, TItem> :
        GZipParser<T, TItem>, IParserWithAlphabet<TItem>
        where T : class, IParserWithAlphabet<TItem>, new()
    {
        /// <summary>
        /// Gets or sets the alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet
        {
            get
            {
                return this.Parser.Alphabet;
            }
            set
            {
                this.Parser.Alphabet = value;
            }
        }
    }

	/// <summary>
	/// A parser which unzips the file and then passes the work off to an inner parser.
	/// </summary>
	public class GZipParser<TP, T> : IParser<T> 
        where TP : class, IParser<T>, new()
	{
        /// <summary>
        /// Inner parser
        /// </summary>
	    protected readonly TP Parser;

        /// <summary>
        /// Default constructor - creates inner parser.
        /// </summary>
	    public GZipParser()
	    {
	        this.Parser = new TP();
	    }

        /// <summary>
        /// Constructor which takes the parser as input.
        /// </summary>
        /// <param name="parser">Parser to use.</param>
	    public GZipParser(TP parser)
	    {
	        this.Parser = parser;
	    }

	    /// <summary>
		/// Initializes a new instance of the FastAParser class by 
		/// loading the specified filename and ensuring that it ends with ".gz"
		/// </summary>
		/// <param name="filename">Name of the File.</param>
	    public bool CanProcessFile(string filename)
	    {
            if (!Util.Helper.FileEndsWithZippedExtension(filename))
                return false;

            filename = filename.Substring(0, filename.Length - Util.Helper.ZippedFileExtension.Length);

	        int extensionDelimiter = filename.LastIndexOf('.');
            if (-1 < extensionDelimiter)
            {
                string fileExtension = filename.Substring(extensionDelimiter);
                string validExtensions = this.Parser.SupportedFileTypes;
                string[] extensions = validExtensions.Split(',');
                return extensions.Any(extension => fileExtension.Equals(extension, StringComparison.CurrentCultureIgnoreCase));
            }

	        return false;
	    }

	    /// <summary>
	    /// Parses a list of biological sequence texts from a given stream.
	    /// </summary>
	    /// <returns>The collection of parsed ISequence objects.</returns>
	    public IEnumerable<T> Parse(Stream stream)
	    {
            using (var gz = new GZipStream(stream, CompressionMode.Decompress))
            {
                foreach (var seq in this.Parser.Parse(gz))
                {
                    yield return seq;
                }
            }

	    }

        /// <summary>
        /// Parse a single data sequence from the file.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Data</returns>
        public T ParseOne(Stream stream)
        {
            return Parse(stream).FirstOrDefault();
        }

	    /// <summary>
	    /// Gets the name of the parser being implemented. 
	    /// This is intended to give the developer name of the parser.
	    /// </summary>
	    public string Name { get { return this.Parser.Name; } }

	    /// <summary>
	    /// Gets the description of the parser being
	    /// implemented. This is intended to give the
	    /// developer some information of the parser.
	    /// </summary>
        public string Description { get { return this.Parser.Description; } }

	    /// <summary>
	    /// Gets the file extensions that the parser supports.
	    /// If multiple extensions are supported then this property 
	    /// will return a string containing all extensions with a ',' delimited.
	    /// </summary>
        public string SupportedFileTypes { get { return this.Parser.SupportedFileTypes; } }
	}
}

