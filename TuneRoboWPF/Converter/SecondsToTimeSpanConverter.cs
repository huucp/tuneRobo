using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TuneRoboWPF.Converter
{
    public class SecondsToTimeSpanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint seconds = value is uint ? (uint) value : 0;
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            if (seconds == 0) return ":";
            return string.Format("{0:D2}:{1:D2}",t.Minutes,t.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
