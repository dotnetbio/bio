using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of profile interface. 
    /// 
    /// Profile is a multiple alignment treated as a sequence by 
    /// regarding each column as an alignable symbol. Thus two sets
    /// of sequences can be aligned by aligning profiles.
    /// 
    /// The symbol is a distribution vector, recording the frequencies
    /// of items in the column.
    /// 
    /// It requires the set of aligned sequences to generate profiles.
    /// 
    /// </summary>
    public interface IProfiles
    {
        /// <summary>
        /// profile is a [sequenceLength x itemSetLength] matrix
        /// with each column as a profile (distribution of items)
        /// </summary>
        List<float[]> ProfilesMatrix { get; set; }

        /// <summary>
        /// Access columns (profiles)
        /// </summary>
        /// <param name="col">zero-based column index</param>
        /// <returns></returns>
        float[] this[int col] { get; set; }

        /// <summary>
        /// The row dimension of profile matrix
        /// </summary>
        int RowSize { get; set; }

        /// <summary>
        /// The column dimension of profile matrix
        /// </summary>
        int ColumnSize { get; set; }

        /// <summary>
        /// Clear the memory
        /// </summary>
        void Clear();
    }
}
