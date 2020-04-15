using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoLibrary.Converters
{
    public class FontColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Black;

            bool checkValue = (bool)value;
            if (checkValue)
            {
                return Brushes.Gray;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
