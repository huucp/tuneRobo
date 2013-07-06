using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
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
        public ObservableCollection<MotionFullInfoItem> LibraryItemsList
        {
            get { return model.LibraryItemsList; }
            set
            {
                model.LibraryItemsList = value;
                NotifyPropertyChanged("LibraryItemsList");
            }
        }

        public MotionTitleItem LastRemoteSelectedMotion { get; set; }

        public MotionTitleItem RemoteSelectedMotion
        {
            get { return model.RemoteSelectedMotion; }
            set
            {
                LastRemoteSelectedMotion = model.RemoteSelectedMotion;
                model.RemoteSelectedMotion = value;
                NotifyPropertyChanged("RemoteSelectedMotion");
            }
        }

        public MotionFullInfoItem LastLibrarySlectedMotion { get; set; }
        public MotionFullInfoItem LibrarySelectedMotion
        {
            get { return model.LibrarySelectedMotion; }
            set
            {
                LastLibrarySlectedMotion = model.LibrarySelectedMotion;
                model.LibrarySelectedMotion = value;
                NotifyPropertyChanged("LibrarySelectedMotion");
            }
        }
        public double Volume
        {
            get { return model.Volume; }
            set
            {
                model.Volume = value;
                NotifyPropertyChanged("Volume");
            }
        }
        public bool VolumeVisibility
        {
            get { return model.VolumeVisibility; }
            set
            {
                if (model.VolumeVisibility == value) return;
                model.VolumeVisibility = value;
                NotifyPropertyChanged("VolumeVisibility");
            }
        }

        public bool NoLocalMotionVisibility
        {
            get { return model.NoLocalMotionVisibility; }
            set
            {
                if(model.NoLocalMotionVisibility == value)return;
                model.NoLocalMotionVisibility = value;
                NotifyPropertyChanged("NoLocalMotionVisibility");
            }
        }

        public bool NoRobotMotionVisibility
        {
            get { return model.NoRobotMotionVisibility; }
            set
            {
                if (model.NoRobotMotionVisibility == value) return;
                model.NoRobotMotionVisibility = value;
                NotifyPropertyChanged("NoRobotMotionVisibility");
            }
        }
        public BitmapImage RobotBackgroundImageSource
        {
            get { return model.RobotBackgroundImageSource; }
            set
            {
                if (Equals(model.RobotBackgroundImageSource, value)) return;
                model.RobotBackgroundImageSource = value;
                NotifyPropertyChanged("RobotBackgroundImageSource");
            }
        }   
    }
}