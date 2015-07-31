using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Representation of any sequence assembly algorithm.
    /// This interface defines contract for classes implementing 
    /// De Bruijn graph based De Novo Sequence assembler.
    /// </summary>
    public interface IDeBruijnDeNovoAssembler : IDeNovoAssembler
    {
        /// <summary>
        /// Gets or sets the kmer length.
        /// </summary>
        int KmerLength { get; set; }

        /// <summary>
        /// Gets the assembler de-bruijn graph.
        /// </summary>
        DeBruijnGraph Graph { get;  }
    }
}
