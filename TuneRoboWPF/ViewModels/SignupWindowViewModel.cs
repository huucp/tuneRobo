using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class SignupWindowViewModel : ViewModelBase
    {
        private SignupWindowModel model = new SignupWindowModel();
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
        public string Username
        {
            get { return model.Username; }
            set
            {
                if (model.Username == value) return;
                model.Username = value;
                NotifyPropertyChanged("Username");
            }
        }
        public string Avatar
        {
            get { return model.Avatar; }
            set
            {
                if (model.Avatar == value) return; ;
                model.Avatar = value;
                NotifyPropertyChanged("Avatar");
            }
        }
    }
}
