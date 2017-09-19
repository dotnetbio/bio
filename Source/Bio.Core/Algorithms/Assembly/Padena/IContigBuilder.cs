using System.Collections.Generic;
using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Framework for building contig sequence from de bruijn graph.
    /// </summary>
    public interface IContigBuilder
    {
        /// <summary>
        /// Contructs the contigs by performing graph walking
        /// or graph modification.
        /// </summary>
        /// <param name="deBruijnGraph">Input graph.</param>
        /// <returns>List of contigs.</returns>
        IEnumerable<ISequence> Build(DeBruijnGraph deBruijnGraph);
    }
}
