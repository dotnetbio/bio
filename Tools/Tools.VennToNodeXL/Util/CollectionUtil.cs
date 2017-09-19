using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Research.CommunityTechnologies.AppLib
{
    //*****************************************************************************
    //  Class: CollectionUtil
    //
    /// <summary>
    /// Static utility methods for working with collections.
    /// </summary>
    ///
    /// <remarks>
    /// All methods are static.
    /// </remarks>
    //*****************************************************************************

    public static class CollectionUtil
    {
        //*************************************************************************
        //  Method: DictionaryKeysToArray()
        //
        /// <summary>
        /// Copies the key collection of a Dictionary to an array.
        /// </summary>
        ///
        /// <param name="dictionary">
        /// Dictionary whose key collection should be copied.
        /// </param>
        ///
        /// <returns>
        /// An array of the dictionary's keys.
        /// </returns>
        //*************************************************************************

        public static TKey[] DictionaryKeysToArray<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            Debug.Assert(dictionary != null);

            return (CollectionToArray<TKey>(dictionary.Keys));
        }

        //*************************************************************************
        //  Method: DictionaryValuesToArray()
        //
        /// <summary>
        /// Copies the value collection of a Dictionary to an array.
        /// </summary>
        ///
        /// <param name="dictionary">
        /// Dictionary whose value collection should be copied.
        /// </param>
        ///
        /// <returns>
        /// An array of the dictionary's values.
        /// </returns>
        //*************************************************************************

        public static TValue[] DictionaryValuesToArray<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            Debug.Assert(dictionary != null);

            return (CollectionToArray<TValue>(dictionary.Values));
        }

        //*************************************************************************
        //  Method: CollectionToArray()
        //
        /// <summary>
        /// Copies a generic ICollection to an array.
        /// </summary>
        ///
        /// <param name="collection">
        /// Collection that should be copied.
        /// </param>
        ///
        /// <returns>
        /// An array of the collection's values.
        /// </returns>
        //*************************************************************************

        public static T[] CollectionToArray<T>(ICollection<T> collection)
        {
            Debug.Assert(collection != null);

            T[] aoValues = new T[collection.Count];

            collection.CopyTo(aoValues, 0);

            return (aoValues);
        }
    }
}
