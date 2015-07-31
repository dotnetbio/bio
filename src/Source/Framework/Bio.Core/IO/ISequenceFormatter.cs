namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface write an ISequence to a particular location, usually a
    /// file. The output is formatted according to the particular file format.
    /// </summary>
    public interface ISequenceFormatter : IFormatter<ISequence>
    {
    }
}
