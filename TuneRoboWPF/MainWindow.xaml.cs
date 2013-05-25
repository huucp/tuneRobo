using System;
using System.Windows;
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
            GetNotificationFromStore();
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
            GlobalVariables.StoreWorker.AddJob(notificationRequest);
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
            var remoteScreen = new TestRemoteScreen(MainDock);
            MainDock.Children.Add(remoteScreen);
        }

   
        private void navigationBar_StoreButtonClick(object sender, RoutedEventArgs e)
        {
            var lastElement = MainDock.Children[MainDock.Children.Count - 1];
            MainDock.Children.Remove(lastElement);
            var testStoreScreen = new TempStoreScreen(MainDock);
            MainDock.Children.Add(testStoreScreen);
        }

        private void navigationBar_RemoteButtonClick(object sender, RoutedEventArgs e)
        {
            var lastElement = MainDock.Children[MainDock.Children.Count - 1];
            MainDock.Children.Remove(lastElement);
            var remoteScreen = new RemoteControlScreen(MainDock);
            MainDock.Children.Add(remoteScreen);
        }               
    }
}
