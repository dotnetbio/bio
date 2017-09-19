using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Segment provides the information on the order in which this entry appears in a
    /// series of discontinuous sequences from the same molecule.
    /// </summary>
    public class SequenceSegment
    {
        #region Properties
        /// <summary>
        /// Current segment number.
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// Total number of segments.
        /// </summary>
        public int Count { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new SequenceSegment that is a copy of the current SequenceSegment.
        /// </summary>
        /// <returns>A new SequenceSegment that is a copy of this SequenceSegment.</returns>
        public SequenceSegment Clone()
        {
            return (SequenceSegment)MemberwiseClone();
        }
        #endregion Methods
    }
}
