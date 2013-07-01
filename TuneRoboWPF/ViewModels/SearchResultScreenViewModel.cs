using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
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
            get
            {
                return string.Format("{1} \"{0}\"", model.SearchQuery,
                                     Application.Current.TryFindResource("ShowResultForText"));
            }
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

        public bool NoResultVisibility
        {
            get { return model.NoResultVisibility; }
            set
            {
                if (model.NoResultVisibility == value) return;                
                model.NoResultVisibility = value;
                NotifyPropertyChanged("NoResultVisibility");
            }
        }
        public string NoResultText
        {
            get
            {
                return string.Format("{1} \"{0}\"", model.SearchQuery,
                                     Application.Current.TryFindResource("NoResultForText"));
            }
            set
            {
                if (model.NoResultText == value) return;
                model.NoResultText = value;
                NotifyPropertyChanged("NoResultText");
            }
        }
    }
}
