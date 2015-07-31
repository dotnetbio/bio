using System;
using System.Collections.Generic;
using System.Text;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// AlignedSequence is a class containing the single aligned unit of alignment.
    /// </summary>
    public class AlignedSequence : IAlignedSequence
    {
        /// <summary>
        /// Initializes a new instance of the AlignedSequence class.
        /// </summary>
        public AlignedSequence()
        {
            this.Metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.Sequences = new List<ISequence>();
        }

        /// <summary>
        /// Initializes a new instance of the AlignedSequence class
        /// Internal constructor to create AlignedSequence instance from IAlignedSequence.
        /// </summary>
        /// <param name="alignedSequence">IAlignedSequence instance.</param>
        internal AlignedSequence(IAlignedSequence alignedSequence)
        {
            this.Metadata = alignedSequence.Metadata;
            this.Sequences = new List<ISequence>(alignedSequence.Sequences);
        }

        /// <summary>
        /// Gets information about the AlignedSequence, like score, offsets, consensus, etc..
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Gets list of sequences involved in the alignment.
        /// </summary>
        public IList<ISequence> Sequences { get; set; }

        /// <summary>
        /// Converts sequenceData of all the Sequences in the list to string.
        /// </summary>
        /// <returns>sequenceData of all the Sequences in the list.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ISequence seq in Sequences)
            {
                builder.AppendLine(seq.ToString());
            }
            return builder.ToString();
        }
    }
}
