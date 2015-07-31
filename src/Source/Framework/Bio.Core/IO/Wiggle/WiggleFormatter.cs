using System;
using System.Collections.Generic;
using System.IO;

using Bio.Extensions;

namespace Bio.IO.Wiggle
{
    /// <summary>
    /// Writes a wiggle annotation format to a file or a stream.
    /// </summary>
    public class WiggleFormatter : IFormatter<WiggleAnnotation>
    {
        /// <summary>
        /// Gets the name of this formatter.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.WiggleName; }
        }

        /// <summary>
        /// Gets a short description of this formatter.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.WiggleFormatterDescription; }
        }

        /// <summary>
        /// Gets the file extensions that the formatter will support.
        /// If multiple extensions are supported then this property 
        /// will return a string containing all extensions with a ',' delimited.
        /// </summary>
        public string SupportedFileTypes { get { return Properties.Resource.Wiggle_FileExtension; } }

        /// <summary>
        /// Writes a single data entry.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="data">The data to write.</param>
        public void Format(Stream stream, WiggleAnnotation data)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            using (var writer = stream.OpenWrite())
            {
                WriteOne(writer, data);
            }
        }

        /// <summary>
        /// Writes a set of entries.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="annotations">The data to write.</param>
        public void Format(Stream stream, IEnumerable<WiggleAnnotation> annotations)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (annotations == null)
            {
                throw new ArgumentNullException("annotations");
            }

            using (var writer = stream.OpenWrite())
            {
                foreach (var entry in annotations)
                {
                    WriteOne(writer, entry);
                }
            }
        }

        /// <summary>
        /// Writes a single data entry.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        /// <param name="annotation">The data to write.</param>
        void WriteOne(StreamWriter writer, WiggleAnnotation annotation)
        {
            // track line
            writer.Write(WiggleSchema.Track);
            foreach (var x in annotation.Metadata)
                writer.Write(" " + x.Key + "=" + (x.Value.Contains(" ") ? "\"" + x.Value + "\"" : x.Value));

            writer.WriteLine();

            // metadata
            writer.Write(annotation.AnnotationType == WiggleAnnotationType.FixedStep ? WiggleSchema.FixedStep : WiggleSchema.VariableStep);
            writer.Write(" {0}={1}", WiggleSchema.Chrom, annotation.Chromosome);

            if (annotation.AnnotationType == WiggleAnnotationType.FixedStep)
            {
                writer.Write(" {0}={1}", WiggleSchema.Start, annotation.BasePosition);
                writer.Write(" {0}={1}", WiggleSchema.Step, annotation.Step);
            }

            if (annotation.Span != -1)
            {
                writer.Write(" {0}={1}", WiggleSchema.Span, annotation.Span);
            }

            writer.WriteLine();

            // write data
            if (annotation.AnnotationType == WiggleAnnotationType.FixedStep)
            {
                foreach (var item in annotation)
                {
                    writer.WriteLine(item.Value);
                }
            }
            else
            {
                foreach (var item in annotation)
                {
                    writer.WriteLine(item.Key + " " + item.Value);
                }
            }

            writer.Flush();
        }
    }
}
