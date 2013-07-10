using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class MotionHorizontalItemViewModel:ViewModelBase
    {
        private MotionHorizontalItemModel model = new MotionHorizontalItemModel();
        public string MotionTitle
        {
            get { return model.MotionTitle; }
            set
            {
                if (model.MotionTitle == value)return;
                model.MotionTitle = value;
                NotifyPropertyChanged("MotionTitle");
            }
        }

        public string ArtistName
        {
            get { return model.ArtistName; }
            set
            {
                if (model.ArtistName == value) return;
                model.ArtistName = value;
                NotifyPropertyChanged("ArtistName");
            }
        }

        public BitmapImage CoverImage
        {
            get { return model.CoverImage; }
            set
            {
                if (Equals(model.CoverImage, value)) return;
                model.CoverImage = value;
                NotifyPropertyChanged("CoverImage");
            }
        }

        public double RatingValue
        {
            get { return model.RatingValue; }
            set
            {
                if (Math.Abs(model.RatingValue - value) < 0.01) return;
                model.RatingValue = value;
                NotifyPropertyChanged("RatingValue");
            }
        }

        public ulong NumberRatings
        {
            get { return model.NumberRatings; }
            set
            {
                if (model.NumberRatings == value) return;
                model.NumberRatings = value;
                NotifyPropertyChanged("NumberRatings");
            }
        }

        public ICommand MotionClick
        {
            get { return model.MotionClick ?? new CommandHandler(MotionClickHandler, true); }
            set
            {
                if (model.MotionClick== value) return;
                model.MotionClick = value;
                NotifyPropertyChanged("MotionClick");
            }
        }
        private void MotionClickHandler()
        {
            Debug.Assert(false, "Not set the motion click event");
        }
    }
}
