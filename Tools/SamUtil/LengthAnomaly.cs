using System;
using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;

namespace SamUtil
{
    /// <summary>
    /// Identify Length Anamoly class.
    /// </summary>
    public class LengthAnomaly
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
        /// Display chromosomes for length anamoly
        /// </summary>
        /// <param name="inputFile"> Input file</param>
        /// <param name="mean">Mean value</param>
        /// <param name="standardDeviation">Standard deviation</param>
        public void LengthAnamoly(string inputFile,
            float mean, float standardDeviation)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                throw new InvalidOperationException("Input File Not specified");
            }

            IdentifyLentghAnamolies(inputFile,
                mean, standardDeviation);
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Indentify hot spot chromosomes for length anamoly regions.
        /// </summary>
        /// <param name="inputFile"> Input file</param>
        /// <param name="mean">Mean value</param>
        /// <param name="standardDeviation">Standard deviation</param>
        private void IdentifyLentghAnamolies(string filename,
             float mean = -1, float deviation = -1)
        {
            bool calculateMeanNdeviation = false;

            if (mean == -1 || deviation == -1)
            {
                calculateMeanNdeviation = true;
            }

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

            if (calculateMeanNdeviation)
            {
                pairedReads = alignmentMapobj.GetPairedReads();
            }
            else
            {
                pairedReads = alignmentMapobj.GetPairedReads(mean, deviation);
            }

            // Get the orphan regions.
            var orphans = pairedReads.Where(PR => PR.PairedType == PairedReadType.Orphan);


            if (orphans.Count() == 0)
            {
                Console.WriteLine("No Orphans to display");
            }

            List<ISequenceRange> orphanRegions = new List<ISequenceRange>(orphans.Count());
            foreach (PairedRead orphanRead in orphans)
            {
                orphanRegions.Add(GetRegion(orphanRead.Read1));
            }

            // Get sequence range grouping for Orphan regions.
            SequenceRangeGrouping orphanRangegroup = new SequenceRangeGrouping(orphanRegions);

            // Get the Length anomalies regions.
            var lengthAnomalies = pairedReads.Where(PE => PE.PairedType == PairedReadType.LengthAnomaly);

            if (lengthAnomalies.Count() == 0)
            {
                Console.WriteLine("No Anomalies to display");
            }

            List<ISequenceRange> lengthAnomalyRegions = new List<ISequenceRange>(lengthAnomalies.Count());
            foreach (PairedRead laRead in lengthAnomalies)
            {
                SequenceRange range = new SequenceRange();
                range.ID = laRead.Read1.RName;
                range.Start = laRead.Read1.Pos;
                range.End = laRead.Read1.Pos + laRead.InsertLength;
                lengthAnomalyRegions.Add(range);
            }

            // Get sequence range grouping for length anomaly regions.
            SequenceRangeGrouping lengthAnomalyRangegroup =
                                new SequenceRangeGrouping(lengthAnomalyRegions);
            if (lengthAnomalyRangegroup.GroupIDs.Count() == 0)
            {
                Console.Write("\r\nNo Length anomalies reads to display");
            }
            else
            {
                Console.Write("Region of length anomaly:");
                DisplaySequenceRange(lengthAnomalyRangegroup);
            }

            if (orphanRangegroup.GroupIDs.Count() == 0)
            {
                Console.Write("\r\nNo Orphan reads to display");
            }
            else
            {
                Console.Write("\r\nRegion of Orphan reads:");
                DisplaySequenceRange(orphanRangegroup);
            }

            SequenceRangeGrouping intersectedRegions =
                lengthAnomalyRangegroup.Intersect(orphanRangegroup);
            if (intersectedRegions.GroupIDs.Count() == 0)
            {
                Console.Write("\r\nNo Hot spots found");
            }
            else
            {
                Console.Write("\r\nChromosomal Hot spot of length anomaly and Orphan region:");
                DisplaySequenceRange(intersectedRegions);
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
            Console.Write("\r\nChromosome\tStart\tEnd");

            foreach (string groupID in rangeGroupIds)
            {
                rangeID = groupID;

                // Get SequenceRangeIds.
                List<ISequenceRange> rangeList = seqRangeGrop.GetGroup(rangeID);


                foreach (ISequenceRange seqRange in rangeList)
                {
                    Console.Write("\n{0}\t\t{1}\t{2}", seqRange.ID.ToString(),
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
