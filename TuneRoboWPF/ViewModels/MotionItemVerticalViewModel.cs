using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class MotionItemVerticalViewModel : ViewModelBase
    {
        private MotionItemVerticalModel model = new MotionItemVerticalModel();

        public BitmapImage ArtworkImage
        {
            get { return model.ArtworkImage; }
            set
            {
                model.ArtworkImage = value;
                NotifyPropertyChanged("ArtworkImage");
            }
        }

        public string MotionTitle
        {
            get { return model.MotionTitle; }
            set
            {
                model.MotionTitle = value;
                NotifyPropertyChanged("MotionTitle");
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
    }
}
