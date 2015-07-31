using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of Binary guide tree.
    /// 
    /// The tree guides progressive alignment algorithm to align sequences from bottom up.
    /// The tree structure reflects the distances between sequences, i.e. the distance on 
    /// the tree of two leaf nodes is proportional to the evolutionary distance of the two
    /// sequences associated to the nodes; the distance between two internal nodes is 
    /// proportional to the distance between two sets of sequences of their subtrees.
    /// 
    /// In the tree, leaves are associated with sequences, and internal nodes, including
    /// root, are associated with aligned sequences. All nodes are also associated with
    /// profile matrices of its subtree sequences. 
    /// 
    /// The tree is constructed by hierarcical clustering method so that it is a binary tree.
    /// First all leaf nodes are created by connecting to input sequences. Then internal nodes
    /// are created by clustering two leaf nodes, or two internal nodes, or a mixture of one 
    /// leaf and one internal nodes. When all the nodes are connected by the root node, the 
    /// tree construction is done.
    /// 
    /// Progressive alignment also follows a prefix order (bottom up, children before parents).
    /// When aligning two children of a node, the aligned sequences are represented by its 
    /// profile, a multiple alignment treated as a sequence by regarding each column as an 
    /// alignable symbol. The profile is then assigned to their parent node. Once the root 
    /// of the tree is aligned, a multiple sequence alignment (MSA) is generated.
    /// 
    /// The class also supports a set of operations on the tree, i.e. cuts a tree into two 
    /// subtrees, and extract the subtree nodes/leaves; compares a newly generated tree
    /// with a previous one and marks the nodes that are different so that re-alignment is 
    /// required on the marked nodes; separates the leaves by fakely cutting an edge, etc.
    /// </summary>
    public class BinaryGuideTree
    {
        #region Fields

        // Tree ID
        private int _id;

        // The number of nodes in the tree
        private int _numberOfNodes;

        // The number of leaves in the tree
        private int _numberOfLeaves;

        // The height of the tree
        private int _height;

        // The root node of the tree. There should be only one root node for BinaryGuideTree.
        private BinaryGuideTreeNode _root;

        // The node list in the tree
        // The order is the align order of progressive alignment
        private List<BinaryGuideTreeNode> _nodes;

        // The edge list in the tree.
        private List<BinaryGuideTreeEdge> _edges;
        #endregion

        #region Properties

        /// <summary>
        /// The ID of this tree.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        /// <summary>
        /// The number of nodes in this tree
        /// </summary>
        public int NumberOfNodes
        {
            get { return _numberOfNodes; }
            set { _numberOfNodes = value; }
        }

        /// <summary>
        /// The number of leaves in this tree
        /// </summary>
        public int NumberOfLeaves
        {
            get { return _numberOfLeaves; }
            set { _numberOfLeaves = value; }
        }

        /// <summary>
        /// The number of edges in this tree
        /// </summary>
        public int NumberOfEdges
        {
            get { return _edges.Count; }
        }

        /// <summary>
        /// The height of this tree
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// The root of this tree
        /// </summary>
        public BinaryGuideTreeNode Root
        {
            get { return _root; }
        }

        /// <summary>
        /// The node list of this tree
        /// </summary>
        public List<BinaryGuideTreeNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }

        /// <summary>
        /// The edge list of this tree
        /// </summary>
        public List<BinaryGuideTreeEdge> Edges
        {
            get { return _edges; }
            set { _edges = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a tree with one root node
        /// </summary>
        public BinaryGuideTree()
        {
            _root = new BinaryGuideTreeNode();
            _numberOfNodes = 1;
            _numberOfLeaves = 1;
        }

        /// <summary>
        /// Create a tree with an assigned tree ID
        /// </summary>
        /// <param name="id">zero-based tree ID</param>
        public BinaryGuideTree(int id)
        {
            _id = id;
            _root = new BinaryGuideTreeNode();
            _numberOfNodes = 1;
            _numberOfLeaves = 1;
        }

        /// <summary>
        /// Create a tree using an existing node as root
        /// </summary>
        /// <param name="r">a BinaryGuideTreeNode</param>
        public BinaryGuideTree(BinaryGuideTreeNode r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r");
            }

            _id = r.ID;
            _root = r;
            _numberOfNodes = 1;
            _numberOfLeaves = 1;
        }

        /// <summary>
        /// Construct a tree by hierarchical clustering method.
        /// 
        /// The node list is already generated in the hierarchical clustering method
        /// and the root will be the last node in the list
        /// </summary>
        /// <param name="hCluster">hierarcical clustering class object</param>
        public BinaryGuideTree(IHierarchicalClustering hCluster)
        {
            if (hCluster == null)
            {
                throw new ArgumentException("null Hierarchical clustering class");
            }
            if (hCluster.Nodes.Count == 0)
            {
                throw new ArgumentException("empty node list in Hierarchical clustering class");
            }
            _nodes = hCluster.Nodes;
            _edges = hCluster.Edges;
            _root = hCluster.Nodes[hCluster.Nodes.Count - 1];
            _numberOfNodes = hCluster.Nodes.Count;
            _numberOfLeaves = (_numberOfNodes + 2) / 2;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Use the input node as root, return the subtree nodes of this node, and keep them
        /// in the alignment order by progressive aligner.
        /// </summary>
        /// <param name="node">root node in a (sub)tree</param>
        public List<BinaryGuideTreeNode> ExtractSubTreeNodes(BinaryGuideTreeNode node)
        {
            // mark whether the node is descendent of this node
            List<bool> mark = new List<bool>(_numberOfNodes);
            for (int i = 0; i < _numberOfNodes; ++i)
            {
                mark.Add(false);
            }

            // Mark the subtree nodes by depth first search algorithm
            DepthFirstSearch(node, mark);

            List<BinaryGuideTreeNode> result = new List<BinaryGuideTreeNode>();
            for (int i = 0; i < mark.Count; ++i)
            {
                if (mark[i])
                {
                    result.Add(_nodes[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Use the input node as root, return the subtree *leaf* nodes of this node, 
        /// and keep them in the alignment order by progressive aligner.
        /// </summary>
        /// <param name="node">root node in a (sub)tree</param>
        public List<BinaryGuideTreeNode> ExtractSubTreeLeafNodes(BinaryGuideTreeNode node)
        {
            // mark whether the node is descendent of this node
            List<bool> mark = new List<bool>(_numberOfNodes);
            for (int i = 0; i < _numberOfNodes; ++i)
            {
                mark.Add(false);
            }

            // Mark the subtree nodes by depth first search algorithm
            DepthFirstSearch(node, mark);

            List<BinaryGuideTreeNode> result = new List<BinaryGuideTreeNode>();
            for (int i = 0; i < mark.Count; ++i)
            {
                if (mark[i] && _nodes[i].IsLeaf)
                {
                    result.Add(_nodes[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Depth first search (DFS) algorithm (recursive version).
        /// Visit the subtree nodes from the current node, and
        /// mark visited nodes by DFS with 'true'.
        /// </summary>
        /// <param name="node">root node of the subtree</param>
        /// <param name="mark">Bool list of all nodes (in a tree)</param>
        public static void DepthFirstSearch(BinaryGuideTreeNode node, List<bool> mark)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            mark[node.ID] = true;
            while (node.LeftChildren != null)
            {
                if (mark[node.LeftChildren.ID] == false)
                {
                    DepthFirstSearch(node.LeftChildren, mark);
                }
                else
                {
                    break;
                }
            }

            while (node.RightChildren != null)
            {
                if (mark[node.RightChildren.ID] == false)
                {
                    DepthFirstSearch(node.RightChildren, mark);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Compare two (sub)trees from root to leaves,
        /// find the top node position that the subtrees of this node are
        /// different between the two trees
        /// 
        /// Normally nodeA is the root of newly generated tree A, and nodeB is the root
        /// of old tree B. This method returns the top node in tree A, so that the subtree
        /// of this node will be re-aligned.
        /// </summary>
        /// <param name="nodeA">root of (sub)tree</param>
        /// <param name="nodeB">root of (sub)tree</param>
        public static BinaryGuideTreeNode FindSmallestTreeDifference(BinaryGuideTreeNode nodeA, BinaryGuideTreeNode nodeB)
        {
            if (nodeA == null)
            {
                throw new ArgumentNullException("nodeA");
            }

            if (nodeB == null)
            {
                throw new ArgumentNullException("nodeB");
            }

            //TODO the order of left/right child nodes do not have to be the same to be identical
            if (nodeA != nodeB)
            {
                return nodeA;
            }
            else
            {
                while (nodeA.LeftChildren != null && nodeB.LeftChildren != null)
                {
                    return FindSmallestTreeDifference(nodeA.LeftChildren, nodeB.LeftChildren);
                }
                while (nodeA.RightChildren != null && nodeB.RightChildren != null)
                {
                    return FindSmallestTreeDifference(nodeA.RightChildren, nodeB.RightChildren);
                }
            }
            return null;
        }

        /// <summary>
        /// Compare two guide (sub)trees and mark the nodes that need to be re-aligned.
        /// 
        /// The algorithm traverses tree A in prefix order (children before parents), 
        /// assigning internal nodes ids N+1 through 2N-1 in the order visited. When visiting 
        /// an internal node, if any child node needs to be re-aligned, the node needs to
        /// be re-aligned too. If the two children are both unmarked, and the two children nodes 
        /// are also having the same parent in tree B, this internal node does not need to be
        /// re-aligned, and be assigned an ID the same as the parent node in tree B.
        /// </summary>
        /// <param name="treeA">binary guide (sub)tree</param>
        /// <param name="treeB">binary guide (sub)tree</param>
        public static void CompareTwoTrees(BinaryGuideTree treeA, BinaryGuideTree treeB)
        {
            if (treeA == null)
            {
                throw new ArgumentNullException("treeA");
            }

            if (treeB == null)
            {
                throw new ArgumentNullException("treeB");
            }

            if (treeA.NumberOfNodes != treeB.NumberOfNodes || treeA.NumberOfLeaves != treeB.NumberOfLeaves)
            {
                throw new ArgumentException("The two trees are not comparable");
            }

            Dictionary<int, int> nodeID2ListIndex = new Dictionary<int, int>(treeB.NumberOfNodes);
            for (int i = 0; i < treeB.NumberOfNodes; ++i)
            {
                nodeID2ListIndex[treeB.Nodes[i].ID] = i;
            }

            BinaryGuideTreeNode node, nodeB;

            for (int i = treeA.NumberOfLeaves; i < treeA.NumberOfNodes; ++i)
            {
                node = treeA.Nodes[i];
                if (node.LeftChildren.NeedReAlignment == true || node.RightChildren.NeedReAlignment == true)
                {
                    node.NeedReAlignment = true;
                }
                else
                {
                    if (!nodeID2ListIndex.ContainsKey(node.LeftChildren.ID) || !nodeID2ListIndex.ContainsKey(node.RightChildren.ID))
                    {
                        node.NeedReAlignment = true;
                    }
                    else
                    {
                        nodeB = treeB.Nodes[nodeID2ListIndex[node.LeftChildren.ID]].Parent;
                        try
                        {
                            if (nodeB.LeftChildren.ID == node.RightChildren.ID || nodeB.RightChildren.ID == node.RightChildren.ID)
                            {
                                node.NeedReAlignment = false;
                                node.ID = nodeB.ID;
                            }
                            else
                            {
                                node.NeedReAlignment = true;
                            }
                        }
                        catch (NullReferenceException)
                        {
                            node.NeedReAlignment = true;
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// Cut a tree at an edge to generate 2 subtrees
        /// </summary>
        /// <param name="edgeIndex">zero-based edge index</param>
        /// <returns>return[0] is the subtree with the same root as the original tree;
        ///          return[1] is the subtree rooted below the cutting edge</returns>
        public BinaryGuideTree[] CutTree(int edgeIndex)
        {
            if (edgeIndex < 0 || edgeIndex >= _edges.Count)
            {
                throw new ArgumentException(string.Format("The edge ID provided when cutting the binary tree was not available. Given edge ID: {0}, available edges: {1}", edgeIndex, _edges.Count));
            }
            if (_edges[edgeIndex].ChildNode == null)
            {
                throw new Exception("The edge specified was not properly extended to a child node.Edge ID: " + edgeIndex);
            }

            _edges[edgeIndex].ChildNode.Parent = null;

            if (_edges[edgeIndex].ParentNode.LeftChildren.ID == _edges[edgeIndex].ChildNode.ID)
            {
                _edges[edgeIndex].ParentNode.LeftChildren = null;
            }
            else
            {
                _edges[edgeIndex].ParentNode.RightChildren = null;
            }

            // generate two new trees
            BinaryGuideTree treeA = new BinaryGuideTree(_root);
            BinaryGuideTree treeB = new BinaryGuideTree(_edges[edgeIndex].ChildNode);

            treeA.NumberOfNodes = _numberOfNodes;
            treeB.NumberOfNodes = _numberOfNodes;
            treeA.NumberOfLeaves = _numberOfLeaves;
            treeB.NumberOfLeaves = _numberOfLeaves;

            treeA.Nodes = _nodes;
            treeA.Edges = _edges;
            treeB.Nodes = _nodes;
            treeB.Edges = _edges;

            // pull the subtree nodes out for the two new roots
            treeA.Nodes = (List<BinaryGuideTreeNode>)ExtractSubTreeNodes(treeA.Root);
            treeB.Nodes = (List<BinaryGuideTreeNode>)ExtractSubTreeNodes(treeB.Root);

            treeA.NumberOfNodes = treeA.Nodes.Count;
            treeB.NumberOfNodes = treeB.Nodes.Count;
            treeA.NumberOfLeaves = 0;
            treeB.NumberOfLeaves = 0;
            for (int i = 0; i < treeA.Nodes.Count; ++i)
            {
                if (treeA.Nodes[i].IsLeaf)
                {
                    ++treeA.NumberOfLeaves;
                }
            }
            for (int i = 0; i < treeB.Nodes.Count; ++i)
            {
                if (treeB.Nodes[i].IsLeaf)
                {
                    ++treeB.NumberOfLeaves;
                }
            }

            return new BinaryGuideTree[2] { treeA, treeB };
        }


        /// <summary>
        /// Separate the leaf nodes into two subsets by cutting an edge.
        /// 
        /// Takes the child node of the edge as the root, and extracts the
        /// leaf nodes of the subtree. The rest leaf nodes are the compensate
        /// leaf nodes set.
        /// 
        /// The method is used in alignment refinement, when separating the
        /// sequences into two sets by cutting an edge of the tree. The two
        /// sets of sequences are re-aligned separately and combined to produce
        /// a new multiple alignment.
        /// </summary>
        /// <param name="edgeIndex">zero-based edge index</param>
        /// <returns>return[0] is the indices of leaves in the first subtree rooted below the edge
        ///          return[1] is the indices of leaves in the second subtree with the same root as the original tree</returns>
        public List<int>[] SeparateSequencesByCuttingTree(int edgeIndex)
        {
            if (edgeIndex < 0 || edgeIndex >= _edges.Count)
            {
                throw new ArgumentException(string.Format("The edge ID provided when cutting the binary tree was not available. Given edge ID: {0}, available edges: {1}", edgeIndex, _edges.Count));
            }
            if (_edges[edgeIndex].ChildNode == null)
            {
                throw new ArgumentException("The edge specified was not properly extended to a child node.Edge ID: " + edgeIndex);
            }

            // pull the subtree nodes out for the two new roots
            List<BinaryGuideTreeNode> NodesA = (List<BinaryGuideTreeNode>)ExtractSubTreeLeafNodes(_edges[edgeIndex].ChildNode);

            List<int> resultA = new List<int>(NodesA.Count);
            List<int> resultB = new List<int>(_numberOfLeaves - NodesA.Count);

            for (int i = 0; i < NodesA.Count; ++i)
            {
                resultA.Add(NodesA[i].SequenceID);
            }
            for (int i = 0; i < _numberOfLeaves; ++i)
            {
                if (resultA.IndexOf(i) < 0)
                {
                    resultB.Add(i);
                }
            }
            return new List<int>[2] { resultA, resultB };
        }
        #endregion
    }
}
