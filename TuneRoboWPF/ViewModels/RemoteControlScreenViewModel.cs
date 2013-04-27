using System.Collections.ObjectModel;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;


namespace TuneRoboWPF.ViewModels
{
    public class RemoteControlScreenViewModel : ViewModelBase
    {
        private RemoteControlScreenModel model = new RemoteControlScreenModel();
        
        public ObservableCollection<MotionTitleItem> RemoteListItem
        {
            get { return model.RemoteListItem; }
            set
            {
                model.RemoteListItem = value;
                NotifyPropertyChanged("RemoteListItem");
            }
        }

        public MotionTitleItem LastSelectedMotionItem { get; set; }
        
        public MotionTitleItem SelectedMotion
        {
            get { return model.SelectedMotion; }
            set
            {
                LastSelectedMotionItem = model.SelectedMotion;                
                model.SelectedMotion = value;
                NotifyPropertyChanged("SelectedMotion");
            }
        }
    }
}