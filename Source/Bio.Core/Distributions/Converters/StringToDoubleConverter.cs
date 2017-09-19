using System.Globalization;
using Bio.Util;

namespace Bio.Distributions.Converters
{
    /// <summary>
    /// Class which converts from string to double
    /// </summary>
    public class StringToDoubleConverter : ValueConverter<string, double>
    {
        /// <summary>
        /// Create instance of StringToDoubleConverter class. 
        /// </summary>
        public StringToDoubleConverter() : base(c => 
            double.Parse(c.Trim(), CultureInfo.InvariantCulture), c => c.ToString(CultureInfo.InvariantCulture)) { }
    }
}
