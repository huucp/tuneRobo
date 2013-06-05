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
    }
}
