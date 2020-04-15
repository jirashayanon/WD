using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoLibrary.Converters
{
    public class VariableToColorDefectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            string type = (string)value;
            if (type.Equals("NOHGA", StringComparison.CurrentCultureIgnoreCase))
                return Brushes.Gray;
            else if (type.Equals("GOOD", StringComparison.CurrentCultureIgnoreCase))
            {
                return Brushes.Green;
            }
            else
            {
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
