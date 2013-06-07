using System;
using System.Globalization;
using System.Windows.Data;

namespace TuneRoboWPF.Converter
{
    public class BoolToFollowButtonVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = value is bool && (bool)value;
            if (state)
            {
                return "Visible";
            }
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
