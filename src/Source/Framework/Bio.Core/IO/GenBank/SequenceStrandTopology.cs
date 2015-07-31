using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A StrandTopology specifies whether the strand is linear or circular.
    /// </summary>
    public enum SequenceStrandTopology
    {
        /// <summary>
        /// None - StrandTopology is unspecified.
        /// </summary>
        None,

        /// <summary>
        /// Linear.
        /// </summary>
        Linear,

        /// <summary>
        /// Circular.
        /// </summary>
        Circular,
    }
}
