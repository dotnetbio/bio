using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bio.Phylogenetics
{
    /// <summary>
    ///     Tree: The full input Newick Format for a single tree
    ///     Tree --> Subtree ";" | Branch ";"
    /// </summary>
    public class Tree
    {
        /// <summary>
        ///     Name of the tree
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Metadata dictionary
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        ///     Root of the tree
        /// </summary>
        public Node Root { get; set; }

        /// <summary>
        ///     Clone object
        /// </summary>
        /// <returns>Tree as object</returns>
        public Tree Clone()
        {
            return (Tree)this.MemberwiseClone();
        }
    }
}