using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MessageBoxUtils;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using user;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        private SignupWindowViewModel ViewModel = new SignupWindowViewModel();
        public SignupWindow()
        {
            InitializeComponent();

            DataContext = new SignupWindowViewModel();
            ViewModel = (SignupWindowViewModel)DataContext;
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private bool ValidateData()
        {
            if (!string.IsNullOrWhiteSpace(ViewModel.Avatar) && !GlobalFunction.IsAnImage(ViewModel.Avatar))
            {
                var title = (string)TryFindResource("AvatarNotImageText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
            if (ViewModel.Email.Length==0)
            {
                var title = (string)TryFindResource("EmailEmptyErrorText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
            if (ViewModel.Username.Length == 0)
            {
                var title = (string)TryFindResource("UsernameEmptyErrorText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
            if (ViewModel.Username.Length > 128)
            {
                var title = (string)TryFindResource("UsernameLengthErrorText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
            return ValidateEmail();
        }

        private bool ValidateEmail()
        {
            var emailRegex =
                new Regex(
                    @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            var match = emailRegex.Match(ViewModel.Email);
            if (match.Success)
            {
                return true;
            }
            var titleError = (string)TryFindResource("CreateAccountEmailErrorText");
            var msgError = (string)TryFindResource("CheckEmailErrorText");
            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                               MessageBoxResult.OK);
            return false;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
            {
                return;
            }
            Cursor = Cursors.Wait;
            ViewModel.EnableUI = false;
            var createRequest = new SignupStoreRequest(ViewModel.Email, ViewModel.Username, ViewModel.Avatar);
            createRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var title = (string)TryFindResource("CreateAccountSuccessfullyText");
                    var msg = (string)TryFindResource("CheckEmailForPasswordText");
                    WPFMessageBox.Show(this, msg, title, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    Close();
                    Cursor = Cursors.Arrow;
                });
            createRequest.ProcessError += (reply, msg) =>
            {
                if (reply == null)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        var titleError = (string)TryFindResource("CreateAccountDefaultErrorText");
                        var msgError = (string)TryFindResource("CheckDefaultErrorText");
                        WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                           MessageBoxResult.OK);
                        Cursor = Cursors.Arrow;
                        ViewModel.EnableUI = true;
                    });
                    Debug.Fail("Reply is null");
                    return;
                }    
                switch (reply.type)
                {
                    case (int)SignupReply.Type.INVALID_EMAIL:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var titleError = (string)TryFindResource("CreateAccountEmailErrorText");
                            var msgError = (string)TryFindResource("CheckEmailErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                               MessageBoxResult.OK);
                            Cursor = Cursors.Arrow;
                            ViewModel.EnableUI = true;
                        });
                        break;
                    case (int)SignupReply.Type.EMAIL_EXISTED:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var titleError = (string)TryFindResource("CreateAccountEmailExistedText");
                            var msgError = (string)TryFindResource("CheckEmailExistedText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                               MessageBoxResult.OK);
                            Cursor = Cursors.Arrow;
                            ViewModel.EnableUI = true;
                        });
                        break;

                    case (int)SignupReply.Type.INVALID_NAME:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var titleError = (string)TryFindResource("CreateAccountNameErrorText");
                            var msgError = (string)TryFindResource("CheckNameErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                               MessageBoxResult.OK);
                            Cursor = Cursors.Arrow;
                            ViewModel.EnableUI = true;
                        });
                        break;
                    default:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var titleError = (string)TryFindResource("CreateAccountDefaultErrorText");
                            var msgError = (string)TryFindResource("CheckDefaultErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                               MessageBoxResult.OK);
                            Cursor = Cursors.Arrow;
                            ViewModel.EnableUI = true;
                        });
                        Debug.Fail(reply.type.ToString(), msg);
                        break;
                }
            };
            GlobalVariables.StoreWorker.AddRequest(createRequest);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }


}
