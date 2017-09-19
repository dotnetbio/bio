using System;
using System.IO;
using System.Linq;

namespace Bio.IO.Xsv
{
    /// <summary>
    ///     Common class for reading character separated value files
    ///     e.g. tab separated value (.tsv), Comma separated value (.csv), etc.
    ///     There is "one" record per line. There are multiple columns per line,
    ///     each containing one field in the record.
    ///     It adds properties for extracting fields in each line.
    ///     It has properties for ignoring/extracting comment lines prefixed by comment characters.
    /// </summary>
    public class XsvTextReader : IDisposable
    {
        /// <summary>
        ///     TextReader we are wrapping.
        /// </summary>
        private readonly TextReader reader;

        /// <summary>
        ///     Contains the list of fields split from the current Line
        /// </summary>
        private string[] fields;

        /// <summary>
        ///     Creates a Reader to read character separated values as records with fields.
        /// </summary>
        /// <param name="xsvReader">
        ///     The source text reader to read from.
        ///     This should point to the start of the TextReader if this has a header row.
        ///     Else it can point to the start of a line in the TextReader.
        /// </param>
        /// <param name="separators">Characters that are valid separators between fields in a line</param>
        /// <param name="ignoreWhiteSpace">If true, the white spaces around fields are removed.</param>
        /// <param name="hasHeader">
        ///     If true, the first line of the reader is treated as a header
        ///     row for the fields
        /// </param>
        public XsvTextReader(TextReader xsvReader, char[] separators, bool ignoreWhiteSpace, bool hasHeader)
        {
            this.Separators = separators;
            this.TrimWhiteSpace = ignoreWhiteSpace;
            this.HasHeader = hasHeader;
            this.reader = xsvReader;
            this.SkipBlankLines = true;
            this.GoToNextLine();

            if (hasHeader)
            {
                this.FieldHeaders = this.Fields;
                this.GoToNextLine();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not blank lines should be skipped when GoToNextLine is called.
        /// </summary>
        public bool SkipBlankLines { get; set; }

        /// <summary>
        ///     Gets current line of text.
        /// </summary>
        protected string Line { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether current line is not past the end of the formatted text.
        /// </summary>
        public bool HasLines
        {
            get
            {
                return this.Line != null;
            }
        }

        /// <summary>
        ///     The Xsv files do not have any indents. So override and always return 0.
        /// </summary>
        public int DataIndent
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        ///     Since the Xsv files do not have line headers, this returns the entire Line.
        /// </summary>
        public string LineData
        {
            get
            {
                return this.Line;
            }
        }

        /// <summary>
        ///     Returns the list of fields in the current line as an array of strings.
        ///     This uses the separators defined for this reader to split the current line and
        ///     return the tokens. It trims the tokens if IgnoreWhiteSpace is true.
        ///     If the current line is a comment line, this throws an exception.
        ///     If end of the reader has been reached and HasLines is false, this returns null.
        /// </summary>
        public string[] Fields
        {
            get
            {
                if (!this.SkipCommentLines && this.HasCommentLine)
                {
                    throw new ArgumentException("Comment line found. Cannot get fields.");
                }

                if (this.fields == null)
                {
                    if (!this.HasLines)
                    {
                        return null;
                    }

                    this.fields = this.Line.Split(this.Separators);
                    if (this.TrimWhiteSpace)
                    {
                        for (int i = 0; i < this.fields.Length; i++)
                        {
                            this.fields[i] = this.fields[i].Trim();
                        }
                    }
                }

                return this.fields;
            }
        }

        /// <summary>
        ///     Characters that separate each column in a line.
        /// </summary>
        public char[] Separators { get; set; }

        /// <summary>
        ///     If true, this trims the white space around the field values (including header names).
        ///     Else all characters between the separators are returned as field value.
        /// </summary>
        public bool TrimWhiteSpace { get; set; }

        /// <summary>
        ///     Returns the field names that from the header row (first line)
        ///     if present (HasHeaders == true). Null otherwise.
        /// </summary>
        public string[] FieldHeaders { get; private set; }

        /// <summary>
        ///     If true, the first row of this reader is considered as a header and
        ///     read into FieldHeaders property.
        /// </summary>
        public bool HasHeader { get; private set; }

        /// <summary>
        ///     HasCommentLine is true and the current line starts with the CommentPrefix,
        ///     this returns the portion of the line after the comment prefix character.
        ///     Null otherwise.
        /// </summary>
        public string CommentLine
        {
            get
            {
                return this.HasCommentLine ? this.Line.Substring(1) : null;
            }
        }

        /// <summary>
        ///     If not null or empty, lines starting with any of these characters this list
        ///     are treated as comment lines. This is effective only if SkipCommentLines is
        ///     set to true.
        /// </summary>
        public char[] CommentPrefixes { get; set; }

        /// <summary>
        ///     If true, this skips lines that are prefixed with the comment prefix characters.
        ///     This is effective only if CommentPrefixes has one or more prefix characters.
        ///     If set to true, comment lines cannot be read using the CommentLine property.
        /// </summary>
        public bool SkipCommentLines { get; set; }

        /// <summary>
        ///     Returns true if the current line a valid comment line.
        ///     a current line should exist, CommentPrefixes should have one or more valid comment prefix
        ///     chars and the current line should start with one of these chars.
        /// </summary>
        public bool HasCommentLine
        {
            get
            {
                return this.CommentPrefixes != null && this.CommentPrefixes.Length > 0
                       && // do we have valid comment prefixes?
                       this.HasLines && !string.IsNullOrEmpty(this.Line) && // do we have a valid line?
                       this.CommentPrefixes.Contains(this.Line[0]); // is it a valid comment line?  
            }
        }

        /// <summary>
        ///     IDisposable for IEnumerator
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Calls the GoToNextLine() of the base class.
        ///     Skips comment lines if present and enabled.
        /// </summary>
        public void GoToNextLine()
        {
            do
            {
                this.Line = this.reader.ReadLine();
            }
            while (this.SkipBlankLines && this.Line != null && string.IsNullOrEmpty(this.Line.Trim()));

            // skip comment lines
            while (this.SkipCommentLines && this.HasCommentLine)
            {
                do
                {
                    this.Line = this.reader.ReadLine();
                }
                while (this.SkipBlankLines && this.Line != null && string.IsNullOrEmpty(this.Line.Trim()));
            }

            this.fields = null;
        }

        /// <summary>
        ///     Derived class implementations
        /// </summary>
        /// <param name="disposing">True if we are disposing, 
        /// false if this is a GC finalizer (must be called by derived classes)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}