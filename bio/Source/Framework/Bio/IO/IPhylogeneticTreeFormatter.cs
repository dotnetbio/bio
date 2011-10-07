using System.IO;
using Bio.Phylogenetics;

namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface write a PhylogeneticTree to a particular location, 
    /// usually a file. The output is formatted according to the particular file format. 
    /// A method is also provided for quickly accessing the content in string form for 
    /// applications that do not need to first write to file.
    /// </summary>
    public interface IPhylogeneticTreeFormatter
    {
        /// <summary>
        /// Writes a PhylogeneticTree to the location specified by the writer.
        /// </summary>
        /// <param name="tree">PhylogeneticTree to format.</param>
        /// <param name="writer">The TextWriter used to write the formatted Phylogenetic Tree text.</param>
        void Format(Tree tree, TextWriter writer);

        /// <summary>
        /// Writes a PhylogeneticTree to the specified file.
        /// </summary>
        /// <param name="tree">PhylogeneticTree to format.</param>
        /// <param name="fileName">The name of the file to write the formatted Phylogenetic Tree text.</param>
        void Format(Tree tree, string fileName);
        
        /// <summary>
        /// Converts a PhylogeneticTree to a formatted string.
        /// </summary>
        /// <param name="tree">PhylogeneticTree to format.</param>
        /// <returns>A string of the formatted text.</returns>
        string FormatString(Tree tree);

        /// <summary>
        /// Gets the name of the Phylogenetic tree being
        /// implemented. This is intended to give the
        /// developer some information of the formatter type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the Phylogenetic tree formatter being
        /// implemented. This is intended to give the
        /// developer some information of the formatter.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the file extensions that the formatter implementation
        /// will support.
        /// </summary>
        string FileTypes { get; }
    }
}
