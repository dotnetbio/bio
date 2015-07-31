using Bio.Algorithms.Alignment;

namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface write an ISequenceAlignment to a particular location, usually a
    /// file. The output is formatted according to the particular file format. A method is
    /// also provided for quickly accessing the content in string form for applications that do not
    /// need to first write to file.
    /// </summary>
    public interface ISequenceAlignmentFormatter : IFormatter<ISequenceAlignment>
    {
    }
}
