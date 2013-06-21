using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.Models;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.StoreService.SimpleRequest;
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
            viewModel = (RemoteControlScreenViewModel)DataContext;

            PlayPauseButtons.UpdateParentControl += PlayPauseButtonsUpdateParentControl;
            NextButton.UpdateParentControl += NextButton_UpdateParentControl;
            PreviousButton.UpdateParentControl += PreviousButton_UpdateParentControl;
            TransformButton.UpdateParentControl += TransformButton_UpdateParentControl;
            if (GlobalVariables.RoboOnline)
            {
                UnconnectedTextBox.Visibility = Visibility.Hidden;
                GetListMotion();
            }
        }

        private void PreviousButton_UpdateParentControl(object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (RemoteListBox.SelectedIndex == 0)
                {
                    RemoteListBox.SelectedIndex =
                        GlobalVariables.CurrentListMotion.Count - 1;
                    return;
                }
                RemoteListBox.SelectedIndex--;
            });
        }

        private void NextButton_UpdateParentControl(object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (RemoteListBox.SelectedIndex ==
                    GlobalVariables.CurrentListMotion.Count - 1)
                {
                    RemoteListBox.SelectedIndex = 0;
                    return;
                }
                RemoteListBox.SelectedIndex++;
            });
        }

        private void TransformButton_UpdateParentControl(object sender)
        {
            switch (TransformButton.ViewModel.State)
            {
                case RobotTransformButtonModel.ButtonState.Transform:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.InActive;
                    SetControlButtonState(false);
                    break;
                case RobotTransformButtonModel.ButtonState.Untransform:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                    SetControlButtonState(true);
                    break;
            }
        }

        private void PlayPauseButtonsUpdateParentControl(object sender)
        {
            UpdateRemoteControl();
        }

        private void UpdateRemoteControl()
        {
            //UpdateMusicState();
            UpdateMotionPlay();
        }

        private void UpdateMusicState()
        {
            switch (GlobalVariables.CurrentRobotState.MusicState)
            {
                case RobotState.MusicStates.MusicPlaying:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Pause;
                    break;
                case RobotState.MusicStates.MusicPaused:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                    break;
                case RobotState.MusicStates.MusicIdled:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.InActive;
                    SetControlButtonState(false);
                    break;
            }
        }

        private void UpdateMotionPlay()
        {
            RemoteListBox.SelectedIndex = GlobalVariables.CurrentRobotState.MotionIndex;

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
                    TransformButton.ViewModel.State = RobotTransformButtonModel.ButtonState.Transform;
                    GetListMotion();
                    Cursor = Cursors.Arrow;
                });
                GlobalVariables.RoboOnline = true;
            };
            helloRequest.ProcessError += (errorCode, msg) =>
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                });
            };

            GlobalVariables.RobotWorker.AddJob(helloRequest);
        }
        private List<MotionInfo> RobotListMotion = new List<MotionInfo>();

        private void GetListMotion()
        {
            var listAllMotionRequest = new ListAllMotionRobotRequest();
            if (viewModel.RemoteItemsList.Count > 0)
            {
                viewModel.RemoteItemsList.Clear();
                RobotListMotion.Clear();
            }
            listAllMotionRequest.ProcessSuccessfully += (listMotionInfo) => Dispatcher.BeginInvoke((Action)delegate
            {
                RobotListMotion.AddRange(listMotionInfo);
                foreach (MotionInfo info in listMotionInfo)
                {
                    if (info.MType != MotionInfo.MotionType.Dance) continue;
                    var motionTitleItem = new MotionTitleItem();
                    motionTitleItem.MotionID = info.MotionID;
                    motionTitleItem.ViewModel.Title = info.Title;
                    motionTitleItem.ViewModel.Duration = info.Duration;
                    motionTitleItem.DeleteMotion += MotionTitleItem_DeleteMotion;
                    viewModel.RemoteItemsList.Add(motionTitleItem);
                }

                GlobalFunction.UpdateCurrentListMotion(listMotionInfo);
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

        private void MotionTitleItem_DeleteMotion(ulong motionID)
        {
            var delRequest = new RemoteRequest(RobotPacket.PacketID.DeleteMotion, -1, motionID);
            delRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    GetListMotion();
                    RemoteListBox.SelectedIndex = 0;
                });

            delRequest.ProcessError += (errorCode, msg) => Debug.Fail(errorCode.ToString(), msg);
            GlobalVariables.RobotWorker.AddJob(delRequest);
        }
        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadLibrary();
            }
        }


        private void LoadLibrary()
        {
            var r = new Random();
            int index = 0;
            foreach (var file in Directory.GetFiles(GlobalFunction.GetSavedDir(), "*.mrb"))
            {
                var motionInfo = new MotionInfo(file);
                var motionItem = new MotionFullInfoItem();
                motionItem.SetMotionInfo(motionInfo);
                motionItem.ViewModel.HitTestVisible = false;
                motionItem.ViewModel.Index = ++index;
                motionItem.CopyMotion+=Library_CopyMotion;
                viewModel.LibraryItemsList.Add(motionItem);
                DownloadImage(motionInfo.MotionID, motionItem.ViewModel);
            }
        }

        private void Library_CopyMotion(object sender, RoutedEventArgs e)
        {
            GetListMotion();
        }

        private void DownloadImage(ulong id, MotionFullInfoItemViewModel model)
        {
            var motionInfoRequest = new GetMotionFullInfoStoreRequest(id);
            motionInfoRequest.ProcessSuccessfully += (data) =>
                                                         {
                                                             var imageDownload = new ImageDownload(data.motion_info.info.icon_url);
                                                             imageDownload.DownloadCompleted += (img) => Dispatcher.
                                                                                                             BeginInvoke((Action)delegate
                                                                                                                                     {
                                                                                                                                         model.CoverImage = img;
                                                                                                                                     });
                                                             GlobalVariables.ImageDownloadWorker.AddDownload(imageDownload);
                                                         };
            motionInfoRequest.ProcessError += (data, msg) => Debug.Fail(data.type.ToString(), msg);
            GlobalVariables.StoreWorker.ForceAddRequest(motionInfoRequest);
        }
    }
}