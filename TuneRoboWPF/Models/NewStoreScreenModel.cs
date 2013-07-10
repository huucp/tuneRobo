using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class NewStoreScreenModel
    {
        public ObservableCollection<MotionHorizontalItem> HotMotionsList = new ObservableCollection<MotionHorizontalItem>();
        public ObservableCollection<ArtistItemVertical> ArtistsList = new ObservableCollection<ArtistItemVertical>();
        public ObservableCollection<MotionHorizontalItem> FeaturedMotionsList = new ObservableCollection<MotionHorizontalItem>();
    }
}
