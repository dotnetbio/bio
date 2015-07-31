using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bio.Util.ArgumentParser
{
    
    //[Parsable]
    /// <summary>
    /// ParsableFile
    /// </summary>
    public class ParsableFile
    {
        /// <summary>
        /// Can set with a short file name, but the 'get' will return the FullName;
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), Parse(ParseAction.Required)]
        public string FullName
        {
            set
            {
                _fileInfo = (value == null) ? null : new FileInfo(value.Replace("(", "").Replace(")", "").Trim('"'));
            }
            get
            {
                return (_fileInfo == null) ? null : _fileInfo.ToString();
            }
        }

        /// <summary>
        /// File Info.
        /// </summary>
        [Parse(ParseAction.Ignore)]
        protected FileInfo _fileInfo = null;

        /// <summary>
        /// FileInfo ToString conversion.
        /// </summary>
        /// <returns>String of fileInfo.</returns>
        public override string ToString()
        {
            return _fileInfo.ToString();
        }

        /// <summary>
        /// Convert FileInfo to Parsable File.
        /// </summary>
        /// <param name="inputFile">Input file.</param>
        /// <returns>Converted FileInfo to Parsable File.</returns>
        public static implicit operator FileInfo(ParsableFile inputFile)
        {
            if (inputFile == null)
                return null;

            return inputFile._fileInfo;
        }

        /// <summary>
        /// Convert Parsable File to FileInfo.
        /// </summary>
        /// <param name="fileInfo">FileInfo.</param>
        /// <returns>Converted Parsable File to FileInfo.</returns>
        public static implicit operator ParsableFile(FileInfo fileInfo)
        {
            return new ParsableFile { _fileInfo = fileInfo };
        }
    }
}
