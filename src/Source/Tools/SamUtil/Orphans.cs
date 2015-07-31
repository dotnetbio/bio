using System;
using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;

namespace SamUtil
{
    /// <summary>
    /// Orphan regions
    /// </summary>
    public class Orphans
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

        #region Public Methods
        /// <summary>
        /// Display Chromosomes with orphan regions.
        /// </summary>
        /// <param name="inputFile">Path of the input file</param>
        /// <param name="mean">Mean value</param>
        /// <param name="standardDeviation">Standard deviation value</param>
        public void DisplayOrpanChromosomes(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                throw new InvalidOperationException("Input File Not specified");
            }

            DisplayOrphans(inputFile);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get chromoses with orphan regions
        /// </summary>
        /// <param name="filename">Path of the BAM file</param>
        /// <param name="mean">Mean value</param>
        /// <param name="deviation">Standard deviation</param>
        /// <returns></returns>
        private void DisplayOrphans(string filename)
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

            // Get Aligned sequences
            IList<SAMAlignedSequence> alignedSeqs = alignmentMapobj.QuerySequences;
            pairedReads = alignmentMapobj.GetPairedReads(0, 0);


            // Get the orphan regions.
            var orphans = pairedReads.Where(PR => PR.PairedType == PairedReadType.Orphan);
            int count = orphans.Count();
            if (count == 0)
            {
                Console.WriteLine("No Orphans to display");
            }

            var orphanRegions = new List<ISequenceRange>(count);
            orphanRegions.AddRange(orphans.Select(orphanRead => GetRegion(orphanRead.Read1)));

            // Get sequence range grouping object.
            SequenceRangeGrouping rangeGroup = new SequenceRangeGrouping(orphanRegions);

            if (!rangeGroup.GroupIDs.Any())
            {
                Console.Write("\r\nNo Orphan reads to display");
            }
            else
            {
                Console.Write("Region of Orphan reads:");
                DisplaySequenceRange(rangeGroup);
            }

            SequenceRangeGrouping mergedRegions = rangeGroup.MergeOverlaps();

            if (!mergedRegions.GroupIDs.Any())
            {
                Console.Write("\r\nNo hot spots to display");
            }
            else
            {
                Console.Write("\r\nChromosomal hot spot:");
                DisplaySequenceRange(mergedRegions);
            }
        }

        /// <summary>
        /// Display Sequence range grops
        /// </summary>
        /// <param name="seqRangeGrops">Sequence Ranges grops</param>
        private static void DisplaySequenceRange(SequenceRangeGrouping seqRangeGrop)
        {
            IEnumerable<string> rangeGroupIds = seqRangeGrop.GroupIDs;
            string rangeID = string.Empty;

            // Display Sequence Ranges
            Console.Write("\r\nChromosome\t\tStart\tEnd");

            foreach (string groupID in rangeGroupIds)
            {
                rangeID = groupID;

                // Get SequenceRangeIds.
                List<ISequenceRange> rangeList = seqRangeGrop.GetGroup(rangeID);

                foreach (ISequenceRange seqRange in rangeList)
                {
                    Console.Write("\n{0}\t\t\t{1}\t{2}", seqRange.ID.ToString(),
                        seqRange.Start.ToString(), seqRange.End.ToString());

                }
            }

        }

        /// <summary>
        /// Gets an instance of SequenceRange class which represets alignment reigon of 
        /// specified aligned sequence (read) with reference sequence.
        /// </summary>
        /// <param name="alignedSequence">Aligned sequence.</param>
        private static ISequenceRange GetRegion(SAMAlignedSequence alignedSequence)
        {
            string refSeqName = alignedSequence.RName;
            long startPos = alignedSequence.Pos;
            long endPos = alignedSequence.RefEndPos;
            return new SequenceRange(refSeqName, startPos, endPos);
        }

        #endregion Private Methods
    }
}
