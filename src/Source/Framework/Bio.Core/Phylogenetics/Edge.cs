namespace Bio.Phylogenetics
{
    /// <summary>
    /// Edge: a tree edge and its descendant subtree.
    /// Edge --> Distance/Length
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Edge()
        {
            //default distance is set to 0
            Distance = 0;
        }

        /// <summary>
        /// Length of a tree edge.
        /// </summary>
        public double Distance { set; get; }

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Edge as object</returns>
        public Edge Clone()
        {
            return (Edge) this.MemberwiseClone();
        }
    }

}
