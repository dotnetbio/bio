using System.Diagnostics;

namespace Bio.Algorithms.SuffixTree
{
    /// <summary>
    /// Structure to hold the suffix edge information.
    /// </summary>
    [DebuggerDisplay("StartIndex= {StartIndex}, IsLeaf= {IsLeaf}")]
    public struct MultiWaySuffixEdge
    {
        /// <summary>
        /// Gets or sets index of first symbol of this edge.
        /// </summary>
        public long StartIndex;

        /// <summary>
        /// Holds child edges.
        /// </summary>
        public MultiWaySuffixEdge[] Children;

        /// <summary>
        /// Holds suffix links.
        /// </summary>
        public MultiWaySuffixEdge[] SuffixLink;

        /// <summary>
        /// Initializes a new instance of the MultiWaySuffixEdge struct.
        /// </summary>
        /// <param name="startIndex">Index of first symbol of the edge.</param>
        public MultiWaySuffixEdge(long startIndex)
        {
            this.StartIndex = startIndex;
            Children = null;
            SuffixLink = null;
        }

        /// <summary>
        /// Gets a value indicating whether the edge is at the leaf.
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                // if there are no children then it is a leaf edge.
                return this.Children == null ? true : false;
            }
        }

        /// <summary>
        /// Gets or sets index of last character.
        /// </summary>
        /// <param name="sequenceLength">Length of the sequence for which 
        /// the suffix tree is created.</param>
        /// <returns>Returns endIndex of this edge.</returns>
        public long GetEndIndex(long sequenceLength)
        {
            if (this.Children != null)
            {
                // return the minimum start index of children -1
                return this.Children[0].StartIndex - 1;
            }

            // Sequence length + length of terminating symbol ($) -1.
            return sequenceLength;
        }
    }
}
