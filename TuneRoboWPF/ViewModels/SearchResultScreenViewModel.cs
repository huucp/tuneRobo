using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using TuneRoboWPF.Models;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class SearchResultScreenViewModel : ViewModelBase
    {
        private SearchResultScreenModel model = new SearchResultScreenModel();

        public string SearchQuery
        {
            get { return string.Format("Showing results for \"{0}\"",model.SearchQuery); }
            set
            {
                model.SearchQuery = value;
                NotifyPropertyChanged("SearchQuery");
            }
        }

        public ObservableCollection<MotionItemVertical> SearchList
        {
            get { return model.SearchList; }
            set
            {
                model.SearchList = value;
                NotifyPropertyChanged("SearchList");
            }
        }
    }
}
