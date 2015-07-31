using System.Collections.Generic;
using System;

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
    public abstract class HierarchicalClustering : IHierarchicalClustering
    {
        #region Fields
        /// <summary>
        /// The node list in the generated binary tree
        /// </summary>
        protected List<BinaryGuideTreeNode> _nodes = null;

        /// <summary>
        /// The edge list 
        /// </summary>
        protected List<BinaryGuideTreeEdge> _edges = null;
        
        /// <summary>
        /// Delegate function for updating distances
        /// </summary>
        protected UpdateDistanceMethodSelector _updateDistanceMethod;

        /// <summary>
        /// The list stores the current clusters generated
        /// </summary>
        protected List<int> _clusters = null;

        /// <summary>
        /// Record the index of cluster using the index in the distance matrix
        /// </summary>
        protected int[] _indexToCluster = null;

        #endregion

        #region Properties

        /// <summary>
        /// The node list of this class
        /// </summary>
        virtual public List<BinaryGuideTreeNode> Nodes 
        { 
            get { return _nodes; } 
        }

        /// <summary>
        /// The edge list of this class
        /// </summary>
        virtual public List<BinaryGuideTreeEdge> Edges 
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
        public HierarchicalClustering(IDistanceMatrix distanceMatrix) 
                        : this(distanceMatrix, UpdateDistanceMethodsTypes.Single)
        {
        }

        /// <summary>
        /// Construct clusters using different update methods
        /// </summary>
        /// <param name="distanceMatrix">IDistanceMatrix</param>
        /// <param name="updateDistanceMethodName">enum EUpdateDistanceMethods</param>
        public HierarchicalClustering(IDistanceMatrix distanceMatrix, UpdateDistanceMethodsTypes updateDistanceMethodName)
        {
            if (distanceMatrix.Dimension <= 0)
            {
                throw new Exception("Invalid distance matrix dimension");
            }

            try
            {
                // The number of nodes in the final tree is 2N-2:
                // N sequence nodes (leaves) and N-2 internal nodes
                // where N is the number of input sequences
                _nodes = new List<BinaryGuideTreeNode>(distanceMatrix.Dimension * 2 - 1);
                _edges = new List<BinaryGuideTreeEdge>(distanceMatrix.Dimension * 2 - 2);

                // The number of clusters is the number of leaves at the beginning
                // As the algorithm merges clusters, only one cluster remains.
                _clusters = new List<int>(distanceMatrix.Dimension);
                
                // Construct _indexToCluster
                _indexToCluster = new int[distanceMatrix.Dimension];
                for (int i = 0; i < distanceMatrix.Dimension; ++i)
                {
                    _indexToCluster[i] = i;
                }
            }
            catch (OutOfMemoryException ex)
            {
                throw new Exception("Out of memory", ex.InnerException);
            }

            // Choose a update-distance method
            switch(updateDistanceMethodName)
            {
                case(UpdateDistanceMethodsTypes.Average):
                    _updateDistanceMethod = new UpdateDistanceMethodSelector(UpdateAverage);
                    break;
                case(UpdateDistanceMethodsTypes.Single):
                    _updateDistanceMethod = new UpdateDistanceMethodSelector(UpdateSingle);
                    break;
                case(UpdateDistanceMethodsTypes.Complete):
                    _updateDistanceMethod = new UpdateDistanceMethodSelector(UpdateComplete);
                    break;
                case(UpdateDistanceMethodsTypes.WeightedMAFFT):
                    _updateDistanceMethod = new UpdateDistanceMethodSelector(UpdateWeightedMAFFT);
                    break;
                default:
                    throw new Exception("invalid update method");
            }
        }
        #endregion

        #region Update cluster methods

        // Check out enum UpdateDistanceMethodsTypes for details
        /// <summary>
        /// arithmetic average of distance[nextA,other] and distance[nextB,other]
        /// </summary>
        /// <param name="distanceMatrix">distance matrix for the cluster</param>
        /// <param name="nextA">integer number of sequence 1 to be clustered next</param>
        /// <param name="nextB">integer number of sequence 2 to be clustered next</param>
        /// <param name="other">the other cluster whose distance will be updated</param>
        protected float UpdateAverage(IDistanceMatrix distanceMatrix, int nextA, int nextB, int other)
        {
            return (distanceMatrix[nextA, other] + distanceMatrix[nextB, other]) / 2;
        }

        /// <summary>
        /// Minimum of distance[nextA,other] and distance[nextB,other]
        /// </summary>
        /// <param name="distanceMatrix">distance matrix for the cluster</param>
        /// <param name="nextA">integer number of sequence 1 to be clustered next</param>
        /// <param name="nextB">integer number of sequence 2 to be clustered next</param>
        /// <param name="other">the other cluster whose distance will be updated</param>
        protected float UpdateSingle(IDistanceMatrix distanceMatrix, int nextA, int nextB, int other)
        {
            return Math.Min(distanceMatrix[nextA, other], distanceMatrix[nextB, other]);
        }

        /// <summary>
        /// Maximum of distance[nextA,other] and distance[nextB,other]
        /// </summary>
        /// <param name="distanceMatrix">distance matrix for the cluster</param>
        /// <param name="nextA">integer number of sequence 1 to be clustered next</param>
        /// <param name="nextB">integer number of sequence 2 to be clustered next</param>
        /// <param name="other">the other cluster whose distance will be updated</param>
        protected float UpdateComplete(IDistanceMatrix distanceMatrix, int nextA, int nextB, int other)
        {
            return Math.Max(distanceMatrix[nextA, other], distanceMatrix[nextB, other]);
        }

        //*****************************************
        // MAFFT: multiple sequence alignment program
        // Copyright (c) 2006 Kazutaka Katoh
        //
        // Redistribution and use in source and binary forms,
        // with or without modification, are permitted provided
        // that the following conditions are met:
        //
        // Redistributions of source code must retain the
        // above copyright notice, this list of conditions
        // and the following disclaimer.  Redistributions in
        // binary form must reproduce the above copyright
        // notice, this list of conditions and the following
        // disclaimer in the documentation and/or other
        // materials provided with the distribution.
        //
        // The name of the author may not be used to endorse
        // or promote products derived from this software without
        // specific prior written permission.
        //
        // THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS"
        // AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
        // BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
        // MERCHANTABILITY AND FITNESS FOR A PARTICULAR
        // PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
        // AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
        // INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
        // DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
        // OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
        // OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
        // AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
        // STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
        // OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
        // THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
        // OF SUCH DAMAGE.
        //*********************************************
        /// <summary>
        /// Adapted from MAFFT software:
        /// weighted mixture of minimum and average linkage 
        /// d = (1-s)*d_min + s*d_avg
        /// where s is 0.1 by default
        /// </summary>
        /// 
        /// <param name="distanceMatrix">distance matrix for the cluster</param>
        /// <param name="nextA">integer number of sequence 1 to be clustered next</param>
        /// <param name="nextB">integer number of sequence 2 to be clustered next</param>
        /// <param name="other">the other cluster whose distance will be updated</param>
        protected float UpdateWeightedMAFFT(IDistanceMatrix distanceMatrix, int nextA, int nextB, int other)
        {
            return (float)(0.9*Math.Min(distanceMatrix[nextA, other], distanceMatrix[nextB, other]) 
                        + 0.1*(distanceMatrix[nextA, other] + distanceMatrix[nextB, other]) / 2);
        }
        #endregion
    }
}
