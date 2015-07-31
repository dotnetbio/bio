using System;

namespace Bio.IO.SAM
{
    /// <summary>
    /// SAM Flags.
    /// This enum represents the bitwise flags of the SAM format.
    /// </summary>
    [Flags]
    public enum SAMFlags
    {
        /// <summary>
        /// 0x0001 The read is paired in sequencing, no matter whether it is mapped in a pair.
        /// </summary>
        PairedRead = 0x0001,

        /// <summary>
        /// 0x0002 The read is mapped in a proper pair (depends on the protocol, normally inferred during alignment).
        /// </summary>
        MappedInProperPair = 0x0002,

        /// <summary>
        /// 0x0004 The query sequence itself is unmapped.
        /// </summary>
        UnmappedQuery = 0x0004,

        /// <summary>
        /// 0x0008 The mate is unmapped.
        /// </summary>
        UnmappedMate = 0x0008,

        /// <summary>
        /// 0x0010 Strand of the query (0 for forward; 1 for reverse strand).
        /// </summary>
        QueryOnReverseStrand = 0x0010,

        /// <summary>
        /// 0x0020 Strand of the mate.
        /// </summary>
        MateOnReverseStrand = 0x0020,

        /// <summary>
        /// 0x0040 The read is the first read in a pair.
        /// </summary>
        FirstReadInPair = 0x0040,

        /// <summary>
        /// 0x0080 The read is the second read in a pair.
        /// </summary>
        SecondReadInPair = 0x0080,

        /// <summary>
        /// 0x0100 The alignment is not primary (a read having split hits may have multiple primary alignment records).
        /// </summary>
        NonPrimeAlignment = 0x0100,

        /// <summary>
        /// 0x0200 The read fails platform/vendor quality checks.
        /// </summary>
        QualityCheckFailure = 0x0200,

        /// <summary>
        /// 0x0400 The read is either a PCR duplicate or an optical duplicate.
        /// </summary>
        Duplicate = 0x0400
    }
}
