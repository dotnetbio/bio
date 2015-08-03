using System;
using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bio.IO.PacBio
{
    /// <summary>
    /// Reads the data from a PacBio BAM file produced by the pbccs program. 
    /// </summary>
    public class PacBioCCSBamReader
    {
        /// <summary>
        /// Parse the CCS reads in a PacBio CCS BAM File.
        /// </summary>
        /// <returns>The CCS reads.</returns>
        /// <param name="stream">A stream to parse.</param>
        public static IEnumerable<PacBioCCSRead> Parse(Stream stream) {

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            BAMParser bp = new BAMParser ();
            var header = bp.GetHeader (stream);
            var field = header.RecordFields.ToList();

            var pg = field.Where( p => p.Typecode=="PG").FirstOrDefault();
            if (pg == null) {
                throw new ArgumentException ("BAM file did not contain a 'PG' tag in header");
            }

            var cl = pg.Tags.Where (z => z.Tag == "CL").FirstOrDefault ();
            if (cl == null) {
                throw new ArgumentException ("BAM file did not contain a 'CL' tag within the 'PG' group in header.");
            }

            var cmd = cl.Value;
            if(!cmd.StartsWith("ccs")) {
                throw new ArgumentException("This is not a BAM file produced by the ccs command.");
            }



            stream.Seek(0, SeekOrigin.Begin);
            foreach (var s in bp.Parse (stream)) {
                yield return new PacBioCCSRead (s as SAMAlignedSequence);
            }
        }

    }
}

