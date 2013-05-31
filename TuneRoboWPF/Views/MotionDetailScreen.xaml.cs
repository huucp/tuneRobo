﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;
using TuneRoboWPF.StoreService.BigRequest;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MotionDetailScreen.xaml
    /// </summary>
    public partial class MotionDetailScreen : UserControl
    {
        private DockPanel MainWindowDockPanel { get; set; }
        private MotionDetailScreenViewModel ViewModel { get; set; }
        private ulong MotionID { get; set; }
        public MotionDetailScreen(DockPanel dockPanel, ulong motionID)
        {
            InitializeComponent();

            MainWindowDockPanel = dockPanel;
            MotionID = motionID;
            DataContext = new MotionDetailScreenViewModel();
            ViewModel = (MotionDetailScreenViewModel)DataContext;

            if (!IsOnLocal())
            {
                ViewModel.DownloadButtonContent = "Free";
            }
            else
            {
                ViewModel.DownloadButtonContent = "Installed";
                DownloadButton.IsEnabled = false;
            }
            GetMotionInfo(motionID);
        }

        private void GetMotionInfo(ulong motionID)
        {
            var infoRequest = new GetMotionFullInfoStoreRequest(motionID);
            infoRequest.ProcessSuccessfully += (reply) => UpdateContent(reply.motion_info.info);
            infoRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.AddJob(infoRequest);
        }

        private void UpdateContent(motion.MotionInfo info)
        {
            if (Dispatcher.CheckAccess())
            {
                UpdateContentValue(info);
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() => UpdateContentValue(info)));
            }
            UpdateArtwork(info.icon_url);
            UpdateScreenshots(info.screenshoot_ulrs);
        }

        private void UpdateContentValue(motion.MotionInfo info)
        {
            ViewModel.RatingValue = info.rating / 10.0;
            ViewModel.ArtistName = info.artist_name;
        }

        private void UpdateArtwork(string url)
        {
            var downloadImageRequest = new ImageDownload(url);
            downloadImageRequest.DownloadCompleted += (image) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.CoverImage = image;
                });
            };

            GlobalVariables.ImageDownloadWorker.AddDownload(downloadImageRequest);
        }

        private void UpdateScreenshots(List<string> urls)
        {
            foreach (string t in urls)
            {
                var downloadImageRequest = new ImageDownload(t);
                downloadImageRequest.DownloadCompleted += (imageSource) =>
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var image = new MotionDetailScreenModel.ScreenshotImage { ImageSource = imageSource };
                            ViewModel.ScreenshotsList.Add(image);

                        });

                GlobalVariables.ImageDownloadWorker.AddDownload(downloadImageRequest);
            }
        }

        private bool IsOnLocal()
        {
            return (MotionFileOnLocal() && MusicFileOnLocal());
        }

        private bool MotionFileOnLocal()
        {
            string motionPath =
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             MotionID.ToString() + ".mrb");
            return File.Exists(motionPath);
        }
        private bool MusicFileOnLocal()
        {
            string musicPath =
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             MotionID.ToString() + ".mp3");
            return File.Exists(musicPath);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVariables.USER_ONLINE)
            {
                var signinWindow = new LoginWindow();
                if (signinWindow.ShowDialog() == false) return;
            }
            var request = new DownloadMotionStoreRequest(MotionID);
            var transferWindow = new TransferWindow(request, MotionID.ToString());
            if (transferWindow.ShowDialog() == true)
            {
                ViewModel.DownloadButtonContent = "Installed";
                DownloadButton.IsEnabled = false;
            }
        }
    }
}
