using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using user;

namespace TuneRoboWPF
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
                                                         Console.WriteLine("Login successfully");
                                                         GlobalVariables.CurrentUser = s.signin.display_name;
                                                         GlobalVariables.CurrentUserID = s.signin.user_id;
                                                         Dispatcher.BeginInvoke((Action) delegate
                                                                                             {
                                                                                                 DialogResult = true;
                                                                                             });
                                                     };
            signinRequest.ProcessError += (s,msg) =>
                                              {
                                                  Console.WriteLine("Login failed: " + msg);
                                              };
            GlobalVariables.StoreWorker.AddJob(signinRequest);
        }        
	}
}