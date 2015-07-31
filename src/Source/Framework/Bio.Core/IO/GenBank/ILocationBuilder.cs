namespace Bio.IO.GenBank
{
    /// <summary>
    /// Interface to build the location from location string and from location object to location string.
    /// </summary>
    public interface ILocationBuilder
    {
        /// <summary>
        /// Returns the location object for the specified location string.
        /// </summary>
        /// <param name="location">Location string.</param>
        ILocation GetLocation(string location);

        /// <summary>
        /// Returns the location string for the specified location.
        /// </summary>
        /// <param name="location">Location instance.</param>
        string GetLocationString(ILocation location);
    }
}
