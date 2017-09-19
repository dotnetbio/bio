using System.Collections.Generic;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// An IOverlapDeNovoAssembly is the result of running 
    /// Overlap based De Novo Assembly on a set of two or more sequences. 
    /// </summary>
    public interface IOverlapDeNovoAssembly : IDeNovoAssembly
    {
        /// <summary>
        /// Gets list of contigs created after Assembly.
        /// </summary>
        IList<Contig> Contigs { get; }

        /// <summary>
        /// Gets list of sequences that could not be merged into any contig.
        /// </summary>
        IList<ISequence> UnmergedSequences { get; }
    }
}
