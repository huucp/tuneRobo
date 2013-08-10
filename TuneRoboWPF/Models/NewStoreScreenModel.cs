using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class NewStoreScreenModel
    {
        public ObservableCollection<MotionHorizontalItem> HotMotionsList = new ObservableCollection<MotionHorizontalItem>();
        public ObservableCollection<ArtistItemVertical> ArtistsList = new ObservableCollection<ArtistItemVertical>();
        public ObservableCollection<MotionHorizontalItem> FeaturedMotionsList = new ObservableCollection<MotionHorizontalItem>();
        public BitmapImage ThumbnailSource1 = null;
        public BitmapImage ThumbnailSource2 = null;
        public BitmapImage ThumbnailSource3 = null;
        public BitmapImage ThumbnailSource4 = null;
    }
}
