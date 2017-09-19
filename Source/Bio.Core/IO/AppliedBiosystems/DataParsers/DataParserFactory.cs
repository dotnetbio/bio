using System;
using System.Globalization;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems.DataParsers
{
    /// <summary>
    /// Creates abi parsers based on file format version.
    /// </summary>
    public static class DataParserFactory
    {
        /// <summary>
        /// Returns a parser for the specific version.
        /// </summary>
        /// <param name="majorVersion"></param>
        /// <returns></returns>
        public static IVersionedDataParser GetParser(int majorVersion)
        {
            if (majorVersion == V1DataParser.MajorVersion)
                return new V1DataParser();

            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, Resource.DataParserFactoryNoParserExistsForVersionFormat, majorVersion));
        }
    }
}
