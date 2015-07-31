using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Represents a path in De Bruijn graph.
    /// </summary>
    public class DeBruijnPath
    {
        /// <summary>
        /// List of node in De Bruijn graph path.
        /// </summary>
        private List<DeBruijnNode> path;

        /// <summary>
        /// Initializes a new instance of the DeBruijnPath class.
        /// </summary>
        public DeBruijnPath()
        {
            this.path = new List<DeBruijnNode>();
        }

        /// <summary>
        /// Initializes a new instance of the DeBruijnPath class with specified nodes.
        /// </summary>
        /// <param name="nodes">List of nodes.</param>
        public DeBruijnPath(IEnumerable<DeBruijnNode> nodes)
        {
            this.path = new List<DeBruijnNode>(nodes);
        }

        /// <summary>
        /// Initializes a new instance of the DeBruijnPath class with specified node.
        /// </summary>
        /// <param name="node">Graph node.</param>
        public DeBruijnPath(DeBruijnNode node)
        {
            this.path = new List<DeBruijnNode> { node };
        }

        /// <summary>
        /// Gets the list of nodes in path.
        /// </summary>
        public IList<DeBruijnNode> PathNodes
        {
            get { return this.path; }
        }

        /// <summary>
        /// Removes all nodes from path that match the given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to remove nodes.</param>
        public void RemoveAll(Predicate<DeBruijnNode> predicate)
        {
            this.path.RemoveAll(predicate);
        }
    }
}
