using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bio.IO.Bed
{
    /// <summary>
    /// Parses ISequenceRange objects that are formatted according to the BED
    /// format. This is a format of tab delimited text where the each line
    /// represents one range and each defines an ID, start and stop index.
    /// 
    /// Info for the specification of this format can be found at:
    /// http://genome.ucsc.edu/FAQ/FAQformat
    /// 
    /// In this format indices start their count from zero. The chromosome start
    /// index is inclusive and the chromosome end index is exclusive.
    /// 
    /// There are three required fields in each line as described above. There
    /// are also 9 option fields. These are: name, score, strand, thickStartm
    /// thickEnd, itemRgb, blockCount, blockSizes, and blockStarts.
    /// 
    /// This parser does not support the bigBED format.
    /// </summary>
    public class BedParser : ISequenceRangeParser
    {
        #region ISequenceRangeParser Members

        /// <summary>
        /// Parse a set of ISequenceRange objects from a file.
        /// </summary>
        /// <param name="fileName">The file to parse.</param>
        /// <returns>The list of sequence ranges.</returns>
        public IList<ISequenceRange> ParseRange(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return ParseRange(new StreamReader(stream));
            }
        }

        /// <summary>
        /// Parse a set of ISequenceRange objects into a SequenceRange
        /// grouping from a file.
        /// </summary>
        /// <param name="fileName">The file to parse.</param>
        /// <returns>The sequence range groups.</returns>
        public SequenceRangeGrouping ParseRangeGrouping(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return ParseRangeGrouping(new StreamReader(stream));
            }
        }

        /// <summary>
        /// Parse a set of ISequenceRange objects from a reader.
        /// </summary>
        /// <param name="reader">The reader from which the sequence range is to be parsed.</param>
        /// <returns>The list of sequence ranges.</returns>
        public IList<ISequenceRange> ParseRange(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            List<ISequenceRange> result = new List<ISequenceRange>();
            char[] splitters = { '\t' };

            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                // TODO: Handle Track definitions
                if (line.StartsWith("track", StringComparison.Ordinal))
                    continue;

                string[] split = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 3)
                    continue;
                
                SequenceRange range = new SequenceRange();
                range.ID = split[0];
                range.Start = long.Parse(split[1], CultureInfo.InvariantCulture);
                range.End = long.Parse(split[2], CultureInfo.InvariantCulture);

                // Optional parameters
                // TODO: When implementing track definitions update this section for 'use{feature}' declarations
                if (split.Length >= 4)
                    range.Metadata["Name"] = split[3];
                if (split.Length >= 5)
                    range.Metadata["Score"] = int.Parse(split[4], CultureInfo.InvariantCulture);
                if (split.Length >= 6)
                    range.Metadata["Strand"] = split[5][0];
                if (split.Length >= 7)
                    range.Metadata["ThickStart"] = int.Parse(split[6], CultureInfo.InvariantCulture);
                if (split.Length >= 8)
                    range.Metadata["ThickEnd"] = int.Parse(split[7], CultureInfo.InvariantCulture);
                if (split.Length >= 9)
                    range.Metadata["ItemRGB"] = split[8];
                if (split.Length >= 10)
                    range.Metadata["BlockCount"] = int.Parse(split[9], CultureInfo.InvariantCulture);
                if (split.Length >= 11)
                    range.Metadata["BlockSizes"] = split[10];
                if (split.Length >= 12)
                    range.Metadata["BlockStarts"] = split[11];

                result.Add(range);
            }

            reader.Close();

            return result;
        }

        /// <summary>
        /// Parse a set of ISequenceRange objects into a SequenceRange
        /// grouping from a reader.
        /// </summary>
        /// <param name="reader">The reader from which the sequence range is to be parsed.</param>
        /// <returns>The sequence range groups.</returns>
        public SequenceRangeGrouping ParseRangeGrouping(TextReader reader)
        {
            SequenceRangeGrouping result = new SequenceRangeGrouping(ParseRange(reader));

            if (null == result.GroupIDs || 0 == result.GroupIDs.Count())
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                throw new InvalidDataException(message);
            }

            return result;
        }

        /// <summary>
        /// The name of this parser: BED
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.BedName; }
        }

        /// <summary>
        /// A short description of the BED parser
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.BedDesc; }
        }

        /// <summary>
        /// Known file extensions for BED files
        /// </summary>
        public string FileTypes
        {
            get { return Properties.Resource.BedFileFormats; }
        }

        #endregion
    }
}
