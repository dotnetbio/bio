using System.Collections.Generic;
using Bio.IO.Bed;
using System;
using System.Linq;

using Bio.Registration;

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
        private static readonly BedParser bed = new BedParser();

        /// <summary>
        /// List of all supported Range-Parsers.
        /// </summary>
        private static readonly List<ISequenceRangeParser> KnownParsers = new List<ISequenceRangeParser> { bed };

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
        public static IReadOnlyList<ISequenceRangeParser> All
        {
            get { return KnownParsers; }
        }

        /// <summary>
        /// Initializes static members of the SequenceRangeParsers class.
        /// </summary>
        static SequenceRangeParsers()
        {
            // get the registered parsers
            IEnumerable<ISequenceRangeParser> registeredParsers = GetSequenceRangeParsers();
            if (null != registeredParsers)
            {
                foreach (var parser in registeredParsers.Where(parser => 
                        parser != null && KnownParsers.All(sp => string.Compare(sp.Name, parser.Name, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    KnownParsers.Add(parser);
                }
            }
        }

        /// <summary>
        /// Gets all the registered ISequenceRangeParser types.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<ISequenceRangeParser> GetSequenceRangeParsers()
        {
            var registeredParsers = new List<ISequenceRangeParser>();
            var implementations = BioRegistrationService.LocateRegisteredParts<ISequenceRangeParser>();

            foreach (var impl in implementations)
            {
                try
                {
                    ISequenceRangeParser parser = Activator.CreateInstance(impl) as ISequenceRangeParser;
                    if (parser != null)
                        registeredParsers.Add(parser);
                }
                catch
                {
                    // Cannot create - no default ctor?
                }
            }

            return registeredParsers;
        }
    }
}
