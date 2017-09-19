using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Bio.Util
{
    /// <summary>
    /// Extensions to IEnumerable{T}
    /// </summary>
    public static class IEnumerableExtensions
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
        /// Shuffles the elements of a sequence.
        /// </summary>
        /// <typeparam name="T">the type of the elements in the sequence</typeparam>
        /// <param name="sequence">The sequence of elements to shuffle</param>
        /// <param name="random">a random number instance</param>
        /// <returns>a list of shuffled items</returns>
        public static List<T> Shuffle<T>(this IEnumerable<T> sequence, Random random)
        {
            List<T> list = new List<T>();
            foreach (T t in sequence)
            {
                list.Add(t); //We put the value here to get the new space allocated
                int oldIndex = random.Next(list.Count);
                list[list.Count - 1] = list[oldIndex];
                list[oldIndex] = t;
            }
            return list;
        }

        /// <summary>
        /// Creates a string from a sequence of elements. No delimiter is used.
        /// </summary>
        /// <param name="sequence">the sequence</param>
        /// <returns>a string</returns>
        public static string StringJoin(this System.Collections.IEnumerable sequence)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object obj in sequence)
            {
                if (obj == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.Append(obj.ToString());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates a delimited string from a sequence of elements.
        /// </summary>
        /// <param name="sequence">the sequence</param>
        /// <param name="separator">the delimiter</param>
        /// <returns>a string</returns>
        public static string StringJoin(this System.Collections.IEnumerable sequence, string separator)
        {
            StringBuilder aStringBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (object obj in sequence)
            {
                if (!isFirst)
                {
                    aStringBuilder.Append(separator);
                }
                else
                {
                    isFirst = false;
                }

                if (obj == null)
                {
                    aStringBuilder.Append("null");
                }
                else
                {
                    aStringBuilder.Append(obj.ToString());
                }
            }
            return aStringBuilder.ToString();
        }

        /// <summary>
        /// Creates a delimited string from a sequence of elements. At most maxLength elements will be used and "..." shows that more elements were in the list.
        /// </summary>
        /// <param name="sequence">a sequence</param>
        /// <param name="separator">the delimiter</param>
        /// <param name="maxLength">the maximum number of elements in the string.  It must be at least 2 or an exception is thrown.</param>
        /// <param name="etcString">the string to use of more than maxLength elements are found</param>
        /// <returns>a string</returns>
        public static string StringJoin(this System.Collections.IEnumerable sequence, string separator, int maxLength, string etcString)
        {
            Helper.CheckCondition(maxLength > 1, () => Properties.Resource.ExpectedMaxLengthToGreaterThanOne);
            StringBuilder aStringBuilder = new StringBuilder();
            int i = -1;
            foreach (object obj in sequence)
            {
                ++i;
                if (i > 0)
                {
                    aStringBuilder.Append(separator);
                }

                if (i >= maxLength)
                {
                    aStringBuilder.Append(etcString);
                    break; // really break, not continue;
                }
                else if (obj == null)
                {
                    aStringBuilder.Append("null");
                }
                else
                {
                    aStringBuilder.Append(obj.ToString());
                }
            }
            return aStringBuilder.ToString();
        }


        /// <summary>
        /// Creates a HashSet from a sequence. If the sequence is already a HashSet, a new HashSet is still created.
        /// </summary>
        /// <typeparam name="T">the type of elements of the sequence</typeparam>
        /// <param name="sequence">the input sequence</param>
        /// <returns>a HashSet</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence)
        {
            return new HashSet<T>(sequence);
        }

        /// <summary>
        /// Creates a Queue from a sequence. If the sequence is already a Queue, a new Queue is still created.
        /// </summary>
        /// <typeparam name="T">the type of elements of the sequence</typeparam>
        /// <param name="sequence">the input sequence</param>
        /// <returns>a Queue</returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> sequence)
        {
            return new Queue<T>(sequence);
        }


        /// <summary>
        /// Creates a HashSet from a sequence. If the sequence is already a HashSet, a new HashSet is still created.
        /// </summary>
        /// <typeparam name="T">the type of elements of the sequence</typeparam>
        /// <param name="sequence">the input sequence</param>
        /// <param name="comparer">The IEqualityComparer used by the HashSet</param>
        /// <returns>a HashSet</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(sequence, comparer);
        }

        /// <summary>
        /// Creates a dictionary from a sequence of KeyValuePairs. If the sequence is already a Dictionary, a new Dictionary is still created.
        /// </summary>
        /// <typeparam name="T1">the type of Key</typeparam>
        /// <typeparam name="T2">the type of Value</typeparam>
        /// <param name="pairSequence">the input pair sequence</param>
        /// <returns>a Dictionary</returns>
        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> pairSequence)
        {
            return pairSequence.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Calls an action on each element of a sequence. The action takes one argument: an element. It has no return value.
        /// </summary>
        /// <typeparam name="T">the type of the elements</typeparam>
        /// <param name="sequence">the input sequence</param>
        /// <param name="action">An Action, that is, a delegate that takes one input and has no output.</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T t in sequence)
            {
                action(t);
            }
        }

        /// <summary>
        /// Calls an action on each element of a sequence. The action takes two arguments: an element and the index of the element.
        /// It has no return value.
        /// </summary>
        /// <typeparam name="T">the type of the elements</typeparam>
        /// <param name="sequence">the input sequence</param>
        /// <param name="action">An action that takes an element and an index and returns nothing.</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action)
        {
            int idx = 0;
            foreach (T t in sequence)
            {
                action(t, idx++);
            }
        }


        /// <summary>
        /// Take the items from a sequence starting with item # start (index 0) and contining for count items.
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="sequence">The input sequence</param>
        /// <param name="start">The index of the first item to take</param>
        /// <param name="count">The number of items to take</param>
        /// <returns>The count items starting with the one with index start.</returns>
        public static IEnumerable<T> SubSequence<T>(this IEnumerable<T> sequence, int start, int count)
        {
            return sequence.Skip(start).Take(count);
        }

        /// <summary>
        /// Same semantics as Enumerable.Single(IEnumerable), but optimized for ILists. Throws an exception if list does not contain exactly 1 element. Otherwise returns that element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static T Single<T>(this IList<T> list)
        {
            Helper.CheckCondition(list.Count == 1, () => string.Format(CultureInfo.InvariantCulture, "Expected list to contain 1 element, but it has {0}.", list.Count));
            return list[0];
        }

        /// <summary>
        /// Same semantics as Enumerable.First(), but optimized for ILists. Throws an exception if the list is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static T First<T>(this IList<T> list)
        {
            return list[0];
        }

        /// <summary>
        /// Same semantics as Enumerable.Last(), but optimized for ILists. Throws an exception if the list is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static T Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        /// <summary>
        /// Yield an Enumeration of the list with the item appended to the end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
        {
            foreach (T e in enumerable)
            {
                yield return e;
            }
            yield return item;
        }

        /// <summary>
        /// Yield a concatenation of the two Enumerable lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="enumerables"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, params IEnumerable<T>[] enumerables)
        {
            foreach (T item in enumerable)
            {
                yield return item;
            }

            foreach (IEnumerable<T> e in enumerables)
            {
                foreach (T item in e)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Yield this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsSingletonEnumerable<T>(this T obj)
        {
            yield return obj;
        }


    }
}

