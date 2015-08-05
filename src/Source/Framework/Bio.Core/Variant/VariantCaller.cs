using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Bio;
using Bio.Extensions;
using Bio.Algorithms.Alignment;

namespace Bio.Variant
{
    /// <summary>
    /// This class takes alignments
    /// and generates a list of variants.
    /// </summary>
    public static class VariantCaller 
    {

        /// <summary>
        /// Given two byte arrays representing a pairwise alignment, shift them so 
        /// that all deletions start as early as possible.  For example:
        /// 
        /// <code>
        /// TTTTAAAATTTT  -> Converts to ->  TTTTAAAATTTT
        /// TTTTAA--TTTT                     TTTT--AATTTT
        /// </code>
        /// 
        /// This function takes a IPairwiseSequenceAlignment and assumes that the first sequence is the reference and second
        /// sequence is the query.  It returns a new Pairwise sequence alignment with all of the indels left aligned as well as a list of variants.
        /// </summary>
        /// <param name="aln">Aln. The second sequence should be of type QualitativeSequence or Sequence</param>
        /// <param name="callVariants">callVariants.  If true, it will call variants, otherwise the second half of tuple will be null. </param>
        public static Tuple<IPairwiseSequenceAlignment, List<Variant>> LeftAlignIndelsAndCallVariants(IPairwiseSequenceAlignment aln, bool callVariants = true) {

            if (aln == null) {
                throw new NullReferenceException ("aln");
            }
            if (aln.PairwiseAlignedSequences == null || aln.PairwiseAlignedSequences.Count != 1) {
                throw new ArgumentException ("The pairwise aligned sequence should only have one alignment");
            }
            var frstAln = aln.PairwiseAlignedSequences.First ();
            var seq1 = frstAln.FirstSequence;
            var seq2 = frstAln.SecondSequence;
            if (seq1 == null) {
                throw new NullReferenceException ("seq1");
            } else if (seq2 == null) {
                throw new NullReferenceException ("seq2");
            }

            //TODO: Might implement an ambiguity check later.
            #if FALSE
            if (seq1.Alphabet.HasAmbiguity || seq2.Alphabet.HasAmbiguity) {
                throw new ArgumentException ("Cannot left align sequences with ambiguous symbols.");
            }
            #endif

            // Note we have to copy unless we can guarantee the array will not be mutated.
            byte[] refseq = seq1.ToArray ();
            ISequence newQuery;
            List<Variant> variants = null;
            // Call variants for a qualitative sequence
            if (seq2 is QualitativeSequence) {
                var qs = seq2 as QualitativeSequence;
                var query = Enumerable.Zip (qs, qs.GetQualityScores (), (bp, qv) => new BPandQV (bp, (byte)qv, false)).ToArray ();
                AlignmentUtils.LeftAlignIndels (refseq, query);
                AlignmentUtils.VerifyNoGapsOnEnds (refseq, query);
                if (callVariants) {
                    variants = VariantCaller.CallVariants (refseq, query, seq2.IsMarkedAsReverseComplement());
                }
                var newQueryQS = new QualitativeSequence (qs.Alphabet, 
                    qs.FormatType,
                    query.Select (z => z.BP).ToArray (),
                    query.Select (p => p.QV).ToArray (),
                    false);
                newQueryQS.Metadata = seq2.Metadata;
                newQuery = newQueryQS;
                
            } else if (seq2 is Sequence) {  // For a sequence with no QV values.
                var qs = seq2 as Sequence;
                var query = qs.Select (v => new BPandQV (v, 0, false)).ToArray();
                AlignmentUtils.LeftAlignIndels (refseq, query);
                AlignmentUtils.VerifyNoGapsOnEnds (refseq, query);
                // ISequence does not have a setable metadata
                var newQueryS = new Sequence(qs.Alphabet, query.Select(z=>z.BP).ToArray(), false);
                newQueryS.Metadata = seq2.Metadata;
                if (callVariants) {
                    variants = VariantCaller.CallVariants (refseq, query, seq2.IsMarkedAsReverseComplement());
                }
                newQuery = newQueryS;
            } else {
                throw new ArgumentException ("Can only left align indels if the query sequence is of type Sequence or QualitativeSequence.");
            }

            if (aln.FirstSequence != null && aln.FirstSequence.ID != null) {
                foreach (var v in variants) {
                    v.RefName = aln.FirstSequence.ID;
                }
            }

            var newRef = new Sequence (seq1.Alphabet, refseq, false);
            newRef.ID = seq1.ID;
            newRef.Metadata = seq1.Metadata;

            newQuery.ID = seq2.ID;

            var newaln = new PairwiseSequenceAlignment (aln.FirstSequence, aln.SecondSequence);
            var pas = new PairwiseAlignedSequence ();
            pas.FirstSequence = newRef;
            pas.SecondSequence = newQuery;
            newaln.Add (pas);
            return new Tuple<IPairwiseSequenceAlignment, List<Variant>> (newaln, variants);
        }

        /// <summary>
        /// Given a pairwise sequence alignment, call variants, producing
        /// a list of SNPs and Indels found in the alignment.
        /// 
        /// This method will first left-align all variants before calling to be consistent with other
        /// software.  The 
        /// </summary>
        /// <param name="aln">The Pairwise alignment to call variants with.</param>
        /// <returns></returns>
        /// 

