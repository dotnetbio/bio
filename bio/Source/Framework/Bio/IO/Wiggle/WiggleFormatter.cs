using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bio.IO.Wiggle
{
    /// <summary>
    /// Writes a wiggle annotation format to a file or a stream.
    /// </summary>
    public class WiggleFormatter : IDisposable
    {
        /// <summary>
        /// Holds stream writer used for writing to file.
        /// </summary>
        private StreamWriter writer = null;

        /// <summary>
        /// Initializes a new instance of the WiggleFormatter class.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        public WiggleFormatter(StreamWriter writer)
        {
            if (this.writer != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.Filename = null;
            this.writer = writer;
        }

        /// <summary>
        /// Initializes a new instance of the WiggleFormatter class.
        /// </summary>
        /// <param name="filename">File to write to.</param>
        public WiggleFormatter(string filename)
        {
            if (this.writer != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.Filename = filename;
            this.writer = new StreamWriter(this.Filename);
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

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
        /// Gets the known file extensions for Wiggle files.
        /// </summary>
        public string FileTypes
        {
            get { return Properties.Resource.Wiggle_FileExtension; }
        }

        /// <summary>
        /// Write annotation to the given file/stream in wiggle format.
        /// </summary>
        /// <param name="annotation">Annotation to write.</param>
        public void Write(WiggleAnnotation annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }

            if (this.writer == null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotOpened);
            }

            // track line
            this.writer.Write(WiggleSchema.Track);
            foreach (var x in annotation.Metadata)
            {
                this.writer.Write(" " + x.Key + "=" + (x.Value.Contains(' ') ? "\"" + x.Value + "\"" : x.Value));
            }

            this.writer.WriteLine();

            // metadata
            this.writer.Write(annotation.AnnotationType == WiggleAnnotationType.FixedStep ? WiggleSchema.FixedStep : WiggleSchema.VariableStep);
            this.writer.Write(string.Format(CultureInfo.InvariantCulture, " {0}={1}", WiggleSchema.Chrom, annotation.Chromosome));
            
            if (annotation.AnnotationType == WiggleAnnotationType.FixedStep)
            {
                this.writer.Write(string.Format(CultureInfo.InvariantCulture, " {0}={1}", WiggleSchema.Start, annotation.BasePosition));
                this.writer.Write(string.Format(CultureInfo.InvariantCulture, " {0}={1}", WiggleSchema.Step, annotation.Step));
            }

            if (annotation.Span != -1)
            {
                this.writer.Write(string.Format(CultureInfo.InvariantCulture, " {0}={1}", WiggleSchema.Span, annotation.Span));
            }

            this.writer.WriteLine();

            // write data
            if (annotation.AnnotationType == WiggleAnnotationType.FixedStep)
            {
                foreach (var item in annotation)
                {
                    this.writer.WriteLine(item.Value);
                }
            }
            else
            {
                foreach (var item in annotation)
                {
                    this.writer.WriteLine(item.Key + " " + item.Value);
                }
            }

            this.writer.Flush();
        }

        /// <summary>
        /// Closes the current formatter and underlying stream.
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>
        /// Close and dispose this formatter.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close and dispose this formatter.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.writer != null)
            {
                this.writer.Flush();
                this.writer.Close();
                this.writer = null;
            }
        }
    }
}
