using System;
using System.Globalization;
using System.Windows.Data;

namespace ReadSimulator
{
    /// <summary>
    /// A useful data banding converter when working with the binding of a boolean to
    /// two radio buttons.
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(bool))]
    public class SuccessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());
            if (value == null)
            {
                return false;
            }
            else
            {
                return !((bool)value ^ param);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());
            return !((bool)value ^ param);
        }
    }
}
