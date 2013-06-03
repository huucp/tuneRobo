using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class RatingWindowViewModel : ViewModelBase
    {
        private RatingWindowModel model = new RatingWindowModel();
        public string Title
        {
            get { return model.Title; }
            set
            {
                model.Title = value;
                NotifyPropertyChanged("Title");
            }
        }
        public string Nickname
        {
            get { return model.Nickname; }
            set
            {
                model.Nickname = value;
                NotifyPropertyChanged("Nickname");
            }
        }
        public string Review
        {
            get { return model.Review; }
            set
            {
                model.Review = value;
                NotifyPropertyChanged("Review");
            }
        }
        public double RatingValue
        {
            get { return model.RatingValue; }
            set
            {
                model.RatingValue = value;
                NotifyPropertyChanged("RatingValue");
            }
        }
    }
}
