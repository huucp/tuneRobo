using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MessageBoxUtils;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for UpdateProfileWindow.xaml
    /// </summary>
    public partial class UpdateProfileWindow : Window
    {
        private UpdateProfileWindowViewModel ViewModel { get; set; }
        public UpdateProfileWindow()
        {
            this.InitializeComponent();

            DataContext = new UpdateProfileWindowViewModel();
            ViewModel = (UpdateProfileWindowViewModel)DataContext;
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
            string displayName = ViewModel.DisplayName;
            string avatarUrl = ViewModel.AvatarUrl;
            if (!string.IsNullOrWhiteSpace(displayName) ||
                !string.IsNullOrWhiteSpace(avatarUrl))
            {
                if (!GlobalFunction.IsAnImage(avatarUrl))
                {
                    var title = (string)TryFindResource("AvatarNotImageText");
                    WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string displayName = ViewModel.DisplayName;
            string avatarUrl = ViewModel.AvatarUrl;
            if (ValidateData())
            {
                Cursor = Cursors.Wait;
                if (avatarUrl != null) avatarUrl = avatarUrl.Trim();
                UpdateProfile(displayName, avatarUrl);
            }
        }

        private void UpdateProfile(string displayName, string avatarUrl)
        {
            var updateProfileRequest = new SetUserInfoStoreRequest(displayName, avatarUrl);
            updateProfileRequest.ProcessSuccessfully += reply =>
                Dispatcher.BeginInvoke((Action)(delegate
                {
                    var msg = (string)TryFindResource("UpdateProfileSuccessfullyText");
                    WPFMessageBox.Show(this, "", msg, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    DialogResult = true;
                    Close();
                    Cursor = Cursors.Arrow;
                }));
            updateProfileRequest.ProcessError += (reply, msg) =>
            {
                Debug.Fail(reply.type.ToString(), msg);
                Dispatcher.BeginInvoke((Action)(delegate
                {
                    var msgError = (string)TryFindResource("CheckDefaultErrorText");
                    var tittle = (string)TryFindResource("UpdateProfileFailedText");
                    WPFMessageBox.Show(this, msgError, tittle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    Cursor = Cursors.Arrow;
                }));

            };
            GlobalVariables.StoreWorker.AddRequest(updateProfileRequest);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DisplayNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateButton.IsEnabled = !string.IsNullOrEmpty(ViewModel.DisplayName) | !string.IsNullOrEmpty(ViewModel.AvatarUrl);
        }

        private void AvatarUrlTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateButton.IsEnabled = !string.IsNullOrEmpty(ViewModel.DisplayName) | !string.IsNullOrEmpty(ViewModel.AvatarUrl);
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}