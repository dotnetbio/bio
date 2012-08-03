using System.IO;
using System.Text;
using Bio.Phylogenetics;

namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface are designed to parse a phylogenetic tree file 
    /// format to produce a PhylogeneticTree object model. 
    /// </summary>
    public interface IPhylogeneticTreeParser
    {
        /// <summary>
        /// Parses a phylogenetic tree text from a reader.
        /// </summary>
        /// <param name="reader">A reader for a phylogenetic tree text.</param>
        /// <returns>Phylogenetic tree object.</returns>
        Tree Parse(TextReader reader);

        /// <summary>
        /// Parses a phylogenetic tree text from a string.
        /// </summary>
        /// <param name="treeBuilder">phylogenetic tree text.</param>
        /// <returns>Phylogenetic tree object.</returns>
        Tree Parse(StringBuilder treeBuilder);

        /// <summary>
        /// Parses phylogenetic tree texts from a file.
        /// </summary>
        /// <param name="fileName">The name of a phylogenetic tree file.</param>
        /// <returns>Phylogenetic tree object.</returns>
        Tree Parse(string fileName);

        /// <summary>
        /// Gets the name of the phylogenetic tree parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the phylogenetic tree parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the file extensions that the parser implementation
        /// will support.
        /// </summary>
        string FileTypes { get; }
    }
}
