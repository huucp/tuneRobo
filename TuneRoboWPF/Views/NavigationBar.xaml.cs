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
        public delegate void LoginSuccessfullyEventHandler(object sender);

        public event LoginSuccessfullyEventHandler LoginProcessSuccessfully;

        private void OnLoginSuccessfully(object sender)
        {
            LoginSuccessfullyEventHandler handler = LoginProcessSuccessfully;
            if (handler != null) handler(sender);
        }

        public delegate void LogoutSuccessfullyEventHandler(object sender);

        public event LogoutSuccessfullyEventHandler LogoutSuccessfully;

        private void OnLogoutSuccessfully(object sender)
        {
            LogoutSuccessfullyEventHandler handler = LogoutSuccessfully;
            if (handler != null) handler(sender);
        }



        public static readonly RoutedEvent StoreButtonClickEvent = EventManager.RegisterRoutedEvent("StoreButtonClick", RoutingStrategy.Bubble,
                                                                                    typeof(RoutedEventHandler),
                                                                                    typeof(NavigationBar));
        public event RoutedEventHandler StoreButtonClick
        {
            add { AddHandler(StoreButtonClickEvent, value); }
            remove { RemoveHandler(StoreButtonClickEvent, value); }
        }

        void OnStoreButtonClickEvent()
        {
            var newEventArgs = new RoutedEventArgs(StoreButtonClickEvent);
            RaiseEvent(newEventArgs);
        }

        public static readonly RoutedEvent RemoteButtonClickEvent = EventManager.RegisterRoutedEvent("RemoteButtonClick", RoutingStrategy.Bubble,
                                                                                    typeof(RoutedEventHandler),
                                                                                    typeof(NavigationBar));
        public event RoutedEventHandler RemoteButtonClick
        {
            add { AddHandler(RemoteButtonClickEvent, value); }
            remove { RemoveHandler(RemoteButtonClickEvent, value); }
        }

        void OnRemoteButtonClickEvent()
        {
            var newEventArgs = new RoutedEventArgs(RemoteButtonClickEvent);
            RaiseEvent(newEventArgs);
        }

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
            var loginWindow = new Windows.LoginWindow();
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == true)
            {
                UserMenu.Visibility = Visibility.Visible;
                SignInButton.Visibility = Visibility.Hidden;
                viewModel.Username = GlobalVariables.CurrentUser;
                OnLoginSuccessfully(null);
            }
        }

        private void AccountMenu_Click(object sender, RoutedEventArgs e)
        {
            var getUserInfoRequest = new GetUserInfoStoreRequest();
            getUserInfoRequest.ProcessSuccessfully += (reply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                                                   {
                                                       MessageBox.Show(reply.profile.email, reply.profile.display_name);
                                                   });
            };
            getUserInfoRequest.ProcessError += (reply, msg) =>
            {

            };
            GlobalVariables.StoreWorker.AddJob(getUserInfoRequest);
        }

        private void SignOutMenu_Click(object sender, RoutedEventArgs e)
        {
            var signoutRequest = new SignoutStoreRequest();
            signoutRequest.ProcessSuccessfully += (reply) =>
                                                      {
                                                          Console.WriteLine("signout successfully");
                                                          OnLogoutSuccessfully(null);
                                                      };
            signoutRequest.ProcessError += (reply, msg) =>
                                               {
                                                   Console.WriteLine("signout failed");
                                               };
            GlobalVariables.StoreWorker.AddJob(signoutRequest);
        }

        private void StoreButton_Click(object sender, RoutedEventArgs e)
        {
            OnStoreButtonClickEvent();
        }
        private void RemoteButton_Click(object sender, RoutedEventArgs e)
        {
            //RemoteButton.Style = (Style)FindResource("ButtonFlatStyle");
            OnRemoteButtonClickEvent();
        }

    }
}