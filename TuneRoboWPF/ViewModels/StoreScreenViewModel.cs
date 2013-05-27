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

        public ObservableCollection<MotionFullInfoItem> HotItemsList
        {
            get { return model.HotItemsList; }
            set
            {
                model.HotItemsList = value;
                NotifyPropertyChanged("HotItemsList");
            }
        }
    }
}
