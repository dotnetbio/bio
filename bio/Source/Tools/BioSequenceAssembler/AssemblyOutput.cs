namespace SequenceAssembler
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Bio.Algorithms.Assembly;

    #endregion -- Using Directive --

    /// <summary>
    /// AssemblyOutput is a buisness object class which
    /// holds the output of a particular assembly process.
    /// </summary>
    public class AssemblyOutput
    {
        #region -- Private Methods -- 

        /// <summary>
        /// Indicates the no of sequences used during assembly process.
        /// </summary>
        private int noOfSequence;

        /// <summary>
        /// Indicates the alignment algorithm used.
        /// </summary>
        private string algorithmNameUsed;

        /// <summary>
        /// Start time of the assembly.
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// End time of the assembly.
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// Number of contigs generated during the assembly process.
        /// </summary>
        private int noOfContigs;

        /// <summary>
        /// Number of un assembled sequences in the assembly.
        /// </summary>
        private int noOfUnassembledSequence;

        /// <summary>
        /// Total length of all the contigs generated.
        /// </summary>
        private long totalLength;

        /// <summary>
        /// No of base pairs 
        /// </summary>
        private int basePairs;

        /// <summary>
        /// Output of assembly process.
        /// </summary>
        private IDeNovoAssembly sequenceAssembly;

        #endregion -- Private Methods --

        #region -- Public Properties --

        /// <summary>
        /// Gets or sets the output of assembly process.
        /// </summary>
        public IDeNovoAssembly SequenceAssembly
        {
            get { return this.sequenceAssembly; }
            set { this.sequenceAssembly = value; }
        }

        /// <summary>
        /// Gets or sets no of base pairs. 
        /// </summary>
        public int BasePairs
        {
            get { return this.basePairs; }
            set { this.basePairs = value; }
        }

        /// <summary>
        /// Gets or sets total length of all the contigs generated.
        /// </summary>
        public long TotalLength
        {
            get { return this.totalLength; }
            set { this.totalLength = value; }
        }

        /// <summary>
        /// Gets or sets number of un assembled sequences in the assembly.
        /// </summary>
        public int NoOfUnassembledSequence
        {
            get { return this.noOfUnassembledSequence; }
            set { this.noOfUnassembledSequence = value; }
        }

        /// <summary>
        /// Gets or sets number of contigs generated during the assembly process.
        /// </summary>
        public int NoOfContigs
        {
            get { return this.noOfContigs; }
            set { this.noOfContigs = value; }
        }

        /// <summary>
        /// Gets or sets end time of the assembly.
        /// </summary>
        public DateTime EndTime
        {
            get { return this.endTime; }
            set { this.endTime = value; }
        }

        /// <summary>
        /// Gets or sets start time of the assembly.
        /// </summary>
        public DateTime StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        /// <summary>
        /// Gets or sets alignment algorithm used.
        /// </summary>
        public string AlgorithmNameUsed
        {
            get { return this.algorithmNameUsed; }
            set { this.algorithmNameUsed = value; }
        }

        /// <summary>
        /// Gets or sets the no of sequences used during assembly process.
        /// </summary>
        public int NoOfSequence
        {
            get { return this.noOfSequence; }
            set { this.noOfSequence = value; }
        }

        /// <summary>
        /// Gets or Sets which assembler is selected by user
        /// </summary>
        public AssemblerType AssemblerUsed { get; set; }

        /// <summary>
        /// Used to store contigs from a denovo assembly result
        /// </summary>
        public IEnumerable<Contig> Contigs { get; set; }

        #endregion -- Public Properties --
    }

    /// <summary>
    /// Holds the assembler types supported
    /// </summary>
    public enum AssemblerType
    {
        SimpleSequenceAssembler,
        PaDeNA
    }
}
