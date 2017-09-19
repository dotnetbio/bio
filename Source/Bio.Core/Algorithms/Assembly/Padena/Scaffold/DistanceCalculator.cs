using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Class calculates distance between contigs using mate pairs mapped to contigs.
    /// Reference: The Greedy Path-Merging Algorithm for contig Scaffolding: HUSON et al.
    /// </summary>
    public class DistanceCalculator : IDistanceCalculator
    {
        /// <summary>
        /// Contigs and mate pairs mapping
        /// </summary>
        private ContigMatePairs contigPairedReads;

        /// <summary>
        /// Distance calculator.
        /// </summary>
        /// <param name="contigPairedReads">Contig pair reads.</param>
        public DistanceCalculator(ContigMatePairs contigPairedReads)
        {
            if (contigPairedReads == null)
            {
                throw new ArgumentNullException("contigPairedReads");
            }

            this.contigPairedReads = contigPairedReads;
        }

        #region IDistanceEstimation Members

        /// <summary>
        /// Calculates distances between contigs.
        /// </summary>
        /// <returns>Contig pair reads. </returns>
        public ContigMatePairs CalculateDistance()
        {
            Parallel.ForEach(
                this.contigPairedReads, 
                (KeyValuePair<ISequence, 
                    Dictionary<ISequence, IList<ValidMatePair>>> contigPairedRead) =>
            {
                CalculateInterContigDistance(contigPairedRead, contigPairedRead.Key.Count);
            });

            return this.contigPairedReads;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculate distance between single pair of contigs.
        /// </summary>
        /// <param name="contigsPairedRead">Contig mate pairs map.</param>
        /// <param name="lengthOfContig">Length of forward contig.</param>
        private static void CalculateInterContigDistance(
            KeyValuePair<ISequence, Dictionary<ISequence, IList<ValidMatePair>>> contigsPairedRead,
            long lengthOfContig)
        {
            Parallel.ForEach(
                contigsPairedRead.Value, 
                (KeyValuePair<ISequence,
                IList<ValidMatePair>> contigPairedRead) =>
            {
                Parallel.ForEach(
                    contigPairedRead.Value, 
                    (ValidMatePair validPairedRead) =>
                {
                    CalculateDistance(validPairedRead, lengthOfContig);
                });

                EdgeBundling(contigPairedRead.Value);
                if (contigPairedRead.Value.Count > 1)
                {
                    CalculateWeigthedEdge(contigPairedRead.Value);
                }
            });
        }

        /// <summary>
        /// Calculates distance between contigs for each pair of Full Overlap.
        /// </summary>
        /// <param name="validPairedRead">Valid mate pairs which have full Overlap 
        /// and support a particular orientation of contigs.</param>
        /// <param name="length">Length of forward contig.</param>
        private static void CalculateDistance(ValidMatePair validPairedRead, long length)
        {
            // For reverse read sequence in forward direction.
            validPairedRead.DistanceBetweenContigs.Add(
                validPairedRead.PairedRead.MeanLengthOfLibrary - (length -
                validPairedRead.ForwardReadStartPosition.First())
                - (validPairedRead.ReverseReadStartPosition.First() + 1));
            validPairedRead.StandardDeviation.Add(validPairedRead.PairedRead.StandardDeviationOfLibrary);
            validPairedRead.Weight = 1;

            // For reverse read sequence in reverse complementary direction.
            validPairedRead.DistanceBetweenContigs.Add(
                validPairedRead.PairedRead.MeanLengthOfLibrary - (length -
                validPairedRead.ForwardReadStartPosition[0])
                - (validPairedRead.ReverseReadReverseComplementStartPosition[0] + 1));

            if (validPairedRead.PairedRead.StandardDeviationOfLibrary != 0)
            {
                validPairedRead.StandardDeviation.Add(validPairedRead.PairedRead.StandardDeviationOfLibrary);
            }
            else
            {
                validPairedRead.StandardDeviation.Add(1);
            }
        }

        /// <summary>
        /// Bundles all valid mate pairs in single edge but considering ±3σ limit.
        /// </summary>
        /// <param name="contigPairedRead">List of Valid Paired Reads.</param>
        private static void EdgeBundling(IList<ValidMatePair> contigPairedRead)
        {
            int index = 0;
            List<ValidMatePair> estimatedDistances;
            while (index < contigPairedRead.Count())
            {
                estimatedDistances = contigPairedRead.Where(distance =>
                    (contigPairedRead[index].DistanceBetweenContigs.First() - (3.0 *
                    contigPairedRead[index].StandardDeviation.First())
                    <= distance.DistanceBetweenContigs.First()) &&
                    distance.DistanceBetweenContigs[0] <=
                    (contigPairedRead[index].DistanceBetweenContigs.First() + (3.0 *
                    contigPairedRead[index].StandardDeviation.First()))).ToList();
                if (estimatedDistances.Count > 1)
                {
                    float p = (float)estimatedDistances.Sum(dist => dist.DistanceBetweenContigs[0]
                        / Math.Pow(dist.StandardDeviation[0], 2));
                    float q = (float)estimatedDistances.Sum(sd => 1 / Math.Pow(sd.StandardDeviation[0], 2));
                    ValidMatePair distance = new ValidMatePair();
                    distance.DistanceBetweenContigs.Add(p / q);
                    distance.StandardDeviation.Add((float)(1 / Math.Sqrt(q)));

                    p = (float)estimatedDistances.Sum(dist => dist.DistanceBetweenContigs[1]
                        / Math.Pow(dist.StandardDeviation[1], 2));
                    q = (float)estimatedDistances.Sum(sd => 1 / Math.Pow(sd.StandardDeviation[1], 2));

                    distance.DistanceBetweenContigs.Add(p / q);
                    distance.StandardDeviation.Add((float)(1 / Math.Sqrt(q)));
                    foreach (ValidMatePair est in estimatedDistances)
                    {
                        contigPairedRead.Remove(est);
                    }

                    distance.Weight = estimatedDistances.Count;
                    contigPairedRead.Add(distance);
                    index = 0;
                }
                else
                {
                    index++;
                }
            }
        }

        /// <summary>
        /// Further estimates distances using weighted mean. 
        /// and standard deviation by merging valid mate pairs.
        /// </summary>
        /// <param name="distances">List of valid mate pairs.</param>
        private static void CalculateWeigthedEdge(IList<ValidMatePair> distances)
        {
            ValidMatePair finalDistance = new ValidMatePair();
            finalDistance.DistanceBetweenContigs.Add(
                distances.Sum(distance => distance.DistanceBetweenContigs[0] * distance.Weight));
            finalDistance.StandardDeviation.Add(
                distances.Sum(distance => distance.StandardDeviation[0] * distance.Weight));
            finalDistance.Weight = distances.Sum(distance => distance.Weight);
            finalDistance.DistanceBetweenContigs[0] /= finalDistance.Weight;
            finalDistance.StandardDeviation[0] /= finalDistance.Weight;
            finalDistance.DistanceBetweenContigs.Add(
                distances.Sum(distance => distance.DistanceBetweenContigs[1] * distance.Weight));
            finalDistance.StandardDeviation.Add(
                distances.Sum(distance => distance.StandardDeviation[0] * distance.Weight));
            finalDistance.DistanceBetweenContigs[1] /= finalDistance.Weight;
            finalDistance.StandardDeviation[1] /= finalDistance.Weight;
            distances.Clear();
            distances.Add(finalDistance);
        }

        #endregion
    }
}
