using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Comparative
{
    /// <summary>
    /// Reads ambiguously placed due to genomic reads.
    /// This step requires mate pair information to resolve the ambiguity about placements of repeated sequences.
    /// </summary>
    public static class RepeatResolver
    {
        /// <summary>
        /// Reads ambiguously placed due to genomic reads.
        /// This step requires mate pair information to resolve the ambiguity about placements of repeated sequences.
        /// </summary>
        /// <param name="alignmentBetweenReferenceAndReads">Alignment between reference genome and reads.</param>
        /// <returns>List of DeltaAlignments after resolving repeating reads.</returns>
        public static IEnumerable<DeltaAlignment> ResolveAmbiguity(DeltaAlignmentCollection alignmentBetweenReferenceAndReads)
        {
            if (alignmentBetweenReferenceAndReads == null)
            {
                throw new ArgumentNullException("alignmentBetweenReferenceAndReads");
            }

            // Process reads and add to result list.
            // Loop till all reads are processed
            foreach (var curReadDeltas in alignmentBetweenReferenceAndReads.GetDeltaAlignmentsByReads())
            {
                if (curReadDeltas == null)
                    continue;

                int deltasInCurrentRead = curReadDeltas.Count;

                // If curReadDeltas has only one delta, then there are no repeats so add it to result
                // Or if any delta is a partial alignment, dont try to resolve, add all deltas to result
                if (deltasInCurrentRead == 1 || curReadDeltas.Any(a => a.SecondSequenceEnd != a.QuerySequence.Count - 1))
                {
                    //result.AddRange(curReadDeltas);
                    foreach (var delta in curReadDeltas)
                    {
                        yield return delta;
                    }
                }
                else if (deltasInCurrentRead == 0)
                {
                    continue;
                }
                else
                {
                    // Resolve repeats
                    string sequenceId = curReadDeltas[0].QuerySequence.ID;
                    string originalSequenceId;
                    bool forwardRead;
                    string pairedReadType;
                    string libraryName;

                    bool pairedRead =Helper.ValidatePairedSequenceId(sequenceId, out originalSequenceId, out forwardRead,out pairedReadType, out libraryName);

                    // If read is not having proper ID, ignore the read
                    if (!pairedRead)
                    {
                        //result.AddRange(curReadDeltas);
                        foreach (var delta in curReadDeltas)
                        {
                            yield return delta;
                        } 
                        
                        continue;
                    }

                    string pairedReadId = Helper.GetPairedReadId(originalSequenceId, Helper.GetMatePairedReadType(pairedReadType), libraryName);

                    // Find mate pair
                    List<DeltaAlignment> mateDeltas = alignmentBetweenReferenceAndReads.GetDeltaAlignmentFor(pairedReadId);

                    // If mate pair not found, ignore current read
                    if (mateDeltas.Count == 0)
                    {
                        //result.AddRange(curReadDeltas);
                        foreach (var delta in curReadDeltas)
                        {
                            yield return delta;
                        }
                        continue;
                    }

                    // Resolve using distance method
                    List<DeltaAlignment> resolvedDeltas = ResolveRepeatUsingMatePair(curReadDeltas, mateDeltas, libraryName);
                    if (resolvedDeltas != null)
                    {
                        //result.AddRange(resolvedDeltas);
                        foreach (var delta in resolvedDeltas)
                        {
                            yield return delta;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resolve repeats between two sets of deltas coming from paired reads
        /// </summary>
        /// <param name="curReadDeltas">Deltas from a read</param>
        /// <param name="mateDeltas">Deltas from mate pair</param>
        /// <param name="libraryName">Clone Library name to use</param>
        /// <returns>Selected delta out of all given deltas</returns>
        private static List<DeltaAlignment> ResolveRepeatUsingMatePair(List<DeltaAlignment> curReadDeltas, List<DeltaAlignment> mateDeltas, string libraryName)
        {
            // Check if all mate pairs are completely aligned, else return null (cannot resolve)
            if (mateDeltas.Any(a => a.SecondSequenceEnd != a.QuerySequence.Count - 1))
            {
                return null;
            }

            // Get clone library information
            CloneLibraryInformation libraryInfo = CloneLibrary.Instance.GetLibraryInformation(libraryName);
            float mean = libraryInfo.MeanLengthOfInsert;
            float stdDeviation = libraryInfo.StandardDeviationOfInsert;

            // Find delta with a matching distance.
            for(int indexFR =0;indexFR<curReadDeltas.Count; indexFR++)
            {
                DeltaAlignment pair1 = curReadDeltas[indexFR];
                for (int indexRR =0;indexRR<mateDeltas.Count;indexRR++)
                {
                    DeltaAlignment pair2 = mateDeltas[indexRR];
                    long distance = Math.Abs(pair1.FirstSequenceStart - pair2.FirstSequenceEnd);

                    // Find delta with matching distance.
                    if (distance - mean <= stdDeviation)
                    {
                        List<DeltaAlignment> resolvedDeltas = new List<DeltaAlignment>(2) { pair1, pair2 };

                        return resolvedDeltas;
                    }
                }
            }

            return null;
        }
    }
}
