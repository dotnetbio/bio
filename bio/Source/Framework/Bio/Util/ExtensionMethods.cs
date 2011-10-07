using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// This class provides various extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="collection"> An System.Collections.Generic.IEnumerable to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified position in the source sequence.</returns>
        public static TSource ElementAt<TSource>(this IEnumerable<TSource> collection, long index)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (index < 0 || index > collection.Count())
            {
                throw new ArgumentOutOfRangeException("index");
            }

            IEnumerator<TSource> enumerator = collection.GetEnumerator();
            for (long i = 0; i <= index; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }

        /// <summary>
        /// Returns a new array with the specified range of values.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="data">Source data.</param>
        /// <param name="startIndex">Index to begind sub array at.</param>
        /// <param name="length">Length of sub array.</param>
        /// <returns></returns>
        public static T[] GetRange<T>(this T[] data, int startIndex, int length)
        {
            if (data == null) throw new ArgumentNullException("data");
            var result = new T[length];

            int index = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                result[index++] = data[i];
            }

            return result;
        }
    }
}
