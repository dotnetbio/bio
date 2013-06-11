using System;
using System.IO;
using System.IO.Compression;
namespace Bio.IO.FastQ
{
	/// <summary>
	/// A derivative of the FastQ parser class designed to work with gzipped FastQ files.
	/// </summary>
	public class FastQZippedParser : FastQParser
	{
		/// <summary>
		/// Initializes a new instance of the FastAParser class by 
		/// loading the specified filename and ensuring that it ends with ".gz"
		/// </summary>
		/// <param name="filename">Name of the File.</param>
		public FastQZippedParser(string filename) : base(filename)
		{
			if(!Bio.Util.Helper.IsZippedFastQ(filename))
			{
				throw new ArgumentException("Attempted to parse file: "+filename+" but extension did not end in "+Bio.Util.Helper.ZippedFileExtension +" or was not recognized as FASTQ file");
			}
		}	                       

		/// <summary>
		/// Returns an IEnumerable of sequences in the file being parsed.  
		/// </summary>
		/// <returns>Returns ISequence arrays.</returns>
		public override System.Collections.Generic.IEnumerable<Bio.QualitativeSequence> Parse ()
		{

			using (GZipStream gz = new GZipStream((new FileInfo(Filename)).OpenRead(), CompressionMode.Decompress)) {
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
	}
}

