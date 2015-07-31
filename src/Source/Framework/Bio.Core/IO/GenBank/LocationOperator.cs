namespace Bio.IO.GenBank
{
    /// <summary>
    /// Enum for location operators.
    /// </summary>
    public enum LocationOperator
    {
        /// <summary>
        /// No Operator.
        /// </summary>
        None,

        /// <summary>
        /// Complement Operator.
        /// </summary>
        Complement,

        /// <summary>
        /// Join Operator.
        /// </summary>
        Join,

        /// <summary>
        /// Order Operator.
        /// </summary>
        Order,

        /// <summary>
        /// Bond Operator.
        /// Found in protein files. 
        /// These generally are used to describe disulfide bonds.
        /// </summary>
        Bond
    }
}
