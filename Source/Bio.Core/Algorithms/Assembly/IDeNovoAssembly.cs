using System.Collections.Generic;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// An IDeNovoAssembly is the result of running De Novo Assembly on a set of two or more sequences. 
    /// </summary>
    public interface IDeNovoAssembly
    {
        /// <summary>
        /// Gets list of sequences created after Assembly.
        /// </summary>
        IList<ISequence> AssembledSequences { get; }

        /// <summary>
        /// Gets or sets the Documentation object is intended for tracking the history, provenance,
        /// and experimental context of a IDeNovoAssembly. The user can adopt any desired
        /// convention for use of this object.
        /// </summary>
        object Documentation { get; set; }
    }
}
