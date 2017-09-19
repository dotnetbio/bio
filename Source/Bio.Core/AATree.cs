using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bio
{
    /// <summary>
    ///  Arne Andersson Self Balancing Binary Search Tree.
    /// </summary>
    /// <typeparam name="T">Type of elements to store.</typeparam>
    public class AATree<T>
    {
        #region AATreeNode Class
        [DebuggerDisplay("Value: {Value}")]
        internal class AATreeNode
        {
            /// <summary>
            /// Constructor to initialize null node.
            /// </summary>
            public AATreeNode()
            {
                this.Left = this;
                this.Right = this;
                this.Level = 0;
            }

            public AATreeNode(T value, AATreeNode nullNode)
            {
                this.Value = value;
                this.Left = nullNode;
                this.Right = nullNode;
                this.Level = 1;
            }

            public T Value { get; set; }
            public AATreeNode Left { get; set; }
            public AATreeNode Right { get; set; }
            public int Level { get; set; }
        }
        #endregion

        #region Member variables
       /// <summary>
       /// Holds null node.
       /// </summary>
        private static AATreeNode NullNode = new AATreeNode();

        /// <summary>
        /// Holds Root node.
        /// </summary>
        private AATreeNode root;

        /// <summary>
        /// Comparer to compare values.
        /// </summary>
        private IComparer<T> Comparer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an instance of AATree class.
        /// </summary>
        public AATree() : this(Comparer<T>.Default) { }

        /// <summary>
        /// Initializes an instance of AATree class with specified comparer.
        /// </summary>
        /// <param name="comparer">Comparer to compare values.</param>
        public AATree(IComparer<T> comparer)
        {
            this.Comparer = comparer;
            this.root = AATree<T>.NullNode;
            this.DefaultValue = default(T);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Default value.
        /// By default this is set to default value of T.
        /// </summary>
        public T DefaultValue { get; set; }

        /// <summary>
        /// Gets number of elements present in the AATree.
        /// </summary>
        public long Count { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Tries to add specified value to the AATree.
        /// If the value is already present in the tree then this method returns without adding.
        /// </summary>
        /// <param name="value">Value to add.</param>
        /// <returns>Returns true if value is added successfully, else returns false.</returns>
        public bool Add(T value)
        {
            bool result = false;
            if (this.root == AATree<T>.NullNode)
            {
                this.root = new AATreeNode(value, AATree<T>.NullNode);
                result = true;
            }

            if (!result)
            {
                AATreeNode node = this.root;
                Stack<AATreeNode> parentNodes = new Stack<AATreeNode>();
                while (true)
                {
                    int keyCompResult = Comparer.Compare(value, node.Value);
                    if (keyCompResult == 0)
                    {
                        // key already exists.
                        result = false;
                        break;
                    }
                    else if (keyCompResult < 0)
                    {
                        // go to left.
                        if (node.Left == AATree<T>.NullNode)
                        {
                            node.Left = new AATreeNode(value, AATree<T>.NullNode);

                            result = true;
                            break;
                        }
                        else
                        {
                            parentNodes.Push(node);
                            node = node.Left;
                        }
                    }
                    else
                    {
                        // go to right.
                        if (node.Right == AATree<T>.NullNode)
                        {
                            node.Right = new AATreeNode(value, AATree<T>.NullNode);
                            result = true;
                            break;
                        }
                        else
                        {
                            parentNodes.Push(node);
                            node = node.Right;
                        }
                    }
                }

                if (result)
                {
                    parentNodes.Push(node);
                    BalanceTreeAfterAdd(parentNodes);
                }
            }

            if (result)
            {
                Count++;
            }

            return result;
        }

        /// <summary>
        /// Tries to remove specified value from the AATree.
        /// </summary>
        /// <param name="value">Value to remove.</param>
        /// <returns>Returns true if the value is found and removed successfully, else returns false.</returns>
        public bool Remove(T value)
        {
            // Step1. Serach for the value in the tree
            // Step2. if not found return false, if found then go to step3.
            // Step3. if the node is a leaf node then delete the node, else if the node has only one child then delete 
            //        the node and assign its child to parent,
            //        else if the node has two child then find a node which can be replaced by the node to be deleted 
            //        (use LeftmostRight or RightmostLeft node)
            //Step4. balance the tree.

            bool result = false;
            Stack<AATreeNode> parentNodes = new Stack<AATreeNode>();
            AATreeNode node = this.root;
            while (node != AATree<T>.NullNode)
            {
                int keyCompResult = this.Comparer.Compare(value, node.Value);
                if (keyCompResult == 0)
                {
                    result = true;
                    break;
                }
                else if (keyCompResult < 0)
                {
                    //if key is lessthan the node.Key goto left
                    parentNodes.Push(node);
                    node = node.Left;
                }
                else
                {
                    //if key is greaterthan the node.Key goto right
                    parentNodes.Push(node);
                    node = node.Right;
                }
            }

            if (!result)
            {
                return result;
            }

            AATreeNode nodeToDelete = node;
            if (nodeToDelete.Left != AATree<T>.NullNode && nodeToDelete.Right != AATree<T>.NullNode)
            {
                parentNodes.Push(node);
                // using right most of left sub tree.
                node = nodeToDelete.Left;
                while (node.Right != AATree<T>.NullNode)
                {
                    parentNodes.Push(node);
                    node = node.Right;
                }

                var tval = nodeToDelete.Value;
                nodeToDelete.Value = node.Value;
                node.Value = tval;
                nodeToDelete = node;
            }

            AATreeNode parentNode = null;
            if (nodeToDelete.Left == AATree<T>.NullNode || nodeToDelete.Right == AATree<T>.NullNode)
            {
                AATreeNode childNode = nodeToDelete.Left == AATree<T>.NullNode ? nodeToDelete.Right : nodeToDelete.Left;

                if (parentNodes.Count > 0)
                {
                    parentNode = parentNodes.Peek();
                }

                if (parentNode != null)
                {
                    if (parentNode.Left == nodeToDelete)
                    {
                        parentNode.Left = childNode;
                    }
                    else
                    {
                        parentNode.Right = childNode;
                    }
                }
                else
                {
                    this.root = childNode;
                }
            }

            BalanceTreeAfterRemove(parentNodes);

            if (result)
            {
                this.Count--;
            }

            return result;
        }

        /// <summary>
        /// Verifies whether the specified value is present in the tree or not.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>Returns true if the specified value is present in the tree, else returns false.</returns>
        public bool Contains(T value)
        {
            T actualValue;
            bool result = TrySearch(value, out actualValue);
            return result;
        }

        /// <summary>
        /// Searches for the specified value in the AATree.
        /// If found returns the value in actualValue out param, else this param contains DefaultValue.
        /// </summary>
        /// <param name="value">Value to search.</param>
        /// <param name="actualValue">Out parameter.</param>
        /// <returns>Returns true if the value is found, else returns false.</returns>
        public bool TrySearch(T value, out T actualValue)
        {
            actualValue = this.DefaultValue;
            AATreeNode node;
            bool result = this.TrySearch(value, out node);
            if (result)
            {
                actualValue = node.Value;
            }

            return result;
        }

        /// <summary>
        /// Gets values using InOrder traversal.
        /// </summary>
        public IEnumerable<T> InOrderTraversal()
        {
            return AATree<T>.InOrderTraversal(this.root);
        }

        /// <summary>
        /// Gets values using PreOrder traversal.
        /// </summary>
        public IEnumerable<T> PreOrderTraversal()
        {
            return AATree<T>.PreOrderTraversal(this.root);
        }

        /// <summary>
        /// Gets values using PostOrder traversal.
        /// </summary>
        public IEnumerable<T> PostOrderTraversal()
        {
            return AATree<T>.PostOrderTraversal(this.root);
        }

        /// <summary>
        /// Searches for the specified value in the AATree.
        /// If found returns the node containing the value in node out param, else this param contains NullNode.
        /// </summary>
        /// <param name="value">Value to search.</param>
        /// <param name="node">AATree node.</param>
        /// <returns>Returns true if the value is found, else returns false.</returns>
        internal bool TrySearch(T value, out AATreeNode node)
        {
            bool result = false;
            node = AATree<T>.NullNode;

            var currentNode = this.root;
            while (currentNode != AATree<T>.NullNode)
            {
                int compResult = this.Comparer.Compare(value, currentNode.Value);
                if (compResult == 0)
                {
                    node = currentNode;
                    result = true;
                    break;
                }

                if (compResult < 0)
                {
                    //goto left
                    currentNode = currentNode.Left;
                }
                else
                {
                    //goto right
                    currentNode = currentNode.Right;
                }
            }

            return result;
        }

        // InOrder Traversal implementation.
        private static IEnumerable<T> InOrderTraversal(AATreeNode node)
        {
            if (node == AATree<T>.NullNode)
            {
                yield break;
            }

            Stack<AATreeNode> nodes = new Stack<AATreeNode>();
            AATreeNode currentNode = node;
            while (nodes.Count > 0 || currentNode != AATree<T>.NullNode)
            {
                if (currentNode != AATree<T>.NullNode)
                {
                    nodes.Push(currentNode);
                    currentNode = currentNode.Left;
                }
                else
                {
                    currentNode = nodes.Pop();
                    yield return currentNode.Value;
                    currentNode = currentNode.Right;
                }
            }
        }

        // PreOrder Traversal implementation.
        private static IEnumerable<T> PreOrderTraversal(AATreeNode node)
        {
            if (node == AATree<T>.NullNode)
            {
                yield break;
            }

            Stack<AATreeNode> stack = new Stack<AATreeNode>();
            stack.Push(node);
            AATreeNode currentNode = AATree<T>.NullNode;
            while (stack.Count > 0)
            {
                currentNode = stack.Pop();
                if (currentNode.Right != AATree<T>.NullNode)
                {
                    stack.Push(currentNode.Right);
                }

                if (currentNode.Left != AATree<T>.NullNode)
                {
                    stack.Push(currentNode.Left);
                }

                yield return currentNode.Value;
            }
        }

        // PostOrder Traversal implementation.
        private static IEnumerable<T> PostOrderTraversal(AATreeNode node)
        {
            if (node == AATree<T>.NullNode)
            {
                yield break;
            }

            Stack<AATreeNode> nodes = new Stack<AATreeNode>();
            AATreeNode currentNode = node;
            while (true)
            {
                if (currentNode != AATree<T>.NullNode)
                {
                    if (currentNode.Right != AATree<T>.NullNode)
                    {
                        nodes.Push(currentNode.Right);
                    }

                    nodes.Push(currentNode);
                    currentNode = currentNode.Left;
                }
                else
                {
                    if (nodes.Count == 0)
                    {
                        break;
                    }

                    currentNode = nodes.Pop();
                    if (currentNode.Right != AATree<T>.NullNode && nodes.Count > 0 && nodes.Peek() == currentNode.Right)
                    {
                        nodes.Pop();  // remove right;
                        nodes.Push(currentNode); // push current
                        currentNode = currentNode.Right;
                    }
                    else
                    {
                        yield return currentNode.Value;
                        currentNode = AATree<T>.NullNode;
                    }
                }
            }
        }

        // Tree Balancing After Adding an item
        private void BalanceTreeAfterAdd(Stack<AATreeNode> parentNodes)
        {
            AATreeNode parentNode = null;
            while (parentNodes.Count > 0)
            {
               var node = parentNodes.Pop();
                if (parentNodes.Count > 0)
                {
                    parentNode = parentNodes.Peek();
                }
                else
                {
                    parentNode = null;
                }

                // maintain two properties
                // 1. if a node and its left child have same level then rotate right.
                // 2. if a node, its right child and right node's right child have same level then rotate left.
                if (parentNode != null)
                {
                    bool isLeftNode = parentNode.Left == node;
                    RotateRight(parentNode, isLeftNode ? parentNode.Left : parentNode.Right);
                    RotateLeft(parentNode, isLeftNode ? parentNode.Left : parentNode.Right);
                }
                else
                {
                    // check root node.
                    RotateRight(null, this.root);
                    RotateLeft(null, this.root);
                }
            }
        }

        // Tree Balancing After remving an item
        private void BalanceTreeAfterRemove(Stack<AATreeNode> parentNodes)
        {
            AATreeNode parentNode = null;
            // balance the tree.
            while (parentNodes.Count > 0)
            {
               var node = parentNodes.Pop();
                if ((node.Level - node.Left.Level) > 1 || (node.Level - node.Right.Level) > 1)
                {
                    node.Level--;
                    if (node.Right.Level > node.Level)
                    {
                        node.Right.Level = node.Level;
                    }

                    if (parentNodes.Count > 0)
                    {
                        parentNode = parentNodes.Peek();
                    }
                    else
                    {
                        parentNode = null;
                    }

                    if (parentNode != null)
                    {
                        bool isLeftNode = parentNode.Left == node;

                        if (RotateRight(parentNode, node))
                        {
                            // get the new child from parent.
                            node = isLeftNode ? parentNode.Left : parentNode.Right;
                        }

                        if (RotateRight(node, node.Right))

                        if (RotateRight(node.Right, node.Right.Right))

                        if (RotateLeft(parentNode, node))
                        {
                            node = isLeftNode ? parentNode.Left : parentNode.Right;
                        }

                        RotateLeft(node, node.Right);
                    }
                    else
                    {
                        RotateRight(null, this.root);
                        RotateRight(this.root, this.root.Right);
                        RotateRight(this.root.Right, this.root.Right.Right);
                        RotateLeft(null, this.root);
                        RotateLeft(this.root, this.root.Right);
                    }
                }
            }
        }

        /// <summary>
        /// Split or Rotate left.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool RotateLeft(AATreeNode parentNode, AATreeNode node)
        {
            bool result = false;
            if (node == AATree<T>.NullNode)
            {
                return result;
            }

            if (node.Level == node.Right.Level && node.Right.Level == node.Right.Right.Level)
            {
                // rotate left.
                var nodeToMoveUp = node.Right;
                node.Right = nodeToMoveUp.Left;
                nodeToMoveUp.Left = node;
                if (parentNode != null)
                {
                    if (parentNode.Left == node)
                    {
                        parentNode.Left = nodeToMoveUp;
                        parentNode.Left.Level++;
                    }
                    else
                    {
                        parentNode.Right = nodeToMoveUp;
                        parentNode.Right.Level++;
                    }
                }
                else
                {
                    this.root = nodeToMoveUp;
                    this.root.Level++;
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Skew or Rotate right.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool RotateRight(AATreeNode parentNode, AATreeNode node)
        {
            bool result = false;
            if (node == AATree<T>.NullNode)
            {
                return result;
            }

            if (node.Level == node.Left.Level)
            {
                // rotate right.
                var nodeToMoveUp = node.Left;
                node.Left = nodeToMoveUp.Right;
                nodeToMoveUp.Right = node;
                if (parentNode != null)
                {
                    if (parentNode.Left == node)
                    {
                        parentNode.Left = nodeToMoveUp;

                    }
                    else
                    {
                        parentNode.Right = nodeToMoveUp;
                    }
                }
                else
                {
                    this.root = nodeToMoveUp;
                }

                result = true;
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// Dictionary like implementation using AATree.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class AATree<TKey, TValue>
    {
        #region Class to compare KeyValuePairs
        private class KeyValuePairComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            public IComparer<TKey> KeyComparer { get; private set; }

            public KeyValuePairComparer(IComparer<TKey> keyComparer)
            {
                this.KeyComparer = keyComparer;
            }

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return this.KeyComparer.Compare(x.Key, y.Key);
            }
        }
        #endregion

        #region Member variables
        private AATree<KeyValuePair<TKey, TValue>> internalTree;
        private IComparer<TKey> Comparer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an instance of AATree class.
        /// </summary>
        public AATree() : this(Comparer<TKey>.Default) { }

        /// <summary>
        /// Initializes an instance of AATree class with specified comparer.
        /// </summary>
        /// <param name="keyComparer">Comparer to compare Keys.</param>
        public AATree(IComparer<TKey> keyComparer)
        {
            this.Comparer = keyComparer;
            this.DefaultValue = default(TValue);
            this.internalTree = new AATree<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer(this.Comparer));
            this.internalTree.DefaultValue = new KeyValuePair<TKey, TValue>(default(TKey), this.DefaultValue);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default value for TValue type.
        /// This DefaultValue is returned from indexer or TryGetValue methods when the key is not found in the AATree.
        /// </summary>
        public TValue DefaultValue { get; set; }

        /// <summary>
        /// Gets the number of elements present in the AATree.
        /// </summary>
        public long Count
        {
            get
            {
                return this.internalTree.Count;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Tries to add specified key and value to the AATree.
        /// If the key is already present then this method returns without adding.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        /// <returns>Returns true if add is successful, else returns false.</returns>
        public bool Add(TKey key, TValue value)
        {
            return this.internalTree.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        ///  Tries to remove specified key and associated value from the AATree.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        /// <returns>Returns true if the key is found and removed successfully, else returns false.</returns>
        public bool Remove(TKey key)
        {
            return this.internalTree.Remove(new KeyValuePair<TKey, TValue>(key, this.DefaultValue));
        }

        /// <summary>
        /// Gets or sets the value for the specified key.
        /// Get Method,
        /// if the key is found then the associated value will be returned, else DefaultValue is returned.
        /// Set Method,
        /// if the key is found then associated value is replaced with the specified value, 
        /// else Add method will be called to add key and value.
        /// </summary>
        /// <param name="key">Key to search.</param>
        public TValue this[TKey key]
        {
            get
            {
                TValue value = this.DefaultValue;
                KeyValuePair<TKey, TValue> keyValuePair;
                if (this.internalTree.TrySearch(new KeyValuePair<TKey, TValue>(key, this.DefaultValue), out keyValuePair))
                {
                    value = keyValuePair.Value;
                }

                return value;
            }
            set
            {
                AATree<KeyValuePair<TKey, TValue>>.AATreeNode node;
                KeyValuePair<TKey, TValue> keyValuePair;
                if (this.internalTree.TrySearch(new KeyValuePair<TKey, TValue>(key, this.DefaultValue), out node))
                {
                    keyValuePair = node.Value;
                    node.Value = new KeyValuePair<TKey, TValue>(keyValuePair.Key, value);
                }
                else
                {
                    this.internalTree.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
        }

        /// <summary>
        /// Verifies whether the specified key is present in the tree or not.
        /// </summary>
        /// <param name="key">key to verify.</param>
        /// <returns>Returns true if the specified key is present in the tree, else returns false.</returns>
        public bool Contains(TKey key)
        {
            return this.internalTree.Contains(new KeyValuePair<TKey, TValue>(key, this.DefaultValue));
        }

        /// <summary>
        /// Searches for the specified key in the AATree.
        /// If found returns the associated value in value out param, else this param contains DefaultValue.
        /// </summary>
        /// <param name="key">Key to search.</param>
        /// <param name="value">Out parameter.</param>
        /// <returns>Returns true if the key is found, else returns false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = this.DefaultValue;
            KeyValuePair<TKey, TValue> keyValuePair;
            bool result = this.internalTree.TrySearch(new KeyValuePair<TKey, TValue>(key, this.DefaultValue), out keyValuePair);

            if (result)
            {
                value = keyValuePair.Value;
            }

            return result;
        }

        /// <summary>
        /// Gets Key and its associated value using InOrder traversal.
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal()
        {
            return this.internalTree.InOrderTraversal();
        }

        /// <summary>
        /// Gets Key and its associated value using PreOrder traversal.
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal()
        {
            return this.internalTree.PreOrderTraversal();
        }

        /// <summary>
        /// Gets Key and its associated value using PostOrder traversal.
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
        {
            return this.internalTree.PostOrderTraversal();
        }
        #endregion
    }

}