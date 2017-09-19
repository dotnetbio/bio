using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of Hierarchical Clustering interface.
    /// </summary>
    public interface IHierarchicalClustering
    {
        /// <summary>
        /// The node list of the interface
        /// </summary>
        List<BinaryGuideTreeNode> Nodes { get; }

        /// <summary>
        /// The edge list of the interface
        /// </summary>
        List<BinaryGuideTreeEdge> Edges { get; }
    }

    /// <summary>
    /// The enum of hierarchical clustering update methods,
    /// which update the distances between newly merged cluster[nextA,nextB] and a single cluster [other]
    /// </summary>
    public enum UpdateDistanceMethodsTypes
    {
        /// <summary>
        /// {average} : 
        /// arithmetic average of distance[nextA,other] and distance[nextB,other]
        /// </summary>
        Average,

        /// <summary>
        /// {single}  : 
        /// minimum of distance[nextA,other] and distance[nextB,other]
        /// </summary>
        Single,

        /// <summary>
        /// {complete}: 
        /// maximum of distance[nextA,other] and distance[nextB,other]
        /// </summary>
        Complete,

        /// <summary>
        /// Adapted from MAFFT software:
        /// weighted mixture of minimum and average linkage 
        /// d = (1-s)*d_min + s*d_avg
        /// where s is 0.1 by default
        /// </summary>
        WeightedMAFFT
    }

    /// <summary>
    /// The delegate function of methods to update the distances between newly merged cluster[nextA,nextB] 
    /// and a single cluster [other].
    /// 
    /// There are three basic different methods avaialbe:
    ///     {average} : arithmetic average of distance[nextA,other] and distance[nextB,other]
    ///     {single}  : minimum of distance[nextA,other] and distance[nextB,other]
    ///     {complete}: maximum of distance[nextA,other] and distance[nextB,other]
    /// and a modified method:
    ///     {weightedMAFFT}: mixture of single and average
    /// </summary>
    /// <param name="distanceMatrix">distance matrix for the cluster</param>
    /// <param name="nextA">integer number of sequence 1 to be clustered next</param>
    /// <param name="nextB">integer number of sequence 2 to be clustered next</param>
    /// <param name="other">the other cluster whose distance will be updated</param>
    public delegate float UpdateDistanceMethodSelector(IDistanceMatrix distanceMatrix, int nextA, int nextB, int other);

}
