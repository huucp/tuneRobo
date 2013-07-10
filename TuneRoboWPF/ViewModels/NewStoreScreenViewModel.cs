using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class NewStoreScreenViewModel:ViewModelBase
    {
        private NewStoreScreenModel model = new NewStoreScreenModel();

        public ObservableCollection<MotionHorizontalItem> HotMotionsList
        {
            get { return model.HotMotionsList; }
            set
            {
                if (model.HotMotionsList == value)return;
                model.HotMotionsList = value;
                NotifyPropertyChanged("HotMotionsList");
            }
        }

        public ObservableCollection<ArtistItemVertical> ArtistsList
        {
            get { return model.ArtistsList; }
            set
            {
                if (model.ArtistsList == value) return;
                model.ArtistsList = value;
                NotifyPropertyChanged("ArtistsList");
            }
        }

        public ObservableCollection<MotionHorizontalItem> FeaturedMotionsList
        {
            get { return model.FeaturedMotionsList; }
            set
            {
                if (model.FeaturedMotionsList == value) return;
                model.FeaturedMotionsList = value;
                NotifyPropertyChanged("FeaturedMotionsList");
            }
        }
    }
}
