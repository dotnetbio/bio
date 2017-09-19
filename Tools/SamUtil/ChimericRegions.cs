using System;
using System.Collections.Generic;
using System.Linq;

using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;
using Bio.Matrix;

namespace SamUtil
{
    /// <summary>
    /// Display Chimeric regions
    /// </summary>
    public class ChimericRegions
    {
        #region Public Fields

        /// <summary>
        /// Usage.
        /// </summary>
        public bool Help;

        /// <summary>
        /// Input file is in SAM format.
        /// </summary>
        public bool SAMInput;

        #endregion Public Fields

        /// <summary>
        /// Display Chimeric regions in the parsed file.
        /// </summary>
        /// <param name="inputFile">Path of the input file</param>      
        public void DisplayChimericRegions(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                throw new InvalidOperationException("Input File Not specified");
            }

            Matrix<string, string, string> statistics =
                GetChimeraData(inputFile);
            Console.Write(statistics.ToString2D());
        }

        #region Private Methods

        /// <summary>
        /// Get Chimera data
        /// </summary>
        /// <param name="filename">Path of the BAM file</param>
        /// <param name="mean">Mean value</param>
        /// <param name="deviation">Standard deviation</param>
        /// <returns></returns>
        private Matrix<string, string, string> GetChimeraData(string filename)
        {
            SequenceAlignmentMap alignmentMapobj = null;

            if (!SAMInput)
            {
                BAMParser bamParser = new BAMParser();
                alignmentMapobj = bamParser.ParseOne<SequenceAlignmentMap>(filename);
            }
            else
            {
                SAMParser samParser = new SAMParser();
                alignmentMapobj = samParser.ParseOne<SequenceAlignmentMap>(filename);
            }

            // get reads from sequence alignment map object.
            IList<PairedRead> pairedReads = null;

            pairedReads = alignmentMapobj.GetPairedReads(200, 50);

            // select chimeras from reads.
            var chimeras = pairedReads.Where(PE => PE.PairedType == PairedReadType.Chimera);

            // Group chimeras based on first reads chromosomes name.
            var groupedChimeras =
            chimeras.GroupBy(PR => PR.Read1.RName);

            IList<string> chrs = alignmentMapobj.GetRefSequences();

            // Declare sparse matrix to store statistics.
            SparseMatrix<string, string, string> statistics =
                SparseMatrix<string, string, string>.CreateEmptyInstance(
                       chrs, chrs, "0");

            // For each group create sub group depending on the second reads chromosomes.
            foreach (var group in groupedChimeras)
            {
                foreach (var subgroup in group.GroupBy(PE => PE.Read2.RName))
                {
                    // store the count to stats
                    statistics[group.Key, subgroup.Key] = subgroup.Count().ToString();
                }
            }

            return statistics;
        }

        #endregion Private Methods
    }
}
