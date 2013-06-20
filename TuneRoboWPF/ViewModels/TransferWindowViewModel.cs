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
        public int Percentage
        {
            get { return model.Percentage; }
            set
            {
                model.Percentage = value;
                NotifyPropertyChanged("Percentage");
            }
        }
        public string TransferText
        {
            get { return model.TransferText; }
            set
            {
                model.TransferText = value;
                NotifyPropertyChanged("TransferText");
            }
        }
    }
}
