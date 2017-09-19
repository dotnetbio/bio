using Bio.IO;

namespace Bio.Web.Blast
{
    /// <summary>
    /// This interface defines the contract that has to be implemented by a class
    /// that parse an output from blast service.
    /// Blast service can be in different format e.g., text / xml
    /// </summary>
    public interface IBlastParser : IParser<BlastResult>
    {
    }
}
