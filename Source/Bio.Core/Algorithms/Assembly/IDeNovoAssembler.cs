using System.Collections.Generic;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Representation of any sequence assembly algorithm.
    /// This interface defines contract for classes implementing De Novo Sequence assembler.
    /// </summary>
    public interface IDeNovoAssembler
    {
        /// <summary>
        /// Gets the name of the sequence assembly algorithm being
        /// implemented. This is intended to give the
        /// developer some information of the current sequence assembly algorithm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the sequence assembly algorithm being
        /// implemented. This is intended to give the
        /// developer some information of the current sequence assembly algorithm.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Assemble the input sequences into the largest possible contigs. 
        /// </summary>
        /// <param name="inputSequences">The sequences to assemble.</param>
        /// <returns>IDeNovoAssembly instance which contains list of 
        /// assembled sequences.</returns>
        IDeNovoAssembly Assemble(IEnumerable<ISequence> inputSequences);
    }
}
