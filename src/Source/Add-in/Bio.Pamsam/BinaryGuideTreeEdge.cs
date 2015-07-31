using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of Edge class in Binary Guide Tree.
    /// 
    /// An edge connects two Binary Guide Tree nodes: parent and child.
    /// 
    /// The length of the edge represents the evolutionary distance between 
    /// the two nodes it connects.
    /// </summary>
    public class BinaryGuideTreeEdge
    {
        #region Fields

        // The Edge ID
        private int _id; 

        // The length of edge reflects the evolutionary distance between the two nodes
        private float _length;
        #endregion

        #region Properties

        /// <summary>
        /// The ID of this edge class
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The length of this edge class
        /// </summary>
        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }

        /// <summary>
        /// The parent node connecting to this edge
        /// </summary>
        public BinaryGuideTreeNode ParentNode { get; set; }

        /// <summary>
        /// The child node connecting to the edge
        /// </summary>
        public BinaryGuideTreeNode ChildNode { get; set; }
        
        #endregion

        #region Constructors
        /// <summary>
        /// Construct an empty edge.
        /// </summary>
        public BinaryGuideTreeEdge()
        {
            _id = -1;
            ParentNode = null;
            ChildNode = null;
        }

        /// <summary>
        /// Construct an edge with an assigned ID.
        /// </summary>
        /// <param name="id">zero-based edge ID</param>
        public BinaryGuideTreeEdge(int id)
        {
            _id = id;
            ParentNode = null;
            ChildNode = null;
        }
        #endregion
    }
}
