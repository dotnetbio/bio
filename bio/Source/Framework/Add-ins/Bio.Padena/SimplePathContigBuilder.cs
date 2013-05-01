using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio.Algorithms.Assembly.Graph;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Class implements algorithm for extracting contig sequences from de 
    /// bruijn graph. It detects simple paths in graph, and for each simple 
    /// path in the graph, it generates corresponding sequence as a contig.
    /// </summary>
    public class SimplePathContigBuilder : IContigBuilder, ILowCoverageContigPurger
    {
        /// <summary>
        /// Holds reference to assembler graph.
        /// </summary>
        private DeBruijnGraph _graph;

        /// <summary>
        /// Holds value of the coverage threshold to be
        /// used for filtering contigs.
        /// </summary>
        private double _coverageThreshold = Double.NaN;

        /// <summary>
        /// Build contigs from graph. For contigs whose coverage is less than 
        /// the specified threshold, remove graph nodes belonging to them.
        /// </summary>
        /// <param name="deBruijnGraph">DeBruijn Graph.</param>
        /// <param name="coverageThresholdForContigs">Coverage Threshold for contigs.</param>
        /// <returns>Number of nodes removed.</returns>
        public long RemoveLowCoverageContigs(DeBruijnGraph deBruijnGraph, double coverageThresholdForContigs)
        {
            if (deBruijnGraph == null)
            {
                throw new ArgumentNullException("deBruijnGraph");
            }

            if (coverageThresholdForContigs <= 0)
            {
                throw new ArgumentException("For removing low coverage contigs, coverage threshold should be a positive number");
            }

            this._coverageThreshold = coverageThresholdForContigs;
            this._graph = deBruijnGraph;
            DeBruijnGraph.ValidateGraph(deBruijnGraph);
            this.ExcludeAmbiguousExtensions();

            Parallel.ForEach(deBruijnGraph.GetNodes(), n => n.ComputeValidExtensions());
            this.GetSimplePaths(false);
            Parallel.ForEach(deBruijnGraph.GetNodes(), n => n.UndoAmbiguousExtensions());
            return deBruijnGraph.RemoveMarkedNodes();
        }

        /// <summary>
        /// Build contig sequences from the graph.
        /// </summary>
        /// <param name="deBruijnGraph">De Bruijn graph.</param>
        /// <returns>List of contig data.</returns>
        public IEnumerable<ISequence> Build(DeBruijnGraph deBruijnGraph)
        {
            if (deBruijnGraph == null)
            {
                throw new ArgumentNullException("deBruijnGraph");
            }

            this._graph = deBruijnGraph;
            this._coverageThreshold = Double.NaN;
            DeBruijnGraph.ValidateGraph(deBruijnGraph);
            this.ExcludeAmbiguousExtensions();
            deBruijnGraph.GetNodes().AsParallel().ForAll(n => n.PurgeInvalidExtensions());
            return this.GetSimplePaths(true);
        }

        /// <summary>
        /// For nodes that have more than one extension in either direction,
        /// mark the extensions invalid. For nodes that have palidromic sequence, 
        /// all extensions are marked invalid. This is because for a palidromic sequence, 
        /// left and right extensions are inter-changable and this causes ambiguity.
        /// Locks: No locks used as extensions are only marked invalid, not deleted.
        /// Write locks not used because in only possible conflict both threads will 
        /// try to write same value to memory. So race is harmless.
        /// </summary>
        private void ExcludeAmbiguousExtensions()
        {
            Parallel.ForEach(this._graph.GetNodes(), node =>
                {
                    // Palindromes cause small cycles in the graph. Such reference cycles will 
                    // be skipped by the contig extension algorithm. Hence, in order to terminate 
                    // contig extension at these points, we remove extensions from palindromic nodes.
                    // Reference: ABySS Release Notes 1.0.2 - "Terminate contig extensions at palindromic kmers"
                    bool isPalindrome = node.IsPalindrome(this._graph.KmerLength);

                    if (isPalindrome || node.LeftExtensionNodesCount > 1)
                    {
                        // Ambiguous. Remove all extensions
                        foreach (DeBruijnNode left in node.GetLeftExtensionNodes())
                        {
                            left.MarkExtensionInvalid(node);
                            node.MarkLeftExtensionAsInvalid(left);
                        }
                    }
                    else
                    {
                        // Remove self loops
                        if (node.LeftExtensionNodesCount == 1 && node.GetLeftExtensionNodes().First() == node)
                        {
                            node.MarkLeftExtensionAsInvalid(node);
                        }
                    }

                    if (isPalindrome || node.RightExtensionNodesCount > 1)
                    {
                        // Ambiguous. Remove all extensions
                        foreach (DeBruijnNode right in node.GetRightExtensionNodes())
                        {
                            right.MarkExtensionInvalid(node);
                            node.MarkRightExtensionAsInvalid(right);
                        }
                    }
                    else
                    {
                        // Remove self loops
                        if (node.RightExtensionNodesCount == 1 && node.GetRightExtensionNodes().First() == node)
                        {
                            node.MarkRightExtensionAsInvalid(node);
                        }
                    }
                });
        }

        /// <summary>
        /// Get simple paths in the graph.
        /// </summary>
        /// <returns>List of simple paths.</returns>
        private IList<ISequence> GetSimplePaths(bool createContigSequences)
        {
            IList<ISequence> paths = new List<ISequence>();
            Parallel.ForEach(this._graph.GetNodes(), node =>
                {
                    int validLeftExtensionsCount = node.LeftExtensionNodesCount;
                    int validRightExtensionsCount = node.RightExtensionNodesCount;

                    if (validLeftExtensionsCount + validRightExtensionsCount == 0)
                    {
                        // Island. Check coverage
                        if (Double.IsNaN(_coverageThreshold))
                        {
                            if (createContigSequences)
                            {
                                lock (paths)
                                {
                                    paths.Add(_graph.GetNodeSequence(node));
                                }
                            }
                        }
                        else
                        {
                            if (node.KmerCount < _coverageThreshold)
                            {
                                node.MarkNodeForDelete();
                            }
                        }
                    }
                    else if (validLeftExtensionsCount == 1 && validRightExtensionsCount == 0)
                    {
                        TraceSimplePath(paths, node, false, createContigSequences);
                    }
                    else if (validRightExtensionsCount == 1 && validLeftExtensionsCount == 0)
                    {
                        TraceSimplePath(paths, node, true, createContigSequences);
                    }
                });

            return paths;
        }

        /// <summary>
        /// Trace simple path starting from 'node' in specified direction.
        /// </summary>
        /// <param name="assembledContigs">List of assembled contigs.</param>
        /// <param name="node">Starting node of contig path.</param>
        /// <param name="isForwardDirection">Boolean indicating direction of path.</param>
        /// <param name="createContigSequences">Boolean indicating whether the contig sequences are to be created or not.</param>
        private void TraceSimplePath(IList<ISequence> assembledContigs, DeBruijnNode node, bool isForwardDirection, bool createContigSequences)
        {
            ISequence nodeSequence = this._graph.GetNodeSequence(node);
            List<byte> contigSequence = new List<byte>(nodeSequence);

            IList<DeBruijnNode> contigPath = new List<DeBruijnNode> { node };
            KeyValuePair<DeBruijnNode, bool> nextNode =
                isForwardDirection ? node.GetRightExtensionNodesWithOrientation().First() : node.GetLeftExtensionNodesWithOrientation().First();
            this.TraceSimplePathLinks(contigPath, contigSequence, isForwardDirection, nextNode.Value, nextNode.Key, createContigSequences);

            // Check to remove duplicates
            if (contigPath[0].NodeValue.CompareTo(contigPath.Last().NodeValue) >= 0)
            {
                // Check contig coverage.
                if (!Double.IsNaN(_coverageThreshold))
                {
                    // Definition from Velvet Manual: http://helix.nih.gov/Applications/velvet_manual.pdf
                    // "k-mer coverage" is how many times a k-mer has been seen among the reads.
                    double coverage = contigPath.Average(n => n.KmerCount);
                    if (coverage < this._coverageThreshold)
                    {
                        contigPath.ForEach(n => n.MarkNodeForDelete());
                        return;
                    }
                }
                else
                {
                    if (createContigSequences)
                    {
                        lock (assembledContigs)
                        {
                            assembledContigs.Add(new Sequence(nodeSequence.Alphabet, contigSequence.ToArray()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Trace simple path in specified direction.
        /// </summary>
        /// <param name="contigPath">List of graph nodes corresponding to contig path.</param>
        /// <param name="contigSequence">Sequence of contig being assembled.</param>
        /// <param name="isForwardDirection">Boolean indicating direction of path.</param>
        /// <param name="sameOrientation">Path orientation.</param>
        /// <param name="node">Next node on the path.</param>
        /// <param name="createContigSequences">Indicates whether the contig sequences are to be created or not.</param>
        private void TraceSimplePathLinks(
            IList<DeBruijnNode> contigPath,
            List<byte> contigSequence,
            bool isForwardDirection,
            bool sameOrientation,
            DeBruijnNode node,
            bool createContigSequences)
        {
            bool endFound = false;
            while (!endFound)
            {
                // Get extensions going in same directions.
                Dictionary<DeBruijnNode, bool> sameDirectionExtensions = (isForwardDirection ^ sameOrientation) ?
                                                                             node.GetLeftExtensionNodesWithOrientation() : node.GetRightExtensionNodesWithOrientation();

                if (sameDirectionExtensions.Count == 0)
                {
                    // Found end of path. Add this and return
                    this.CheckAndAddNode(contigPath, contigSequence, node, isForwardDirection, sameOrientation, createContigSequences);
                    endFound = true;
                }
                else
                {
                    var sameDirectionExtension = sameDirectionExtensions.First();

                    // (sameDirectionExtensions == 1 && oppDirectionExtensions == 1)
                    // Continue traceback in the same direction. Add this node to list and continue.
                    if (!this.CheckAndAddNode(contigPath, contigSequence, node, isForwardDirection, sameOrientation, createContigSequences))
                    {
                        // Loop is found. Cannot extend simple path further 
                        break;
                    }
                    else
                    {
                        node = sameDirectionExtension.Key;
                        sameOrientation =
                            !(sameOrientation ^ sameDirectionExtension.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if 'node' can be added to 'path' without causing a loop.
        /// If yes, adds node to path and returns true. If not, returns false.
        /// </summary>
        /// <param name="contigPath">List of graph nodes corresponding to contig path.</param>
        /// <param name="contigSequence">Sequence of contig being assembled.</param>
        /// <param name="nextNode">Next node on the path to be added.</param>
        /// <param name="isForwardDirection">Boolean indicating direction.</param>
        /// <param name="isSameOrientation">Boolean indicating orientation.</param>
        /// <param name="createContigSequences">Boolean indicating whether contig sequences are to be created or not.</param>
        /// <returns>Boolean indicating if path was updated successfully.</returns>
        private bool CheckAndAddNode(
            IList<DeBruijnNode> contigPath,
            List<byte> contigSequence,
            DeBruijnNode nextNode,
            bool isForwardDirection,
            bool isSameOrientation,
            bool createContigSequences)
        {
            if (contigPath.Contains(nextNode))
            {
                // there is a loop in this link
                // Return false indicating no update has been made
                return false;
            }
            else
            {
                // Add node to contig list
                contigPath.Add(nextNode);

                if (createContigSequences)
                {
                    // Update contig sequence with sequence from next node
                    byte symbol = this._graph.GetNextSymbolFrom(nextNode, isForwardDirection, isSameOrientation);

                    if (isForwardDirection)
                    {
                        contigSequence.Add(symbol);
                    }
                    else
                    {
                        contigSequence.Insert(0, symbol);
                    }
                }

                return true;
            }
        }
    }
}
