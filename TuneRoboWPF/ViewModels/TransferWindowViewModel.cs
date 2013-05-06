using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class TransferWindowViewModel : ViewModelBase
    {
        private TransferWindowModel model = new TransferWindowModel();
        public string Title
        {
            get { return model.Title; }
            set
            {
                model.Title = value;
                NotifyPropertyChanged("Title");
            }
        }
        public string WindowTitle
        {
            get { return model.WindowTitle; }
            set
            {
                model.WindowTitle = value;
                NotifyPropertyChanged("WindowTitle");
            }
        }
    }
}
