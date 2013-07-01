using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                    RemoteListBox.SelectedIndex = GlobalVariables.CurrentListMotion.Count - 1;
                    return;
                }
                RemoteListBox.SelectedIndex--;
                //UpdateRemoteControl();
            });
        }

        private void NextButton_UpdateParentControl(object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (RemoteListBox.SelectedIndex == GlobalVariables.CurrentListMotion.Count - 1)
                {
                    RemoteListBox.SelectedIndex = 0;
                    return;
                }
                RemoteListBox.SelectedIndex++;
                //UpdateRemoteControl();
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
                    UpdateRemoteControl();
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
            viewModel.Volume = GlobalVariables.CurrentRobotState.Volume;
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
        private void ConnectMrobo()
        {
            var helloRequest = new RemoteRequest(RobotPacket.PacketID.Hello);
            helloRequest.ProcessSuccessfully += (data) =>
            {
               
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var title = (string)TryFindResource("ConnectToRobotSuccesfullyText");
                    WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Information,
                                       MessageBoxResult.OK);

                    UnconnectedTextBox.Visibility = Visibility.Hidden;
                    TransformButton.ViewModel.State = RobotTransformButtonModel.ButtonState.Transform;                    
                    GetListMotion();

                    UpdateRemoteControl();
                });
                GlobalVariables.RoboOnline = true;
            };
            helloRequest.ProcessError += (errorCode, msg) =>
            {
                Console.WriteLine("Cannot connect to robot:" + msg + errorCode);
                
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var titleError = (string)TryFindResource("ConnectToRobotErrorText");
                    var msgError = (string)TryFindResource("CheckDefaultErrorText");
                    WPFMessageBox.Show(StaticMainWindow.Window, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,
                                       MessageBoxResult.OK);
                    Cursor = Cursors.Arrow;
                });
            };

            GlobalVariables.RobotWorker.AddJob(helloRequest);
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
            int index = 0;
            viewModel.NoLocalMotionVisibility = false;
            foreach (var file in Directory.GetFiles(GlobalFunction.GetSavedDir(), "*.mrb"))
            {
                var motionInfo = new MotionInfo(file);
                var motionItem = new MotionFullInfoItem();
                motionItem.SetMotionInfo(motionInfo);
                motionItem.ViewModel.HitTestVisible = false;
                motionItem.ViewModel.Index = ++index;
                motionItem.CopyMotion+=Library_CopyMotion;
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
            var volumeRequest = new RemoteRequest(RobotPacket.PacketID.SetVolumeLevel, (int) viewModel.Volume);
            volumeRequest.ProcessSuccessfully += (reply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    UpdateRemoteControl();
                });
            };
            volumeRequest.ProcessError += (reply, msg) =>
                                              {
                                                  if (reply==null) Debug.Fail(msg);
                                                  Debug.Fail(reply.ToString(),msg);
                                              };
            GlobalVariables.StoreWorker.AddRequest(volumeRequest);
        }
    }
}