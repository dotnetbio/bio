using System;

namespace Bio
{
    /// <summary>
    /// Extension methods on Arrays
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns the LongLength property of an array.
        /// If running inside silverlight, will return the Length property.
        /// </summary>
        /// <param name="arr">Array of which the length has to be found.</param>
        /// <returns>Length of the array.</returns>
        public static long LongLength(this Array arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException("arr");
            }

            #if (SILVERLIGHT == false)
                return arr.LongLength; 
            #else
                return arr.Length; 
            #endif
        }
    }
}
