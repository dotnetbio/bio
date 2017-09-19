using System.Collections.Generic;
using Bio.IO.SAM;
using System;
using System.Linq;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold BAM file index information related to a reference sequence.
    /// Holds bin and linear index information for a reference sequence.
    /// </summary>
    public class BAMReferenceIndexes
    {
        /// <summary>
        /// Gets list of Bin index information.
        /// </summary>
        public IList<Bin> Bins { get; private set; }

        /// <summary>
        /// Gets list of Linear file offsets.
        /// An array holding the linear offsets to be output
        /// </summary>
        public IList<FileOffset> LinearIndex { get; private set; }

        /// <summary>
        /// An array holding the linear offsets to be output
        /// </summary>
        LinearIndexArrayMaker LinearIndexMaker;
       
        /// <summary>
        /// Does this read have meta data associated with it? (number of mapped/unmapped reads?)
        /// Note that the meta-data is not standardized yet across implementations
        /// </summary>
        public bool HasMetaData {get;internal set;}
        /// <summary>
        /// Count of the number of mapped reads, may be inferred from index file.
        /// </summary>
        public ulong MappedReadsCount { get; internal set; }
        /// <summary>
        /// Count of the number of unmapped reads (Typically 
        /// those where a read does not align to a reference but its 
        /// mate does, though this is not guaranteed).
        /// </summary>
        public ulong UnMappedReadsCount { get; internal set; }

        /// <summary>
        /// The location of the first offset in this file for a read from this index
        /// </summary>
        public FileOffset FirstOffSetSeen { get; private set; }
        /// <summary>
        /// The location of the last offset in this file for a read from this index.
        /// </summary>
        public FileOffset LastOffSetSeen{ get; private set; }

        /// <summary>
        /// Creates new instance of BAMReferenceIndexes class. That will be populated with data to make an
        /// index
        /// </summary>
        public BAMReferenceIndexes(int refSequenceLength)
        {
            Bins = new List<Bin>();
            LinearIndexMaker = new LinearIndexArrayMaker(refSequenceLength);            
        }
        /// <summary>
        /// Create a new instance of the class that will be populated with data to make an instance.
        /// </summary>
        public BAMReferenceIndexes()
        {
            Bins = new List<Bin>();
            LinearIndex = new List<FileOffset>();
            
        }
        /// <summary>
        /// Add a read being parsed to the index
        /// </summary>
        /// <param name="alignedSequence"></param>
        /// <param name="offset"></param>
        internal void AddReadToIndexInformation(SAM.SAMAlignedSequence alignedSequence, FileOffset offset)
        {
            if (offset < FirstOffSetSeen) { FirstOffSetSeen = offset; }
            else if (offset > LastOffSetSeen) { LastOffSetSeen = offset; }            
            //update meta-data
            if (alignedSequence.Pos < 1 || (alignedSequence.Flag&SAMFlags.UnmappedQuery)==SAMFlags.UnmappedQuery)
            {
                UnMappedReadsCount++;
                return;
            }
            else
            {
                MappedReadsCount++;
            }
            //update linear index
            LinearIndexMaker.UpdateLinearArrayIndex(alignedSequence, offset);
        }
        /// <summary>
        /// Finish creating the class.
        /// TODO: Separate out into factory method?
        /// </summary>
        internal void Freeze()
        {
            if (MappedReadsCount > 0 || UnMappedReadsCount > 0)
                HasMetaData = true;
            LinearIndex=LinearIndexMaker.Freeze();
            LinearIndexMaker = null;
        }
        /// <summary>
        /// Array which holds the linear index and associated offsets for each 
        /// of the bins possible for all 16 kb regions of this sequence.
        /// </summary>
        internal class LinearIndexArrayMaker
        {          
          
            private FileOffset[] offSetArray;
            private int largestBinSeen=0;
            internal LinearIndexArrayMaker(int refSeqLength)
            {
                int size;
                if (refSeqLength <= 0) { size = BAMIndexStorage.MaxLinerindexArraySize; }
                else { size = BAMIndexStorage.LargestBinPossibleForSequenceLength(refSeqLength); }
                //TODO: Plus one now because weird things could happen with alignment length such that the bin is one higher
                //never seen this, but not sure it can't happen.
                offSetArray = new FileOffset[size+1];
            }
            /// <summary>
            /// Update the linear index array based on an aligned read and its current coordinates
            /// </summary>
            /// <param name="alignedSeq"></param>
            /// <param name="offset"></param>
            internal void UpdateLinearArrayIndex(SAMAlignedSequence alignedSeq, FileOffset offset)
            {
                int pos = alignedSeq.Pos > 0 ? alignedSeq.Pos - 1 : 0;
                int end = alignedSeq.RefEndPos > 0 ? alignedSeq.RefEndPos - 1 : 0;
                pos = pos >> 14;
                end = end >> 14;
                if (end > largestBinSeen) {largestBinSeen = end;}
                for (int i = pos; i <= end; i++)
                {
                    var cur = offSetArray[i];
                    //TODO: Is second check necessary?  Seems to always be true as we are doing things in order
                    if (cur.BothDataElements == 0 || cur > offset) {
                        offSetArray[i] = offset;
                    }
                }
            }
            /// <summary>
            /// Called after all the data has been added
            /// </summary>
            internal List<FileOffset> Freeze()
            {
                if (largestBinSeen != 0)
                {
                    Array.Resize(ref offSetArray, largestBinSeen + 1);
                    FileOffset lastOffSetSeen = new FileOffset();
                    for (int i = 0; i < offSetArray.Length; i++)
                    {
                        if (offSetArray[i].BothDataElements == 0)
                        {
                            offSetArray[i] = lastOffSetSeen;
                        }
                        else { lastOffSetSeen = offSetArray[i]; }
                    }
                }
                else {
                    Array.Resize(ref offSetArray, 0);

                }
                return offSetArray.ToList();
            }       
        }
    }
}
