using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Util
{
    /// <summary>
    /// Extension methods related to HashSet
    /// </summary>
    public static class HashSetExtensions
    {

        /// <summary>
        /// Add a range of values to a hashset. It is OK if the values are already of the hashset.
        /// </summary>
        /// <typeparam name="T">The type of the hashset's elements</typeparam>
        /// <param name="hashSet">The hashset to add values to</param>
        /// <param name="sequence">A sequence of values to add to the hashset.</param>
        public static void AddNewOrOldRange<T>(this HashSet<T> hashSet, IEnumerable<T> sequence)
        {
            if (hashSet == null)
            {
                throw new ArgumentNullException("hashSet");
            }

            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            foreach (T t in sequence)
            {
                hashSet.Add(t);
            }
        }
    }
}
