using System.Collections.ObjectModel;
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
    }
}
