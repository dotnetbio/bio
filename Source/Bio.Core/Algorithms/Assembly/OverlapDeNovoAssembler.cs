using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Util.Logging;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Implements a simple greedy assembly algorithm for DNA.
    /// </summary>
    public class OverlapDeNovoAssembler : IOverlapDeNovoAssembler
    {
        #region Fields
        /// <summary>
        /// The alphabet type of sequences to be assembled
        /// </summary>
        private IAlphabet _sequenceAlphabet;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the OverlapDeNovoAssembler class.
        /// Sets default threshold values, pairwise aligner, consensusResolver.
        /// Users will typically reset these using parameters 
        /// specific to their particular sequences and needs.
        /// </summary>
        public OverlapDeNovoAssembler()
        {
            // The following definitions give the default values 
            // for different parameters used for initialization
            // Note: Each of the following constructor calls, 
            // in turn might set default values for its required parameters.

            // By default, set merge threshold to 3
            MergeThreshold = 3;

            // By default, use PairwiseOverlapAligner
            OverlapAlgorithm = new PairwiseOverlapAligner();

            // By default, value of AssumeStandardOrientation is set as true
            AssumeStandardOrientation = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Threshold determines how much overlap is needed 
        /// for two sequences to be merged. The score from the overlap algorithm 
        /// must at least equal Threshold for a merge to occur.
        /// </summary>
        public double MergeThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether standard orientation is assumed.
        /// if true, assume that the input sequences are in 5'-to-3' orientation.
        /// This means that only normal and reverse-complement overlaps need to be tested.
        /// if false, need to try both orientations for overlaps.
        /// </summary>
        public bool AssumeStandardOrientation { get; set; }

        /// <summary>
        /// Gets or sets the pairwise sequence aligner that will be used to compute overlap during assembly.
        /// </summary>
        public ISequenceAligner OverlapAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the method that will be used to compute a contig's consensus during assembly.
        /// </summary>
        public IConsensusResolver ConsensusResolver { get; set; }

        /// <summary>
        /// Gets the name of the current assembly algorithm used.
        /// This property returns the Name of our assembly algorithm i.e 
        /// Simple-sequence algorithm.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.SIMPLE_NAME;
            }
        }

        /// <summary>
        /// Gets the description of the current assembly algorithm used.
        /// This property returns a simple description of what 
        ///  SimpleSequenceAssembler class implements.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.SIMPLE_DESCRIPTION;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Assemble the input sequences into the largest possible contigs. 
        /// </summary>
        /// <remarks>
        /// The algorithm is:
        /// 1.  initialize list of contigs to empty list. List of seqs is passed as argument.
        /// 2.  compute pairwise overlap scores for each pair of input seqs (with reversal and
        ///     complementation as appropriate).
        /// 3.  choose best overlap score. the “merge items” (can be seqs or contigs) are the 
        ///     items with that score. If best score is less than threshold, assembly is finished.
        /// 4.  merge the merge items into a single contig and remove them from their list(s)
        /// 5.  compute the overlap between new item and all existing items
        /// 6.  go to step 3
        /// </remarks>
        /// <param name="inputSequences">The sequences to assemble.</param>
        /// <returns>Returns the OverlapDeNovoAssembly instance which contains list of 
        /// contigs and list of unmerged sequences which are result of this assembly.</returns>
        public IDeNovoAssembly Assemble(IEnumerable<ISequence> inputSequences)
        {
            if (null == inputSequences)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameInputSequences);
            }

            // numbering convention: every pool item (whether sequence or contig)
            // gets a fixed number.
            // sequence index = index into inputs (which we won't modify)
            // contig index = nSequences + index into contigs
            List<PoolItem> pool = inputSequences.Select(seq => new PoolItem(seq)).ToList();

            // Initialization
            int sequenceCount = pool.Count;
            if (sequenceCount > 0)
            {
                _sequenceAlphabet = pool[0].Sequence.Alphabet;

                if (ConsensusResolver == null)
                {
                    ConsensusResolver = new SimpleConsensusResolver(_sequenceAlphabet);
                }
                else
                {
                    ConsensusResolver.SequenceAlphabet = _sequenceAlphabet;
                }
            }

            // put all the initial sequences into the pool, and generate the pair scores.
            // there are no contigs in the pool yet.
            // to save an iteration, we'll also find the best global score as we go.
            ItemScore globalBest = new ItemScore(-1, -1, false, false, 0, 0);
            int globalBestLargerIndex = -1;
            int unconsumedCount = sequenceCount;

            // Compute alignment scores for all combinations between input sequences
            // Store these scores in the poolItem corresponding to each sequence
            for (int newSeq = 0; newSeq < pool.Count; ++newSeq)
            {
                PoolItem newItem = pool[newSeq];
                for (int oldSeq = 0; oldSeq < newSeq; ++oldSeq)
                {
                    PoolItem oldItem = pool[oldSeq];
                    ItemScore score = AlignSequence(oldItem.SequenceOrConsensus, newItem.SequenceOrConsensus, oldSeq, newSeq);
                    newItem.Scores.Add(score);
                    if (score.OverlapScore > globalBest.OverlapScore)
                    {
                        globalBest = new ItemScore(score);
                        globalBestLargerIndex = newSeq;
                    }
                }
            }

            // Merge sequence if best score is above threshold 
            // and add new contig to pool
            if (globalBest.OverlapScore >= MergeThreshold)
            {
                if (Trace.Want(Trace.AssemblyDetails))
                {
                    ApplicationLog.WriteLine("Merging (overlap score {0}):", globalBest.OverlapScore);
                }

                PoolItem mergeItem1 = pool[globalBest.OtherItem];
                PoolItem mergeItem2 = pool[globalBestLargerIndex];
                Contig newContig = new Contig();
                if (Trace.Want(Trace.AssemblyDetails))
                {
                    ApplicationLog.WriteLine(
                        "new pool item {0} will merge old items {1} and {2}",
                        pool.Count,
                        globalBest.OtherItem,
                        globalBestLargerIndex);
                }

                MergeLowerIndexedSequence(newContig, globalBest, mergeItem1.Sequence);
                MergeHigherIndexedSequence(newContig, globalBest, mergeItem2.Sequence);
                MakeConsensus(newContig);

                // Set ConsumedBy value and 
                // free memory as these sequences are no longer used
                mergeItem1.ConsumedBy = pool.Count;
                mergeItem2.ConsumedBy = pool.Count;
                mergeItem1.FreeSequences();
                mergeItem2.FreeSequences();
                pool.Add(new PoolItem(newContig));
                unconsumedCount--;

                while (unconsumedCount > 1)
                {
                    // Compute scores for each unconsumed sequence with new contig
                    int newSeq = pool.Count - 1;
                    PoolItem newItem = pool[newSeq];
                    for (int oldSeq = 0; oldSeq < pool.Count - 1; ++oldSeq)
                    {
                        PoolItem oldItem = pool[oldSeq];
                        if (oldItem.ConsumedBy >= 0)
                        {
                            // already consumed - just add dummy score to maintain correct indices
                            newItem.Scores.Add(new ItemScore());
                        }
                        else
                        {
                            ItemScore score = AlignSequence(oldItem.SequenceOrConsensus, newItem.SequenceOrConsensus, oldSeq, newSeq);
                            newItem.Scores.Add(score);
                        }
                    }

                    // find best global score in the modified pool.
                    globalBest = new ItemScore(-1, -1, false, false, 0, 0);
                    globalBestLargerIndex = -1;
                    for (int current = 0; current < pool.Count; ++current)
                    {
                        PoolItem curItem = pool[current];
                        if (curItem.ConsumedBy < 0)
                        {
                            for (int other = 0; other < current; ++other)
                            {
                                if (pool[other].ConsumedBy < 0)
                                {
                                    ItemScore itemScore = curItem.Scores[other];
                                    if (itemScore.OverlapScore > globalBest.OverlapScore)
                                    {
                                        globalBest = new ItemScore(itemScore);  // copy the winner so far
                                        globalBestLargerIndex = current;
                                    }
                                }
                            }
                        }
                    }

                    if (globalBest.OverlapScore >= MergeThreshold)
                    {
                        // Merge sequences / contigs if above threshold
                        mergeItem1 = pool[globalBest.OtherItem];
                        mergeItem2 = pool[globalBestLargerIndex];
                        newContig = new Contig();

                        if (mergeItem1.IsContig)
                        {
                            if (Trace.Want(Trace.AssemblyDetails))
                            {
                                ApplicationLog.WriteLine(
                                    "item {0} is a contig (reversed = {1}, complemented = {2}, offset = {3}",
                                    globalBest.OtherItem,
                                    globalBest.Reversed,
                                    globalBest.Complemented,
                                    globalBest.FirstOffset);
                            }

                            MergeLowerIndexedContig(newContig, globalBest, mergeItem1.Contig);
                        }
                        else
                        {
                            if (Trace.Want(Trace.AssemblyDetails))
                            {
                                ApplicationLog.WriteLine(
                                    "item {0} is a sequence (reversed = {1}, complemented = {2}, offset = {3}",
                                    globalBest.OtherItem,
                                    globalBest.Reversed,
                                    globalBest.Complemented,
                                    globalBest.FirstOffset);
                            }

                            MergeLowerIndexedSequence(newContig, globalBest, mergeItem1.Sequence);
                        }

                        if (mergeItem2.IsContig)
                        {
                            if (Trace.Want(Trace.AssemblyDetails))
                            {
                                ApplicationLog.WriteLine(
                                    "item {0} is a contig (offset = {1}",
                                    globalBestLargerIndex,
                                    globalBest.SecondOffset);
                            }

                            MergeHigherIndexedContig(newContig, globalBest, mergeItem2.Contig);
                        }
                        else
                        {
                            if (Trace.Want(Trace.AssemblyDetails))
                            {
                                ApplicationLog.WriteLine(
                                    "item {0} is a sequence (offset = {1}",
                                    globalBestLargerIndex,
                                    globalBest.SecondOffset);
                            }

                            MergeHigherIndexedSequence(newContig, globalBest, mergeItem2.Sequence);
                        }

                        MakeConsensus(newContig);
                        if (Trace.Want(Trace.AssemblyDetails))
                        {
                            Dump(newContig);
                        }

                        // Set ConsumedBy value for these poolItems and 
                        // free memory as these sequences are no longer used
                        mergeItem1.ConsumedBy = pool.Count;
                        mergeItem2.ConsumedBy = pool.Count;
                        mergeItem1.FreeSequences();
                        mergeItem2.FreeSequences();

                        pool.Add(new PoolItem(newContig));
                        unconsumedCount--;
                    }
                    else
                    {
                        // None of the alignment scores cross threshold
                        // No more merges possible. So end iteration.
                        break;
                    }
                }
            }

            // no further qualifying merges, so we're done.
            // populate contigs and unmergedSequences
            OverlapDeNovoAssembly sequenceAssembly = new OverlapDeNovoAssembly();
            foreach (PoolItem curItem in pool)
            {
                if (curItem.ConsumedBy < 0)
                {
                    if (curItem.IsContig)
                    {
                        sequenceAssembly.Contigs.Add(curItem.Contig);
                    }
                    else
                    {
                        sequenceAssembly.UnmergedSequences.Add(curItem.Sequence);
                    }
                }
            }

            return sequenceAssembly;
        }

        /// <summary>
        /// Analyze the passed contig and store a consensus into its Consensus property.
        /// Public method to allow testing of consensus generation part.
        /// Used by test automation.
        /// </summary>
        /// <param name="alphabet">Sequence alphabet</param>
        /// <param name="contig">Contig for which consensus is to be constructed</param>
        public void MakeConsensus(IAlphabet alphabet, Contig contig)
        {
            _sequenceAlphabet = alphabet;
            if (ConsensusResolver == null)
            {
                ConsensusResolver = new SimpleConsensusResolver(_sequenceAlphabet);
            }
            else
            {
                ConsensusResolver.SequenceAlphabet = _sequenceAlphabet;
            }

            MakeConsensus(contig);
        }

        /// <summary>
        /// Removes gaps that are inserted by overlap algorithm at beginning or end of sequence.
        /// </summary>
        /// <param name="inputSequence">input sequence</param>
        /// <returns>Sequence without gaps</returns>
        private static ISequence SequenceWithoutTerminalGaps(ISequence inputSequence)
        {
            //string input = inputSequence.ToString();
            long start = 0;
            while (inputSequence[start] == '-')
            {
                ++start;
            }

            long len = inputSequence.Count - start;
            while (inputSequence[len - 1] == '-')
            {
                --len;
            }

            ISequence seq = inputSequence.GetSubSequence(start, len);
            seq.ID = inputSequence.ID;
            return seq;
        }

        /// <summary>
        /// Method to merge lower-indexed item with new constructed contig.
        /// Merges consumed contig with new contig. For each sequence in consumed contig, 
        /// compute sequence and offset to be added to new contig.
        /// </summary>
        /// <param name="newContig">New contig for merging</param>
        /// <param name="globalBest">Best Score along with offsets information</param>
        /// <param name="consumedContig">Contig to be merged</param>
        private static void MergeLowerIndexedContig(Contig newContig, ItemScore globalBest, Contig consumedContig)
        {
            foreach (Contig.AssembledSequence aseq in consumedContig.Sequences)
            {
                Contig.AssembledSequence newASeq = new Contig.AssembledSequence();

                // lower-indexed item might be reversed or complemented. 
                // Construct new sequence based on setting in globalBest
                // reverse of reverse, or comp of comp, equals no-op. So use xor
                newASeq.IsReversed = aseq.IsReversed ^ globalBest.Reversed;
                newASeq.IsComplemented = aseq.IsComplemented ^ globalBest.Complemented;

                // position in the new contig is adjusted by alignment of the merged items.
                // this depends on whether the contig is reverse-aligned.
                if (globalBest.Reversed)
                {
                    long rightOffset = consumedContig.Length - (aseq.Sequence.Count + aseq.Position);
                    newASeq.Position = globalBest.FirstOffset + rightOffset;
                }
                else
                {
                    newASeq.Position = globalBest.FirstOffset + aseq.Position;
                }

                newASeq.Sequence = SequenceWithoutTerminalGaps(aseq.Sequence);
                newContig.Sequences.Add(newASeq);
                if (Trace.Want(Trace.AssemblyDetails))
                {
                    ApplicationLog.WriteLine(
                        "\tseq (rev = {0} comp = {1} pos = {2}) {3}",
                        newASeq.IsReversed,
                        newASeq.IsComplemented,
                        newASeq.Position,
                        newASeq.Sequence);
                }
            }
        }

        /// <summary>
        /// Method to merge lower-indexed item with new constructed contig
        /// Merges consumed sequence with new contig. For the consumed sequence,
        /// compute new sequence and offset to be added to new contig.
        /// </summary>
        /// <param name="newContig">New contig for merging</param>
        /// <param name="globalBest">Best Score, consensus, their offsets</param>
        /// <param name="consumedSequence">Consumed Sequence to be merged</param>
        private static void MergeLowerIndexedSequence(Contig newContig, ItemScore globalBest, ISequence consumedSequence)
        {
            Contig.AssembledSequence newASeq = new Contig.AssembledSequence();

            // lower-indexed item might be reversed or complemented. 
            // Retreive information from globalBest
            newASeq.IsReversed = globalBest.Reversed;
            newASeq.IsComplemented = globalBest.Complemented;
            newASeq.Position = globalBest.FirstOffset;
            newASeq.Sequence = SequenceWithoutTerminalGaps(consumedSequence);

            newContig.Sequences.Add(newASeq);
            if (Trace.Want(Trace.AssemblyDetails))
            {
                ApplicationLog.WriteLine(
                    "seq (rev = {0} comp = {1} pos = {2}) {3}",
                    newASeq.IsReversed,
                    newASeq.IsComplemented,
                    newASeq.Position,
                    newASeq.Sequence);
            }
        }

        /// <summary>
        /// Method to merge higher-indexed item with new constructed contig.
        /// Merges consumed contig with new contig. For each sequence in consumed contig, 
        /// compute sequence and offset to be added to new contig.
        /// </summary>
        /// <param name="newContig">New contig for merging</param>
        /// <param name="globalBest">Best Score, consensus, their offsets</param>
        /// <param name="consumedContig">Consumed Contig to be merged</param>
        private static void MergeHigherIndexedContig(Contig newContig, ItemScore globalBest, Contig consumedContig)
        {
            foreach (Contig.AssembledSequence aseq in consumedContig.Sequences)
            {
                Contig.AssembledSequence newASeq = new Contig.AssembledSequence();

                // as the higher-index item, this contig is never reversed or complemented, so:
                newASeq.IsReversed = aseq.IsReversed;
                newASeq.IsComplemented = aseq.IsComplemented;

                // position in the new contig adjusted by alignment of the merged items.
                newASeq.Position = globalBest.SecondOffset + aseq.Position;
                newASeq.Sequence = SequenceWithoutTerminalGaps(aseq.Sequence);

                newContig.Sequences.Add(newASeq);
                if (Trace.Want(Trace.AssemblyDetails))
                {
                    ApplicationLog.WriteLine(
                        "\tseq (rev = {0} comp = {1} pos = {2}) {3}",
                        newASeq.IsReversed,
                        newASeq.IsComplemented,
                        newASeq.Position,
                        newASeq.Sequence);
                }
            }
        }

        /// <summary>
        /// Method to merge higher-indexed item with new constructed contig.
        /// Merges consumed sequence with new contig. For the consumed sequence,
        /// compute new sequence and offset to be added to new contig.
        /// </summary>
        /// <param name="newContig">New contig for merging</param>
        /// <param name="globalBest">Best Score, consensus, their offsets</param>
        /// <param name="consumedSequence">Consumed Sequence to be merged</param>
        private static void MergeHigherIndexedSequence(Contig newContig, ItemScore globalBest, ISequence consumedSequence)
        {
            Contig.AssembledSequence newASeq = new Contig.AssembledSequence();

            // as the higher-index item, this sequence is never reversed or complemented, so:
            newASeq.IsReversed = false;
            newASeq.IsComplemented = false;
            newASeq.Position = globalBest.SecondOffset;
            newASeq.Sequence = SequenceWithoutTerminalGaps(consumedSequence);

            newContig.Sequences.Add(newASeq);
            if (Trace.Want(Trace.AssemblyDetails))
            {
                ApplicationLog.WriteLine(
                    "seq (rev = {0} comp = {1} pos = {2}) {3}",
                    newASeq.IsReversed,
                    newASeq.IsComplemented,
                    newASeq.Position,
                    newASeq.Sequence);
            }
        }

        /// <summary>
        /// Write sequence alignment to application log
        /// </summary>
        /// <param name="alignment">sequence alignment</param>
        private static void Dump(IList<ISequenceAlignment> alignment)
        {
            if (alignment.Count > 0)
            {
                ApplicationLog.WriteLine("score: {0}", alignment[0].AlignedSequences[0].Metadata["Score"]);
                ApplicationLog.WriteLine("first: {0}", alignment[0].AlignedSequences[0].Sequences[0]);
                ApplicationLog.WriteLine("second: {0}", alignment[0].AlignedSequences[0].Sequences[1]);
            }
        }

        /// <summary>
        /// Write contig to application log
        /// </summary>
        /// <param name="contig">contig to be dumped</param>
        private static void Dump(Contig contig)
        {
            ApplicationLog.WriteLine("contig has {0} seqs, length {1}", contig.Sequences.Count, contig.Length);
            ApplicationLog.WriteLine("consensus: {0}", contig.Consensus);
            foreach (Contig.AssembledSequence aseq in contig.Sequences)
            {
                ApplicationLog.WriteLine(
                    "seq (rev = {0} comp = {1} pos = {2}) {3}",
                    aseq.IsReversed,
                    aseq.IsComplemented,
                    aseq.Position,
                    aseq.Sequence);
            }

            ApplicationLog.WriteLine(string.Empty);
        }

        /// <summary>
        /// Aligns the two input sequences, their reverseComplement, complement and reverse
        /// Keeps track of best score for these combinations.
        /// </summary>
        /// <param name="lowerIndexedSequence">Lower-indexed sequence to be aligned</param>
        /// <param name="higherIndexedSequence">Higher-indexed sequence to be aligned</param>
        /// <param name="lowerIndex">Index of first sequence in pool</param>
        /// <param name="higherIndex">Index of second sequence in pool</param>
        /// <returns>ItemScore containing score, consensus, offset of best alignment</returns>
        private ItemScore AlignSequence(ISequence lowerIndexedSequence, ISequence higherIndexedSequence, int lowerIndex, int higherIndex)
        {
            ItemScore bestScore = new ItemScore(lowerIndex, -1, false, false, 0, 0);
            bestScore = AlignAndUpdateBestScore(lowerIndexedSequence, higherIndexedSequence, false, false, bestScore, lowerIndex, higherIndex, Properties.Resource.MsgPlainOverlapItems);

            // Always try reverse complement
            bestScore = AlignAndUpdateBestScore(lowerIndexedSequence.GetReverseComplementedSequence(), higherIndexedSequence, true, true, bestScore, lowerIndex, higherIndex, Properties.Resource.MsgReverseComplementOverlapItems);

            if (!AssumeStandardOrientation)
            {
                // Reverse
                bestScore = AlignAndUpdateBestScore(lowerIndexedSequence.GetReversedSequence(), higherIndexedSequence, true, false, bestScore, lowerIndex, higherIndex, Properties.Resource.MsgReverseOverlapItems);

                // Complement
                bestScore = AlignAndUpdateBestScore(lowerIndexedSequence.GetComplementedSequence(), higherIndexedSequence, false, true, bestScore, lowerIndex, higherIndex, Properties.Resource.MsgComplementOverlapItems);
            }

            return bestScore;
        }

        /// <summary>
        /// Aligns the two input sequence
        /// Updates best score, if necessary
        /// </summary>
        /// <param name="sequence1">First Sequence to be aligned</param>
        /// <param name="sequence2">Second Sequence to be aligned</param>
        /// <param name="reversed">Is first sequence reversed?</param>
        /// <param name="complement">Is first sequence complemented?</param>
        /// <param name="bestScore">Structure to track best score</param>
        /// <param name="sequence1PoolIndex">Index of first sequence in pool.
        /// Used in printing for debug purpose.</param>
        /// <param name="sequence2PoolIndex">Index of second sequence in pool.
        /// Used in printing for debug purpose.</param>
        /// <param name="message">Message to be printed for debug purpose</param>
        /// <returns>Updated best score</returns>
        private ItemScore AlignAndUpdateBestScore(
            ISequence sequence1,
            ISequence sequence2,
            bool reversed,
            bool complement,
            ItemScore bestScore,
            int sequence1PoolIndex,
            int sequence2PoolIndex,
            string message)
        {
            // we will look for the best (largest) overlap score. Note that
            // lower-index items are already in place, so can do this in same pass
            IList<ISequenceAlignment> alignment = RunAlignSimple(sequence1, sequence2);
            if (Trace.Want(Trace.AssemblyDetails))
            {
                ApplicationLog.WriteLine("{0} {1} and {2}", message, sequence1PoolIndex, sequence2PoolIndex);
                Dump(alignment);
            }

            if (alignment.Count > 0 &&
                alignment[0].AlignedSequences.Count > 0)
            {
                long score = 0;
                if (alignment[0].AlignedSequences[0].Metadata.ContainsKey("Score"))
                {
                    try
                    {
                        score = (long) alignment[0].AlignedSequences[0].Metadata["Score"];
                    }
                    catch
                    {
                        // no impl.
                    }
                }

                if (score > bestScore.OverlapScore)
                {
                    bestScore.OverlapScore = score;
                    bestScore.Reversed = reversed;
                    bestScore.Complemented = complement;

                    long offsets = 0;
                    if (alignment[0].AlignedSequences[0].Metadata.ContainsKey("FirstOffset"))
                    {
                        try
                        {
                            offsets = (long) alignment[0].AlignedSequences[0].Metadata["FirstOffset"];
                        }
                        catch
                        {
                            // no impl.
                        }
                    }

                    bestScore.FirstOffset = offsets;
                    offsets = 0;
                    if (alignment[0].AlignedSequences[0].Metadata.ContainsKey("SecondOffset"))
                    {
                        try
                        {
                            offsets = (long) alignment[0].AlignedSequences[0].Metadata["SecondOffset"];
                        }
                        catch
                        {
                            // no impl.
                        }
                    }

                    bestScore.SecondOffset = offsets;
                }
            }

            return bestScore;
        }

        /// <summary>
        /// Execute Simple Align and return Sequence alignment
        /// </summary>
        /// <param name="sequence1">First sequence item</param>
        /// <param name="sequence2">Second sequence item</param>
        /// <returns>List of Sequence alignment</returns>
        private IList<ISequenceAlignment> RunAlignSimple(ISequence sequence1, ISequence sequence2)
        {
            return OverlapAlgorithm.AlignSimple(new[] { sequence1, sequence2 });
        }

        /// <summary>
        /// Analyze the passed contig and store a consensus into its Consensus property.
        /// </summary>
        /// <param name="contig">Contig for which consensus is to be constructed</param>
        private void MakeConsensus(Contig contig)
        {
            List<byte> positionItems = new List<byte>(), consensusSequence = new List<byte>();

            // there's no simple way to pre-guess the length of the contig
            long position = 0;
            while (true)
            {
                // Initialization
                positionItems.Clear();

                // Add the sequences
                positionItems.AddRange(from aseq in contig.Sequences
                                       where position >= aseq.Position && position < aseq.Position + aseq.Sequence.Count
                                       let seqPos = aseq.IsReversed ? (aseq.Sequence.Count() - 1) - (position - aseq.Position) : position - aseq.Position
                                       select aseq.IsComplemented ? aseq.Sequence.GetComplementedSequence()[seqPos] : aseq.Sequence[seqPos]);

                if (positionItems.Count == 0)
                {
                    // This means no sequences at this position. We're done
                    contig.Consensus = new Sequence(Alphabets.AmbiguousAlphabetMap[_sequenceAlphabet], consensusSequence.ToArray());
                    return;
                }

                consensusSequence.Add(ConsensusResolver.GetConsensus(positionItems.ToArray()));
                position++;
            }
        }
        #endregion

        #region Nested Structs
        /// <summary>
        /// An ItemScore is the overlap score between the current item (owner of this struct)
        /// and a lower-indexed item. The lower-indexed item may have been reversed or 
        /// complemented (or both) to get that score. We always perform reverse and/or
        /// complement on the lower-indexed item (at no loss of generality).
        /// </summary>
        private struct ItemScore
        {
            /// <summary>
            /// the pool index of the lower-indexed item 
            /// </summary>
            internal int OtherItem;

            /// <summary>
            /// the overlap score 
            /// </summary>
            internal double OverlapScore;

            /// <summary>
            /// true if the lower-indexed item was reversed 
            /// </summary>
            internal bool Reversed;

            /// <summary>
            /// true if the lower-indexed item was complemented 
            /// </summary>
            internal bool Complemented;

            /// <summary>
            /// the offset to apply to the first sequence (from the
            /// right if reversed, from the left if not reversed) 
            /// </summary>
            internal long FirstOffset;

            /// <summary>
            /// the offset to apply to the second sequence (always from 
            /// the left, since it's never reversed)
            /// </summary>
            internal long SecondOffset;

            /// <summary>
            /// Initializes a new instance of the ItemScore struct
            /// constructor that sets all properties 
            /// </summary>
            /// <param name="otherItem">Pool index of the lower-indexed item</param>
            /// <param name="overlapScore">Overlap score</param>
            /// <param name="reversed">Was lower-indexed item reversed</param>
            /// <param name="complemented">Was lower-indexed item complemented</param>
            /// <param name="firstOffset">First sequence offset</param>
            /// <param name="secondOffset">Second sequence offset</param>
            internal ItemScore(
                int otherItem,
                double overlapScore,
                bool reversed,
                bool complemented,
                long firstOffset,
                long secondOffset)
            {
                OtherItem = otherItem;
                OverlapScore = overlapScore;
                Reversed = reversed;
                Complemented = complemented;
                FirstOffset = firstOffset;
                SecondOffset = secondOffset;
            }

            /// <summary>
            /// Initializes a new instance of the ItemScore struct
            /// copy constructor
            /// </summary>
            /// <param name="other">instance from which item score need to be copied</param>
            internal ItemScore(ItemScore other)
            {
                OtherItem = other.OtherItem;
                OverlapScore = other.OverlapScore;
                Reversed = other.Reversed;
                Complemented = other.Complemented;
                FirstOffset = other.FirstOffset;
                SecondOffset = other.SecondOffset;
            }
        }
        #endregion

        #region Nested Classes
        /// <summary>
        /// A PoolItem is one item in the merge pool (either a sequence or a contig)
        /// along with its overlap scores with lower-numbered items.
        /// </summary>
        private class PoolItem
        {
            /// <summary>
            /// Flag that distinguished whether _item is a contig or sequence. 
            /// </summary>
            private bool _isContig;

            /// <summary>
            /// Initializes a new instance of the PoolItem class.
            /// constructor for contig.
            /// </summary>
            /// <param name="item">Pool object</param>
            internal PoolItem(Contig item)
                : this(item, true)
            {
            }

            /// <summary>
            /// Initializes a new instance of the PoolItem class.
            /// constructor for sequence.
            /// </summary>
            /// <param name="item">Pool object</param>
            internal PoolItem(ISequence item)
                : this(item, false)
            {
            }

            /// <summary>
            /// Initializes a new instance of the PoolItem class.
            /// constructor that sets item and type.
            /// </summary>
            /// <param name="item">Pool object</param>
            /// <param name="isContig">Is it contig</param>
            private PoolItem(object item, bool isContig)
            {
                _isContig = isContig;
                Item = item;
                Scores = new List<ItemScore>();
                ConsumedBy = -1;
            }

            /// <summary>
            /// Gets a value indicating whether item is a contig. 
            /// </summary>
            internal bool IsContig
            {
                get
                {
                    return _isContig;
                }
            }

            /// <summary>
            /// Gets stored Contig, assuming there is one.
            /// </summary>
            internal Contig Contig
            {
                get
                {
                    if (!_isContig)
                    {
                        string message = Properties.Resource.PoolItemNotContig;
                        Trace.Report(message);
                        throw new Exception(message);
                    }

                    return (Contig)Item;
                }
            }

            /// <summary>
            /// Gets stored sequence, assuming there is one.
            /// </summary>
            internal ISequence Sequence
            {
                get
                {
                    if (_isContig)
                    {
                        string message = Properties.Resource.PoolItemNotSequence;
                        Trace.Report(message);
                        throw new Exception(message);
                    }

                    return (ISequence)Item;
                }
            }

            /// <summary>
            /// Gets the sequence that will be aligned with other pool items. For
            /// a contig, this is the consensus; for a sequence, it's just the sequence. 
            /// </summary>
            internal ISequence SequenceOrConsensus
            {
                get
                {
                    if (_isContig)
                    {
                        return ((Contig)Item).Consensus;
                    }
                    else
                    {
                        return (ISequence)Item;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the item.
            /// Item can either be an ISequence, or a Contig.
            /// </summary>
            internal object Item { get; set; }

            /// <summary>
            /// Gets or sets list of overlap scores.
            /// List stores the overlap scores with all pool items 
            /// that have a lower index than this.
            /// </summary>
            internal List<ItemScore> Scores { get; set; }

            /// <summary>
            /// Gets or sets the index of the pool item that replaced it.
            /// If a pool item has been merged, consumedBy will be
            /// the index of the pool item that replaced it.
            /// A negative value means the  item is still unmerged.
            /// </summary>
            internal int ConsumedBy { get; set; }

            /// <summary>
            /// Free the item reference when no longer needed.
            /// </summary>
            internal void FreeSequences()
            {
                Item = null;
            }
        }
        #endregion
    }
}
