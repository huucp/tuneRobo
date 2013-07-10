using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TuneRoboWPF.Models
{
    public class MotionHorizontalItemModel
    {
        public string MotionTitle = string.Empty;
        public string ArtistName = string.Empty;
        public BitmapImage CoverImage = null;
        public double RatingValue = 0;
        public ulong NumberRatings = 0;
        public ICommand MotionClick = null;
    }
}
