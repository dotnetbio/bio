using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Variant
{
    /// <summary>
    /// A simple struct that represents a base and quality value from a sequencing read.
    /// </summary>
    public struct BaseAndQuality
    {
        /// <summary>
        /// The base present at this position (either: A, C, G, T , N or '-' encoded as 0-5);
        /// </summary>
        public byte Base;

        /// <summary>
        /// The log base 10 probability that the base is correct.
        /// </summary>
        public double Log10ProbCorrect;

        /// <summary>
        /// Create a new base/probability pair.
        /// </summary>
        /// <param name="bp">A, C, G, T, N or '-'</param>
        /// <param name="log10ProbCorrect">The probability that the base is </param>
        public BaseAndQuality(byte bp, double log10ProbCorrect)
        {
            var baseIndex = validBases[bp];
            var validData = baseIndex > 0 && log10ProbCorrect <= 0;
            if (!validData)
            {
                throw new ArgumentException("Tried to create a BaseAndQual field with invalid data (BP = " + bp.ToString() + " Qual = " +
                                    log10ProbCorrect.ToString() + ").");
            }
            Base = (byte) (baseIndex - 1);
            Log10ProbCorrect = log10ProbCorrect;
        }
        /// <summary>
        /// An array with >0 values A, C, G, T, N and '-'
        /// Can be used both to validate if a base is acceptable (element > 0) and
        /// determine a lower order encoding for the base ( = element - 1).
        /// </summary>
        static readonly int[] validBases = new int[byte.MaxValue + 1];

        /// <summary>
        /// A list of valid basepairs as well as their ordering when projected down to a smaller array.
        /// e.g. We change the encoding as follows:
        /// 'A' = 0
        /// 'C' = 1
        /// 'G' = 2
        /// 'T' = 3
        /// 'N' = 4
        /// '-' = 5
        /// </summary>
        public static readonly char[] BasesInOrder = new char[] { 'A', 'C', 'G', 'T', 'N', '-' };
        static BaseAndQuality()
        {
            int i = 1;
            foreach (var bp in valid)
            {
                validBases[bp] = i++;
            }
        }
    }
}

