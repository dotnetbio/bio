using System.Collections.Generic;
using Bio.IO.Bed;

namespace Bio.IO
{
    /// <summary>
    /// SequenceRangeParsers class is an abstraction class which provides instances
    /// and lists of all Range-Parsers currently supported by .NET Bio.
    /// </summary>
    public static class SequenceRangeParsers
    {
        /// <summary>
        /// A singleton instance of BedParser class which is capable of
        /// parsing BED format files.
        /// </summary>
        private static BedParser bed = new BedParser();

        /// <summary>
        /// List of all supported Range-Parsers.
        /// </summary>
        private static List<ISequenceRangeParser> all = new List<ISequenceRangeParser>() { bed };

        /// <summary>
        /// Gets an instance of BedParser class which is capable of
        /// parsing BED format files.
        /// </summary>
        public static BedParser Bed
        {
            get
            {
                return bed;
            }
        }

        /// <summary>
        /// Gets the list of all Range-parsers which is supported by the framework.
        /// </summary>
        public static IList<ISequenceRangeParser> All
        {
            get
            {
                return all.AsReadOnly();
            }
        }
    }
}
