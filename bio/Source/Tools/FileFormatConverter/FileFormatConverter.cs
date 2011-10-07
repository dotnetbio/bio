using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio;
using Bio.IO;
using System.IO;

namespace FileFormatConverter
{
    public class FileFormatConverter
    {
        #region Fields
        /// <summary>
        /// input file
        /// </summary>
        public string InputFile;

        /// <summary>
        /// output file
        /// </summary>
        public string OutputFile;

        /// <summary>
        /// Usage.
        /// </summary>
        public bool Help;

        #endregion

        #region Methods
        /// <summary>
        /// convert input file to output file using the specified format conversion
        /// </summary>
        public void ConvertFile()
        {
            //make sure input file is valid
            if (!File.Exists(InputFile))
            {
                throw new Exception("Input file does not exist.");
            }

            //Finds a parser and opens the file
            var inputParser = SequenceParsers.FindParserByFileName(InputFile);

            if (inputParser == null)
            {
                throw new Exception("Input file not a valid file format to parse.");
            }

            IEnumerable<ISequence> sequences;

            //Finds a formatter and opens the file
            var outputFormatter = SequenceFormatters.FindFormatterByFileName(OutputFile);
            if (inputParser == null)
            {
                throw new Exception("Output file not a valid file format for conversion.");
            }

            sequences = inputParser.Parse();
            foreach (ISequence sequence in sequences)
            {
                outputFormatter.Write(sequence);
            }

            outputFormatter.Close();
            inputParser.Close();
        }

        /// <summary>
        /// provides a list of extensions that are available for parsing
        /// </summary>
        /// <returns></returns>
        public string ListOfExtensionsToParse()
        {
            string ret = string.Empty;

            foreach (var sp in SequenceParsers.All)
            {
                ret += String.Format("{0} Parse From:{1}\n", sp.Name, sp.SupportedFileTypes);
            }

            return ret;
        }

        /// <summary>
        /// provides the list of extensions that are available to the user
        /// for converting into
        /// </summary>
        /// <returns></returns>
        public string ListOfExtensionsForConversion()
        {
            string ret = string.Empty;

            foreach (var sf in SequenceFormatters.All)
            {
                ret += String.Format("{0} Convert To: {1}\n", sf.Name, sf.SupportedFileTypes);
            }

            return ret;
        }
        #endregion
    }
}

