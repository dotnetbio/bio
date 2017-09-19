using System.Collections.Generic;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Aligners will implements this interface to list the attributes supported
    /// or required.
    /// </summary>
    public interface IAlignmentAttributes
    {
        /// <summary>
        /// Gets list of attributes.
        /// </summary>
        Dictionary<string, AlignmentInfo> Attributes { get; }
    }
}
