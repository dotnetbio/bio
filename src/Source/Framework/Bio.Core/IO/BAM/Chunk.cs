using System.Collections.Generic;
using System;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to hold start and end offsets of a BAM file chunk related to a bin.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// Gets or sets start offset of this chunk.
        /// </summary>
        public FileOffset ChunkStart { get; set; }

        /// <summary>
        /// Gets or sets end offset of this chunk.
        /// </summary>
        public FileOffset ChunkEnd { get; set; }
    }

    /// <summary>
    /// Singleton Class to Sort Chunks to help merging of chunks.
    /// </summary>
    public class ChunkSorterForMerging : IComparer<Chunk>
    {
        /// <summary>
        /// Static instance to maintain single instance.
        /// </summary>
        private static ChunkSorterForMerging Instance = new ChunkSorterForMerging();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ChunkSorterForMerging() { }

        /// <summary>
        /// Gets the singleton instance of the ChunkSorterForMerging class.
        /// </summary>
        public static ChunkSorterForMerging GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Compares two Chunks in the following order.
        ///   Compares X.ChunkStart with Y.ChunkStart if the result is non zero then returns the result.
        ///   else compares Y.ChunnkEnd with X.ChunkEnd and returns the result.
        /// </summary>
        /// <param name="x">First chunk to compare.</param>
        /// <param name="y">Second chunk to compare.</param>
        public int Compare(Chunk x, Chunk y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            int result = x.ChunkStart.CompareTo(y.ChunkStart);
            if (result == 0)
            {
                // compare y to x so that if the x.ChunkEnd is more than Y.ChunkEnd then X should appear first.
                result = y.ChunkEnd.CompareTo(x.ChunkEnd);
            }

            return result;
        }
    }
}
