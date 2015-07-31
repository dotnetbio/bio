using System;
using System.Collections;
using System.Collections.Generic;

namespace Bio
{
    /// <summary>
    /// Represents a strongly typed list of objects.
    /// Uses BigArray to store objects.
    /// </summary>
    /// <typeparam name="T">Type of elements to store.</typeparam>
    public class BigList<T> : IEnumerable<T>
    {
        #region Member variables
        /// <summary>
        /// Default capacity, used while adding first element 
        /// when capacity is not specified.
        /// </summary>
        private const int DefaultCapacity = 4;

        /// <summary>
        /// Empty array.
        /// </summary>
        private static readonly BigArray<T> _emptyArray = new BigArray<T>(0);

        /// <summary>
        /// BigArray instance to store elements.
        /// </summary>
        private BigArray<T> _items;

        /// <summary>
        /// Holds number of elements present in the BigList.
        /// </summary>
        private long _size;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the BigList.
        /// </summary>
        public BigList()
        {
            this._items = _emptyArray;
        }

        /// <summary>
        /// Initializes a new instance of the BigList with specified capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        public BigList(long capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            this._items = new BigArray<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the BigList with elements from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new BigList.</param>
        public BigList(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            ICollection<T> collectionObj = collection as ICollection<T>;
            if (collectionObj != null)
            {
                int count = collectionObj.Count;
                this._items = new BigArray<T>(count);
                int index = 0;
                foreach (var item in collectionObj)
                {
                    this._items[index++] = item;
                }
                this._size = count;
            }
            else
            {
                this._size = 0;
                this._items = new BigArray<T>(BigList<T>.DefaultCapacity);
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        this.Add(enumerator.Current);
                    }
                }
            }
        }
        /// <summary>
        /// Initialize a new big list with a collection and known size.
        /// </summary>
        /// <param name="collection">Items to add</param>
        /// <param name="collectionCount">Size of list</param>
        public BigList(IEnumerable<T> collection,long collectionCount)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (collectionCount < 0)
            {
                throw new ArgumentException("Cannot make new big list with < 0 items","collectionCount");
            }
                this._items = new BigArray<T>(collectionCount);
                int index = 0;
                foreach (var item in collection)
                {
                    this._items[index++] = item;
                }
                this._size = collectionCount;
            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        public long Capacity
        {
            get
            {
                return this._items.Length;
            }
            set
            {
                if (value < this._size)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this._items.Length)
                {
                    if (value > 0)
                    {
                        if (this._size == 0)
                        {
                            this._items = new BigArray<T>(value);
                        }
                        else
                        {
                            this._items.Resize(value);
                        }
                    }
                    else
                    {
                        this._items = BigList<T>._emptyArray;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of elements present in the BigList.
        /// </summary>
        public long Count
        {
            get
            {
                return this._size;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the entire BigList.
        /// </summary>
        /// <param name="item">The object to locate in the BigList. The value
        ///     can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire BigList,
        ///     if found; otherwise, –1.</returns>
        public long IndexOf(T item)
        {
            return this._items.IndexOf(item, 0, this._size);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the range of elements in the BigList
        ///     that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the BigList. The value
        ///     can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) 
        /// is valid in an empty BigList.</param>
        /// <returns> The zero-based index of the first occurrence of item within the range of
        ///     elements in the BigList that extends from index to the last element, 
        ///     if found; otherwise, –1.</returns>
        public long IndexOf(T item, long startIndex)
        {
            return this._items.IndexOf(item, startIndex, this._size - startIndex);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the range of elements in the BigList
        ///     that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the BigList. The value
        ///     can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty
        ///     BigList.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of item within the range of
        ///     elements in the BigList that starts at index and
        ///     contains count number of elements, if found; otherwise, –1.</returns>
        public long IndexOf(T item, long startIndex, long count)
        {
            if (count > this._size - startIndex)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return this._items.IndexOf(item, startIndex, count);
        }

        /// <summary>
        ///  Inserts an element into the BigList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public void Insert(long index, T item)
        {
            if (index > this._size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (this._size == this._items.Length)
            {
                this.EnsureCapacity(this._size + 1);
            }

            if (index < this._size)
            {
                for (long i = this._size; i > index; i--)
                {
                    this._items[i] = this._items[i - 1];
                }
            }

            this._items[index] = item;
            this._size++;
        }

        /// <summary>
        ///  Removes the element at the specified index of the BigList.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(long index)
        {
            if (index >= this._size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this._size--;
            if (index < this._size)
            {
                for (long i = index; i < this._size; i++)
                {
                    this._items[i] = this._items[i + 1];
                }
            }

            this._items[this._size] = default(T);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[long index]
        {
            get
            {
                if (index >= this._size)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return this._items[index];
            }
            set
            {
                if (index >= this._size)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                this._items[index] = value;
            }
        }

        /// <summary>
        /// Adds an object to the end of the BigList.
        /// </summary>
        /// <param name="item">The object to be added to the end of the BigList.</param>
        public void Add(T item)
        {
            if (this._size == this._items.Length)
            {
                this.EnsureCapacity(this._size + 1);
            }
            this._items[this._size++] = item;
        }

        /// <summary>
        ///  Removes all elements from the BigList.
        /// </summary>
        public void Clear()
        {
            if (this._size > 0)
            {
                this._items.Clear();
                this._size = 0;
            }
        }

        /// <summary>
        /// Determines whether an element is in the BigList.
        /// </summary>
        /// <param name="item">The object to locate in the BigList.</param>
        /// <returns>true if item is found in the BigList, else false.</returns>
        public bool Contains(T item)
        {
            return this._items.IndexOf(item, 0, this._size) >= 0;
        }

        /// <summary>
        /// Copies a range of elements from the BigList to a compatible one-dimensional array.
        /// </summary>
        /// <param name="index">The zero-based index in the source BigList at
        ///     which copying begins.</param>
        /// <param name="destinationArray">The one-dimensional array that is the destination of the elements
        ///     copied from BigList.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(long index, T[] destinationArray, long count)
        {
            this.CopyTo(index, destinationArray, 0, count);
        }

        /// <summary>
        /// Copies a range of elements from the BigList to a compatible one-dimensional array, 
        /// starting at the specified index of the destination array.
        /// </summary>
        /// <param name="index">The zero-based index in the source BigList at
        ///     which copying begins.</param>
        /// <param name="destinationArray">The one-dimensional array that is the destination of the elements
        ///     copied from BigList.</param>
        /// <param name="destinationIndex">The zero-based index in destinationArray at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(long index, T[] destinationArray, int destinationIndex, long count)
        {
            if (count > (this.Count - index))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            this._items.CopyTo(index, destinationArray, destinationIndex, count);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the BigList.
        /// </summary>
        /// <param name="item">The object to remove from the BigList.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also
        ///     returns false if item was not found in the BigList.</returns>
        public bool Remove(T item)
        {
            long index = this.IndexOf(item);
            if (index != -1)
            {
                this.RemoveAt(index);
            }

            return index != -1;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the BigList,
        ///  if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            long num = (long)(this._items.Length * 0.9);
            if (this._size < num)
            {
                this.Capacity = this._size;
            }
        }

        /// <summary>
        /// Trims the list and removes all elements above newSize
        /// </summary>
        /// <param name="newSize">size of new array</param>
        public void TrimToSize(long newSize)
        {
            if (newSize > this.Count || newSize<0)
            {
                throw new ArgumentException("Cannot trim BigList class to value less than 0 or larger than original size","newSize");
            }
            this._items.Resize(newSize);
        }

        /// <summary>
        /// Performs the specified action on each element of the BigList.
        /// </summary>
        /// <param name="action">The delegate to perform on each element of the BigList.</param>
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            for (int i = 0; i < this._size; i++)
            {
                action(this._items[i]);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the BigList.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (long i = 0; i < this._size; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the BigList.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // Ensures the capacity.
        private void EnsureCapacity(long minCapacityRequired)
        {
            if (this._items.Length < minCapacityRequired)
            {
                long newCapacity = 0;
                if (this._items.Length == 0)
                {
                    newCapacity = BigList<T>.DefaultCapacity;
                }
                else if (this._items.Length * 2 < this._items.BlockSize)
                {
                    newCapacity = this._items.Length * 2;
                }
                else
                {
                    long rem = this._items.Length % this._items.BlockSize;
                    if (rem > 0)
                    {
                        newCapacity = this._items.Length + rem;
                    }
                    else
                    {
                        newCapacity = this._items.Length + this._items.BlockSize;
                    }
                }

                if (newCapacity < minCapacityRequired)
                {
                    newCapacity = minCapacityRequired;
                }

                this.Capacity = newCapacity;
            }
        }
        #endregion
    }
}
