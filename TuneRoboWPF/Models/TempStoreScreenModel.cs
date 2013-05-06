using System.Collections.ObjectModel;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class TempStoreScreenModel
    {
        public ObservableCollection<MotionTitleItem> HotItemsList = new ObservableCollection<MotionTitleItem>();
        public ObservableCollection<MotionTitleItem> FeaturedItemsList = new ObservableCollection<MotionTitleItem>();
        public ObservableCollection<MotionTitleItem> ArtistItemsList = new ObservableCollection<MotionTitleItem>();
    }
}
