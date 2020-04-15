using PhotoLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhotoLibrary.Converters
{
    class StatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                 object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string checkValue = value.ToString();
            if (checkValue.Equals("Good", StringComparison.InvariantCultureIgnoreCase))
            {
                return "/Assets/1439930421_Check.png";
            }
            else if (checkValue.Equals("Reject", StringComparison.InvariantCultureIgnoreCase))
            {
                return "/Assets/1439930433_Delete.png";
            }
            else if (checkValue.Equals("None", StringComparison.InvariantCultureIgnoreCase))
            {
                return "/Assets/1439930436_Help.png";
            }

            LogHelper.AppendWarningFile("Status of HGA is not 'Good', 'Reject', or 'None'.");

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
