using System;
using System.Windows;
using System.Windows.Input;
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
        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Signup_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
            var signupWindow = new SignupWindow();
            signupWindow.ShowDialog(StaticMainWindow.Window);
        }

        private void Forgot_Click(object sender, MouseButtonEventArgs e)
        {  
            Close();
            var forgotWindow = new ForgotWindow();
            forgotWindow.ShowDialog(StaticMainWindow.Window);
        }        
    }
}