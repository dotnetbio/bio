using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Convertible to and from type FileInfo. Creates the output directory, if needed.
    /// </summary>
    //[Parsable]
    public class OutputFile : ParsableFile, IParsable
    {
        /// <summary>
        /// Finalize Parse.
        /// </summary>
        public void FinalizeParse()
        {
            if (FullName != null && FullName != "-")
                _fileInfo.CreateDirectoryForFileIfNeeded();
        }

        /// <summary>
        /// Convert File Info to Output File type.
        /// </summary>
        /// <param name="outputFile">OutPut File.</param>
        /// <returns>Converted File Info to Output File type.</returns>
        public static implicit operator FileInfo(OutputFile outputFile)
        {
            if (outputFile == null)
                return null;
            return outputFile._fileInfo;
        }

        /// <summary>
        /// Convert Output File to File Info type.
        /// </summary>
        /// <param name="fileInfo">File Info.</param>
        /// <returns>Converted Output File to File Info type.</returns>
        public static implicit operator OutputFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
                return null;

            return new OutputFile { _fileInfo = fileInfo };
        }

    }
}
