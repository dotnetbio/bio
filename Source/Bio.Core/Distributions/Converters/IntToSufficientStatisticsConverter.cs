using Bio.Util;

namespace Bio.Distributions.Converters
{
    /// <summary>
    /// Converts Integer to SufficientStatistics.
    /// </summary>
    public class IntToSufficientStatisticsConverter : ValueConverter<int, SufficientStatistics>
    {
        /// <summary>
        /// Instantiate a new instance of IntToSufficientStatistics class.
        /// </summary>
        public IntToSufficientStatisticsConverter() : base(i => DiscreteStatistics.GetInstance(i), stats => (int)stats.AsDiscreteStatistics()) { }
    }
}
