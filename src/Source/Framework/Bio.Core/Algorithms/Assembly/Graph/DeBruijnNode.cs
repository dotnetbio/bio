using System;
using System.Collections.Generic;
using Bio.Algorithms.Kmer;
using System.Diagnostics;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Represents a node in the De Bruijn graph
    /// A node is associated with a k-mer. 
    /// Also holds adjacency information with other nodes.
    /// </summary>
    [DebuggerDisplay("Seq = {ORIGINAL_SYMBOLS}")]
    public class DeBruijnNode
    {
        //temp addition
        /// <summary>
        /// 
        /// </summary>
        public string ORIGINAL_SYMBOLS
        {
            get {
                return new Bio.Sequence(DnaAlphabet.Instance, GetOriginalSymbols(6)).ConvertToString(0,6);
            }
        }

        #region Node Operations Masks
        private const byte NodeOperationMaskLeftExtension       = 56;   // 8,16,24 
        private const byte NodeOperationMaskRightExtension      = 7;    // 1,2,4
        private const byte NodeOperationMaskIsMarkedForDelete   = 64;
        private const byte NodeOperationMaskIsValidExtension    = 128;
        #endregion

        #region Node Orientation Masks
        private const byte NodeMaskRightExtension0 = 1;
        private const byte NodeMaskRightExtension1 = 2;
        private const byte NodeMaskRightExtension2 = 4;
        private const byte NodeMaskRightExtension3 = 8;

        private const byte NodeMaskLeftExtension0 = 16;
        private const byte NodeMaskLeftExtension1 = 32;
        private const byte NodeMaskLeftExtension2 = 64;
        private const byte NodeMaskLeftExtension3 = 128;
        #endregion

        #region Node Info Masks
        private const byte NodeInfoMaskDeleted = 1;
        private const byte NodeInfoVisitedFlag = 2;
        #endregion

        /// <summary>
        /// Holds a flag to indicate whether this node is deleted or not.
        /// </summary>
        private byte _nodeInfo;

        /// <summary>
        /// Holds the value of validextension required, is node marked for deletion , right extension count and left extension count
        /// in 8, 7, 4 to 6 and 1 to 3 bits respectively.
        /// </summary>
        private byte _nodeOperations;

        /// <summary>
        /// Stores the node orientation.
        /// First 4 bits Forward links orientation, next 4 bits reverse links orientation (from Right to Left).
        /// If bit values are 1 then same orientation. If bit values are 0 then orientation is different.
        /// </summary>
        private byte _nodeOrientation;

        /// <summary>
        /// Stores the valid Node extensions
        /// First 4 bits Forward links orientation, next 4 bits reverse links orientation (from Right to Left).
        /// If bit values are 0 then valid extension. If bit values are 1 then not a valid extension.
        /// </summary>
        private byte _validNodeExtensions;

        /// <summary>
        /// Initializes a new instance of the DeBruijnNode class.
        /// </summary>
        public DeBruijnNode(KmerData32 value, byte count)
        {
            this.NodeValue = value;
            this.KmerCount = count;
        }

        /// <summary>
        /// Gets or sets the value of an DeBrujinNode.
        /// </summary>
        public KmerData32 NodeValue { get; set; }

        /// <summary>
        /// Gets or sets the number of duplicate kmers in the DeBrujin graph.
        /// </summary>
        public byte KmerCount { get; set; }

        /// <summary>
        /// Gets or sets the Left node, used by binary tree.
        /// </summary>
        public DeBruijnNode Left { get; set; }

        /// <summary>
        /// Gets or sets the Right Node, used by binary tree.
        /// </summary>
        public DeBruijnNode Right { get; set; }

        /// <summary>
        /// Gets a value indicating whether the node is marked for deletion or not.
        /// </summary>
        public bool IsMarkedForDelete
        {
            get
            {
                return ((this._nodeOperations & NodeOperationMaskIsMarkedForDelete) == NodeOperationMaskIsMarkedForDelete);
            }

            private set
            {
                if (value)
                {
                    this._nodeOperations = (byte)(this._nodeOperations | NodeOperationMaskIsMarkedForDelete);
                }
                else
                {
                    this._nodeOperations = (byte)(this._nodeOperations & (~NodeOperationMaskIsMarkedForDelete));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node is deleted or not.
        /// Note: As we are only periodically not deleting any nodes from the Tree, this flag helps to
        /// identify which nodes are deleted. 
        /// 
        /// TODO: Ensure this variable cannot be modified without modifying the parent graphs node count
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return (this._nodeInfo & NodeInfoMaskDeleted) == NodeInfoMaskDeleted;
            }

            set
            {
                if (value)
                {
                    this._nodeInfo = (byte)(this._nodeInfo | NodeInfoMaskDeleted);
                }
                else
                {
                    throw new ArgumentException("Deleted nodes should not be restored, use IsMarkedForDelete if unsure of the nodes future.");
                }
            }
        }

        /// <summary>
        /// A flag that can be used to determine if the node has been visited 
        /// during a specific step
        /// </summary>
        public bool IsVisited
        {
            get
            {
                return (this._nodeInfo & NodeInfoVisitedFlag) == NodeInfoVisitedFlag;
            }

            set
            {
                if (value)
                {
                    this._nodeInfo = (byte)(this._nodeInfo | NodeInfoVisitedFlag);
                }
                else
                {
                    this._nodeInfo = (byte)(this._nodeInfo & ~NodeInfoVisitedFlag);
                }
            }
        }
      

        /// <summary>
        /// Gets the number of extension nodes.
        /// </summary>
        public int ExtensionsCount
        {
            get
            {
                return this.LeftExtensionNodesCount + this.RightExtensionNodesCount;
            }
        }

        /// <summary>
        /// Gets or sets the number of right extension nodes.
        /// </summary>
        public byte RightExtensionNodesCount
        {
            get
            {
                int count = (this._nodeOperations & NodeOperationMaskRightExtension);
                if (this.ValidExtensionsRequried)
                {
                    if (this.InvalidRightExtension0) 
                    { 
                        count--; 
                    }

                    if (this.InvalidRightExtension1) 
                    { 
                        count--; 
                    }

                    if (this.InvalidRightExtension2) 
                    { 
                        count--; 
                    }

                    if (this.InvalidRightExtension3) 
                    { 
                        count--; 
                    }
                }

                return (byte)count;
            }

            set
            {
                if (value > 4)
                {
                    throw new ArgumentException("Value cant be more than 4");
                }

                this._nodeOperations = (byte)(this._nodeOperations & ~(NodeOperationMaskRightExtension));
                this._nodeOperations = (byte)(this._nodeOperations | (NodeOperationMaskRightExtension & value));
            }
        }

        /// <summary>
        /// Gets or sets the number of left extension nodes.
        /// </summary>
        public byte LeftExtensionNodesCount
        {
            get
            {
                int count = ((this._nodeOperations & NodeOperationMaskLeftExtension) >> 3);
                if (this.ValidExtensionsRequried)
                {
                    if (this.InvalidLeftExtension0) 
                    { 
                        count--; 
                    }
                    
                    if (this.InvalidLeftExtension1) 
                    { 
                        count--; 
                    }
                    
                    if (this.InvalidLeftExtension2) 
                    { 
                        count--; 
                    }
                    
                    if (this.InvalidLeftExtension3) 
                    { 
                        count--; 
                    }
                }

                return (byte)count;
            }
            set
            {
                if (value > 4)
                {
                    throw new ArgumentException("Value cant be more than 4");
                }

                this._nodeOperations = (byte)(this._nodeOperations & (~NodeOperationMaskLeftExtension));
                this._nodeOperations = (byte)(this._nodeOperations | (NodeOperationMaskLeftExtension & (value << 3)));
            }
        }

        #region Private Properties

        /// <summary>
        /// Gets or sets a value indicating whether node has valid extension or not.
        /// </summary>
        private bool ValidExtensionsRequried
        {
            get
            {
                return ((this._nodeOperations & NodeOperationMaskIsValidExtension) == NodeOperationMaskIsValidExtension);
            }
            set
            {
                if (value)
                {
                    this._nodeOperations = (byte)(this._nodeOperations | NodeOperationMaskIsValidExtension);
                }
                else
                {
                    this._nodeOperations = (byte)(this._nodeOperations & (~NodeOperationMaskIsValidExtension));
                }
            }
        }

        #region Node Orientation Private Properties

        private bool OrientationRightExtension0
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskRightExtension0) == NodeMaskRightExtension0); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskRightExtension0);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskRightExtension0));
                }
            }
        }

        private bool OrientationRightExtension1
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskRightExtension1) == NodeMaskRightExtension1); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskRightExtension1);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskRightExtension1));
                }
            }
        }

        private bool OrientationRightExtension2
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskRightExtension2) == NodeMaskRightExtension2); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskRightExtension2);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskRightExtension2));
                }
            }
        }

        private bool OrientationRightExtension3
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskRightExtension3) == NodeMaskRightExtension3); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskRightExtension3);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskRightExtension3));
                }
            }
        }

        private bool OrientationLeftExtension0
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskLeftExtension0) == NodeMaskLeftExtension0); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskLeftExtension0);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskLeftExtension0));
                }
            }
        }

        private bool OrientationLeftExtension1
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskLeftExtension1) == NodeMaskLeftExtension1); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskLeftExtension1);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskLeftExtension1));
                }
            }
        }

        private bool OrientationLeftExtension2
        {
            get 
            { 
                return ((this._nodeOrientation & NodeMaskLeftExtension2) == NodeMaskLeftExtension2); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskLeftExtension2);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskLeftExtension2));
                }
            }
        }

        private bool OrientationLeftExtension3
        {
            get
            { 
                return ((this._nodeOrientation & NodeMaskLeftExtension3) == NodeMaskLeftExtension3); 
            }

            set
            {
                if (value)
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation | NodeMaskLeftExtension3);
                }
                else
                {
                    this._nodeOrientation = (byte)(this._nodeOrientation & (~NodeMaskLeftExtension3));
                }
            }
        }
        #endregion

        #region Node Extensions Private Properties

        private bool InvalidRightExtension0
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskRightExtension0) == NodeMaskRightExtension0);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskRightExtension0);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskRightExtension0));
                }
            }
        }

        private bool InvalidRightExtension1
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskRightExtension1) == NodeMaskRightExtension1);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskRightExtension1);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskRightExtension1));
                }
            }
        }

        private bool InvalidRightExtension2
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskRightExtension2) == NodeMaskRightExtension2);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskRightExtension2);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskRightExtension2));
                }
            }
        }

        private bool InvalidRightExtension3
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskRightExtension3) == NodeMaskRightExtension3);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskRightExtension3);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskRightExtension3));
                }
            }
        }

        private bool InvalidLeftExtension0
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskLeftExtension0) == NodeMaskLeftExtension0);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskLeftExtension0);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskLeftExtension0));
                }
            }
        }

        private bool InvalidLeftExtension1
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskLeftExtension1) == NodeMaskLeftExtension1);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskLeftExtension1);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskLeftExtension1));
                }
            }
        }

        private bool InvalidLeftExtension2
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskLeftExtension2) == NodeMaskLeftExtension2);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskLeftExtension2);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskLeftExtension2));
                }
            }
        }

        private bool InvalidLeftExtension3
        {
            get
            {
                return ((this._validNodeExtensions & NodeMaskLeftExtension3) == NodeMaskLeftExtension3);
            }

            set
            {
                if (value)
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions | NodeMaskLeftExtension3);
                }
                else
                {
                    this._validNodeExtensions = (byte)(this._validNodeExtensions & (~NodeMaskLeftExtension3));
                }
            }
        }
        #endregion

        #region Node Extension Properties

        /// <summary>
        /// Gets or sets the RightExtension node for dna symbol 'A'.
        /// </summary>
        private DeBruijnNode RightExtension0 { get; set; }

        /// <summary>
        /// Gets or sets the RightExtension node for dna symbol 'C'.
        /// </summary>
        private DeBruijnNode RightExtension1 { get; set; }

        /// <summary>
        /// Gets or sets the RightExtension node for dna symbol 'G'.
        /// </summary>
        private DeBruijnNode RightExtension2 { get; set; }

        /// <summary>
        /// Gets or sets the RightExtension node for dna symbol 'T'.
        /// </summary>
        private DeBruijnNode RightExtension3 { get; set; }

        /// <summary>
        /// Gets or sets the Left Extension node for dna symbol 'A'.
        /// </summary>
        private DeBruijnNode LeftExtension0 { get; set; }

        /// <summary>
        /// Gets or sets the Left Extension node for dna symbol 'C'.
        /// </summary>
        private DeBruijnNode LeftExtension1 { get; set; }

        /// <summary>
        /// Gets or sets the Left Extension node for dna symbol 'G'.
        /// </summary>
        private DeBruijnNode LeftExtension2 { get; set; }

        /// <summary>
        /// Gets or sets the Left Extension node for dna symbol 'T'.
        /// </summary>
        private DeBruijnNode LeftExtension3 { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Marks the LeftExtensions of the current node as invalid.
        /// </summary>
        /// <param name="node">Debruijn node which matches one of the left extensions of the current node.</param>
        public bool MarkLeftExtensionAsInvalid(DeBruijnNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (this.LeftExtension0 == node)
            {
                this.InvalidLeftExtension0 = true;
                return true;
            }
            else if (this.LeftExtension1 == node)
            {
                this.InvalidLeftExtension1 = true;
                return true;
            }
            else if (this.LeftExtension2 == node)
            {
                this.InvalidLeftExtension2 = true;
                return true;
            }
            else if (this.LeftExtension3 == node)
            {
                this.InvalidLeftExtension3 = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Marks the RightExtensions of the current node as invalid.
        /// </summary>
        /// <param name="node">Debruijn node which matches one of the right extensions of the current node.</param>
        public bool MarkRightExtensionAsInvalid(DeBruijnNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (this.RightExtension0 == node)
            {
                this.InvalidRightExtension0 = true;
                return true;
            }
            else if (this.RightExtension1 == node)
            {
                this.InvalidRightExtension1 = true;
                return true;
            }
            else if (this.RightExtension2 == node)
            {
                this.InvalidRightExtension2 = true;
                return true;
            }
            else if (this.RightExtension3 == node)
            {
                this.InvalidRightExtension3 = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes the extension nodes those are marked for deletion.
        /// </summary>
        public void RemoveMarkedExtensions()
        {
            // If node is marked for deletion, ignore it. No need for any update.
            if (this.IsMarkedForDelete)
            {
                return;
            }

            if (this.RightExtension0 != null && this.RightExtension0.IsMarkedForDelete)
            {
                this.RightExtension0 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.RightExtension1 != null && this.RightExtension1.IsMarkedForDelete)
            {
                this.RightExtension1 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.RightExtension2 != null && this.RightExtension2.IsMarkedForDelete)
            {
                this.RightExtension2 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.RightExtension3 != null && this.RightExtension3.IsMarkedForDelete)
            {
                this.RightExtension3 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.LeftExtension0 != null && this.LeftExtension0.IsMarkedForDelete)
            {
                this.LeftExtension0 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }

            if (this.LeftExtension1 != null && this.LeftExtension1.IsMarkedForDelete)
            {
                this.LeftExtension1 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }

            if (this.LeftExtension2 != null && this.LeftExtension2.IsMarkedForDelete)
            {
                this.LeftExtension2 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }

            if (this.LeftExtension3 != null && this.LeftExtension3.IsMarkedForDelete)
            {
                this.LeftExtension3 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }
        }
                
        /// <summary>
        /// Sets the extension nodes of the current node.
        /// </summary>
        /// <param name="isForwardDirection">True indicates Right extension and false indicates left extension.</param>
        /// <param name="sameOrientation">Orientation of the connecting edge.</param>
        /// <param name="extensionNode">Node to which the extension is to be set.</param>
        public void SetExtensionNode(bool isForwardDirection, bool sameOrientation, DeBruijnNode extensionNode)
        {
            if (extensionNode == null)
            {
                return;
            }

            lock (this)
            {
                // First 4 bits Forward links orientation, next 4 bits reverse links orientation
                // If bit values are 1 then same orientation. If bit values are 0 then orientation is different.
                if (isForwardDirection)
                {                   
                  
                        if (this.RightExtension0 == null)
                        {
                            this.RightExtension0 = extensionNode;
                            this.OrientationRightExtension0 = sameOrientation;
                        }
                        else if (this.RightExtension1 == null)
                        {
                            this.RightExtension1 = extensionNode;
                            this.OrientationRightExtension1 = sameOrientation;
                        }
                        else if (this.RightExtension2 == null)
                        {
                            this.RightExtension2 = extensionNode;
                            this.OrientationRightExtension2 = sameOrientation;
                        }
                        else if (this.RightExtension3 == null)
                        {
                            this.RightExtension3 = extensionNode;
                            this.OrientationRightExtension3 = sameOrientation;
                        }
                        else
                        {
                            throw new ArgumentException("Can't set more than four extensions.");
                        }
                  
                    this.RightExtensionNodesCount += 1;
                }
                else
                {
                   
                        if (this.LeftExtension0 == null)
                        {
                            this.LeftExtension0 = extensionNode;
                            this.OrientationLeftExtension0 = sameOrientation;
                        }
                        else if (this.LeftExtension1 == null)
                        {
                            this.LeftExtension1 = extensionNode;
                            this.OrientationLeftExtension1 = sameOrientation;
                        }
                        else if (this.LeftExtension2 == null)
                        {
                            this.LeftExtension2 = extensionNode;
                            this.OrientationLeftExtension2 = sameOrientation;
                        }
                        else if (this.LeftExtension3 == null)
                        {
                            this.LeftExtension3 = extensionNode;
                            this.OrientationLeftExtension3 = sameOrientation;
                        }
                        else
                        {
                            throw new ArgumentException("Can't set more than four extensions.");
                        }
                    
                    this.LeftExtensionNodesCount += 1;
                }
            }
        }

        /// <summary>
        /// Returns all the left extension and right extension nodes of the current node.
        /// </summary>
        /// <returns>Left extension and right extension nodes.</returns>
        public IEnumerable<DeBruijnNode> GetExtensionNodes()
        {
            if (this.LeftExtension0 != null)
            {
                yield return this.LeftExtension0;
            }

            if (this.LeftExtension1 != null)
            {
                yield return this.LeftExtension1;
            }

            if (this.LeftExtension2 != null)
            {
                yield return this.LeftExtension2;
            }

            if (this.LeftExtension3 != null)
            {
                yield return this.LeftExtension3;
            }

            if (this.RightExtension0 != null)
            {
                yield return this.RightExtension0;
            }

            if (this.RightExtension1 != null)
            {
                yield return this.RightExtension1;
            }

            if (this.RightExtension2 != null)
            {
                yield return this.RightExtension2;
            }

            if (this.RightExtension3 != null)
            {
                yield return this.RightExtension3;
            }
        }

        /// <summary>
        /// Retrives the list of right extension nodes along with the orientation.
        /// </summary>
        /// <returns>Dictionary with the right extension node and the orientation.</returns>
        public Dictionary<DeBruijnNode, bool> GetRightExtensionNodesWithOrientation()
        {
            lock (this)
            {
                Dictionary<DeBruijnNode, bool> extensions = new Dictionary<DeBruijnNode, bool>();

                if (this.ValidExtensionsRequried)
                {
                    if (this.RightExtension0 != null && !this.InvalidRightExtension0)
                    {
                        if (!extensions.ContainsKey(this.RightExtension0))
                        {
                            extensions.Add(this.RightExtension0, this.OrientationRightExtension0);
                        }
                    }

                    if (this.RightExtension1 != null && !this.InvalidRightExtension1)
                    {
                        if (!extensions.ContainsKey(this.RightExtension1))
                        {
                            extensions.Add(this.RightExtension1, this.OrientationRightExtension1);
                        }
                    }

                    if (this.RightExtension2 != null && !this.InvalidRightExtension2)
                    {
                        if (!extensions.ContainsKey(this.RightExtension2))
                        {
                            extensions.Add(this.RightExtension2, this.OrientationRightExtension2);
                        }
                    }

                    if (this.RightExtension3 != null && !this.InvalidRightExtension3)
                    {
                        if (!extensions.ContainsKey(this.RightExtension3))
                        {
                            extensions.Add(this.RightExtension3, this.OrientationRightExtension3);
                        }
                    }
                }
                else
                {
                    if (this.RightExtension0 != null)
                    {
                        if (!extensions.ContainsKey(this.RightExtension0))
                        {
                            extensions.Add(this.RightExtension0, this.OrientationRightExtension0);
                        }
                    }

                    if (this.RightExtension1 != null)
                    {
                        if (!extensions.ContainsKey(this.RightExtension1))
                        {
                            extensions.Add(this.RightExtension1, this.OrientationRightExtension1);
                        }
                    }

                    if (this.RightExtension2 != null)
                    {
                        if (!extensions.ContainsKey(this.RightExtension2))
                        {
                            extensions.Add(this.RightExtension2, this.OrientationRightExtension2);
                        }
                    }

                    if (this.RightExtension3 != null)
                    {
                        if (!extensions.ContainsKey(this.RightExtension3))
                        {
                            extensions.Add(this.RightExtension3, this.OrientationRightExtension3);
                        }
                    }
                }

                return extensions;
            }
        }

        /// <summary>
        /// Retrives the list of left extension nodes along with the orientation.
        /// </summary>
        /// <returns>Dictionary with the left extension node and the orientation.</returns>
        public Dictionary<DeBruijnNode, bool> GetLeftExtensionNodesWithOrientation()
        {
            lock (this)
            {
                Dictionary<DeBruijnNode, bool> extentions = new Dictionary<DeBruijnNode, bool>();

                if (this.ValidExtensionsRequried)
                {
                    if (this.LeftExtension0 != null && !this.InvalidLeftExtension0)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension0))
                        {
                            extentions.Add(this.LeftExtension0, this.OrientationLeftExtension0);
                        }
                    }

                    if (this.LeftExtension1 != null && !this.InvalidLeftExtension1)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension1))
                        {
                            extentions.Add(this.LeftExtension1, this.OrientationLeftExtension1);
                        }
                    }

                    if (this.LeftExtension2 != null && !this.InvalidLeftExtension2)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension2))
                        {
                            extentions.Add(this.LeftExtension2, this.OrientationLeftExtension2);
                        }
                    }

                    if (this.LeftExtension3 != null && !this.InvalidLeftExtension3)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension3))
                        {
                            extentions.Add(this.LeftExtension3, this.OrientationLeftExtension3);
                        }
                    }
                }
                else
                {
                    if (this.LeftExtension0 != null)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension0))
                        {
                            extentions.Add(this.LeftExtension0, this.OrientationLeftExtension0);
                        }
                    }

                    if (this.LeftExtension1 != null)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension1))
                        {
                            extentions.Add(this.LeftExtension1, this.OrientationLeftExtension1);
                        }
                    }

                    if (this.LeftExtension2 != null)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension2))
                        {
                            extentions.Add(this.LeftExtension2, this.OrientationLeftExtension2);
                        }
                    }

                    if (this.LeftExtension3 != null)
                    {
                        if (!extentions.ContainsKey(this.LeftExtension3))
                        {
                            extentions.Add(this.LeftExtension3, this.OrientationLeftExtension3);
                        }
                    }
                }

                return extentions;
            }
        }

        /// <summary>
        /// Removes edge corresponding to the node from appropriate data structure,
        /// after checking whether given node is part of left or right extensions.
        /// Thread-safe method.
        /// </summary>
        /// <param name="node">Node for which extension is to be removed.</param>
        public void RemoveExtensionThreadSafe(DeBruijnNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            lock (this)
            {
                if (this.RightExtension0 == node)
                {
                    this.RightExtension0 = null;
                    this.RightExtensionNodesCount--;
                    return;
                }

                if (this.RightExtension1 == node)
                {
                    this.RightExtension1 = null;
                    this.RightExtensionNodesCount--;
                    return;
                }

                if (this.RightExtension2 == node)
                {
                    this.RightExtension2 = null;
                    this.RightExtensionNodesCount--;
                    return;
                }

                if (this.RightExtension3 == node)
                {
                    this.RightExtension3 = null;
                    this.RightExtensionNodesCount--;
                    return;
                }
            }

            lock (this)
            {
                if (this.LeftExtension0 == node)
                {
                    this.LeftExtension0 = null;
                    this.LeftExtensionNodesCount--;
                    return;
                }

                if (this.LeftExtension1 == node)
                {
                    this.LeftExtension1 = null;
                    this.LeftExtensionNodesCount--;
                    return;
                }

                if (this.LeftExtension2 == node)
                {
                    this.LeftExtension2 = null;
                    this.LeftExtensionNodesCount--;
                    return;
                }

                if (this.LeftExtension3 == node)
                {
                    this.LeftExtension3 = null;
                    this.LeftExtensionNodesCount--;
                    return;
                }
            }
        }

      
        /// <summary>
        /// Retrieves all the Left extension nodes of the current node.
        /// </summary>
        /// <returns>Right extension nodes.</returns>
        public IEnumerable<DeBruijnNode> GetLeftExtensionNodes()
        {
            if (this.ValidExtensionsRequried)
            {
                if (this.LeftExtension0 != null && !this.InvalidLeftExtension0)
                {
                    yield return this.LeftExtension0;
                }

                if (this.LeftExtension1 != null && !this.InvalidLeftExtension1)
                {
                    yield return this.LeftExtension1;
                }

                if (this.LeftExtension2 != null && !this.InvalidLeftExtension2)
                {
                    yield return this.LeftExtension2;
                }

                if (this.LeftExtension3 != null && !this.InvalidLeftExtension3)
                {
                    yield return this.LeftExtension3;
                }
            }
            else
            {
                if (this.LeftExtension0 != null)
                {
                    yield return this.LeftExtension0;
                }

                if (this.LeftExtension1 != null)
                {
                    yield return this.LeftExtension1;
                }

                if (this.LeftExtension2 != null)
                {
                    yield return this.LeftExtension2;
                }

                if (this.LeftExtension3 != null)
                {
                    yield return this.LeftExtension3;
                }
            }
        }

        /// <summary>
        /// Retrieves all the Right extension nodes of the current node.
        /// </summary>
        /// <returns>Right extension nodes.</returns>
        public IEnumerable<DeBruijnNode> GetRightExtensionNodes()
        {
            if (this.ValidExtensionsRequried)
            {
                if (this.RightExtension0 != null && !this.InvalidRightExtension0)
                {
                    yield return this.RightExtension0;
                }

                if (this.RightExtension1 != null && !this.InvalidRightExtension1)
                {
                    yield return this.RightExtension1;
                }

                if (this.RightExtension2 != null && !this.InvalidRightExtension2)
                {
                    yield return this.RightExtension2;
                }

                if (this.RightExtension3 != null && !this.InvalidRightExtension3)
                {
                    yield return this.RightExtension3;
                }
            }
            else
            {
                if (this.RightExtension0 != null)
                {
                    yield return this.RightExtension0;
                }

                if (this.RightExtension1 != null)
                {
                    yield return this.RightExtension1;
                }

                if (this.RightExtension2 != null)
                {
                    yield return this.RightExtension2;
                }

                if (this.RightExtension3 != null)
                {
                    yield return this.RightExtension3;
                }
            }
        }

        /// <summary>
        /// Sets whether valid extensions are required or not.
        /// </summary>
        public void ComputeValidExtensions()
        {
            this.ValidExtensionsRequried = true;
        }

        /// <summary>
        /// Deletes all the extension marked for deletion and sets the node extensions as valid.
        /// </summary>
        public void UndoAmbiguousExtensions()
        {
            // done with the valid extensions set vaidExtensionsRequired to false.
            this.ValidExtensionsRequried = false;

            this.RemoveMarkedExtensions();

            // mark all extensions as valid.
            this._validNodeExtensions = 0;
        }

        /// <summary>
        /// Removes all the invalid extensions permanently.
        /// </summary>
        public void PurgeInvalidExtensions()
        {
            if (this.RightExtension0 != null && this.InvalidRightExtension0)
            {
                this.RightExtension0 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.RightExtension1 != null && this.InvalidRightExtension1)
            {
                this.RightExtension1 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.RightExtension2 != null && this.InvalidRightExtension2)
            {
                this.RightExtension2 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.RightExtension3 != null && this.InvalidRightExtension3)
            {
                this.RightExtension3 = null;
                lock (this)
                {
                    this.RightExtensionNodesCount--;
                }
            }

            if (this.LeftExtension0 != null && this.InvalidLeftExtension0)
            {
                this.LeftExtension0 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }

            if (this.LeftExtension1 != null && this.InvalidLeftExtension1)
            {
                this.LeftExtension1 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }

            if (this.LeftExtension2 != null && this.InvalidLeftExtension2)
            {
                this.LeftExtension2 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }

            if (this.LeftExtension3 != null && this.InvalidLeftExtension3)
            {
                this.LeftExtension3 = null;
                lock (this)
                {
                    this.LeftExtensionNodesCount--;
                }
            }
        }

        /// <summary>
        /// Marks the node for deletion.
        /// </summary>
        public void MarkNodeForDelete()
        {
            this.IsMarkedForDelete = true;
        }

        /// <summary>
        /// Makes extension edge corresponding to the node invalid,
        /// after checking whether given node is part of left or right extensions.
        /// Not Thread-safe. Use lock at caller if required.
        /// </summary>
        /// <param name="node">Node for which extension is to be made invalid.</param>
        public void MarkExtensionInvalid(DeBruijnNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (!this.MarkRightExtensionAsInvalid(node))
            {
                this.MarkLeftExtensionAsInvalid(node);
            }
        }

        /// <summary>
        /// Gets the original symbols.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Return the decompressed kmer data.</returns>
        public byte[] GetOriginalSymbols(int kmerLength)
        {
            return this.NodeValue.GetOriginalSymbols(kmerLength);
        }

        /// <summary>
        /// Gets the reverse complement of original symbols.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Returns the reverse complement of the current node value.</returns>
        public byte[] GetReverseComplementOfOriginalSymbols(int kmerLength)
        {
            return this.NodeValue.GetReverseComplementOfOriginalSymbols(kmerLength);
        }

        /// <summary>
        /// Checks whether the node value (kmer data) is palindrome or not.
        /// </summary>
        /// <returns>True if the node value is palindrome otherwise false.</returns>
        public bool IsPalindrome(int kmerLength)
        {
            return this.NodeValue.IsPalindrome(kmerLength);
        }
    }
}
