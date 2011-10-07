using System.Collections.Generic;
using System;

namespace Bio.Phylogenetics
{
    /// <summary>
    /// Edge: a tree edge and its descendant subtree.
    /// Edge --> Distance/Length
    /// </summary>
    public class Edge : ICloneable
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Edge()
        {
            //default distance is set to 0
            Distance = 0;
        }
        #endregion Constructors

        #region -- Properties --
        /// <summary>
        /// Length of a tree edge.
        /// </summary>
        public double Distance { set; get; }
        #endregion -- Properties --

        #region -- Methods --
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Edge as object</returns>
        public object Clone()
        {
            Edge newEdge = (Edge)this.MemberwiseClone();
            return newEdge;
        }
        #endregion -- Methods --
    }

}
