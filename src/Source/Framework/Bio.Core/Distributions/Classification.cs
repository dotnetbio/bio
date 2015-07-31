namespace Bio.Distributions
{
    /// <summary>
    /// Enumeration representing binary distribution.
    /// </summary>
    public enum Classification
    {
        /// <summary>
        /// Missing values which cannot be classified in either of two states.
        /// </summary>
        Missing = BooleanStatistics.Missing,

        /// <summary>
        /// False state.
        /// </summary>
        False = BooleanStatistics.False,

        /// <summary>
        /// True State.
        /// </summary>
        True = BooleanStatistics.True
    };
}
