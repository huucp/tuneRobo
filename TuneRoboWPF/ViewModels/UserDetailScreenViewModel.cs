using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.ViewModels
{
    public class UserDetailScreenViewModel : ViewModelBase
    {
        private UserDetailScreenModel model = new UserDetailScreenModel();
        public string Username
        {
            get { return model.Username; }
            set
            {
                model.Username = value;
                NotifyPropertyChanged("Username");
            }
        }
        public BitmapImage Avatar
        {
            get { return model.Avatar; }
            set
            {
                model.Avatar = value;
                NotifyPropertyChanged("Avatar");
            }
        }
        public ObservableCollection<MotionItemVertical> PurchasedMotionList
        {
            get { return model.PurchasedMotionList; }
            set { model.PurchasedMotionList = value;
            NotifyPropertyChanged("PurchasedMotionList");}
        }
    }
}
