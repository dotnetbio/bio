using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Node class of Binary Guide Tree.
    /// 
    /// The leaf nodes are associated with sequences;
    /// the internal nodes are associated with the set of aligned sequences of 
    /// its subtree.
    /// 
    /// All the nodes are associated with a profile matrix of its sequence(s).
    /// 
    /// The root node is the alignment of all sequences.
    /// </summary>
    public class BinaryGuideTreeNode
    {
        #region Fields

        // Zero-based node ID
        private int _id;

        // Sequence ID:
        // for leave node, sequence ID is the index of sequence it represent;
        // for internal node, sequence ID equals to the left most leaf sequence 
        // ID of its subtree
        private int _sequenceID;

        // Defined in MUSCLE (Edgar 2004) paper.
        // eString stores the operation of the child node sequence to become
        // the aligned sequence in its parent node.
        // 
        // eString is a vector of integers: positive integer n means 
        // skip n letters; negative integer -n means insert n indels 
        // at the current position.
        // 
        // The alignment path of sequence (leaf node) is the series of 
        // eString through internal nodes from this leaf to root (including
        // leaf node).
        // 
        // With eString, the operation on sequences is avoided until
        // progressive alignment is finished, then aligned sequences will be 
        // produced by applying eStrings to the sequences.
        private List<int> _eString = null;


        // The weight of sequences assigned of this node
        private float _weight;

        // The edge that link this node to its parent
        private BinaryGuideTreeEdge _parentEdge = null;
        #endregion

        #region Properties

        /// <summary>
        /// The ID of this node
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The ID of sequence connected to this node
        /// </summary>
        public int SequenceID
        {
            get { return _sequenceID; }
            set { _sequenceID = value; }
        }

        /// <summary>
        /// The EString (alignment path) on this node
        /// </summary>
        public List<int> EString 
        { 
            get { return _eString; }
            set { _eString = value; }
        }

        /// <summary>
        /// The left child node of the current node
        /// </summary>
        public BinaryGuideTreeNode LeftChildren
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The right child node of the current node
        /// </summary>
        public BinaryGuideTreeNode RightChildren
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The parent node of the current node
        /// </summary>
        public BinaryGuideTreeNode Parent 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The profile associated with the current node.
        /// Internal node is associated with an aligned profile from its two chilrend's profiles.
        /// Leaf node is associated with a profile converted from a single sequence.
        /// </summary>
        public IProfileAlignment ProfileAlignment 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Check whether this node is root in the tree
        /// </summary>
        public bool IsRoot
        {
            get { return Parent == null; }
        }

        /// <summary>
        /// Check whether this node is a leaf in the tree
        /// </summary>
        public bool IsLeaf
        {
            get { return LeftChildren == null && RightChildren == null; }
        }

        /// <summary>
        /// It is false if ProfileAlignment is already generated, and in the tree comparison,
        /// the left and right subtrees are the same as in old tree. Then the alignment of this 
        /// node can be ignored.
        /// </summary>
        public bool NeedReAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// The weight of sequences assigned of this node
        /// </summary>
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// The edge that link this node to its parent
        /// </summary>
        public BinaryGuideTreeEdge ParentEdge
        {
            get { return _parentEdge; }
            set { _parentEdge = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct an empty node
        /// </summary>
        public BinaryGuideTreeNode()
        {
            LeftChildren = null;
            RightChildren = null;
            Parent = null;
            ProfileAlignment = new ProfileAlignment();
            _id = -1;
            _sequenceID = -1;
            _eString = new List<int>();
            NeedReAlignment = true;
        }

        /// <summary>
        /// Construct a node with assigned ID
        /// </summary>
        /// <param name="id">zero-based node ID</param>
        public BinaryGuideTreeNode(int id)
        {
            LeftChildren = null;
            RightChildren = null;
            Parent = null;
            ProfileAlignment = new ProfileAlignment();
            _id = id;
            _sequenceID = id;
            _eString = new List<int>();
            NeedReAlignment = true;
        }
        #endregion
    }
}
