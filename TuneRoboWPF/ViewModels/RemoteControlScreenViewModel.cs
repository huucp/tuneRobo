using System.Collections.ObjectModel;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;


namespace TuneRoboWPF.ViewModels
{
    public class RemoteControlScreenViewModel : ViewModelBase
    {
        private RemoteControlScreenModel model = new RemoteControlScreenModel();

        private ObservableCollection<MotionTitleItem> _remoteItemsList;
        public ObservableCollection<MotionTitleItem> RemoteItemsList
        {
            get { return model.RemoteItemsList; }
            set
            {
                model.RemoteItemsList = value;
                NotifyPropertyChanged("RemoteItemsList");
            }
        }
        public ObservableCollection<MotionTitleItem> LibraryItemsList
        {
            get { return model.LibraryItemsList; }
            set
            {
                model.LibraryItemsList = value;
                NotifyPropertyChanged("LibraryItemsList");
            }
        }

        public MotionTitleItem LastSelectedMotionItem { get; set; }
        
        public MotionTitleItem RemoteSelectedMotion
        {
            get { return model.RemoteSelectedMotion; }
            set
            {
                LastSelectedMotionItem = model.RemoteSelectedMotion;                
                model.RemoteSelectedMotion = value;
                NotifyPropertyChanged("RemoteSelectedMotion");
            }
        }
    }
}