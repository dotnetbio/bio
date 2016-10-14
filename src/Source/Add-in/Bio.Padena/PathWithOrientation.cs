using System;
using System.Collections.Generic;
using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Structure that stores list of nodes in path, 
    /// along with path orientation.
    /// </summary>
    internal class PathWithOrientation
    {
        /// <summary>
        /// Flag to indicate if this path is "Active" or still being extended.
        /// </summary>
        internal bool EndReached;

        /// <summary>
        /// List of nodes in path.
        /// </summary>
        internal List<DeBruijnNode> Nodes;

        /// <summary>
        /// Initializes a new instance of the PathWithOrientation class.
        /// </summary>
        /// <param name="node1">First node to add.</param>
        /// <param name="node2">Second node to add.</param>
        /// <param name="orientation">Path orientation.</param>
        internal PathWithOrientation(DeBruijnNode node1, DeBruijnNode node2, bool orientation)
        {
            if (node1 == null)
            {
                throw new ArgumentNullException("node1");
            }

            if (node2 == null)
            {
                throw new ArgumentNullException("node2");
            }

            this.Nodes = new List<DeBruijnNode> { node1, node2 };
            this.GrabNextNodesOnLeft = orientation;
            this.EndReached = false;
        }

        /// <summary>
        /// Indicates if at the end of the path, the next nodes should come from the
        /// left or right extensions
        /// </summary>
        internal bool GrabNextNodesOnLeft;
    }
}
