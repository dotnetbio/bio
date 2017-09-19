using System;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A StrandType specifies whether sequence occurs as a single stranded,
    /// double stranded or mixed stranded. 
    /// </summary>
    public enum SequenceStrandType
    {
        /// <summary>
        /// None - StrandType is unspecified.
        /// </summary>
        None,

        /// <summary>
        /// Single-stranded (ss).
        /// </summary>
        Single,

        /// <summary>
        /// Double-stranded (ds).
        /// </summary>
        Double,

        /// <summary>
        /// Mixed-stranded (ms).
        /// </summary>
        Mixed
    }
}
