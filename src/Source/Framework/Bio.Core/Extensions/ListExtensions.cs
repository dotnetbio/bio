using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bio.Extensions
{
    /// <summary>
    /// Extension methods on List(T)
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Runs each item through a conversion and returns the produced list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="input"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static List<TOutput> ConvertAll<T, TOutput>(this IList<T> input, Func<T, TOutput> converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter cannot be null.");

            return new List<TOutput>(input.Select(converter));
        }

        /// <summary>
        /// Adds a set of items to a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="newItems"></param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
                list.Add(item);
        }

        /// <summary>
        /// Convert a List into a ReadOnly list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IList<T> AsReadOnly<T>(this IList<T> input)
        {
            return new ReadOnlyCollection<T>(input);
        }

    }
}
