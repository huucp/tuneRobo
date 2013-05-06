using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class TempStoreScreenViewModel : ViewModelBase
    {
        private TempStoreScreenModel model = new TempStoreScreenModel();

        public ObservableCollection<MotionTitleItem> HotItemsList
        {
            get { return model.HotItemsList; }
            set
            {
                model.HotItemsList = value;
                NotifyPropertyChanged("HotItemsList");
            }
        }

        public ObservableCollection<MotionTitleItem> FeaturedItemsList
        {
            get { return model.FeaturedItemsList; }
            set
            {
                model.FeaturedItemsList = value;
                NotifyPropertyChanged("FeaturedItemsList");
            }
        }

        public ObservableCollection<MotionTitleItem> ArtistItemsList
        {
            get { return model.ArtistItemsList; }
            set
            {
                model.ArtistItemsList = value;
                NotifyPropertyChanged("ArtistItemsList");
            }
        }
    }
}
