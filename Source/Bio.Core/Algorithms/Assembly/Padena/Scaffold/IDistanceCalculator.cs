namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Classes implementing interface calculates distance between contigs using 
    /// mate pair information.
    /// </summary>
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calculates distances between contigs.
        /// </summary>
        ContigMatePairs CalculateDistance();
    }
}
