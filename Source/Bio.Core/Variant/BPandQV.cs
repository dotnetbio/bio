using System;

namespace Bio.Variant
{
    /// <summary>
    /// A Basepair and corresponding QV value, used to keep the two values together.
    /// </summary>
    public struct BPandQV
    {
        /// <summary>
        /// A, C, G, T or '-'
        /// </summary>
        public readonly byte BP;

        /// <summary>
        /// A QV Value for the base. Should be set to 0 if no information is available
        /// or is a gap character.
        /// </summary>
        public readonly byte QV;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bio.Variant.BPandQV"/> struct.
        /// </summary>
        /// <param name="bp">Bp.</param>
        /// <param name="qv">QV value</param>
        /// <param name="validate">If set to <c>true</c> validate that the BP is an A, C, G or T</param>
        public BPandQV (byte bp, byte qv, bool validate=true)
        {
            if (validate && 
               !(bp != 'A' ||
                bp != 'C' ||
                bp != 'G' ||
                bp != 'T' ||
                bp != '-')) {
                throw new ArgumentOutOfRangeException (nameof(bp), "Basepairs must be A, C, G, T or -");
            }
            BP = bp;
            QV = qv;
        }
    }
}

