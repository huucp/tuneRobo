using System;
using System.Windows;
using System.Windows.Controls;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF
{
    /// <summary>
    /// Interaction logic for NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        public NavigationBar()
        {
            InitializeComponent();
            var viewModels = new NavigationBarViewModel();
            DataContext = viewModels;
            viewModel = (NavigationBarViewModel)DataContext;
        }

        private NavigationBarViewModel viewModel = new NavigationBarViewModel();

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == true)
            {
                UserMenu.Visibility = Visibility.Visible;
                SignInButton.Visibility = Visibility.Collapsed;
                viewModel.Username = GlobalVariables.CurrentUser;
            }
        }

        private void RemoteButton_Click(object sender, RoutedEventArgs e)
        {
            RemoteButton.Style = (Style)FindResource("ButtonFlatStyle");
        }

        private void AccountMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SignOutMenu_Click(object sender, RoutedEventArgs e)
        {
            var signoutRequest = new SignoutStoreRequest();
            signoutRequest.ProcessSuccessfully += (reply) =>
                                                      {
                                                          Console.WriteLine("signout successfully");
                                                      };
            signoutRequest.ProcessError += (reply, msg) =>
                                               {
                                                   Console.WriteLine("signout failed");
                                               };
            GlobalVariables.StoreWorker.AddJob(signoutRequest);
        }
    }
}