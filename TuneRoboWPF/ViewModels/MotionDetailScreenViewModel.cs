﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;
using System.Collections.ObjectModel;
namespace TuneRoboWPF.ViewModels
{
    public class MotionDetailScreenViewModel : ViewModelBase
    {
        private MotionDetailScreenModel model = new MotionDetailScreenModel();

        public BitmapImage CoverImage
        {
            get { return model.CoverImage; }
            set
            {
                model.CoverImage = value;
                NotifyPropertyChanged("CoverImage");
            }
        }

        public string DownloadButtonContent
        {
            get { return model.DownloadButtonContent; }
            set
            {
                model.DownloadButtonContent = value;
                NotifyPropertyChanged("DownloadButtonContent");
            }
        }

        public double RatingValue
        {
            get { return model.RatingValue; }
            set
            {
                model.RatingValue = value;
                NotifyPropertyChanged("RatingValue");
            }
        }

        public string ArtistName
        {
            get { return model.ArtistName; }
            set
            {
                model.ArtistName = value;
                NotifyPropertyChanged("ArtistName");
            }
        }

        public ObservableCollection<MotionDetailScreenModel.ScreenshotImage> ScreenshotsList
        {
            get { return model.ScreenshotsList; }
            set
            {
                model.ScreenshotsList = value;
                NotifyPropertyChanged("ScreenshotsList");
            }
        }

        public string MotionDescription
        {
            get { return model.MotionDescription; }
            set
            {
                model.MotionDescription = value;
                NotifyPropertyChanged("MotionDescription");
            }
        }
    }
}