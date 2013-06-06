using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class ArtistDetailScreenViewModel : ViewModelBase
    {
        private ArtistDetailScreenModel model = new ArtistDetailScreenModel();

        public BitmapImage ArtistAvatar
        {
            get { return model.ArtistAvatar; }
            set
            {
                model.ArtistAvatar = value;
                NotifyPropertyChanged("ArtistAvatar");
            }
        }

        public ObservableCollection<MotionItemVertical> ArtistMotionsList
        {
            get { return model.ArtistMotionsList; }
            set
            {
                model.ArtistMotionsList = value;
                NotifyPropertyChanged("ArtistMotionsList");
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

        public uint NumberMotion
        {
            get { return model.NumberMotion; }
            set
            {
                model.NumberMotion = value;
                NotifyPropertyChanged("NumberMotion");
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

        public string Biography
        {
            get { return model.Biography; }
            set
            {
                model.Biography = value;
                NotifyPropertyChanged("Biography");
            }
        }
    }
}
