using System;
using System.IO;
using System.Text;

using Bio.Extensions;
using Bio.Phylogenetics;
using Bio.Properties;
using System.Globalization;
using System.Collections.Generic;

namespace Bio.IO.Newick
{
    /// <summary>
    /// Formats a PhylogeneticTree object into newick text (usually a file). 
    /// The output is formatted according to the Newick format. A method is 
    /// also provided for quickly accessing the content in string form 
    /// for applications that do not need to first write to file.
    /// Documentation for the latest Newick file format can be found at
    /// http://evolution.genetics.washington.edu/phylip/newicktree.html
    /// http://en.wikipedia.org/wiki/Newick_format
    /// </summary>
    public class NewickFormatter : IPhylogeneticTreeFormatter
    {
        /// <summary>
        /// Gets the type of Formatter i.e NEWICK.
        /// This is intended to give developers some information 
        /// of the formatter class.
        /// </summary>
        public  string Name
        {
            get
            {
                return Resource.NEWICK_NAME;
            }
        }

        /// <summary>
        /// Gets the description of Newick formatter.
        /// This is intended to give developers some information 
        /// of the formatter class. This property returns a simple description of what the
        /// NewickFormatter class achieves.
        /// </summary>
        public  string Description
        {
            get
            {
                return Resource.NEWICK_FORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma separated values of the possible
        /// file extensions for a newick file.
        /// </summary>
        public  string SupportedFileTypes
        {
            get
            {
                return Resource.NEWICK_FILE_EXTENSION;
            }
        }

        /// <summary>
        /// Writes a PhylogeneticTree to the writer.
        /// </summary>
        /// <param name="stream">The Stream used to write the formatted Phylogenetic tree text, it will remain open.</param>
        /// <param name="tree">PhylogeneticTree to format.</param>
        public void Format(Stream stream, Tree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(string.Format
                    (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                    "Tree object is null"));
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var stringBuilder = new StringBuilder();
            this.Write(tree.Root, null, ref stringBuilder);

            // Append end char ";"
            stringBuilder.Append(";");

            using (var writer = stream.OpenWrite())
            {
                writer.Write(stringBuilder.ToString());
                writer.Flush();
            }
        }
        
        /// <summary>
        /// Recursive method to get each node into string
        /// </summary>
        /// <param name="node">tree node</param>
        /// <param name="edge">edge</param>
        /// <param name="stringBuilder">output newick string</param>
        void Write(Node node, Edge edge, ref StringBuilder stringBuilder)
        {
            if (node.IsLeaf)
            {
                stringBuilder.Append(string.Format("{0}:{1}", node.Name, edge.Distance));
            }
            else
            {
                
                stringBuilder.Append("(");
                bool firstNode = true;
                foreach (KeyValuePair<Node, Edge> child in node.Children)
                {
                    Edge localEdge = child.Value;
                    Node localNode = child.Key;

                    if (localNode == node)
                    {
                        throw new ArgumentException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                            "Tree object has more than one root"));
                    }
                    if (!firstNode)
                    {
                        stringBuilder.Append(",");
                    }
                    this.Write(localNode, localEdge, ref stringBuilder);
                    firstNode = false;
                }

                //last node
                stringBuilder.Append(null == edge ? ")" : string.Format("):{0}", edge.Distance));
            }
        }
    }
}
