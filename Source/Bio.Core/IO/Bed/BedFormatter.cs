using System;
using System.Collections.Generic;
using System.IO;

using Bio.Extensions;
using Bio.Properties;

namespace Bio.IO.Bed
{
    /// <summary>
    ///     Formats lists of ISequenceRange or SequenceRangeGroupings into a file
    ///     formatted in the BED format.
    ///     Info for the specification of this format can be found at:
    ///     http://genome.ucsc.edu/FAQ/FAQformat
    /// </summary>
    public class BedFormatter : ISequenceRangeFormatter
    {
        /// <summary>
        ///     Writes out a list of ISequenceRange objects to a specified
        ///     stream. The stream is closed at the end.
        /// </summary>
        /// <param name="stream">The stream where the formatted data is to be written.</param>
        /// <param name="ranges">The range collection to be formatted.</param>
        public void Format(Stream stream, IList<ISequenceRange> ranges)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }

            // TODO: Need support for tracks and for optional metadata columns

            // Open the output stream - we leave the underlying stream open.
            using (StreamWriter writer = stream.OpenWrite())
            {
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

                    // Flush every 500 lines.
                    if (lineCount++ % 500 == 0)
                    {
                        writer.Flush();
                    }
                }
            }
        }

        /// <summary>
        ///     Writes out a grouping of ISequenceRange objects to a specified
        ///     stream.
        /// </summary>
        /// <param name="stream">The stream where the formatted data is to be written, it will be closed at the end.</param>
        /// <param name="rangeGroup">The range grouping to be formatted.</param>
        public void Format(Stream stream, SequenceRangeGrouping rangeGroup)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (rangeGroup == null)
            {
                throw new ArgumentNullException("rangeGroup");
            }

            this.Format(stream, rangeGroup.Flatten());
        }

        /// <summary>
        ///     The name of this format: BED
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.BedName;
            }
        }

        /// <summary>
        ///     A short description of the format
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.BedDesc;
            }
        }

        /// <summary>
        ///     Known file extensions for the BED format
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.BedFileFormats;
            }
        }
    }
}