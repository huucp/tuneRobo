using System;
using System.Collections.Generic;
using System.Linq;
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

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private bool canUpdatePassword = false;
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OldPassword.Password))
            {
                MessageBox.Show("Your password cannot be empty.");
                return;                
            }
            if (string.IsNullOrWhiteSpace(NewPassword.Password))
            {
                MessageBox.Show("Your new password cannot be empty.");
                return;
            }
            ChangePass();
        }

        private void ChangePass()
        {
            var changePassRequest = new ChangePasswordStoreRequest(OldPassword.Password, NewPassword.Password);
            changePassRequest.ProcessSuccessfully += (reply) => 
                Dispatcher.BeginInvoke((Action) delegate
                {
                    MessageBox.Show("Update password successfully.");
                    Close();
                });
            changePassRequest.ProcessError += (reply, msg) =>
            {
                if (reply.type == (decimal) ChangePassReply.Type.OLD_PASS_ERROR)
                {
                    Dispatcher.BeginInvoke((Action) (() => MessageBox.Show("Your password is wrong")));
                }
            };
            GlobalVariables.StoreWorker.AddRequest(changePassRequest);
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}
