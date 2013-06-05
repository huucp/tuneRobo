using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class ArtistDetailScreenModel
    {
        public BitmapImage ArtistAvatar = null;
        public ObservableCollection<MotionItemVertical> ArtistMotionsList = new ObservableCollection<MotionItemVertical>();
    }
}
