using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Phylogenetics
{
    /// <summary>
    /// Node : Node of the tree which can be either Leaf or another branch node.
    /// </summary>
    public class Node :ICloneable
    {
        #region -- Member Variables --
        private Dictionary<Node, Edge> children;
        #endregion  -- Member Variables --

        #region -- Constructors --
        /// <summary>
        /// Default constructor
        /// </summary>
        public Node()
        {
            children= new Dictionary<Node,Edge>();
        }
        #endregion -- Constructors --

        #region -- Properties --
        /// <summary>
        /// Metadata encoded as string
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Dictionary<string, string> MetaData {get;set;}

        /// <summary>
        /// Get Childern nodes
        /// </summary>
        public Dictionary<Node, Edge> Children 
        { 
            get 
            { 
                return children; 
            } 
        }

        /// <summary>
        /// Get list of Nodes
        /// </summary>
        public IList<Node> Nodes 
        {
            get 
            { 
                return children.Keys.ToList(); 
            }
        }

        /// <summary>
        /// Get list of Edges
        /// </summary>
        public IList<Edge> Edges 
        { 
            get 
            { 
                return children.Values.ToList(); 
            } 
        }

        /// <summary>
        /// Either node is leaf or not
        /// </summary>
        public bool IsLeaf 
        { 
            get 
            {
                return Children.Keys.Count == 0; 
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
        #endregion -- Properties --

        #region -- Methods --
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Node as object</returns>
        public object Clone()
        {
            Node newNode = (Node)this.MemberwiseClone();
            return newNode;
        }
        #endregion -- Methods --
    }
}
