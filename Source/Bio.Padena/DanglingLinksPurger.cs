using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio.Util;
using Bio.Algorithms.Assembly.Graph;

namespace Bio.Algorithms.Assembly.Padena
{
    /// <summary>
    /// Dangling links are caused by errors occurring at the end of read.
    /// This class implements the methods for detecting dangling links
    /// and removing the nodes on dangling links from the graph.
    /// This also implements graph erosion, where ends of graph which 
    /// have low coverage are removed. 
    /// </summary>
    public class DanglingLinksPurger : IGraphErrorPurger, IGraphEndsEroder
    {
        /// <summary>
        /// User Input Parameter
        /// Threshold for eroding low coverage ends.
        /// </summary>
        private int erodeThreshold;

        /// <summary>
        /// Field used to keep track of the minimum length of 
        /// dangling link that crosses the length threshold.
        /// This is used to update the threshold for next round.
        /// </summary>
        private SortedSet<int> danglingLinkLengths;

        /// <summary>
        /// Tasks queued to extend dangling links following
        /// graph clean-up after erosion.
        /// </summary>
        private List<Task<int>> danglingLinkExtensionTasks;

        /// <summary>
        /// Initializes a new instance of the DanglingLinksPurger class.
        /// </summary>
        /// <param name="threshold">Threshold for dangling links.</param>
        /// <param name="erodeThreshold">Threshold for eroding endpoints.</param>
        public DanglingLinksPurger(int threshold = 0, int erodeThreshold = -1)
        {
            LengthThreshold = threshold;
            this.erodeThreshold = erodeThreshold;
        }

        /// <summary>
        /// Gets the name of the algorithm.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.DanglingLinksPurger; }
        }

