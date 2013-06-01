using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class CommentItemViewModel : ViewModelBase
    {
        private CommentItemModel model = new CommentItemModel();

        public double RatingValue
        {
            get { return model.RatingValue; }
            set
            {
                model.RatingValue = value;
                NotifyPropertyChanged("RatingValue");
            }
        }

        public string CommentTitle
        {
            get { return model.CommentTitle; }
            set
            {
                model.CommentTitle = value;
                NotifyPropertyChanged("CommentTitle");
            }
        }

        public string UserReview
        {
            get { return model.UserReview; }
            set
            {
                model.UserReview = value;
                NotifyPropertyChanged("UserReview");
            }
        }

        public string CommentContent
        {
            get { return model.CommentContent; }
            set
            {
                model.CommentContent = value;
                NotifyPropertyChanged("CommentContent");
            }
        }
    }
}
