using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class StoreScreenViewModel : ViewModelBase
    {
        private StoreScreenModel model = new StoreScreenModel();

        public ObservableCollection<MotionItemVertical> HotItemsList
        {
            get { return model.HotItemsList; }
            set
            {
                model.HotItemsList = value;
                NotifyPropertyChanged("HotItemsList");
            }
        }
        public ObservableCollection<MotionItemVertical> FeaturedItemsList
        {
            get { return model.FeaturedItemsList; }
            set
            {
                model.FeaturedItemsList = value;
                NotifyPropertyChanged("FeaturedItemList");
            }
        }
        public ObservableCollection<ArtistItemVertical> ArtistList
        {
            get { return model.ArtistList; }
            set
            {
                model.ArtistList = value;
                NotifyPropertyChanged("ArtistList");
            }
        }
    }
}
