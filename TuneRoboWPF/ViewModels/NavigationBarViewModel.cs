using TuneRoboWPF.Models;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
        private NavigationBarModel model = new NavigationBarModel();

        public string Username
        {
            get { return model.Username; }
            set
            {
                if (model.Username == value) return;
                model.Username = value;
                NotifyPropertyChanged("Username");
            }
        }

        public bool PreviousEnable
        {
            get { return model.PreviousEnable; }
            set
            {
                if (model.PreviousEnable == value) return;
                model.PreviousEnable = value;
                NotifyPropertyChanged("PreviousEnable");
            }
        }

        public bool ForwardEnable
        {
            get { return model.ForwardEnable; }
            set
            {
                if (model.ForwardEnable == value) return;
                model.ForwardEnable = value;
                NotifyPropertyChanged("ForwardEnable");
            }
        }

        public bool OnStore
        {
            get { return model.OnStore; }
            set
            {
                if (model.OnStore == value) return;
                model.OnStore = value;
                NotifyPropertyChanged("OnStore");
            }
        }
    }
}
