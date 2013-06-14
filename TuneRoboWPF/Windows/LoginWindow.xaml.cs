using System;
using System.Diagnostics;
using System.Windows;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using user;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for SigninWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            this.InitializeComponent();

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            var signinRequest = new SigninStoreRequest(UsernameTextBox.Text, PasswordBox.Password,
                                                       SigninRequest.Type.USER);
            signinRequest.ProcessSuccessfully += (s) =>
                                                     {
                                                         GlobalVariables.CurrentUser = new UserProfile(s.signin);
                                                         GlobalVariables.UserOnline = true;
                                                         Dispatcher.BeginInvoke((Action)delegate
                                                                                             {
                                                                                                 DialogResult = true;
                                                                                             });
                                                     };
            signinRequest.ProcessError += (s, msg) =>
                                              {
                                                  Console.WriteLine("Login failed: " + msg);
                                              };
            GlobalVariables.StoreWorker.AddRequest(signinRequest);
        }

    }
}