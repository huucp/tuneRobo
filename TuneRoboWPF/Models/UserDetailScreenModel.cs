using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class UserDetailScreenModel
    {
        public string Username = string.Empty;
        public BitmapImage Avatar = null;
        public ObservableCollection<MotionItemVertical> PurchasedMotionList = new ObservableCollection<MotionItemVertical>();
    }
}
