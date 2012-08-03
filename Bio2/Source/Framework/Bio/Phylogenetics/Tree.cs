using System;

namespace Bio.Phylogenetics
{
    /// <summary>
    /// Tree: The full input Newick Format for a single tree
    /// Tree --> Subtree ";" | Branch ";"
    /// </summary>
    public class Tree : ICloneable
    {
        #region -- Constructors --
        /// <summary>
        /// default constructor
        /// </summary>
        public Tree()
        {
        }
        #endregion -- Constructors --

        #region -- Properties --
        /// <summary>
        /// Name of the tree
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Root of the tree
        /// </summary>
        public Node Root { get; set; }
        #endregion -- Properties --

        #region -- Methods --
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Tree as object</returns>
        public object Clone()
        {
            Tree newTree = (Tree)this.MemberwiseClone();
            return newTree;
        }
        #endregion -- Methods --
    }

}
