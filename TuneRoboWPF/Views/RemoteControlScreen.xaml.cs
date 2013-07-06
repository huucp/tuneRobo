﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MessageBoxUtils;
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

        }

        private void PreviousButton_UpdateParentControl(object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                //if (RemoteListBox.SelectedIndex == 0)
                //{
                //    RemoteListBox.SelectedIndex = GlobalVariables.CurrentListMotion.Count - 1;
                //    return;
                //}
                //RemoteListBox.SelectedIndex--;
                if ((string)sender == "MustReconnect")
                {
                    var titleError = (string)TryFindResource("RobotConnectionLostText");
                    var msgError = (string)TryFindResource("WantReconnectRobotText");
                    var result = WPFMessageBox.Show(StaticMainWindow.Window, msgError,
                                             titleError, MessageBoxButton.YesNo,
                                             MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        ConnectMrobo();
                    }
                    else
                    {
                        GlobalVariables.RoboOnline = false;
                        UnconnectedTextBox.Visibility = Visibility.Visible;
                    }                   
                    return;
                }
                UpdateRemoteControl();
            });
        }

        private void NextButton_UpdateParentControl(object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                //if (RemoteListBox.SelectedIndex == GlobalVariables.CurrentListMotion.Count - 1)
                //{
                //    RemoteListBox.SelectedIndex = 0;
                //    return;
                //}
                //RemoteListBox.SelectedIndex++;
                if ((string)sender == "MustReconnect")
                {
                    var titleError = (string)TryFindResource("RobotConnectionLostText");
                    var msgError = (string)TryFindResource("WantReconnectRobotText");
                    var result = WPFMessageBox.Show(StaticMainWindow.Window, msgError,
                                             titleError, MessageBoxButton.YesNo,
                                             MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        ConnectMrobo();
                    }
                    else
                    {
                        GlobalVariables.RoboOnline = false;
                    }
                    return;
                }
                UpdateRemoteControl();
            });
        }

        private void TransformButton_UpdateParentControl(object sender)
        {
            if ((string)sender == "MustReconnect")
            {
                var titleError = (string)TryFindResource("RobotConnectionLostText");
                var msgError = (string)TryFindResource("WantReconnectRobotText");
                var result = WPFMessageBox.Show(StaticMainWindow.Window, msgError,
                                         titleError, MessageBoxButton.YesNo,
                                         MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    ConnectMrobo();
                }
                else
                {
                    GlobalVariables.RoboOnline = false;
                }
                return;
            }
            GetState();
            //switch (TransformButton.ViewModel.State)
            //{
            //    case RobotTransformButtonModel.ButtonState.Transform:
            //        PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.InActive;
            //        SetControlButtonState(false);
            //        break;
            //    case RobotTransformButtonModel.ButtonState.Untransform:
            //        PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
            //        SetControlButtonState(true);
            //        UpdateRemoteControl();
            //        break;
            //}
        }

        private void PlayPauseButtonsUpdateParentControl(object sender)
        {
            if ((string)sender == "MustReconnect")
            {
                var titleError = (string)TryFindResource("RobotConnectionLostText");
                var msgError = (string)TryFindResource("WantReconnectRobotText");
                var result = WPFMessageBox.Show(StaticMainWindow.Window, msgError,
                                         titleError, MessageBoxButton.YesNo,
                                         MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    ConnectMrobo();
                }
                else
                {
                    GlobalVariables.RoboOnline = false;
                }
                return;
            }
            UpdateRemoteControl();
        }

        private void UpdateRemoteControl()
        {
            UpdateRemoteBackground();
            UpdateMusicState();
            UpdateMotionPlay();
            viewModel.Volume = GlobalVariables.CurrentRobotState.Volume;
            UpdateControlButtonState();
            //UpdateTransformButton();
        }
        
        private void UpdateRemoteBackground()
        {
            RobotState state = GlobalVariables.CurrentRobotState;
            switch (state.TransformState)
            {
                case RobotState.TransformStates.Closed:
                case RobotState.TransformStates.Closing:
                    viewModel.RobotBackgroundImageSource = (BitmapImage) TryFindResource("UntransformRobotImage");
                    break;
                case RobotState.TransformStates.Openning:
                case RobotState.TransformStates.Opened:
                    viewModel.RobotBackgroundImageSource = (BitmapImage)TryFindResource("TransformedRobotImage");
                    break;
            }
        }

        private void UpdateTransformButton()
        {
            switch (TransformButton.ViewModel.State)
            {
                case RobotTransformButtonModel.ButtonState.Transform:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.InActive;
                    break;
                case RobotTransformButtonModel.ButtonState.Untransform:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                    break;
            }
        }

        private void UpdateControlButtonState()
        {
            RobotState state = GlobalVariables.CurrentRobotState;
            SetControlButtonState(true);
            switch (state.TransformState)
            {
                case RobotState.TransformStates.Closed:
                case RobotState.TransformStates.Closing:
                    TransformButton.ViewModel.State = RobotTransformButtonModel.ButtonState.Transform;                    
                    break;
                case RobotState.TransformStates.Openning:
                case RobotState.TransformStates.Opened:
                    TransformButton.ViewModel.State = RobotTransformButtonModel.ButtonState.Untransform;
                    break;
            }
        }

        private void UpdateMusicState()
        {
            var state = GlobalVariables.CurrentRobotState;
            if (state.TransformState == RobotState.TransformStates.Openning ||
                state.TransformState == RobotState.TransformStates.Closing)
            {
                PlayPauseButtons.ViewModel.StateButton=PlayPauseButtonModel.ButtonState.Play;
            }
            switch (state.MusicState)
            {
                case RobotState.MusicStates.MusicPlaying:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Pause;
                    break;
                case RobotState.MusicStates.MusicPaused:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                    break;
                case RobotState.MusicStates.MusicIdled:
                    PlayPauseButtons.ViewModel.StateButton = PlayPauseButtonModel.ButtonState.Play;
                    //SetControlButtonState(false);
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
            viewModel.VolumeVisibility = state;
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
        private void ConnectMrobo(bool reconnect = false)
        {
            Cursor = Cursors.Wait;
            var helloRequest = new RemoteRequest(RobotPacket.PacketID.Hello);
            helloRequest.ProcessSuccessfully += (data) =>
            {

                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (!reconnect)
                    {
                        var title = (string)TryFindResource("ConnectToRobotSuccesfullyText");
                        WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Information,
                                           MessageBoxResult.OK);
                    }

                    UnconnectedTextBox.Visibility = Visibility.Hidden;
                    TransformButton.ViewModel.State = RobotTransformButtonModel.ButtonState.Transform;
                    GetListMotion();

                    GetState();
                });
                GlobalVariables.RoboOnline = true;
            };
            helloRequest.ProcessError += (errorCode, msg) =>
            {
                Debug.Fail(msg, Enum.GetName((typeof(RobotRequest.ErrorCode)), errorCode));

                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (!reconnect)
                    {
                        var titleError = (string)TryFindResource("ConnectToRobotErrorText");
                        var msgError = (string)TryFindResource("CheckDefaultErrorText");
                        WPFMessageBox.Show(StaticMainWindow.Window, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                           MessageBoxResult.OK);
                    }
                    UnconnectedTextBox.Visibility = Visibility.Visible;
                    Cursor = Cursors.Arrow;
                });
            };

            GlobalVariables.RobotWorker.AddJob(helloRequest);
        }

        private void GetState()
        {
            var stateRequest = new GetStateRequest();
            stateRequest.ProcessSuccessfully += data => Dispatcher.BeginInvoke((Action)delegate
            {
                UpdateRemoteControl();
                Cursor = Cursors.Arrow;
            });
            stateRequest.ProcessError += (errorCode, msg) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                    switch (errorCode)
                    {
                        case RobotRequest.ErrorCode.SetupConnection:
                        case RobotRequest.ErrorCode.WrongSessionID:
                            var titleError = (string) TryFindResource("RobotConnectionLostText");
                            var msgError = (string) TryFindResource("WantReconnectRobotText");
                            var result = WPFMessageBox.Show(StaticMainWindow.Window, msgError,
                                                     titleError, MessageBoxButton.YesNo,
                                                     MessageBoxImage.Question, MessageBoxResult.Yes);
                            if (result == MessageBoxResult.Yes)
                            {
                                ConnectMrobo();
                            }
                            else
                            {
                                GlobalVariables.RoboOnline = false;
                                UnconnectedTextBox.Visibility = Visibility.Visible;
                            }
                            break;
                    }
                });

                Debug.Fail(msg, Enum.GetName((typeof(RobotRequest.ErrorCode)), errorCode));
            };
            GlobalVariables.RobotWorker.AddJob(stateRequest);
        }

        private void GetListMotion()
        {
            var listAllMotionRequest = new ListAllMotionRobotRequest();
            viewModel.NoRobotMotionVisibility = false;
            if (GlobalVariables.CurrentListMotion.Count > 0)
            {
                viewModel.RemoteItemsList.Clear();
                GlobalVariables.CurrentListMotion.Clear();
            }

            listAllMotionRequest.ProcessSuccessfully += (listMotionInfo) => Dispatcher.BeginInvoke((Action)delegate
            {
                if (listMotionInfo.Count == 0) viewModel.NoRobotMotionVisibility = true;
                listMotionInfo.Sort((x, y) => string.Compare(x.Title, y.Title)); // Sort title
                foreach (MotionInfo info in listMotionInfo)
                {
                    if (info.MType != MotionInfo.MotionType.Dance) continue;
                    var motionTitleItem = new MotionTitleItem();
                    motionTitleItem.MotionID = info.MotionID;
                    motionTitleItem.ViewModel.Title = info.Title;
                    motionTitleItem.ViewModel.Duration = info.Duration;
                    motionTitleItem.DeleteMotion += MotionTitleItem_DeleteMotion;
                    viewModel.RemoteItemsList.Add(motionTitleItem);

                    GlobalVariables.CurrentListMotion.Add(info);
                }

                //Dispatcher.BeginInvoke((Action)delegate
                //                                    {
                //                                        Cursor = Cursors.Arrow;
                //                                    });
            });
            listAllMotionRequest.ProcessError += (e, msg) =>
            {
                //Dispatcher.BeginInvoke((Action)delegate
                //{
                //    Cursor = Cursors.Arrow;
                //});
                switch (e)
                {
                    case ListAllMotionRobotRequest.ErrorCode.SetupConnection:
                    case ListAllMotionRobotRequest.ErrorCode.WrongSessionID:
                        var result = WPFMessageBox.Show(StaticMainWindow.Window, "Your connection to robot is lost",
                                                 "Do you want to reconnect", MessageBoxButton.YesNo,
                                                 MessageBoxImage.Question, MessageBoxResult.Yes);
                        if (result == MessageBoxResult.Yes)
                        {
                            ConnectMrobo();
                        }
                        else
                        {
                            GlobalVariables.RoboOnline = false;
                            UnconnectedTextBox.Visibility = Visibility.Visible;
                        }
                        break;
                }
                Debug.Fail(msg, Enum.GetName((typeof(ListAllMotionRobotRequest.ErrorCode)), e));
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
            GlobalVariables.ImageDownloadWorker.ClearAll();
            if (Visibility == Visibility.Visible)
            {
                LoadLibrary();
                if (GlobalVariables.RoboOnline)
                {
                    UnconnectedTextBox.Visibility = Visibility.Hidden;
                    //GetState();
                    //GetListMotion();
                    ConnectMrobo(true);
                }
            }
        }


        private void LoadLibrary()
        {
            int index = 0;
            viewModel.NoLocalMotionVisibility = false;
            string[] listLocalFile = Directory.GetFiles(GlobalFunction.GetSavedDir(), "*.mrb");
            Array.Sort(listLocalFile);
            foreach (var file in listLocalFile)
            {
                var motionInfo = new MotionInfo(file);
                var motionItem = new MotionFullInfoItem();
                motionItem.SetMotionInfo(motionInfo);
                motionItem.ViewModel.HitTestVisible = false;
                motionItem.ViewModel.Index = ++index;
                motionItem.CopyMotion += Library_CopyMotion;
                motionItem.DelteMotion += Library_DelteMotion;
                viewModel.LibraryItemsList.Add(motionItem);
                DownloadImage(motionInfo.MotionID, motionItem.ViewModel);
            }
            if (index == 0) viewModel.NoLocalMotionVisibility = true;
        }

        private void Library_DelteMotion(object sender, RoutedEventArgs e)
        {
            viewModel.LibraryItemsList.Clear();
            LoadLibrary();
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
            motionInfoRequest.ProcessError += (data, msg) =>
                                                  {
                                                      if (data == null) Debug.Fail(msg);
                                                      else Debug.Fail(data.type.ToString(), msg);
                                                  };
            GlobalVariables.StoreWorker.ForceAddRequest(motionInfoRequest);
        }

        private void volumeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volumeRequest = new RemoteRequest(RobotPacket.PacketID.SetVolumeLevel, (int)viewModel.Volume);
            volumeRequest.ProcessSuccessfully += (reply) => Dispatcher.BeginInvoke((Action)UpdateRemoteControl);
            volumeRequest.ProcessError += (reply, msg) =>
                                              {
                                                  if (reply == null) Debug.Fail(msg);
                                                  Debug.Fail(reply.ToString(), msg);
                                              };
            GlobalVariables.StoreWorker.AddRequest(volumeRequest);
        }

        private void RemoteListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = RemoteListBox.SelectedIndex;
            var playID = GlobalVariables.CurrentListMotion[index].MotionID;
            Play(playID);
        }

        private void Play(ulong motionID)
        {
            Cursor = Cursors.Wait;
            var playRequest = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay, -1, motionID);
            playRequest.ProcessSuccessfully += (data) => Dispatcher.BeginInvoke((Action)GetState);
            playRequest.ProcessError += (errorCode, msg) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Cursor = Cursors.Arrow;
                    switch (errorCode)
                    {
                        case RobotRequest.ErrorCode.SetupConnection:
                        case RobotRequest.ErrorCode.WrongSessionID:
                            var result = WPFMessageBox.Show(StaticMainWindow.Window, "Your connection to robot is lost",
                                                     "Do you want to reconnect", MessageBoxButton.YesNo,
                                                     MessageBoxImage.Question, MessageBoxResult.Yes);
                            if (result == MessageBoxResult.Yes)
                            {
                                ConnectMrobo();
                            }
                            else
                            {
                                GlobalVariables.RoboOnline = false;
                                UnconnectedTextBox.Visibility = Visibility.Visible;
                            }
                            break;
                    }
                });
                Debug.Fail(msg);
            };
            GlobalVariables.RobotWorker.AddJob(playRequest);
        }
    }
}