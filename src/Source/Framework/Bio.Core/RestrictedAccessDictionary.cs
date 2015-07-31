using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// Access flags that define what is allowed in a RestrictedAccessDictionary. The can be combined with bit-wise OR.
    /// </summary>
    [Flags]
    public enum AccessFlags : int
    {
        /// <summary>
        /// Allow elements to be added.
        /// </summary>
        Add = 1,
        /// <summary>
        /// Allow elements to be removed.
        /// </summary>
        Remove = 2,
        /// <summary>
        /// Allow elements to be changed.
        /// </summary>
        ChangeElements = 4
    };

    /// <summary>
    /// A thin wrapper around Dictionary that allows access permissions to be set. Any changes not allowed result in an exception.
    /// </summary>
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <typeparam name="TValue">The type of the value</typeparam>
    public class RestrictedAccessDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private AccessFlags _accessFlags;
        private IDictionary<TKey, TValue> _dict;

        /// <summary>
        /// Create read-only dictionary by wrapping a dictionary.
        /// </summary>
        /// <param name="baseDictionary">The dictionary to wrap</param>
        public RestrictedAccessDictionary(IDictionary<TKey, TValue> baseDictionary) :
            this(baseDictionary, 0) { }

        /// <summary>
        /// Create a restricted access dictionary by wrapping a dictionary.
        /// </summary>
        /// <param name="baseDictionary">The dictionary to wrap</param>
        /// <param name="accessFlags">The flags that define how to restrict the dictionary.</param>
        public RestrictedAccessDictionary(IDictionary<TKey, TValue> baseDictionary, AccessFlags accessFlags)
        {
            _dict = baseDictionary;
            _accessFlags = accessFlags;
        }


        /// <summary>
        /// True if and only if this dictionary allows elements to be added.
        /// </summary>
        public bool AddIsAllowed
        {
            get { return (_accessFlags & AccessFlags.Add) == AccessFlags.Add; }
        }

        /// <summary>
        /// True if and only if this dictionary allows elments to be removed.
        /// </summary>
        public bool RemoveIsAllowed
        {
            get { return (_accessFlags & AccessFlags.Remove) == AccessFlags.Remove; }
        }

        /// <summary>
        /// True if and only if this dictionary allows elements to change
        /// </summary>
        public bool ChangeElementsIsAllowed
        {
            get { return (_accessFlags & AccessFlags.ChangeElements) == AccessFlags.ChangeElements; }
        }

        #region IDictionary<TKey,TValue> Members

        #pragma warning disable 1591
        public void Add(TKey key, TValue value)
        #pragma warning restore 1591
        {
            if (!AddIsAllowed)
            {
                throw new NotSupportedException("This restricted Dictionary does not allow adding items.");
            }
            _dict.Add(key, value);
        }

        #pragma warning disable 1591
        public bool ContainsKey(TKey key)
        #pragma warning restore 1591
        {
            return _dict.ContainsKey(key);
        }

        #pragma warning disable 1591
        public ICollection<TKey> Keys
        #pragma warning restore 1591
        {
            get { return _dict.Keys; }
        }

        #pragma warning disable 1591
        public bool Remove(TKey key)
        #pragma warning restore 1591
        {
            if (!RemoveIsAllowed)
            {
                throw new NotSupportedException("This restricted access dictionary does not allow removing items.");
            }
            return _dict.Remove(key);
        }

        #pragma warning disable 1591
        public bool TryGetValue(TKey key, out TValue value)
        #pragma warning restore 1591
        {
            return _dict.TryGetValue(key, out value);
        }

        #pragma warning disable 1591
        public ICollection<TValue> Values
        #pragma warning restore 1591
        {
            get { return _dict.Values; }
        }

        #pragma warning disable 1591
        public TValue this[TKey key]
        #pragma warning restore 1591
        {
            get
            {
                return _dict[key];
            }
            set
            {
                if (!ChangeElementsIsAllowed && _dict.ContainsKey(key))
                {
                    throw new NotSupportedException("This restricted access dictionary does not allow changing entry values.");
                }
                if (!AddIsAllowed && !_dict.ContainsKey(key))
                {
                    throw new NotSupportedException("This restricted access dictionary does not allow adding items.");
                }
                _dict[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        #pragma warning disable 1591
        public void Add(KeyValuePair<TKey, TValue> item)
        #pragma warning restore 1591
        {
            if (!AddIsAllowed)
            {
                throw new NotSupportedException("This restricted access dictionary does not allow adding items.");
            }
            _dict.Add(item);
        }

        #pragma warning disable 1591
        public void Clear()
        #pragma warning restore 1591
        {
            if (!RemoveIsAllowed)
            {
                throw new NotSupportedException("This restricted access dictionary does not allow removing items.");
            }
            _dict.Clear();
        }

        #pragma warning disable 1591
        public bool Contains(KeyValuePair<TKey, TValue> item)
        #pragma warning restore 1591
        {
            return _dict.Contains(item);
        }

        #pragma warning disable 1591
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        #pragma warning restore 1591
        {
            _dict.CopyTo(array, arrayIndex);
        }

        #pragma warning disable 1591
        public int Count
        #pragma warning restore 1591
        {
            get { return _dict.Count(); }
        }

        #pragma warning disable 1591
        public bool IsReadOnly
        #pragma warning restore 1591
        {
            get { return !AddIsAllowed || !RemoveIsAllowed || !ChangeElementsIsAllowed; }
        }

        /// <summary>
        /// Remove an item from a restricted dictionary if that is allowd. It is not, raise an exception.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>true if the item was in the dictionary; otherwise, false</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!RemoveIsAllowed)
            {
                throw new NotSupportedException("This restricted access dictionary does not allow removing items.");
            }
            return _dict.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Enumerate the KeyValuePairs of the dictionary
        /// </summary>
        /// <returns>A sequence of KeyValue pairs</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion
    }
}
