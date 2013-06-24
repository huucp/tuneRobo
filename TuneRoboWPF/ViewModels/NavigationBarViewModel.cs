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
                model.Username = value;
                NotifyPropertyChanged("Username");
            }
        }               
    }
}
