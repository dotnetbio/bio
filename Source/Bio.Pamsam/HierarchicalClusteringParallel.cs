using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Hierarchically clusters sequences based on distance matrix.
    /// 
    /// Steps: 
    /// 1) Initially all sequences are clusters themselves.
    /// 2) Iteratively: identify the cloesest pair of sequences (clusters) in the distance matrix,
    /// cluster them into one cluster, and update distances of this cluster with the rest clusters.
    /// 3) Terminate when only one cluster left. 
    /// 
    /// A binary guide tree is then generated: 
    /// the root of the tree is the final cluster; leaves are sequence clusters.
    /// From bottom up, the nodes order represents the clustering order,
    /// and it's kept in a node list.
    /// The progressive aligner will then follow this order to align the set of sequences.
    /// </summary>
    public sealed class HierarchicalClusteringParallel : HierarchicalClustering
    {
        #region Fields
        // The number of clusters; initially it's the number of sequences,
        // The clustering terminates when the number of clusters becomes 1
        private int _numberOfClusters;

        // The index of the cloest pair of clusters
        private int _nextA, _nextB;

        // Incrementally indicates the next generated cluster ID
        // and use it as the node ID which represent the new cluster
        private int _currentClusterID;

        // Temperary distance variables when choosing the closest pair
        private float _currentDistance, _smallestDistance;

        #endregion

        #region Properties
        /// <summary>
        /// The node list of this class
        /// </summary>
        override public List<BinaryGuideTreeNode> Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// The edge list of this class
        /// </summary>
        override public List<BinaryGuideTreeEdge> Edges
        {
            get { return _edges; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct clusters based on distance matrix.
        /// The default distance update method is 'average'
        /// </summary>
        /// <param name="distanceMatrix">IDistanceMatrix</param>
        public HierarchicalClusteringParallel(IDistanceMatrix distanceMatrix)
            : this(distanceMatrix, UpdateDistanceMethodsTypes.Average)
        {
        }

        /// <summary>
        /// Construct clusters using different update methods
        /// </summary>
        /// <param name="distanceMatrix">IDistanceMatrix</param>
        /// <param name="updateDistanceMethodName">enum EUpdateDistanceMethods</param>
        public HierarchicalClusteringParallel(IDistanceMatrix distanceMatrix, UpdateDistanceMethodsTypes updateDistanceMethodName)
            : base(distanceMatrix, updateDistanceMethodName)
        {
            // Initialize the clusters
            Initialize(distanceMatrix);

            // Clustering...
            while (_numberOfClusters > 1)
            {
                try
                {
                    GetNextPairOfCluster(distanceMatrix);
                    CreateCluster(distanceMatrix);
                    UpdateClusters();
                    UpdateDistance(distanceMatrix);
                }
                catch (OutOfMemoryException ex)
                {
                    throw new Exception("Our of memory", ex.InnerException);
                }
            }
        }
        #endregion

        #region Clustering related methods
        /// <summary>
        /// Initialize: make each sequence a cluster
        /// </summary>
        /// <param name="distanceMatrix">distance matrix</param>
        private void Initialize(IDistanceMatrix distanceMatrix)
        {
            _numberOfClusters = distanceMatrix.Dimension;
            for (int i = 0; i < _numberOfClusters; ++i)
            {
                // Both node ID and sequence ID equal to the sequence index
                _nodes.Add(new BinaryGuideTreeNode(i));
                _clusters.Add(i);
            }
            _currentClusterID = distanceMatrix.Dimension - 1;
        }

        /// <summary>
        /// O(N) algorithm to get the next closest pair of clusters
        /// </summary>
        /// <param name="distanceMatrix">distance matrix</param>
        private void GetNextPairOfCluster(IDistanceMatrix distanceMatrix)
        {
            _smallestDistance = float.MaxValue;

            Parallel.ForEach(_clusters, PAMSAMMultipleSequenceAligner.ParallelOption, i =>
            {
                int currentIndex = _nodes[i].SequenceID;
                _currentDistance = distanceMatrix.NearestDistances[currentIndex];
                if (_currentDistance < _smallestDistance)
                {
                    _smallestDistance = _currentDistance;
                    _nextA = i;
                    _nextB = _indexToCluster[distanceMatrix.NearestNeighbors[currentIndex]];
                }
            });
        }

        /// <summary>
        /// Combine cluster nextA and nextB into a new cluster
        /// </summary>
        /// <param name="distanceMatrix">distance matrix</param>
        private void CreateCluster(IDistanceMatrix distanceMatrix)
        {
            BinaryGuideTreeNode node = new BinaryGuideTreeNode(++_currentClusterID);

            // link the two nodes nextA and nextB with the new node
            node.LeftChildren = Nodes[_nextA];
            node.RightChildren = Nodes[_nextB];
            Nodes[_nextA].Parent = node;
            Nodes[_nextB].Parent = node;

            // use the leftmost leave's sequenceID
            int next = Math.Min(_nextA, _nextB);
            node.SequenceID = Nodes[next].SequenceID;
            _indexToCluster[node.SequenceID] = _currentClusterID;

            Nodes.Add(node);

            // Add edges
            BinaryGuideTreeEdge edgeA = new BinaryGuideTreeEdge(Nodes[_nextA].ID);
            BinaryGuideTreeEdge edgeB = new BinaryGuideTreeEdge(Nodes[_nextB].ID);

            edgeA.ParentNode = node;
            edgeB.ParentNode = node;
            edgeA.ChildNode = Nodes[_nextA];
            edgeB.ChildNode = Nodes[_nextB];

            Nodes[_nextA].ParentEdge = edgeA;
            Nodes[_nextB].ParentEdge = edgeB;

            // the length of the edge is the percent identity of two node sequences
            // or the average of identities between two sets of sequences
            //_edge1.Length = KimuraDistanceScoreCalculator.calculateDistanceScore(
            //    seqs[nodes[nextA].sequenceID], seqs[nodes[nextB].sequenceID]);

            // modified: define kimura distance as sequence distance
            edgeA.Length = _smallestDistance;
            edgeB.Length = _smallestDistance;

            _edges.Add(edgeA);
            _edges.Add(edgeB);
        }

        /// <summary>
        /// Update the distance between the new cluster with the rest clusters
        /// </summary>
        /// <param name="distanceMatrix">distance matrix</param>
        private void UpdateDistance(IDistanceMatrix distanceMatrix)
        {
            _smallestDistance = float.MaxValue;

            int nextIndex = Nodes[_currentClusterID].SequenceID;

            distanceMatrix.NearestDistances[nextIndex] = float.MaxValue;

            Parallel.ForEach(_clusters, PAMSAMMultipleSequenceAligner.ParallelOption, i =>
            {
                if (i != _currentClusterID)
                {
                    int currentIndex = Nodes[i].SequenceID;

                    // Update distance of the newly merged cluster with another cluster
                    _currentDistance = _updateDistanceMethod(distanceMatrix, Nodes[_nextA].SequenceID, Nodes[_nextB].SequenceID, currentIndex);

                    // Update distance matrix
                    distanceMatrix[currentIndex, nextIndex] = _currentDistance;

                    // Update distance matrix NearestNeighbors
                    if (distanceMatrix.NearestNeighbors[currentIndex] == Nodes[_nextA].SequenceID || distanceMatrix.NearestNeighbors[currentIndex] == Nodes[_nextB].SequenceID)
                    {
                        // Update NearestDistance and NearestNeighbors for column currentIndex
                        UpdateNearestColumn(distanceMatrix, currentIndex);
                    }
                }
            });
        }

        /// <summary>
        /// Update clusters:
        /// remove clusters nextA and nextB and add a new merged cluster[nextA, nextB]
        /// </summary>
        private void UpdateClusters()
        {
            _clusters.Remove(_nextA);
            _clusters.Remove(_nextB);
            _clusters.Add(_currentClusterID);
            --_numberOfClusters;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the nearest neighbor and nearest distance of a column in a distance matrix 
        /// </summary>
        /// <param name="distanceMatrix">distance matrix</param>
        /// <param name="column">zero-based integer</param>
        private void UpdateNearestColumn(IDistanceMatrix distanceMatrix, int column)
        {
            float min = float.MaxValue;
            foreach (int i in _clusters)
            {
                int currentIndex = Nodes[i].SequenceID;
                if (currentIndex != column)
                {
                    if (distanceMatrix[currentIndex, column] < min)
                    {
                        distanceMatrix.NearestDistances[column] = min;
                        distanceMatrix.NearestNeighbors[column] = currentIndex;
                    }
                }
            }
        }
        #endregion
    }
}
