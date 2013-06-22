using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
namespace TuneRoboWPF.Converter
{
    public class IndexToColorConverter : IValueConverter
    {
        private const string MotionFullItemBackgroundEven = "#FAFAFA";
        private const string MotionFullItemBackgroundOdd = "#F1F4F7";
        private const string MotionFullItemTopSeperatorEven = "#FEFEFE";
        private const string MotionFullItemTopSeperatorOdd = "#ECECEC";
        private const string MotionFullItemBottomSeperatorEven = "#F6F9FB";
        private const string MotionFullItemBottomSeperatorOdd = "#E7E9EC";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = value is int ? (int)value : 0;
            int type = parameter is string ? Int32.Parse((string)parameter) : 0;
            switch (type)
            {
                case Background:
                    if (index % 2 == 0)
                    {
                        return MotionFullItemBackgroundEven;
                    }
                    return MotionFullItemBackgroundOdd;
                case TopSeperator:
                    if (index % 2 == 0)
                    {
                        return MotionFullItemTopSeperatorEven;
                    }
                    return MotionFullItemTopSeperatorOdd;
                case BottomSeperator:

                    if (index % 2 == 0)
                    {
                        return MotionFullItemBottomSeperatorEven;
                    }
                    return MotionFullItemBottomSeperatorOdd;
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
