using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bio.Phylogenetics
{
    /// <summary>
    /// Node : Node of the tree which can be either Leaf or another branch node.
    /// </summary>
    public class Node
    {
        private readonly Dictionary<Node, Edge> children;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Node()
        {
            this.children = new Dictionary<Node, Edge>();
        }

        /// <summary>
        /// Metadata encoded as string
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// Get Children nodes
        /// </summary>
        public IDictionary<Node, Edge> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Get list of Nodes
        /// </summary>
        public IList<Node> Nodes
        {
            get
            {
                return this.children.Keys.ToList();
            }
        }

        /// <summary>
        /// Get list of Edges
        /// </summary>
        public IList<Edge> Edges
        {
            get
            {
                return this.children.Values.ToList();
            }
        }

        /// <summary>
        /// Either node is leaf or not
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return this.Children.Keys.Count == 0;
            }
        }

        /// <summary>
        /// Either node is root node or not
        /// </summary>
        public bool IsRoot { set; get; }

        /// <summary>
        /// If its leaf node, then use name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Node as object</returns>
        public Node Clone()
        {
            return (Node)this.MemberwiseClone();
        }
    }
}