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
    class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            bool checkValue = (bool)value;
            return !checkValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
