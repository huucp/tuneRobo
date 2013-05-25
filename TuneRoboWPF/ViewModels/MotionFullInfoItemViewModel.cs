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
    }
}
