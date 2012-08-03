using System;
using System.Collections.Generic;
using System.IO;

namespace Bio.IO.Bed
{
    /// <summary>
    /// Formats lists of ISequenceRange or SequenceRangeGroupings into a file
    /// formatted in the BED format.
    /// 
    /// Info for the specification of this format can be found at:
    /// http://genome.ucsc.edu/FAQ/FAQformat
    /// 
    /// </summary>
    public class BedFormatter : ISequenceRangeFormatter
    {
        #region ISequenceRangeFormatter Members

        /// <summary>
        /// Writes out a list of ISequenceRange objects to a specified
        /// file location.
        /// </summary>
        /// <param name="ranges">The range collection to be formatted.</param>
        /// <param name="fileName">The file where the formatted data is to be written.</param>
        public void Format(IList<ISequenceRange> ranges, string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Format(ranges, new StreamWriter(stream));
            }
        }

        /// <summary>
        /// Writes out a list of ISequenceRange objects to a specified
        /// text writer.
        /// </summary>
        /// <param name="ranges">The range collection to be formatted.</param>
        /// <param name="writer">The writer stream where the formatted data is to be written.</param>
        public void Format(IList<ISequenceRange> ranges, TextWriter writer)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            // TODO: Need support for tracks and for optional metadata columns

            int lineCount = 0;
            foreach (ISequenceRange range in ranges)
            {
                writer.Write(range.ID);
                writer.Write('\t');
                writer.Write(range.Start);
                writer.Write('\t');
                writer.Write(range.End);

                if (range.Metadata.Count > 0)
                {
                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("Name"))
                    {
                        writer.Write(range.Metadata["Name"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("Score"))
                    {
                        writer.Write(range.Metadata["Score"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("Strand"))
                    {
                        writer.Write(range.Metadata["Strand"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("ThickStart"))
                    {
                        writer.Write(range.Metadata["ThickStart"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("ThickEnd"))
                    {
                        writer.Write(range.Metadata["ThickEnd"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("ItemRGB"))
                    {
                        writer.Write(range.Metadata["ItemRGB"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("BlockCount"))
                    {
                        writer.Write(range.Metadata["BlockCount"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("BlockSizes"))
                    {
                        writer.Write(range.Metadata["BlockSizes"]);
                    }

                    writer.Write('\t');
                    if (range.Metadata.ContainsKey("BlockStarts"))
                    {
                        writer.Write(range.Metadata["BlockStarts"]);
                    }
                }

                writer.WriteLine();

                if (lineCount++ % 500 == 0)
                    writer.Flush();
            }

            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Writes out a grouping of ISequenceRange objects to a specified
        /// file location.
        /// </summary>
        /// <param name="rangeGroup">The range grouping to be formatted.</param>
        /// <param name="fileName">The file where the formatted data is to be written.</param>
        public void Format(SequenceRangeGrouping rangeGroup, string fileName)
        {
            if (rangeGroup == null)
            {
                throw new ArgumentNullException("rangeGroup");
            }

            Format(rangeGroup.Flatten(), fileName);
        }

        /// <summary>
        /// Writes out a grouping of ISequenceRange objects to a specified
        /// text writer.
        /// </summary>
        /// <param name="rangeGroup">The range grouping to be formatted.</param>
        /// <param name="writer">The writer stream where the formatted data is to be written.</param>
        public void Format(SequenceRangeGrouping rangeGroup, TextWriter writer)
        {
            if (rangeGroup == null)
            {
                throw new ArgumentNullException("rangeGroup");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            Format(rangeGroup.Flatten(), writer);
        }

        /// <summary>
        /// The name of this format: BED
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.BedName; }
        }

        /// <summary>
        /// A short description of the format
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.BedDesc; }
        }

        /// <summary>
        /// Known file extensions for the BED format
        /// </summary>
        public string FileTypes
        {
            get { return Properties.Resource.BedFileFormats; }
        }

        #endregion
    }
}
