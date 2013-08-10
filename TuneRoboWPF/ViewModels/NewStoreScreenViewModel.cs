using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class NewStoreScreenViewModel : ViewModelBase
    {
        private NewStoreScreenModel model = new NewStoreScreenModel();

        public ObservableCollection<MotionHorizontalItem> HotMotionsList
        {
            get { return model.HotMotionsList; }
            set
            {
                if (model.HotMotionsList == value) return;
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

        public BitmapImage ThumbnailSource1
        {
            get { return model.ThumbnailSource1; }
            set
            {
                if (Equals(model.ThumbnailSource1, value)) return;
                model.ThumbnailSource1 = value;
                NotifyPropertyChanged("ThumbnailSource1");
            }
        }
        public BitmapImage ThumbnailSource2
        {
            get { return model.ThumbnailSource2; }
            set
            {
                if (Equals(model.ThumbnailSource2, value)) return;
                model.ThumbnailSource2 = value;
                NotifyPropertyChanged("ThumbnailSource2");
            }
        }
        public BitmapImage ThumbnailSource3
        {
            get { return model.ThumbnailSource3; }
            set
            {
                if (Equals(model.ThumbnailSource3, value)) return;
                model.ThumbnailSource3 = value;
                NotifyPropertyChanged("ThumbnailSource3");
            }
        }
        public BitmapImage ThumbnailSource4
        {
            get { return model.ThumbnailSource4; }
            set
            {
                if (Equals(model.ThumbnailSource4, value)) return;
                model.ThumbnailSource4 = value;
                NotifyPropertyChanged("ThumbnailSource4");
            }
        }
    }
}
