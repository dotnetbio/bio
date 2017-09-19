namespace Bio
{
    /// <summary>
    /// Sequence with qualitative data
    /// </summary>
    public interface IQualitativeSequence : ISequence
    {
        /// <summary>
        /// Gets the quality scores format type.
        /// Ex: Illumina/Solexa/Sanger.
        /// </summary>
        FastQFormatType FormatType { get;  }

        /// <summary>
        /// Gets the encoded quality score found at the specified index if within bounds. Note that the index value start at 0.
        /// </summary>
        /// <param name="index">Index at which the symbol is required.</param>
        /// <returns>Quality Score at the given index.</returns>
        byte GetEncodedQualityScore(long index);

        /// <summary>
        /// Gets the encoded quality scores.
        /// </summary>
        byte[] GetEncodedQualityScores();
    }
}