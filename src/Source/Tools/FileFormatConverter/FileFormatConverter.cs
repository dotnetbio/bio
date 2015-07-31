using System;
using System.IO;
using System.Linq;

using Bio;
using Bio.IO;

namespace FileFormatConverter
{
    public class FileFormatConverter
    {
        #region Fields

        /// <summary>
        /// File inputs
        /// </summary>
        public string[] FileList;

        /// <summary>
        /// input file
        /// </summary>
        public string InputFile;

        /// <summary>
        /// output file
        /// </summary>
        public string OutputFile;

        #endregion

        #region Methods

        /// <summary>
        /// convert input file to output file using the specified format conversion
        /// </summary>
        public void ConvertFile()
        {
            //make sure input file is valid
            if (!File.Exists(this.InputFile))
            {
                throw new Exception("Input file does not exist.");
            }

            //Finds a parser and opens the file
            ISequenceParser inputParser = SequenceParsers.FindParserByFileName(this.InputFile);

            if (inputParser == null)
            {
                throw new Exception("Input file not a valid file format to parse.");
            }

            //Finds a formatter and opens the file
            ISequenceFormatter outputFormatter = SequenceFormatters.FindFormatterByFileName(this.OutputFile);

            if (outputFormatter == null)
            {
                throw new Exception("Output file not a valid file format for conversion.");
            }

            try
            {
                foreach (ISequence sequence in inputParser.Parse())
                {
                    outputFormatter.Format(sequence);
                }
            }
            catch
            {
                throw new OperationCanceledException(
                    string.Format(
                        "Unable to convert sequences from {0} to {1} - verify that the input sequences have the appropriate type of data to convert to the output formatter.",
                        inputParser.Name,
                        outputFormatter.Name));
            }
            finally
            {
                outputFormatter.Close();
                inputParser.Close();
            }
        }

        /// <summary>
        /// provides a list of extensions that are available for parsing
        /// </summary>
        /// <returns></returns>
        public string ListOfExtensionsToParse()
        {
            return SequenceParsers.All.Aggregate(
                string.Empty,
                (current, sp) => current + String.Format("{0} Parse From:{1}\n", sp.Name, sp.SupportedFileTypes));
        }

        /// <summary>
        /// provides the list of extensions that are available to the user
        /// for converting into
        /// </summary>
        /// <returns></returns>
        public string ListOfExtensionsForConversion()
        {
            return SequenceFormatters.All.Aggregate(
                string.Empty,
                (current, sf) => current + String.Format("{0} Convert To: {1}\n", sf.Name, sf.SupportedFileTypes));
        }

        #endregion
    }
}