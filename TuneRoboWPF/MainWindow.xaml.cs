using System;
using System.Diagnostics;
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
            navigationBar.LogoutSuccessfully += navigationBar_LogoutSuccessfully;            
        }

        private void navigationBar_LogoutSuccessfully(object sender)
        {
            GlobalVariables.CurrentUser = null;
            GlobalVariables.UserOnline = false;

            Dispatcher.BeginInvoke((Action)delegate
            {
                navigationBar.UserMenu.Visibility = Visibility.Hidden;
                navigationBar.SignInButton.Visibility = Visibility.Visible;
                //var storeScreen = new StoreScreen();
                var storeScreen = new NewStoreScreen();
                storeScreen.SetInfo(true);
                ChangeScreen(storeScreen);
            });
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
            navigationBar.ViewModel.Username = GlobalVariables.CurrentUser.DisplayName;

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
                Console.WriteLine("GetNotificationFromStore failed: {0} - {1}", reply.type.ToString(), msg);
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

            //if (GlobalVariables.ServerConnection.ConfigAndConnectSocket() == 0)
            //{
            //    //MessageBox.Show("Cannot connect to server!", "Connection error", MessageBoxButton.OK,
            //    //    MessageBoxImage.Error);
            //    Debug.Fail("Cannot connect to server");
            //}

            var firstScreen = new RemoteControlScreen();
            MainDock.Children.Add(firstScreen);
        }


        private void navigationBar_StoreButtonClick(object sender, RoutedEventArgs e)
        {
            var testStoreScreen = new NewStoreScreen();
            testStoreScreen.SetInfo(true);
            ChangeScreen(testStoreScreen);
        }

        private void navigationBar_RemoteButtonClick(object sender, RoutedEventArgs e)
        {
            var remoteScreen = new RemoteControlScreen();
            ChangeScreen(remoteScreen);
        }

        private void OnRemoteCategory()
        {
            navigationBar.ViewModel.OnStore = false;
            navigationBar.UserMenu.Visibility = Visibility.Hidden;
            navigationBar.SignInButton.Visibility = Visibility.Hidden;
        }
        private void OnStoreCategory()
        {
            navigationBar.ViewModel.OnStore = true;
            if (GlobalVariables.UserOnline)
            {
                navigationBar.UserMenu.Visibility = Visibility.Visible;
                navigationBar.SignInButton.Visibility = Visibility.Hidden;
            }
            else
            {
                navigationBar.UserMenu.Visibility = Visibility.Hidden;
                navigationBar.SignInButton.Visibility = Visibility.Visible;
            }
        }

        public void ChangeScreen(UserControl screen)
        {
            //foreach (var element in MainDock.Children)
            //{
            //    if (element.GetType().Name == "LoadingScreen")
            //    {
            //        MainDock.Children.Remove((UIElement) element);
            //    }
            //}

            if (screen.GetType().Name == "RemoteControlScreen")
            {
                OnRemoteCategory();
            }
            else
            {
                OnStoreCategory();
            }
            var lastElement = MainDock.Children[MainDock.Children.Count - 1];
            //if (lastElement.GetType() == screen.GetType())
            //{
            //    return;                
            //}
            MainDock.Children.Remove(lastElement);
            MainDock.Children.Add(screen);

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

        }

        private UIElement MainContentScreen;
        public void ShowLoadingScreen()
        {
            MainContentScreen = MainDock.Children[MainDock.Children.Count - 1];
            MainContentScreen.Visibility = Visibility.Collapsed;
            var loadingScreen = new LoadingScreen();
            MainDock.Children.Add(loadingScreen);
        }

        public void ShowErrorScreen()
        {
            var errorScreen = new ErrorScreen();
            ChangeScreen(errorScreen);
        }

        public void ShowNoneScreen()
        {
            var noneScreen = new LoadingScreen();
            ChangeScreen(noneScreen);
        }

        public void ShowContentScreen()
        {
            var lastElement = MainDock.Children[MainDock.Children.Count - 1];
            MainDock.Children.Remove(lastElement);
            MainContentScreen.Visibility = Visibility.Visible;
        }
    }
}
