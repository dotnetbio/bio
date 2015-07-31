namespace Bio.Distributions
{
    /// <summary>
    ///	 none (the old Mixture==false), if a position contains {ab}, consider it missing with regards to a and b, but say it doesn't have c.
    ///	 pure (the old Mixture==true), if a position contains {ab} say that it has neither.
    ///	 any  (a new possibility), if a position contains {ab}, say that it has both.
    /// </summary>
    public enum MixtureSemantics
    {
        /// <summary>
        /// (the old Mixture==false),   
        /// </summary>
        none,
        /// <summary>
        /// (the old Mixture==true), if a position contains {ab} say that it has neither.
        /// </summary>
        pure,
        /// <summary>
        /// any  (a new possibility), if a position contains {ab}, say that it has both.
        /// </summary>
        any
    }
}
