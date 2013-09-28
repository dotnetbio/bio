using System;
using System.Collections;
using System.Collections.Generic;

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
        /// Metadata dictionary
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Dictionary<string, string> MetaData { get; set; }
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
