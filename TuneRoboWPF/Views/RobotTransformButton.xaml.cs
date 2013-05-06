using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.Models;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for RobotTransformButton.xaml
    /// </summary>
    public partial class RobotTransformButton : UserControl
    {
        public delegate void UpdateParentControlEventHandler(object sender);

        public event UpdateParentControlEventHandler UpdateParentControl;

        private void OnUpdateParentControl(object sender)
        {
            UpdateParentControlEventHandler handler = UpdateParentControl;
            if (handler != null) handler(sender);
        }
        public RobotTransformButton()
        {
            InitializeComponent();
            var viewModel = new RobotTransformButtonViewModel();
            DataContext = viewModel;
            ViewModel = DataContext as RobotTransformButtonViewModel;
        }

        public RobotTransformButtonViewModel ViewModel;

        // Button sate - Robot state
        // Transform   - Untransformed
        // Untransform - Transformed
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModel.State)
            {
                case RobotTransformButtonModel.ButtonState.InActive:
                    break;
                case RobotTransformButtonModel.ButtonState.Transform:
                    TransformRequest();
                    Cursor = Cursors.Wait;
                    break;
                case RobotTransformButtonModel.ButtonState.Untransform:
                    UntransformRequest();
                    Cursor = Cursors.Wait;
                    break;
            }
        }

        private void TransformRequest()
        {
            var transformRequest = new RemoteRequest(RobotPacket.PacketID.OpenTransform);
            transformRequest.ProcessSuccessfully += (data) =>
                                                        {
                                                            ViewModel.State =
                                                                RobotTransformButtonModel.ButtonState.Untransform;
                                                            Dispatcher.BeginInvoke((Action)delegate
                                                            {
                                                                Cursor = Cursors.Arrow;
                                                                OnUpdateParentControl(null);
                                                            });
                                                        };
            transformRequest.ProcessError += (errorCode, msg) =>
                                                 {
                                                     Dispatcher.BeginInvoke((Action)delegate
                                                     {
                                                         Cursor = Cursors.Arrow;
                                                     });
                                                     Console.WriteLine(msg);
                                                 };
            GlobalVariables.RobotWorker.AddJob(transformRequest);
        }

        private void UntransformRequest()
        {
            var untransformRequest = new RemoteRequest(RobotPacket.PacketID.CloseTransform);
            untransformRequest.ProcessSuccessfully += (data) =>
                                                          {
                                                              ViewModel.State =
                                                                  RobotTransformButtonModel.ButtonState.Transform;
                                                              Dispatcher.BeginInvoke((Action)delegate
                                                              {
                                                                  Cursor = Cursors.Arrow;
                                                                  OnUpdateParentControl(null);
                                                              });
                                                          };
            untransformRequest.ProcessError += (errorCode, msg) =>
                                                   {
                                                       Dispatcher.BeginInvoke((Action)delegate
                                                       {
                                                           Cursor = Cursors.Arrow;
                                                       });
                                                       Console.WriteLine(msg);
                                                   };
            GlobalVariables.RobotWorker.AddJob(untransformRequest);
        }
    }
}
