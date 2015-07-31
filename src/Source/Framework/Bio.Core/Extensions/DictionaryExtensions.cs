using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// Extension methods for IDictionary{TKey,TValue}
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// With throw exception if not 1-1 mapping.
        /// </summary>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict.Select(kvp => new KeyValuePair<TValue, TKey>(kvp.Value, kvp.Key)).ToDictionary();
        }


        /// <summary>
        /// Creates a shallow ReadOnly dictionary wrapper around the given dictionary.
        /// </summary>
        public static RestrictedAccessDictionary<TKey, TValue> AsRestrictedAccessDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new RestrictedAccessDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Creates a shallow restricted access dictionary wrapper around the given dictionary. Only access specified by the flags is allowed.
        /// </summary>
        public static RestrictedAccessDictionary<TKey, TValue> AsRestrictedAccessDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, AccessFlags flags)
        {
            return new RestrictedAccessDictionary<TKey, TValue>(dictionary, flags);
        }


        /// <summary>
        /// Returns the value associated with key in the dictionary. If not present, adds the default value to the dictionary and returns that
        /// value.
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, bool insertIfMissing) where TValue : new()
        {
            return GetValueOrDefault<TKey, TValue>(dictionary, key, new TValue(), insertIfMissing);
        }


        /// <summary>
        /// Gets a value from a dictionary. If they value is not there, adds the default value to the dictionary and returns that.
        /// Not thread safe because it can add items to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the dictionary's key</typeparam>
        /// <typeparam name="TValue">The type of the dictionary's value</typeparam>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <returns>A value for this key.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = new TValue();	// create a default value and add it to the dictionary
                dictionary.Add(key, value);
            }
            return value;
        }


        /// <summary>
        /// Returns the value associated with key in the dictionary. If not present, adds the default value to the dictionary and returns that
        /// value.
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                dictionary.Add(key, defaultValue);
                return defaultValue;
            }
            else
            {
                return value;
            }
        }



        /// <summary>
        /// Returns the value associated with key in the dictionary. If not present, adds the default value to the dictionary and returns that
        /// value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> defaultValueConstructor, bool insertIfMissing = true)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                TValue defaultValue = defaultValueConstructor();
                if (insertIfMissing)
                {
                    dictionary.Add(key, defaultValue);
                }
                return defaultValue;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the value associated with key in the dictionary. If not present, adds the default value to the dictionary and returns that
        /// value.
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue, bool insertIfMissing)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                if (insertIfMissing)
                {
                    dictionary.Add(key, defaultValue);
                }
                return defaultValue;
            }
            else
            {
                return value;
            }
        }


    }
}
