using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
//drop shadow
namespace TuneRoboWPF.Converter
{
    public class IndexToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = value is int ? (int) value : 0;
            //if (index == -1) return "#9db8d8";
            if (index % 2 == 0)
            {
                return "#F0F0F0";
            }
            return "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
