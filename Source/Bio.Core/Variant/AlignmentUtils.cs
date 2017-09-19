using System;
using Bio.Algorithms.Alignment;
using System.Collections.Generic;
using System.Linq;
using Bio.Extensions;
using System.Diagnostics;

namespace Bio.Variant
{
    /// <summary>
    /// Utilities to manipulate and work with alignments when calling variants.
    /// </summary>
    public static class AlignmentUtils
    {

        /// <summary>
        /// The maximum number of times we can shift an indel before we fail.
        /// It should never go anywhere near this number and this is only implemented as a guard.
        /// </summary>
        private const int MAX_LOOPS = 50;
        private const string MAX_LOOPS_ERROR = "The Left-align indel step did not converge within 50 iterations";

        /// <summary>
        /// Verifies the alignment has no leading or trailing gaps and throws an exception otherwise.
        /// </summary>
        /// <param name="refseq">Refseq.</param>
        /// <param name="query">Query.</param>
        internal static void VerifyNoGapsOnEnds(byte[] refseq, BPandQV[] query) {
            var gap = DnaAlphabet.Instance.Gap;
            if (refseq [0] == gap ||
                refseq [refseq.Length - 1] == gap ||
                query [0].BP == gap ||
                query [query.Length - 1].BP == gap) {
                var refseqs = new string(refseq.Select(x=>(char)x).ToArray());
                var qseqs = new string(query.Select(x=>(char)x.BP).ToArray());
                throw new FormatException ("Alignment query and/or reference started with a gap character. " +
                    "Alignments must be hard-clipped to remove starting and trailing variants " +
                    "before variants can be called.  Alignment was:\n" + refseqs + "\n" + qseqs);
            }
        }

        /// <summary>
        /// Given two byte arrays representing a pairwise alignment, shift them so 
        /// that all deletions start as early as possible.  For example:
        /// TTTTAAAATTTT   -> Converts to -> TTTTAAAATTTT
        /// TTTTAA--TTTT                     TTTT--AATTTT
        /// 
        /// This modifies the array in place.
        /// </summary>
        /// <param name="refseq">Reference Sequency</param>
        /// <param name="query">Query Sequence</param>
        /// <returns></returns>
        public static void LeftAlignIndels(byte[] refseq, BPandQV[] query)
        {          
            // Validation
            if (refseq.Length != query.Length) {
                throw new ArgumentException("Alignment passed to LeftAlignIndels had unequal length sequences");
            }

            ValidateNoOverlappingGaps (refseq, query);
            byte gap = DnaAlphabet.Instance.Gap;
            // Keep left aligning until we can't anymore, this is a 
            // do while loop because some downstream left alignments open up
            // further ones upstream, even though this is rare.
            int change_count = 0;
            int loopsThrough = 0;
            do
            {
                loopsThrough++;
                change_count = 0;
                for (int i = 1; i < refseq.Length; i++)
                {
                    if (refseq[i] == gap)
                    {
                        int len = GetGapLength(i, refseq);
                        int left_side = i - 1;
                        int right_side = i  - 1 + len;
                        while (left_side >= 0 && refseq[left_side] != gap && (refseq[left_side] == query[right_side].BP))
                        {
                            // Move the gap left.
                            if (right_side < refseq.Length) {
                                refseq[right_side] = refseq[left_side];
                            }
                            refseq[left_side] = gap;
                            left_side--;
                            right_side--;
                            change_count++;
                        }
                        if (loopsThrough > MAX_LOOPS) {
                            throw new Exception(MAX_LOOPS_ERROR);
                        }
                    }
                    else if (query[i].BP == gap)
                    {
                        int len = GetGapLength(i, query);
                        int left_side = i - 1;
                        int right_side = i - 1 + len;
                        while (left_side >= 0 && query[left_side].BP != gap && (query[left_side].BP == refseq[right_side]))
                        {
                            // Move the gap left.
                            if (right_side < query.Length) {
                            query[right_side] = query[left_side];
                            }
                            query[left_side] = new BPandQV(gap, 0);
                            left_side--;
                            right_side--;
                            change_count++;
                        }
                        if (loopsThrough > MAX_LOOPS) {
                            throw new Exception(MAX_LOOPS_ERROR);
                        }
                    }
                }
            } while (change_count > 0);
        }

        /// <summary>
        /// Given the start position of a gap, returns how long it is.
        /// For example:
        /// 
        /// AAAA---TTTT returns 3.
        /// </summary>
        /// <param name="pos">0 indexed</param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int GetGapLength(int pos, BPandQV[] array)
        {
            var gap = DnaAlphabet.Instance.Gap;
            int len = 1;
            while (++pos < array.Length)
            {
                if (array[pos].BP == gap)
                {
                    len += 1;
                }
                else
                {
                    break;
                }
            }
            return len;
        }

