using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment;
using System;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Comparative
{
    /// <summary>
    /// Generates consensus of alignment (contigs) from alignment layout. 
    /// </summary>
    public static class ConsensusGeneration
    {
        /// <summary>
        /// Generates consensus sequences from alignment layout.
        /// </summary>
        /// <param name="alignmentBetweenReferenceAndReads">Input list of reads.</param>
        /// <returns>List of contigs.</returns>
        public static IEnumerable<ISequence> GenerateConsensus(DeltaAlignmentCollection alignmentBetweenReferenceAndReads)
        {
            if (alignmentBetweenReferenceAndReads == null)
            {
                throw new ArgumentNullException("alignmentBetweenReferenceAndReads");
            }

            SimpleConsensusResolver resolver = new SimpleConsensusResolver(AmbiguousDnaAlphabet.Instance, 49);

            // this dictionary will not grow more than a few hundread in worst scenario,
            // as this stores delta and its corresponding sequences 
            Dictionary<DeltaAlignment, ISequence> deltasInCurrentContig = new Dictionary<DeltaAlignment, ISequence>();

            long currentAlignmentStartOffset = 0;
            long currentIndex = 0;

            List<byte> currentContig = new List<byte>();
            List<DeltaAlignment> deltasToRemove = new List<DeltaAlignment>();

            // no deltas
            if (alignmentBetweenReferenceAndReads.Count == 0)
            {
                yield break;
            }

            long index = 0;

            DeltaAlignment lastDelta = alignmentBetweenReferenceAndReads[index];
            do
            {
                // Starting a new contig
                if (deltasInCurrentContig.Count == 0)
                {
                    currentAlignmentStartOffset = lastDelta.FirstSequenceStart;
                    currentIndex = 0;
                    currentContig.Clear();
                }

                // loop through all deltas at current index and find consensus
                do
                {
                    // Proceed creating consensus till we find another delta stats aligning
                    while (lastDelta != null && lastDelta.FirstSequenceStart == currentAlignmentStartOffset + currentIndex)
                    {
                        deltasInCurrentContig.Add(lastDelta, GetSequenceFromDelta(lastDelta));

                        // Get next delta
                        index++;
                        if (alignmentBetweenReferenceAndReads.Count > index)
                        {
                            lastDelta = alignmentBetweenReferenceAndReads[index];
                            continue; // see if new delta starts from the same offset
                        }
                        else
                        {
                            lastDelta = null;
                        }
                    }

                    byte[] symbolsAtCurrentIndex = new byte[deltasInCurrentContig.Count];
                    int symbolCounter = 0;

                    foreach (var delta in deltasInCurrentContig)
                    {
                        long inDeltaIndex = currentIndex - (delta.Key.FirstSequenceStart - currentAlignmentStartOffset);
                        symbolsAtCurrentIndex[symbolCounter++] = delta.Value[inDeltaIndex];

                        if (inDeltaIndex == delta.Value.Count - 1)
                        {
                            deltasToRemove.Add(delta.Key);
                        }
                    }

                    if (deltasToRemove.Count > 0)
                    {
                        for (int i = 0; i < deltasToRemove.Count; i++)
                        {
                            deltasInCurrentContig.Remove(deltasToRemove[i]);
                        }

                        deltasToRemove.Clear();
                    }

                    byte consensusSymbol = resolver.GetConsensus(symbolsAtCurrentIndex);
                    currentContig.Add(consensusSymbol);

                    currentIndex++;

                    // See if another delta is adjacent
                    if (deltasInCurrentContig.Count == 0 && lastDelta != null && lastDelta.FirstSequenceStart == currentAlignmentStartOffset + currentIndex)
                    {
                        deltasInCurrentContig.Add(lastDelta, GetSequenceFromDelta(lastDelta));

                        // check next delta
                        index++;
                        if (alignmentBetweenReferenceAndReads.Count > index)
                        {
                            lastDelta = alignmentBetweenReferenceAndReads[index];
                            continue; // read next delta to see if it starts from current reference sequence offset
                        }
                        else
                        {
                            lastDelta = null;
                        }
                    }
                }
                while (deltasInCurrentContig.Count > 0);

                yield return new Sequence(AmbiguousDnaAlphabet.Instance, currentContig.ToArray(), false);
            }
            while (lastDelta != null);
        }

        /// <summary>
        /// Gets the error removed sequence from the delta.
        /// </summary>
        /// <param name="deltaAlignment">DeltaAlignment instance.</param>
        private static ISequence GetSequenceFromDelta(DeltaAlignment deltaAlignment)
        {
            int indelListIndex = 0;
            long indelIndex = 0;
            long nextIndelPosition = 0;
            long indelCount = deltaAlignment.Deltas.Count;

            if (indelListIndex < indelCount)
            {
                indelIndex = deltaAlignment.Deltas[indelListIndex++];
            }

            nextIndelPosition = deltaAlignment.SecondSequenceStart - 1;
            nextIndelPosition += indelIndex >= 0 ? indelIndex : -indelIndex;


            long symbolsCount = deltaAlignment.SecondSequenceEnd - deltaAlignment.SecondSequenceStart + 1 +
                deltaAlignment.Deltas.Count(I => I > 0) - deltaAlignment.Deltas.Count(I => I < 0);

            long symbolIndex = 0;
            byte[] symbols = new byte[symbolsCount];

            for (long index = deltaAlignment.SecondSequenceStart; index <= deltaAlignment.SecondSequenceEnd; )
            {
                if (indelIndex != 0 && index == nextIndelPosition)
                {
                    if (indelIndex > 0)
                    {
                        // a symbol is deleted from the query, thus insert a gap symbol in query.
                        symbols[symbolIndex] = AmbiguousDnaAlphabet.Instance.Gap;
                        symbolIndex++;
                        nextIndelPosition--;
                    }
                    else
                    {
                        // a symbol is inserted to query, thus delete the symbol from query.
                        // skip one symbol from the query sequence.
                        index++;
                    }

                    // Get nextIndelPosition.
                    if (indelListIndex < indelCount)
                    {
                        indelIndex = deltaAlignment.Deltas[indelListIndex++];
                    }
                    else
                    {
                        indelIndex = 0;
                    }

                    nextIndelPosition += indelIndex >= 0 ? indelIndex : -indelIndex;
                }
                else
                {
                    symbols[symbolIndex] = deltaAlignment.QuerySequence[index];
                    symbolIndex++;
                    index++;
                }
            }

            return new Sequence(AmbiguousDnaAlphabet.Instance, symbols) { ID = deltaAlignment.QuerySequence.ID };
        }
    }
}
