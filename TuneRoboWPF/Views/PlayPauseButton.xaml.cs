using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.Models;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for PlayPauseButton.xaml
    /// </summary>
    public partial class PlayPauseButton : UserControl
    {
        public delegate void UpdateParentControlEventHandler(object sender);

        public event UpdateParentControlEventHandler UpdateParentControl;

        private void OnUpdateParentControl(object sender)
        {
            UpdateParentControlEventHandler handler = UpdateParentControl;
            if (handler != null) handler(sender);
        }

        public PlayPauseButtonViewModel ViewModel;

        public PlayPauseButton()
        {
            InitializeComponent();
            var viewModel = new PlayPauseButtonViewModel();
            DataContext = viewModel;
            ViewModel = (PlayPauseButtonViewModel)(DataContext);
        }

        // Button state - Music state
        // Play         - Pause
        // Pause        - Play
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (ViewModel.StateButton)
            {
                case PlayPauseButtonModel.ButtonState.InActive:
                    break;
                case PlayPauseButtonModel.ButtonState.Play:
                    PlayRequest();
                    Cursor = Cursors.Wait;
                    break;
                case PlayPauseButtonModel.ButtonState.Pause:
                    PauseRequest();
                    Cursor = Cursors.Wait;
                    break;
            }
        }

        private void PlayRequest()
        {
            var playRequest = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay,-1,GlobalVariables.CurrentListMotion[1].MotionID);
            playRequest.ProcessSuccessfully += (data) =>
            {
                ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Pause;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                    OnUpdateParentControl(null);
                });
            };
            playRequest.ProcessError += (errorCode, msg) =>
                                            {
                                                Dispatcher.BeginInvoke((Action)delegate
                                                {
                                                    Cursor = Cursors.Arrow;
                                                });
                                                Console.WriteLine(msg);
                                            };
            GlobalVariables.RobotWorker.AddJob(playRequest);
        }   
        private void PauseRequest()
        {
            var pauseRequest = new RemoteRequest(RobotPacket.PacketID.Pause);
            pauseRequest.ProcessSuccessfully += (data) =>
            {
                ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                    OnUpdateParentControl(null);
                });
            };
            pauseRequest.ProcessError += (errorCode, msg) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                });
                Console.WriteLine(msg);
            };
            GlobalVariables.RobotWorker.AddJob(pauseRequest);
        }
    }
}
