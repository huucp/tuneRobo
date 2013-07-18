using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TuneRoboWPF.Models
{
    public class MotionFullInfoItemModel
    {
        public BitmapImage CoverImage = null;
        public double RatingValue = 0;
        public string MotionTitle = "Motion title";
        public string ArtistName = "Artist name";
        public string MotionDuration = string.Empty;
        public bool HitTestVisible = false;
        public int Index = 0;
        public bool NeedUpdate = false;
        public ICommand MotionClick = null;
    }
}
