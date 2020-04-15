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
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null)
                return (parameter != null && parameter.ToString() == "Collapsed") ? Visibility.Collapsed : Visibility.Hidden;

            bool checkValue = (bool)value;
            if (checkValue)
            {
                return Visibility.Visible;
            }
            else
            {
                return (parameter != null && parameter.ToString() == "Collapsed") ? Visibility.Collapsed : Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
