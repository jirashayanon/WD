using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PhotoLibrary.Converters
{
    class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Hidden;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            if (checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
