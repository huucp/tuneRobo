using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class UpdateProfileWindowViewModel : ViewModelBase
    {
        private UpdateProfileWindowModel model = new UpdateProfileWindowModel();
        public string DisplayName
        {
            get { return model.DisplayName; }
            set
            {
                model.DisplayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }
        public string AvatarUrl
        {
            get { return model.AvatarUrl; }
            set
            {
                model.AvatarUrl = value;
                NotifyPropertyChanged("AvatarUrl");
            }
        }
    }
}
