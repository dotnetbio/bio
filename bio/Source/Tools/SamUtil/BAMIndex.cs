using System;
using Bio.IO.BAM;
using SamUtil.Properties;

namespace SamUtil
{
    /// <summary>
    /// Class implementing index command of SAM Utility.
    /// </summary>
    public class Index
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Public method implementing Index method of SAM tool.
        /// SAMUtil.exe index in.bam (output file: in.bam.bai)
        /// </summary>
        public void GenerateIndexFile()
        {
            if (FilePath == null)
            {
                throw new InvalidOperationException("FilePath");
            }

            switch (FilePath.Length)
            {
                case 1:
                {
                    try
                    {
                        BAMFormatter.CreateBAMIndexFile(FilePath[0]);
                    }
                    catch
                    {
                        throw new InvalidOperationException(Resources.InvalidBAMFile);
                    }

                    break;
                }
                case 2:
                {
                    try
                    {
                        BAMFormatter.CreateBAMIndexFile(FilePath[0], FilePath[1]);
                    }
                    catch
                    {
                        throw new InvalidOperationException(Resources.InvalidBAMFile);
                    }

                    break;
                }
                default:
                {
                    throw new InvalidOperationException(Resources.IndexHelp);
                }
            }
        }

        #endregion
    }
}
