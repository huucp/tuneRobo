using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TuneRoboWPF.ViewModels
{
    public class RemoteControlScreenModel : ViewModelBase
    {
        public RemoteControlScreenModel()
        {

        }

        private ObservableCollection<MotionTitleItem> remoteListItem;
        public ObservableCollection<MotionTitleItem> RemoteListItem
        {
            get { return remoteListItem; }
            set
            {
                remoteListItem = value;
                NotifyPropertyChanged("RemoteListItem");
            }
        }

        public MotionTitleItem LastSelectedMotionItem { get; set; }
        private MotionTitleItem selectedMotion;
        public MotionTitleItem SelectedMotion
        {
            get { return selectedMotion; }
            set
            {
                LastSelectedMotionItem = selectedMotion;                
                selectedMotion = value;
                NotifyPropertyChanged("SelectedMotion");
            }
        }
    }
}