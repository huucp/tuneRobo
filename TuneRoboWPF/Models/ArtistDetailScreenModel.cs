using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class ArtistDetailScreenModel
    {
        public BitmapImage ArtistAvatar = null;
        public ObservableCollection<MotionItemVertical> ArtistMotionsList = new ObservableCollection<MotionItemVertical>();
        public string ArtistName = string.Empty;
        public uint NumberMotion = 0;
        public double RatingValue = 0;
        public string Biography = string.Empty;
        public bool FollowButtonVisibility = false;
        public bool FollowState = false;
        public string NumberRate = string.Empty;
    }
}
