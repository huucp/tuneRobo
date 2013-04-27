using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class RemoteControlScreenModel
    {
        public ObservableCollection<MotionTitleItem> RemoteListItem;
        public MotionTitleItem SelectedMotion;
    }
}
