using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TuneRoboWPF.Converter
{
    public class NumberMotionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint number = value is uint ? (uint) value : 0;
            switch (number)
            {
                case 0:
                    return "No motion";
                case 1:
                    return "1 motion";
                default:
                    return number.ToString() + " motions";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
