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
    }
}
