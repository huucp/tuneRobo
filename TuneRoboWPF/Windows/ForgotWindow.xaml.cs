﻿using System;
using System.Diagnostics;
using System.Windows;
using MessageBoxUtils;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using user;
using System.Windows.Input;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for ForgotWindow.xaml
    /// </summary>
    public partial class ForgotWindow : Window
    {
        private ForgotWindowViewModel ViewModel = new ForgotWindowViewModel();
        public ForgotWindow()
        {
            InitializeComponent();

            DataContext = new ForgotWindowViewModel();
            ViewModel = (ForgotWindowViewModel) DataContext;
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
       

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            ViewModel.EnableUI = false;
            var forgotRequest = new ForgotPasswordStoreRequest(ViewModel.Email);
            forgotRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var msg = (string)TryFindResource("ResetPasswordSuccessfullyText");
                    WPFMessageBox.Show(this, "", msg, MessageBoxButton.OK, MessageBoxImage.Information,
                                       MessageBoxResult.OK);
                    Cursor = Cursors.Arrow;
                    Close();
                });
            forgotRequest.ProcessError += (reply, msg) =>
            {
                if (reply == null)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        var titleError = (string)TryFindResource("ResetPasswordDefaultErrorText");
                        var msgError = (string)TryFindResource("CheckDefaultErrorText");
                        WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        Cursor = Cursors.Arrow;
                        ViewModel.EnableUI = true;
                    });
                    Debug.Fail("Reply is null");
                    return;
                }                
                switch (reply.type)
                {
                    case (int)ForgotPassReply.Type.EMAIL_ERROR:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var titleError = (string)TryFindResource("ResetPasswordEmailErrorText");
                            var msgError = (string)TryFindResource("ResetPasswordCheckEmailErrorText");
                            WPFMessageBox.Show(this,msgError,titleError,MessageBoxButton.OK,MessageBoxImage.Error,MessageBoxResult.OK);
                            Cursor = Cursors.Arrow;
                            ViewModel.EnableUI = true;
                        });
                        break;
                    default:
                        Debug.Fail("unknown error when reset password",Enum.GetName(typeof(comm.Reply),reply.type));
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var titleError = (string)TryFindResource("ResetPasswordDefaultErrorText");
                            var msgError = (string) TryFindResource("CheckDefaultErrorText");
                            WPFMessageBox.Show(this, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            Cursor = Cursors.Arrow;
                            ViewModel.EnableUI = true;
                        });
                        break;
                }
            };
            GlobalVariables.StoreWorker.AddRequest(forgotRequest);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