        public static List<Variant> CallVariants(IPairwiseSequenceAlignment aln) {
            return LeftAlignIndelsAndCallVariants (aln).Item2;
        }

        /// <summary>
        /// Calls the variants.
        /// 
        /// Should only be used internally as assumptions are made that the alignments are left-aligned and fulfill certain criteria.
        /// </summary>
        /// <returns>The variants.</returns>
        /// <param name="refSeq">Reference seq.</param>
        /// <param name="querySeq">Query seq.</param>
        /// <param name="originallyReverseComplemented">If set to <c>true</c> the query sequence was originally reverse complemented. (this affects QV value scoring)</param>
        internal static List<Variant> CallVariants(byte[] refSeq, BPandQV[] querySeq, bool originallyReverseComplemented)
        {
            if (originallyReverseComplemented) {
                AlignmentUtils.ReverseQVValuesForHomopolymers (querySeq);
            }
            List<Variant> variants = new List<Variant>();

            // Now call variants.
            var gap = DnaAlphabet.Instance.Gap;
            int i = 0;
            int refPos = 0;
            int queryPos = 0;
            while( i < refSeq.Length)
            {
                if (refSeq[i] == gap)
                {
                    int len = AlignmentUtils.GetGapLength(i, refSeq);
                    var nextBasePos = (i + len);
                    // Should alway be true as we don't end in gaps
                    Debug.Assert (nextBasePos < refSeq.Length);
                    var hplenAndChar = determineHomoPolymerLength (nextBasePos, refSeq);
                    var bases = getBases(querySeq, i, len);
                    var newVariant = new IndelVariant(refPos - 1, len, bases, IndelType.Insertion,  
                                                      hplenAndChar.Item2, hplenAndChar.Item1, 
                                                      (i == 0 || (i + len + hplenAndChar.Item1) >= refSeq.Length));                   
                    newVariant.QV = querySeq[queryPos].QV;
                    variants.Add(newVariant);
                    i += len;
                    queryPos += len;
                }
                else if (querySeq[i].BP == gap)
                {
                    int len = AlignmentUtils.GetGapLength(i, querySeq);
                    var bases = getBases(refSeq, i, len);
                    var hplenAndChar = determineHomoPolymerLength (i, refSeq);
                    var newVariant = new IndelVariant(refPos - 1, len, bases, 
                                                      IndelType.Deletion, hplenAndChar.Item2, 
                                                      hplenAndChar.Item1, (i == 0 || (i + len + hplenAndChar.Item1) >= refSeq.Length));
                    /* An insertion mutation occurs BEFORE pos, so normally we get the next base
                     * or the last one if it's a reverse complemented alignment.  However, this is not true if 
                     * it is a homopolymer because what would have been the previous position is the next position
                     * after left aligning and reversing the position of the QV value.
                     * 
                     * Consider the following
                     * --*-       -*--
                     * A-TA   --> TA-T
                     * AGTA       TACT
                     * 
                     * However, 
                     * --*--         --*--
                     * A-TTA   ----> T-AAT
                     * ATTTA         TAAAT
                     * 
                     */
                    if ((i + len ) < querySeq.Length) {
                        
                        var qc_pos = originallyReverseComplemented ? i - 1 : i + len;
                        if (newVariant.InHomopolymer) {
                            qc_pos = i + len;
                        }
                        newVariant.QV = querySeq[qc_pos].QV;
                    }
                    variants.Add(newVariant);
                    i += len;
                    refPos += len;
                }
                else
                {
                    if (querySeq[i].BP != refSeq[i])
                    {
                        var newVariant = new SNPVariant(refPos, (char) querySeq[i].BP, (char)refSeq[i]);
                        newVariant.QV = querySeq [queryPos].QV;
                        variants.Add(newVariant);
                    }
                    i++; refPos++; queryPos++;
                }
            }
            return variants;
        }

        /// <summary>
        /// Converts a subset of bases in the array into a string.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string getBases(byte[] array, int position, int length)
        {
            char[] chars = new char[length];
            for(int i=0; i<length; i++)
            {
                chars[i] = (char)array[i+position];
            }
            return new string(chars);
        }

        /// <summary>
        /// Gets the bases for a length of the sequence as a string.
        /// </summary>
        /// <returns>The bases.</returns>
        /// <param name="array">Array.</param>
        /// <param name="position">Position.</param>
        /// <param name="length">Length.</param>
        private static string getBases(BPandQV[] array, int position, int length)
        {
            char[] chars = new char[length];
            for(int i=0; i<length; i++)
            {
                chars[i] = (char)array[i+position].BP;
            }
            return new string(chars);
        }


        /// <summary>
        /// Returns the length of the homopolymer 
        /// </summary>
        /// <returns>The homopolymer length.</returns>
        private static Tuple<int, char> determineHomoPolymerLength(int pos, byte[] refSeq)
        {

            byte start_bp = refSeq[pos];
            int length = 1;
            while ( ++pos < refSeq.Length &&
                refSeq [pos] == start_bp) {
                length++;
            }
            var homopolymerBase = (char)start_bp;
            return new Tuple<int,char> (length, homopolymerBase);
        }
    }
}

