using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.IO.SAM;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class implements sorting in a SequenceAlignmentMap.
    /// Sorts the data in "SequenceAlignmentMap" By
    ///  ChromosomeCoordinates
    ///  Or
    ///  ReadNames
    /// </summary>
    public class BAMSort
    {
        /// <summary>
        /// Maximum limit of sorted bucket.
        /// Write the list to file once the limit is reached.
        /// The max count number is an optimized value based on local tests.
        /// </summary>
        private const int SortedListMaxCount = 100000;

        /// <summary>
        /// Holds SequenceAlignmentMap object to be sorted.
        /// </summary>
        private readonly SequenceAlignmentMap sequenceAlignMap;

        /// <summary>
        /// Type of sort needed.
        /// </summary>
        private readonly BAMSortByFields sortField;

        /// <summary>
        /// Default Constructor
        /// Initializes a new instance of the BAMSorter class
        /// </summary>
        /// <param name="seqAlignMap">SequenceAlignmentMap object to be sorted.</param>
        /// <param name="sortType">Type of sort required.</param>
        public BAMSort(SequenceAlignmentMap seqAlignMap, BAMSortByFields sortType)
        {
            sequenceAlignMap = seqAlignMap;
            sortField = sortType;
        }

        /// <summary>
        /// Sorts the SequenceAlignmentMap based on the sort by fields,
        /// either chromosome coordinates or read names and retuns sorted BAM indexer
        /// </summary>
        /// <example>
        /// 1. Sort by chromosome name.
        /// BAMSort sorter = new BAMSort([SequenceAlignmentMap], BAMSortByFields.ChromosomeCoordinates);
        /// IList&lt;BAMSortedIndex&gt; sortedGroups = sorter.Sort();
        /// foreach (BAMSortedIndex sortedGroup in sortedGroups)
        /// {
        ///     sortedGroup.GroupName // Containes the RName
        ///     foreach (int index in sortedGroup)
        ///     {
        ///         index // index of SequenceAlignmentMap.QuerySequences 
        ///     }
        /// }
        /// 2. Sort by read name.
        /// BAMSort sorter = new BAMSort([SequenceAlignmentMap], BAMSortByFields.ChromosomeCoordinates);
        /// IList&lt;BAMSortedIndex&gt; sortedGroups = sorter.Sort();
        /// foreach (int index in sortedGroups[0]) // There will be only Group in list.
        /// {
        ///     index // index of SequenceAlignmentMap.QuerySequences 
        /// }
        /// </example>
        /// <returns>sorted BAM indexer</returns>
        public IList<BAMSortedIndex> Sort()
        {
            IList<BAMSortedIndex> sortedIndices = new List<BAMSortedIndex>();

            switch (sortField)
            {
                case BAMSortByFields.ChromosomeNameAndCoordinates:
                
                    // Sort by Chromosomes (RName) and then by Positions (Pos) and retun "BAMSortedIndex"
                    // containing the indices of sorted SequenceAlignmentMap.QuerySequences items.
                    SortedDictionary<string, IList<string>> sortedFiles = SortByChromosomeCoordinates();

                    foreach (KeyValuePair<string, IList<string>> sortedFile in sortedFiles)
                    {
                        BAMSortedIndex sortedIndex = new BAMSortedIndex(sortedFile.Value, sortField);
                        sortedIndex.GroupName = sortedFile.Key;
                        sortedIndices.Add(sortedIndex);
                    }
                    break;
                case BAMSortByFields.ChromosomeCoordinates:
                    // Sort by Chromosomes Positions (Pos) and retun "BAMSortedIndex"
                    // containing the indices of sorted SequenceAlignmentMap.QuerySequences items.
                    sortedFiles = SortByChromosomeCoordinates();
                    foreach (string refName in sequenceAlignMap.GetRefSequences())
                    {
                        IList<string> filenames =null;
                        if (sortedFiles.TryGetValue(refName, out filenames))
                        {
                            BAMSortedIndex sortedIndex = new BAMSortedIndex(filenames, sortField);
                            sortedIndex.GroupName = refName;
                            sortedIndices.Add(sortedIndex);
                            
                        }
                    }

                    break;

                case BAMSortByFields.ReadNames:
                    // Sort by Read name (QName) and retun "BAMSortedIndex" containing the indices of 
                    // sorted SequenceAlignmentMap.QuerySequences items.
                    sortedIndices.Add(new BAMSortedIndex(SortByReadNames(), sortField));
                    break;
            }

            return sortedIndices;
        }

        /// <summary>
        /// Sort the index of SequenceAlignmentMap by QName.
        /// Fill the index (sorted by QName) into a list, when the list size reaches
        /// the maximum limit, write the list to file and clear the list.
        /// </summary>
        private IList<string> SortByReadNames()
        {
            IList<string> files = new List<string>();

            var sortedList = new System.Collections.Generic.SortedList<object,string>();

            for (int index = 0; index < sequenceAlignMap.QuerySequences.Count; index++)
            {
                SAMAlignedSequence alignedSeq = sequenceAlignMap.QuerySequences[index];
                string indices = string.Empty;
                if (!sortedList.TryGetValue(alignedSeq.QName, out indices))
                {
                    sortedList.Add(alignedSeq.QName, index.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    indices = string.Format(CultureInfo.InvariantCulture, "{0},{1}", indices, index.ToString(CultureInfo.InvariantCulture));
                    sortedList[alignedSeq.QName] = indices;
                }

                if (sortedList.Count >= SortedListMaxCount)
                {
                    if (files == null)
                    {
                        files = new List<string>();
                    }

                    files.Add(WriteToFile(sortedList));
                    sortedList.Clear();
                }
            }

            if (sortedList.Count > 0)
            {
                files.Add(WriteToFile(sortedList));
                sortedList.Clear();
            }
            
            return files;
        }

        /// <summary>
        /// Sort the index of SequenceAlignmentMap by RName then by Pos.
        /// Fill the index (sorted by RName then by Pos) into a list, when the list size reaches
        /// the maximum limit, write the list to file and clear the list.
        /// </summary>
        private SortedDictionary<string, IList<string>> SortByChromosomeCoordinates()
        {
            SortedDictionary<string, IList<string>> sortedFiles = new SortedDictionary<string, IList<string>>();
            IList<string> files = null;

            var groups = new System.Collections.Generic.SortedList<string, System.Collections.Generic.SortedList<object, string>>();
            System.Collections.Generic.SortedList<object, string> sortedList = null;

            for (int index = 0; index < sequenceAlignMap.QuerySequences.Count; index++)
            {
                SAMAlignedSequence alignedSeq = sequenceAlignMap.QuerySequences[index];
                if (!groups.TryGetValue(alignedSeq.RName, out sortedList))
                {
                    sortedList = new System.Collections.Generic.SortedList<object, string>();
                    groups.Add(alignedSeq.RName, sortedList);
                }

                string indices = string.Empty;
                if (!sortedList.TryGetValue(alignedSeq.Pos, out indices))
                {
                    sortedList.Add(alignedSeq.Pos, index.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    indices = string.Format(CultureInfo.InvariantCulture, "{0},{1}", indices, index.ToString(CultureInfo.InvariantCulture));
                    sortedList[alignedSeq.Pos] = indices;
                }

                if (sortedList.Count >= SortedListMaxCount)
                {
                    if (!sortedFiles.TryGetValue(alignedSeq.RName, out files))
                    {
                        files = new List<string>();
                        sortedFiles.Add(alignedSeq.RName, files);
                    }

                    files.Add(WriteToFile(sortedList));
                    sortedList.Clear();
                }
            }

            foreach (KeyValuePair<string, System.Collections.Generic.SortedList<object, string>> group in groups)
            {
                if (group.Value.Count > 0)
                {
                    if (!sortedFiles.TryGetValue(group.Key, out files))
                    {
                        files = new List<string>();
                        sortedFiles.Add(group.Key, files);
                    }

                    files.Add(WriteToFile(group.Value));
                    group.Value.Clear();
                }
            }

            return sortedFiles;
        }

        /// <summary>
        /// Creates a file in Temp folder.
        /// Write the data in SortedList to a file.
        /// Returns the filename
        /// </summary>
        /// <param name="sortedList">List to be written to file.</param>
        /// <returns>File name.</returns>
        private static string WriteToFile(System.Collections.Generic.SortedList<object, string> sortedList)
        {
            string[] indices;
            string fileName = Path.GetTempFileName();

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (KeyValuePair<object, string> entry in sortedList)
                {
                    indices = entry.Value.Split(',');
                    for (int count = 0; count < indices.Length; count++)
                    {
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0},{1}", entry.Key, indices[count]));
                    }
                }

                writer.Flush();
            }

            return fileName;
        }
    }
}
