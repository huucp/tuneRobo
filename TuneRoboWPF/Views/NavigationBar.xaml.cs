using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.Views;

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
            DataContext = new NavigationBarViewModel();            
            ViewModel = (NavigationBarViewModel)DataContext;
        }

        public NavigationBarViewModel ViewModel = new NavigationBarViewModel();

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Windows.LoginWindow();
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == true)
            {                
                OnLoginSuccessfully(null);
            }
        }

        private void AccountMenu_Click(object sender, RoutedEventArgs e)
        {           
            var profileScreen = new UserDetailScreen();
            StaticMainWindow.Window.ChangeScreen(profileScreen);
        }

        private void SignOutMenu_Click(object sender, RoutedEventArgs e)
        {
            var signoutRequest = new SignoutStoreRequest();
            signoutRequest.ProcessSuccessfully += (reply) => OnLogoutSuccessfully(null);
            signoutRequest.ProcessError += (reply, msg) => Debug.Fail(reply.type.ToString(),msg);
            GlobalVariables.StoreWorker.AddRequest(signoutRequest);
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

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var searchScreen = new SearchResultScreen();
                searchScreen.SetQuery(SearchTextBox.Text);
                StaticMainWindow.Window.ChangeScreen(searchScreen);
                SearchTextBox.Text = string.Empty;
            }
            if (e.Key == Key.Escape)
            {
                SearchTextBox.Text = string.Empty;
            }
        }

    }
}