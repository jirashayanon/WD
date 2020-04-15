using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoLibrary.Converters
{
    public class BoolToBorderColorDefectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            bool checkValue = (bool)value;
            if (checkValue)
            {
                return Brushes.Red;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
