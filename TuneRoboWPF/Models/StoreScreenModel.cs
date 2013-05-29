using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class StoreScreenModel
    {
        public ObservableCollection<MotionItemVertical> HotItemsList = new ObservableCollection<MotionItemVertical>();
        public ObservableCollection<MotionItemVertical> FeaturedItemsList = new ObservableCollection<MotionItemVertical>();
        public ObservableCollection<ArtistItemVertical> ArtistList = new ObservableCollection<ArtistItemVertical>();
    }
}
