using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class RemoteControlScreenModel
    {
        public ObservableCollection<MotionTitleItem> RemoteItemsList = new ObservableCollection<MotionTitleItem>();
        public ObservableCollection<MotionFullInfoItem> LibraryItemsList = new ObservableCollection<MotionFullInfoItem>();
        public MotionTitleItem RemoteSelectedMotion;
        public MotionFullInfoItem LibrarySelectedMotion;
        public double Volume = 0;
        public bool VolumeVisibility = false;
        public bool NoLocalMotionVisibility = false;
        public bool NoRobotMotionVisibility = false;
        public BitmapImage RobotBackgroundImageSource = null;
    }
}
