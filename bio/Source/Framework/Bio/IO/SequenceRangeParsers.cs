using System.Collections.Generic;
using Bio.IO.Bed;
using System;
using System.Linq;

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

        #region Constructors
#if !SILVERLIGHT
        /// <summary>
        /// Initializes static members of the SequenceRangeParsers class.
        /// </summary>
        static SequenceRangeParsers()
        {
            // get the registered parsers
            IList<ISequenceRangeParser> registeredParsers = GetSequenceRangeParsers();
            if (null != registeredParsers)
            {
                foreach (var parser in registeredParsers.Where(parser => 
                        parser != null && !all.Any(sp => string.Compare(sp.Name, parser.Name, StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    all.Add(parser);
                }
            }
        }
#endif
        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// Gets all registered parsers in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered parsers.</returns>
        private static IList<ISequenceRangeParser> GetSequenceRangeParsers()
        {
            IList<ISequenceRangeParser> registeredParsers = new List<ISequenceRangeParser>();

            IList<ISequenceRangeParser> addInParsers = Registration.RegisteredAddIn.GetComposedInstancesFromAssemblyPath<ISequenceRangeParser>(
                        ".NetBioSequenceRangeParsersExport", Registration.RegisteredAddIn.AddinFolderPath, Registration.RegisteredAddIn.DLLFilter);
            if (null != addInParsers && addInParsers.Count > 0)
            {
                foreach (ISequenceRangeParser parser in
                    addInParsers.Where(parser => parser != null
                        && !registeredParsers.Any(sp =>
                            string.Compare(sp.Name, parser.Name,
                                StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    registeredParsers.Add(parser);
                }
            }

            return registeredParsers;
        }
#endif
    }
}
