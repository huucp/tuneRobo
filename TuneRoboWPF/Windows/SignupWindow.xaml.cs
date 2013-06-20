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
            ViewModel = (SignupWindowViewModel) DataContext;
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var createRequest = new SignupStoreRequest(ViewModel.Email, ViewModel.Username,ViewModel.Avatar);
            createRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var msg = (string)FindResource("CreateAccountSuccessfullyText");
                    MessageBox.Show(msg);
                    Close();
                });
            createRequest.ProcessError += (reply, msg) =>
            {
                switch (reply.type)
                {
                    case (int)SignupReply.Type.EMAIL_ERROR:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var msgError = (string)FindResource("CreateAccountEmailErrorText");
                            MessageBox.Show(msgError);
                        });
                        break;
                    case (int)SignupReply.Type.NAME_ERROR:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var msgError = (string)FindResource("CreateAccountNameErrorText");
                            MessageBox.Show(msgError);
                        });
                        break;
                    default:
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var msgError = (string)FindResource("CreateAccountDefaultErrorText");
                            MessageBox.Show(msgError);
                        });
                        Debug.Fail(reply.type.ToString(),msg);
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
