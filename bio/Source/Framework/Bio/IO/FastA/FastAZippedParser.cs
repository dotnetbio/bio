using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Bio.IO.FastA
{
	/// <summary>
	/// A derivative of the FastA Parser class designed to work with gzipped FastA files.
	/// </summary>
	public class FastAZippedParser : FastAParser
	{
        /// <summary>
        /// Default constructor used to load into SequenceParsers.
        /// </summary>
	    public FastAZippedParser()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the FastAParser class by 
		/// loading the specified filename and ensuring that it ends with ".gz"
		/// </summary>
		/// <param name="filename">Name of the File.</param>
		public FastAZippedParser(string filename) : base(filename)
		{
			if(!Util.Helper.IsZippedFasta(filename))
			{
				throw new ArgumentException("Attempted to parse file: " + filename 
                    + " but extension did not end in " + Util.Helper.ZippedFileExtension
                    + " or was not recognized as FastA file");
			}
		}	                       

		/// <summary>
		/// Returns an IEnumerable of sequences in the file being parsed.  
		/// </summary>
		/// <returns>Returns ISequence arrays.</returns>
		public override IEnumerable<ISequence> Parse ()
		{
			byte[] buffer = new byte[BufferSize];
			using (GZipStream gz = new GZipStream((new FileInfo(this.Filename)).OpenRead(), CompressionMode.Decompress)) 
            {
				using (StreamReader reader = new StreamReader(gz)) 
                {
					do 
                    {
						var seq = this.ParseOne (reader, buffer);
						if (seq != null)
							yield return seq;
					} 
                    while (!reader.EndOfStream);
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
                return Properties.Resource.ZippedFASTAParserDescription;
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
                return Properties.Resource.ZippedFASTAName;
            }
        }

	    /// <summary>
	    /// Gets a comma separated values of the possible FastA
	    /// file extensions.
	    /// </summary>
	    public override string SupportedFileTypes
        {
            get
            {
                return String.Join(",", base.SupportedFileTypes.Split(',').Select(x => x + Util.Helper.ZippedFileExtension));
            }
        }
	}
}

