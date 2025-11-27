using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace QuanlyCuaHangTapHoa.Converters
{
    public class BoolToStatusTextConverter : IValueConverter
    {
        // true / false -> "Đang sử dụng" / "Ngưng dùng"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? "Đang sử dụng" : "Ngưng dùng";
            }

            return "Không rõ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Không cần convert ngược
            throw new NotImplementedException();
        }
    }
}
