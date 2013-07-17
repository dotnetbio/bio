using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Bio.IO.FastQ
{
	/// <summary>
	/// A derivative of the FastQ parser class designed to work with gzipped FastQ files.
	/// </summary>
	public class FastQZippedParser : FastQParser
	{
        /// <summary>
        /// Default constructor used to load parser into SequenceParsers list.
        /// </summary>
        public FastQZippedParser()
        {
        }

		/// <summary>
		/// Initializes a new instance of the FastAParser class by 
		/// loading the specified filename and ensuring that it ends with ".gz"
		/// </summary>
		/// <param name="filename">Name of the File.</param>
		public FastQZippedParser(string filename) : base(filename)
		{
			if(!Util.Helper.IsZippedFastQ(filename))
			{
				throw new ArgumentException("Attempted to parse file: " + filename 
                    + " but extension did not end in " + Util.Helper.ZippedFileExtension 
                    + " or was not recognized as FASTQ file");
			}
		}	                       

		/// <summary>
		/// Returns an IEnumerable of sequences in the file being parsed.  
		/// </summary>
		/// <returns>Returns ISequence arrays.</returns>
		public override IEnumerable<QualitativeSequence> Parse ()
		{
			using (GZipStream gz = new GZipStream((new FileInfo(Filename)).OpenRead(), CompressionMode.Decompress)) 
            {
				using (StreamReader streamReader = new StreamReader(gz))
				{
					FastQFormatType formatType = this.FormatType;
					do
					{
						var seq = ParseOne(streamReader, formatType);
						if (seq != null)
							yield return seq;
					}
					while (!streamReader.EndOfStream);
				}
			}
		}

	    /// <summary>
	    /// Gets the description of the parser.
	    /// This is intended to give developers some information 
	    /// of the parser class. This property returns a simple description of what this
	    ///  class achieves.
	    /// </summary>
	    public override string Description
        {
            get
            {
                return Properties.Resource.ZippedFASTQDescription;
            }
        }

	    /// <summary>
	    /// Gets the type of parser.
	    /// This is intended to give developers name of the parser.
	    /// </summary>
	    public override string Name
        {
            get
            {
                return Properties.Resource.ZippedFASTQName;
            }
        }

	    /// <summary>
	    /// Gets a comma separated values of the possible FastQ
	    /// file extensions.
	    /// </summary>
	    public override string SupportedFileTypes
        {
            get
            {
                var unzipped = base.SupportedFileTypes.Split(',').Select(x => x + Bio.Util.Helper.ZippedFileExtension);
                return String.Join(",", unzipped);
            }
        }
	}
}

