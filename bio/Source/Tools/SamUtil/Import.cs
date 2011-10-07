using System;
using System.IO;
using Bio.IO.BAM;
using Bio.IO.SAM;
using Bio.Util;
using SamUtil.Properties;

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

            if (_sequenceAlignmentMap.Header.RecordFields.Count == 0)
            {
                if (string.IsNullOrEmpty(ReferenceListFile))
                {
                    throw new InvalidOperationException(Resources.HeaderAbsent);
                }
                else
                {
                    CreateHeader();
                }
            }

            PerformFormat();
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
                catch
                {
                    throw new InvalidOperationException(Resources.WriteBAM);
                }
            }
            else
            {
                SAMFormatter format = new SAMFormatter();
                try
                {
                    format.Format(_sequenceAlignmentMap, _outputFile);
                }
                catch
                {
                    throw new InvalidOperationException(Resources.WriteSAM);
                }
            }
        }

        /// <summary>
        /// Creates the header for SAM file if header is not present.
        /// </summary>
        private void CreateHeader()
        {
            string typecode = "SQ";
            string snTag = "SN";
            string lnTag = "LN";

            using (StreamReader reader = new StreamReader(ReferenceListFile))
            {
                string read = reader.ReadLine();
                while (!string.IsNullOrEmpty(read))
                {
                    string[] splitRegion = read.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitRegion.Length > 1)
                    {
                        SAMRecordField recfield = new SAMRecordField(typecode);
                        recfield.Tags.Add(new SAMRecordFieldTag(snTag, splitRegion[0]));
                        recfield.Tags.Add(new SAMRecordFieldTag(lnTag, splitRegion[1]));
                        _sequenceAlignmentMap.Header.RecordFields.Add(recfield);
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
                catch
                {
                    throw new InvalidOperationException(Resources.InvalidBAMFile);
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
                catch
                {
                    throw new InvalidOperationException(Resources.InvalidSAMFile);
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
