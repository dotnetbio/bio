using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph
{
    /// <summary>
    /// Represents a node in the Overlap graph
    /// A node is associated with a contig. 
    /// Also holds adjacency information with other nodes.
    /// </summary>
    public class Node
    {
        #region Fields

        /// <summary>
        /// Holds sequence index of contig.
        /// </summary>
        private int sequenceIndex;

        /// <summary>
        /// Length of contig associated with the node.
        /// </summary>
        private long contigLength;

        /// <summary>
        /// Right Extension edges. Edge contains connecting node, and orientation of edge. 
        /// A right-end extension edge will be added from node A to node B, if there is an 
        /// overlap of length (k-1) between right end of sequence A and left end of sequences B. 
        /// Orientation is same, if overlapping sequences in adjacent nodes 
        /// are normal orientation. Orientation is opposite, if one of the 
        /// sequences is reverse complement.
        /// </summary>
        private Dictionary<Node, Edge> rightEndExtensionNodes;

        /// <summary>
        /// Left Extension edges. Edge contains connecting node, and orientation of edge. 
        /// A left-end extension edge will be added from node A to node B, if there is an 
        /// overlap of length (k-1) between left end of sequence A and right end of sequences B. 
        /// Orientation is same, if overlapping sequences in adjacent nodes 
        /// are normal orientation. Orientation is opposite, if one of the 
        /// sequences is reverse complement.
        /// </summary>
        private Dictionary<Node, Edge> leftEndExtensionNodes;

        /// <summary>
        /// Depicts that whether the node is marked or not.
        /// </summary>
        private bool isMarked = false;

        /// <summary>
        /// Coverage of contig. (No. of reads aligned to contig)
        /// </summary>
        private int coverage = 0;
        
        #endregion

        #region Constructors and Properties

        /// <summary>
        /// Initializes a new instance of the Node class.
        /// Creates graph node with sequence index.
        /// </summary>
        /// <param name="length">Length of contig.</param>
        /// <param name="sequenceIndex">Sequence Index for contig.</param>
        public Node(long length, int sequenceIndex)
        {
            if (length <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            this.sequenceIndex = sequenceIndex;
            this.contigLength = length;
            this.rightEndExtensionNodes = new Dictionary<Node, Edge>();
            this.leftEndExtensionNodes = new Dictionary<Node, Edge>();
            this.coverage = 1;
        }

        /// <summary>
        /// Gets the right extension edges.
        /// </summary>
        public Dictionary<Node, Edge> RightExtensionNodes
        {
            get
            {
                return this.rightEndExtensionNodes;
            }
        }

        /// <summary>
        /// Gets the left extension edges.
        /// </summary>
        public Dictionary<Node, Edge> LeftExtensionNodes
        {
            get
            {
                return this.leftEndExtensionNodes;
            }
        }

        /// <summary>
        /// Gets the length of contig.
        /// </summary>
        public long ContigLength
        {
            get { return this.contigLength; }
        }

        /// <summary>
        /// Gets index of source sequence for contig.
        /// </summary>
        public int SequenceIndex
        {
            get { return this.sequenceIndex; }
        }

        /// <summary>
        /// Coverage of contig. (No. of reads aligned to contig)
        /// </summary>
        public int Coverage
        {
            get { return this.coverage; }

            set { this.coverage = value; }
        }

        /// <summary>
        /// Gets the total number of extension edges for the node.
        /// </summary>
        public int ExtensionsCount
        {
            get { return this.RightExtensionNodes.Count + this.LeftExtensionNodes.Count; }
        }
        
        #endregion

        /// <summary>
        /// Add node with given orientation to left extension edges.
        /// Not thread-safe. Use lock at caller if required.
        /// </summary>
        /// <param name="node">Node to add left-extension to.</param>
        /// <param name="isSameOrientation">Orientation of connecting edge.</param>
        public void AddLeftEndExtension(Node node, bool isSameOrientation)
        {
            ValidateNode(node);
            Edge edge;
            if (this.leftEndExtensionNodes.TryGetValue(node, out edge))
            {
                this.leftEndExtensionNodes[node].IsSameOrientation ^= isSameOrientation;
            }
            else
            {
                this.leftEndExtensionNodes[node] = new Edge(isSameOrientation);
            }
        }

        /// <summary>
        /// Add node with given orientation to right extension edges.
        /// Not thread-safe. Use lock at caller if required.
        /// </summary>
        /// <param name="node">Node to add right-extension to.</param>
        /// <param name="isSameOrientation">Orientation of connecting edge.</param>
        public void AddRightEndExtension(Node node, bool isSameOrientation)
        {
            ValidateNode(node);
            Edge edge;
            if (this.rightEndExtensionNodes.TryGetValue(node, out edge))
            {
                this.rightEndExtensionNodes[node].IsSameOrientation ^= isSameOrientation;
            }
            else
            {
                this.rightEndExtensionNodes[node] = new Edge(isSameOrientation);
            }
        }

        /// <summary>
        /// Mark nodes as visited.
        /// WARNING: DO NOT USE this if you need contig count information.
        /// contig count field is being re-used for this purpose.
        /// Old value of contig count will be over-written.
        /// </summary>
        public void MarkNode()
        {
            this.isMarked = true;
        }

        /// <summary>
        /// Check if node is marked as visited
        /// Checks if the contig count field is set to a specific value.
        /// </summary>
        /// <returns>True if marked; otherwise false.</returns>
        public bool IsMarked()
        {
            return this.isMarked;
        }

        /// <summary>
        /// Check if input node is null.
        /// </summary>
        /// <param name="node">Input node.</param>
        private static void ValidateNode(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
        }
    }
}
