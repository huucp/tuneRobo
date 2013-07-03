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
using MessageBoxUtils;
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
                var title = (string)TryFindResource("OldPasswordEmptyText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword.Password))
            {
                var title = (string)TryFindResource("NewPasswordEmptyText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            Cursor = Cursors.Wait;
            ChangePass();
        }

        private void ChangePass()
        {
            var changePassRequest = new ChangePasswordStoreRequest(OldPassword.Password, NewPassword.Password);
            changePassRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var title = (string)TryFindResource("UpdatePasswordSuccffullyText");
                    WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    Cursor = Cursors.Arrow;
                    Close();
                });
            changePassRequest.ProcessError += (reply, msg) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    string titleError, msgError;
                    switch (reply.type)
                    {
                        case (int)ChangePassReply.Type.OLD_PASS_ERROR:
                            titleError = (string)TryFindResource("OldPasswordErrorText");
                            msgError = (string)TryFindResource("CheckOldPasswordErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            break;
                        case (int)ChangePassReply.Type.NEW_PASS_ERROR:

                            titleError = (string)TryFindResource("NewPasswordErrorText");
                            msgError = (string)TryFindResource("CheckNewPasswordErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

                            break;
                        default:
                            titleError = (string)TryFindResource("ChangePasswordDefaultText");
                            msgError = (string)TryFindResource("CheckDefaultErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            break;
                    }
                    Cursor = Cursors.Arrow;
                });

            GlobalVariables.StoreWorker.AddRequest(changePassRequest);
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}
