using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Bio.IO;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Convertible to and from type FileInfo. Verifies that the file exists.
    /// </summary>
    //[Parsable]
    public class InputFile : ParsableFile, IParsable
    {
        /// <summary>
        /// Finalize Parse.
        /// </summary>
        public void FinalizeParse()
        {
            Helper.CheckCondition<ParseException>(_fileInfo.Name == "-" || _fileInfo.Exists, "File {0} does not exist.", FullName);
        }

        /// <summary>
        /// Convert fileInfo to input file.
        /// </summary>
        /// <param name="inputFile">Input file.</param>
        /// <returns>Converted operator.</returns>
        public static implicit operator FileInfo(InputFile inputFile)
        {
            if (inputFile == null)
                return null;

            return inputFile._fileInfo;
        }

        /// <summary>
        /// Convert input file to FileInfo type.
        /// </summary>
        /// <param name="fileInfo">File Info.</param>
        /// <returns>>Converted fileInfo to input file..</returns>
        public static implicit operator InputFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
                return null;

            return new InputFile { _fileInfo = fileInfo };
        }

        /// <summary>
        /// Convert NamedStreamcreator to InputFile type.
        /// </summary>
        /// <param name="inputFile">Input File.</param>
        /// <returns>Converted NamedStreamCreator to InputFile.</returns>
        public static implicit operator NamedStreamCreator(InputFile inputFile)
        {
            if (inputFile == null)
                return null;
            return new NamedStreamCreator(inputFile._fileInfo.Name, () => inputFile._fileInfo.OpenRead());
        }
    }
}
