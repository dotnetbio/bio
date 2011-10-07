using System.Collections.Generic;
using Bio.IO.Bed;

namespace Bio.IO
{
    /// <summary>
    /// SequenceRangeFormatter class is an abstraction class which provides instances
    /// and lists of all Range-Formatter currently supported by .NET Bio.
    /// </summary>
    public static class SequenceRangeFormatters
    {
        /// <summary>
        /// A singleton instance of BedFormatter class which is capable of
        /// saving a ISequenceRange according to the BED file format.
        /// </summary>
        private static BedFormatter bed = new BedFormatter();

        /// <summary>
        /// List of all supported Range-Formatters.
        /// </summary>
        private static List<ISequenceRangeFormatter> all = new List<ISequenceRangeFormatter>() { bed };

        /// <summary>
        /// Gets an instance of BedFormatter class which is capable of
        /// saving a ISequenceRange according to the BED file format.
        /// </summary>
        public static BedFormatter Bed
        {
            get
            {
                return bed;
            }
        }

        /// <summary>
        /// Gets the list of all range-formatters supported by the framework.
        /// </summary>
        public static IList<ISequenceRangeFormatter> All
        {
            get
            {
                return all.AsReadOnly();
            }
        }

    }
}
