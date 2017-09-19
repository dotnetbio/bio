using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bio.Algorithms.Kmer;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// A Binary Search Tree for Debruijin Nodes
    /// </summary>
    public class BinaryTreeOfDebrujinNodes
    {
        /// <summary>
        /// Holds Root node.
        /// </summary>
        private DeBruijnNode _root;

        /// <summary>
        /// Gets number of elements present in the BinaryTree.
        /// </summary>
        public long Count { get; private set; }

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
            DeBruijnNode toReturn;
            if (_root == null)
            {
                toReturn = MakeNewNode(value);
                _root = toReturn;
            }
            else
            {
                ulong newKey = value.KmerData;
                DeBruijnNode node = _root;
                while (true)
                {
                    ulong currentKey = node.NodeValue.KmerData;
                    if (currentKey == newKey)
                    {
                        // key already exists.
                        toReturn = node;
                        break;
                    }
                    
                    if (newKey < currentKey)
                    {
                        // go to left.
                        if (node.Left == null)
                        {
                            toReturn = MakeNewNode(value);
                            node.Left = toReturn;
                            break;
                        }
                        node = node.Left;
                    }
                    else
                    {
                        // go to right.
                        if (node.Right == null)
                        {
                            toReturn = MakeNewNode(value);
                            node.Right = toReturn;
                            break;
                        }
                        node = node.Right;
                    }
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Makes a new DeBruijinNode for a kmer, ignores orientation
        /// </summary>
        /// <param name="value">Kmer to make node with</param>
        private DeBruijnNode MakeNewNode(KmerData32 value)
        {
            Count++;
            return new DeBruijnNode(value, 0);
        }


        /// <summary>
        ///     Searches for a particular node in the tree.
        /// </summary>
        /// <param name="kmerValue">The node to be searched.</param>
        /// <returns>Actual node in the tree.</returns>
        public DeBruijnNode SearchTree(KmerData32 kmerValue)
        {
            DeBruijnNode startNode = _root;
            while (startNode != null)
            {
                ulong currentValue = startNode.NodeValue.KmerData;
                
                // parameter value found
                if (currentValue == kmerValue.KmerData)
                    break;

                startNode = kmerValue.KmerData < currentValue ? startNode.Left : startNode.Right;
            }

            return startNode;
        }


        /// <summary>
        ///     Gets all nodes in tree.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<DeBruijnNode> GetNodes()
        {
            if (Count > 0)
            {
                var traversalStack = new Stack<DeBruijnNode>((int)Math.Log(Count, 2.0));
                traversalStack.Push(_root);
                while (traversalStack.Count > 0)
                {
                    DeBruijnNode current = traversalStack.Pop();
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