using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for CommentItem.xaml
    /// </summary>
    public partial class CommentItem : UserControl
    {
        private RatingInfo Info { get; set; }
        private CommentItemViewModel ViewModel { get; set; }
        public CommentItem(RatingInfo ratingInfo)
        {
            InitializeComponent();

            Info = ratingInfo;
            DataContext = new CommentItemViewModel();
            ViewModel = (CommentItemViewModel)DataContext;

            ViewModel.CommentTitle = ratingInfo.comment_title;
            ViewModel.RatingValue = ratingInfo.rating / GlobalVariables.RateValueMultiplierFactor;
            ViewModel.UserReview = FormatUserReviewString(ratingInfo.user_name, ratingInfo.rating_time);
            ViewModel.CommentContent = ratingInfo.comment_content;
        }

        private string FormatUserReviewString(string username, ulong time)
        {
            DateTime reviewDate = GlobalFunction.ConvertFromServerTimestamp(time);
            return string.Format("by {0} - {1:MMM dd,yyyy}", username, reviewDate);
        }             
    }
}
