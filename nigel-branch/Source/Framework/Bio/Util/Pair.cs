using System;
namespace Bio.Util
{
    /// <summary>
    /// A pair object that hashes (and equals) based on the values of its elements.
    /// </summary>
    /// <typeparam name="T1">The type of its first element.</typeparam>
    /// <typeparam name="T2">The type of its second element.</typeparam>
    [Obsolete("Tuple<T1,T2>")]
    public class Pair<T1, T2> : Tuple<T1, T2>
    {
        /// <summary>
        /// The first element.
        /// </summary>
        public T1 First
        {
            get
            {
                return Item1;
            }
        }

        /// <summary>
        /// The second element.
        /// </summary>
        public T2 Second
        {
            get
            {
                return Item2;
            }
        }

        /// <summary>
        /// Creates a pair.
        /// </summary>
        /// <param name="first">The first element of the pair</param>
        /// <param name="second">The second element of the pair</param>
        public Pair(T1 first, T2 second)
            :base(first,second)
        {
        }

    }
}
