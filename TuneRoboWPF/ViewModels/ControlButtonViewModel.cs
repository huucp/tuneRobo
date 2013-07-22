using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class ControlButtonViewModel : ViewModelBase
    {
        private ControlButtonModel model = new ControlButtonModel();
        public string InactiveImageSource { get; set; }
        public string ActiveImageSource { get; set; }

        public string ImageSource
        {
            get
            {
                if (Active) model.ImageSource = ActiveImageSource;
                //else model.ImageSource = InactiveImageSource;
                return model.ImageSource;
            }
        }

        public bool Active
        {
            get { return model.Active; }
            set
            {
                if (model.Active == value) return;                
                model.Active = value;
                NotifyPropertyChanged("ImageSource");
            }
        }
    }
}
