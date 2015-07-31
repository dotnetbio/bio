using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph;
using Bio.Properties;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Performs Breadth First Search on Contig Overlap Graph
    /// using distance between contigs as constrain.
    /// </summary>
    public class TracePath : ITracePath
    {
        /// <summary>
        /// Contig Overlap Graph.
        /// </summary>
        private ContigGraph graph;

        /// <summary>
        /// Depth to which graph is traversed.
        /// </summary>
        private int depth;

        /// <summary>
        /// Length of Kmer (indicates kmer -1 overlap between contigs).
        /// </summary>
        private int kmerLength;

        /// <summary>
        /// Performs Breadth First Search to traverse through graph to generate scaffold paths.
        /// </summary>
        /// <param name="overlapGraph">Contig Overlap Graph.</param>
        /// <param name="contigPairedReadMaps">InterContig Distances.</param>
        /// <param name="lengthOfKmer">Length of Kmer.</param>
        /// <param name="searchDepth">Depth to which graph is searched.</param>
        /// <returns>List of paths/scaffold.</returns>
        public IList<ScaffoldPath> FindPaths(
            ContigGraph overlapGraph,
            ContigMatePairs contigPairedReadMaps,
            int lengthOfKmer,
            int searchDepth = 10)
        {
            if (overlapGraph == null)
            {
                throw new ArgumentNullException("deBruijnGraph");
            }

            if (contigPairedReadMaps == null)
            {
                throw new ArgumentNullException("contigPairedReadMaps");
            }

            if (lengthOfKmer <= 0)
            {
                throw new ArgumentException(Resource.KmerLength);
            }

            if (searchDepth <= 0)
            {
                throw new ArgumentException(Resource.Depth);
            }

            this.graph = overlapGraph;
            this.kmerLength = lengthOfKmer;
            this.depth = searchDepth;

            List<ScaffoldPath> scaffoldPaths = new List<ScaffoldPath>();
            Parallel.ForEach(
                overlapGraph.Nodes, 
                (Node node) =>
                {
                Dictionary<ISequence, IList<ValidMatePair>> contigPairedReadMap;
                if (contigPairedReadMaps.TryGetValue(overlapGraph.GetNodeSequence(node), out contigPairedReadMap))
                {
                    List<ScaffoldPath> scaffoldPath = TraverseGraph(node, contigPairedReadMap);
                    lock (scaffoldPaths)
                    {
                        scaffoldPaths.AddRange(scaffoldPath);
                    }
                }
            });

            return scaffoldPaths;
        }

        /// <summary>
        /// Performs Breadth First Search.
        /// </summary>
        /// <param name="node">Start Node.</param>
        /// <param name="contigPairedReadMap">Map of all contigs having valid 
        /// mate pairs with given node contig.</param>
        /// <returns>List of paths.</returns>
        private List<ScaffoldPath> TraverseGraph(
            Node node,
            Dictionary<ISequence, IList<ValidMatePair>> contigPairedReadMap)
        {
            Queue<Paths> search = new Queue<Paths>();
            List<Paths> paths = new List<Paths>();
            this.LeftExtension(
                new KeyValuePair<Node, Edge>(node, new Edge(false)),
                search,
                paths,
                null,
                contigPairedReadMap);
            this.RightExtension(
                new KeyValuePair<Node, Edge>(node, new Edge(true)),
                search,
                paths,
                null,
                contigPairedReadMap);
            Paths parentPath;
            while (search.Count != 0)
            {
                parentPath = search.Dequeue();
                if (parentPath.NodeOrientation)
                {
                    if (parentPath.CurrentNode.Value.IsSameOrientation)
                    {
                        this.RightExtension(
                            parentPath.CurrentNode,
                            search,
                            paths,
                            parentPath.FamilyTree,
                            contigPairedReadMap);
                    }
                    else
                    {
                        this.LeftExtension(
                            parentPath.CurrentNode,
                            search,
                            paths,
                            parentPath.FamilyTree,
                            contigPairedReadMap);
                    }
                }
                else if (parentPath.CurrentNode.Value.IsSameOrientation)
                {
                    this.LeftExtension(
                        parentPath.CurrentNode,
                        search,
                        paths,
                        parentPath.FamilyTree,
                        contigPairedReadMap);
                }
                else
                {
                    this.RightExtension(
                        parentPath.CurrentNode,
                        search,
                        paths,
                        parentPath.FamilyTree,
                        contigPairedReadMap);
                }
            }

            return new List<ScaffoldPath>(paths.Select(t => t.FamilyTree)); 
        }

        /// <summary>
        /// Add left extension of the nodes to queue.  
        /// </summary>
        /// <param name="node">Current node.</param>
        /// <param name="search">Queue for BFS.</param>
        /// <param name="paths">List of paths.</param>
        /// <param name="familyTree">Nodes visited for construction of paths.</param>
        /// <param name="contigPairedReadMap">Contig and valid mate pair map.</param>
        private void LeftExtension(
            KeyValuePair<Node, Edge> node,
            Queue<Paths> search,
            List<Paths> paths,
            ScaffoldPath familyTree,
            Dictionary<ISequence, IList<ValidMatePair>> contigPairedReadMap)
        {
            Paths childPath;
            if (node.Key.LeftExtensionNodes.Count > 0)
            {
                foreach (KeyValuePair<Node, Edge> child in node.Key.LeftExtensionNodes)
                {
                    childPath = new Paths();
                    childPath.CurrentNode = child;
                    if (familyTree == null)
                    {
                        childPath.FamilyTree.Add(node);
                    }
                    else
                    {
                        childPath.FamilyTree.AddRange(familyTree);
                        childPath.FamilyTree.Add(node);
                    }

                    childPath.NodeOrientation = false;
                    if (this.DistanceConstraint(childPath, contigPairedReadMap) &&
                        childPath.FamilyTree.Count < this.depth && 
                        !contigPairedReadMap.All(
                        t => childPath.FamilyTree.Any(k => t.Key == this.graph.GetNodeSequence(k.Key))))
                    {
                        search.Enqueue(childPath);
                    }
                    else
                    {
                        if (contigPairedReadMap.All(
                            t => childPath.FamilyTree.Any(k => t.Key == this.graph.GetNodeSequence(k.Key))))
                        {
                            paths.Add(childPath);
                        }
                    }
                }
            }
            else
            {
                childPath = new Paths();
                if (familyTree == null)
                {
                    childPath.FamilyTree.Add(node);
                }
                else
                {
                    childPath.FamilyTree.AddRange(familyTree);
                    childPath.FamilyTree.Add(node);
                }

                if (contigPairedReadMap.All(
                    t => childPath.FamilyTree.Any(k => t.Key == this.graph.GetNodeSequence(k.Key))))
                {
                    paths.Add(childPath);
                }
            }
        }

        /// <summary>
        /// Add right extension of the nodes to queue.
        /// </summary>
        /// <param name="node">Current node.</param>
        /// <param name="search">Queue for BFS.</param>
        /// <param name="paths">List of paths.</param>
        /// <param name="familyTree">Nodes visited for construction of paths.</param>
        /// <param name="contigPairedReadMap">Contig and valid mate pair map.</param>
        private void RightExtension(
            KeyValuePair<Node, Edge> node,
            Queue<Paths> search,
            List<Paths> paths,
            ScaffoldPath familyTree,
            Dictionary<ISequence, IList<ValidMatePair>> contigPairedReadMap)
        {
            Paths childPath;
            if (node.Key.RightExtensionNodes.Count > 0)
            {
                foreach (KeyValuePair<Node, Edge> child in node.Key.RightExtensionNodes)
                {
                    childPath = new Paths();
                    childPath.CurrentNode = child;
                    if (familyTree == null)
                    {
                        childPath.FamilyTree.Add(node);
                    }
                    else
                    {
                        childPath.FamilyTree.AddRange(familyTree);
                        childPath.FamilyTree.Add(node);
                    }

                    childPath.NodeOrientation = true;
                    if (this.DistanceConstraint(childPath, contigPairedReadMap) &&
                        childPath.FamilyTree.Count < this.depth && 
                        !contigPairedReadMap.All(
                        t => childPath.FamilyTree.Any(k => t.Key == this.graph.GetNodeSequence(k.Key))))
                    {
                        search.Enqueue(childPath);
                    }
                    else
                    {
                       if (contigPairedReadMap.All(
                            t => childPath.FamilyTree.Any(k => t.Key == this.graph.GetNodeSequence(k.Key))))
                        {
                            paths.Add(childPath);
                        }
                    }
                }
            }
            else
            {
                childPath = new Paths();
                if (familyTree == null)
                {
                    childPath.FamilyTree.Add(node);
                }
                else
                {
                    childPath.FamilyTree.AddRange(familyTree);
                    childPath.FamilyTree.Add(node);
                }

               if (contigPairedReadMap.All(
                    t => childPath.FamilyTree.Any(k => t.Key == this.graph.GetNodeSequence(k.Key))))
                {
                    paths.Add(childPath);
                }
            }
        }

        /// <summary>
        /// Apply Distance constrains on given two nodes.
        /// The distances between contigs are calculated using paired read information.
        /// </summary>
        /// <param name="childPath">Destination node.</param>
        /// <param name="contigPairedReadMaps">Map of contigs and paired reads.</param>
        /// <returns>Whether Distance between contig nodes lie in constraint or not.</returns>
        private bool DistanceConstraint(
            Paths childPath,
            Dictionary<ISequence, IList<ValidMatePair>> contigPairedReadMaps)
        {
            IList<ValidMatePair> map;
            if (contigPairedReadMaps.TryGetValue(this.graph.GetNodeSequence(childPath.CurrentNode.Key), out map))
            {
                float pathlength = this.GetPathLength(childPath);
                if (childPath.CurrentNode.Value.IsSameOrientation)
                {
                    if ((map[0].DistanceBetweenContigs[0] -
                        (3 * map[0].StandardDeviation[0]) <= pathlength) &&
                        (pathlength <= map[0].DistanceBetweenContigs[0] +
                        (3 * map[0].StandardDeviation[0])))
                    {
                        return true;
                    }
                    return false;
                }
                
                if ((map[0].DistanceBetweenContigs[1] -
                     (3 * map[0].StandardDeviation[1]) <= pathlength) &&
                    (pathlength <= map[0].DistanceBetweenContigs[1] +
                     (3 * map[0].StandardDeviation[1])))
                {
                    return true;
                }
            
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Get length of path traversed using BFS.
        /// </summary>
        /// <param name="childPath">Path travelled to reach destination node.</param>
        /// <returns>Distance between first and last contig node.</returns>
        private float GetPathLength(Paths childPath)
        {
            float distance = 0;
            for (int index = 1; index < childPath.FamilyTree.Count; index++)
            {
                distance += this.graph.GetNodeSequence(childPath.FamilyTree[index].Key).Count - this.kmerLength;
            }

            distance -= this.kmerLength;
            return distance;
        }
    }

    /// <summary>
    /// Class stores information of path traveled while performing BFS.
    /// </summary>
    internal class Paths
    {
        /// <summary>
        /// Path traveled.
        /// </summary>
        private readonly ScaffoldPath path = new ScaffoldPath();

        /// <summary>
        /// Gets or sets value of current Node.
        /// </summary>
        public KeyValuePair<Node, Edge> CurrentNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether orientation of current node.
        /// </summary>
        public bool NodeOrientation { get; set; }

        /// <summary>
        /// Gets the value of family tree/path traversed to reach to current node.
        /// </summary>
        public ScaffoldPath FamilyTree
        {
            get { return this.path; }
        }
    }
}
