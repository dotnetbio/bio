using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph
{
    /// <summary>
    /// Representation of a De Bruijn Graph.
    /// Graph is encoded as a collection of de Bruijn nodes.
    /// The nodes themselves hold the adjacency information.
    /// </summary>
    public class ContigGraph : IDisposable
    {
        /// <summary>
        /// Base sequence that holds the list of input sequences.
        /// Nodes reference into base sequence for k-mers.
        /// </summary>
        private IList<ISequence> baseSequences;

        /// <summary>
        /// List of graph nodes.
        /// </summary>
        private HashSet<Node> kmerNodes;

        /// <summary>
        /// Gets the list of nodes in graph.
        /// </summary>
        public HashSet<Node> Nodes
        {
            get { return this.kmerNodes; }
        }

        /// <summary>
        /// Validate input graph.
        /// Throws exception if graph is null.
        /// </summary>
        /// <param name="graph">Input graph.</param>
        public static void ValidateGraph(ContigGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }
        }

        /// <summary>
        /// Gets the sequence for kmer associated with input node.
        /// Uses index and position information along with base sequence 
        /// to construct sequence. 
        /// There should be atleast one valid position in the node.
        /// Since all positions indicate the same kmer sequence, 
        /// the position information from the first kmer is used
        /// to construct the sequence.
        /// </summary>
        /// <param name="node">Graph Node.</param>
        /// <returns>Sequence associated with input node.</returns>
        public ISequence GetNodeSequence(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            // Get sequence index and validate.
            int sequenceIndex = node.SequenceIndex;
            if (sequenceIndex < 0 || sequenceIndex >= this.baseSequences.Count)
            {
                throw new ArgumentOutOfRangeException("node", Properties.Resource.KmerIndexOutOfRange);
            }

            // Get base sequence, position and validate.
            return this.baseSequences[sequenceIndex];
        }

        /// <summary>
        /// Builds a contig graph from kmer graph using contig data information.
        /// Creates a graph node for each contig, computes adjacency 
        /// for contig graph using edge information in kmer graph.
        /// Finally, all kmer nodes are deleted from the graph.
        /// </summary>
        /// <param name="contigs">List of contig data.</param>
        /// <param name="kmerLength">Kmer length.</param>
        public void BuildContigGraph(IList<ISequence> contigs, int kmerLength)
        {
            if (contigs == null)
            {
                throw new ArgumentNullException("contigs");
            }

            if (kmerLength <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            // Create contig nodes
            Node[] contigNodes = new Node[contigs.Count()];
            Parallel.For(0, contigs.Count, (int ndx) => contigNodes[ndx] = new Node(contigs[ndx].Count, ndx));

            GenerateContigAdjacency(contigs, kmerLength, contigNodes);

            // Update graph with new nodes
            this.baseSequences = new List<ISequence>(contigs);
            this.kmerNodes = new HashSet<Node>(contigNodes);
        }

        /// <summary>
        /// Remove all nodes in input list from graph.
        /// </summary>
        /// <param name="nodes">Nodes to be removed.</param>
        public void RemoveNodes(IEnumerable<Node> nodes)
        {
            this.kmerNodes.ExceptWith(nodes);
        }

        /// <summary>
        /// Implements dispose to supress GC finalize
        /// This is done as one of the methods uses ReadWriterLockSlim
        /// which extends IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose field instances.
        /// </summary>
        /// <param name="disposeManaged">If disposeManaged equals true, clean all resources.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.kmerNodes = null;
                this.baseSequences = null;
            }
        }

        /// <summary>
        /// Generate adjacency information between contig nodes
        /// by computing overlapping regions between contig sequences.
        /// </summary>
        /// <param name="contigs">List of contig data.</param>
        /// <param name="kmerLength">Kmer length.</param>
        /// <param name="contigNodes">Array of contig nodes.</param>
        private static void GenerateContigAdjacency(IList<ISequence> contigs, long kmerLength, Node[] contigNodes)
        {
            // Create dictionaries that map (k-1) left and right substrings of contigs to contig indexes.
            Dictionary<ISequence, List<int>> leftKmerMap = new Dictionary<ISequence, List<int>>(new SequenceEqualityComparer());
            Dictionary<ISequence, List<int>> rightKmerMap = new Dictionary<ISequence, List<int>>(new SequenceEqualityComparer());
            Parallel.For( 
                0, 
                contigs.Count, 
                ndx =>
            {
                ISequence contig = contigs[ndx];
                List<int> contigIndexes;
                ISequence kmer;

                if (contig.Count < kmerLength)
                {
                    throw new ArgumentException(Properties.Resource.KmerLengthIsTooLong);
                }

                // update left map
                kmer = contig.GetSubSequence(0, kmerLength - 1);
                lock (leftKmerMap)
                {
                    if (!leftKmerMap.TryGetValue(kmer, out contigIndexes))
                    {
                        contigIndexes = new List<int>();
                        leftKmerMap.Add(kmer, contigIndexes);
                    }
                }

                lock (contigIndexes)
                {
                    contigIndexes.Add(ndx);
                }

                // Update right map
                kmer = contig.GetSubSequence(contig.Count - (kmerLength - 1), kmerLength - 1);
                lock (rightKmerMap)
                {
                    if (!rightKmerMap.TryGetValue(kmer, out contigIndexes))
                    {
                        contigIndexes = new List<int>();
                        rightKmerMap.Add(kmer, contigIndexes);
                    }
                }

                lock (contigIndexes)
                {
                    contigIndexes.Add(ndx);
                }
            });

            AddContigGraphEdges(contigNodes, leftKmerMap, rightKmerMap);
        }

        /// <summary>
        /// Checks for and adds edges between contigs 
        /// based on left, right kmer maps.
        /// </summary>
        /// <param name="contigNodes">Array of contig nodes.</param>
        /// <param name="leftKmerMap">Map of left k-mer to contig nodes.</param>
        /// <param name="rightKmerMap">Map of right k-mer to contig nodes.</param>
        private static void AddContigGraphEdges(
            Node[] contigNodes,
            Dictionary<ISequence, List<int>> leftKmerMap,
            Dictionary<ISequence, List<int>> rightKmerMap)
        {
            // Check and add left extensions. No locks used here since each iteration works with a different contigNode.
            Parallel.ForEach(
                leftKmerMap,
                leftKmer =>
            {
                List<int> positions;
                if (rightKmerMap.TryGetValue(leftKmer.Key, out positions))
                {
                    foreach (int leftNodeIndex in leftKmer.Value)
                    {
                        foreach (int rightNodeIndex in positions)
                        {
                            contigNodes[leftNodeIndex].AddLeftEndExtension(contigNodes[rightNodeIndex], true);
                        }
                    }
                }

                if (leftKmerMap.TryGetValue(leftKmer.Key.GetReverseComplementedSequence(), out positions))
                {
                    foreach (int leftNodeIndex in leftKmer.Value)
                    {
                        foreach (int rightNodeIndex in positions)
                        {
                            contigNodes[leftNodeIndex].AddLeftEndExtension(contigNodes[rightNodeIndex], false);
                        }
                    }
                }
            });

            // Check and add right extensions. No locks used here since each iteration works with a different contigNode.
            Parallel.ForEach(
            rightKmerMap,
            rightKmer =>
            {
                List<int> positions;
                if (leftKmerMap.TryGetValue(rightKmer.Key, out positions))
                {
                    foreach (int rightNodeIndex in rightKmer.Value)
                    {
                        foreach (int leftNodeIndex in positions)
                        {
                            contigNodes[rightNodeIndex].AddRightEndExtension(contigNodes[leftNodeIndex], true);
                        }
                    }
                }

                if (rightKmerMap.TryGetValue(rightKmer.Key.GetReverseComplementedSequence(), out positions))
                {
                    foreach (int rightNodeIndex in rightKmer.Value)
                    {
                        foreach (int leftNodeIndex in positions)
                        {
                            contigNodes[rightNodeIndex].AddRightEndExtension(contigNodes[leftNodeIndex], false);
                        }
                    }
                }
            });
        }
    }
}
