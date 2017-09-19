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
    public interface IPhylogeneticTreeFormatter : IFormatter
    {
        /// <summary>
        /// Writes a PhylogeneticTree to the location specified by the writer.
        /// </summary>
        /// <param name="stream">The Stream used to write the formatted Phylogenetic Tree text.</param>
        /// <param name="tree">PhylogeneticTree to format.</param>
        void Format(Stream stream, Tree tree);
    }
}
