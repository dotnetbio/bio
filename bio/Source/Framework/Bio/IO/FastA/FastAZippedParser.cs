using System;
using System.IO;
using System.IO.Compression;

namespace Bio.IO.FastA
{
	/// <summary>
	/// A derivative of the FastA Parser class designed to work with gzipped FastA files.
	/// </summary>
	public class FastAZippedParser : FastAParser
	{
		/// <summary>
		/// Initializes a new instance of the FastAParser class by 
		/// loading the specified filename and ensuring that it ends with ".gz"
		/// </summary>
		/// <param name="filename">Name of the File.</param>
		public FastAZippedParser(string filename) : base(filename)
		{
            
			if(!Bio.Util.Helper.IsZippedFasta(filename))
			{
				throw new ArgumentException("Attempted to parse file: "+filename+" but extension did not end in "+Bio.Util.Helper.ZippedFileExtension+" or was not recognized as FastA file");
			}
		}	                       

		/// <summary>
		/// Returns an IEnumerable of sequences in the file being parsed.  
		/// </summary>
		/// <returns>Returns ISequence arrays.</returns>
		public override System.Collections.Generic.IEnumerable<ISequence> Parse ()
		{
			byte[] buffer = new byte[BufferSize];
			using (GZipStream gz = new GZipStream((new FileInfo(this.Filename)).OpenRead(), CompressionMode.Decompress)) {
				using (StreamReader reader = new StreamReader(gz)) {
					do {
						var seq = this.ParseOne (reader, buffer);
						if (seq != null)
							yield return seq;
					} while (!reader.EndOfStream);
				}
			}
		}
	}
}

