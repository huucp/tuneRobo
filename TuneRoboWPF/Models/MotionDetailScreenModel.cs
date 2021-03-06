﻿using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class MotionDetailScreenModel
    {
        public BitmapImage CoverImage = null;
        public string DownloadButtonContent = string.Empty;
        public double RatingValue = 0;
        public string ArtistName = "Tosy";
        public ObservableCollection<ScreenshotImage> ScreenshotsList = new ObservableCollection<ScreenshotImage>();
        public BitmapImage ImageSource = null;        

        public string MotionDescription = string.Empty;
        public ObservableCollection<MotionItemVertical> RelatedMotionsList = new ObservableCollection<MotionItemVertical>();
        public string MoreByTextBlock = "Related motion";
        public string MotionTitle = string.Empty;
        public string NumberRating = string.Empty;
    }
}
