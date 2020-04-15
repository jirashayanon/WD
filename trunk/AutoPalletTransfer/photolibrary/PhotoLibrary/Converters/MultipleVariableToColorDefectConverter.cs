using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoLibrary.Converters
{
    public class MultipleVariableToColorDefectConverter : IMultiValueConverter
    {
        public object Convert(object[] value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.Length < 2)
            {
                return null;
            }
            bool IsDefect = (bool)value[0];
            string type = (string)value[1];
            string vmiResultString = (string)value[2];
            if (!IsDefect)
            {
                if (PhotoLibrary.Model.DefectCollection.CheckSkipDefect(vmiResultString))
                {
                    return Brushes.LightGreen;
                }
                return Brushes.Green;
            }
            else
            {
                if (PhotoLibrary.Model.DefectCollection.CheckSkipDefect(vmiResultString))
                {
                    return Brushes.LightGreen;
                }
                else if (type.Equals("NOHGA", StringComparison.CurrentCultureIgnoreCase))
                {
                    return Brushes.Gray;
                }
                else
                {
                    return Brushes.Red;
                }
            }
        }

        public object[] ConvertBack(object value, System.Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
