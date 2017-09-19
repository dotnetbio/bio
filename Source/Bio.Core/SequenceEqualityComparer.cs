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
        /// <param name="firstSequence">First sequence.</param>
        /// <param name="secondSequence">Second sequence.</param>
        /// <returns>Returns true if both sequence have equal data.</returns>
        public bool Equals(ISequence firstSequence, ISequence secondSequence)
        {
            if (firstSequence == null)
            {
                throw new ArgumentNullException("firstSequence");
            }

            if (secondSequence == null)
            {
                throw new ArgumentNullException("secondSequence");
            }

            if (firstSequence.Count == secondSequence.Count)
            {
                for (long index = 0; index < firstSequence.Count; index++)
                {
                    if (firstSequence[index] != secondSequence[index])
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
