using System;
using System.Globalization;
using System.IO;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// This is used to read a sparse sequences from a text reader available as 
    /// character separated values in a line. There is one sequence item per line and 
    /// each record has two fields: position (int) and sequence item symbol (char).
    /// A comment line should be the first line for the reader and it contains
    /// the following fields separated by the separator character:
    /// 1. the starting offset (in case this is an aligned sequence, 0 by default),
    /// 2. the count of the sequence, and 
    /// 3. the sequence ID string (newlines removed).
    /// e.g. reader for sparse sequence source with '#' as sequence ID prefix and ',' as separator 
    /// would read the following file contents:
    /// 
    /// #0,100,A sparse sequence with 5 items
    /// 12,A
    /// 29,T
    /// 56,G
    /// 85,A
    /// 32,C
    /// </summary>
    public class XsvSparseReader : XsvTextReader 
    {
        /// <summary>
        /// Do not allow changing IgnoreComment since comments are required to 
        /// be enabled to set sequence I.
        /// </summary>
        protected new bool SkipCommentLines { get; set; }

        /// <summary>
        /// Do not allow changing CommentPrefix since it is set by the constructor 
        /// as the sequenceIDPrefix.
        /// </summary>
        protected new char[] CommentPrefixes { get; set; }

        /// <summary>
        /// Creates a reader for a sparse sequence that has sequence items as 
        /// character separated values, one per line.
        /// </summary>
        /// <param name="reader">The text reader with the contents of the sparse sequence</param>
        /// <param name="separator">The character that separates the sequence item position from its symbol. 
        /// This is also used to separate the offset, count, id in the sequence comment line.</param>
        /// <param name="sequenceIDPrefix">The character used to prefix the sequence comment line that 
        /// contains the offset, count, id in the sequence. this is used as a comment prefix character
        /// in the underlying XsvTextReader.</param>
        public XsvSparseReader(TextReader reader, char separator, char sequenceIDPrefix) : 
                base(reader, new[] {separator}, true, false) 
        {
            base.SkipCommentLines = false;
            base.CommentPrefixes = new[] {sequenceIDPrefix};
        }

        /// <summary>
        /// If the current line is the sequence start line it returns the sequence ID field in it.
        /// This throws an exception if the current line is not prefixed by the sequence id prefix,
        /// or if the sequence id is not present as the third character separated field.
        /// </summary>
        /// <returns>The sequence ID string for this sparse sequence</returns>
        public string GetSequenceId() 
        {
            string comment = base.CommentLine;
            int commaIndex = comment.IndexOfAny(Separators);
            if (commaIndex < 1)
                throw new FormatException(Properties.Resource.Parser_InvalidFileFormat);
            
            commaIndex = comment.IndexOfAny(Separators, commaIndex + 1);
            if (commaIndex < 3)
                throw new FormatException(Properties.Resource.Parser_InvalidFileFormat);

            return comment.Substring(commaIndex + 1);
        }

        /// <summary>
        /// If the current line is the sequence start line it returns the sequence offset field.
        /// This thows an exception if the current line is not prefixed by the sequence id prefix,
        /// or if the sequence offset is not present as an integer in the first character separated field.
        /// </summary>
        /// <returns>The sequence offset integer for this sparse sequence</returns>
        public int GetSequenceOffset() 
        {
            string comment = base.CommentLine;
            string[] splits = comment.Split(Separators);
            if (splits.Length < 3)
                throw new FormatException(Properties.Resource.Parser_InvalidFileFormat);
            return int.Parse(splits[0], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// If the current line is the sequence start line it returns the sequence count field.
        /// This throws an exception if the current line is not prefixed by the sequence id prefix,
        /// or if the sequence count is not present as an integer in the second character separated field.
        /// </summary>
        /// <returns>The sequence count integer for this sparse sequence</returns>
        public int GetSequenceCount () 
        {
            string comment = base.CommentLine;
            string[] splits = comment.Split(Separators);
            if (splits.Length < 3)
                throw new FormatException(Properties.Resource.Parser_InvalidFileFormat);
            return int.Parse(splits[1], CultureInfo.InvariantCulture);
        }
    }
}
