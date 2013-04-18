using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuneRoboWPF.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
        private string username = "";
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                NotifyPropertyChanged("Username");
            }
        }       
    }
}
