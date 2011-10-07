namespace SequenceAssembler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Generic virtual list implementation
    /// </summary>
    /// <typeparam name="T">Type of elements to be held in the list</typeparam>
    public class VirtualList<T> : IList<T>
    {
        /// <summary>
        /// This structure holds the source list and the index of the target item.
        /// </summary>
        private struct ListIndexPair
        {
            /// <summary>
            /// Reference to the source list
            /// </summary>
            public IList<T> SourceList;

            /// <summary>
            /// Index of target item
            /// </summary>
            public int Index;
        }

        /// <summary>
        /// Internal list which is used to serve as a source list in case an element is added without a soruce list.
        /// </summary>
        List<T> defaultList = new List<T>();

        /// <summary>
        /// Internal list which will hold the references to other lists using ListIndexPair instances
        /// </summary>
        private List<ListIndexPair> internalList = new List<ListIndexPair>();

        /// <summary>
        /// Returns the index of the specified item
        /// </summary>
        /// <param name="item">Item to be searched for</param>
        /// <returns>Zero based index or -1 if not found</returns>
        public int IndexOf(T item)
        {
            if (item != null)
            {
                for (int i = 0; i < internalList.Count; i++ )
                {
                    if (item.Equals(internalList[i].SourceList[internalList[i].Index]))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item at the specified index
        /// </summary>
        /// <param name="index">Index to which the item should be inserted</param>
        /// <param name="item">Item to be inserted</param>
        public void Insert(int index, T item)
        {
            defaultList.Add(item);
            internalList.Insert(index, new ListIndexPair { SourceList = defaultList, Index = defaultList.Count - 1 });
        }

        /// <summary>
        /// Remove an item at the specified index
        /// </summary>
        /// <param name="index">Index of item to be removed</param>
        public void RemoveAt(int index)
        {
            internalList.RemoveAt(index);

            if (internalList.Count == 0)
            {
                defaultList.Clear();
            }
        }

        /// <summary>
        /// Index for accessing any item by index
        /// </summary>
        /// <param name="index">Index of the item to be retrieved</param>
        /// <returns>Item at the specified index</returns>
        public T this[int index]
        {
            get
            {
                ListIndexPair targetPair = internalList[index];
                return targetPair.SourceList[targetPair.Index];
            }
            set
            {
                // This virtual list cannot support setting the element at a specified index
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void Add(T item)
        {
            defaultList.Add(item); // add item to defaultList as there is no source list specifed
            internalList.Add(new ListIndexPair {SourceList = defaultList, Index = defaultList.Count - 1 });
        }

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        /// <param name="sourceList">Instance of the source list of the item</param>
        /// <param name="index">Index of the item in the source list</param>
        public void Add(IList<T> sourceList, int index)
        {
            internalList.Add(new ListIndexPair { SourceList = sourceList, Index = index });
        }

        /// <summary>
        /// Clear the virtual list
        /// </summary>
        public void Clear()
        {
            internalList.Clear();
            defaultList.Clear();
        }

        /// <summary>
        /// Check if a particular item exists in this list
        /// </summary>
        /// <param name="item">Item to search for</param>
        /// <returns>True if the item exists, else false</returns>
        public bool Contains(T item)
        {
            foreach (ListIndexPair pair in internalList)
            {
                if (item.Equals(pair.SourceList[pair.Index]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copy the elements of this list to an array
        /// The result array will hold strong references to the elements
        /// </summary>
        /// <param name="array">Target array to copy to</param>
        /// <param name="arrayIndex">Starting index of the source array to start wirting from</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (T currentItem in this)
            {
                array[index++] = currentItem;
            }
        }

        /// <summary>
        /// Count of items in this list
        /// </summary>
        public int Count
        {
            get { return internalList.Count; }
        }

        /// <summary>
        /// Flag to say if this is a read only list
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Remove the specified item from the list
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>True if successful, else false</returns>
        public bool Remove(T item)
        {
            for (int i = 0; i < internalList.Count; i++)
            {
                if (item.Equals(internalList[i].SourceList[internalList[i].Index]))
                {
                    internalList.RemoveAt(i);

                    if (internalList.Count == 0)
                    {
                        defaultList.Clear();
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator of type T to loop through this list
        /// </summary>
        /// <returns>Enumerator instance of the element type</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new GenericIListEnumerator<T>(this);
        }

        /// <summary>
        /// Returns an enumerator of type T to loop through this list
        /// </summary>
        /// <returns>Enumerator instance of the element type</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new GenericIListEnumerator<T>(this);
        }
    }

    /// <summary>
    /// A generic enumerator implementation which can be used to enumerate through any class which is based on a generic IList implementation
    /// </summary>
    /// <typeparam name="T">Type of the IList implementation</typeparam>
    public class GenericIListEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// Target list to enumerate
        /// </summary>
        IList<T> internalList;

        /// <summary>
        /// Index of the current item
        /// </summary>
        int currentIndex;

        /// <summary>
        /// Initializes a new instance of the GenericIListEnumerator
        /// </summary>
        /// <param name="sourceList">Target list to enumerate on</param>
        public GenericIListEnumerator(IList<T> sourceList)
        {
            internalList = sourceList;
            Reset();
        }

        /// <summary>
        /// Returns the current item
        /// </summary>
        public T Current
        {
            get { return internalList[currentIndex]; }
        }

        /// <summary>
        /// Dispose the enumerator
        /// </summary>
        public void Dispose()
        {
            // Do not dispose the internal list as the items are pointing to another list in memory which we dont want to dispose
        }

        /// <summary>
        /// Returns the current item
        /// </summary>
        object IEnumerator.Current
        {
            get { return internalList[currentIndex]; }
        }

        /// <summary>
        /// Move to the next item in the list
        /// </summary>
        /// <returns>True if move was successful, false if reached end of list</returns>
        public bool MoveNext()
        {
            if (currentIndex < internalList.Count - 1)
            {
                currentIndex++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the current item pointer
        /// </summary>
        public void Reset()
        {
            currentIndex = -1;
        }
    }
}
