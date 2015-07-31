namespace Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph
{
    /// <summary>
    /// Represents an edge in the de bruijn graph.
    /// Stores orientation (same or opposite) and whether the edge is valid.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Initializes a new instance of the DeBruijnEdge class.
        /// </summary>
        /// <param name="isSameOrientation">Orientation of edge.</param>
        /// <param name="isValid">Indicates if this is a valid edge.</param>
        public Edge(bool isSameOrientation, bool isValid)
        {
            this.IsSameOrientation = isSameOrientation;
            this.IsValid = isValid;
        }

        /// <summary>
        /// Initializes a new instance of the DeBruijnEdge class.
        /// Creates a 'valid' edge by default.
        /// </summary>
        /// <param name="orientation">Orientation of edge.</param>
        public Edge(bool orientation) : this(orientation, true) 
        { 
        }

        /// <summary>
        /// Gets or sets a value indicating whether orientation of edge is same or opposite.
        /// If two connected nodes have overlapping sequence, the orientation is 
        /// true (same orientation). If sequence of one overlaps with the reverse-complement 
        /// of other's sequence, the orientation is false (opposite orientation).
        /// </summary>
        public bool IsSameOrientation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a valid edge.
        /// Used to temporarily remove edges from graph.
        /// </summary>
        public bool IsValid { get; set; }
    }
}
