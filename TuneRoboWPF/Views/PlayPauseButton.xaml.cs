using System;
using System.Diagnostics;
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
            //var playRequest = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay,-1,GlobalVariables.CurrentListMotion[1].MotionID);
            var playRequest = new RemoteRequest(RobotPacket.PacketID.Play);
            playRequest.ProcessSuccessfully += (data) =>
            {
                ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Pause;
                Dispatcher.BeginInvoke((Action)GetState);
            };
            playRequest.ProcessError += (errorCode, msg) =>
                                            {
                                                Dispatcher.BeginInvoke((Action)delegate
                                                {
                                                    Cursor = Cursors.Arrow;
                                                });
                                                Debug.Fail(msg);
                                            };
            GlobalVariables.RobotWorker.AddJob(playRequest);
        }   
        private void PauseRequest()
        {
            var pauseRequest = new RemoteRequest(RobotPacket.PacketID.Pause);
            pauseRequest.ProcessSuccessfully += (data) =>
            {
                ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                Dispatcher.BeginInvoke((Action)GetState);
            };
            pauseRequest.ProcessError += (errorCode, msg) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                });
                Debug.Fail(msg);
            };
            GlobalVariables.RobotWorker.AddJob(pauseRequest);
        }

        private void GetState()
        {
            var stateRequest = new GetStateRequest();
            stateRequest.ProcessSuccessfully += data =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                    UpdateButtonState();
                    OnUpdateParentControl(null);
                });
                
            };
            stateRequest.ProcessError += (errorCode, msg) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                });
                Debug.Fail(msg);
            };
            GlobalVariables.RobotWorker.AddJob(stateRequest);
        }

        private void UpdateButtonState()
        {
            switch (GlobalVariables.CurrentRobotState.MusicState)
            {
                case RobotState.MusicStates.MusicPlaying:
                    ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Pause;
                    break;
                case RobotState.MusicStates.MusicPaused:
                    ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                    break;
                case RobotState.MusicStates.MusicIdled:
                    //ViewModel.StateButton = PlayPauseButtonModel.ButtonState.InActive;
                    break;
            }
        }
    }
}
