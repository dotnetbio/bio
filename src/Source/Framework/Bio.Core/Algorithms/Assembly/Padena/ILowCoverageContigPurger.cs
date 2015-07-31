using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Interface removing contigs with low coverage.
    /// </summary>
    public interface ILowCoverageContigPurger
    {
        /// <summary>
        /// Build contigs from graph. For contigs whose coverage is less than 
        /// the specified threshold, remove graph nodes belonging to them.
        /// </summary>
        /// <param name="deBruijnGraph">DeBruijn Graph.</param>
        /// <param name="coverageThresholdForContigs">Coverage Threshold for contigs.</param>
        /// <returns>Number of nodes removed.</returns>
        long RemoveLowCoverageContigs(DeBruijnGraph deBruijnGraph, double coverageThresholdForContigs);
    }
}
