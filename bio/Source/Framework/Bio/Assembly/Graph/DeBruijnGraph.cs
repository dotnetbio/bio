using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bio.Algorithms.Kmer;
using Bio.Util;
using System.Text;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Representation of a De Bruijn Graph.
    /// Graph is encoded as a collection of de Bruijn nodes.
    /// The nodes themselves hold the adjacency information.
    /// </summary>
    public class DeBruijnGraph
    {
        private const int StopAddThreshold = 2000000;
        
        /// <summary>
        /// Max kmer length allowed
        /// </summary>
        public const int MaxKmerLength = 31;

        /// <summary>
        /// Holds dna symbols.
        /// </summary>
        private readonly char[] _dnaSymbols = new[] { 'A', 'T', 'G', 'C' };

        /// <summary>
        /// Holds complement dna symbols.
        /// </summary>
        private readonly char[] _dnaSymbolsComplement = new[] { 'T', 'A', 'C', 'G' };

        /// <summary>
        /// Holds node count.
        /// </summary>
        private long _nodeCount;

        /// <summary>
        /// Holds the number of input sequences processed while building graph.
        /// </summary>
        private long _processedSequencesCount;

        /// <summary>
        /// Holds number of sequences skipped while building graph.
        /// </summary>
        private long _skippedSequencesCount;

        /// <summary>
        /// Initializes a new instance of the DeBruijnGraph class.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        public DeBruijnGraph(int kmerLength)
        {
            Root = null;
            KmerLength = kmerLength;
        }

        /// <summary>
        /// Gets or sets the number of nodes available in the graph.
        /// </summary>
        public long NodeCount
        {
            get { return this._nodeCount; }
            set { this._nodeCount = value; }
        }

        /// <summary>
        /// Gets the kmerlength of the graph.
        /// </summary>
        public int KmerLength { get; private set; }

        /// <summary>
        /// Gets the root node of the graph.
        /// </summary>
        public DeBruijnNode Root { get; private set; }

        /// <summary>
        /// Gets number of sequences processed while building the graph.
        /// </summary>
        public long ProcessedSequencesCount
        {
            get { return this._processedSequencesCount; }
        }

        /// <summary>
        /// Gets number of sequences skipped from the input sequences.
        /// </summary>
        public long SkippedSequencesCount
        {
            get { return this._skippedSequencesCount; }
        }

        /// <summary>
        /// Gets a value indicating that whether the graph is built or not.
        /// </summary>
        public bool GraphBuildCompleted { get; private set; }

        /// <summary>
        /// Gets a value indicating that whether the Link generating is completed or not.
        /// </summary>
        public bool LinkGenerationCompleted { get; private set; }

        /// <summary>
        /// Validate input graph.
        /// Throws exception if graph is null.
        /// </summary>
        /// <param name="graph">Input graph.</param>
        public static void ValidateGraph(DeBruijnGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }
        }

        /// <summary>
        /// Build graph nodes and edges from list of k-mers.
        /// Creates a node for every unique k-mer (and reverse-complement) 
        /// in the read. Then, generates adjacency information between nodes 
        /// by computing pairs of nodes that have overlapping regions 
        /// between node sequences.
        /// </summary>
        /// <param name="sequences">List of input sequences.</param>
        public void Build(IEnumerable<ISequence> sequences)
        {
            if (sequences == null)
                throw new ArgumentNullException("sequences");

            if (KmerLength <= 0)
                throw new ArgumentException("KmerLengthShouldBePositive");

            if (KmerLength > MaxKmerLength)
                throw new ArgumentException("KmerLengthGreaterThan32");

            var kmerDataCollection = new BlockingCollection<DeBruijnNode>();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    IAlphabet alphabet = Alphabets.DNA;

                    HashSet<byte> gapSymbols;
                    alphabet.TryGetGapSymbols(out gapSymbols);

                    // Generate the kmers from the sequences
                    foreach (ISequence sequence in sequences)
                    {
                        // if the sequence alphabet is not of type DNA then ignore it.
                        if (sequence.Alphabet != Alphabets.DNA)
                        {
                            Interlocked.Increment(ref _skippedSequencesCount);
                            Interlocked.Increment(ref _processedSequencesCount);
                            continue;
                        }

                        // if the sequence contains any gap symbols then ignore the sequence.
                        bool skipSequence = false;
                        foreach (byte symbol in gapSymbols)
                        {
                            for (long index = 0; index < sequence.Count; ++index)
                            {
                                if (sequence[index] == symbol)
                                {
                                    skipSequence = true;
                                    break;
                                }
                            }

                            if (skipSequence)
                                break;
                        }

                        if (skipSequence)
                        {
                            Interlocked.Increment(ref _skippedSequencesCount);
                            Interlocked.Increment(ref _processedSequencesCount);
                            continue;
                        }

                        // if the blocking collection count is exceeding 2 million wait for 5 sec 
                        // so that the task can remove some kmers and creat the nodes. 
                        // This will avoid OutofMemoryException
                        while (kmerDataCollection.Count > StopAddThreshold)
                        {
                            Thread.Sleep(5);
                        }

                        // Generate the kmers from each sequence
                        long count = sequence.Count;
                        for (long i = 0; i <= count - KmerLength; ++i)
                        {
                            var kmerData = new KmerData32();
                            bool orientation = kmerData.SetKmerData(sequence, i, KmerLength);
                            kmerDataCollection.Add(new DeBruijnNode(kmerData, orientation, 1));
                        }

                        Interlocked.Increment(ref _processedSequencesCount);
                    }
                }
                finally
                {
                    kmerDataCollection.CompleteAdding();
                }
            });

            // The main thread will then process all the data - this will loop until the above
            // task completes adding the kmers.
            foreach (var newNode in kmerDataCollection.GetConsumingEnumerable())
            {
                // Create a new node
                if (Root == null) // first element being added
                {
                    Root = newNode; // set node as root of the tree
                    NodeCount++;
                    continue;
                }

                int result = 0;
                DeBruijnNode temp = Root;
                DeBruijnNode parent = Root;

                // Search the tree where the new node should be inserted
                while (temp != null)
                {
                    result = newNode.NodeValue.CompareTo(temp.NodeValue);
                    if (result == 0)
                    {
                        if (temp.KmerCount <= 255)
                        {
                            temp.KmerCount++;
                            break;
                        }
                    }
                    else if (result > 0) // move to right sub-tree
                    {
                        parent = temp;
                        temp = temp.Right;
                    }
                    else if (result < 0) // move to left sub-tree
                    {
                        parent = temp;
                        temp = temp.Left;
                    }
                }

                // position found
                if (result > 0) // add as right child
                {
                    parent.Right = newNode;
                    NodeCount++;
                }
                else if (result < 0) // add as left child
                {
                    parent.Left = newNode;
                    NodeCount++;
                }
            }

            // Done adding - we can throw away the kmer collection as we now have the graph
            kmerDataCollection.Dispose();
            this.GraphBuildCompleted = true;

            // Generate the links
            this.GenerateLinks();
        }

        /// <summary>
        /// Searches for a particular node in the tree.
        /// </summary>
        /// <param name="kmerValue">The node to be searched.</param>
        /// <returns>Actual node in the tree.</returns>
        public DeBruijnNode SearchTree(KmerData32 kmerValue)
        {
            DeBruijnNode startNode = Root;
            while (startNode != null)
            {
                int result = kmerValue.CompareTo(startNode.NodeValue);
                if (result == 0)  // not found
                    break;

                // Search left if the value is smaller than the current node
                startNode = result < 0 ? startNode.Left : startNode.Right;
            }

            return startNode;
        }

        /// <summary>
        /// Gets the nodes present in this graph.
        /// Nodes marked for delete are not returned.
        /// </summary>
        /// <returns>The list of all available nodes in the graph.</returns>
        public IEnumerable<DeBruijnNode> GetNodes()
        {
            var traversalStack = new Stack<DeBruijnNode>();

            traversalStack.Push(Root);
            while (traversalStack.Count > 0)
            {
                DeBruijnNode current = traversalStack.Pop();
                if (current != null)
                {
                    traversalStack.Push(current.Right);
                    traversalStack.Push(current.Left);
                    if (!current.IsDeleted)
                        yield return current;
                }
            }

            traversalStack.TrimExcess();
        }

        /// <summary>
        /// Gets the sequence from the specified node.
        /// </summary>
        /// <param name="node">DeBruijn node.</param>
        /// <returns>Returns an instance of sequence.</returns>
        public ISequence GetNodeSequence(DeBruijnNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            return new Sequence(Alphabets.DNA, node.GetOriginalSymbols(KmerLength));
        }

        /// <summary>
        /// Remove all nodes in input list from graph.
        /// </summary>
        /// <param name="nodes">Nodes to be removed.</param>
        public void RemoveNodes(IEnumerable<DeBruijnNode> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            foreach (DeBruijnNode node in nodes.Where(node => !node.IsDeleted))
            {
                node.IsDeleted = true;
                Interlocked.Decrement(ref _nodeCount);
            }
        }

        /// <summary>
        /// Removes the nodes which are maked for delete.
        /// </summary>
        public int RemoveMarkedNodes()
        {
            int count = 0;
            Parallel.ForEach(GetMarkedNodes(),
                node =>
                {
                    node.IsDeleted = true;
                    Interlocked.Increment(ref count);
                    Interlocked.Decrement(ref _nodeCount);
                });

            return count;
        }

        /// <summary>
        /// Gets the last or first symbol in the node depending on the isForwardDirection flag is true or false.
        /// If the isSameOrientation flag is false then symbol will be taken from the ReverseComplement of the kmer data.
        /// </summary>
        /// <param name="node">DeBruijn node.</param>
        /// <param name="isForwardDirection">Flag to indicate whether the node is in forward direction or not.</param>
        /// <param name="isSameOrientation">Flag to indicate the orientation.</param>
        /// <returns>Byte represnting the symbol.</returns>
        public byte GetNextSymbolFrom(DeBruijnNode node, bool isForwardDirection, bool isSameOrientation)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            byte[] nextSequence = isSameOrientation
                ? node.GetOriginalSymbols(KmerLength)
                : node.GetReverseComplementOfOriginalSymbols(KmerLength);

            return isForwardDirection ? nextSequence.Last() : nextSequence.First();
        }

        /// <summary>
        /// Adds the links between the nodes of the graph.
        /// </summary>
        private void GenerateLinks()
        {
            Parallel.ForEach(GetNodes(),
                node =>
                {
                    DeBruijnNode searchResult;
                    KmerData32 searchNodeValue = new KmerData32();
                    string kmerString, kmerStringRc;
                    if (node.NodeDataOrientation)
                    {
                        kmerString = Encoding.Default.GetString(node.NodeValue.GetKmerData(KmerLength));
                        kmerStringRc = Encoding.Default.GetString(node.NodeValue.GetReverseComplementOfKmerData(KmerLength));
                    }
                    else
                    {
                        kmerStringRc = Encoding.Default.GetString(node.NodeValue.GetKmerData(KmerLength));
                        kmerString = Encoding.Default.GetString(node.NodeValue.GetReverseComplementOfKmerData(KmerLength));
                    }

                    // Right Extensions
                    string nextKmer = kmerString.Substring(1);
                    string nextKmerRC = kmerStringRc.Substring(0, KmerLength - 1);
                    for (int i = 0; i < _dnaSymbols.Length; i++)
                    {
                        string tmpNextKmer = nextKmer + _dnaSymbols[i];
                        searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpNextKmer), KmerLength);
                        searchResult = SearchTree(searchNodeValue);

                        if (searchResult != null)
                        {
                            node.SetExtensionNode(true, searchResult.NodeDataOrientation, searchResult);
                        }
                        else
                        {
                            string tmpnextKmerRC = _dnaSymbolsComplement[i] + nextKmerRC;
                            searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpnextKmerRC), KmerLength);
                            searchResult = SearchTree(searchNodeValue);
                            if (searchResult != null)
                            {
                                node.SetExtensionNode(true, !searchResult.NodeDataOrientation, searchResult);
                            }
                        }
                    }

                    // Left Extensions
                    nextKmer = kmerString.Substring(0, KmerLength - 1);
                    nextKmerRC = kmerStringRc.Substring(1);
                    for (int i = 0; i < _dnaSymbols.Length; i++)
                    {
                        string tmpNextKmer = _dnaSymbols[i] + nextKmer;
                        searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpNextKmer), KmerLength);
                        searchResult = SearchTree(searchNodeValue);
                        if (searchResult != null)
                        {
                            node.SetExtensionNode(false, searchResult.NodeDataOrientation, searchResult);
                        }
                        else
                        {
                            string tmpNextKmerRC = nextKmerRC + _dnaSymbolsComplement[i];
                            searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpNextKmerRC), KmerLength);
                            searchResult = SearchTree(searchNodeValue);
                            if (searchResult != null)
                            {
                                node.SetExtensionNode(false, !searchResult.NodeDataOrientation, searchResult);
                            }
                        }
                    }
                });

            LinkGenerationCompleted = true;
        }

        /// <summary>
        /// Gets the nodes present in this graph.
        /// Nodes marked for delete are not returned.
        /// </summary>
        /// <returns>List of DeBruin node that are maked for deletion.</returns>
        private IEnumerable<DeBruijnNode> GetMarkedNodes()
        {
            var traversalStack = new Stack<DeBruijnNode>();

            traversalStack.Push(Root);
            while (traversalStack.Count > 0)
            {
                DeBruijnNode current = traversalStack.Pop();
                if (current != null)
                {
                    traversalStack.Push(current.Right);
                    traversalStack.Push(current.Left);

                    if (current.IsMarkedForDelete && !current.IsDeleted)
                        yield return current;
                }
            }

            traversalStack.TrimExcess();
        }

        /// <summary>
        /// Change the VisitFlag of all nodes in the graph
        /// </summary>
        /// <param name="stateToSet">Visited or Not?</param>
        public void SetNodeVisitState(bool stateToSet)
        {
            Parallel.ForEach(GetNodes(), node => node.IsVisited = stateToSet);
        }

        /// <summary>
        /// Return all nodes in the array with the visit flag set to false.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeBruijnNode> GetUnvisitedNodes()
        {
            return GetNodes().Where(node => node.IsVisited == false /*& node.IsDeleted == false*/);
        }
    }
}
