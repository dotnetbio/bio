using System;
using System.Collections.Generic;

namespace Bio.Util
{
    /// <summary>
    /// Wrapper class to use Comparison delegate.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class ComparisonWrapper<T> : IComparer<T>
    {
        /// <summary>
        /// Gets Comparison delegate.
        /// </summary>
        public Comparison<T> Comparison { get; private set; }

        /// <summary>
        /// Creates an instance of ComparisonWrapper class.
        /// </summary>
        /// <param name="comparison">Comparison delegate to use.</param>
        public ComparisonWrapper(Comparison<T> comparison)
        {
            Comparison = comparison;
        }

        /// <summary>
        /// Compares two instance and returns a value indicating whether one is less than,
        ///     equal to, or greater than the other.
        ///     <para>For more detail see the below table.</para> 
        ///     <para>-----------------------------------------</para>
        ///     <para>Value             | Meaning             |</para>
        ///     <para>-----------------------------------------</para>
        ///     <para>Less than zero    | x is less than y.   |</para>
        ///     <para>Zero              | x equals y          |</para>
        ///     <para>Greater than zero | x is greater than y |</para>
        ///     <para>-----------------------------------------</para>
        /// </summary>
        /// <param name="x">The first instance to compare</param>
        /// <param name="y">The second instance to compare</param>
        public int Compare(T x, T y)
        {
            return Comparison(x, y);
        }
    }
}