        /// <summary>
        /// Gets the description of algorithm.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.DanglingLinksPurgerDescription; }
        }

        /// <summary>
        /// Gets or sets the threshold length.
        /// </summary>
        public int LengthThreshold { get; set; }

        /// <summary>
        /// Erode ends of graph that have coverage less than given erodeThreshold.
        /// As optimization, we also check for dangling links and keeps track of the
        /// lengths of the links found. No removal is done at this step.
        /// This is done to get an idea of the different lengths at 
        /// which to run the dangling links purger step.
        /// This method returns the lengths of dangling links found.
        /// Locks: Method only does reads. No locking necessary here. 
        /// </summary>
        /// <param name="graph">Input graph.</param>
        /// <param name="erosionThreshold">Threshold for erosion.</param>
        /// <returns>List of lengths of dangling links detected.</returns>
        public IEnumerable<int> ErodeGraphEnds(DeBruijnGraph graph, int erosionThreshold = -1)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            this.erodeThreshold = erosionThreshold;
            this.danglingLinkLengths = new SortedSet<int>();
            this.danglingLinkExtensionTasks = new List<Task<int>>();

            IEnumerable<DeBruijnNode> graphNodes = graph.GetNodes();

            BlockingCollection<int> linkLengths = new BlockingCollection<int>();
            
            // Start consumer task
            // TODO: This length list should contain elements from ~1-19, decide if 
            // having a separate thread keep those numbers sorted is worthwhile
            Task collectionTask = Task.Run(() =>
            {
                while (!linkLengths.IsCompleted)
                {
                    int length;
                    if (linkLengths.TryTake(out length))
                    {
                        this.danglingLinkLengths.Add(length);
                    }
                }
            });
           
            //and now the producer
            bool continueSearching = true;
            while (continueSearching)
            {
                continueSearching = false;

                int threshold = erosionThreshold;
                Parallel.ForEach(graphNodes, node =>
                    {
                        continueSearching = true;
                        if (node.ExtensionsCount == 0)
                        {
                            if (threshold != -1 && node.KmerCount < threshold)
                            {
                                // Mark node for erosion
                                node.MarkNodeForDelete();
                            }
                            else
                            {
                                // Single node island.
                                linkLengths.Add(1);
                            }
                        }
                        else if (node.RightExtensionNodesCount == 0)
                        {
                            // End of possible dangling link
                            // Trace back to see if it is part of a dangling link.
                            DeBruijnPath link = TraceDanglingExtensionLink(false, new DeBruijnPath(), node, true);
                            if (link != null && link.PathNodes.Count > 0)
                            {
                                linkLengths.Add(link.PathNodes.Count);
                            }
                        }
                        else if (node.LeftExtensionNodesCount == 0)
                        {
                            // End of possible dangling link
                            // Trace back to see if it is part of a dangling link.
                            DeBruijnPath link = TraceDanglingExtensionLink(true, new DeBruijnPath(), node, true);
                            if (link != null && link.PathNodes.Count > 0)
                            {
                                linkLengths.Add(link.PathNodes.Count);
                            }
                        }
                    });

                // Remove eroded nodes. In the out parameter, get the list of new 
                // end-points that was created by removing eroded nodes.
                IList<DeBruijnNode> nodes = RemoveErodedNodes(graph);
                if (nodes.Count == 0)
                {
                    break;
                }

                graphNodes = nodes;
            }

            linkLengths.CompleteAdding();

            Task.WaitAll(collectionTask);

            ExtendDanglingLinks();
            return this.danglingLinkLengths;
        }

        /// <summary>
        /// Detect nodes that are part of dangling links. 
        /// Locks: Method only does reads. No locking necessary here or its callees. 
        /// </summary>
        /// <param name="deBruijnGraph">Input graph.</param>
        /// <returns>List of nodes in dangling links.</returns>
        public DeBruijnPathList DetectErroneousNodes(DeBruijnGraph deBruijnGraph)
        {
            if (deBruijnGraph == null)
                throw new ArgumentNullException("deBruijnGraph");

            var debruijnPaths = new ConcurrentBag<DeBruijnPath>();

            Parallel.ForEach(deBruijnGraph.GetNodes(), node =>
            {
                    if (node.ExtensionsCount == 0)
                    {
                        // Single node island
                        debruijnPaths.Add(new DeBruijnPath(node));
                    }
                    else if (node.RightExtensionNodesCount == 0)
                    {
                        // End of possible dangling link
                        // Trace back to see if it is part of a dangling link
                        var link = TraceDanglingExtensionLink(false, new DeBruijnPath(), node, true);
                        if (link != null  && link.PathNodes.Count>0)
                        {
                            debruijnPaths.Add(link);
                        }
                    }
                    else if (node.LeftExtensionNodesCount == 0)
                    {
                        // End of possible dangling link
                        // Trace back to see if it is part of a dangling link
                        var link = TraceDanglingExtensionLink(true, new DeBruijnPath(), node, true);
                        if (link != null && link.PathNodes.Count>0)//if the first node is below the threshold, it is not added, leaving a link with no nodes, so check is needed
                        {
                            debruijnPaths.Add(link);
                        }
                    }
                }
            );

            return new DeBruijnPathList(debruijnPaths);
        }

        /// <summary>
        /// Removes nodes that are part of dangling links.
        /// </summary>
        /// <param name="deBruijnGraph">Input graph.</param>
        /// <param name="nodesList">List of dangling link nodes.</param>
        public void RemoveErroneousNodes(DeBruijnGraph deBruijnGraph, DeBruijnPathList nodesList)
        {
            // Argument Validation
            if (deBruijnGraph == null)
                throw new ArgumentNullException("deBruijnGraph");

            if (nodesList == null)
                throw new ArgumentNullException("nodesList");
            
            var lastNodes = new HashSet<DeBruijnNode>(nodesList.Paths.Select(nl => nl.PathNodes.Last()));

            // Update extensions and Delete nodes from graph.
            deBruijnGraph.RemoveNodes(
                nodesList.Paths.AsParallel().SelectMany(nodes =>
                {
                    RemoveLinkNodes(nodes, lastNodes);
                    return nodes.PathNodes;
                }));
        }

        /// <summary>
        /// Delete nodes marked for erosion. Update adjacent nodes to update their extension tables.
        /// After nodes are deleted, some new end-points might be created. We need to check for 
        /// dangling links at these new points. This list is returned in the out parameter.
        /// 
        /// TODO: Perhaps refactor code so that the graph is only manipulated by itself?
        /// Might make it easier to implement future performance improvements, or cost performance
        /// </summary>
        /// <param name="graph">De Bruijn Graph.</param>
        private static IList<DeBruijnNode> RemoveErodedNodes(DeBruijnGraph graph)
        {
            bool eroded = graph.RemoveMarkedNodes()>0;

            IList<DeBruijnNode> graphNodes;
            if (eroded)
            {
                graphNodes = graph.GetNodes().AsParallel().Where(n =>
                {
                    bool wasEndPoint = (n.LeftExtensionNodesCount == 0 || n.RightExtensionNodesCount == 0);
                    n.RemoveMarkedExtensions();
                    // Check if this is a new end point.
                    return (wasEndPoint || (n.LeftExtensionNodesCount == 0 || n.RightExtensionNodesCount == 0));
                }).ToList();
            }
            else
            {
                graphNodes = new List<DeBruijnNode>();
            }

            return graphNodes;
        }

        /// <summary>
        /// Removes nodes in link from the graph.
        /// Parallelization Note: Locks required here. We are modifying graph structure here.
        /// </summary>
        /// <param name="nodes">List of nodes to remove.</param>
        /// <param name="lastNodes">Set of all nodes occurring at end of dangling links.</param>
        private static void RemoveLinkNodes(DeBruijnPath nodes, HashSet<DeBruijnNode> lastNodes)
        {
            // Nodes in the list are part of a single dangling link.
            // Only the last element of link can have left or right extensions that are valid parts of graph.
            DeBruijnNode linkStartNode = nodes.PathNodes.Last();

            // Update adjacency of nodes connected to the last node. 
            // Read lock not required as linkStartNode's dictionary will not get updated
            // Locks used during removal of extensions.
            foreach (DeBruijnNode graphNode in linkStartNode.GetExtensionNodes())
            {
                // Condition to avoid updating other linkStartNode's dictionary. Reduces conflicts.
                if (!lastNodes.Contains(graphNode))
                {
                    graphNode.RemoveExtensionThreadSafe(linkStartNode);
                }
            }
        }

        /// <summary>
        /// Starting from potential end of dangling link, trace back along 
        /// extension edges in graph to find if it is a valid dangling link.
        /// Parallelization Note: No locks used in TraceDanglingLink. 
        /// We only read graph structure here. No modifications are made.
        /// </summary>
        /// <param name="isForwardDirection">Boolean indicating direction of dangling link.</param>
        /// <param name="link">Dangling Link.</param>
        /// <param name="node">Node that is next on the link.</param>
        /// <param name="sameOrientation">Orientation of link.</param>
        /// <returns>List of nodes in dangling link.</returns>
        private DeBruijnPath TraceDanglingExtensionLink(bool isForwardDirection, DeBruijnPath link, DeBruijnNode node, bool sameOrientation)
        {
            for (; ;)
            {
                // Get extensions going in same and opposite directions.
                Dictionary<DeBruijnNode, bool> sameDirectionExtensions;
                int sameDirectionExtensionsCount, oppDirectionExtensionsCount;
                
                if (isForwardDirection ^ sameOrientation)
                {
                    sameDirectionExtensionsCount = node.LeftExtensionNodesCount;
                    oppDirectionExtensionsCount = node.RightExtensionNodesCount;
                    sameDirectionExtensions = node.GetLeftExtensionNodesWithOrientation();
                }
                else
                {
                    sameDirectionExtensionsCount = node.RightExtensionNodesCount;
                    oppDirectionExtensionsCount = node.LeftExtensionNodesCount;
                    sameDirectionExtensions = node.GetRightExtensionNodesWithOrientation();
                }

                bool reachedEndPoint;
                if (sameDirectionExtensionsCount == 0)
                {
                    // Found other end of dangling link
                    return CheckAndAddDanglingNode(link, node, out reachedEndPoint);
                }

                if (oppDirectionExtensionsCount > 1)
                {
                    // Have reached a point of ambiguity. Return list without updating it.
                    if (this.erodeThreshold != -1 && !node.IsMarkedForDelete)
                    {
                        lock (this.danglingLinkExtensionTasks)
                        {
                            // This task essentially just returns back to this method after other ones are removed
                            this.danglingLinkExtensionTasks.Add(new Task<int>(_ => 
                                ExtendDanglingLink(isForwardDirection, link, node, sameOrientation, false), TaskCreationOptions.None));
                        }
                        return null;
                    }
                    return link;
                }

                if (sameDirectionExtensionsCount > 1)
                {
                    // Have reached a point of ambiguity. Return list after updating it.
                    link = CheckAndAddDanglingNode(link, node, out reachedEndPoint);
                    if (this.erodeThreshold != -1 && reachedEndPoint != true && !node.IsMarkedForDelete)
                    {
                        lock (this.danglingLinkExtensionTasks)
                        {
                            this.danglingLinkExtensionTasks.Add(new Task<int>(_ => 
                                ExtendDanglingLink(isForwardDirection, link, node, sameOrientation, true), TaskCreationOptions.None));
                        }
                        return null;
                    }
                    return link;
                }

                // (sameDirectionExtensions == 1 && oppDirectionExtensions == 1)
                // Continue trace back. Add this node to that list and recurse.
                link = CheckAndAddDanglingNode(link, node, out reachedEndPoint);
                if (reachedEndPoint)
                {
                    // Loop is found or threshold length has been exceeded.
                    return link;
                }

                // Still in loop, so just add the extension and keeps going
                var item = sameDirectionExtensions.First();
                node = item.Key;
                sameOrientation = !(sameOrientation ^ item.Value);
            }
        }

        /// <summary>
        /// Checks if 'node' can be added to 'link' without 
        /// violating any conditions pertaining to dangling links.
        /// Returns null if loop is found or length exceeds threshold.
        /// Otherwise, adds node to link and returns.
        /// </summary>
        /// <param name="link">Dangling link.</param>
        /// <param name="node">Node to be added.</param>
        /// <param name="reachedErrorEndPoint">Indicates if we have reached end of dangling link.</param>
        /// <returns>Updated dangling link.</returns>
        private DeBruijnPath CheckAndAddDanglingNode(DeBruijnPath link, DeBruijnNode node, out bool reachedErrorEndPoint)
        {
            if (this.erodeThreshold != -1 && link.PathNodes.Count == 0 && node.KmerCount < this.erodeThreshold)
            {
                if (node.IsMarkedForDelete)
                {
                    // There is a loop in this link. No need to update link. 
                    // Set flag for end point reached as true and return.
                    reachedErrorEndPoint = true;
                    return link;
                }
                
                node.MarkNodeForDelete();
                reachedErrorEndPoint = false;
                return link;
            }

            if (link.PathNodes.Contains(node))
            {
                // There is a loop in this link. No need to update link. 
                // Set flag for end point reached as true and return.
                reachedErrorEndPoint = true;
                return link;
            }

            if (link.PathNodes.Count >= LengthThreshold)
            {
                // Length crosses threshold. Not a dangling link.
                // So set reached error end point as true and return null.
                reachedErrorEndPoint = true;
                return null;
            }

            // No error conditions found. Add node to link.
            reachedErrorEndPoint = false;
            link.PathNodes.Add(node);
            return link;
        }

        /// <summary>
        /// Try and extend previously terminated dangling links.
        /// </summary>
        private void ExtendDanglingLinks()
        {
            if (this.danglingLinkExtensionTasks != null && this.danglingLinkExtensionTasks.Count > 0)
            {
                Task<int>[] tasks = this.danglingLinkExtensionTasks.ToArray();
                tasks.ForEach(t => t.Start());
                Task.WaitAll(tasks);

                var t2 = tasks.AsParallel().Select(t => t.Result).ToList();
                this.danglingLinkLengths.Union(t2.Where(l => l > 0));

                // variable may be referenced later so should not be set to null
                this.danglingLinkExtensionTasks.Clear();
            }
        }

        /// <summary>
        /// Try and extend dangling links following
        /// graph clean-up after erosion.
        /// </summary>
        /// <param name="isForwardDirection">Boolean indicating direction of dangling link.</param>
        /// <param name="danglingLink">Dangling Link.</param>
        /// <param name="node">Node that is next on the link.</param>
        /// <param name="sameOrientation">Orientation of link.</param>
        /// <param name="removeLast">Boolean indicating if last node 
        /// in link has to be removed before extending.</param>
        /// <returns>Length of dangling link found after extension.</returns>
        private int ExtendDanglingLink(bool isForwardDirection, DeBruijnPath danglingLink, DeBruijnNode node, bool sameOrientation, bool removeLast)
        {
            if (removeLast)
            {
                danglingLink.PathNodes.Remove(node);
            }

            if (danglingLink.PathNodes.Count == 0)
            {
                // DanglingLink is empty. So check if node is an end-point.
                if (node.RightExtensionNodesCount == 0)
                {
                    danglingLink = TraceDanglingExtensionLink(false, new DeBruijnPath(), node, true);
                }
                else if (node.LeftExtensionNodesCount == 0)
                {
                    danglingLink = TraceDanglingExtensionLink(true, new DeBruijnPath(), node, true);
                }
                else
                {
                    // Not an end-point. Return length as 0
                    return 0;
                }
            }
            else
            {
                // Extend existing link.
                danglingLink = TraceDanglingExtensionLink(isForwardDirection, danglingLink, node, sameOrientation);
            }

            // Return length of dangling link found.
            return danglingLink == null ? 0 : danglingLink.PathNodes.Count;

        }
    }
}
