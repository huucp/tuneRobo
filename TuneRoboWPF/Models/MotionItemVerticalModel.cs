﻿using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TuneRoboWPF.Models
{
    public class MotionItemVerticalModel
    {
        public BitmapImage ArtworkImage = null;
        public string MotionTitle = "Motion title";
        public string ArtistName = "Tosy";
        public ICommand DescriptionClick = null;
        public ICommand ImageClick = null;
    }
}
