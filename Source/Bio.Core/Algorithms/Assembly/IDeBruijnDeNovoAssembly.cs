using System.Collections.Generic;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// An IDeBruijnDeNovoAssembly is the result of running De Bruijn graph based 
    /// De Novo Assembly on a set of sequences. 
    /// </summary>
    public interface IDeBruijnDeNovoAssembly : IDeNovoAssembly
    {
        /// <summary>
        /// Gets list of contig sequences created by assembler.
        /// </summary>
        IList<ISequence> ContigSequences { get; }

        /// <summary>
        /// Gets the list of assembler scaffolds.
        /// </summary>
        IList<ISequence> Scaffolds { get; }
    }
}
