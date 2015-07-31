using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bio.SimilarityMatrices;
using System.Linq;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Static functions for Multiple Sequence Alignment
    /// </summary>
    public static class MsaUtils
    {
        private static float EPSILON = 0.001f;

        /// <summary>
        /// Normalize a vector
        /// so that the summation of the vector is 1
        /// </summary>
        /// <param name="counts">float array</param>
        public static void Normalize(float[] counts)
        {
            float s = counts.Sum();
            if (Math.Abs(s - 0) < EPSILON)
            {
                //throw new Exception("The summation of the vector is 0");
                return;
            }
            for (int i = 0; i < counts.Length; ++i)
            {
                counts[i] /= s;
            }
        }

        /// <summary>
        /// Normalize a vector
        /// so that summation of the vector is 1
        /// 
        /// The input int[] is converted to float[]
        /// </summary>
        /// <param name="counts">integer array</param>
        public static float[] Normalize(int[] counts)
        {
            int s = 0;
            for (int i = 0; i < counts.Length; ++i)
            {
                if (counts[i] < 0)
                {
                    throw new Exception("counts cannot be negative");
                }
                s += counts[i];
            }
            if (s == 0)
            {
                throw new Exception("The summation of the vector is 0");
            }
            float[] _result = new float[counts.Length];
            for (int i = 0; i < counts.Length; ++i)
            {
                _result[i] = (float)counts[i] / s;
            }
            return _result;
        }

        /// <summary>
        /// Normalize values in a dictionary
        /// so that the summation of values is 1.
        /// </summary>
        /// <param name="countsD">kmer dictionary</param>
        public static void Normalize(Dictionary<String, float> countsD)
        {
            float s = countsD.Sum(pair => pair.Value);
            if (Math.Abs(s - 0) < EPSILON)
                throw new Exception("The summation of the vector is 0");

            foreach (String key in new List<String>(countsD.Keys))
            {
                countsD[key] /= s;
            }
        }

        /// <summary>
        /// Find and return the maximum item's index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        public static int FindMaxIndex<T>(T[] vector) where T : System.IComparable<T>
        {
            if (vector.Length <= 0)
            {
                throw new Exception("Unvalidated array");
            }
            T min = vector[0];
            int result = 0;
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i].CompareTo(min) > 0)
                {
                    min = vector[i];
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate the mean of a vector
        /// </summary>
        /// <param name="vector">a non-empty vector</param>
        public static float Mean(float[] vector)
        {
            if (vector.Length <= 0)
            {
                throw new Exception("Unvalidated array");
            }

            float result = 0;
            foreach (float i in vector)
            {
                result += i;
            }
            result /= vector.Length;
            return result;
        }

        /// <summary>
        /// Calculate variance
        /// </summary>
        /// <param name="vector">a non-empty vector</param>
        public static float Variance(float[] vector)
        {
            float mean = Mean(vector);

            float sum = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                sum += (float)Math.Pow((vector[i] - mean), 2);
            }
            return (float)(sum / vector.Length);
        }

        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        /// <param name="vector">a non-empty vector</param>
        public static float StandardDeviation(float[] vector)
        {
            return (float)Math.Sqrt(Variance(vector));
        }

        /// <summary>
        /// Calculate Pearson correlation between two vectors
        /// </summary>
        /// <param name="vectorA">the first non-empty vector</param>
        /// <param name="vectorB">the second non-empty vector</param>
        public static float Correlation(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new Exception("Length of sources is different");
            }
            float averageX = Mean(vectorA);
            float standardDeviationX = StandardDeviation(vectorA);
            float averageY = Mean(vectorB);
            float standardDeviationY = StandardDeviation(vectorB);
            int length = vectorA.Length;

            float correlation = 0;
            for (int i = 0; i < length; i++)
            {
                correlation += (vectorA[i] - averageX) * (vectorB[i] - averageY);
            }
            correlation = (float)(correlation / (standardDeviationX * standardDeviationY) / length);
            return correlation;
        }

        /// <summary>
        /// Calculate Kullback-Leibler distance between two vectors.
        /// D(vectorA, vectorB) = \sum_i [ vectorA[i] * log2 (vectorA[i] / vectorB[i])]
        /// </summary>
        /// <param name="vectorA">the first input vector</param>
        /// <param name="vectorB">the second input vector</param>
        public static float KullbackLeiblerDistance(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new Exception("Inputs are not equal in length");
            }
            float result = 0;

            for (int i = 0; i < vectorA.Length; ++i)
            {
                if (vectorA[i] != 0 && vectorB[i] != 0)
                {
                    result += vectorA[i] * (float)Math.Log(vectorA[i] / vectorB[i], 2);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate symmetrized entropy between two vectors.
        /// D(vectorA, vectorB) = (KullbackLeiblerDistance(vectorA, vectorB) + KullbackLeiblerDistance(vectorB, vectorA)) /2
        /// </summary>
        /// <param name="vectorA">the first input vector</param>
        /// <param name="vectorB">the second input vector</param>
        public static float SymmetrizedEntropy(float[] vectorA, float[] vectorB)
        {
            return (KullbackLeiblerDistance(vectorA, vectorB) + KullbackLeiblerDistance(vectorB, vectorA)) / 2;
        }

        /// <summary>
        /// Calculate Jensen-Shannon divergence of two vectors.
        /// D(vectorA, vectorB) = (KullbackLeiblerDistance(vectorA, Average(vectorA, vectorB)) + KullbackLeiblerDistance(vectorB, Average(vectorA, vectorB))) /2
        /// </summary>
        /// <param name="vectorA">the first input vector</param>
        /// <param name="vectorB">the second input vector</param>
        public static float JensenShannonDivergence(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new Exception("Inputs are not equal in length");
            }
            float[] average = new float[vectorA.Length];

            for (int i = 0; i < vectorA.Length; ++i)
            {
                average[i] = (vectorA[i] + vectorB[i]) / 2;
            }

            return (KullbackLeiblerDistance(vectorA, average) + KullbackLeiblerDistance(vectorB, average)) / 2;
        }


        /// <summary>
        /// Calculate pairwise score of a pair of aligned sequences.
        /// The score is the sum over all position score given by the similarity matrix.
        /// The positions with only indels, e.g. gaps, are discarded. Gaps in the remaining 
        /// columns are assessed affined score: g + w * e, where g is open penalty, and e
        /// is extension penalty.
        /// </summary>
        /// <param name="sequenceA">aligned sequence</param>
        /// <param name="sequenceB">aligned sequence</param>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="gapOpenPenalty">negative open gap penalty</param>
        /// <param name="gapExtensionPenalty">negative extension gap penalty</param>
        public static float PairWiseScoreFunction(ISequence sequenceA, ISequence sequenceB, SimilarityMatrix similarityMatrix,
                                                int gapOpenPenalty, int gapExtensionPenalty)
        {
            if (sequenceA.Count != sequenceB.Count)
            {
                throw new Exception("Unaligned sequences");
            }
            float result = 0;

            bool isGapA = false;
            bool isGapB = false; // used for flagging gaps while creating gaps in alignment

            bool isSourceitemAGap = false;
            bool isSourceitemBGap = false; // used for flagging if the source sequence item is a gap

            for (int i = 0; i < sequenceA.Count; ++i)
            {
                isSourceitemAGap = sequenceA.Alphabet.CheckIsGap(sequenceA[i]);
                isSourceitemBGap = sequenceB.Alphabet.CheckIsGap(sequenceB[i]);

                if (isSourceitemAGap && isSourceitemBGap)
                {
                    continue;
                }
                if (isSourceitemAGap && !isSourceitemBGap)
                {
                    if (isGapB)
                    {
                        isGapB = false;
                    }
                    if (isGapA)
                    {
                        result += gapExtensionPenalty;
                    }
                    else
                    {
                        result += gapOpenPenalty;
                        isGapA = true;
                    }
                    continue;
                }
                if (!isSourceitemAGap && isSourceitemBGap)
                {
                    if (isGapA)
                    {
                        isGapA = false;
                    }
                    if (isGapB)
                    {
                        result += gapExtensionPenalty;
                    }
                    else
                    {
                        result += gapOpenPenalty;
                        isGapB = true;
                    }
                    continue;
                }

                result += similarityMatrix[sequenceA[i], sequenceB[i]];
            }
            return result / sequenceA.Count;
        }

        /// <summary>
        /// Calculate alignment score of a set of aligned sequences.
        /// The score is the average over all pairs of sequences of their pairwise alignment score.
        /// </summary>
        /// <param name="sequences">a set of aligned sequences</param>
        /// <param name="similarityMatrix">similarity matrix</param>
        /// <param name="gapOpenPenalty">negative open gap penalty</param>
        /// <param name="gapExtensionPenalty">negative extension gap penalty</param>
        public static float MultipleAlignmentScoreFunction(IList<ISequence> sequences, SimilarityMatrix similarityMatrix,
                                                                int gapOpenPenalty, int gapExtensionPenalty)
        {
            double result = 0;
            object threadLock = new object();

            Parallel.For<double>(0, sequences.Count - 1, () => 0, (i, loop, subtotal) =>
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    subtotal += PairWiseScoreFunction(sequences[i], sequences[j], similarityMatrix, gapOpenPenalty,
                        gapExtensionPenalty);
                }

                return subtotal;
            }, (x) =>
            {
                lock (threadLock)
                {
                    result += x;
                }
            });
   
            return (float)(Math.Round((result /= sequences.Count * (sequences.Count - 1) / 2), 6));
        }

        /// <summary>
        /// Remove the gaps in a sequence to make it unaligned.
        /// </summary>
        /// <param name="alignedSequence">An aligned sequence with gaps</param>
        public static ISequence UnAlign(ISequence alignedSequence)
        {
            //ISequence unalignedSequence = new Sequence(alignedSequence.Alphabet);
            List<byte> seqBytes = new List<byte>((int)alignedSequence.Count);
            seqBytes.AddRange(alignedSequence.Where(t => !alignedSequence.Alphabet.CheckIsGap(t)));

            return new Sequence(alignedSequence.Alphabet, seqBytes.ToArray())
            {
                ID = alignedSequence.ID,
            };
        }

        /// <summary>
        /// Remove the gaps in a set of sequences to make them unaligned.
        /// </summary>
        /// <param name="alignedSequences">a set of sequences</param>
        public static List<ISequence> UnAlign(IList<ISequence> alignedSequences)
        {
            if (alignedSequences.Count == 0)
                throw new ArgumentException("Invalid input sequences");

            return new List<ISequence>(alignedSequences.Select(UnAlign));
        }

        /// <summary>
        /// Map the position index of query sequence residues in the reference sequence.
        /// Positive number is the position index; -1 is a gap.
        /// The two sequences need to have the same set of residues in the same order.
        /// </summary>
        /// <param name="sequence">query sequence</param>
        /// <param name="sequenceRef">reference sequence</param>
        public static List<int> CalculateOffset(ISequence sequence, ISequence sequenceRef)
        {
            List<int> result = new List<int>();
            int indexA = 0, indexRef = 0;

            while (indexA < sequence.Count && indexRef < sequenceRef.Count)
            {
                while (indexA < sequence.Count && sequence.Alphabet.CheckIsGap(sequence[indexA]))
                {
                    result.Add(-1);
                    ++indexA;
                }
                while (indexRef < sequenceRef.Count && sequenceRef.Alphabet.CheckIsGap(sequenceRef[indexRef]))
                {
                    ++indexRef;
                }
                result.Add(indexRef);
                ++indexA;
                ++indexRef;
            }
            while (indexA < sequence.Count)
            {
                result.Add(-1);
                ++indexA;
            }
            return result;
        }

        /// <summary>
        /// Q score: the number of correctly aligned residue pairs divided by the number of residue pairs in the reference alignment
        /// </summary>
        /// <param name="sequences">aligned sequences</param>
        /// <param name="sequencesRef">reference alignments from benchmark database</param>
        public static float CalculateAlignmentScoreQ(IList<ISequence> sequences, IList<ISequence> sequencesRef)
        {
            if (sequences.Count != sequencesRef.Count)
            {
                throw new ArgumentException("Inputs have different number of sequences");
            }
            if (sequences.Count == 0)
            {
                throw new ArgumentException("Empty input sequences");
            }

            List<List<int>> offsets = new List<List<int>>(sequences.Count);
            for (int i = 0; i < sequences.Count; ++i)
            {
                List<int> offset = CalculateOffset(sequences[i], sequencesRef[i]);
                offsets.Add(offset);
            }

            int numberOfCorrectResidues = 0;
            int sequenceLength = (int)sequences[0].Count;
            int sequenceLengthRef = (int)sequencesRef.ElementAt(0).Count;

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    for (int k = 0; k < sequenceLength; ++k)
                    {
                        if (offsets[i][k] == -1 && offsets[j][k] != -1)
                        {
                            try
                            {
                                if (sequencesRef[i].Alphabet.CheckIsGap(sequencesRef[i][offsets[j][k]]))
                                {
                                    ++numberOfCorrectResidues;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                            }
                        }
                        else if (offsets[j][k] == -1 && offsets[i][k] != -1)
                        {
                            try
                            {
                                if (sequencesRef[j].Alphabet.CheckIsGap(sequencesRef[j][offsets[i][k]]))
                                {
                                    ++numberOfCorrectResidues;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                            }
                        }
                        else if (offsets[j][k] != -1 && offsets[i][k] != -1)
                        {
                            if (offsets[i][k] == offsets[j][k])
                            {
                                ++numberOfCorrectResidues;
                            }
                        }
                    }
                }
            }
            return (float)numberOfCorrectResidues / (float)sequenceLengthRef / (sequences.Count * (sequences.Count - 1) / 2);
        }

        /// <summary>
        /// TC score: the number of correctly aligned columns divided by the number of columns in the reference alignment
        /// </summary>
        /// <param name="sequences">aligned sequences</param>
        /// <param name="sequencesRef">reference alignments from benchmark database</param>
        public static float CalculateAlignmentScoreTC(IList<ISequence> sequences, IList<ISequence> sequencesRef)
        {
            if (sequences.Count != sequencesRef.Count)
            {
                throw new ArgumentException("Inputs have different number of sequences");
            }
            if (sequences.Count == 0)
            {
                throw new ArgumentException("Empty input sequences");
            }

            List<List<int>> offsets = new List<List<int>>(sequences.Count);
            for (int i = 0; i < sequences.Count; ++i)
            {
                List<int> offset = CalculateOffset(sequences[i], sequencesRef[i]);
                offsets.Add(offset);
            }

            int numberOfCorrectColumns = 0;
            int sequenceLength = (int)sequences[0].Count;
            int sequenceLengthRef = (int)sequencesRef[0].Count;

            for (int k = 0; k < sequenceLength; ++k)
            {
                bool allEqual = true;
                for (int i = 0; i < sequences.Count - 1; ++i)
                {
                    for (int j = i + 1; j < sequences.Count; ++j)
                    {

                        if (offsets[i][k] == -1 && offsets[j][k] != -1)
                        {
                            try
                            {
                                if (!sequencesRef[i].Alphabet.CheckIsGap(sequencesRef[i][offsets[j][k]]))
                                {
                                    allEqual = false;
                                    break;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                            }
                        }
                        else if (offsets[j][k] == -1 && offsets[i][k] != -1)
                        {
                            try
                            {
                                if (!sequencesRef[j].Alphabet.CheckIsGap(sequencesRef[j][offsets[i][k]]))
                                {
                                    allEqual = false;
                                    break;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                            }
                        }
                        else if (offsets[j][k] != -1 && offsets[i][k] != -1)
                        {
                            if (offsets[i][k] != offsets[j][k])
                            {
                                allEqual = false;
                                break;
                            }
                        }
                    }
                    if (!allEqual)
                    {
                        break;
                    }
                }
                if (allEqual)
                {
                    ++numberOfCorrectColumns;
                }
            }
            return (float)numberOfCorrectColumns / (float)sequenceLengthRef;
        }

        /// <summary>
        /// Given molecule type, construct ItemSet, AmbiguousCharactersMap for Profiles class
        /// </summary>
        /// <param name="alphabetType">molecule type: DNA, RNA or Protein</param>
        public static void SetProfileItemSets(IAlphabet alphabetType)
        {

            // Get sequenceItem-index mapping dictionary ready
            ISequence templateSequence = null;
            Dictionary<byte, List<byte>> ambiguousCharacterMap = new Dictionary<byte, List<byte>>();
            int numberOfBasicResudes;
            byte[] basics;

            if (alphabetType is DnaAlphabet)
            {
                templateSequence = new Sequence(Alphabets.AmbiguousDNA, "ATGCSWRYKMBVHDN-");
                basics = new byte[2] { Alphabets.DNA.A, Alphabets.DNA.C };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.AC, new List<byte>(basics));
                basics = new byte[2] { Alphabets.DNA.G, Alphabets.DNA.C };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.GC, new List<byte>(basics));
                basics = new byte[2] { Alphabets.DNA.A, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.AT, new List<byte>(basics));
                basics = new byte[2] { Alphabets.DNA.A, Alphabets.DNA.G };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.GA, new List<byte>(basics));
                basics = new byte[2] { Alphabets.DNA.C, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.TC, new List<byte>(basics));
                basics = new byte[2] { Alphabets.DNA.G, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.GT, new List<byte>(basics));
                basics = new byte[3] { Alphabets.DNA.C, Alphabets.DNA.G, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.GTC, new List<byte>(basics));
                basics = new byte[3] { Alphabets.DNA.A, Alphabets.DNA.C, Alphabets.DNA.G };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.GCA, new List<byte>(basics));
                basics = new byte[3] { Alphabets.DNA.A, Alphabets.DNA.C, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.ACT, new List<byte>(basics));
                basics = new byte[3] { Alphabets.DNA.A, Alphabets.DNA.G, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.GAT, new List<byte>(basics));
                basics = new byte[4] { Alphabets.DNA.A, Alphabets.DNA.C, Alphabets.DNA.G, Alphabets.DNA.T };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousDNA.Any, new List<byte>(basics));
                numberOfBasicResudes = 4;
            }
            else if (alphabetType is ProteinAlphabet)
            {
                templateSequence = new Sequence(Alphabets.AmbiguousProtein, "ARNDCQEGHILKMFPSTWYVBJZX*-");
                basics = new byte[2] { Alphabets.Protein.N, Alphabets.AmbiguousProtein.D };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousProtein.B, new List<byte>(basics));
                basics = new byte[2] { Alphabets.Protein.L, Alphabets.Protein.I };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousProtein.J, new List<byte>(basics));
                basics = new byte[2] { Alphabets.Protein.Q, Alphabets.Protein.Q };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousProtein.Z, new List<byte>(basics));
                basics = new byte[0] { };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousProtein.X, new List<byte>(basics));
                numberOfBasicResudes = 20;
            }
            else if (alphabetType is RnaAlphabet)
            {
                templateSequence = new Sequence(Alphabets.AmbiguousRNA, "AUGCSWRYKMBVHDN-");
                basics = new byte[2] { Alphabets.RNA.A, Alphabets.RNA.C };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.AC, new List<byte>(basics));
                basics = new byte[2] { Alphabets.RNA.G, Alphabets.RNA.C };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.GC, new List<byte>(basics));
                basics = new byte[2] { Alphabets.RNA.A, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.AU, new List<byte>(basics));
                basics = new byte[2] { Alphabets.RNA.A, Alphabets.RNA.G };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.GA, new List<byte>(basics));
                basics = new byte[2] { Alphabets.RNA.C, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.UC, new List<byte>(basics));
                basics = new byte[2] { Alphabets.RNA.G, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.GU, new List<byte>(basics));
                basics = new byte[3] { Alphabets.RNA.C, Alphabets.RNA.G, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.GUC, new List<byte>(basics));
                basics = new byte[3] { Alphabets.RNA.A, Alphabets.RNA.C, Alphabets.RNA.G };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.GCA, new List<byte>(basics));
                basics = new byte[3] { Alphabets.RNA.A, Alphabets.RNA.C, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.ACU, new List<byte>(basics));
                basics = new byte[3] { Alphabets.RNA.A, Alphabets.RNA.G, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.GAU, new List<byte>(basics));
                basics = new byte[4] { Alphabets.RNA.A, Alphabets.RNA.C, Alphabets.RNA.G, Alphabets.RNA.U };
                ambiguousCharacterMap.Add(Alphabets.AmbiguousRNA.Any, new List<byte>(basics));
                numberOfBasicResudes = 4;
            }
            else
            {
                throw new Exception("Invalid molecular type");
            }

            // Add lowercase symbols to ambiguous symbol map as well
            Dictionary<byte, List<byte>> tmpCharacterMap = new Dictionary<byte, List<byte>>();
            foreach (var item in ambiguousCharacterMap)
            {
                tmpCharacterMap.Add((byte)char.ToLower((char)item.Key), item.Value);
            }
            foreach (var item in tmpCharacterMap)
            {
                ambiguousCharacterMap.Add(item.Key, item.Value);
            }

            Dictionary<byte, int> itemSet = new Dictionary<byte, int>();
            for (int i = 0; i < numberOfBasicResudes; ++i)
            {
                itemSet.Add(templateSequence[i], i);
                itemSet.Add((byte)char.ToLower((char)templateSequence[i]), i);
            }
            itemSet.Add(templateSequence[templateSequence.Count - 1], numberOfBasicResudes);
            Profiles.ItemSet = itemSet;
            Profiles.AmbiguousCharactersMap = ambiguousCharacterMap;
            Profiles.NumberOfBasicCharacters = numberOfBasicResudes;
        }

        /// <summary>
        /// Create the indices of an array
        /// </summary>
        /// <param name="length">The length of the array</param>
        public static int[] CreateIndexArray(int length)
        {
            int[] result = new int[length];
            for (int i = 0; i < length; ++i)
            {
                result[i] = i;
            }
            return result;
        }

        /// <summary>
        /// profileAlignment.ProfilesMatrix.ColumnSize
        /// </summary>
        /// <param name="inputArray">The array that will be sorted</param>
        /// <param name="inputIndex">The input array indices</param>
        /// <param name="begin">Start position</param>
        /// <param name="end">End position</param>
        public static void QuickSortM(float[] inputArray, out int[] inputIndex, int begin, int end)
        {
            int length = inputArray.Length;
            inputIndex = CreateIndexArray(length);
            float[] inputArrayCopy = new float[length];
            //inputArrayCopy = inputArray;
            for (int i = 0; i < length; ++i)
            {
                inputArrayCopy[i] = inputArray[i];
            }
            QuickSort(inputArrayCopy, inputIndex, begin, end);
        }

        /// <summary>
        /// Sort by quicksort algorithm
        /// </summary>
        /// <param name="inputArray">The array that will be sorted</param>
        /// <param name="inputIndex">The input array indices</param>
        /// <param name="begin">Start position</param>
        /// <param name="end">End position</param>
        public static float[] QuickSort(float[] inputArray, int[] inputIndex, int begin, int end)
        {
            if (begin < end)
            {
                int q = Partition(inputArray, inputIndex, begin, end);
                inputArray = QuickSort(inputArray, inputIndex, begin, q);
                inputArray = QuickSort(inputArray, inputIndex, q + 1, end);
            }
            return inputArray;
        }
        /// <summary>
        /// Sub-operation in QuickSort algorithm
        /// </summary>
        /// <param name="inputArray">The array that will be sorted</param>
        /// <param name="inputIndex">The input array indices</param>
        /// <param name="p">Start position</param>
        /// <param name="r">End position</param>
        private static int Partition(float[] inputArray, int[] inputIndex, int p, int r)
        {
            float x = inputArray[p];
            int i = p - 1;
            int j = r + 1;
            float tmp = 0;
            int tmpi = 0;
            while (true)
            {
                do
                {
                    j--;
                } while (inputArray[j] < x);

                do
                {
                    i++;
                } while (inputArray[i] > x);
                if (i < j)
                {
                    tmp = inputArray[i];
                    inputArray[i] = inputArray[j];
                    inputArray[j] = tmp;

                    tmpi = inputIndex[i];
                    inputIndex[i] = inputIndex[j];
                    inputIndex[j] = tmpi;
                }
                else return j;
            }
        } 
    }
}
