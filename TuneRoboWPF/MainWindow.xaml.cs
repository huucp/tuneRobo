using System;
using System.Windows;
using System.Windows.Controls;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.Views;

namespace TuneRoboWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            navigationBar.LoginProcessSuccessfully += navigationBar_LoginProcessSuccessfully;            
        }

        private void navigationBar_LoginProcessSuccessfully(object sender)
        {
            UpdateLoginSuccessfully();
            GetNotificationFromStore();
        }

        public void UpdateLoginSuccessfully()
        {
            navigationBar.UserMenu.Visibility = Visibility.Visible;
            navigationBar.SignInButton.Visibility = Visibility.Hidden;
            navigationBar.ViewModel.Username = GlobalVariables.CurrentUser;

            var lastElement = MainDock.Children[MainDock.Children.Count - 1];
            if (lastElement is ArtistDetailScreen)
            {                
                ((ArtistDetailScreen)lastElement).CheckFollowState();
            }

        }

        

        private void GetNotificationFromStore()
        {
            var notificationRequest = new GetNotificationStoreRequest();
            notificationRequest.ProcessSuccessfully += (reply) =>
            {
                Console.WriteLine("GetNotificationFromStore succeed ");
                Dispatcher.BeginInvoke((Action)delegate
                {
                    //testStoreScreen.Visibility = Visibility.Visible;
                    //remoteScreen.Visibility = Visibility.Collapsed;
                });
                
            };
            notificationRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("GetNotificationFromStore failed: " + msg);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    //testStoreScreen.Visibility = Visibility.Visible;
                    //remoteScreen.Visibility = Visibility.Collapsed;
                });
            };
            GlobalVariables.StoreWorker.AddRequest(notificationRequest);
        }

        private void MainScreen_ContentRendered(object sender, EventArgs e)
        {
            //if (GlobalVariables.ServerConnection.ConfigAndConnectSocket() == 0)
            //{
            //    MessageBox.Show("Cannot connect to server!", "Connection error", MessageBoxButton.OK,
            //        MessageBoxImage.Error);
            //}
        }

        private void MainScreen_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalFunction.GetTempDataFolder();            
            if (GlobalVariables.ServerConnection.ConfigAndConnectSocket() == 0)
            {
                MessageBox.Show("Cannot connect to server!", "Connection error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            //var remoteScreen = new RemoteControlScreen(MainDock);
            //var remoteScreen = new TestRemoteScreen(MainDock);
            var storeScreen = new StoreScreen(MainDock);
            MainDock.Children.Add(storeScreen);
        }

   
        private void navigationBar_StoreButtonClick(object sender, RoutedEventArgs e)
        {           
            var testStoreScreen = new StoreScreen(MainDock);
            ChangeScreen(testStoreScreen);
        }

        private void navigationBar_RemoteButtonClick(object sender, RoutedEventArgs e)
        {
            var remoteScreen = new RemoteControlScreen(MainDock);
            ChangeScreen(remoteScreen);
        }               

        public void ChangeScreen(UserControl screen)
        {
            var lastElement = MainDock.Children[MainDock.Children.Count - 1];
            MainDock.Children.Remove(lastElement);
            MainDock.Children.Add(screen);
        }
    }
}
