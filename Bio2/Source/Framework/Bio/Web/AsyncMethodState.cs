namespace Bio.Web
{
    /// <summary>
    /// This Enumeration represents state of asynchronous web method call process.
    /// </summary>
    public enum AsyncMethodState
    {
        /// <summary>
        /// Not started
        /// </summary>
        NotStarted,
        /// <summary>
        /// Started
        /// </summary>
        Started,
        /// <summary>
        /// Passed
        /// </summary>
        Passed,
        /// <summary>
        /// Failed
        /// </summary>
        Failed,
    }
}
