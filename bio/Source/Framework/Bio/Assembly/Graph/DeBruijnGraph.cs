using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bio.Algorithms.Kmer;
using Bio.Util;
using System.Globalization;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Representation of a De Bruijn Graph.
    /// Graph is encoded as a collection of de Bruijn nodes.
    /// The nodes themselves hold the adjacency information.
    /// </summary>
    public class DeBruijnGraph 
    {
        /// <summary>
        /// Holds node count.
        /// </summary>
        private long _nodeCount;

        /// <summary>
        /// Collection of nodes in the graph.
        /// </summary>
        private IEnumerable<DeBruijnNode> _nodes;

        /// <summary>
        /// Flag to indicate if the node collection should be compacted
        /// must be set whenever nodes are deleted;
        /// </summary>
        private bool _nodesNeedCompacting;


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
            KmerLength = kmerLength;
           
        }

        /// <summary>
        /// Gets or sets the number of nodes available in the graph.
        /// </summary>
        public long NodeCount
        {
            get
            {
                return _nodeCount;
            }
        }

        /// <summary>
        /// Gets the kmerlength of the graph.
        /// </summary>
        public int KmerLength { get; private set; }

        /// <summary>
        /// Gets number of sequences processed while building the graph.
        /// </summary>
        public long ProcessedSequencesCount
        {
            get
            {
                return _processedSequencesCount;
            }
        }

        /// <summary>
        /// Gets number of sequences skipped from the input sequences.
        /// </summary>
        public long SkippedSequencesCount
        {
            get
            {
                return _skippedSequencesCount;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt64.ToString"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public void Build(IEnumerable<ISequence> sequences)
        {
            // Size of Kmer List to grab, somewhat arbitrary but want to keep list size below large object threshold, which is ~85 kb 
            const int blockSize = 4096;

            // When to add list to blocking collection, most short reads are <=151 bp so this should avoid needing to grow the list
            const int addThreshold = blockSize - 151;

            // When to pause adding
            const int stopAddThreshold = 2000000 / blockSize;

            if (sequences == null)
                throw new ArgumentNullException("sequences");

            if (KmerLength > KmerData32.MAX_KMER_LENGTH)
                throw new ArgumentException(Properties.Resource.KmerLengthGreaterThan31);

            // A dictionary kmers to debruijin nodes
            KmerDictionary kmerManager = new KmerDictionary();

            // Create the producer thread.
            var kmerDataCollection = new BlockingCollection<List<KmerData32>>();
            Task producer = Task.Factory.StartNew(() =>
            {
                try
                {
                    List<KmerData32> kmerList = new List<KmerData32>(blockSize);

                    IAlphabet alphabet = Alphabets.DNA;
                    HashSet<byte> gapSymbols;
                    alphabet.TryGetGapSymbols(out gapSymbols);

                    // Generate the kmers from the sequences
                    foreach (ISequence sequence in sequences)
                    {
                        // if the sequence alphabet is not of type DNA then ignore it.
                        bool skipSequence = false;
                        if (sequence.Alphabet != Alphabets.DNA)
                        {
                            skipSequence = true;
                        }
                        else
                        {
                            // if the sequence contains any gap symbols then ignore the sequence.
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
                        }

                        if (skipSequence)
                        {
                            Interlocked.Increment(ref _skippedSequencesCount);
                            Interlocked.Increment(ref _processedSequencesCount);
                            continue;
                        }

                        // if the blocking collection count is exceeding 2 million kmers wait for 5 sec 
                        // so that the task can remove some kmers and create the nodes. 
                        // This will avoid OutofMemoryException
                        while (kmerDataCollection.Count > stopAddThreshold)
                        {
                            Thread.Sleep(5);
                        }

                        // Convert sequences to k-mers
                        kmerList.AddRange(KmerData32.GetKmers(sequence, KmerLength));

                        // Most reads are <=150 basepairs, so this should avoid having to grow the list
                        // by keeping it below blockSize
                        if (kmerList.Count > addThreshold)
                        {
                            kmerDataCollection.Add(kmerList);
                            kmerList = new List<KmerData32>(4092);
                        }
                        Interlocked.Increment(ref _processedSequencesCount);
                    }

                    if (kmerList.Count <= addThreshold)
                        kmerDataCollection.Add(kmerList);
                }
                finally
                {
                    kmerDataCollection.CompleteAdding();
                }
            });

            // Consume k-mers by addding them to binary tree structure as nodes
            Parallel.ForEach(kmerDataCollection.GetConsumingEnumerable(),newKmerList=>
            {
                foreach (KmerData32 newKmer in newKmerList)
                {
                    // Create Vertex
                    DeBruijnNode node = kmerManager.SetNewOrGetOld(newKmer);

                    // Need to lock node if doing this in parallel
                    if (node.KmerCount <= 255)
                    {
                        lock (node)
                        {
                            node.KmerCount++;
                        }
                    }
                }
            });

            // Ensure producer exceptions are handled.
            producer.Wait();

            // Done filling binary tree
            kmerDataCollection.Dispose();

            //NOTE: To speed enumeration make the nodes into an array and dispose of the collection
            _nodeCount = kmerManager.NodeCount;
            _nodes = kmerManager.GenerateNodeArray();
            
            // Generate the links
            GenerateLinks(kmerManager);
            
            // Since we no longer need to search for values set left and right nodes of child array to null
            // so that they are available for GC if no longer needed
            foreach (DeBruijnNode node in _nodes)
            {
                node.Left = node.Right = null;
            }

            GraphBuildCompleted = true;
        }
        
        /// <summary>
        /// Gets the nodes present in this graph.
        /// Nodes marked for delete are not returned.
        /// </summary>
        /// <returns>The list of all available nodes in the graph.</returns>
        public IEnumerable<DeBruijnNode> GetNodes()
        {
            //Removing deleted nodes and then returning array
            if (_nodesNeedCompacting)
            {
                RemoveDeletedNodesFromArray();
            }
            return _nodes;
        }
        /// <summary>
        /// Return all nodes in the array with the visit flag set to false.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<DeBruijnNode> GetUnvisitedNodes()
        {
            return _nodes.Where(node => node.IsVisited == false && node.IsDeleted==false);
        }

        /// <summary>
        /// Change the VisitFlag of all nodes in the graph
        /// </summary>
        /// <param name="stateToSet">Visited or Not?</param>
        public void SetNodeVisitState(bool stateToSet)
        {
            Parallel.ForEach(_nodes, node => node.IsVisited = stateToSet);
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

            return new Sequence(Alphabets.DNA, node.GetOriginalSymbols(KmerLength));
        }

        /// <summary>
        /// Remove all nodes in input list from graph.
        /// </summary>
        /// <param name="nodesToRemove">Nodes to be removed.</param>
        public void RemoveNodes(IEnumerable<DeBruijnNode> nodesToRemove)
        {
            if (nodesToRemove == null)
            {
                throw new ArgumentNullException("nodesToRemove");
            }
            long oldCount = _nodeCount;
            foreach (DeBruijnNode node in nodesToRemove)
            {
                if (!node.IsDeleted)
                {
                    node.IsDeleted = true;
                    Interlocked.Decrement(ref _nodeCount);
                }
            }
            if (oldCount != _nodeCount)
            {
                _nodesNeedCompacting = true;
            }
        }

        /// <summary>
        /// Removes the nodes which are marked for delete.
        /// </summary>
        public long RemoveMarkedNodes()
        {
            long count = 0;
            Parallel.ForEach(GetNodes(), node =>
                {
                    if (node.IsMarkedForDelete && !node.IsDeleted)
                    {
                        node.IsDeleted = true;
                        Interlocked.Increment(ref count);
                        Interlocked.Decrement(ref _nodeCount);
                    }
                });

            if (count > 0)
                _nodesNeedCompacting = true;

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
            
            return isForwardDirection 
                ? nextSequence.Last() 
                : nextSequence.First();
        }
        
        /// <summary>
        /// Adds the links between the nodes of the graph.
        /// </summary>
        private void GenerateLinks(KmerDictionary kmerManager)
        {
            // Prepare a mask to remove the bits representing the first nucleotide (or left most bits in the encoded kmer)
            // First calculate how many bits do you have to move down a character until you are at the start of the kmer encoded sequence
            int distancetoShift=2*(KmerLength-1);
            ulong rightMask = ~( ((ulong)3) << distancetoShift);
            Parallel.ForEach(_nodes, node =>
                {
                    DeBruijnNode searchResult = null;
                    KmerData32 searchNodeValue = new KmerData32();
                    
                    // Right Extensions - Remove first position from the value
                    // Remove the left most value by using an exclusive 
                    ulong nextKmer = node.NodeValue.KmerData & rightMask;
                    
                    // Move it over two to get make a position for the next pair of bits to represent a new nucleotide
                    nextKmer= nextKmer << 2;
                    for (ulong i = 0; i < 4; i++)
                    {
                        ulong tmpNextKmer = nextKmer | i;// Equivalent to "ACGTA"+"N" where N is the 0-3 encoding for A,C,G,T
                        
                        // Now to set the kmer value to this, the orientationForward value is equal to false if the 
                        // reverse compliment of the kmer is used instead of the kmer value itself.
                        bool matchIsRC = searchNodeValue.SetKmerData(tmpNextKmer, KmerLength);
                        searchResult = kmerManager.TryGetOld(searchNodeValue);
                        if (searchResult != null)
                        {
                            node.SetExtensionNode(true, matchIsRC, searchResult);
                        }
                    }

                    // Left Extensions
                    nextKmer = node.NodeValue.KmerData;
                    
                    //Chop off the right most basepair
                    nextKmer >>= 2;
                    for (ulong i = 0; i < 4; i++) // Cycle through A,C,G,T
                    {
                        // Add the character on to the left side of the kmer
                        // Equivalent to "N" + "ACGAT" where the basepair is added on as the 2 bits
                        ulong tmpNextKmer = (i<<distancetoShift) | nextKmer; 
                        bool matchIsRC=searchNodeValue.SetKmerData(tmpNextKmer, KmerLength);
                        searchResult = kmerManager.TryGetOld(searchNodeValue);
                        if (searchResult != null)
                        {
                            node.SetExtensionNode(false, matchIsRC, searchResult);
                        }
                    }
                });

            LinkGenerationCompleted = true;
        }
       
        /// <summary>
        /// Cleans out the deleted nodes from the array
        /// </summary>
        private void RemoveDeletedNodesFromArray()
        {
            // Compacts the node array by replacing all positions with deleted nodes early in the list with nodes from the end of the array.
            if (_nodes is List<DeBruijnNode>)
            {
                CompactDeletedNodesFromList();
            }
            else if (_nodes is BigList<DeBruijnNode>)
            {
                CompactDeletedNodesFromBigList();
            }
            else
            {
                throw new Exception("Unknown Node Array Type");
            }
            _nodesNeedCompacting = false;
        }

        /// <summary>
        /// Compact the node list by removing deleted nodes
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private void CompactDeletedNodesFromList()
        {
            //start 3 threads, one to find indexes to fill, one to find things to fill them with, and one to do the filling
            var lnodes = _nodes as List<DeBruijnNode>;
            if (lnodes == null)
            {
                throw new NullReferenceException("Tried to use node collection as list when it was null or another type");
            }
            
            var deletedFrontIndexes = new BlockingCollection<int>();
            int spotsToFind = lnodes.Count - (int) _nodeCount;
            int emptySpotsFound = 0;
            
            // Task to find empty spots in top of list
            Task findEmptyFrontIndexes = Task.Factory.StartNew(() =>
            {
                try
                {
                    for (int curForward = 0; curForward < _nodeCount && emptySpotsFound != spotsToFind; curForward++)
                    {
                        DeBruijnNode cnode = lnodes[curForward];
                        if (cnode.IsDeleted)
                        {
                            deletedFrontIndexes.Add(curForward);
                            emptySpotsFound++;
                        }
                    }
                }
                finally
                {
                    deletedFrontIndexes.CompleteAdding();
                }
            });

            // Task to find undeleted nodes in back of list
            var undeletedBackNodes = new BlockingCollection<DeBruijnNode>();
            int filledSpotsFound = 0;

            Task findFullBackIndexes = Task.Factory.StartNew(() =>
            {
                try
                {
                    for (int curBackward = (lnodes.Count - 1);
                         curBackward >= _nodeCount && filledSpotsFound != spotsToFind;
                         curBackward--)
                    {
                        DeBruijnNode cnode = lnodes[curBackward];
                        if (!cnode.IsDeleted)
                        {
                            undeletedBackNodes.Add(cnode);
                            filledSpotsFound++;
                        }
                    }
                }
                finally
                {
                    undeletedBackNodes.CompleteAdding();
                }

                // Wait for the prior task to finish.
                findEmptyFrontIndexes.Wait();

                // This will prevent the program from hanging if a bad area is found in the code so that there is nothing to fill an index
                if (emptySpotsFound != filledSpotsFound)
                {
                    throw new ApplicationException("The node array in the graph has become corrupted, node count does not match the number of undeleted nodes");
                }
            });

            // Task to move things that have been found in the back to the front
            Task moveNodes = Task.Factory.StartNew(() =>
            {
                // The logic here requires that the items missing in the front match those in the back
                while (!deletedFrontIndexes.IsCompleted && !undeletedBackNodes.IsCompleted)
                {
                    DeBruijnNode tm = undeletedBackNodes.Take();
                    if (tm == null)
                        throw new NullReferenceException("Cannot move null node!");

                    int index = deletedFrontIndexes.Take();
                    lnodes[index] = tm;
                }
            });

            Task.WaitAll(new[] { findEmptyFrontIndexes, findFullBackIndexes, moveNodes });
            
            // Now the tail should only be deleted nodes and nodes that have been copied further up in the list
            lnodes.RemoveRange((int)_nodeCount, lnodes.Count - (int)_nodeCount);
        }

        /// <summary>
        /// Compact the node list by removing deleted nodes
        /// </summary>
        private void CompactDeletedNodesFromBigList()
        {
            //NOTE: Same method as CompactDeletedNodesFromList but using long instead of int
            //start 3 threads, one to find indexes to fill, one to find things to fill them with, and one to do the filling
            var lnodes = _nodes as BigList<DeBruijnNode>;
            if (lnodes == null)
            {
                throw new NullReferenceException("Tried to use node collection as list when it was null or another type");
            }

            BlockingCollection<long> deletedFrontIndexes = new BlockingCollection<long>();
            BlockingCollection<DeBruijnNode> undeletedBackNodes = new BlockingCollection<DeBruijnNode>();
            //task to find empty spots in top of list
            long emptySpotsFound = 0;
            Task findEmptyFrontIndexes = Task.Factory.StartNew(() =>
            {
                Thread.BeginCriticalRegion();
                for (long curForward = 0; curForward < _nodeCount; curForward++)
                {
                    DeBruijnNode cnode = lnodes[curForward];
                    if (cnode.IsDeleted)
                    {
                        deletedFrontIndexes.Add(curForward);
                        emptySpotsFound++;
                    }
                }
                deletedFrontIndexes.CompleteAdding();
                Thread.EndCriticalRegion();
            });
            //task to find undeleted nodes in back of list
            long filledSpotsFound = 0;
            Task findFullBackIndexes = Task.Factory.StartNew(() =>
            {
                Thread.BeginCriticalRegion();
                for (long curBackward = (lnodes.Count - 1); curBackward >= _nodeCount; curBackward--)
                {
                    DeBruijnNode cnode = lnodes[curBackward];
                    if (!cnode.IsDeleted)
                    {
                        undeletedBackNodes.Add(cnode);
                        filledSpotsFound++;
                    }
                }
                undeletedBackNodes.CompleteAdding();
                findEmptyFrontIndexes.Wait();
                //This will prevent the program from hanging if a bad area is found in the code so that there is nothing to fill an index
                if (emptySpotsFound != filledSpotsFound)
                {
                    throw new NullReferenceException("The node array in the graph has become corrupted, node count does not match the number of undeleted nodes");
                }
                Thread.EndCriticalRegion();
            });
            //task to move things that have been found in the back to the front
            Task moveNodes = Task.Factory.StartNew(() =>
            {
                Thread.BeginCriticalRegion();
                //the logic here requires that the items missing in the front match those in the back
                while (!deletedFrontIndexes.IsCompleted)
                {
                    DeBruijnNode tm; long index;
                    undeletedBackNodes.TryTake(out tm, -1);
                    deletedFrontIndexes.TryTake(out index, -1);
                    lnodes[index] = tm;
                }
            });
            Task.WaitAll(new Task[] { findEmptyFrontIndexes, findFullBackIndexes, moveNodes });
            //now the tail should only be deleted nodes and nodes that have been copied further up in the list
            lnodes.TrimToSize(_nodeCount);
        }

        /// <summary>
        /// Gets the nodes present in this graph.
        /// Nodes marked for delete are not returned.
        /// </summary>
        /// <returns>List of DeBruin node that are maked for deletion.</returns>
        public IEnumerable<DeBruijnNode> GetMarkedNodes()
        {
            return GetNodes().Where(node => node.IsMarkedForDelete);
        }
    }
}
