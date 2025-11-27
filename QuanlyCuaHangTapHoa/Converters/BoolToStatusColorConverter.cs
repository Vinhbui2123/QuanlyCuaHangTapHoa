using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace QuanlyCuaHangTapHoa.Converters
{
    public class BoolToStatusColorConverter : IValueConverter
    {
        // true -> xanh lá, false -> đỏ
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Colors.LimeGreen : Colors.Red;
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
