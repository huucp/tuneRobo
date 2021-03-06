﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBoxUtils;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using motion;
using MotionInfo = TuneRoboWPF.Utility.MotionInfo;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MotionFullInfoItem.xaml
    /// </summary>
    public partial class MotionFullInfoItem : UserControl
    {
        public delegate void MotionClickEventHandler(ulong motionID);

        public event MotionClickEventHandler MotionClicked;

        public void OnMotionClick(ulong motionID)
        {
            MotionClickEventHandler handler = MotionClicked;
            if (handler != null) handler(motionID);
        }
        public MotionFullInfoItem()
        {
            this.InitializeComponent();

            DataContext = new MotionFullInfoItemViewModel();
            ViewModel = (MotionFullInfoItemViewModel)DataContext;

            ViewModel.MotionClick = new ViewModelBase.CommandHandler(MotionClickHandler, true);
        }
        public MotionFullInfoItemViewModel ViewModel = new MotionFullInfoItemViewModel();
        private ulong MotionID { get; set; }

        public void SetMotionInfo(Utility.MotionInfo info)
        {
            ViewModel.MotionTitle = info.Title;
            ViewModel.ArtistName = info.Artist;
            TimeSpan t = TimeSpan.FromSeconds(info.Duration);
            if (info.Duration > -1)
            {
                ViewModel.MotionDuration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            }
            ViewModel.RatingValue = 0.6;
            MotionID = info.MotionID;
        }
        private void MotionClickHandler()
        {
            OnMotionClick(MotionID);
        }

        public void SetMotionInfo(MotionShortInfo info)
        {
            ViewModel.MotionTitle = info.title;
            ViewModel.ArtistName = info.artist_name;
            ViewModel.RatingValue = info.rating / GlobalVariables.RateValueMultiplierFactor;
            MotionID = info.motion_id;
        }



        public void SetRatingVisible(bool visible = false)
        {
            RatingControl.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        // Create a custom routed event by first registering a RoutedEventID 
        // This event uses the bubbling routing strategy 
        public static readonly RoutedEvent CopyMotionEvent = EventManager.RegisterRoutedEvent(
            "CopyMotion", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MotionTitleItem));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler CopyMotion
        {
            add { AddHandler(CopyMotionEvent, value); }
            remove { RemoveHandler(CopyMotionEvent, value); }
        }



        // This method raises the CopyMotion event

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVariables.RoboOnline)
            {
                var title = string.Format("{0}!", TryFindResource("ConnectToRobotText"));
                WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Warning,
                                   MessageBoxResult.OK);
            }
            else
            {
                if (CheckExistInRobot(GlobalVariables.CurrentListMotion, MotionID))
                {
                    var titleRecopy = (string)TryFindResource("ExistInRobotText");
                    var msgRecopy = (string)TryFindResource("WantRecopyMotionText");
                    var recopyResult = WPFMessageBox.Show(StaticMainWindow.Window, msgRecopy, titleRecopy,
                                                          MessageBoxButton.YesNo, MessageBoxImage.Question,
                                                          MessageBoxResult.No);
                    if (recopyResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                
                if (GlobalVariables.CurrentRobotState.MusicState == RobotState.MusicStates.MusicPlaying)
                {
                    var titleStop = (string)TryFindResource("StopDanceToCopyText");
                    var msgStop = (string)TryFindResource("WantToStopDanceText");
                    var result = WPFMessageBox.Show(StaticMainWindow.Window, msgStop, titleStop, MessageBoxButton.YesNo,
                                                MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {

                        var musicStopRequest = new RemoteRequest(RobotPacket.PacketID.Stop);
                        musicStopRequest.ProcessSuccessfully += (data) =>
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                var transferRequest = new TransferMotionToRobot(MotionID);
                                var transferWindow = new Windows.TransferWindow(transferRequest, MotionID.ToString());
                                if (transferWindow.ShowDialog(StaticMainWindow.Window) == true)
                                {
                                    var newEventArgs = new RoutedEventArgs(CopyMotionEvent);
                                    RaiseEvent(newEventArgs);
                                }
                            });
                        };
                        musicStopRequest.ProcessError += (data, msg) => Debug.Fail(msg);
                        GlobalVariables.RobotWorker.AddJob(musicStopRequest);

                    }
                }
                else
                {
                    var transferRequest = new TransferMotionToRobot(MotionID);
                    var transferWindow = new Windows.TransferWindow(transferRequest, MotionID.ToString());
                    if (transferWindow.ShowDialog(StaticMainWindow.Window) == true)
                    {
                        var newEventArgs = new RoutedEventArgs(CopyMotionEvent);
                        RaiseEvent(newEventArgs);
                    }

                }
            }
        }

        private bool CheckExistInRobot(List<MotionInfo> list, ulong motionID)
        {
            return list.Any(motionInfo => motionID == motionInfo.MotionID);
        }

        public static readonly RoutedEvent DeleteMotionEvent = EventManager.RegisterRoutedEvent(
           "DeleteMotion", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MotionTitleItem));

        public event RoutedEventHandler DelteMotion
        {
            add { AddHandler(DeleteMotionEvent, value); }
            remove { RemoveHandler(DeleteMotionEvent, value); }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteLocalMotion())
            {
                var newEventArgs = new RoutedEventArgs(DeleteMotionEvent);
                RaiseEvent(newEventArgs);
            }
        }

        private bool DeleteLocalMotion()
        {
            var title = (string)TryFindResource("WantDeleteMotionText") + " " + ViewModel.MotionTitle + "?";
            var result = WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.YesNo,
                                            MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result == MessageBoxResult.No) return false;
            var motionPath = GlobalFunction.GetLocalMotionPath(MotionID);
            var musicPath = GlobalFunction.GetLocalMusicPath(MotionID);
            try
            {
                File.Delete(motionPath);
                File.Delete(musicPath);
                return true;
            }
            catch (IOException)
            {
                Debug.Fail("Cannot delete");
                return false;
            }
        }
    }
}