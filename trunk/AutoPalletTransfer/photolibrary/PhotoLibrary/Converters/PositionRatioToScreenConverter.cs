using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhotoLibrary.Converters
{
    class PositionRatioToScreenConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value[0]) * System.Convert.ToDouble(value[1]);
        }

        public object[] ConvertBack(object value, System.Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
