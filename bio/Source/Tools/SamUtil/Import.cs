using System;
using System.IO;
using Bio.IO.BAM;
using Bio.IO.SAM;
using Bio.Util;
using SamUtil.Properties;
using System.Globalization;

namespace SamUtil
{
    /// <summary>
    /// Class implementing Import command of SAM Utility.
    /// </summary>
    public class Import
    {
        #region Public Fields

        /// <summary>
        /// Paths of input and output file.
        /// </summary>
        public string[] FilePath;

        /// <summary>
        /// Usage(Help)
        /// </summary>
        public bool Help;

        /// <summary>
        /// This file is TAB-delimited. 
        /// Each line must contain the reference name and the length of the reference, one line for each distinct reference; 
        /// additional fields are ignored.
        /// </summary>
        public string ReferenceListFile;

        #endregion

        #region Private Fields

        /// <summary>
        /// SAM object holding data from parsed file.
        /// </summary>
        private SequenceAlignmentMap _sequenceAlignmentMap;

        /// <summary>
        /// Path of input file.
        /// </summary>
        private string _inputFile;

        /// <summary>
        /// Path of output file.
        /// </summary>
        private string _outputFile;

        /// <summary>
        /// Whether input file SAM/BAM
        /// </summary>
        private bool _isSAM;

        #endregion

        #region Public Methods

        /// <summary>
        /// Import converts SAM <=> BAM file formats.
        /// SAMUtil.exe import out.sam in.bam
        /// </summary>
        public void DoImport()
        {
            if (FilePath == null)
            {
                throw new InvalidOperationException("FilePath");
            }

            if (!string.IsNullOrEmpty(ReferenceListFile) && !File.Exists(ReferenceListFile))
            {
                throw new InvalidOperationException("File " + ReferenceListFile + " does not exist");
            }

            switch (FilePath.Length)
            {
                case 1:
                    {
                        _inputFile = FilePath[0];
                        break;
                    }
                case 2:
                    {
                        _inputFile = FilePath[1];
                        _outputFile = FilePath[0];
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(Resources.ImportHelp);
                    }
            }

            PerformParse();
            if (_sequenceAlignmentMap == null)
            {
                throw new InvalidOperationException(Resources.EmptyFile);
            }

            if (!string.IsNullOrEmpty(ReferenceListFile))
            {
                CreateHeader();
            }

            PerformFormat();

            if (FilePath.Length == 1)
            {
                Console.WriteLine(Properties.Resources.SuccessMessageWithOutputFileName, _outputFile);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes the SAM object to file in SAM/BAM format.
        /// </summary>
        private void PerformFormat()
        {
            if (_isSAM)
            {
                BAMFormatter format = new BAMFormatter();
                try
                {
                    format.Format(_sequenceAlignmentMap, _outputFile);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(Resources.WriteBAM + Environment.NewLine + ex.Message);
                }
            }
            else
            {
                SAMFormatter format = new SAMFormatter();
                try
                {
                    format.Format(_sequenceAlignmentMap, _outputFile);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(Resources.WriteSAM + Environment.NewLine + ex.Message);
                }
            }
        }

        /// <summary>
        /// Creates the header for SAM file if header is not present.
        /// </summary>
        private void CreateHeader()
        {
            using (StreamReader reader = new StreamReader(ReferenceListFile))
            {
                _sequenceAlignmentMap.Header.ReferenceSequences.Clear();
                string read = reader.ReadLine();
                while (!string.IsNullOrEmpty(read))
                {
                    string[] splitRegion = read.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitRegion.Length > 1)
                    {
                        string name = splitRegion[0];
                        long len = long.Parse(splitRegion[1], CultureInfo.InvariantCulture);
                        _sequenceAlignmentMap.Header.ReferenceSequences.Add(new ReferenceSequenceInfo(name, len));
                    }
                    else
                    {
                        throw new InvalidOperationException(Resources.ReferenceFile);
                    }

                    read = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Parses SAM/BAm file based on input file.
        /// </summary>
        private void PerformParse()
        {
            string samExtension = ".sam";
            string bamExtension = ".bam";

            if (Helper.IsBAM(_inputFile))
            {
                BAMParser parser = new BAMParser();
                try
                {
                    _sequenceAlignmentMap = parser.Parse(_inputFile);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(Resources.InvalidBAMFile, ex);
                }

                if (string.IsNullOrEmpty(_outputFile))
                {
                    _outputFile = _inputFile + samExtension;
                }
            }
            else
            {
                SAMParser parser = new SAMParser();
                try
                {
                    _sequenceAlignmentMap = parser.Parse(_inputFile);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(Resources.InvalidSAMFile, ex);
                }

                _isSAM = true;
                if (string.IsNullOrEmpty(_outputFile))
                {
                    _outputFile = _inputFile + bamExtension;
                }
            }
        }

        #endregion
    }
}
