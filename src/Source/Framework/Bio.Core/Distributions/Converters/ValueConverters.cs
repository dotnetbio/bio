using Bio.Util;

namespace Bio.Distributions.Converters
{
    /// <summary>
    /// Static class which contains converters. 
    /// </summary>
    public static class ValueConverters
    {
        /// <summary>
        /// Converts from string to double;
        /// </summary>
        public static readonly ValueConverter<string, double> StringToDoubleConverter = new StringToDoubleConverter();

        /// <summary>
        /// Converts from int to Sufficient Statistics.
        /// </summary>
        public static readonly ValueConverter<int, SufficientStatistics> IntToSufficientStatistics = new IntToSufficientStatisticsConverter();

        /// <summary>
        /// Converts from Sufficient statistics to integer.
        /// </summary>
        public static readonly ValueConverter<SufficientStatistics, int> SufficientStatisticsToInt = IntToSufficientStatistics.Inverted;

        /// <summary>
        /// Convert from character to sufficient statistics.
        /// </summary>
        public static readonly ValueConverter<char, SufficientStatistics> CharToSufficientStatistics = new CharToSufficientStatisticsConverter();

        /// <summary>
        /// Convert from Sufficient statistics to character.
        /// </summary>
        public static readonly ValueConverter<SufficientStatistics, char> SufficientStatisticsToChar = CharToSufficientStatistics.Inverted;
    }
}
