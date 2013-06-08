using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class SearchResultScreenModel
    {
        public string SearchQuery = string.Empty;
        public ObservableCollection<MotionItemVertical> SearchList = new ObservableCollection<MotionItemVertical>();
    }
}
