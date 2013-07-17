using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBoxUtils;
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

            SubmitButton.IsEnabled = false;
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
            if (!ValidateData()) return;
            Comment();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private bool ValidateData()
        {
            var title = ViewModel.Title;
            var review = ViewModel.Review;
            if (!string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(review))
            {
                var titleWarning = (string)TryFindResource("TitleRatingWarningText");
                WPFMessageBox.Show(this, "", titleWarning, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(review) && string.IsNullOrWhiteSpace(title))
            {

                var titleWarning = (string)TryFindResource("ReviewRatingWarningText");
                WPFMessageBox.Show(this, "", titleWarning, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
            return true;
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
            ratingRequest.ProcessError += (reply, msg) =>
                                              {
                                                  if (reply == null)
                                                  {
                                                      Debug.Fail(msg);
                                                      return;
                                                  }
                                                  Debug.Fail(msg, Enum.GetName(typeof (Reply.Type), reply.type));
                                              };
            GlobalVariables.StoreWorker.AddRequest(ratingRequest);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //SubmitButton.IsEnabled = !string.IsNullOrEmpty(ViewModel.Title);
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void RatingControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            //SubmitButton.IsEnabled = true;
            if (Math.Abs(ViewModel.RatingValue - 0) < 0.0001) SubmitButton.IsEnabled = false;
        }
    }
}
