using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (!GlobalFunction.IsAnImage(ViewModel.Avatar))
            {
                var title = (string)TryFindResource("AvatarNotImageText");
                WPFMessageBox.Show(this, "", title, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
            return true;
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
                switch (reply.type)
                {
                    case (int)SignupReply.Type.EMAIL_ERROR:
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
                    case (int)SignupReply.Type.NAME_ERROR:
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
