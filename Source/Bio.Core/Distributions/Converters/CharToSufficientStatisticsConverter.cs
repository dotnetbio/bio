using System.Linq;

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
        public CharToSufficientStatisticsConverter()
            : base(c => SufficientStatistics.Parse(c.ToString()),
                ss => ss.ToString().Cast<char>().Single())
        {
        }
    }
}
