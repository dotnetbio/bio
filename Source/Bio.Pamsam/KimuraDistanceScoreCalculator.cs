using System.Collections.Generic;
using System;
using Bio;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of Kimura distance score calculator class.
    /// 
    /// Kimura distance is additive mutation distance between two *aligned* sequences.
    /// Given the fractional identity D (percent identity), the mutation distance 
    /// can be approximated as 1-D. As sequences diverge, there is an increasing 
    /// probability of multiple mutations at a single site. Kimura correction is 
    /// used to correct this problem.
    /// 
    /// The class implements two functions: percent identity calculation, and 
    /// Kimura correction.
    /// </summary>
    public sealed class KimuraDistanceScoreCalculator
    {
        #region Static Methods

        /// <summary>
        /// Assigns a real number distance score to a pair of aligned sequences
        /// </summary>
        /// <param name="sequenceA">aligned sequence</param>
        /// <param name="sequenceB">aligned sequence</param>
        public static float CalculateDistanceScore(ISequence sequenceA, ISequence sequenceB)
        {
            if (sequenceA.Count != sequenceB.Count)
            {
                throw new ArgumentException("Unaligned sequences");
            }
            if (sequenceA.Alphabet != sequenceB.Alphabet)
            {
                throw new ArgumentException("Sequences use different Alphabets");
            }

            float percentIdentity = CalculatePercentIdentity(sequenceA, sequenceB);

            return KimuraFunction(percentIdentity);
        }

        /// <summary>
        /// Calculate percent of identical items between two aligned sequences.
        /// </summary>
        /// <param name="sequenceA">aligned sequence</param>
        /// <param name="sequenceB">aligned sequence</param>
        public static float CalculatePercentIdentity(ISequence sequenceA, ISequence sequenceB)
        {
            // calculate the percent of identical ISequenceItems between the two sequences
            float sameCount = 0;

            for (int i = 0; i < sequenceA.Count; ++i)
            {
                // Gaps are ignored.
                if (sequenceA.Alphabet.CheckIsGap(sequenceA[i]) || sequenceB.Alphabet.CheckIsGap(sequenceB[i]))
                {
                    continue;
                }
                if (sequenceA[i] == sequenceB[i])
                {
                    ++sameCount;
                }
            }
            return (sameCount / sequenceA.Count); 
        }

        /// <summary>
        /// Calculate Kimura score from percent identity
        /// Detailed can be find in MUSCLE Edgar 2004 paper.
        /// 
        /// The Kimura measure is defined to be:
        ///
        ///		log_e(1 - p - p*p/5)
        ///
        /// where p is the fraction of residues that differ, i.e.:
        ///
        ///		p = (1 - percent_identity)
        ///
        /// This measure is infinite for p = 0.8541 and is considered
        /// unreliable for p >= 0.75 (according to the ClustalW docs).
        /// </summary>
        /// <param name="percentIdentity">float percentIdentity</param>
        public static float KimuraFunction(float percentIdentity)
        {
            float distanceScore;

            // convert percentIdentity to Kimura Score
            double p = 1 - percentIdentity;
            // Typical case: use Kimura's empirical formula
            if (p < 0.75)
            {
                distanceScore = (float)(-(Math.Log(1 - p - ((p * p) / 5))));
            }
            // Per ClustalW, return 10.0 for anything over 93%
            else if (p > 0.93)
            {
                distanceScore = (float)10.0;
            }
            else
            {
                // Use table lookup
                int tableIndex = (int)((p - 0.75) * 1000 + 0.5);

                if (tableIndex < 0 || tableIndex >= dayhoffPams.Length)
                {
                    throw new IndexOutOfRangeException("Index out of range");
                }
                distanceScore = (float)(dayhoffPams[tableIndex] / 100.0);
            }

            return distanceScore;
        }
        #endregion

        // The following table was copied from the ClustalW file dayhoff.h.
        static int[] dayhoffPams = {
            195,    196,    197,    198,    199,    200,    200,    201,    202,  203,    
            204,    205,    206,    207,    208,    209,    209,    210,    211,  212,    
            213,    214,    215,    216,    217,    218,    219,    220,    221,  222,    
            223,    224,    226,    227,    228,    229,    230,    231,    232,  233,    
            234,    236,    237,    238,    239,    240,    241,    243,    244,  245,    
            246,    248,    249,    250,    252,    253,    254,    255,    257,  258,    
            260,    261,    262,    264,    265,    267,    268,    270,    271,  273,    
            274,    276,    277,    279,    281,    282,    284,    285,    287,  289,    
            291,    292,    294,    296,    298,    299,    301,    303,    305,  307,    
            309,    311,    313,    315,    317,    319,    321,    323,    325,  328,    
            330,    332,    335,    337,    339,    342,    344,    347,    349,  352,    
            354,    357,    360,    362,    365,    368,    371,    374,    377,  380,    
            383,    386,    389,    393,    396,    399,    403,    407,    410,  414,    
            418,    422,    426,    430,    434,    438,    442,    447,    451,  456,    
            461,    466,    471,    476,    482,    487,    493,    498,    504,  511,    
            517,    524,    531,    538,    545,    553,    560,    569,    577,  586,    
            595,    605,    615,    626,    637,    649,    661,    675,    688,  703,    
            719,    736,    754,    775,    796,    819,    845,    874,    907,  945,
            988 
        };
    }
}
