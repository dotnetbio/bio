using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Bio.IO.BAM
{
    /// <summary>
    /// This class implements indexer for Sorted BAM Index.
    /// Reads
    ///  Index for a file (contains data sorted by index) and return index
    ///  Or
    ///  Indices from multiple file (contains data sorted by index in each file) and returns smallest index.
    /// </summary>
    public class BAMSortedIndex :IEnumerable<int>, IEnumerator<int>
    {
        /// <summary>
        /// List of file readers.
        /// </summary>
        private IList<StreamReader> readers;

        /// <summary>
        /// Next data object to processed in each file.
        /// </summary>
        private IList<Tuple<string, int>> data;

        /// <summary>
        /// holds filenames (sorted files) like chr1_1, chr1_2, chr2 etc.
        /// </summary>
        private readonly IList<string> sortedFilenames;

        /// <summary>
        /// Field name using which data is to be sorted.
        /// </summary>
        private readonly BAMSortByFields sortField;

        /// <summary>
        /// Holds current sorted index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Constructor to initialize an instance of BAMSortedIndex class with specified list of filenames.
        /// </summary>
        /// <param name="filenames">Sorted filenames.</param>
        /// <param name="sortType">Type of sort required.</param>
        public BAMSortedIndex(IList<string> filenames, BAMSortByFields sortType)
        {
            sortedFilenames = filenames;
            sortField = sortType;
        }

        /// <summary>
        /// Constructor to initialize an instance of BAMSortedIndex class with specified filename.
        /// </summary>
        /// <param name="filename">Sorted filename.</param>
        /// <param name="sortType">Type of sort required.</param>
        public BAMSortedIndex(string filename, BAMSortByFields sortType)
        {
            sortedFilenames = new List<string> { filename };
            sortField = sortType;
        }

        /// <summary>
        /// Gets or sets the Chromosome name of this Sorted BAM Indexer
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets the current sorted index.
        /// </summary>
        public int Current
        {
            get 
            {
                return currentIndex; 
            }
        }

        /// <summary>
        /// Disposes this object by discording any resources held.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose field instances
        /// </summary>
        /// <param name="disposeManaged">If disposeManaged equals true, clean all resources</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                // close all file pointers here.
                if (readers != null)
                {
                    for (int index = 0; index < readers.Count; index++)
                    {
                        readers[index].Close();
                        readers[index].Dispose();
                        readers[index] = null;
                    }
                }

                if (sortedFilenames != null)
                {
                    foreach (string filename in sortedFilenames)
                    {
                        if (File.Exists(filename))
                        {
                            try
                            {
                                File.Delete(filename);
                            }
                            catch (IOException)
                            {
                                // Ignore the exception
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current sorted index.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get
            {

                return currentIndex;
            }
        }

        /// <summary>
        /// Fetches next sorted index.
        /// </summary>
        /// <returns>Returns true on successful fetch, else return false.</returns>
        public bool MoveNext()
        {
            // code here to get the next smaller index.
            if (null == data)
            {
                if (sortedFilenames == null || sortedFilenames.Count == 0)
                {
                    return false;
                }

                readers = new List<StreamReader>();
                data = new List<Tuple<string, int>>();
                foreach (string filename in sortedFilenames)
                {
                    StreamReader reader = new StreamReader(filename);
                    string[] fileData = reader.ReadLine().Split(',');
                    readers.Add(reader);
                    data.Add(new Tuple<string, int>(fileData[0], int.Parse(fileData[1], CultureInfo.InvariantCulture)));
                }
            }

            int smallestIndex = -1;
            for(int index = 0; index < data.Count; index++)
            {
                if (data[index] != null)
                {
                    if (smallestIndex == -1)
                    {
                        smallestIndex = index;
                    }
                    else
                    {
                        switch (sortField)
                        {
                            case BAMSortByFields.ReadNames:
                                if (0 < string.Compare(data[smallestIndex].Item1, data[index].Item1, 
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    smallestIndex = index;
                                }
                                break;

                            case BAMSortByFields.ChromosomeCoordinates:
                                if (int.Parse(data[index].Item1, CultureInfo.CurrentCulture) 
                                        < int.Parse(data[smallestIndex].Item1, CultureInfo.CurrentCulture))
                                {
                                    smallestIndex = index;
                                }
                                break;
                        }
                    }
                }
            }

            if (smallestIndex > -1)
            {
                currentIndex = data[smallestIndex].Item2;

                Tuple<string, int> dataObject = null;

                if (!readers[smallestIndex].EndOfStream)
                {
                    string[] fileData = readers[smallestIndex].ReadLine().Split(',');
                    dataObject = new Tuple<string, int>(fileData[0], int.Parse(fileData[1], CultureInfo.InvariantCulture));
                }

                data[smallestIndex] = dataObject;
            }
            else
            {
                Reset();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets this instance to initial state.
        /// </summary>
        public void Reset()
        {
            // code here to reset this object to initial state.
            data = null;

            foreach (StreamReader reader in readers)
            {
                reader.Close();
            }

            readers = null;
        }

        /// <summary>
        /// Returns the enumerator object
        /// </summary>
        /// <returns>enumerator object</returns>
        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Returns the enumerator object
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
