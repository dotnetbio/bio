namespace Bio.IO.AppliedBiosystems.DataParsers
{
    /// <summary>
    /// Handle's parsing of a abi parser context based on a specific file version.
    /// </summary>
    public interface IVersionedDataParser
    {
        /// <summary>
        /// Parser data.
        /// </summary>
        /// <param name="context"></param>
        void ParseData(IParserContext context);
    }
}
