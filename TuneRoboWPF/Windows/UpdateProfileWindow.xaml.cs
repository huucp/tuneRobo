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
		    ViewModel = (UpdateProfileWindowViewModel) DataContext;
		}

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string displayName = ViewModel.DisplayName;
            string avatarUrl = ViewModel.AvatarUrl.Trim();
            if (!string.IsNullOrWhiteSpace(displayName) ||
                !string.IsNullOrWhiteSpace(avatarUrl))
            {
                UpdateProfile(displayName, avatarUrl);                
            }
        }

	    private void UpdateProfile(string displayName, string avatarUrl)
	    {
	        var updateProfileRequest = new SetUserInfoStoreRequest(displayName, avatarUrl);
	        updateProfileRequest.ProcessSuccessfully += reply =>
                Dispatcher.BeginInvoke((Action) (delegate
                {
                    //MessageBox.Show("Update successfully");
                    WPFMessageBox.Show(this,"Update Successfully","Update Profile",MessageBoxButton.OK,MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }));
	        updateProfileRequest.ProcessError += (reply, msg) =>
            {
                Debug.Fail(reply.type.ToString(),msg);
                Dispatcher.BeginInvoke((Action)(() => MessageBox.Show("Update failed")));
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