using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bio.Algorithms.Kmer;
using Bio.Util;
using System.Diagnostics;
using System.Globalization;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Representation of a De Bruijn Graph.
    /// Graph is encoded as a collection of de Bruijn nodes.
    /// The nodes themselves hold the adjacency information.
    /// </summary>
    public class DeBruijnGraph : Graph<DeBruijnNode, object>
    {
        //Note: DeBruijnGraph is not using Edge data structure of the Graph. It is using Graph to hold the DeBruijnNodes.
        // DebruijnNode it self holds the reference to other nodes.
        //TODO: Use the  Edge and store the data to link two DeBruijnNode in the edge 
        // this requires to Modify DeBuijnNode and DeBruijnGraph and other rquired places.

        /// <summary>
        /// Holds dna symbols.
        /// </summary>
        private readonly char[] DnaSymbols = new char[] { 'A', 'T', 'G', 'C' };

        /// <summary>
        /// Holds complement dna symbols.
        /// </summary>
        private readonly char[] DnaSymbolsComplement = new char[] { 'T', 'A', 'C', 'G' };

        /// <summary>
        /// Holds node count.
        /// </summary>
        private long nodeCount = 0;

        private LongSerialNumbers<IKmerData> kmerManager;

        /// <summary>
        /// Holds kmer length.
        /// </summary>
        private int kmerLength;

        /// <summary>
        /// Holds the number of input sequences processed while building graph.
        /// </summary>
        private long processedSequencesCount;

        /// <summary>
        /// Holds number of sequences skipped while building graph.
        /// </summary>
        private long skippedSequencesCount;

        /// <summary>
        /// Initializes a new instance of the DeBruijnGraph class.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        public DeBruijnGraph(int kmerLength)
        {
            this.kmerLength = kmerLength;
            kmerManager = new LongSerialNumbers<IKmerData>();
        }

        /// <summary>
        /// Gets or sets the number of nodes available in the graph.
        /// </summary>
        public long NodeCount
        {
            get
            {
                return this.nodeCount;
            }

            set
            {
                this.nodeCount = value;
            }
        }

        /// <summary>
        /// Gets the kmerlength of the graph.
        /// </summary>
        public int KmerLength
        {
            get { return this.kmerLength; }
        }

        /// <summary>
        /// Gets number of sequences processed while building the graph.
        /// </summary>
        public long ProcessedSequencesCount
        {
            get
            {
                return this.processedSequencesCount;
            }
        }

        /// <summary>
        /// Gets number of sequences skipped from the input sequences.
        /// </summary>
        public long SkippedSequencesCount
        {
            get
            {
                return this.skippedSequencesCount;
            }
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
            {
                throw new ArgumentNullException("sequences");
            }

            if (this.kmerLength <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            if (this.kmerLength > 32)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthGreaterThan32);
            }

            BlockingCollection<DeBruijnNode> kmerDataCollection = new BlockingCollection<DeBruijnNode>();

            Task createKmers = Task.Factory.StartNew(() =>
            {
                Thread.BeginCriticalRegion();
                IAlphabet alphabet = Alphabets.DNA;

                HashSet<byte> gapSymbols;
                alphabet.TryGetGapSymbols(out gapSymbols);

                // Generate the kmers from the sequences
                foreach (ISequence sequence in sequences)
                {
                    // if the sequence alphabet is not of type DNA then ignore it.
                    if (sequence.Alphabet != Alphabets.DNA)
                    {
                        Interlocked.Increment(ref this.skippedSequencesCount);
                        Interlocked.Increment(ref this.processedSequencesCount);
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
                        {
                            break;
                        }
                    }

                    if (skipSequence)
                    {
                        Interlocked.Increment(ref this.skippedSequencesCount);
                        Interlocked.Increment(ref this.processedSequencesCount);
                        continue;
                    }

                    // if the blocking collection count is exceeding 2 million wait for 5 sec 
                    // so that the task can remove some kmers and creat the nodes. 
                    // This will avoid OutofMemoryException
                    while (kmerDataCollection.Count > 2000000)
                    {
                        System.Threading.Thread.Sleep(5);
                    }

                    long count = sequence.Count;

                    // generate the kmers from each sequence
                    for (long i = 0; i <= count - this.kmerLength; ++i)
                    {
                        IKmerData kmerData = this.GetNewKmerData();
                        bool orientation = kmerData.SetKmerData(sequence, i, this.kmerLength);
                        kmerDataCollection.Add(new DeBruijnNode(kmerData, orientation, 1));
                    }

                    Interlocked.Increment(ref this.processedSequencesCount);
                }

                kmerDataCollection.CompleteAdding();
                Thread.EndCriticalRegion();
            });

            Task buildKmers = Task.Factory.StartNew(() =>
            {
                Thread.BeginCriticalRegion();
                while (!kmerDataCollection.IsCompleted)
                {
                    DeBruijnNode newNode = null;
                    if (kmerDataCollection.TryTake(out newNode, -1))
                    {
                        // Create Vertex
                        long newVertexId = base.TotalVertexCount;
                        long kmerId = this.kmerManager.GetNewOrOld(newNode.NodeValue);
                        if (kmerId == newVertexId)
                        {
                            // new kmer, Add vertex
                            base.AddVertex(newNode);
                            NodeCount++;
                        }
                        else if (kmerId < newVertexId)
                        {
                            // Kmer already exists.
                            var vertex = base.GetVertex(kmerId);
                            if (vertex == null)
                            {
                                throw new Exception(string.Format(CultureInfo.CurrentCulture, "Unexpected Error, Null vertex found for vertex id: {0}", kmerId));
                            }

                            if (vertex.Data.NodeValue.CompareTo(newNode.NodeValue) != 0)
                            {
                                throw new Exception(string.Format(CultureInfo.CurrentCulture,
                                  "Vertex Id should be sync with Kmer Id but Vertex Id {0} contains Kmer: {1} instead of kmer: {2}",
                                  vertex.Id, vertex.Data.NodeValue.ToString(), newNode.NodeValue.ToString()));
                            }

                            if (vertex.Data.KmerCount <= 255)
                            {
                                vertex.Data.KmerCount++;
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format(CultureInfo.CurrentCulture,
                               "Vertex Id should be sync with Kmer Id, but the newKmer id: {0} and new Vertex Id:{1}",
                               kmerId, newVertexId));
                        }

                    } // End of tree node creation.
                }

                Thread.EndCriticalRegion();
            });

            Task.WaitAll(createKmers, buildKmers);

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
        public DeBruijnNode SearchTree(IKmerData kmerValue)
        {
            long kmerId;
            DeBruijnNode node = null;
            if (kmerValue != null && this.kmerManager.TryGetOld(kmerValue, out kmerId))
            {
                node = base.GetVertex(kmerId).Data;
            }

            return node;
        }

        /// <summary>
        /// Gets the nodes present in this graph.
        /// Nodes marked for delete are not returned.
        /// </summary>
        /// <returns>The list of all available nodes in the graph.</returns>
        public IEnumerable<DeBruijnNode> GetNodes()
        {
            foreach (var vertex in base.GetVertices())
            {
                DeBruijnNode node = vertex.Data;
                if (node != null && !node.IsDeleted)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Gets the sequence from the specified node.
        /// </summary>
        /// <param name="node">DeBruijn node.</param>
        /// <returns>Returns an instance of sequence.</returns>
        public ISequence GetNodeSequence(DeBruijnNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return new Sequence(Alphabets.DNA, node.GetOriginalSymbols(this.kmerLength));
        }

        /// <summary>
        /// Remove all nodes in input list from graph.
        /// </summary>
        /// <param name="nodes">Nodes to be removed.</param>
        public void RemoveNodes(IEnumerable<DeBruijnNode> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            foreach (DeBruijnNode node in nodes)
            {
                if (!node.IsDeleted)
                {
                    node.IsDeleted = true;
                    Interlocked.Decrement(ref this.nodeCount);
                }
            }
        }

        /// <summary>
        /// Removes the nodes which are maked for delete.
        /// </summary>
        public int RemoveMarkedNodes()
        {
            int count = 0;
            Parallel.ForEach(
                this.GetMarkedNodes(),
                (node) =>
                {
                    node.IsDeleted = true;
                    Interlocked.Increment(ref count);
                    Interlocked.Decrement(ref this.nodeCount);
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
            {
                throw new ArgumentNullException("node");
            }

            byte[] nextSequence = isSameOrientation ? node.GetOriginalSymbols(this.kmerLength) : node.GetReverseComplementOfOriginalSymbols(this.kmerLength);

            if (isForwardDirection)
            {
                return nextSequence.Last();
            }
            else
            {
                return nextSequence.First();
            }
        }

        /// <summary>
        /// Gets outgoing Vertices of a given vertex.
        /// </summary>
        /// <param name="vertex">Vertex</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<Vertex<DeBruijnNode>> GetOutgoingVertices(Vertex<DeBruijnNode> vertex)
        {
            for (int i = 0; i < vertex.OutgoingEdgeCount; i++)
            {
                var edge = this.GetEdge(vertex.GetOutgoingEdge(i));
                yield return this.GetVertex(edge.VertexId2);
            }
        }

        /// <summary>
        /// Gets incoming Vertices of a given vertex.
        /// </summary>
        /// <param name="vertex">Vertex</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<Vertex<DeBruijnNode>> GetIncomingVertices(Vertex<DeBruijnNode> vertex)
        {
            for (int i = 0; i < vertex.IncomingEdgeCount; i++)
            {
                var edge = this.GetEdge(vertex.GetIncomingEdge(i));
                yield return this.GetVertex(edge.VertexId1);
            }
        }

        /// <summary>
        /// Gets the new instance of KmerData depending on the kmerLength.
        /// </summary>
        /// <returns>Returns a new instance of KmerData.</returns>
        private IKmerData GetNewKmerData()
        {
            if (this.kmerLength <= 32)
            {
                return new KmerData32();
            }

            throw new ArgumentException("Kmerlength more than 32 is not supported");
        }

        /// <summary>
        /// Adds the links between the nodes of the graph.
        /// </summary>
        private void GenerateLinks()
        {
            Parallel.ForEach(
                this.GetNodes(),
                node =>
                {
                    DeBruijnNode searchResult = null;
                    IKmerData searchNodeValue = GetNewKmerData();
                    string kmerString;
                    string kmerStringRC;
                    if (node.NodeDataOrientation)
                    {
                        kmerString = Encoding.Default.GetString(node.NodeValue.GetKmerData(this.kmerLength));
                        kmerStringRC = Encoding.Default.GetString(node.NodeValue.GetReverseComplementOfKmerData(this.KmerLength));
                    }
                    else
                    {
                        kmerStringRC = Encoding.Default.GetString(node.NodeValue.GetKmerData(this.kmerLength));
                        kmerString = Encoding.Default.GetString(node.NodeValue.GetReverseComplementOfKmerData(this.KmerLength));
                    }

                    string nextKmer;
                    string nextKmerRC;

                    // Right Extensions
                    nextKmer = kmerString.Substring(1);
                    nextKmerRC = kmerStringRC.Substring(0, kmerLength - 1);
                    for (int i = 0; i < DnaSymbols.Length; i++)
                    {
                        string tmpNextKmer = nextKmer + DnaSymbols[i];
                        searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpNextKmer), this.kmerLength);
                        searchResult = this.SearchTree(searchNodeValue);
                        if (searchResult != null)
                        {
                            node.SetExtensionNodes(true, searchResult.NodeDataOrientation, searchResult);
                        }
                        else
                        {
                            string tmpnextKmerRC = DnaSymbolsComplement[i] + nextKmerRC;
                            searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpnextKmerRC), this.kmerLength);
                            searchResult = this.SearchTree(searchNodeValue);
                            if (searchResult != null)
                            {
                                node.SetExtensionNodes(true, !searchResult.NodeDataOrientation, searchResult);
                            }
                        }
                    }

                    // Left Extensions
                    nextKmer = kmerString.Substring(0, kmerLength - 1);
                    nextKmerRC = kmerStringRC.Substring(1);
                    for (int i = 0; i < DnaSymbols.Length; i++)
                    {
                        string tmpNextKmer = DnaSymbols[i] + nextKmer;
                        searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpNextKmer), this.kmerLength);
                        searchResult = this.SearchTree(searchNodeValue);
                        if (searchResult != null)
                        {
                            node.SetExtensionNodes(false, searchResult.NodeDataOrientation, searchResult);
                        }
                        else
                        {
                            string tmpNextKmerRC = nextKmerRC + DnaSymbolsComplement[i];
                            searchNodeValue.SetKmerData(Encoding.Default.GetBytes(tmpNextKmerRC), this.kmerLength);
                            searchResult = this.SearchTree(searchNodeValue);
                            if (searchResult != null)
                            {
                                node.SetExtensionNodes(false, !searchResult.NodeDataOrientation, searchResult);
                            }
                        }
                    }
                });

            this.LinkGenerationCompleted = true;
        }

        /// <summary>
        /// Gets the nodes present in this graph.
        /// Nodes marked for delete are not returned.
        /// </summary>
        /// <returns>List of DeBruin node that are maked for deletion.</returns>
        private IEnumerable<DeBruijnNode> GetMarkedNodes()
        {
            foreach (var node in this.GetNodes())
            {
                if (node.IsMarkedForDelete)
                {
                    yield return node;
                }
            }
        }
    }
}
