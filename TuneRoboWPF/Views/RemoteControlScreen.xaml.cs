using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        private DockPanel MainWindowDock { get; set; }
        public RemoteControlScreen(DockPanel dock)
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
            var remoteViewModel = new RemoteControlScreenViewModel();
            DataContext = remoteViewModel;
            viewModel = (RemoteControlScreenViewModel)DataContext;
            MainWindowDock = dock;

            PlayPauseButtons.UpdateParentControl += PlayPauseButtonsUpdateParentControl;
            NextButton.UpdateParentControl += NextButton_UpdateParentControl;
            PreviousButton.UpdateParentControl += PreviousButton_UpdateParentControl;
            TransformButton.UpdateParentControl += TransformButton_UpdateParentControl;
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
            //RemoteListBox.SelectedIndex = GlobalVariables.CurrentRobotState.MotionIndex;
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
            if (viewModel.LastRemoteSelectedMotion != null)
            {
                viewModel.LastRemoteSelectedMotion.ViewModel.RectangleFillColor = "Yellow";
            }
            viewModel.RemoteSelectedMotion.ViewModel.RectangleFillColor = "Red";
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
                GlobalVariables.WIRELESS_CONNECTION = true;
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

        private void GetListMotion()
        {
            var listAllMotionRequest = new ListAllMotionRequest();
            if (viewModel.RemoteItemsList.Count > 0) viewModel.RemoteItemsList.Clear();
            listAllMotionRequest.ProcessSuccessfully += (listMotionInfo) => Dispatcher.BeginInvoke((Action)delegate
            {
                var listMotion = new ObservableCollection<MotionTitleItem>();
                foreach (MotionInfo info in listMotionInfo)
                {
                    if (info.MType != MotionInfo.MotionType.Dance) continue;
                    var motionTitleItem = new MotionTitleItem();
                    motionTitleItem.ViewModel.Title = info.Title;
                    listMotion.Add(motionTitleItem);
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
            GlobalVariables.StoreWorker.IsClearWorker = false;
            int index = 0;
            foreach (var file in Directory.GetFiles(GlobalFunction.GetSavedDir(), "*.mrb"))
            {
                var motionInfo = new MotionInfo(file);
                var motionItem = new MotionFullInfoItem();
                motionItem.SetMotionInfo(motionInfo);
                motionItem.ViewModel.RatingValue = r.Next(1, 5) / 5.0;
                motionItem.ViewModel.HitTestVisible = false;
                motionItem.ViewModel.Index = ++index;
                viewModel.LibraryItemsList.Add(motionItem);
                DownloadImage(motionInfo.MotionID, motionItem.ViewModel);
            }
            GlobalVariables.StoreWorker.IsClearWorker = true;
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
                                                      Console.WriteLine("get info failed: {0}", msg);
                                                  };

            GlobalVariables.StoreWorker.AddJob(motionInfoRequest);
        }
        

        private void LibraryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel.LastLibrarySlectedMotion != null)
            {
                viewModel.LastLibrarySlectedMotion.ViewModel.Index =  LibraryListBox.Items.IndexOf(viewModel.LastLibrarySlectedMotion);
            }
            viewModel.LibrarySelectedMotion.ViewModel.Index = LibraryListBox.SelectedIndex;
        }
    }
}