using System;
using System.ComponentModel.Composition;
using Bio.Algorithms.Assembly;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// This class will write a contig as a list of sparse sequences using the 
    /// XsvSparseFormatter. The first sequence is the consensus, and the rest are
    /// the assembled sequences offset from the consensus. 
    /// E.g. formatting a Contig with 2 assembled sequences, using '#' as sequence prefix and ',' as character separator.
    /// # 0,100,Consensus
    /// 12,A
    /// 29,T
    /// 39,G
    /// #3,10,Fragment1
    /// 9,A
    /// #25,20,Fragment2
    /// 4,T
    /// 14,G
    /// </summary>
    [PartNotDiscoverable]
    public class XsvContigFormatter : XsvSparseFormatter 
    {
        /// <summary>
        /// Creates a formatter for contigs using the given separator and 
        /// sequence start line prefix characacter.
        /// </summary>
        /// <param name="filePath">File to be parsed.</param>
        /// <param name="separator">The character to separate position of the sequence 
        /// item from its symbol, and separate the offset, count and sequence ID in the 
        /// sequence start line.</param>
        /// <param name="sequenceIDPrefix">The character to refix the sequence start line.</param>
        public XsvContigFormatter(string filePath, char separator, char sequenceIDPrefix)
            : base(filePath, separator, sequenceIDPrefix)
        { }

        /// <summary>
        /// Formats a (sparse) contig to a charcter separated value file,
        /// writing the consensus first, followed by the sequence separator,
        /// and each assembled sequences followed by the sequence separator.
        /// The consensus has an offet of 0, while the assembed sequences have the
        /// offset as present in AssembledSequence.Position.
        /// </summary>
        /// <param name="contig">The contig to format as a set of sparse sequences.</param>
        public void Write (Contig contig) 
        {
            if (contig == null)
            {
                throw new ArgumentNullException("contig");
            }

            Write(contig.Consensus);
            foreach (Contig.AssembledSequence aSeq in contig.Sequences) 
            {
                Format(aSeq.Sequence, (long)aSeq.Sequence.Metadata[XsvSparseParser.MetadataOffsetKey]);
            }
        }
    }
}
