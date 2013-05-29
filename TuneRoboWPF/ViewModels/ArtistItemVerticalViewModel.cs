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
    public class ArtistItemVerticalViewModel:ViewModelBase
    {
        private ArtistItemVerticalModel model = new ArtistItemVerticalModel();
        public BitmapImage ArtistIcon
        {
            get { return model.ArtistIcon; }
            set
            {
                model.ArtistIcon = value;
                NotifyPropertyChanged("ArtistIcon");
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

        public ICommand DescriptionClick
        {
            get { return model.DescriptionClick ?? (new CommandHandler(MotionClickHandler, true)); }
            set
            {
                model.DescriptionClick = value;
                NotifyPropertyChanged("Description");
            }
        }

        public ICommand ImageClick
        {
            get { return model.ImageClick ?? new CommandHandler(MotionClickHandler, true); }
            set
            {
                model.ImageClick = value;
                NotifyPropertyChanged("ImageClick");
            }
        }

        private void MotionClickHandler()
        {
            Debug.Assert(false, "Not set the motion click event");
        }
    }
}
