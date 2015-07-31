using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of modified CLUSTALW sequence weighting method.
    /// 
    /// MUSCLE (Edgar 2004) showed that using sequence weights slightly
    /// improves the accuracy results by 1% on BAliBASE, and there's little
    /// difference between the alternative schemes. Thus sequence weighting
    /// is only an option and modified CLUSTALW method is implemented.
    /// 
    /// The weights are constructed from guide tree. The weights of each 
    /// internal node is the average (or min, max, depending on the clustering
    /// method) of the subtree. The weight of each sequence (leave of the tree)
    /// is the summation of the weights of the edge length through the leave to the root.
    /// 
    /// The final weights are normalized so that the average weight is 1.
    /// </summary>
    public sealed class SequenceWeighting
    {
        #region Fields

        // The weights of each sequence (leaves of the tree)
        private float[] _weights = null;
        #endregion

        #region Properties
        /// <summary>
        /// The weights of this class
        /// </summary>
        public float[] Weights
        {
            get { return _weights; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Calculate sequence weights from the guide tree
        /// </summary>
        /// <param name="tree">a binary guide tree</param>
        public SequenceWeighting(BinaryGuideTree tree)
        {
            _weights = new float[tree.NumberOfLeaves];

            BinaryGuideTreeEdge _edge;
            BinaryGuideTreeNode _node;

            // Initialize: all weights are 0.
            // Then sum up the weights from the leaf to the root
            for (int i = 0; i < _weights.Length; ++i)
            {
                _weights[i] = 0;
                _node = tree.Nodes[i];
                while (!_node.IsRoot)
                {
                    _edge = _node.ParentEdge;
                    _weights[i] += _edge.Length;
                    _node = _node.Parent;
                }
            }

            // Normalize so that the average is 1.            
            float s = 0;
            for (int i = 0; i < _weights.Length; ++i)
            {
                s += _weights[i];
            }
            for (int i = 0; i < _weights.Length; ++i)
            {
                _weights[i] = _weights[i] * _weights.Length / s;
                _weights[i] = 1 / Weights[i];
            }
            
        }
        #endregion
    }
}
