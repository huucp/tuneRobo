using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using TuneRoboWPF.Utility;
//drop shadow
namespace TuneRoboWPF.Converter
{
    public class IndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = value is int ? (int)value : 0;
            int type = parameter is string ? int.Parse((string)parameter) : 0;
            switch (type)
            {
                case Background:
                    if (index % 2 == 0)
                    {
                        return Color.MotionFullItemBackgroundEven;
                    }
                    return Color.MotionFullItemBackgroundOdd;
                case TopSeperator:
                    if (index % 2 == 0)
                    {
                        return Color.MotionFullItemTopSeperatorEven;
                    }
                    return Color.MotionFullItemTopSeperatorOdd;
                case BottomSeperator:

                    if (index % 2 == 0)
                    {
                        return Color.MotionFullItemBottomSeperatorEven;
                    }
                    return Color.MotionFullItemBottomSeperatorOdd;
                default:
                    Debug.Assert(type != 0, "Null type");
                    return null;
            }
        }

        protected const int Background = 0;
        protected const int TopSeperator = 1;
        protected const int BottomSeperator = 2;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
