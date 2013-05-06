using System.Collections.ObjectModel;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class RemoteControlScreenModel
    {
        public ObservableCollection<MotionTitleItem> RemoteItemsList = new ObservableCollection<MotionTitleItem>();
        public ObservableCollection<MotionTitleItem> LibraryItemsList = new ObservableCollection<MotionTitleItem>();
        public MotionTitleItem RemoteSelectedMotion;
    }
}
