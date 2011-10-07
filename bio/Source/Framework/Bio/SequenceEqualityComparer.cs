using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio
{
    /// <summary>
    /// This class gives the Sequence Equality Comparer.
    /// </summary>
    public class SequenceEqualityComparer : IEqualityComparer<ISequence>
    {
        /// <summary>
        /// Two sequences data are equal or not.
        /// </summary>
        /// <param name="x">First sequence.</param>
        /// <param name="y">Second sequence.</param>
        /// <returns>Returns true if both sequence have equal data.</returns>
        public bool Equals(ISequence x, ISequence y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x.Count == y.Count)
            {
                for (long index = 0; index < x.Count; index++)
                {
                    if (x[index] != y[index])
                    {
                      return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets hash code.
        /// </summary>
        /// <param name="obj">The sequence object.</param>
        /// <returns>Returns the hash code.</returns>
        public int GetHashCode(ISequence obj)
        {
            return new string(obj.Select(a => (char)a).ToArray()).GetHashCode();
        }
    }
}
