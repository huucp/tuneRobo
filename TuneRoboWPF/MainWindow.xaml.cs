using System;
using System.Windows;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;

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
                    testStoreScreen.Visibility = Visibility.Visible;
                    remoteScreen.Visibility = Visibility.Collapsed;
                });
                
            };
            notificationRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("GetNotificationFromStore failed: " + msg);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    testStoreScreen.Visibility = Visibility.Visible;
                    remoteScreen.Visibility = Visibility.Collapsed;
                });
            };
            GlobalVariables.StoreWorker.AddJob(notificationRequest);

        }

        private void MainScreen_ContentRendered(object sender, System.EventArgs e)
        {
            if (GlobalVariables.ServerConnection.ConfigAndConnectSocket() == 0)
            {
                MessageBox.Show("Cannot connect to server!", "Connection error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
