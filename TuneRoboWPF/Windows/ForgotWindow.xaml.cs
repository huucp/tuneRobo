using System;
using System.Windows;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using user;

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
            var forgotRequest = new ForgotPasswordStoreRequest(ViewModel.Email);
            forgotRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var msg = (string)FindResource("ResetPasswordSuccessfullyText");
                    MessageBox.Show(msg);
                    Close();
                });
            forgotRequest.ProcessError += (reply, msg) =>
            {
                switch (reply.type)
                {
                    case (int)ForgotPassReply.Type.EMAIL_ERROR:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var msgError = (string)FindResource("ResetPasswordEmailErrorText");
                            MessageBox.Show(msgError);
                        });
                        break;
                    default:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var msgError = (string)FindResource("ResetPasswordDefaultErrorText");
                            MessageBox.Show(msgError);
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
