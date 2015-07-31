using System;
using System.Collections.Generic;

using Bio.Util.Logging;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Contig is a data storage object representing a set of sequences
    /// that have been assembled into a new, longer sequence.
    /// </summary>
    public class Contig
    {
        #region Member Variables

        /// <summary>
        /// Assembled sequences.
        /// </summary>
        private IList<AssembledSequence> sequences = new List<AssembledSequence>();

        #endregion        

        #region Properties

        /// <summary>
        /// Gets or sets the set of sequences that have been assembled to form the contig.
        /// </summary>
        public IList<AssembledSequence> Sequences
        {
            get
            {
                return this.sequences;
            }

            set
            {
                this.sequences = value;
            }
        }

        /// <summary>
        /// Gets or sets a sequence derived from the input sequences as assembled, representing the
        /// contents of the whole range of the contig.
        /// <remarks>
        /// This is built by an IConsensusMethod.
        /// </remarks>
        /// </summary>
        public ISequence Consensus { get; set; }

        /// <summary>
        /// Gets the length of the contig equals the length of its consensus.
        /// </summary>
        public long Length
        {
            get
            {
                if (this.Consensus == null)
                {
                    string message = Properties.Resource.ContigLength;
                    Trace.Report(message);
                    throw new ArgumentNullException(message);
                }

                return this.Consensus.Count;
            }
        }        

        #endregion

        #region Nested Structs
        /// <summary>
        /// A sequence, as it has been located into the contig. This includes
        /// possible reversal, complementation, or both.
        /// </summary>
        public struct AssembledSequence
        {
            /// <summary>
            /// Gets or sets the sequence, as possibly modified (via gap insertion) by
            /// the overlap algorithm.
            /// </summary>
            public ISequence Sequence { get; set; }

            /// <summary>
            /// Gets or sets the offset from the start of the contig where this sequence begins.
            /// </summary>
            public long Position { get; set; }
            
            /// <summary>
            /// Gets or sets a value indicating whether the sequence was complemented in order to find sufficient overlap.
            /// </summary>
            public bool IsComplemented { get; set; }
            
            /// <summary>
            /// Gets or sets a value indicating whether the orientation of the sequence was reversed in order to find
            /// sufficient overlap.
            /// <remarks>
            /// If the assembly algorithm used AssumeStandardOrientation=true, then IsReversed
            /// and IsComplemented will both be true (reverse complement) or both be false.
            /// </remarks>
            /// </summary>
            public bool IsReversed { get; set; }

            /// <summary>
            /// Gets or sets the position of the Read in alignment.
            /// </summary>
            public long ReadPosition { get; set; }

            /// <summary>
            /// Gets or sets the length of alignment between read and contig.
            /// </summary>
            public long Length { get; set; }
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Converts Consensus Sequence data to string.
        /// </summary>
        /// <returns>Consensus Sequence Data.</returns>
        public override string ToString()
        {
            return this.Consensus.ToString();
        }

        #endregion
    }
}
