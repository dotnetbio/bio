using System.Collections.Generic;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Removes containing paths and merge Overlapping scaffold paths.
    /// Containing Paths
    /// -------------- Contig 1
    ///     --------   Contig 2
    /// Overlapping Paths
    /// --------        Contig 1 
    ///     ---------   Contig 2
    /// </summary>
    public interface IPathPurger
    {
        /// <summary>
        /// Removes containing paths and merge overlapping paths.
        /// </summary>
        /// <param name="scaffoldPaths">Input paths/scaffold.</param>
        void PurgePath(IList<ScaffoldPath> scaffoldPaths);
    }
}
