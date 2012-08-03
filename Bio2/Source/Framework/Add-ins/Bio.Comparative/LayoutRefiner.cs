using System;
using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using Bio.Util;
using System.IO;

namespace Bio.Algorithms.Assembly.Comparative
{
    /// <summary>
    /// Class Layout Refiner.
    /// </summary>
    public static class LayoutRefiner
    {
        /// <summary>
        /// Refines alignment layout by taking in consideration indels (insertions and deletions) and rearrangements between two genomes. 
        /// Requires mate-pair information to resolve ambiguity.
        /// </summary>
        /// <param name="orderedDeltas">Order deltas.</param>
        public static IEnumerable<DeltaAlignment> RefineLayout(DeltaAlignmentCollection orderedDeltas)
        {
            if (orderedDeltas == null)
            {
                throw new ArgumentNullException("orderedDeltas");
            }

            if (orderedDeltas.Count == 0)
            {
                yield break;
            }

            // As we dont know what is the maximum posible insert and deltes, 
            // assuming 1,000,000 deltas are sufficient for operation.
            int windowSize = 1000;

            VirtualDeltaAlignmentCollection deltaCatche = new VirtualDeltaAlignmentCollection(orderedDeltas, windowSize);

            List<DeltaAlignment> deltasOverlappingAtCurrentIndex = null;
            List<DeltaAlignment> leftSideDeltas = null;
            List<DeltaAlignment> rightSideDeltas = null;
            List<DeltaAlignment> unloadedDeltas = null;
            try
            {
                deltasOverlappingAtCurrentIndex = new List<DeltaAlignment>();
                leftSideDeltas = new List<DeltaAlignment>();
                rightSideDeltas = new List<DeltaAlignment>();

                long currentProcessedOffset = 0;
                DeltaAlignment alignment = deltaCatche[0];
                deltasOverlappingAtCurrentIndex.Add(alignment);
                DeltaAlignment deltaWithLargestEndIndex = alignment;

                for (int currentIndex = 0; currentIndex < deltaCatche.Count - 1; currentIndex++)
                {
                    DeltaAlignment nextDelta = deltaCatche[currentIndex + 1];
                    unloadedDeltas = null;
                    if (deltaCatche.TryUnload(currentIndex + 1, out unloadedDeltas))
                    {
                        for (int i = 0; i < unloadedDeltas.Count; i++)
                        {
                            yield return unloadedDeltas[i];
                        }

                        unloadedDeltas.Clear();
                    }

                    if (currentProcessedOffset != 0)
                    {
                        nextDelta.FirstSequenceStart += currentProcessedOffset;
                        nextDelta.FirstSequenceEnd += currentProcessedOffset;
                    }

                    // Check if next delta is just adjacent
                    if (nextDelta.FirstSequenceStart - 1 == deltaWithLargestEndIndex.FirstSequenceEnd)
                    {
                        // If next delta is adjacent there is a possible insertion in target (deletion in reference)
                        // Try to extend the deltas from both sides and make them meet
                        leftSideDeltas.Clear();
                        for (int index = 0; index < deltasOverlappingAtCurrentIndex.Count; index++)
                        {
                            DeltaAlignment delta = deltasOverlappingAtCurrentIndex[index];
                            if (delta.FirstSequenceEnd >= deltaWithLargestEndIndex.FirstSequenceEnd)
                            {
                                leftSideDeltas.Add(delta);
                            }
                        }

                        // Find all deltas starting at the adjacent right side
                        rightSideDeltas.Clear();
                        for (long index = currentIndex + 1; index < deltaCatche.Count; index++)
                        {
                            DeltaAlignment delta = deltaCatche[index];
                            unloadedDeltas = null;
                            if (deltaCatche.TryUnload(currentIndex + 1, out unloadedDeltas))
                            {
                                for (int i = 0; i < unloadedDeltas.Count; i++)
                                {
                                    yield return unloadedDeltas[i];
                                }

                                unloadedDeltas.Clear();
                            }

                            if (delta.FirstSequenceStart != nextDelta.FirstSequenceStart)
                            {
                                break;
                            }

                            rightSideDeltas.Add(delta);
                        }

                        long offset = ExtendDeltas(leftSideDeltas, rightSideDeltas);

                        if (offset != 0)
                        {
                            nextDelta.FirstSequenceStart += offset;
                            nextDelta.FirstSequenceEnd += offset;
                        }

                        currentProcessedOffset += offset;
                    }
                    else
                        if (nextDelta.FirstSequenceStart <= deltaWithLargestEndIndex.FirstSequenceEnd)
                        {
                            // Check if next delta overlaps with current overlap group
                            deltasOverlappingAtCurrentIndex.Add(nextDelta);

                            // Check if nextDelta is reaching farther than the current farthest delta
                            if (nextDelta.FirstSequenceEnd > deltaWithLargestEndIndex.FirstSequenceEnd)
                            {
                                deltaWithLargestEndIndex = nextDelta;
                            }

                            if (deltasOverlappingAtCurrentIndex.Count > windowSize)
                            {
                                for (int i = deltasOverlappingAtCurrentIndex.Count - 1; i >= 0; i--)
                                {
                                    if (deltasOverlappingAtCurrentIndex[i].FirstSequenceEnd < deltaWithLargestEndIndex.FirstSequenceEnd)
                                    {
                                        deltasOverlappingAtCurrentIndex.RemoveAt(i);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // No overlap with nextDelta, so there is a gap at the end of deltaWithLargestEndIndex
                            // Try fix insertion in reference by pulling together two ends of deltas on both sides of the gap
                            leftSideDeltas.Clear();
                            for (int index = 0; index < deltasOverlappingAtCurrentIndex.Count; index++)
                            {
                                DeltaAlignment delta = deltasOverlappingAtCurrentIndex[index];
                                if (delta.FirstSequenceEnd >= deltaWithLargestEndIndex.FirstSequenceEnd)
                                {
                                    leftSideDeltas.Add(delta);
                                }
                            }

                            // Find all deltas starting at the right end of the gap
                            rightSideDeltas.Clear();
                            for (long index = currentIndex + 1; index < deltaCatche.Count; index++)
                            {
                                DeltaAlignment delta = deltaCatche[index];
                                unloadedDeltas = null;
                                if (deltaCatche.TryUnload(currentIndex + 1, out unloadedDeltas))
                                {
                                    for (int i = 0; i < unloadedDeltas.Count; i++)
                                    {
                                        yield return unloadedDeltas[i];
                                    }

                                    unloadedDeltas.Clear();
                                }

                                if (delta.FirstSequenceStart != nextDelta.FirstSequenceStart)
                                {
                                    break;
                                }

                                rightSideDeltas.Add(delta);
                            }

                            int score = 0;
                            for (int i = 0; i < leftSideDeltas.Count; i++)
                            {
                                var l = leftSideDeltas[i];
                                int j = 0;

                                for (; j < rightSideDeltas.Count; j++)
                                {
                                    var r = rightSideDeltas[j];

                                    // if (object.ReferenceEquals(l.QuerySequence, r.QuerySequence))
                                    // As reference check is not posible, verifying ids here. as id are unique for a given read.
                                    if (l.QuerySequence.ID == r.QuerySequence.ID)
                                    {
                                        score++;
                                        break;
                                    }
                                }

                                if (j == rightSideDeltas.Count)
                                {
                                    score--;
                                }
                            }

                            // Score > 0 means most deltas share same query sequence at both ends, so close this gap
                            if (score > 0)
                            {
                                long gaplength = (nextDelta.FirstSequenceStart - deltaWithLargestEndIndex.FirstSequenceEnd) - 1;
                                currentProcessedOffset -= gaplength;

                                // Pull deltas on right side to close the gap
                                for (int i = 0; i < rightSideDeltas.Count; i++)
                                {
                                    DeltaAlignment delta = rightSideDeltas[i];
                                    delta.FirstSequenceStart -= gaplength;
                                    delta.FirstSequenceEnd -= gaplength;
                                    // deltaCatche.Update(delta.Id);
                                }
                            }

                            // Start a new group from the right side of the gap
                            deltaWithLargestEndIndex = nextDelta;
                            deltasOverlappingAtCurrentIndex.Clear();
                            deltasOverlappingAtCurrentIndex.Add(nextDelta);
                        }
                }

                unloadedDeltas = deltaCatche.GetCachedDeltas();

                for (int i = 0; i < unloadedDeltas.Count; i++)
                {
                    yield return unloadedDeltas[i];
                }

                unloadedDeltas.Clear();
            }
            finally
            {
                if (deltasOverlappingAtCurrentIndex != null)
                {
                    deltasOverlappingAtCurrentIndex.Clear();
                    deltasOverlappingAtCurrentIndex = null;
                }

                if (leftSideDeltas != null)
                {
                    leftSideDeltas.Clear();
                    leftSideDeltas = null;
                }

                if (rightSideDeltas != null)
                {
                    rightSideDeltas.Clear();
                    rightSideDeltas = null;
                }

                if (deltaCatche != null)
                {
                    deltaCatche = null;
                }
            }
        }

        /// <summary>
        /// Extended Deltas.
        /// </summary>
        /// <param name="leftSideDeltas">Left Side Deltas.</param>
        /// <param name="rightSideDeltas">Right Side Deltas.</param>
        /// <returns>Returns Extend Deltas.</returns>
        private static long ExtendDeltas(List<DeltaAlignment> leftSideDeltas, List<DeltaAlignment> rightSideDeltas)
        {
            long extendedIndex = 1;
            int[] symbolCount = new int[255];
            List<byte> leftExtension = new List<byte>();
            List<byte> rightExtension = new List<byte>();
            #region left extension

            // Left extension
            do
            {
                symbolCount['A'] = symbolCount['C'] = symbolCount['G'] = symbolCount['T'] = 0;

                // loop through all queries at current index and find symbol counts
                for (int index = 0; index < leftSideDeltas.Count; index++)
                {
                    DeltaAlignment da = leftSideDeltas[index];

                    if (da.QuerySequence.Count > da.SecondSequenceEnd + extendedIndex)
                    {
                        char symbol = (char)da.QuerySequence[da.SecondSequenceEnd + extendedIndex];
                        symbolCount[char.ToUpperInvariant(symbol)]++;
                    }
                }

                // no symbols at current position, then break;
                if (symbolCount['A'] == 0 && symbolCount['C'] == 0 && symbolCount['G'] == 0 && symbolCount['T'] == 0)
                {
                    break;
                }

                // find symbol with max occurence
                byte indexLargest, indexSecond;
                FindLargestAndSecondLargest(symbolCount, out indexLargest, out indexSecond);

                // Dont extend if largest symbol count is higher than double of second largest symbol count
                if (symbolCount[indexSecond] > symbolCount[indexLargest] / 2)
                {
                    return 0;
                }

                leftExtension.Add(indexLargest); // index will be the byte value of the appropriate symbol

                extendedIndex++;
            } while (true);

            #endregion

            #region Right extension

            // Right extension
            extendedIndex = 1;
            do
            {
                symbolCount['A'] = symbolCount['C'] = symbolCount['G'] = symbolCount['T'] = 0;

                // loop through all queries at current index and find symbol counts
                for (int index = 0; index < rightSideDeltas.Count; index++)
                {
                    DeltaAlignment da = rightSideDeltas[index];

                    if (da.SecondSequenceStart - extendedIndex >= 0)
                    {
                        char symbol = (char)da.QuerySequence[da.SecondSequenceStart - extendedIndex];
                        symbolCount[char.ToUpperInvariant(symbol)]++;
                    }
                }

                // no symbols at current position, then break;
                if (symbolCount['A'] == 0 && symbolCount['C'] == 0 && symbolCount['G'] == 0 && symbolCount['T'] == 0)
                {
                    break;
                }

                // find symbol with max occurence
                byte indexLargest, indexSecond;
                FindLargestAndSecondLargest(symbolCount, out indexLargest, out indexSecond);

                // Dont extend if largest symbol count is higher than double of second largest symbol count
                if (symbolCount[indexSecond] > symbolCount[indexLargest] / 2)
                {
                    return 0;
                }

                rightExtension.Insert(0, indexLargest); // index will be the byte value of the appropriate symbol

                extendedIndex++;
            } while (true);

            #endregion

            // One of the side cannot be extended, so cancel extension
            if (leftExtension.Count == 0 || rightExtension.Count == 0)
            {
                return 0;
            }

            int overlapStart = FindMaxOverlap(leftExtension, rightExtension);

            if (overlapStart == -1)
            {
                return 0;
            }
            else
            {
                // Update left side deltas
                for (int index = 0; index < leftSideDeltas.Count; index++)
                {
                    var d = leftSideDeltas[index];
                    d.FirstSequenceEnd += (d.QuerySequence.Count - 1) - d.SecondSequenceEnd;
                    d.SecondSequenceEnd = d.QuerySequence.Count - 1;
                }

                // Update right side deltas
                int toRightOffset = rightExtension.Count + overlapStart;
                for (int index = 0; index < rightSideDeltas.Count; index++)
                {
                    var d = rightSideDeltas[index];
                    d.FirstSequenceStart += toRightOffset - d.SecondSequenceStart;

                    // Subtracting as all these deltas will be processed in the outer loop
                    d.FirstSequenceStart -= toRightOffset;
                    d.SecondSequenceStart = 0;
                }

                return toRightOffset;
            }
        }

        /// <summary>
        /// Find Max Overlap.
        /// </summary>
        /// <param name="leftExtension">Left Extension.</param>
        /// <param name="rightExtension">Right Extension.</param>
        /// <returns>Returns Max overLap.</returns>
        private static int FindMaxOverlap(List<byte> leftExtension, List<byte> rightExtension)
        {
            for (int left = 0; left < leftExtension.Count; left++)
            {
                int right;
                for (right = 0; right < leftExtension.Count - left && right < rightExtension.Count; right++)
                {
                    if (leftExtension[left + right] != rightExtension[right])
                    {
                        break;
                    }
                }

                if (right == leftExtension.Count - left)
                {
                    return left;
                }
            }

            return -1;
        }

        /// <summary>
        /// Find Largest And SecondLargest.
        /// </summary>
        /// <param name="values">Set of Values.</param>
        /// <param name="indexLargest">Out param largest index.</param>
        /// <param name="indexSecond">Out param Second largest index.</param>
        private static void FindLargestAndSecondLargest(int[] values, out byte indexLargest, out byte indexSecond)
        {
            indexLargest = indexSecond = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > values[indexLargest])
                {
                    indexSecond = indexLargest;
                    indexLargest = (byte)i;
                }
            }
        }

        /// <summary>
        /// Virtual DeltaAlingment collection.
        /// Holds 1000 times the windows size.
        /// </summary>
        private class VirtualDeltaAlignmentCollection
        {
            private int windowSize;
            private long startIndexInCatchedList;
            private long orderDeltasIndex = 0;
            private List<DeltaAlignment> catchedDeltas;
            private int catchSize;
            private IEnumerator<DeltaAlignment> sourceDeltas;

            /// <summary>
            /// Initializes a new instance of the VirtualDeltaAlignmentCollection class.
            /// </summary>
            /// <param name="orderedDeltas"></param>
            /// <param name="windowSize"></param>
            public VirtualDeltaAlignmentCollection(DeltaAlignmentCollection orderedDeltas, int windowSize)
            {
                this.windowSize = windowSize;
                this.Count = orderedDeltas.Count;
                this.catchedDeltas = new List<DeltaAlignment>();
                this.catchSize = windowSize * 1000;
                this.sourceDeltas = orderedDeltas.DeltaAlignmentParser.Parse().GetEnumerator();
            }

            /// <summary>
            /// Gets the number of delta alignments present in this instance.
            /// </summary>
            public long Count { get; private set; }

            /// <summary>
            /// Gets the delta alignment present at specified index.
            /// </summary>
            /// <param name="index">Index of the delta alignment required.</param>
            public DeltaAlignment this[long index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    long catchIndex = index - startIndexInCatchedList;

                    if (catchIndex < 0)
                    {
                        throw new ArgumentException("Catch size is not enough");
                    }

                    if (catchIndex >= this.catchedDeltas.Count)
                    {
                        LoadNextWindow();

                    }

                    return catchedDeltas[(int)catchIndex];
                }
            }

            /// <summary>
            /// Trys to unload the delta alignments from the cache.
            /// </summary>
            /// <param name="index">index at which current operations are taking place.
            /// This is required to judge whether to unload a window of delta alignment or not.</param>
            /// <param name="unloadedDeltas">If this method succeeded then unloaded deltas are passed back in this parameter.</param>
            /// <returns>Returns true if a cache window is unloaded else false.</returns>
            public bool TryUnload(long index, out List<DeltaAlignment> unloadedDeltas)
            {
                bool result = false;
                unloadedDeltas = null;
                long catchIndex = index - startIndexInCatchedList;
                if (catchIndex >= this.catchSize - windowSize)
                {
                    if (orderDeltasIndex + windowSize < this.Count)
                    {
                        // unload only when there is a data to load another window.
                        unloadedDeltas = Unload();
                        if (unloadedDeltas.Count > 0)
                        {
                            result = true;
                        }
                    }

                    if (orderDeltasIndex < this.Count)
                    {
                        LoadNextWindow();
                    }
                }

                return result;
            }

            /// <summary>
            /// Gets the cached deltas.
            /// </summary>
            public List<DeltaAlignment> GetCachedDeltas()
            {
                return this.catchedDeltas;
            }

            /// <summary>
            /// Unloads a cache window.
            /// </summary>
            /// <returns>Returns unloaded deltas.</returns>
            private List<DeltaAlignment> Unload()
            {
                List<DeltaAlignment> unloadedDeltas = new List<DeltaAlignment>();

                for (int i = 0; i < windowSize; i++)
                {
                    DeltaAlignment delta = this.catchedDeltas[i];
                    unloadedDeltas.Add(delta);
                }

                this.catchedDeltas.RemoveRange(0, windowSize);
                startIndexInCatchedList += windowSize;
                return unloadedDeltas;
            }

            /// <summary>
            /// Loads a window a deltas to the cache.
            /// </summary>
            private void LoadNextWindow()
            {
                for (long index = 1; index <= windowSize && orderDeltasIndex < this.Count; index++)
                {
                    if (sourceDeltas.MoveNext())
                    {
                        catchedDeltas.Add(sourceDeltas.Current);
                    }

                    orderDeltasIndex++;
                }
            }
        }
    }
}
