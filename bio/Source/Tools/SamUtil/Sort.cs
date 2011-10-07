using System;
using Bio.IO.BAM;
using Bio.IO.SAM;
using SamUtil.Properties;

namespace SamUtil
{
    /// <summary>
    /// Class implementing sort command of SAMUtility.
    /// </summary>
    public class Sort
    {
        #region Field Variables

        /// <summary>
        /// Paths of output file and input files.
        /// </summary>
        public string[] FilePaths;

        /// <summary>
        /// Sort input files by read names.
        /// </summary>
        public bool SortByReadName;

        /// <summary>
        /// Usage.
        /// </summary>
        public bool Help;

        #endregion

        #region Public Methods

        /// <summary>
        /// Public method to sort BAM file.
        /// SAMUtil.exe in.bam out.bam
        /// </summary>
        public void DoSort()
        {
            string sortExtension = ".sort";
            if (FilePaths == null)
            {
                throw new InvalidOperationException("FilePaths");
            }

            if (FilePaths.Length < 1)
            {
                throw new InvalidOperationException(Resources.SortHelp);
            }

            BAMParser parse = new BAMParser();
            SequenceAlignmentMap map = null;
            try
            {
                map = parse.Parse(FilePaths[0]);
            }
            catch
            {
                throw new InvalidOperationException(Resources.InvalidBAMFile);
            }
            BAMFormatter format = new BAMFormatter();
            format.CreateSortedBAMFile = true;
            format.SortType = SortByReadName ? BAMSortByFields.ReadNames : BAMSortByFields.ChromosomeCoordinates;
            if (FilePaths.Length > 1)
            {
                format.Format(map, FilePaths[1]);
            }
            else
            {
                format.Format(map, FilePaths[0] + sortExtension);
            }
        }

        #endregion
    }
}
