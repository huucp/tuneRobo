using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.ViewModels
{
    public class MotionTitleItemViewModel : ViewModelBase
    {
        private string title = "Motion title";
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string rectangleFillColor = "Yellow";
        public string RectangleFillColor
        {
            get { return rectangleFillColor; }
            set
            {
                rectangleFillColor = value;
                NotifyPropertyChanged("RectangleFillColor");
            }
        }
    }
}