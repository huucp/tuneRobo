using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.Models;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for RemoteControlScreen.xaml
    /// </summary>
    public partial class RemoteControlScreen : UserControl
    {
        public RemoteControlScreen()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
            var remoteViewModel = new RemoteControlScreenViewModel();
            DataContext = remoteViewModel;
            viewModel = ((RemoteControlScreenViewModel)(DataContext));
            PlayPauseButton.UpdateParentControl += PlayPauseButton_UpdateParentControl;
        }

        private void PlayPauseButton_UpdateParentControl(object sender)
        {
            UpdateRemoteControl();
        }

        private void UpdateRemoteControl()
        {
            UpdateMusicState();
            UpdateMotionPlay();
        }

        private void UpdateMusicState()
        {
            switch (GlobalVariables.CurrentRobotState.CurrentMusicState)
            {
                case RobotState.MusicState.MusicPlaying:
                    PlayPauseButton.ViewModel.State = PlayPauseButtonModel.ButtonState.Pause;
                    break;
                case RobotState.MusicState.MusicPaused:
                    PlayPauseButton.ViewModel.State = PlayPauseButtonModel.ButtonState.Play;
                    break;
                case RobotState.MusicState.MusicIdled:
                    PlayPauseButton.ViewModel.State = PlayPauseButtonModel.ButtonState.InActive;
                    SetControlButtonState(false);
                    break;
            }
        }

        private void UpdateMotionPlay()
        {
            RemoteListBox.SelectedIndex = GlobalVariables.CurrentRobotState.CurrentMotionIndex;
        }

        private void SetControlButtonState(bool state)
        {
            NextButton.ViewModel.Active = state;
            PreviousButton.ViewModel.Active = state;
            VolumeButton.ViewModel.Active = state;           
        }

        private RemoteControlScreenViewModel viewModel;

        private void RemoteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel.LastSelectedMotionItem != null)
            {
                viewModel.LastSelectedMotionItem.ViewModel.RectangleFillColor = "Yellow";
            }
            viewModel.SelectedMotion.ViewModel.RectangleFillColor = "Red";
        }

        private void UnconnectedTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Wait;
            ConnectMrobo();


        }
        private void ConnectMrobo()
        {
            var helloRequest = new RemoteRequest(RobotPacket.PacketID.Hello);
            helloRequest.ProcessSuccessfully += (data) =>
            {
                MessageBox.Show("Connect successfully!", "Connect to mRobo via Wireless connection", MessageBoxButton.OK);
                Dispatcher.BeginInvoke((Action)delegate
                                                   {
                                                       UnconnectedTextBox.Visibility = Visibility.Hidden;
                                                       PlayPauseButton.ViewModel.State = PlayPauseButtonModel.ButtonState.Play;
                                                       SetControlButtonState(true);
                                                       GetListMotion();
                                                   });
            };
            helloRequest.ProcessError += (errorCode, msg) => MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            GlobalVariables.RobotWorker.AddJob(helloRequest);
        }

        private void GetListMotion()
        {
            var listAllMotionRequest = new ListAllMotionRequest();
            listAllMotionRequest.ProcessSuccessfully += (s) => Dispatcher.BeginInvoke((Action)delegate
            {
                var listMotion = new ObservableCollection<MotionTitleItem>();
                foreach (MotionInfo info in s)
                {
                    var motionTitleItem = new MotionTitleItem();
                    motionTitleItem.ViewModel.Title = info.Title;
                    listMotion.Add(motionTitleItem);
                }
                viewModel.RemoteListItem = listMotion;
                Dispatcher.BeginInvoke((Action)delegate
                                                    {
                                                        Cursor = Cursors.Arrow;                                                        
                                                    });
            });
            listAllMotionRequest.ProcessError += (e, msg) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                });
                Console.WriteLine(msg);
            };
            GlobalVariables.RobotWorker.AddJob(listAllMotionRequest);
        }
    }
}