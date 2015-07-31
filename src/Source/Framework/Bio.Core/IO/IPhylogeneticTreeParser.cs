using System.IO;
using Bio.Phylogenetics;

namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface are designed to parse a phylogenetic tree file 
    /// format to produce a PhylogeneticTree object model. 
    /// </summary>
    public interface IPhylogeneticTreeParser : IParser
    {
        /// <summary>
        /// Name for the tree
        /// </summary>
        string TreeName { get; set; }

        /// <summary>
        /// Parses a phylogenetic tree text from a stream.
        /// </summary>
        /// <param name="stream">A stream for a phylogenetic tree text, it will be left open.</param>
        /// <returns>Phylogenetic tree object.</returns>
        Tree Parse(Stream stream);
    }
}
