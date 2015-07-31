namespace Bio.Util.Distribute
{
    /// <summary>
    /// An interface that describes an object that can distribute something of type IDistributable.
    /// </summary>
    public interface IDistribute
    {
        /// <summary>
        /// Run the work on this distributableObject
        /// </summary>
        /// <param name="distributableObject">Distributable Object.</param>
        void Distribute(IDistributable distributableObject);
    }
}
