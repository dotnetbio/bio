using System;
using System.Collections.Generic;
using System.IO;
using Bio.Algorithms.Assembly;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// This creates a contig parser that uses an XSV sparse reader to parse
    /// a contig from a list of sparse sequences, where the first sequence is the
    /// consensus and the rest are sequences aligned to it.
    /// </summary>
    public class XsvContigParser : XsvSparseParser
    {
        private readonly char separator;
        private readonly char sequenceIdPrefix;

        ///<summary>
        /// Creates a contig parser that parses Contigs using the given encoding
        /// and alphabet, by creating an XsvSparseReader that uses the given separator 
        /// and sequenceIdPrefix characters.
        ///</summary>
        ///<param name="alphabet">Alphabet to use for the consensus and assembled sequences that are parsed.</param>
        ///<param name="separatorChar">Character used to separate sequence item position and symbol in the Xsv file</param>
        ///<param name="sequenceIdPrefixChar">Character used at the beginning of the sequence start line.</param>
        public XsvContigParser(IAlphabet alphabet, char separatorChar, char sequenceIdPrefixChar)
            : base(alphabet, separatorChar, sequenceIdPrefixChar)
        {
            separator = separatorChar;
            sequenceIdPrefix = sequenceIdPrefixChar;
        }

        /// <summary>
        /// This converts a list of sparse sequences read from the passed stream into a contig.
        /// Assumes the first sequence is the consensus and the rest are assembled sequences.
        /// The positions of the assembed sequences are the offsets of the sparse sequences in
        /// the sequence start line. The positions of the sequence items are the same as their
        /// position field value in each character separated line 
        /// (i.e. they are not incremented by the offset)
        /// </summary>
        /// <returns>The parsed contig with consensus and assembled sequences, all represented 
        /// as SparseSequences. 
        /// Null if no lines were present in the reader. Exception if valid sparse sequences
        /// were not present. 
        /// NOTE: This does not check if the assembled sequence positions are valid with respect to the consensus.
        /// </returns>
        public Contig ParseContig(Stream stream)
        {
            // parse the consensus
            using (var reader = new StreamReader(stream))
            {
                XsvSparseReader sparseReader = new XsvSparseReader(reader, separator, sequenceIdPrefix);
                ISequence consensus = ParseOne(sparseReader);
                return consensus == null
                           ? null
                           : new Contig { Consensus = consensus, Sequences = this.ParseAssembledSequence(sparseReader) };
            }
        }

        /// <summary>
        /// Parses a list of assembled sparse sequences from the reader.
        /// </summary>
        /// <param name="contigReader">The reader to read the assembled sparse sequences from
        /// Flag to indicate whether the resulting sequences should be in readonly mode or not.
        /// If this flag is set to true then the resulting sequences's isReadOnly property 
        /// will be set to true, otherwise it will be set to false.
        /// </param>
        /// <returns>Returns contig assemble sequence.</returns>
        protected IList<Contig.AssembledSequence> ParseAssembledSequence(XsvSparseReader contigReader)
        {
            // Check input arguments
            if (contigReader == null) 
            {
                throw new ArgumentNullException("contigReader");
            }

            var sequenceList = new List<Contig.AssembledSequence>();
            while (contigReader.HasLines)
            {
                sequenceList.Add(new Contig.AssembledSequence { Sequence = this.ParseOne(contigReader) });
            }
            return sequenceList;
        }
    }
}
