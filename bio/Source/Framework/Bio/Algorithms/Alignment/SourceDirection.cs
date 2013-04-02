namespace Bio.Algorithms.Alignment
{
    /// <summary> Direction to source of cell value, used during traceback. </summary>
    static class SourceDirection
    {
        // This is coded as a set of consts rather than using an enum.  Enums are ints and 
        // referring to these in the code requires casts to and from (sbyte), which makes
        // the code more difficult to read.

        /// <summary> Source was up and left from current cell. </summary>
        public const sbyte Diagonal = 0;

        /// <summary> Source was up from current cell. </summary>
        public const sbyte Up = 1;

        /// <summary> Source was left of current cell. </summary>
        public const sbyte Left = 2;

        /// <summary> During traceback, stop at this cell (used by SmithWaterman). </summary>
        public const sbyte Stop = -1;

        /// <summary> During traceback, stop at this cell. </summary>
        public const sbyte Block = -2;

        /// <summary> Error code, if cell has code Invalid error has occurred. </summary>
        public const sbyte Invalid = -3;
    }
}