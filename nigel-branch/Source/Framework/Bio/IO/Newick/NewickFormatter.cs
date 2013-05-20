using System;
using System.IO;
using System.Text;
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
    /// Documentation for the latest Newic file format can be found at
    /// http://evolution.genetics.washington.edu/phylip/newicktree.html
    /// http://en.wikipedia.org/wiki/Newick_format
    /// </summary>
    public class NewickFormatter : IPhylogeneticTreeFormatter
    {
        #region -- Member Variables --
        private Tree _tree;
        #endregion -- Member Variables --
        
        #region -- Constructors --
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NewickFormatter()
            : base()
        {
        }

        #endregion -- Constructors --

        #region -- Public Methods -- 
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
        /// NewickFormatter class acheives.
        /// </summary>
        public  string Description
        {
            get
            {
                return Resource.NEWICK_FORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma seperated values of the possible
        /// file extensions for a newick file.
        /// </summary>
        public  string FileTypes
        {
            get
            {
                return Resource.NEWICK_FILE_EXTENSION;
            }
        }
        
        /// <summary>
        /// Writes a Phylogenetic tree to the specified file.
        /// </summary>
        /// <param name="tree">PhylogeneticTree to format.</param>
        /// <param name="fileName">The name of the file to write the formatted Phylogenetic tree text.</param>
        public void Format(Tree tree, string fileName)
        {
            Validate(tree);
            using (TextWriter writer = new StreamWriter(fileName))
            {
                Format(writer);
            }
        }

        /// <summary>
        /// Writes a PhylogeneticTree to the writer.
        /// </summary>
        /// <param name="tree">PhylogeneticTree to format.</param>
        /// <param name="writer">The TextWriter used to write the formatted Phylogenetic tree text.</param>
        public void Format(Tree tree, TextWriter writer)
        {
            Validate(tree);
            Format(writer);
        }
        
        /// <summary>
        /// Converts a PhylogeneticTree to a formatted text.
        /// </summary>
        /// <param name="tree">PhylogeneticTree to format.</param> 
        /// <returns>A string of the formatted Phylogenetic tree text.</returns>
        public string FormatString(Tree tree)
        {
            Validate(tree);
            using (TextWriter writer = new StringWriter())
            {
                Format(writer);
                return writer.ToString();
            }
        }
        #endregion -- Public Methods --

        #region -- Private Methods --
        /// <summary>
        /// Basic PhylogeneticTree object validation
        /// </summary>
        /// <param name="tree">PhylogeneticTree object</param>
        private void Validate(Tree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(string.Format
                    (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                    "Tree object is null"));
            }
            _tree = tree;
        }
        
        /// <summary>
        /// Writes a PhylogeneticTree to a Newick format in the writer.
        /// </summary>
        /// <param name="writer">The TextWriter used to write the formatted Phylogenetic tree text.</param>
        private void Format(TextWriter writer)
        {
            StringBuilder stringBuilder = new StringBuilder();

            Format(_tree.Root, null, ref stringBuilder);
            // append end char ";"
            stringBuilder.Append(";");
            writer.Write(stringBuilder.ToString());
            writer.Flush();
            writer.Close();
        }
        
        /// <summary>
        /// Recursive method to get each node into string
        /// </summary>
        /// <param name="node">tree node</param>
        /// <param name="edge">edge</param>
        /// <param name="stringBuilder">output newick string</param>
        private void Format(Node node, Edge edge, ref StringBuilder stringBuilder)
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
                    Format(localNode, localEdge, ref stringBuilder);
                    firstNode = false;
                }

                //last node
                if (null == edge)
                {
                    stringBuilder.Append(")");
                }
                else
                {
                    stringBuilder.Append(string.Format("):{0}", edge.Distance));
                }
            }
        }
        #endregion -- Private Methods --
    }
}
