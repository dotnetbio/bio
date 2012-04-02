using System.Collections.Generic;
using Bio.IO.Bed;
using System;
using System.Linq;

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

                #region Constructors
#if !SILVERLIGHT
        /// <summary>
        /// Initializes static members of the SequenceRangeFormatters class.
        /// </summary>
        static SequenceRangeFormatters()
        {
            // get the registered parsers
            IList<ISequenceRangeFormatter> registeredFormatters = GetSequenceRangeFormatters();
            if (null != registeredFormatters)
            {
                foreach (var formatter in registeredFormatters.Where(
                    fmt => fmt != null && !all.Any(sfmt => 
                        string.Compare(sfmt.Name, fmt.Name, StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    all.Add(formatter);
                }
            }
        }
#endif
        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// Gets all registered formatters in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered parsers.</returns>
        private static IList<ISequenceRangeFormatter> GetSequenceRangeFormatters()
        {
            IList<ISequenceRangeFormatter> registeredFormatters = new List<ISequenceRangeFormatter>();

            IList<ISequenceRangeFormatter> addInFormatters = Registration.RegisteredAddIn.GetComposedInstancesFromAssemblyPath<ISequenceRangeFormatter>(
                        ".NetBioSequenceRangeFormattersExport", Registration.RegisteredAddIn.AddinFolderPath, Registration.RegisteredAddIn.DLLFilter);
            if (null != addInFormatters && addInFormatters.Count > 0)
            {
                foreach (ISequenceRangeFormatter fmt in
                    addInFormatters.Where(sfmt => sfmt != null 
                        && !registeredFormatters.Any(sp => string.Compare(sp.Name, sfmt.Name, StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    registeredFormatters.Add(fmt);
                }
            }

            return registeredFormatters;
        }
#endif
    }
}
