using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TuneRoboWPF.Converter
{
    public class NumberRatingsToStringConverter:IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ulong number = value is ulong ? (ulong)value : 0;
            if (number<2)
            {
                return string.Format("{0} {1}", number.ToString(), Application.Current.TryFindResource("RatingText"));
            }
            return string.Format("{0} {1}", number.ToString(), Application.Current.TryFindResource("RatingsText"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
