using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Redundant links are caused by single point mutations occuring in middle of reads.
    /// This class implements the methods for detecting redundant paths, 
    /// and then removing all but one path.
    /// </summary>
    public class RedundantPathsPurger : IGraphErrorPurger
    {
        #region Fields, Constructor, Properties
        /// <summary>
        /// Threshold for length of redundant paths.
        /// </summary>
        private int pathLengthThreshold;

        /// <summary>
        /// Holds reference to assembler graph.
        /// </summary>
        private DeBruijnGraph graph;

        /// <summary>
        /// Initializes a new instance of the RedundantPathsPurger class.
        /// Takes user parameter for threshold. 
        /// </summary>
        /// <param name="length">Threshold length.</param>
        public RedundantPathsPurger(int length)
        {
            this.pathLengthThreshold = length;
        }

        /// <summary>
        /// Gets the name of the algorithm.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.RedundantPathsPurger; }
        }

        /// <summary>
        /// Gets the description of the algorithm.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.RedundantPathsPurgerDescription; }
        }

        /// <summary>
        /// Gets or sets threshold for length of redundant paths.
        /// 
        /// Given two diverging paths leaving a node, we extend the paths for a maximum up to LengthThreshold
        /// looking for it to converge with itself before giving up.
        /// </summary>
        public int LengthThreshold
        {
            get { return this.pathLengthThreshold; }
            set { this.pathLengthThreshold = value; }
        }
        #endregion

        /// <summary>
        /// Detect nodes that are on redundant paths. 
        /// Start from any node that has ambiguous (more than one) extensions.
        /// From this node, trace path for each extension until either they 
        /// converge to a single node or threshold length is exceeded. 
        /// In case they converge, we have a set of redundant paths. 
        /// We pick the best path based on the kmer counts of the path nodes.
        /// All paths other than the best one are returned for removal.
        /// Locks: Method only does reads. No locking necessary here or its callees. 
        /// </summary>
        /// <param name="deBruijnGraph">De Bruijn Graph.</param>
        /// <returns>List of path nodes to be deleted.</returns>
        public DeBruijnPathList DetectErroneousNodes(DeBruijnGraph deBruijnGraph)
        {
            DeBruijnGraph.ValidateGraph(deBruijnGraph);
            this.graph = deBruijnGraph;
            // List of the collection of redundant paths, passed in to method to be filled
            // TODO: Paths are tranversed and returned in both directions here: we should probably simplify...
            List<DeBruijnPathList> redundantPaths = new List<DeBruijnPathList>();

            Parallel.ForEach(
                deBruijnGraph.GetNodes(),
                node =>
                {
                    // Need to check for both left and right extensions for ambiguity.
                    if (node.RightExtensionNodesCount > 1)
                    {
                        TraceDivergingExtensionPaths(node, node.GetRightExtensionNodesWithOrientation(), true, redundantPaths);
                    }

                    if (node.LeftExtensionNodesCount > 1)
                    {
                        TraceDivergingExtensionPaths(node, node.GetLeftExtensionNodesWithOrientation(), false, redundantPaths);
                    }
                });
            // Now to check that for each path they all go in the same way.
            redundantPaths = RemoveDuplicates(redundantPaths);
            return DetachBestPath(redundantPaths);
        }

        /// <summary>
        /// Removes nodes that are part of redundant paths. 
        /// </summary>
        /// <param name="deBruijnGraph">De Bruijn graph.</param>
        /// <param name="nodesList">Path nodes to be deleted.</param>
        public void RemoveErroneousNodes(DeBruijnGraph deBruijnGraph, DeBruijnPathList nodesList)
        {

            DeBruijnGraph.ValidateGraph(deBruijnGraph);

            if (nodesList == null)
            {
                throw new ArgumentNullException("nodesList");
            }

            this.graph = deBruijnGraph;

            // Neighbors of all nodes have to be updated.
            HashSet<DeBruijnNode> deleteNodes = new HashSet<DeBruijnNode>(
                nodesList.Paths.AsParallel().SelectMany(nl => nl.PathNodes));

            // Update extensions for deletion
            // No need for read-write lock as deleteNode's dictionary is being read, 
            // and only other graph node's dictionaries are updated.
            Parallel.ForEach(
                deleteNodes,
                node =>
                {
                    foreach (DeBruijnNode extension in node.GetExtensionNodes())
                    {
                        // If the neighbor is also to be deleted, there is no use of updation in that case
                        if (!deleteNodes.Contains(extension))
                        {
                            extension.RemoveExtensionThreadSafe(node);
                        }
                    }
                });

            // Delete nodes from graph
            this.graph.RemoveNodes(deleteNodes);
        }

        /// <summary>
        /// Extract best path from the list of paths in each cluster.
        /// Take off the best path from list and return rest of the paths
        /// for removal.
        /// </summary>
        /// <param name="pathClusters">List of path clusters.</param>
        /// <returns>List of path nodes to be removed.</returns>
        private static DeBruijnPathList DetachBestPath(List<DeBruijnPathList> pathClusters)
        {
            return new DeBruijnPathList(
                pathClusters.AsParallel().SelectMany(paths => ExtractBestPath(paths).Paths));
        }

        /// <summary>
        /// Extract best path from list of paths. For the current cluster 
        /// of paths, return only those that should be removed.
        /// </summary>
        /// <param name="divergingPaths">List of redundant paths.</param>
        /// <returns>List of paths nodes to be deleted.</returns>
        private static DeBruijnPathList ExtractBestPath(DeBruijnPathList divergingPaths)
        {
            // Find "best" path. Except for best path, return rest for removal 
            int bestPathIndex = GetBestPath(divergingPaths);

            DeBruijnPath bestPath = divergingPaths.Paths[bestPathIndex];
            divergingPaths.Paths.RemoveAt(bestPathIndex);

            // There can be overlap between redundant paths.
            // Remove path nodes that occur in best path
            foreach (var path in divergingPaths.Paths)
            {
                path.RemoveAll(n => bestPath.PathNodes.Contains(n));
            }

            return divergingPaths;
        }

        /// <summary>
        /// Gets the best path from the list of diverging paths.
        /// Path that has maximum average of 'count' of belonging k-mers is best.
        /// In case there are multiple 'best' paths, we arbitrarily return one of them.
        /// </summary>
        /// <param name="divergingPaths">List of diverging paths.</param>
        /// <returns>Index of the best path.</returns>
        private static int GetBestPath(DeBruijnPathList divergingPaths)
        {
            // We find the index of the 'best' path.
            double max = -1;
            int maxIndex = -1;

            // Path that has the maximum sum of 'count' of belonging k-mers is the winner
            for (int i = 0; i < divergingPaths.Paths.Count; i++)
            {
                double sum = divergingPaths.Paths[i].PathNodes.Average(n => (double)n.KmerCount);
                if (sum > max)
                {
                    max = sum;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Gets start node of redundant path cluster
        /// All paths in input are part of a redundant path cluster
        /// So all of them have the same start and the end node.
        /// Return the first node of first path.
        /// </summary>
        /// <param name="paths">List of redundant paths.</param>
        /// <returns>Start node of redundant path cluster.</returns>
        private static DeBruijnNode GetStartNode(DeBruijnPathList paths)
        {
            return paths.Paths.First().PathNodes.First();
        }

        /// <summary>
        /// Gets end node of redundant path cluster
        /// All paths in input are part of a redundant path cluster
        /// So all of them have the same start and the end node.
        /// Return the last node of first path.
        /// </summary>
        /// <param name="paths">List of redundant paths.</param>
        /// <returns>End node of redundant path cluster.</returns>
        private static DeBruijnNode GetEndNode(DeBruijnPathList paths)
        {
            return paths.Paths.First().PathNodes.Last();
        }

        /// <summary>
        /// Some set of paths will appear twice, one traced in forward direction
        /// and other in opposite. This method eliminate duplicates.
        /// </summary>
        /// <param name="redundantPathClusters">List of path cluster.</param>
        /// <returns>List of unique path clusters.</returns>
        private static List<DeBruijnPathList> RemoveDuplicates(List<DeBruijnPathList> redundantPathClusters)
        {
            // Divide the list into two groups. One with paths that do not 
            // have duplicates, and one with paths that do not have duplicate
            List<IGrouping<bool, DeBruijnPathList>> uniqueAndDuplicatedPaths =
            redundantPathClusters.AsParallel().GroupBy(pc1 =>
                redundantPathClusters.Any(pc2 =>
                    GetStartNode(pc1) == GetEndNode(pc2) && GetEndNode(pc1) == GetStartNode(pc2))).ToList();

            List<DeBruijnPathList> uniquePaths = new List<DeBruijnPathList>();
            foreach (IGrouping<bool, DeBruijnPathList> group in uniqueAndDuplicatedPaths)
            {
                if (!group.Key)
                {
                    // Add all paths that do have duplicates to final list
                    uniquePaths.AddRange(group);
                }
                else
                {
                    // Each element in this list contains a duplicate in the list
                    // Add only those where the start node has a sequence that is
                    // lexicographically greater than the end node sequence. This
                    // operation will eliminate duplicates effectively.
                    uniquePaths.AddRange(
                        group.AsParallel().Where(pc =>
                                GetStartNode(pc).NodeValue.CompareTo(
                                GetEndNode(pc).NodeValue) >= 0));
                }
            }

            return uniquePaths;
        }

        /// <summary>
        /// Traces diverging paths in given direction.
        /// For each path in the set of diverging paths, extend path by one node
        /// at a time. Continue this until all diverging paths converge to a 
        /// single node or length threshold is exceeded.
        /// If paths converge, add path cluster containing list of redundant 
        /// path nodes to list of redundant paths and return.
        /// </summary>
        /// <param name="startNode">Node at starting point of divergence.</param>
        /// <param name="divergingNodes">List of diverging nodes.</param>
        /// <param name="isForwardExtension">Bool indicating direction of divergence. (Right = true)</param>
        /// <param name="redundantPaths">List of redundant paths.</param>
        private void TraceDivergingExtensionPaths(
            DeBruijnNode startNode,
            Dictionary<DeBruijnNode, bool> divergingNodes,
            bool isForwardExtension,
            List<DeBruijnPathList> redundantPaths)
        {
            List<PathWithOrientation> divergingPaths = new List<PathWithOrientation>(
                divergingNodes.Select(n =>
                    new PathWithOrientation(startNode, n.Key, (isForwardExtension ^ n.Value))));
            int divergingPathLength = 2;

            /* These are nodes with >= 2 coming in as the 
             * in the same direction as a path we are following.  If two paths
             * both enter the same node from the same direction, they can be redundant.
             */
            HashSet<DeBruijnNode> possibleEndNodes = new HashSet<DeBruijnNode>();
            int finishedCount = 0;
            // Extend each path in cluster. While performing path extension 
            // also keep track of whether they have converged, which we indicate by setting 
            // this to the first node that two paths both encounter.
            DeBruijnNode convergentNode = null;
            // Extend paths till length threshold is exceeded.
            // or possible paths are exhausted
            while (divergingPathLength <= this.pathLengthThreshold &&
                finishedCount != divergingPaths.Count &&
                convergentNode == null)
            {                
                foreach(PathWithOrientation path in divergingPaths) {                    
                    if (path.EndReached) {
                        continue; 
                    }
                    DeBruijnNode endNode = path.Nodes.Last();
                    Dictionary<DeBruijnNode, bool> extensions
                        = path.GrabNextNodesOnLeft ? endNode.GetLeftExtensionNodesWithOrientation() : endNode.GetRightExtensionNodesWithOrientation();

                    // Extension is possible only if end point of all paths has exactly one extension
                    // In case extensions count is 0, no extensions possible for some path (or)
                    // if extensions is more than 1, they are diverging further. Not considered a redundant path
                    if (extensions.Count > 1 || extensions.Count == 0) {
                        path.EndReached = true;
                        finishedCount++;
                    } else {
                        // Get next node
                        KeyValuePair<DeBruijnNode, bool> nextNodeTuple = extensions.First ();
                        DeBruijnNode nextNode = nextNodeTuple.Key;
                        // Have we formed a circle? If so, we are done.
                        // TODO: This is almost certainly very slow for long paths, can replace with Hash and remove possibleEndNodes variable
                        if (path.Nodes.Contains (nextNode)) {
                            finishedCount++;
                            path.EndReached = true;
                        } else {
                            // Update path orientation
                            path.GrabNextNodesOnLeft = !(path.GrabNextNodesOnLeft ^ nextNodeTuple.Value);
                            path.Nodes.Add (nextNode);

                            /* Did any other nodes come in to this node from the same direction 
                             * (path or N-1 basepairs shared)? */
                            var sameInputsCount = path.GrabNextNodesOnLeft ? nextNode.RightExtensionNodesCount : nextNode.LeftExtensionNodesCount;
                            if (sameInputsCount > 1) {
                                if (possibleEndNodes.Contains (nextNode)) {
                                    path.EndReached = true;
                                    convergentNode = nextNode;
                                    finishedCount++;
                                } else {
                                    possibleEndNodes.Add (nextNode);
                                }
                            }
                        }
                    }
                }
                divergingPathLength++;
                // Paths have been extended. Check for convergence
                if (convergentNode != null)
                {
                    bool redundantPathFound = ConfirmAndAddRedundantPaths (convergentNode, divergingPaths, redundantPaths);
                    if (redundantPathFound) {
                        return;
                    } else {
                        /* If we didn't find any paths, it means the nodes came in from different directions, so we
                         * didn't find a truly convergent node, and the search continues.  This should basically never happen.
                        */     
                        convergentNode = null;
                    }
                }
            }
        }

        /// <summary>
        /// Once we have a set of paths where at least two of these paths converge on the same node.
        /// This method checks that the paths are truly convergent (converge from same direction)
        /// trims off any excess (indels can lead to unequl paths), and adds it to the redundant path list.
        /// </summary>
        /// <param name="convergentNode">Convergent node.</param>
        /// <param name="divergingPaths">Paths.</param>
        /// <param name="redundantPaths">Redundant paths.</param>
        private bool ConfirmAndAddRedundantPaths(DeBruijnNode convergentNode, List<PathWithOrientation> divergingPaths,
            List<DeBruijnPathList> redundantPaths)
        {
            bool foundRedundantPaths = false;
            /* Now it is possible that two (or more) paths have converged on a node but from
             * different directions, so we check for this */

            // Get paths that converge on this node 
            var convergingPaths = divergingPaths.Select (x => new KeyValuePair<PathWithOrientation, int> (x, x.Nodes.IndexOf (convergentNode))).
                Where(z => z.Value != -1).ToList ();

            // Now trim them all to the appropriate length so convergent node is the end node
            // (in case of unequal paths they may differ)
            foreach (var pathLocation in convergingPaths) {
                var location = pathLocation.Value;
                var path = pathLocation.Key;
                if (location != path.Nodes.Count - 1) {
                    path.Nodes.RemoveRange (location + 1, path.Nodes.Count - (location + 1));
                }
            }

            /* Now we have to make a path of all nodes that converge in the same direction */
            List<DeBruijnNode>[] sideExtensions =  { convergentNode.GetLeftExtensionNodes().ToList(),
                convergentNode.GetRightExtensionNodes().ToList()};
            foreach (var extensions in sideExtensions) {
                var convergeFromSameSide = convergingPaths.Where (p => extensions.Contains (p.Key.Nodes [p.Key.Nodes.Count - 2])).ToList();
                if (convergeFromSameSide.Count > 1) {
                    foundRedundantPaths = true;
                    // Note: all paths have the same end node.
                    lock (redundantPaths)
                    {
                        redundantPaths.Add(new DeBruijnPathList(divergingPaths.Select(p => new DeBruijnPath(p.Nodes))));
                    }
                }
            }
            return foundRedundantPaths;
        }

    }


}
