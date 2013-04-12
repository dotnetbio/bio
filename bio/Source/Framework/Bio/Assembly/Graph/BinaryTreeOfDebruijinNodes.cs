using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Graph;
using Bio.Algorithms.Kmer;

namespace Bio
    {
        /// <summary>
        ///  A Binary Search Tree for Debruijin Nodes
        /// </summary>
        public class BinaryTreeOfDebrujinNodes 
        {
            #region Member variables
            /// <summary>
            /// Holds Root node.
            /// </summary>
            private DeBruijnNode root;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes an instance of BinaryTree class.
            /// </summary>
            public BinaryTreeOfDebrujinNodes() 
            {

            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets number of elements present in the BinaryTree.
            /// </summary>
            public long Count { get; private set; }
            #endregion

            #region Methods
           

            /// <summary>
            /// Tries to add specified value to the BinaryTree.
            /// If the value is already present in the tree then this method returns the value already in the tree.
            /// Useful when two values that are equal by comparison are not equal by reference.
            /// </summary>
            /// <param name="value">Value to add.</param>
            /// <returns>Returns the value added or already in the tree, else returns false.</returns>
            public DeBruijnNode AddOrReturnCurrent(KmerData32 value)
            {
                DeBruijnNode toReturn = null;
                if (this.root == null)
                {
                    toReturn = makeNewNode(value);
                    this.root = toReturn;
                    
                }
                else
                {
                    ulong newKey=value.KmerData;
                    DeBruijnNode node = this.root;
                    while (true)
                    {
                        ulong currentKey=node.NodeValue.KmerData;
                        if (currentKey==newKey)
                        {
                            // key already exists.
                            toReturn = node;
                            break;
                        }
                        else if (newKey<currentKey)
                        {
                            // go to left.
                            if (node.Left == null)
                            {
                                toReturn = makeNewNode(value);
                                node.Left = toReturn;
                                break;
                            }
                            else
                            {
                                node = node.Left;
                            }
                        }
                        else
                        {
                            // go to right.
                            if (node.Right == null)
                            {
                                toReturn = makeNewNode(value);
                                node.Right = toReturn;
                                break;
                            }
                            else
                            {
                                node = node.Right;
                            }
                        }
                    }                  
                }
                return toReturn;
            }

            /// <summary>
            /// Makes a new DeBruijinNode for a kmer, ignores orientation
            /// </summary>
            /// <param name="value">Kmer to make node with</param>
            private DeBruijnNode makeNewNode(KmerData32 value)
            {
                Count++;
                return new DeBruijnNode(value, 0);
            }


        /// <summary>
        /// Searches for a particular node in the tree.
        /// </summary>
        /// <param name="kmerValue">The node to be searched.</param>
        /// <returns>Actual node in the tree.</returns>
        public DeBruijnNode SearchTree(KmerData32 kmerValue)
        {	 	 	 	 	 	 	 	 	 	
            DeBruijnNode startNode = this.root;
 	 	 	while(startNode!=null)
            {
                ulong currentValue = startNode.NodeValue.KmerData;
                // parameter value found
                if (currentValue == kmerValue.KmerData)
                {
                    break;
                }
                else if (kmerValue.KmerData < currentValue)
                {
                    // Search left if the value is smaller than the current node
                    startNode = startNode.Left; // search left
                }
                else
                {
                    startNode = startNode.Right; // search right
                    
                }
            }
            return startNode;
        }


            /// <summary>
            /// Gets all nodes in tree.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
            public IEnumerable<DeBruijnNode> GetNodes()
            {
                if (this.Count > 0)
                {
                    int guess = (int)Math.Log(this.Count, 2.0);//tree isn't balanced so can't no for sure how big it needs to be, starting guess
                    guess = guess > 1 ? guess : 4;
                    Stack<DeBruijnNode> traversalStack = new Stack<DeBruijnNode>((int)Math.Log(this.Count, 2.0));
                    DeBruijnNode current;
                    traversalStack.Push(this.root);
                    while (traversalStack.Count > 0)
                    {
                        current = traversalStack.Pop();
                        if (current != null)
                        {
                            traversalStack.Push(current.Right);
                            traversalStack.Push(current.Left);
                            if (!current.IsDeleted)
                            {
                                yield return current;
                            }
                        }
                    }
                }
            }
            #endregion


         


        }
}
