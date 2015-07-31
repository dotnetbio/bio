using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// Generic SortedList for Portable Class Library
    /// </summary>
    internal class SortedList<TKey,TValue> : IDictionary<TKey,TValue>
    {
        private readonly Dictionary<TKey, TValue> data; 
        private readonly IComparer<TKey> comparer;
        private class EqualityComparer : IEqualityComparer<TKey>
        {
            private readonly IComparer<TKey> comparer;

            public EqualityComparer(IComparer<TKey> comparer)
            {
                this.comparer = comparer;
            }

            public bool Equals(TKey x, TKey y)
            {
                return comparer.Compare(x, y) == 0;
            }

            public int GetHashCode(TKey obj)
            {
                return 0;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SortedList() : this(null, 0)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialCapacity"></param>
        public SortedList(int initialCapacity) : this(null, initialCapacity)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        public SortedList(IComparer<TKey> comparer, int capacity = 0)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            if (comparer == null) 
                comparer = Comparer<TKey>.Default;

            this.comparer = comparer;
            data = new Dictionary<TKey, TValue>(capacity, new EqualityComparer(comparer));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="comparer"></param>
        public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer = null)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (comparer == null)
                comparer = Comparer<TKey>.Default;
            this.comparer = comparer;
            data = new Dictionary<TKey, TValue>(dictionary, new EqualityComparer(comparer));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            data.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return data.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length + arrayIndex < data.Count)
                throw new ArgumentException("Not enough space in array.");
            int pos = arrayIndex;
            foreach (var item in data)
            {
                array[pos++] = item;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return data.Remove(item.Key);
        }

        public int Count
        {
            get { return this.data.Count; }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(TKey key, TValue value)
        {
            data.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return data.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return data.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return data.Values;
            }
        }

        public TKey[] Keys
        {
            get
            {
                var keys = data.Keys.ToArray();
                Array.Sort(keys, comparer);
                return keys;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return this.Keys;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var keys = data.Keys.ToArray();
            Array.Sort(keys, comparer);
            return keys.Select(k => new KeyValuePair<TKey, TValue>(k, this.data[k])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}