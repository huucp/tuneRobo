using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using comm;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for RatingWindow.xaml
    /// </summary>
    public partial class RatingWindow : Window
    {
        private RatingWindowViewModel ViewModel = new RatingWindowViewModel();
        private ulong MotionID { get; set; }
        private ulong VersionID { get; set; }
        public RatingWindow()
        {
            InitializeComponent();

            DataContext = new RatingWindowViewModel();
            ViewModel = (RatingWindowViewModel)DataContext;

            
        }

        private void UpdateExistComment()
        {
            var commentRequest = new GetUserRatingStoreRequest(MotionID);
            commentRequest.ProcessSuccessfully += (reply) =>
            {
                var ratingInfo = reply.my_rating_info.rating_info;                                                      
                if ( ratingInfo != null)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        ViewModel.Title = ratingInfo.comment_title;
                        ViewModel.Review = ratingInfo.comment_content;
                        ViewModel.RatingValue = ratingInfo.rating/GlobalVariables.RateValueMultiplierFactor;
                    });
                }
            };
            commentRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.ForceAddRequest(commentRequest);
        }

        public void SetInfo(ulong motionID, ulong versionID)
        {
            MotionID = motionID;
            VersionID = versionID;
            UpdateExistComment();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Comment();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Comment()
        {
            var ratingRequest = new RatingMotionStoreRequest(MotionID, (uint)(ViewModel.RatingValue * GlobalVariables.RateValueMultiplierFactor), VersionID,
                                                             ViewModel.Title, ViewModel.Review);
            ratingRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate()
                {
                    DialogResult = true;
                    Cursor = Cursors.Arrow;
                    Close();
                });
            ratingRequest.ProcessError += (reply, msg) => Debug.Fail(msg,Enum.GetName(typeof(Reply.Type),reply.type));
            GlobalVariables.StoreWorker.AddRequest(ratingRequest);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitButton.IsEnabled = !string.IsNullOrEmpty(ViewModel.Title);
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}
