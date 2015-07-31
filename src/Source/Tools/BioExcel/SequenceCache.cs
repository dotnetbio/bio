namespace BiodexExcel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Office.Interop.Excel;

    /// <summary>
    /// Cache implementation for storing sequences and query regions.
    /// Stores everything as objects so can take anything for that matter.
    /// Cache is limited with a maximum size, when more items comes in, it keeps the recently accessed items and removes the last one.
    /// Maximum size is the number of sheets in the workbook + CACHE_BUFFER_SIZE
    /// </summary>
    public static class SequenceCache
    {
        /// <summary>
        /// Number of items to be added to the number of sheets present to determine the maximum number of items to be stored in cache.
        /// </summary>
        private const int CACHE_BUFFER_SIZE = 20;

        /// <summary>
        /// Dictionary which holds the objects which is cached
        /// </summary>
        private static Dictionary<string, CachedSequenceData> cache = new Dictionary<string, CachedSequenceData>();

        /// <summary>
        /// Field to indicate if we've initialized the sequence cache.
        /// </summary>
        public static bool IsInitialized
        {
            get; private set;
        }

        /// <summary>
        /// Tries to get a sequence from the cache for the given range
        /// </summary>
        /// <param name="selectedRange">Range for which the request is made</param>
        /// <param name="inputParamsAsKey"></param>
        /// <returns>Returns the sequence object or null if not found</returns>
        public static object TryGetSequence(Range selectedRange, string inputParamsAsKey)
        {
            RemoveDirtyItems();

            string key = GetRangeAddress(selectedRange, inputParamsAsKey);
            lock (typeof(SequenceCache))
            {
                if (cache.ContainsKey(key))
                {
                    CachedSequenceData seq = cache[key];
                    seq.LastAccessTime = DateTime.Now;
                    return seq.CachedData;
                }
                return null;
            }
        }

        /// <summary>
        /// Tries to get a sequence from the cache for the given range list
        /// </summary>
        /// <param name="selectedRanges">Range list for which the request is made</param>
        /// <param name="inputParamsAsKey"></param>
        /// <returns>Returns the sequence object or null if not found</returns>
        public static object TryGetSequence(IEnumerable<Range> selectedRanges, string inputParamsAsKey)
        {
            RemoveDirtyItems();

            string key = GetRangeAddress(selectedRanges, inputParamsAsKey);
            if (cache.ContainsKey(key))
            {
                CachedSequenceData seq = cache[key];
                seq.LastAccessTime = DateTime.Now;
                return seq.CachedData;
            }

            return null;
        }


        /// <summary>
        /// Add a new item to the cache
        /// </summary>
        /// <param name="selectedRange">Range to use as the key for caching</param>
        /// <param name="dataToCache">Object to be cached</param>
        /// <param name="inputParamsAsKey">Input param names as key</param>
        /// <returns>True if caching was successful</returns>
        public static bool Add(Range selectedRange, object dataToCache, string inputParamsAsKey)
        {
            CleanUp(); // cleanup as cache may be full or this sheet may be marked dirty already

            string key = GetRangeAddress(selectedRange, inputParamsAsKey);

            if (cache.ContainsKey(key)) // remove if exists
            {
                cache[key] = new CachedSequenceData(dataToCache);
            }
            else
            {
                cache.Add(key, new CachedSequenceData(dataToCache));
            }

            return true;
        }


        /// <summary>
        /// Add a new item to the cache
        /// </summary>
        /// <param name="selectedRanges">Range to use as the key for caching</param>
        /// <param name="dataToCache">Object to be cached</param>
        /// <param name="inputParamsAsKey">Input param names as key</param>
        /// <returns>True if caching was successful</returns>
        public static bool Add(IEnumerable<Range> selectedRanges, object dataToCache, string inputParamsAsKey)
        {
            CleanUp(); // cleanup as cache may be full or this sheet may be marked dirty already

            string key = GetRangeAddress(selectedRanges, inputParamsAsKey);
            if (cache.ContainsKey(key)) // remove if exists
            {
                cache[key] = new CachedSequenceData(dataToCache);
            }
            else
            {
                cache.Add(key, new CachedSequenceData(dataToCache));
            }
            return true;
        }

        public static void Initialize()
        {
            IsInitialized = true;
            activeSheets.AddRange(Globals.ThisAddIn.Application.Worksheets
                        .Cast<Worksheet>().Select(ws => ws.Name));
        }

        #region Cache cleanup section

        private static List<string> dirtySheets = new List<string>();
        private static List<string> activeSheets = new List<string>();

        /// <summary>
        /// Event raised when any data in any open sheet has changed, so as to cleanup the cache if anything has got dirty.
        /// </summary>
        /// <param name="sheet">Sheet which raised the event.</param>
        /// <param name="target">Range which raised the event.</param>
        public static void OnSheetDataChanged(object sheet, Range target)
        {
            dirtySheets.Add(target.Worksheet.Name);
        }

        /// <summary>
        /// Does a cleanup of cache
        /// </summary>
        private static void RemoveDirtyItems()
        {
            // Get the currently loaded worksheets.
            var currentSheets = new List<string>(Globals.ThisAddIn.Application.Worksheets
                                                        .Cast<Worksheet>().Select(ws => ws.Name));

            // Mark any previously known sheets as dirty.
            dirtySheets.AddRange(activeSheets.Where(ws => !currentSheets.Contains(ws)));

            // Swap active sheets with latest set of sheets
            activeSheets = currentSheets; 

            foreach (string key in dirtySheets.Distinct()
                .Select(changedSheet => cache.Keys.Where(key => key.Contains(changedSheet)).ToList())
                .SelectMany(keysToRemove => keysToRemove))
                cache.Remove(key);

            dirtySheets.Clear();
        }

        /// <summary>
        /// Removes one item from cache when the cache hits the maximum size and a new item is to be put inside.
        /// </summary>
        private static void CleanUp()
        {
            RemoveDirtyItems(); // remove dirty and re-check if still cache is full
            if (cache.Count >= Globals.ThisAddIn.Application.Worksheets.Count + CACHE_BUFFER_SIZE)
            {
                // If cache size exceeded the limit, remove one item keeping the recent items.
                CachedSequenceData itemToRemove = null;
                string keyToRemove = string.Empty;
                foreach (var item in cache)
                {
                    if (itemToRemove == null)
                    {
                        itemToRemove = item.Value;
                        keyToRemove = item.Key;
                    }
                    if (itemToRemove.LastAccessTime > item.Value.LastAccessTime)
                    {
                        itemToRemove = item.Value;
                        keyToRemove = item.Key;
                    }
                }

                cache.Remove(keyToRemove);
            }
        }

        #endregion

        /// <summary>
        /// Gets the address for a range list.
        /// </summary>
        public static string GetRangeAddress(IEnumerable<Range> ranges, string inputParamsAsKey)
        {
            StringBuilder rangeAddress = new StringBuilder();

            foreach (Range currentRange in ranges)
            {
                rangeAddress.Append(GetRangeAddress(currentRange, ""));
                rangeAddress.Append(",");
            }

            rangeAddress.Remove(rangeAddress.Length - 1, 1);
            return rangeAddress.ToString() + inputParamsAsKey;
        }


        /// <summary>
        /// Gets the address of a range.
        /// </summary>
        public static string GetRangeAddress(Range range, string inputParamsAsKey)
        {
            StringBuilder rangeAddress = new StringBuilder();

            foreach (Range r in range.Areas)
            {
                rangeAddress.Append(r.Worksheet.Name);
                rangeAddress.Append("!");
                rangeAddress.Append(r.Address.Replace("$", ""));
                rangeAddress.Append(GetRowsColsKey(r));
                rangeAddress.Append(",");
            }
            rangeAddress.Remove(rangeAddress.Length - 1, 1);

            return rangeAddress.ToString() + inputParamsAsKey;
        }

        /// <summary>
        /// Create a string to append to the key for caching a sequence.
        /// User selected ranges can contain hidden rows and columns, 
        /// so need to store the list of hidden rows and columns in the key.
        /// </summary>
        /// <param name="targetRange">Range of which key is needed</param>
        /// <returns>String with data about hidden rows and columns in the given range.</returns>
        private static string GetRowsColsKey(Range targetRange)
        {
            StringBuilder key = new StringBuilder();
            bool enteredHiddenSection = false; // Flag to mark if current position is inside a set of hidden indexes
            ulong startingHiddenIndex = 0; // For each hidden bracket, this holds the starting index

            key.Append("R-"); // to denote row indexes
            foreach (Range r in targetRange.Rows)
            {
                if (((bool)r.EntireRow.Hidden) == true)
                {
                    if (!enteredHiddenSection) // If previous row was not hidden
                    {
                        enteredHiddenSection = true;
                        key.Append(r.Row.ToString());
                        startingHiddenIndex = (ulong)r.Row;
                    }
                }
                else if (enteredHiddenSection) // Was in hidden section and now exited from it
                {
                    enteredHiddenSection = false;
                    if (startingHiddenIndex != (ulong)(r.Row - 1)) // If more than one hidden row in a stretch, use R:R format
                    {
                        key.Append(":");
                        key.Append(r.Row);
                    }
                    key.Append(",");
                }
            }

            // Same logic above: now for columns
            enteredHiddenSection = false;
            startingHiddenIndex = 0;

            key.Append("C-"); // to denote column indexes
            foreach (Range r in targetRange.Columns)
            {
                if (((bool)r.EntireColumn.Hidden) == true)
                {
                    if (!enteredHiddenSection) // If previous column was not hidden
                    {
                        enteredHiddenSection = true;
                        key.Append(r.Column.ToString());
                        startingHiddenIndex = (ulong)r.Column;
                    }
                }
                else if (enteredHiddenSection) // Was in hidden section and now exited from it
                {
                    enteredHiddenSection = false;
                    if (startingHiddenIndex != (ulong)(r.Column - 1)) // If more than one hidden column in a stretch, use C:C format
                    {
                        key.Append(":");
                        key.Append(r.Column);
                    }
                    key.Append(",");
                }
            }

            key.Remove(key.Length - 1, 1); // remove last comma
            return key.ToString();
        }
    }
}
