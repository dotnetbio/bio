using System.Collections.Generic;
using Bio.IO.Bed;
using System;
using System.Linq;

using Bio.Registration;

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
        private static readonly BedFormatter bed = new BedFormatter();

        /// <summary>
        /// List of all supported Range-Formatters.
        /// </summary>
        private static readonly List<ISequenceRangeFormatter> KnownFormatters = new List<ISequenceRangeFormatter>() { bed };

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
        public static IReadOnlyList<ISequenceRangeFormatter> All
        {
            get
            {
                return KnownFormatters;
            }
        }

        /// <summary>
        /// Initializes static members of the SequenceRangeFormatters class.
        /// </summary>
        static SequenceRangeFormatters()
        {
            // get the registered parsers
            IEnumerable<ISequenceRangeFormatter> registeredFormatters = GetSequenceRangeFormatters();
            if (null != registeredFormatters)
            {
                foreach (var formatter in registeredFormatters.Where(
                    fmt => fmt != null && KnownFormatters.All(sfmt => 
                        string.Compare(sfmt.Name, fmt.Name, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    KnownFormatters.Add(formatter);
                }
            }
        }
        /// <summary>
        /// Gets all registered formatters in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered parsers.</returns>
        private static IEnumerable<ISequenceRangeFormatter> GetSequenceRangeFormatters()
        {
            IList<ISequenceRangeFormatter> registeredFormatters = new List<ISequenceRangeFormatter>();
            var implementations = BioRegistrationService.LocateRegisteredParts<ISequenceRangeParser>();

            foreach (var impl in implementations)
            {
                try
                {
                    ISequenceRangeFormatter formatter = Activator.CreateInstance(impl) as ISequenceRangeFormatter;
                    if (formatter != null)
                        registeredFormatters.Add(formatter);
                }
                catch
                {
                    // Cannot create - no default ctor?
                }
            }


            return registeredFormatters;
        }
    }
}
