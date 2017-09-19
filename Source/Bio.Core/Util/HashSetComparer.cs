using System.Collections.Generic;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// A basic hashset comparer which compares the contents of two hashsets.
    /// Basically built to use with Dictionary.
    /// </summary>
    /// <typeparam name="T">Type of hashset.</typeparam>
    public class HashSetComparer<T> : IEqualityComparer<HashSet<T>>
    {
        /// <summary>
        /// Checks if two hashSets contain same set of items or not.
        /// </summary>
        /// <param name="x">First hashset.</param>
        /// <param name="y">Second hashset.</param>
        /// <returns>Returns true if both have equal data.</returns>
        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x.Count == y.Count)
            {
                return x.Union(y).Count() == x.Count;
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <param name="obj">Object of which hashcode has to calculated.</param>
        /// <returns>A hash code for the current System.Object.</returns>
        public int GetHashCode(HashSet<T> obj)
        {
            // Returning 0 to force Equals call.
            return 0;
        }
    }
}
