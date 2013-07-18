using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class MotionFullInfoItemViewModel : ViewModelBase
    {
        private MotionFullInfoItemModel model = new MotionFullInfoItemModel();

        public BitmapImage CoverImage
        {
            get { return model.CoverImage; }
            set
            {
                model.CoverImage = value;
                NotifyPropertyChanged("CoverImage");
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

        public string MotionDuration
        {
            get { return model.MotionDuration; }
            set
            {
                model.MotionDuration = value;
                NotifyPropertyChanged("MotionDuration");
            }
        }

        public bool HitTestVisible
        {
            get { return model.HitTestVisible; }
            set
            {
                model.HitTestVisible = value;
                NotifyPropertyChanged("HitTestVisible");
            }
        }

        public int Index
        {
            get { return model.Index; }
            set
            {
                model.Index = value;
                NotifyPropertyChanged("Index");
            }
        }

        public bool NeedUpdate
        {
            get { return model.NeedUpdate; }
            set
            {
                if (model.NeedUpdate == value) return;
                model.NeedUpdate = value;
                NotifyPropertyChanged("NeedUpdate");
            }
        }

        public ICommand MotionClick
        {
            get { return model.MotionClick ?? new CommandHandler(MotionClickHandler, true); }
            set
            {
                if (model.MotionClick == value) return;
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
