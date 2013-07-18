using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class ForgotWindowViewModel : ViewModelBase
    {
        private ForgotWindowModel model = new ForgotWindowModel();
        public string Email
        {
            get { return model.Email; }
            set
            {
                if (model.Email == value) return;
                model.Email = value;
                NotifyPropertyChanged("Email");
            }
        }
        public bool EnableUI
        {
            get { return model.EnableUI; }
            set
            {
                if (model.EnableUI == value) return;
                model.EnableUI = value;
                NotifyPropertyChanged("EnableUI");
            }
        }
    }
}
