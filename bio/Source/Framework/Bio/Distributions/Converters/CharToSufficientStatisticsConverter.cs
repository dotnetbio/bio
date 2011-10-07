using System;
using System.Collections.Generic;
using Bio.Util;

namespace Bio.Distributions.Converters
{
    /// <summary>
    /// Converts character to sufficient statistics.
    /// </summary>
    public class CharToSufficientStatisticsConverter : ValueConverter<char, SufficientStatistics>
    {
        /// <summary>
        /// Create instance of CharToSufficientStatisticsConverter class.
        /// </summary>
        public CharToSufficientStatisticsConverter() : base(c => SufficientStatistics.Parse(c.ToString()), ss => FirstAndOnly(ss.ToString())) { }

        /// <summary>
        /// Gets first instance of the specified type from an enum of type T.
        /// If more than one instance of requested type, throws exception.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <param name="enumeration">Enumeration to search in.</param>
        /// <returns>First instance of requested type.</returns>
        public static T FirstAndOnly<T>(IEnumerable<T> enumeration)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException("enumeration");
            }

            IEnumerator<T> enumor = enumeration.GetEnumerator();
            Helper.CheckCondition(enumor.MoveNext(), "Can't get first item");
            T t = enumor.Current;
            Helper.CheckCondition(!enumor.MoveNext(), "More than one item available");
            return t;
        }
    }
}
