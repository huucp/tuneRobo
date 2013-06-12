using System;
using System.Globalization;
using System.Windows.Data;

namespace TuneRoboWPF.Converter
{
    public class BoolToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = value is bool && (bool)value;
            string visibility = parameter is string ? (string) parameter : "Collapsed";
            if (state)
            {
                return "Visible";
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
