using System.IO;
using Bio.Algorithms.Alignment;

namespace Bio.IO
{
    /// <summary>
    /// Interface that defines a contract for parser to parse sequence alignment files. 
    /// For advanced users, the ability to select an encoding for the internal memory 
    /// representation is provided. Implementations also have a default encoding for 
    /// each alphabet that may be encountered.
    /// </summary>
    public interface ISequenceAlignmentParser : IParser<ISequenceAlignment>
    {
    }
}
