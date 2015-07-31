using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio;
using Bio.IO.SAM;
using System.Diagnostics;

namespace Bio.Variant
{
    /// <summary>
    /// A class the produces read pile ups (columns of multiple sequence alignments) from a stream of 
    /// SAM aligned quality sequences.
    /// </summary>
    public class PileUpProducer
    {        
        /// <summary>
        /// Takes a sorted list of SAMAligned sequences and converts them in to read pile ups.
        /// </summary>
        /// <param name="sequences"></param>
        /// <returns></returns>
        public IEnumerable<PileUp> CreatePileupFromReads(IEnumerable<SAMAlignedSequence> sequences)
        {
            LinkedList<PileUp> pileupsToEmit = null;
            string cur_ref, last_ref = null;

            // Variable for catching errors, can be removed later after once no problems show up.
            int lastEmittedPosition = -1;
            foreach (var seq in sequences)
            {
                validateSequence(seq);
                cur_ref = seq.RName;
                
                // initalize if first sequence, a new reference or outside the previous interval
                if (pileupsToEmit == null || last_ref != cur_ref)
                {
                    if (pileupsToEmit != null)
                    {
                        foreach (var pu in pileupsToEmit)
                        {
                            Debug.Assert(pu.Position >= lastEmittedPosition, "Emitting pile-up out of order");
                            lastEmittedPosition = pu.Position;
                            yield return pu;
                        }
                    }
                    last_ref = cur_ref;
                    lastEmittedPosition = -1;
                    var newPileups = generateNewPileupsForInterval(cur_ref, seq.Pos, seq.RefEndPos);
                    pileupsToEmit = new LinkedList<PileUp>(newPileups);
                }

                // Get read data to add to the pile-up.
                var bases = getBasesForSequence(seq);

                // Advance the pile-ups until we start to overlap.
                while (pileupsToEmit.Count > 0 && pileupsToEmit.First.Value.Position < bases[0].Position)
                {
                    var pu = pileupsToEmit.First.Value;
                    Debug.Assert(pu.Position >= lastEmittedPosition, "Emitting pile-up out of order");
                    lastEmittedPosition = pu.Position;
                    yield return pu;
                    pileupsToEmit.RemoveFirst();
                }
                var cur_pileup = pileupsToEmit.First;
                // Now add this reads bases to the pile-up
                foreach (var bp in bases) 
                {   
                    if (cur_pileup == null) {
                        // If null, then this base and all following bases are new additions.
                        pileupsToEmit.AddFirst(new PileUp(cur_ref, bp));                         
                    } 
                    else if ( bp.Position < cur_pileup.Value.Position)
                    {
                        //If the read is before the last position, this should be added in front
                        pileupsToEmit.AddBefore(cur_pileup, new PileUp(cur_ref, bp));
                    }
                    else
                    {
                        //the current base is >= the current position
                        while (cur_pileup != null )
                        {

                        }
                        foreach(var v in pileupsToEmit) {
                            var cur_pileup = pileupsToEmit.First;
                            // If at the same position we should be fine
                            if (cur_pileup.Value.Position == bp.Position)
                            {
                                if (
                                cur_pileup.Bases.Add(bp.BaseWithQuality);
                            }                    
                            else
                            {

                            }
                            }
                        

                }
              




            }
            

        }
        /// <summary>
        /// Method throws an exception if sequence violates any assumption made by this class anywhere.
        /// Avoids, separate checks within each method.
        /// </summary>
        /// <param name="seq"></param>
        private void validateSequence(SAMAlignedSequence seq)
        {
            if (seq == null) {
                throw new ArgumentNullException("seq");
            }
            if (String.IsNullOrEmpty(seq.RName) || 
                seq.RefEndPos <= seq.Pos || 
                String.IsNullOrEmpty(seq.CIGAR) || 
                seq.CIGAR =="*" ||
                !(seq.QuerySequence is QualitativeSequence) )
            {
                throw new ArgumentException("Tried to build a pileup with an invalid sequence.  Sequence was:\n"+
                    seq.ToString());
            }
        }
        /// <summary>
        /// Create new pileup instances from start to end (inclusive).
        /// </summary>
        IEnumerable<PileUp> generateNewPileupsForInterval(string refName, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                yield return new PileUp(refName, i);
            }
        }

        List<BaseAndQualityAndPosition> getBasesForSequence(SAMAlignedSequence seq)
        {
            List<BaseAndQualityAndPosition> toReturn = new List<BaseAndQualityAndPosition>(seq.RefEndPos - seq.Pos + 10);
            // Decode the cigar string into operations.
            // TODO: This code is duplicated in many places
            string CIGAR = seq.CIGAR;
            List<KeyValuePair<char, int>> charsAndPositions = new List<KeyValuePair<char, int>>();
            for (int i = 0; i < CIGAR.Length; i++)
            {
                char ch = CIGAR[i];
                if (Char.IsDigit(ch))
                {
                    continue;
                }
                charsAndPositions.Add(new KeyValuePair<char, int>(ch, i));
            }

            // Get sequence bases and error probabilities
            var qseq = seq.QuerySequence as QualitativeSequence;
            var seq_log10ErrorProb = qseq.GetPhredQualityScores().Select(Utils.GetLog10ErrorProbability).ToArray();
            var seq_bases = qseq.ToArray();
            // Use the cigar operations to emit bases.
            int curRef = seq.Pos;
            int curQuery = 0;
            for (int i = 0; i < charsAndPositions.Count; i++)
            {
                // Parse the current cigar operation
                char ch = charsAndPositions[i].Key;
                int cig_start = i==0 ? 0 : charsAndPositions[i - 1].Value + 1;
                int cig_end = charsAndPositions[i].Value - cig_start;
                int cig_len = int.Parse(CIGAR.Substring(cig_start, cig_end));
                // Emit or advance based on cigar operation.
                switch (ch)
                {
                    case 'P': //padding (Silent deltions from padded reference)
                    case 'N': //skipped region from reference
                        throw new Exception("Pile up methods not built to handle reference clipping (Cigar P or N) yet.");
                    case 'M': //match or mismatch
                    case '=': //match
                    case 'X': //mismatch
                        for (int k = 0; k < cig_len; k++)
                        {                            
                            var bqp= new BaseAndQualityAndPosition(curRef,0, new BaseAndQuality(seq_bases[curQuery], seq_log10ErrorProb[curQuery]));
                            toReturn.Add(bqp);
                            curQuery++;
                            curRef++;
                        }
                        break;
                    case 'I'://insertion to the reference
                        for (int k = 0; k < cig_len; k++)
                        {                            
                            var bqp =  new BaseAndQualityAndPosition(curRef,k, new BaseAndQuality(seq_bases[curQuery], seq_log10ErrorProb[curQuery]));
                            toReturn.Add(bqp);
                            curQuery++;
                        }
                        break;
                    case 'D'://Deletion from the reference
                        for (int k = 0; k < cig_len; k++)
                        {                            
                            var bqp = new BaseAndQualityAndPosition(curRef,k, new BaseAndQuality((byte)'-', Double.NaN));
                            toReturn.Add(bqp);
                            curRef++;
                        }
                        break;
                    case 'S': //soft clipped
                        curQuery += cig_len;
                        break;
                    case 'H'://had clipped
                        break;
                    default:
                        throw new FormatException("Unexpected SAM Cigar element found " + ch.ToString());
                }                
            }
            return toReturn;
        }
        
    }
}