        /// <summary>
        /// Given the start position of a gap, returns how long it is.
        /// For example:
        /// 
        /// AAAA---TTTT returns 3.
        /// </summary>
        /// <param name="pos">0 indexed</param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int GetGapLength(int pos, byte[] array)
        {
            var gap = DnaAlphabet.Instance.Gap;
            int len = 1;
            while (++pos < array.Length)
            {
                if (array[pos] == gap)
                {
                    len += 1;
                }
                else
                {
                    break;
                }
            }
            return len;
        }


        /// <summary>
        /// Simple check that the alignment does not have a gap on top of a
        /// gap, which violates several assumptions.
        /// </summary>
        /// <param name="seq1"></param>
        /// <param name="seq2"></param>
        internal static void ValidateNoOverlappingGaps(byte[] seq1, BPandQV[] seq2)
        {
            var gap = DnaAlphabet.Instance.Gap;
            for(int i=0;i<seq1.Length;i++)
            {
                if (seq1[i] == gap && seq2[i].BP == gap)
                    throw new Exception("You have an alignment with overlapping gaps.  Input problem!");
            }
        }


        /// <summary>
        /// Reverse complements the BP and QV values accounting for homopolymers.
        /// 
        /// This is not a simple operation because in addition to reversing the QV scores, one must account for the fact that 
        /// the quality value for homopolymer indel errors (that is a deletion or insertion) is typically only placed at the 
        /// first base in a homopolymer (though this is not standardized).  To account for this, when reverse complementing, we switch the 
        /// QV value of the first and last base in homopolymers if the first base is lower quality than the last base.
        /// </summary>
        /// <returns>The reverse complemented sequence.</returns>
        /// <param name="toFlip">The array with the QV values to flip.</param>
        /// <param name="flipHpQvValues">If set to <c>true</c> flip hp qv values.</param>
        internal static BPandQV[] GetReverseComplementedSequence(BPandQV[] toFlip, bool flipHpQvValues = false)
        {
            BPandQV[] newData = new BPandQV[toFlip.Length];

            for (long index = 0; index < toFlip.Length; index++)
            {
                byte complementedSymbol;
                byte symbol = toFlip[toFlip.Length - index - 1].BP;

                if (!DnaAlphabet.Instance.TryGetComplementSymbol(symbol, out complementedSymbol))
                {
                    throw new NotSupportedException("Bad character in BPandQV array: " + symbol.ToString());
                }
                var bpandq = new BPandQV(complementedSymbol, toFlip[toFlip.Length - index -1].QV);
                newData [index] = bpandq;
            }

            if (flipHpQvValues) {
                ReverseQVValuesForHomopolymers (newData);
            }
            return newData;
        }

        /// <summary>
        /// Reverses the QV values for homopolymers.
        /// This is not a simple operation because in addition to reversing the QV scores, one must account for the fact that 
        /// the quality value for homopolymer indel errors (that is a deletion or insertion) is typically only placed at the 
        /// first base in a homopolymer (though this is not standardized).  To account for this, when reverse complementing, we switch the 
        /// QV value of the first and last base in homopolymers if the first base is lower quality than the last base.
        /// </summary>
        /// <returns>The QV values for homopolymers.</returns>
        /// <param name="toFlip">To flip.</param>
        internal static void ReverseQVValuesForHomopolymers(BPandQV[] toFlip) {
            // Basic idea is to assume it is A, C, G, T alphabet and flip HP values
            // Also assumes all low QV is due to HP deletion/insertion error.
            if (toFlip.Length > 1) {
                byte lastbp = toFlip [0].BP;
                int firstPos = 0;
                int curLength = 1;
                for (int i = 1; i < toFlip.Length; i++) {
                    byte newbp = toFlip [i].BP;
                    if (newbp != lastbp) {
                        if (curLength > 1) {
                            var right = toFlip [i - 1];
                            var left = toFlip[firstPos];
                            Debug.Assert (right.BP == left.BP);
                            if (right.QV < left.QV) {
                                toFlip [i - 1] = left;
                                toFlip [firstPos] = right;
                            }
                        }
                        firstPos = i;
                        lastbp = newbp;
                        curLength = 1;
                    } else if (newbp == lastbp) {
                        curLength++;
                    }
                }
                // Finally flip the end
                if (curLength > 1) {
                    var tmp = toFlip [toFlip.Length - 1];
                    toFlip [toFlip.Length - 1] = toFlip [firstPos];
                    toFlip [firstPos] = tmp;
                }
            }          
        }
    }
}